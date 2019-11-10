using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace NKPB
{
    [System.Serializable]
    public class MapPalette : ScriptableObject
    {
        public List<int> wall;
        public List<int> floor;
        public void Init(bool isAddOne)
        {
            wall = new List<int>();
            floor = new List<int>();
            if (!isAddOne)
            {
                wall.Add(0);
                floor.Add(0);
            }
        }

        public void Add()
        {
            wall.Add(0);
            floor.Add(0);
        }

        public void Remove()
        {
            if (wall.Count > 2)
            {
                wall.RemoveAt(wall.Count - 1);
                floor.RemoveAt(floor.Count - 1);
            }
        }

        public int Count()
        {
            return wall.Count;
        }

        public int GetWall(int palNo)
        {
            return wall[palNo];
        }

        public int GetFloor(int palNo)
        {
            return floor[palNo];
        }

        public void SetWall(int palNo, int value)
        {
            wall[palNo] = value;
        }

        public void SetFloor(int palNo, int value)
        {
            floor[palNo] = value;
        }

        //コピー
        public MapPalette GetCopy()
        {
            MapPalette res = CreateInstance<MapPalette>();
            res.Init(true);
            foreach (var item in this.wall) res.wall.Add(item);
            foreach (var item in this.floor) res.floor.Add(item);
            return res;
        }
        public MapPalette GetClone()
        {
            return GetCopy();
        }
    }
}