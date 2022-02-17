﻿using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace kTools.Motion
{
    [Serializable, VolumeComponentMenu("kMotion/Motion Blur")]
    public sealed class MotionBlur: VolumeComponent, IPostProcessComponent
    {
        /// <summary>
        /// Enable/Disable camera-based motion blur. Object-based motion blur is applied anyway.
        /// </summary>
        [Tooltip("Enable/Disable camera-based motion blur. Object-based motion blur is applied anyway.")]
        public BoolParameter cameraBasedMB = new BoolParameter(true);
        
        /// <summary>
        /// The quality of the effect. Lower presets will result in better performance at the expense of visual quality.
        /// </summary>
        [Tooltip("The quality of the effect. Lower presets will result in better performance at the expense of visual quality.")]
        public MotionBlurQualityParameter quality = new MotionBlurQualityParameter(MotionBlurQuality.Low);
        
        /// <summary>
        /// The strength of the motion blur filter. Acts as a multiplier for velocities.
        /// </summary>
        [Tooltip("The strength of the motion blur filter. Acts as a multiplier for velocities.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        /// <summary>
        /// The minimum velocity for the motion blur filter to be applied.
        /// </summary>
        [Tooltip("The minimum velocity for the motion blur filter to be applied.")]
        public ClampedFloatParameter threshold = new ClampedFloatParameter(0.01f, 0f, 1f);

        /// <summary>
        /// Is the component active?
        /// </summary>
        /// <returns>True is the component is active</returns>
        public bool IsActive() => intensity.value > 0f;

        /// <summary>
        /// Is the component compatible with on tile rendering
        /// </summary>
        /// <returns>false</returns>
        public bool IsTileCompatible() => false;
    }
}
