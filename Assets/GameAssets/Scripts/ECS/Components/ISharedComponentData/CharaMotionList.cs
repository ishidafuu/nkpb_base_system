using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    [Serializable]
    public struct CharaMotionList : IEquatable<CharaMotionList>, ISharedComponentData
    {
        List<CharacterMotionData> m_charaMotionList;

        public void Init()
        {
            m_charaMotionList = new List<CharacterMotionData>();

            var loadObjects = Resources.LoadAll<CharacterMotionMaster>(PathSettings.CharaMotion);
            if (loadObjects.Length == 0)
            {
                Debug.LogError("CharacterMotionMaster None");
                return;
            }

            m_charaMotionList = loadObjects[0].motionData;
        }

        public bool Equals(CharaMotionList obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Transform>.Default.GetHashCode();
        }
    }
}