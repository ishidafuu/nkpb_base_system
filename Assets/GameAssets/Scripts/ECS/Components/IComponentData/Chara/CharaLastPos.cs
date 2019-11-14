using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    public struct CharaLastPos : IComponentData
    {
        public int m_lastMapY;
        public int m_lastMapZ;
        public int m_lastLeftMapX;
        public int m_lastCenterMapX;
        public int m_lastRightMapX;
    }
}
