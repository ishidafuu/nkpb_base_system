using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class MapTips : ScriptableObject
{
    public int mapSizeX;
    public int mapSizeY;
    public int mapSizeZ;
    public int[] shapes;
    public int[] palettes;

    public void Init(int sizeX, int sizeY, int sizeZ)
    {
        mapSizeX = sizeX;
        mapSizeY = sizeY;
        mapSizeZ = sizeZ;
        shapes = new int[mapSizeX * mapSizeY * mapSizeZ];
        palettes = new int[mapSizeX * mapSizeY * mapSizeZ];
    }

    public int this[int x, int y, int z, bool isPal]
    {
        set
        {
            int yy = (y * mapSizeX);
            int zz = (z * mapSizeX * mapSizeY);
            if (isPal)
            {
                palettes[x + yy + zz] = value;
            }
            else
            {
                shapes[x + yy + zz] = value;
            }
        }
        get
        {
            int yy = (y * mapSizeX);
            int zz = (z * mapSizeX * mapSizeY);
            if (isPal)
            {
                return palettes[x + yy + zz];
            }
            else
            {
                return shapes[x + yy + zz];
            }

        }
    }

    public int this[Vector3Int vec, bool isPal]
    {
        set
        {
            this[vec.x, vec.y, vec.z, isPal] = value;
        }
        get
        {
            return this[vec.x, vec.y, vec.z, isPal];
        }
    }

    public int GetShape(Vector3Int pos)
    {
        //範囲外の時は0
        int res = 0;
        if (IsSafePos(pos))
            res = this[pos.x, pos.y, pos.z, false];
        return res;
    }

    public int GetPalette(Vector3Int pos)
    {
        //範囲外の時は0
        int res = 0;
        if (IsSafePos(pos))res = this[pos.x, pos.y, pos.z, true];
        return res;
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
                    Vector3Int tagpos = new Vector3Int(pos.x, pos.y, pos.z);
                    tagpos.x += x;
                    tagpos.y += y;
                    tagpos.z += z;
                    Assert.IsTrue(this.IsSafePos(tagpos)); //はみでチェック
                    res[x, y, z, true] = this[tagpos, true];
                    res[x, y, z, false] = this[tagpos, false];
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
                    Vector3Int tagpos = new Vector3Int(pos.x, pos.y, pos.z);
                    tagpos.x += x;
                    tagpos.y += y;
                    tagpos.z += z;
                    if (this.IsSafePos(tagpos)) //はみでチェック
                    {
                        this[tagpos, true] = tips[x, y, z, true];
                        this[tagpos, false] = tips[x, y, z, false];
                    }
                }
            }
        }
    }
}
