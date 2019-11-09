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
        public string motionName;
        public CharacterMotionFrame[] frameData;
    }

    [Serializable]
    public struct CharacterMotionFrame
    {
        public string imageName;
        public HitboxColliderData[] collider;
        public HitboxFrameEventData[] events;
    }

    [Serializable]
    public struct HitboxAnimation
    {
        public AnimationClip clip;
        public HitboxAnimationFrame[] frameData;
    }

    [Serializable]
    public struct HitboxAnimationFrame
    {
        public string imageName;
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
