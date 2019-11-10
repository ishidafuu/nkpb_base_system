using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    public struct CharaDelta : IComponentData
    {
        public Vector3Int m_position;
        public Vector3Int m_delta;
    }
}
