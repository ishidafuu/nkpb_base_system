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
    public struct internalHitboxData
    {
        public HitboxColliderData[] collider;

        public static implicit operator internalHitboxData(HitboxAnimationFrame v)
        {
            return new internalHitboxData
            {
                collider = v.collider,
            };
        }
    }

}
