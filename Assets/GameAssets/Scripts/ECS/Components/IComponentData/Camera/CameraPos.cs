using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    public struct CameraPos : IComponentData
    {
        public float m_position;
        public float m_delta;
    }
}
