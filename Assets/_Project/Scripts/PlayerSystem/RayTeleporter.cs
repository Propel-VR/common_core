using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTeleporter : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] float deadzone = 0.1f;

    [Header("Projectile Parameters")]
    [SerializeField] bool autoUpdate = true;
    [SerializeField] bool raycastWhenGoingUp = false; // The player is usually not going to teleport to the ceiling. Avoid doing wasteful raycasts
    [SerializeField] float throwSpeed = 5f; // How fast the fake projectile is being throw. Used in the kinematic equation
    [SerializeField] float gravity = -9.8f; // How much the fake projectile will accelerate downwards
    [SerializeField] int pointFrequency = 20; // How many points per seconds are tested in the projectile's path
    [SerializeField] LayerMask colliderLayers;
    [SerializeField] QueryTriggerInteraction triggerInteraction;
    [SerializeField] string targetFloorTag;

    [Header("Landing Parameters")]
    [SerializeField] float maxAngle = 45f; // What's the maximum angle for a slow before getting considered too steep to be landed on.
    [SerializeField] float minVerticalHeight = 0f; // Any projectile that get lower than this point are too low to hit anything
    [SerializeField] float scrollSpeed = -0.5f;
    [SerializeField] Gradient safeLandingGradient;
    [SerializeField] Gradient unsafeLandingGradient;

    [Header("Reference")]
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform origin;
    [SerializeField] CapsuleCollider playerCollider;
    [SerializeField] PlayerController player;
    [SerializeField] Transform teleportPad;

    private bool wasTeleporting;
    private int targetFloorHash = -1;
    Vector3[] points;

    private void Awake ()
    {
        points = new Vector3[pointFrequency * 10];

        if (!string.IsNullOrEmpty(targetFloorTag))
        {
            targetFloorHash = targetFloorTag.GetHashCode();
        }
    }

    private void OnEnable ()
    {
        lineRenderer.enabled = true;
    }
    private void OnDisable ()
    {
        lineRenderer.enabled = false;
    }

    private void Update ()
    {
        if (autoUpdate) OnUpdate();

        if(lineRenderer.isVisible)
        {
            lineRenderer.material.mainTextureOffset = new Vector4(Time.time * scrollSpeed, 0f);
        }
    }

    public void OnUpdate ()
    {
        if (!gameObject.activeInHierarchy || !enabled)
        {
            teleportPad.gameObject.SetActive(false);
            return;
        }

        // Manage inputs
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("XRI_Left_Trigger"));
        float inputMag = input.magnitude;
        input /= Mathf.Min(inputMag, 1f);
        inputMag = Mathf.Min(inputMag, 1f);
        bool isTeleporting = Mathf.Abs(input.x) < Mathf.Abs(input.y) && input.y > 0f;
        isTeleporting = isTeleporting && inputMag > deadzone;
        bool releasedTeleporting = wasTeleporting && !isTeleporting;
        wasTeleporting = isTeleporting;

        // Ensure the ray is still getting fired one frame after release
        isTeleporting = isTeleporting || releasedTeleporting;


        lineRenderer.enabled = isTeleporting;
        teleportPad.gameObject.SetActive(isTeleporting);

        if (!isTeleporting) return;



        // Prepare variable for the fake projectile
        gravity = Mathf.Min(-1f, gravity);
        pointFrequency = Mathf.Max(pointFrequency, 1);

        Vector3 initVel = origin.forward * throwSpeed;
        Vector3 initPoint = origin.position;
        Vector3 initDir = origin.forward;

        Vector3 point = initPoint;
        Vector3 dir = initDir;
        float pointInterv = 1f/pointFrequency;
        float time = 0f;
        int pointCount = 0;

        bool isLandingSafe = false;
        Vector3 hitNormal = Vector3.up;
        Collider hitCollider = null;

        points[0] = point;
        pointCount++;


        // Check segment per segment before the min height is reached.
        while(point.y > minVerticalHeight)
        {
            // If the maximum point count is reach, cancel now
            if(pointCount >= points.Length) break;

            // Raycast from the current point to the next
            Vector3 nextPoint = GetPointAtTime(initPoint, initVel, time);
            dir = (nextPoint - point).normalized;
            bool canRaycast = Vector3.Dot(dir, Vector3.up) > 0f || raycastWhenGoingUp;
            if (canRaycast && Physics.Raycast(point, dir, out RaycastHit hitInfo, Vector3.Distance(point, nextPoint), colliderLayers.value, triggerInteraction))
            {
                // We have hit something
                isLandingSafe = true;
                point = hitInfo.point;
                hitNormal = hitInfo.normal;
                hitCollider = hitInfo.collider;
            } 
            else
            {
                // We have hit nothing, continue onto the next point
                point = nextPoint;
            }
            points[pointCount] = point;
            
            // Increment time and point count
            pointCount++;
            time += pointInterv;
            if (isLandingSafe) break;
        }

        // The final point for the line renderer should be slightly out of the terrain
        // If that breaks the line renderer, remove the n-1 th point
        if(pointCount >= 2 && isLandingSafe) {
            points[pointCount - 1] = point - dir * 0.05f;

            // The line is broken because the two last segment are oposite
            if(Vector3.Dot(dir, (points[pointCount - 1] - points[pointCount - 2]).normalized) < 0)
            {
                points[pointCount - 2] = points[pointCount - 1];
                pointCount--;
            }
        }

        // Check if region is safe to be landed in
        float hitAngle = Mathf.Asin(Vector3.Dot(hitNormal, Vector3.up)) * Mathf.Rad2Deg;
        if(hitAngle < maxAngle)
        {
            // Cancel if the landing angle is bad
            isLandingSafe = false;
        }
        if(isLandingSafe && Physics.Linecast(player.transform.position, initPoint, colliderLayers.value, triggerInteraction))
        {
            // Cancel if the arm is not linked to the body (trying to put your hands throught the wall to teleport throught it
            isLandingSafe = false;
        }
        Vector3 startCheckPos = point + hitNormal * playerCollider.radius + Vector3.up * 0.1f;
        if (isLandingSafe && Physics.CheckCapsule(
            start: startCheckPos,
            end: startCheckPos + Vector3.up * (playerCollider.height - playerCollider.radius * 2),
            playerCollider.radius, colliderLayers.value, triggerInteraction))
        {
            // Cancel if the landing spot is obstructed
            isLandingSafe = false;
        }
        if(isLandingSafe)
        {
            if(targetFloorHash != -1 && hitCollider.tag.GetHashCode() != targetFloorHash)
            {
                isLandingSafe = false;
            }
        }

        // Set positions in the line renderer
        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(points);

        lineRenderer.colorGradient = isLandingSafe ? safeLandingGradient : unsafeLandingGradient;
        teleportPad.gameObject.SetActive(isLandingSafe);
        teleportPad.position = point;
        teleportPad.up = hitNormal;

        // Teleport once the joystick gets released
        if(releasedTeleporting && isLandingSafe)
        {
            player.TeleportTo(point, true);
        }
    }

    // Kinematic equation for projectile
    private Vector3 GetPointAtTime (Vector3 initPos, Vector3 initSpeed, float time)
    {
        return initPos + new Vector3(
            initSpeed.x * time,
            initSpeed.y * time + 0.5f * gravity * (time * time),
            initSpeed.z * time
        );
    }
}
