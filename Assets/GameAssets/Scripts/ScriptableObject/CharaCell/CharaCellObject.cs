using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace NKPB
{
    [System.Serializable]
    public class CharaCellObject : ScriptableObject
    {
        public List<CharaCell> param = new List<CharaCell>();
    }
}
