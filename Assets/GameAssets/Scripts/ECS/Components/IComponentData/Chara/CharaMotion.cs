using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    /// <summary>
    /// キャラのモーション
    /// </summary>
    public struct CharaMotion : IComponentData
    {
        public EnumMotion motionType;
        public int count;
        public int totalCount;
    }
}
