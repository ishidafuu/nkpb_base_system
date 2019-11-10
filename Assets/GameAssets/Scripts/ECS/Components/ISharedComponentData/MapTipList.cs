using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    [Serializable]
    public struct MapTipList : IEquatable<MapTipList>, ISharedComponentData
    {
        List<MapTips> m_MapTipList;

        public void Init()
        {
            m_MapTipList = new List<MapTips>();

            var loadObjects = Resources.LoadAll<MapTips>(PathSettings.MapData);
            if (loadObjects.Length == 0)
            {
                Debug.LogError("MapTip None");
                return;
            }

            foreach (var item in loadObjects)
            {
                m_MapTipList.Add(item);
            }
        }

        public bool Equals(MapTipList obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Transform>.Default.GetHashCode();
        }
    }
}