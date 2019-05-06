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
    public struct Matrix2x2
    {
        public Vector2 x;
        public Vector2 y;
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
        public Vector2Int capsuleOffset;
        public float time;
        public bool smoothedOffset;
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
