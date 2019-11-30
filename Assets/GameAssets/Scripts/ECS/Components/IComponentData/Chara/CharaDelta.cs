using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    public struct CharaDelta : IComponentData
    {
        public int m_deltaX;
        public int m_deltaY;
        public int m_deltaZ;
    }
}
