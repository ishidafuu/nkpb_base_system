using System;
using System.Collections.Generic;
using UnityEngine;

namespace NKPB
{
    [Serializable]
    public class CharacterMotionMaster : ScriptableObject
    {
        public List<CharacterMotionData> motionDatas = new List<CharacterMotionData>();
    }
}
