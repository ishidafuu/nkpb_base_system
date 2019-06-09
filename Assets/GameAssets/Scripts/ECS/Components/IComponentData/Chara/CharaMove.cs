using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    /// <summary>
    /// キャラの座標移動量
    /// </summary>
    public struct CharaMove : IComponentData
    {
        public Vector3Int position;
        public Vector3Int delta;
    }
}
