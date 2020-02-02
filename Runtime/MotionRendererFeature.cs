using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace kTools.Motion
{
    sealed class MotionRendererFeature : ScriptableRendererFeature
    {
#region Fields
        static MotionRendererFeature s_Instance;
        readonly MotionVectorRenderPass m_MotionVectorRenderPass;
        readonly MotionBlurRenderPass m_MotionBlurRenderPass;

        Dictionary<Camera, MotionData> m_MotionDatas;
        uint  m_FrameCount;
        float m_LastTime;
        float m_Time;
#endregion

#region Constructors
        internal MotionRendererFeature()
        {
            // Set data
            s_Instance = this;
            m_MotionVectorRenderPass = new MotionVectorRenderPass();
            m_MotionBlurRenderPass = new MotionBlurRenderPass();
            m_MotionDatas = new Dictionary<Camera, MotionData>();
        }
#endregion

#region Initialization
        public override void Create()
        {
            name = "Motion";
        }
#endregion
        
#region RenderPass
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // Get MotionData
            var camera = renderingData.cameraData.camera;
            MotionData motionData;
            if(!m_MotionDatas.TryGetValue(camera, out motionData))
            {
                motionData = new MotionData();
                m_MotionDatas.Add(camera, motionData);
            }

            // Calculate motion data
            CalculateTime();
            UpdateMotionData(camera, motionData);

            // Motion vector pass
            m_MotionVectorRenderPass.Setup(motionData);
            renderer.EnqueuePass(m_MotionVectorRenderPass);

            // Motion blur pass
            var stack = VolumeManager.instance.stack;
            var motionBlur = stack.GetComponent<MotionBlur>();
            if (motionBlur.IsActive() && !renderingData.cameraData.isSceneViewCamera)
            {
                m_MotionBlurRenderPass.Setup(motionBlur);
                renderer.EnqueuePass(m_MotionBlurRenderPass);
            }
        }
#endregion

        void CalculateTime()
        {
            // Get data
            float t = Time.realtimeSinceStartup;
            uint  c = (uint)Time.frameCount;

            // SRP.Render() can be called several times per frame.
            // Also, most Time variables do not consistently update in the Scene View.
            // This makes reliable detection of the start of the new frame VERY hard.
            // One of the exceptions is 'Time.realtimeSinceStartup'.
            // Therefore, outside of the Play Mode we update the time at 60 fps,
            // and in the Play Mode we rely on 'Time.frameCount'.
            bool newFrame;
            if (Application.isPlaying)
            {
                newFrame = m_FrameCount != c;
                m_FrameCount = c;
            }
            else
            {
                newFrame = (t - m_Time) > 0.0166f;
                m_FrameCount += newFrame ? (uint)1 : (uint)0;
            }

            if (newFrame)
            {
                // Make sure both are never 0.
                m_LastTime = (m_Time > 0) ? m_Time : t;
                m_Time  = t;
            }
        }

        void UpdateMotionData(Camera camera, MotionData motionData)
        {
            // The actual projection matrix used in shaders is actually massaged a bit to work across all platforms
            // (different Z value ranges etc.)
            var gpuProj = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true); // Had to change this from 'false'
            var gpuView = camera.worldToCameraMatrix;
            var gpuVP = gpuProj * gpuView;

            // Set last frame data
            // A camera could be rendered multiple times per frame, only updates the previous view proj & pos if needed
            if (motionData.lastFrameActive != Time.frameCount)
            {
                motionData.isFirstFrame = false;
                motionData.previousViewProjectionMatrix = motionData.isFirstFrame ? 
                        gpuVP : motionData.viewProjectionMatrix;
            }

            // Set current frame data
            motionData.viewProjectionMatrix = gpuVP;
            motionData.lastFrameActive = Time.frameCount;
        }
    }
}
