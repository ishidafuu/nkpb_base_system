using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    public struct CharaPos : IComponentData
    {
        public Vector3Int m_position { get; private set; }
        public int m_mapY;
        public int m_mapZ;
        public int m_mapXLeft;
        public int m_mapXCenter;
        public int m_mapXRight;

        public int m_tipY;
        public int m_tipZ;
        public int m_tipLeftX;
        public int m_tipCenterX;
        public int m_tipRightX;

        const int RAW_PIX = 8;
        const int PIX_MAP = 3;
        const int RAW_MAP = RAW_PIX + PIX_MAP;

        public void SetPixX(int x)
        {
            SetPosition(new Vector3Int(x << RAW_PIX, m_position.y, m_position.z));
        }

        public void SetPixY(int y)
        {
            SetPosition(new Vector3Int(m_position.x, y << RAW_PIX, m_position.z));
        }

        public void SetPixZ(int z)
        {
            SetPosition(new Vector3Int(m_position.x, m_position.y, z << RAW_PIX));
        }

        public void SetPosition(Vector3Int newPosition)
        {


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

            m_mapY = m_position.y >> RAW_MAP;
            m_mapZ = m_position.z >> RAW_MAP;

            int centerPixX = m_position.x >> RAW_PIX;
            int leftPixX = centerPixX - 7;
            int rightPixX = centerPixX + 7;

            m_mapXCenter = centerPixX >> PIX_MAP;
            m_mapXLeft = leftPixX >> PIX_MAP;
            m_mapXRight = rightPixX >> PIX_MAP;

            m_tipCenterX = (centerPixX ^ ((centerPixX >> PIX_MAP) << PIX_MAP));
            m_tipLeftX = (leftPixX ^ ((leftPixX >> PIX_MAP) << PIX_MAP));
            m_tipRightX = (rightPixX ^ ((rightPixX >> PIX_MAP) << PIX_MAP));

            // Debug.Log($"m_position : {m_position} m_centerX : {m_centerMapX} m_innerCenterX : {m_tipCenterX}");
        }
    }
}
