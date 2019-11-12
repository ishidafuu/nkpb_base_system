using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    [Serializable]
    public struct MapTipList : IEquatable<MapTipList>, ISharedComponentData
    {
        public List<NativeMapTips> m_MapTipList;

        public void Init()
        {
            m_MapTipList = new List<NativeMapTips>();

            var loadObjects = Resources.LoadAll<MapTips>(PathSettings.MapData);
            if (loadObjects.Length == 0)
            {
                Debug.LogError("MapTip None");
                return;
            }

            foreach (var item in loadObjects)
            {
                var newMapTips = new NativeMapTips(item);
                m_MapTipList.Add(newMapTips);
            }
        }

        public void Dispose()
        {
            foreach (var item in m_MapTipList)
            {
                item.Dispose();
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