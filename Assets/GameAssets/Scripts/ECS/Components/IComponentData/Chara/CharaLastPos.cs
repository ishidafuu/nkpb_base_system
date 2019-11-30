using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    public struct CharaLastPos : IComponentData
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
        public int m_tipLeftX;
        public int m_tipCenterX;
        public int m_tipRightX;
    }
}
