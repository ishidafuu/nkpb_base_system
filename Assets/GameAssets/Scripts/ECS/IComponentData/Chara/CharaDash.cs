using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    /// <summary>
    /// キャラの向き
    /// </summary>
    public struct CharaDash : IComponentData
    {
        public EnumMuki dashMuki;
    }
}
