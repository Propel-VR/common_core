using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Salvage.ClothingCuller.Editor
{
    public static class UnityRenderPipelineHelper
    {
        public static Lazy<RenderPipelineType> RenderPipeline;
        public static Lazy<Shader> DefaultShader;

        #region Constructor

        static UnityRenderPipelineHelper()
        {
            RenderPipeline = new Lazy<RenderPipelineType>(getRenderPiplelineType);
            DefaultShader = new Lazy<Shader>(getDefaultShader);
        }

        #endregion

        #region Private Methods

        private static RenderPipelineType getRenderPiplelineType()
        {
            if (GraphicsSettings.renderPipelineAsset == null)
            {
                return RenderPipelineType.Standard;
            }

            if (GraphicsSettings.renderPipelineAsset.GetType().Name == "HDRenderPipelineAsset")
            {
                return RenderPipelineType.HighDefinition;
            }

            return RenderPipelineType.Universal;
        }

        private static Shader getDefaultShader()
        {
            switch (RenderPipeline.Value)
            {
                case RenderPipelineType.HighDefinition:
                case RenderPipelineType.Universal:
#if UNITY_2019_1_OR_NEWER
                    string defaultShaderName = GraphicsSettings.renderPipelineAsset.defaultShader.name;
                    return Shader.Find(defaultShaderName.Replace("Lit", "Unlit"));
#else
                    string defaultShaderName = GraphicsSettings.renderPipelineAsset.GetDefaultShader().name;
                    return Shader.Find(defaultShaderName.Replace("Lit", "Unlit"));
#endif
                default:
                    return Shader.Find("Standard");

            }
        }

        #endregion

        #region Public Methods

        public static Material CreateOpaqueMaterial(string name, Color color)
        {
            var material = new Material(DefaultShader.Value)
            {
                name = name,
                hideFlags = HideFlags.HideAndDontSave
            };

            SetColor(material, color);

            switch (RenderPipeline.Value)
            {
                case RenderPipelineType.HighDefinition:
                    material.SetFloat("_Smoothness", 0f);
                    break;
                case RenderPipelineType.Universal:
                    material.SetFloat("_Glossiness", 0f);
                    break;
                case RenderPipelineType.Standard:
                    material.SetFloat("_Glossiness", 0f);
                    break;
            }

            return material;
        }

        public static Material CreateTransparentMaterial(string name, Color color)
        {
            var material = new Material(DefaultShader.Value)
            {
                name = name,
                hideFlags = HideFlags.HideAndDontSave,
                renderQueue = 3000
            };

            SetColor(material, color);
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);

            switch (RenderPipeline.Value)
            {
                case RenderPipelineType.HighDefinition:
                    material.SetFloat("_Smoothness", 0f);
                    material.SetFloat("_SurfaceType", 1);
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetShaderPassEnabled("DistortionVectors", false);
                    material.SetShaderPassEnabled("TransparentDepthPrepass", false);
                    material.SetShaderPassEnabled("TransparentDepthPostpass", false);
                    material.SetShaderPassEnabled("TransparentBackface", false);
                    material.SetShaderPassEnabled("MOTIONVECTORS", false);
                    material.shaderKeywords = new string[]
                    {
                        "_BLENDMODE_ALPHA",
                        "_BLENDMODE_PRESERVE_SPECULAR_LIGHTING",
                        "_ENABLE_FOG_ON_TRANSPARENT",
                        "_NORMALMAP_TANGENT_SPACE",
                        "_SURFACE_TYPE_TRANSPARENT"
                    };
                    break;
                case RenderPipelineType.Universal:
                    material.SetFloat("_Glossiness", 0f);
                    material.SetFloat("_Surface", 1f);
                    material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                    break;
                case RenderPipelineType.Standard:
                    material.SetFloat("_Glossiness", 0f);
                    material.SetFloat("_Mode", 3);
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.shaderKeywords = new string[]
                    {
                        "_ALPHAPREMULTIPLY_ON"
                    };
                    break;
            }

            return material;
        }

        public static void SetColor(Material material, Color color)
        {
            switch (RenderPipeline.Value)
            {
                case RenderPipelineType.HighDefinition:
                    material.SetColor("_BaseColor", color);
                    material.SetColor("_UnlitColor", color);
                    break;
                case RenderPipelineType.Universal:
                    material.SetColor("_BaseColor", color);
                    break;
            }

            material.SetColor("_Color", color);
        }

        #endregion

    }
}
