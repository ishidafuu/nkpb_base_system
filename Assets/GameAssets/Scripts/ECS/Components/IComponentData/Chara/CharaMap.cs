using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    public struct CharaMap : IComponentData
    {
        public Vector3Int m_leftPos;
        public Vector3Int m_centerPos;
        public Vector3Int m_rightPos;

        public Vector3Int m_lastLeftPos;
        public Vector3Int m_lastCenterPos;
        public Vector3Int m_lastRightPos;
    }
}
