using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    public struct CharaMotion : IComponentData
    {
        public EnumMotionType m_motionType;
        public int m_count;
        public int m_frame;
    }
}
