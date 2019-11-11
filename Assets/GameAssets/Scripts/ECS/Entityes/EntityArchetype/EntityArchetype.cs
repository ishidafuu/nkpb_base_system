﻿using Unity.Entities;
using Unity.Transforms;
namespace NKPB
{
    public static class ComponentTypes
    {
        public static ComponentType[] CharaComponentType = {
            typeof(CharaId),
            typeof(Translation),
            typeof(CharaDelta),
            typeof(CharaMuki),
            typeof(CharaDash),
            typeof(CharaLook),
            typeof(CharaMotion),
            typeof(CharaFlag),
            typeof(CharaQueue),
            typeof(CharaMap),
            // typeof(PadScan), // 入力（ここでは付けずCharaEntityFactoryで必要なキャラのみつける）
        };
    }
}
