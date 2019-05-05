using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.InteropServices;

namespace BlackGardenStudios.HitboxStudioPro
{
    public static class IntConverter
    {
        [StructLayout(LayoutKind.Explicit)]
        struct IntConverterData
        {
            [FieldOffset(0)]
            public Int32 Value;
            [FieldOffset(0)]
            public Int16 LoValue;
            [FieldOffset(2)]
            public Int16 HiValue;
        }

        static public Vector2 DecodeIntToVector2(int value)
        {
            var data = new IntConverterData { Value = value };

            return new Vector2(data.LoValue, data.HiValue);
        }

        static public int EncodeVector2ToInt(Vector2 value)
        {
            return new IntConverterData
            {
                LoValue = (short)value.x,
                    HiValue = (short)value.y
            }.Value;
        }
    }
}
