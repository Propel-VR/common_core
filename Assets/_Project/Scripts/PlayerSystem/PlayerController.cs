using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SpatialTracking;

public enum PlayerControlType
{
    ContinuousAndTeleport,
    FirstPerson
}

public enum PlayerTurnType
{
    Snap,
    Continuous
}

[DefaultExecutionOrder(-29999)]
public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] PlayerControlType controlType;                     // How the controller will behave
    [SerializeField] PlayerTurnType turnType;                           // How the turning will behave
    [SerializeField] bool enableSnapTurns = true;                       // Is a side swipe on the joystick going to turn the camera
    [SerializeField] float turnDegree = 45f;                            // How much degree to turn each snap turn
    [SerializeField, Range(0f, 1f)] float turnDeadZone = 0.8f;          // How much the joystick needs to be incline to register the turn
    [SerializeField] float speed = 1f;                                  // Max speed (in m/s)
    [SerializeField, Range(0f, 1f)] float deadZone = 0.2f;              // How much the joystick needs to be inclined to start walking
    [SerializeField] float maxClippingDistance = 1f;                    // How much can you clip in a wall before getting unclipped back
    [SerializeField] float clipSmoothSpeed = 2f;                        // How fast the camera corrections (clipping, going up a slope) occure (in m/s)
    [SerializeField] float cameraFadeRadius = 0.1f;                     // The camera will begin to fade after clipping a certain radius.
    [SerializeField, Range(0f, 1f)] float minCameraFade = 0.5f;         // At what percentage of the radius will the clip-fade be fully applied
    [SerializeField] float fadeBeforeTeleportTime = 0.1f;               // How much time the fade before and after a teleportation takes
    [SerializeField, Range(0f, 1f)] float minCameraTeleportFade = 0.8f; // What percentage of fade 100% will be remapped to. 0.75 means the opacity will reach 100% after 75% of the fade
    [SerializeField] LayerMask colliderLayers;
    [SerializeField] QueryTriggerInteraction triggerInteraction;
    [SerializeField] string targetFloorTag;

    [Header("Camera Reference")]
    [SerializeField] new Camera camera;
    [SerializeField] Camera cameraHighlight;
    [SerializeField] Transform cameraTr;
    [SerializeField] Transform cameraOffset;
    [SerializeField] SphereCollider cameraCollider;
    [SerializeField] EyeOccluderSystem eyeOccluder;
    [SerializeField] FPSCamera fpsCamera;
    [SerializeField] TrackedPoseDriver headTracker;

    [Header("Body Reference")]
    [SerializeField] Transform bodyAnchor;
    [SerializeField] CharacterController character;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] RayTeleporter rayTeleporter;
    [SerializeField] XROrigin xrOrigin;

    [Header("Other References")]
    [SerializeField] Image crosshair;


    [Header("Events")]
    [SerializeField] UnityEvent OnAfterRigUpdate;
    [SerializeField] UnityEvent OnTeleport;


    private bool isGrounded;
    private Vector3 anchorOffset;
    private Vector3 anchorYOffset;
    private float yVel = 0f;
    private Transform tr;
    private Collider[] colls;
    private float fadeValue2 = 0f;
    private bool doFadeValue2;
    private Vector2 lastInput;
    private float lastInputMag;
    private bool hasRotated;
    private int turnCache = 0;
    private int targetFloorHash = -1;
    private int xrOriginFix;
    private Vector3 startLookAt;
    private Transform spawnAnchor;



    #region Unity Callbacks
    private void Awake ()
    {
        colls = new Collider[64];

        // Save variables
        capsuleCollider.radius = character.radius + character.skinWidth;
        tr = character.transform;
        cameraTr.parent = cameraOffset;
        cameraTr.localPosition = Vector3.up * capsuleCollider.height;
        if(!string.IsNullOrEmpty(targetFloorTag)) targetFloorHash = targetFloorTag.GetHashCode();

        // Change controls first
        SetControlType(controlType);

        Transform initPosition = spawnAnchor;
        if (initPosition == null) initPosition = transform;
        eyeOccluder.OnUpdate(1f);


        // Reset camera to avoid slide-in at start
        TeleportTo(initPosition.position, false);
        startLookAt = initPosition.position + Quaternion.Euler(0f, initPosition.eulerAngles.y, 0f) * Vector3.forward * 100;
        transform.eulerAngles = Vector3.zero;
        xrOrigin.transform.eulerAngles = Vector3.zero;
    }

    private void OnEnable () => Application.onBeforeRender += OnCameraUpdate;
    private void OnDisable () => Application.onBeforeRender -= OnCameraUpdate;

    private void Update ()
    {
        if(controlType == PlayerControlType.FirstPerson)
            bodyAnchor.localPosition = Vector3.zero;

        // Toggling the XR origin on and off fixes some unity bug that causes jitter with the camera
        if (xrOriginFix == 1) xrOrigin.enabled = true;
        if (xrOriginFix == 2) xrOrigin.enabled = false;

        // Fixing a bug where you can't align your camera too early
        if (xrOriginFix < 50)
        {
            fadeValue2 = 1f;
            RotateTowards(startLookAt);
        }

        // Handle translation and rotation
        HandleMovementAndInputs();
        UpdateAnchor();

        // Apply gravity
        if (isGrounded) yVel = 0f;
        else yVel += Time.deltaTime * 9.81f;
        character.Move(Vector3.down * yVel * Time.deltaTime);

        xrOriginFix++;
    }

    private void FixedUpdate ()
    {
        //UpdateAnchor();
    }
    #endregion


    #region Public Methods
    /// <summary>
    /// Sets the current control type for the character controller.
    /// - Continuous offers slow and smooth movement movement in VR
    /// - Teleport allows the teleport ray to be used to move around in VR
    /// - FirstPerson allows for continuous movement without a VR headset
    /// </summary>
    public void SetSnapTurn (bool enableSnapTurns)
    {
        this.enableSnapTurns = enableSnapTurns;
    }
    public PlayerControlType GetControlType () => this.controlType;
    public void SetControlType (PlayerControlType controlType)
    {
        this.controlType = controlType;

        switch (controlType)
        {
            case PlayerControlType.ContinuousAndTeleport:
                crosshair.enabled = false;
                capsuleCollider.enabled = true;
                rayTeleporter.enabled = true;
                xrOrigin.enabled = true;
                fpsCamera.enabled = false;
                headTracker.enabled = true;
                break;
            case PlayerControlType.FirstPerson:
                crosshair.enabled = true;
                capsuleCollider.enabled = false;
                rayTeleporter.enabled = false;
                xrOrigin.enabled = false;
                fpsCamera.enabled = true;
                headTracker.enabled = false;
                break;
        }
    }

    public void SetTurnType (PlayerTurnType turnType) => this.turnType = turnType;

    /// <summary>
    /// Teleport the character to a new location. Use this over transform.position = point to avoid issues.
    /// </summary>
    /// <param name="point">The bottom point of the character controller.</param>
    /// <param name="doFade">Wheter or not to fade in and out before and after teleportation.</param>
    public void TeleportTo (Vector3 point, bool doFade = true, System.Action postTeleport = null)
    {
        if (doFade)
        {
            StartCoroutine(TeleportWithFade(point, postTeleport));
        }
        else
        {
            TeleportWithoutFade(point, postTeleport);
        }
    }

    public void RotateTowards (Vector3 target)
    {
        Vector2 from = new Vector2(cameraTr.forward.x, cameraTr.forward.z).normalized;
        Vector2 to = (new Vector2(target.x, target.z) - new Vector2(cameraTr.position.x, cameraTr.position.z)).normalized;
        float angleDiff = -Vector2.SignedAngle(from, to);

        bodyAnchor.position -= anchorYOffset;
        bodyAnchor.RotateAround(new Vector3(cameraTr.position.x, 0f, cameraTr.position.z), Vector3.up, angleDiff);
        anchorOffset = bodyAnchor.position - tr.position;
        bodyAnchor.position = tr.position + anchorOffset + anchorYOffset;
    }

    public void SetSpawnAnchor (Transform spawnAnchor)
    {
        this.spawnAnchor = spawnAnchor;
    }
    #endregion


    #region Private Methods
    private void HandleMovementAndInputs ()
    {
        // Gather inputs
        Vector2 input = new Vector2(Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal"), 
                                    Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical"));
        Vector2 inputSec = new Vector2(Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal"), 
                                       Input.GetAxis("XRI_Right_Primary2DAxis_Vertical"));
        float inputMag = input.magnitude;
        input /= Mathf.Max(inputMag, 1f);
        inputMag = Mathf.Min(inputMag, 1f);
        float inputSecMag = inputSec.magnitude;
        inputSec /= Mathf.Max(inputSecMag, 1f);
        inputSecMag = Mathf.Min(inputSecMag, 1f);

        // Translation
        if (controlType == PlayerControlType.ContinuousAndTeleport)
        {
            // If second joystick is in rest position, reset turn flag
            if (inputSecMag < deadZone) hasRotated = false;

            // Translation
            if (inputMag > deadZone)
            {
                Vector3 positionDelta = Quaternion.Euler(0f, cameraTr.eulerAngles.y, 0f) * (speed*Time.deltaTime*new Vector3(input.x, 0f, input.y));
                character.Move(positionDelta);
            }

            // Rotation
            if(turnType == PlayerTurnType.Continuous)
            {
                if (inputSecMag > turnDeadZone)
                {
                    hasRotated = true;
                    turnCache += (int)Mathf.Sign(inputSec.x);
                }
            }
            else
            {
                if (inputSecMag > turnDeadZone && !hasRotated)
                {
                    hasRotated = true;
                    turnCache += (int)Mathf.Sign(inputSec.x);
                }
            }
        }
        else if (controlType == PlayerControlType.FirstPerson)
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"),
                                Input.GetAxisRaw("Vertical"));

            Vector3 positionDelta = Quaternion.Euler(0f, fpsCamera.transform.eulerAngles.y, 0f) * (speed*Time.deltaTime*new Vector3(input.x, 0f, input.y));
            character.Move(positionDelta);
            isGrounded = Physics.SphereCast(
                tr.position + Vector3.up * character.radius,
                character.radius, Vector3.down, out RaycastHit floorInfo, 0.01f, colliderLayers.value, triggerInteraction);
        }

        lastInput = input;
        lastInputMag = inputMag;
    }

    private void UpdateAnchor ()
    {
        if (controlType == PlayerControlType.FirstPerson) return;

        Vector3 offsetXZ = (cameraTr.position - anchorYOffset) - tr.position;
        offsetXZ.y = 0f;
        Vector3 offsetDir = offsetXZ.normalized;
        float offsetDist = offsetXZ.magnitude;

        // HANDLE ROTATIONS STORED PREVIOUSLY
        if(turnCache != 0)
        {
            bool onTeleport = false;
            bodyAnchor.position -= anchorYOffset;
            if (turnType == PlayerTurnType.Continuous)
            {
                bodyAnchor.RotateAround(new Vector3(cameraTr.position.x, 0f, cameraTr.position.z), Vector3.up, turnCache * Time.deltaTime * 45f);
            }
            else
            {
                bodyAnchor.RotateAround(new Vector3(cameraTr.position.x, 0f, cameraTr.position.z), Vector3.up, turnCache * turnDegree);
                onTeleport = true;
            }
            anchorOffset = bodyAnchor.position - tr.position;
            bodyAnchor.position = tr.position + anchorOffset + anchorYOffset;
            turnCache = 0;
            if(onTeleport)
            {
                OnTeleport?.Invoke();
            }
        }

        // HANDLE MOVING AROUND SLOWLY WITHOUT CLIPPING IN COLLISION
        else if (offsetDist < maxClippingDistance)
        {
            Vector3 p1 = tr.position + character.center + tr.up * (character.height * 0.5f - character.radius);
            Vector3 p2 = tr.position + character.center;
            Vector3 center = tr.position + character.center;
            Vector3 targetCenter = center + offsetXZ;

            Vector3 newOffset = offsetXZ;
            if (Physics.CapsuleCast(p1, p2, character.radius, offsetDir, out RaycastHit hitInfo, offsetDist, colliderLayers.value, triggerInteraction))
            {
                Physics.ComputePenetration(
                    capsuleCollider, tr.position, tr.rotation, hitInfo.collider,
                    hitInfo.collider.transform.position, hitInfo.transform.rotation, out Vector3 penDir, out float penDist);

                newOffset += new Vector3(penDir.x, 0, penDir.z) * penDist;
                if (Physics.CapsuleCast(p1, p2, character.radius, newOffset.normalized, out hitInfo, newOffset.magnitude, colliderLayers.value, triggerInteraction))
                {
                    newOffset = Vector3.zero;
                }
            }
            else
            {
                isGrounded = false;
                if (Physics.SphereCast(targetCenter, character.radius, Vector3.down, out RaycastHit floorInfo, character.height * 0.5f, colliderLayers.value, triggerInteraction))
                {
                    isGrounded = true;
                    if(targetFloorHash == -1 || floorInfo.collider.tag.GetHashCode() == targetFloorHash)
                    {
                        float floorHeight = floorInfo.point.y + character.skinWidth;
                        anchorYOffset += Vector3.up * (tr.position.y - floorHeight);
                        tr.position = new Vector3(tr.position.x, floorHeight, tr.position.z);
                    }
                    else
                    {
                        newOffset = Vector3.zero;
                    }                    
                }
            }

            character.enabled = false;
            tr.position += newOffset;
            character.enabled = true;
            anchorOffset -= newOffset;
            bodyAnchor.position = tr.position + anchorOffset + anchorYOffset;
        }

        // HANDLE CLIPPIN IN WALLS
        else
        {
            SnapCameraToCollider();
        }
    }

    // HANDLE CAMERA STUFF (Check if obstructed)
    [BeforeRenderOrder(-29999)]
    private void OnCameraUpdate ()
    {
        anchorYOffset = Vector3.MoveTowards(anchorYOffset, Vector3.zero, clipSmoothSpeed * Time.deltaTime);
        bodyAnchor.position = tr.position + anchorOffset + anchorYOffset;

        cameraCollider.radius = cameraFadeRadius;
        bool hasColl = Physics.Linecast(new Vector3(tr.position.x, cameraTr.position.y, tr.position.z), cameraTr.position, out RaycastHit hit, colliderLayers.value, triggerInteraction);
        float closestDist = cameraFadeRadius;
        if (hasColl)
        {
            closestDist = 0f;
            /*if (Physics.ComputePenetration(
                cameraCollider, cameraTr.position, cameraTr.rotation,
                hit.collider, hit.collider.transform.position, hit.collider.transform.rotation, out Vector3 dir, out float dist))
            {
                closestDist = Mathf.Min(closestDist, cameraFadeRadius - dist);
            }*/
        }
        int collCount = Physics.OverlapSphereNonAlloc(cameraTr.position, cameraFadeRadius, colls, colliderLayers.value, triggerInteraction);
        //float closestDist = cameraFadeRadius;
        for (int i = 0; i < collCount; i++)
        {
            if(Physics.ComputePenetration(
                cameraCollider, cameraTr.position, cameraTr.rotation,
                colls[i], colls[i].transform.position, colls[i].transform.rotation, out Vector3 dir, out float dist))
            {
                closestDist = Mathf.Min(closestDist, cameraFadeRadius - dist);
            }
            
        }

        eyeOccluder.OnUpdate(alpha: Mathf.Max(
            1f - Mathf.InverseLerp(minCameraFade, 1f, (closestDist / cameraFadeRadius)), 
            Mathf.InverseLerp(0f, minCameraTeleportFade, fadeValue2)));
        fadeValue2 = Mathf.Clamp01(fadeValue2 + (Time.deltaTime / fadeBeforeTeleportTime) * (doFadeValue2 ? 1 : -1));

        OnAfterRigUpdate.Invoke();
    }

    private void SnapCameraToCollider ()
    {
        Vector3 offsetXZ = (cameraTr.position - anchorYOffset) - tr.position;
        offsetXZ.y = 0f;
        anchorOffset -= offsetXZ;
        anchorYOffset += offsetXZ;
        //anchorYOffset = Vector3.zero;
        bodyAnchor.position = tr.position + anchorOffset + anchorYOffset;
    }

    private IEnumerator TeleportWithFade (Vector3 point, System.Action postTeleport = null)
    {
        doFadeValue2 = true;
        yield return new WaitUntil(()=>fadeValue2 == 1);

        TeleportWithoutFade(point, postTeleport);

        // Start fading back out. No need to wait
        doFadeValue2 = false;
    }

    private void TeleportWithoutFade (Vector3 point, System.Action postTeleport = null)
    {
        // Teleport and reset camera offset
        character.enabled = false;
        if(Physics.SphereCast(point + Vector3.up * (0.5f + capsuleCollider.radius), capsuleCollider.radius, Vector3.down, out RaycastHit hitInfo))
        {
            tr.position = hitInfo.point;
        }
        else
        {
            tr.position = point;
        }
        
        Vector3 offsetXZ = (cameraTr.position - anchorYOffset) - tr.position;
        offsetXZ.y = 0f;
        anchorOffset += offsetXZ;
        anchorYOffset = Vector3.zero;
        bodyAnchor.position = tr.position + anchorOffset + anchorYOffset;
        character.enabled = true;
        OnTeleport?.Invoke();
        postTeleport?.Invoke();
    }

    public Vector3 GetPosition ()
    {
        return character.transform.position;
    }
    #endregion
}

