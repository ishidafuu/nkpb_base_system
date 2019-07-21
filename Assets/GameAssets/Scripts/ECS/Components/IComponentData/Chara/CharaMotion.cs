using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    public struct CharaMotion : IComponentData
    {
        public EnumMotion motionType;
        public int count;
        public int totalCount;

        public void SwitchMotion(EnumMotion _motionType)
        {
            motionType = _motionType;
            count = 0;
            totalCount = 0;
        }
    }
}
