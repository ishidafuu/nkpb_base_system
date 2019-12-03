using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    public struct CharaPos : IComponentData
    {
        public int m_posX;
        public int m_posY;
        public int m_posZ;
        public int m_mapY;
        public int m_mapZ;
        public int m_mapXLeft;
        public int m_mapXCenter;
        public int m_mapXRight;

        public int m_tipY;
        public int m_tipZ;
        public int m_tipXLeft;
        public int m_tipXCenter;
        public int m_tipXRight;

        const int RAW_PIX = 8;
        const int PIX_MAP = 3;
        const int RAW_MAP = RAW_PIX + PIX_MAP;

        public void SetPixX(int x)
        {
            Debug.Log($"SetPixX : {x}");
            SetPosition(new Vector3Int(x << RAW_PIX, m_posY, m_posZ));
        }

        public void SetPixY(int y)
        {
            Debug.Log($"SetPixY : {y}");
            SetPosition(new Vector3Int(m_posX, y << RAW_PIX, m_posZ));
        }

        public void SetPixZ(int z)
        {
            Debug.Log($"SetPixZ : {z}");
            SetPosition(new Vector3Int(m_posX, m_posY, z << RAW_PIX));
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

            m_posX = newPosition.x;
            m_posY = newPosition.y;
            m_posZ = newPosition.z;

            m_mapY = m_posY >> RAW_MAP;
            m_mapZ = m_posZ >> RAW_MAP;

            int centerPixX = m_posX >> RAW_PIX;
            int leftPixX = centerPixX - 7;
            int rightPixX = centerPixX + 7;

            m_mapXCenter = centerPixX >> PIX_MAP;
            m_mapXLeft = leftPixX >> PIX_MAP;
            m_mapXRight = rightPixX >> PIX_MAP;

            m_tipXCenter = (centerPixX ^ ((centerPixX >> PIX_MAP) << PIX_MAP));
            m_tipXLeft = (leftPixX ^ ((leftPixX >> PIX_MAP) << PIX_MAP));
            m_tipXRight = (rightPixX ^ ((rightPixX >> PIX_MAP) << PIX_MAP));
        }
    }
}
