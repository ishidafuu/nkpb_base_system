using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    public struct CharaQueue : IComponentData
    {
        public boolean m_isQueue;
        public EnumMotionType m_motionType;
        public EnumMuki m_muki;

        public void SetQueue(EnumMotionType motionType, EnumMuki muki = EnumMuki.None)
        {
            m_isQueue = true;
            m_motionType = motionType;
            if (muki != EnumMuki.None)
            {
                m_muki = muki;
            }

            // Debug.Log($"SetQueue:{motionType}");
        }
    }
}
