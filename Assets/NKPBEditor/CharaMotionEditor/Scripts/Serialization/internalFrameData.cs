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
    public struct internalFrameData
    {
        public struct serializableData
        {
            public int numColliders;
            public float framerate;
            public bool hasnextframe;

            public static implicit operator serializableData(internalFrameData v)
            {
                return new serializableData
                {
                    framerate = v.framerate,
                    hasnextframe = v.hasnextframe,
                    numColliders = (v.frame.collider == null ? 0 : v.frame.collider.Length),
                };
            }
        }

        public internalHitboxData frame;
        public float framerate;
        public bool hasnextframe;

        public internalFrameData(HitboxAnimation source, int frameID)
        {
            frame = source.frameData[frameID];
            hasnextframe = ((frameID + 1) < source.frameData.Length);
            framerate = (source.clip != null)
                ? source.clip.frameRate
                : 0f;
        }

#if UNITY_EDITOR
        /// <summary>
        /// シリアライズ
        /// /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            List<byte> data = new List<byte>(256);
            data.AddRange(BinaryStructConverter.ToByteArray((serializableData)this));

            if (frame.collider != null && frame.collider.Length > 0)
                for (int i = 0; i < frame.collider.Length; i++)
                {
                    data.AddRange(BinaryStructConverter.ToByteArray(frame.collider[i]));
                }

            return Convert.ToBase64String(data.ToArray());
        }
#endif

        /// <summary>
        /// デシリアライズ
        /// /// </summary>
        /// <param name="s"></param>
        static public internalFrameData Deserialize(string s)
        {
            var data = Convert.FromBase64String(s);
            var serializedResult = BinaryStructConverter.FromByteArray<serializableData>(data);
            var count = serializedResult.numColliders;
            internalFrameData result = serializedResult;

            if (count > 0)
            {
                int sizeofFrame = Marshal.SizeOf(typeof(serializableData));
                int sizeofCollider = Marshal.SizeOf(typeof(HitboxColliderData));
                var colliderArray = new HitboxColliderData[count];

                for (int i = 0; i < count; i++)
                {
                    colliderArray[i] = BinaryStructConverter.FromByteArray<HitboxColliderData>(data, sizeofFrame + sizeofCollider * i);
                }

                result.frame.collider = colliderArray;
            }

            return result;
        }

        static public implicit operator internalFrameData(serializableData v)
        {
            return new internalFrameData
            {
                framerate = v.framerate,
                hasnextframe = v.hasnextframe,
            };
        }

    }

}
