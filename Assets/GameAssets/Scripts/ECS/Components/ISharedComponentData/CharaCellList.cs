using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    [Serializable]
    public struct CharaCellList : IEquatable<CharaCellList>, ISharedComponentData
    {
        List<CharaCell> m_charaCellList;

        public void Init()
        {
            m_charaCellList = new List<CharaCell>();

            var loadObjects = Resources.LoadAll<CharaCellObject>(PathSettings.CharaCell);
            if (loadObjects.Length == 0)
            {
                Debug.LogError("CharaCell None");
                return;
            }

            m_charaCellList = loadObjects[0].param;
        }

        public bool Equals(CharaCellList obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Transform>.Default.GetHashCode();
        }
    }
}