using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    public struct CharaPos : IComponentData
    {
        public Vector3Int m_position { get; private set; }
        public int m_Y;
        public int m_Z;
        public int m_leftX;
        public int m_centerX;
        public int m_rightX;

        public int m_innerY;
        public int m_innerZ;
        public int m_innerLeftX;
        public int m_innerCenterX;
        public int m_innerRightX;

        public int m_lastY;
        public int m_lastZ;
        public int m_lastLeftX;
        public int m_lastCenterX;
        public int m_lastRightX;

        public void SetPosition(Vector3Int newPosition)
        {
            m_position = newPosition;

            m_Y = m_position.y >> 11;
            m_Z = m_position.z >> 11;
            m_centerX = m_position.x >> 11;

            int leftX = (m_position.x - 7);
            int rightX = (m_position.x + 7);
            m_leftX = leftX >> 11;
            m_rightX = rightX >> 11;

            m_innerCenterX = (m_centerX << 3) ^ m_position.x;
            m_innerLeftX = (m_leftX << 3) ^ leftX;
            m_innerRightX = (m_rightX << 3) ^ rightX;
        }
    }
}
