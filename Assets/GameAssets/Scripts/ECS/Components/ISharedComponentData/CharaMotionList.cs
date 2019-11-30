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

        public CharacterMotionFrame GetMotionData(CharaMotion motion)
        {
            if ((int)motion.m_motionType >= m_charaMotionList.Count)
            {
                Debug.LogError($"Out of m_charaMotionList.Count {motion.m_motionType}");
            }
            // Debug.Log(motion.m_motionType); 
            int len = m_charaMotionList[(int)motion.m_motionType].frameData.Length;
            int index = motion.m_frame % len;

            return m_charaMotionList[(int)motion.m_motionType].frameData[index];
        }

        public string GetImageName(CharaMotion motion)
        {
            return GetMotionData(motion).imageName;
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