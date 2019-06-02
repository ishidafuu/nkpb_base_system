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

        public CharaCell this[int index]
        {
            set
            {
                param[index] = value;
            }
            get
            {
                return param[index];
            }
        }

        public CharaCellObject GetClone()
        {
            CharaCellObject res = CreateInstance<CharaCellObject>();
            foreach (var item in param)
            {
                res.param.Add(item.ShallowCopy());

            }
            return res;
        }

    }
}
