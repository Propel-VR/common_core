#if UNITY_2018_1_OR_NEWER
#define VLB_SRP_SUPPORT // Comment this to disable SRP support
#endif

#if VLB_SRP_SUPPORT
#if UNITY_2019_1_OR_NEWER
using AliasCurrentPipeline = UnityEngine.Rendering.RenderPipelineManager;
using AliasCameraEvents = UnityEngine.Rendering.RenderPipelineManager;
using CallbackType = System.Action<UnityEngine.Rendering.ScriptableRenderContext, UnityEngine.Camera>;
#else
using AliasCurrentPipeline = UnityEngine.Experimental.Rendering.RenderPipelineManager;
using AliasCameraEvents = UnityEngine.Experimental.Rendering.RenderPipeline;
using CallbackType = System.Action<UnityEngine.Camera>;
#endif // UNITY_2019_1_OR_NEWER
#endif // VLB_SRP_SUPPORT

namespace VLB
{
    public static class SRPHelper
    {
        public static RenderPipeline projectRenderPipeline
        {
            get
            {
                // cache the value to prevent from comparing strings (in ComputeRenderPipeline) each frame when SRPBatcher is enabled
                if (!m_IsRenderPipelineCached)
                {
                    m_RenderPipelineCached = ComputeRenderPipeline();
                    m_IsRenderPipelineCached = true;
                }
                return m_RenderPipelineCached;
            }
        }


        static bool m_IsRenderPipelineCached = false;
        static RenderPipeline m_RenderPipelineCached;

        static RenderPipeline ComputeRenderPipeline()
        {
#if VLB_SRP_SUPPORT
        var rp = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
        if (rp)
        {
            var name = rp.GetType().ToString();
            if (name.Contains("Universal"))     return RenderPipeline.URP;
            if (name.Contains("Lightweight"))   return RenderPipeline.URP;
            if (name.Contains("HD"))            return RenderPipeline.HDRP;
        }
#endif
            return RenderPipeline.BuiltIn;
        }

#if VLB_SRP_SUPPORT
    public static bool IsUsingCustomRenderPipeline()
    {
        // TODO: optimize and use renderPipelineType
        return AliasCurrentPipeline.currentPipeline != null || UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null;
    }

    public static void RegisterOnBeginCameraRendering(CallbackType cb)
    {
        if (IsUsingCustomRenderPipeline())
        {
            AliasCameraEvents.beginCameraRendering -= cb;
            AliasCameraEvents.beginCameraRendering += cb;
        }
    }

    public static void UnregisterOnBeginCameraRendering(CallbackType cb)
    {
        if (IsUsingCustomRenderPipeline())
        {
            AliasCameraEvents.beginCameraRendering -= cb;
        }
    }
#else
        public static bool IsUsingCustomRenderPipeline() { return false; }
#endif
    }
}

