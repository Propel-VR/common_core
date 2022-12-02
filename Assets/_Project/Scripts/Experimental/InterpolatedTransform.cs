using UnityEngine;
using System.Collections;

[DefaultExecutionOrder(1000)]
public class InterpolatedTransform : MonoBehaviour
{
    private TransformFrameData[] _lastFrameData;
    private int _newTransformIndex;
    private int OldTransformIndex => (_newTransformIndex == 0 ? 1 : 0);
    private bool copyFlag = false;
    private TransformFrameData preInterpFrame;


    void OnEnable () { ResetFrames(); InterpolationController.RegisterTransform(this); }
    void OnDisable () => InterpolationController.UnregisterTransform(this);
    private void FixedUpdate ()
    {
        copyFlag = true;
    }
    void LateUpdate ()
    {
        if(copyFlag)
        {
            CopyNewFrame();
            copyFlag = false;
        }
    }



    // Sets both saved frames to the current transform frame
    private void ResetFrames ()
    {
        _lastFrameData = new TransformFrameData[2];
        TransformFrameData t = new(
            transform.localPosition,
            transform.localRotation,
            transform.localScale);
        _lastFrameData[0] = t;
        _lastFrameData[1] = t;
        _newTransformIndex = 0;
    }

    // Copies new tranform frame
    private void CopyNewFrame ()
    {
        _newTransformIndex = OldTransformIndex;
        _lastFrameData[_newTransformIndex] = CopyFrameData();
    }

    // Copies new tranform frame to a transform frame data struct
    private TransformFrameData CopyFrameData ()
    {
        return new(
            transform.localPosition,
            transform.localRotation,
            transform.localScale);
    }

    // Applies interpolation from saved frame
    public void ApplyInterpolation (float interpolationFactor)
    {
        preInterpFrame = CopyFrameData();
        TransformFrameData newestTransform = _lastFrameData[_newTransformIndex];
        TransformFrameData olderTransform = _lastFrameData[OldTransformIndex];

        transform.localPosition = Vector3.Lerp(
            olderTransform.position,
            newestTransform.position,
            interpolationFactor);

        transform.localRotation = Quaternion.Slerp(
            olderTransform.rotation,
            newestTransform.rotation,
            interpolationFactor);

        transform.localScale = Vector3.Lerp(
            olderTransform.scale,
            newestTransform.scale,
            interpolationFactor);
    }

    // Reverts interpolation to last known state
    public void ClearInterpolation ()
    {
        ApplyFrameData(preInterpFrame);
    }

    private void ApplyFrameData (TransformFrameData frameData)
    {
        transform.localPosition = frameData.position;
        transform.localRotation = frameData.rotation;
        transform.localScale = frameData.scale;
    }

    private struct TransformFrameData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformFrameData (Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}