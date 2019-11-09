using System;
using System.Collections.Generic;
using UnityEngine;

namespace NKPB
{
    [Serializable]
    public class CharacterMotionMaster : ScriptableObject
    {
        public List<CharacterMotionData> motionData = new List<CharacterMotionData>();
    }
}
