using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class MapTips : ScriptableObject
{
    [SerializeField]
    public int mapSizeX;
    [SerializeField]
    public int mapSizeY;
    [SerializeField]
    public int mapSizeZ;
    [SerializeField]
    public enShapeType[] shapes;
    [SerializeField]
    public int[] events;

    public void Init(int sizeX, int sizeY, int sizeZ)
    {
        mapSizeX = sizeX;
        mapSizeY = sizeY;
        mapSizeZ = sizeZ;
        shapes = new enShapeType[mapSizeX * mapSizeY * mapSizeZ];
        events = new int[mapSizeX * mapSizeY * mapSizeZ];
    }

    enShapeType this[int x, int y, int z]
    {
        set
        {
            shapes[ConvertXYZToIndex(x, y, z)] = value;
        }
        get
        {
            return shapes[ConvertXYZToIndex(x, y, z)];
        }
    }

    enShapeType this[Vector3Int vec]
    {
        set
        {
            this[vec.x, vec.y, vec.z] = value;
        }
        get
        {
            return this[vec.x, vec.y, vec.z];
        }
    }

    int ConvertXYZToIndex(int x, int y, int z)
    {
        int yy = (y * mapSizeX);
        int zz = (z * mapSizeX * mapSizeY);
        return x + yy + zz;
    }

    int ConvertVector3IntToIndex(Vector3Int pos)
    {
        int yy = (pos.y * mapSizeX);
        int zz = (pos.z * mapSizeX * mapSizeY);
        return pos.x + yy + zz;
    }

    public enShapeType GetShape(Vector3Int pos)
    {
        //範囲外の時は0
        enShapeType res = enShapeType.Empty;
        if (IsSafePos(pos))
            res = this[pos.x, pos.y, pos.z];
        return res;
    }

    public enShapeType GetShape(int x, int y, int z)
    {
        return GetShape(new Vector3Int(x, y, z));
    }

    public int GetEvent(Vector3Int pos)
    {
        //範囲外の時は0
        int res = 0;

        if (IsSafePos(pos))
        {
            res = events[ConvertVector3IntToIndex(pos)];
        }

        return res;
    }

    public int GetEvent(int x, int y, int z)
    {
        return GetEvent(new Vector3Int(x, y, z));
    }

    public void SetShape(enShapeType value, Vector3Int pos)
    {
        if (IsSafePos(pos))
        {
            shapes[ConvertVector3IntToIndex(pos)] = value;
        }
    }

    public void SetShape(enShapeType value, int x, int y, int z)
    {
        SetShape(value, new Vector3Int(x, y, z));
    }

    public void SetEvent(int value, Vector3Int pos)
    {
        if (IsSafePos(pos))
        {
            events[ConvertVector3IntToIndex(pos)] = value;
        }
    }

    public void SetEvent(int value, int x, int y, int z)
    {
        SetEvent(value, new Vector3Int(x, y, z));
    }

    public bool IsSafePos(Vector3Int pos)
    {
        bool res = (pos.x >= 0) && (pos.y >= 0) && (pos.z >= 0)
            && (pos.x < mapSizeX) && (pos.y < mapSizeY) && (pos.z < mapSizeZ);
        return res;
    }
    //コピー
    public MapTips GetCopy(Vector3Int pos, Vector3Int size)
    {
        //MapTips res = new MapTips(size.x, size.y, size.z);
        MapTips res = CreateInstance<MapTips>();
        res.Init(size.x, size.y, size.z);

        for (int x = 0; x < size.x; ++x)
        {
            for (int y = 0; y < size.y; ++y)
            {
                for (int z = 0; z < size.z; ++z)
                {
                    Vector3Int tagpos = new Vector3Int(pos.x + x, pos.y + y, pos.z + z);
                    // Assert.IsTrue(this.IsSafePos(tagpos));
                    Debug.Log(tagpos + " shape" + GetShape(tagpos));
                    res.SetShape(GetShape(tagpos), x, y, z);
                    // res.SetEvent(GetEvent(tagpos), tagpos);
                }
            }
        }

        return res;
    }
    public MapTips GetClone()
    {
        return GetCopy(new Vector3Int(0, 0, 0), new Vector3Int(this.mapSizeX, this.mapSizeY, this.mapSizeZ));
    }
    //貼り付け
    public void SetPaste(Vector3Int pos, MapTips tips)
    {
        for (int x = 0; x < tips.mapSizeX; ++x)
        {
            for (int y = 0; y < tips.mapSizeY; ++y)
            {
                for (int z = 0; z < tips.mapSizeZ; ++z)
                {
                    Vector3Int tagpos = new Vector3Int(pos.x + x, pos.y + y, pos.z + z);
                    if (this.IsSafePos(tagpos)) //はみでチェック
                    {
                        SetShape(tips.GetShape(x, y, z), tagpos);
                    }
                }
            }
        }
    }
}
