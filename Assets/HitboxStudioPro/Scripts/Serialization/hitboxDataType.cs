using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.InteropServices;

namespace NKPB
{

    [Serializable]
    public struct CharacterMotionData
    {
        public string imageName;
        public HitboxAnimationFrame[] framedata;
    }

    [Serializable]
    public struct HitboxAnimation
    {
        public AnimationClip clip;
        public HitboxAnimationFrame[] framedata;
    }

    [Serializable]
    public struct HitboxAnimationFrame
    {
        public HitboxColliderData[] collider;
        public HitboxFrameEventData[] events;
        public float time;
    }

    [Serializable]
    public struct HitboxFrameEventData
    {
        public FrameEvent id;
        public int intParam;
        public float floatParam;
        public string stringParam;
    }

    [Serializable]
    public struct HitboxColliderData
    {
        public RectInt rect;
        public HitboxType type;
    }
}
