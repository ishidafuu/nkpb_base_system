using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    public struct CharaMove : IComponentData
    {
        public Vector3Int position;
        public Vector3Int delta;
    }
}
