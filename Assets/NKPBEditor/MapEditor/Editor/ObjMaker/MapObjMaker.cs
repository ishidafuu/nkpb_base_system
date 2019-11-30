using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKPB
{
    public class MapObjMaker : Editor
    {
        bool[] m_isXZ;

        //オブジェクト作成
        public void CreateObject(MapTips mapTips3, int mapId)
        {
            m_isXZ = new bool[mapTips3.mapSizeX * mapTips3.mapSizeZ];

            //XZ面で長方形サーチを掛けていく
            //未チェックの箇所があった場合、そこから長方形探索ループに入る
            //Xのラインで大きい長方形が出来るように内側のループをZでサーチを掛けていく
            List<MapNodePlate> nodeList = SearchMapNode(mapTips3);
            List<TileFloor> floorList = SearchFloor(mapTips3);
            List<TileWall> wallList = SearchWall(mapTips3);

            CreateMapPath.CreateMapPathObject(mapId, nodeList);

        }

        //床
        List<TileFloor> SearchFloor(MapTips mapTips3)
        {
            List<TileFloor> floorList = new List<TileFloor>();
            for (int x = 0; x < mapTips3.mapSizeX; ++x)
            {
                for (int z = 0; z < mapTips3.mapSizeZ; ++z)
                {
                    //int index = (x + (mapSizeX * z));
                    TileFloor sqFloor = GetFloor(true, x, z, mapTips3);
                    if (sqFloor != null) floorList.Add(sqFloor);

                    TileFloor triFloor = GetFloor(false, x, z, mapTips3);
                    if (triFloor != null) floorList.Add(triFloor);
                }
            }
            return floorList;
        }
        TileFloor GetFloor(bool isSquare, int stX, int stZ, MapTips mapTips3)
        {
            //真上からブロックにぶつかるまで探す
            int baseY = -1;
            EnumShapeType baseShape = 0;
            int basePal = 0;
            bool isFind = false;
            for (int y = 0; y < mapTips3.mapSizeY; ++y)
            {
                int revY = (mapTips3.mapSizeY - y - 1);
                Vector3Int pos = new Vector3Int(stX, revY, stZ);
                EnumShapeType shape = mapTips3.GetShape(pos);
                int pal = mapTips3.GetEvent(pos);
                if (shape == 0) continue; //空チップ

                if (isSquare)
                {
                    if (IsTriFloor(shape)) continue;
                }
                else
                {
                    if (!IsTriFloor(shape)) continue;
                }

                baseY = revY + 1;
                baseShape = shape;
                basePal = pal;
                isFind = true;
                break;
            }

            if (isFind)
            {
                TileFloor res = new TileFloor(baseShape, basePal, baseY, stX, stZ);
                return res;
            }
            else
            {
                return null;
            }

        }
        bool IsTriFloor(EnumShapeType shape)
        {
            bool res = false;
            switch (shape)
            {
                case EnumShapeType.SlashWall:
                case EnumShapeType.BSlashWall:
                    res = true;
                    break;
            }
            return res;
        }

        //壁
        List<TileWall> SearchWall(MapTips mapTips3)
        {
            List<TileWall> wallList = new List<TileWall>();
            for (int x = 0; x < mapTips3.mapSizeX; ++x)
            {
                for (int y = 0; y < mapTips3.mapSizeY; ++y)
                {
                    //int index = (x + (mapSizeX * y));
                    TileWall sqWall = GetWall(true, x, y, mapTips3);
                    if (sqWall != null) wallList.Add(sqWall);

                    TileWall triWall = GetWall(false, x, y, mapTips3);
                    if (triWall != null) wallList.Add(triWall);
                }
            }
            return wallList;
        }
        TileWall GetWall(bool isSquare, int stX, int stY, MapTips mapTips3)
        {
            //手前からブロックにぶつかるまで探す
            int baseZ = -1;
            EnumShapeType baseShape = EnumShapeType.Empty;
            int basePal = 0;
            bool isFind = false;
            for (int z = 0; z < mapTips3.mapSizeZ; ++z)
            {
                Vector3Int pos = new Vector3Int(stX, stY, z);
                EnumShapeType shape = mapTips3.GetShape(pos);
                int pal = mapTips3.GetEvent(pos);
                if (shape == 0) continue; //空チップ

                if (isSquare)
                {
                    if (IsTriWall(shape)) continue;
                }
                else
                {
                    if (!IsTriWall(shape)) continue;
                }

                baseZ = z;
                baseShape = shape;
                basePal = pal;
                isFind = true;
                break;
            }

            if (isFind)
            {
                TileWall res = new TileWall(baseShape, basePal, baseZ, stX, stY);
                return res;
            }
            else
            {
                return null;
            }
        }
        bool IsTriWall(EnumShapeType tipNo)
        {
            bool res = false;
            switch (tipNo)
            {
                case EnumShapeType.LUpSlope:
                case EnumShapeType.RUpSlope:
                case EnumShapeType.LUpSlope2H:
                case EnumShapeType.LUpSlope2L:
                case EnumShapeType.RUpSlope2L:
                case EnumShapeType.RUpSlope2H:
                    res = true;
                    break;
            }
            return res;
        }

        //パス
        List<MapNodePlate> SearchMapNode(MapTips mapTips3)
        {
            List<MapNodePlate> pathList = new List<MapNodePlate>();
            for (int x = 0; x < mapTips3.mapSizeX; ++x)
            {
                for (int z = 0; z < mapTips3.mapSizeZ; ++z)
                {
                    var path = GetMapPath(x, z, mapTips3);
                    if (path.Count != 0) pathList.AddRange(path);
                }
            }

            return pathList;
        }
        List<MapNodePlate> GetMapPath(int stX, int stZ, MapTips mapTips3)
        {
            List<MapNodePlate> res = new List<MapNodePlate>();
            //基準点ブロック
            //真上からブロックにぶつかるまで探す
            int baseY = -1;
            bool isTriFloor = false;
            for (int y = 0; y < mapTips3.mapSizeY; ++y)
            {
                int revY = (mapTips3.mapSizeY - y - 1);
                Vector3Int pos = new Vector3Int(stX, revY, stZ);
                EnumShapeType shape = mapTips3.GetShape(pos);
                if (shape == 0) continue; //空チップ

                baseY = revY;
                res.Add(new MapNodePlate(shape, baseY, stX, stZ, mapTips3.mapSizeX, mapTips3.mapSizeZ));
                isTriFloor = IsTriFloor(shape);
                break;
            }

            //三角床の場合は、下面もサーチ
            if (isTriFloor)
            {
                for (int y = baseY; y < mapTips3.mapSizeY; ++y)
                {
                    int revY = (mapTips3.mapSizeY - y - 1);
                    Vector3Int pos = new Vector3Int(stX, revY, stZ);
                    EnumShapeType shape = mapTips3.GetShape(pos);
                    if (shape != EnumShapeType.Box)
                        continue;

                    res.Add(new MapNodePlate(shape, revY, stX, stZ, mapTips3.mapSizeX, mapTips3.mapSizeZ));
                    break;
                }
            }

            return res;
        }

    }
}