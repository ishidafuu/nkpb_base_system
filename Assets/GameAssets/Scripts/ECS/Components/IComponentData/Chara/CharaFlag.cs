using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    public struct CharaFlag : IComponentData
    {
        public boolean mukiFlag;
        public FlagInputCheck inputCheckFlag;
        public FlagMove moveFlag;
        public FlagMotion motionFlag;
    }
}
