using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    public struct CharaPos : IComponentData
    {
        public Vector3Int m_position { get; private set; }
        public int m_mapY;
        public int m_mapZ;
        public int m_leftMapX;
        public int m_centerMapX;
        public int m_rightMapX;

        public int m_tipY;
        public int m_tipZ;
        public int m_tipLeftX;
        public int m_tipCenterX;
        public int m_tipRightX;

        public int m_lastMapY;
        public int m_lastMapZ;
        public int m_lastLeftMapX;
        public int m_lastCenterMapX;
        public int m_lastRightMapX;


        public void SetX(int x)
        {
            SetPosition(new Vector3Int(x, m_position.y, m_position.z));
        }

        public void SetY(int y)
        {
            SetPosition(new Vector3Int(m_position.x, y, m_position.z));
        }

        public void SetZ(int z)
        {
            SetPosition(new Vector3Int(m_position.x, m_position.y, z));
        }

        public void SetPosition(Vector3Int newPosition)
        {
            const int SHIFT_PIXEL = 8;
            const int SHIFT_TIP = 3;
            const int SHIFT_MAP = 11;

            if (newPosition.z < 0)
            {
                newPosition.z = 0;
            }
            // TODO:仮
            if (newPosition.x < 0)
            {
                newPosition.x = 0;
            }

            m_position = newPosition;

            m_mapY = m_position.y >> SHIFT_MAP;
            m_mapZ = m_position.z >> SHIFT_MAP;

            int leftX = (m_position.x - 7);
            int rightX = (m_position.x + 7);
            m_centerMapX = m_position.x >> SHIFT_MAP;
            m_leftMapX = leftX >> SHIFT_MAP;
            m_rightMapX = rightX >> SHIFT_MAP;

            m_tipCenterX = ((m_position.x >> SHIFT_PIXEL) ^ ((m_position.x >> (SHIFT_PIXEL + SHIFT_TIP)) << SHIFT_TIP));
            m_tipLeftX = ((leftX >> SHIFT_PIXEL) ^ ((leftX >> (SHIFT_PIXEL + SHIFT_TIP)) << SHIFT_TIP));
            m_tipRightX = ((rightX >> SHIFT_PIXEL) ^ ((rightX >> (SHIFT_PIXEL + SHIFT_TIP)) << SHIFT_TIP));

            // Debug.Log($"m_position : {m_position} m_centerX : {m_centerMapX} m_innerCenterX : {m_tipCenterX}");
        }
    }
}
