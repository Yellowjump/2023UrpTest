using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;


[VolumeComponentMenu("你好/时间结界")]
public class TimeFieldVolume : VolumeComponent, IPostProcessComponent {
    public Vector2Parameter screenPos = new(new(0.5f, 0.5f));
    public MinFloatParameter intensity = new(10, 0);
    public ClampedFloatParameter scatter = new(0, 0, 1.5f);


    public bool IsActive() {
        return true;
    }
    public bool IsTileCompatible() {
        return false;
    }
}