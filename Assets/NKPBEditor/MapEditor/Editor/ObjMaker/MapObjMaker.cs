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

        //パス
        // List<MapPathPlate> SearchMapPath(MapTips mapTips3)
        //{
        //	List<MapPathPlate> pathList = new List<MapPathPlate>();
        //	for (int x = 0; x < mapTips3.mapSizeX; ++x)
        //	{
        //		for (int z = 0; z < mapTips3.mapSizeZ; ++z)
        //		{
        //			int index = (x + (mapTips3.mapSizeX * z));
        //			if (!isXZ_[index])
        //			{
        //				var path = GetMapPath(x, z, mapTips3);
        //				if (path != null) pathList.Add(path);
        //			}
        //		}
        //	}

        //	return pathList;
        //}
        // MapPathPlate GetMapPath(int stX, int stZ, MapTips mapTips3)
        //{

        //	//基準点ブロック
        //	//真上からブロックにぶつかるまで探す
        //	int baseY = -1;
        //	int baseShape = 0;
        //	bool isFind = false;
        //	for (int y = 0; y < mapTips3.mapSizeY; ++y)
        //	{
        //		int revY = (mapTips3.mapSizeY - y - 1);
        //		Vector3Int pos = new Vector3Int(stX, revY, stZ);
        //		int shape = mapTips3.GetShape(pos);
        //		if (shape == 0) continue;//空チップ

        //		//if (tip != 1) continue;//壁チップ以外は別で

        //		baseY = revY;
        //		baseShape = shape;
        //		isFind = true;
        //		break;
        //	}

        //	if (!isFind) return null;

        //	int sameW = 1;//Z方向が同じ幅
        //	int restX = (mapTips3.mapSizeX - stX);
        //	for (int x = 0; x < restX; ++x)
        //	{
        //		Vector3Int pos = new Vector3Int(stX + x, baseY, stZ);
        //		Vector3Int uppos = new Vector3Int(stX + x, baseY + 1, stZ);

        //		//範囲外	//違うチップ //上が空じゃない
        //		if (!mapTips3.IsSafePos(pos)
        //			|| (mapTips3.GetShape(pos) != baseShape)
        //			|| (mapTips3.GetShape(uppos) != 0)
        //			|| (isXZ_[(stX + x) + (mapTips3.mapSizeX * (stZ))])
        //			)
        //		{
        //			break;
        //		}
        //		else
        //		{
        //			sameW = x + 1;
        //		}
        //	}

        //	int sameD = 1;
        //	int restZ = (mapTips3.mapSizeZ - stZ);
        //	for (int z = 0; z < restZ; ++z)
        //	{
        //		if (z == 0) continue;
        //		bool isAllXOK = true;
        //		for (int x = 0; x < sameW; ++x)
        //		{
        //			Vector3Int pos = new Vector3Int((stX + x), baseY, (stZ + z));
        //			Vector3Int uppos = new Vector3Int((stX + x), baseY + 1, (stZ + z));
        //			//Debug.Log(i + "pos" + (pos));
        //			//範囲外	//違うチップ //上が空じゃない
        //			if (!mapTips3.IsSafePos(pos)
        //				|| (mapTips3.GetShape(pos) != baseShape)
        //				|| (mapTips3.GetShape(uppos) != 0)
        //				|| (isXZ_[(stX + x) + (mapTips3.mapSizeX * (stZ + z))])
        //				)
        //			{
        //				isAllXOK = false;
        //				break;
        //			}
        //		}

        //		if (isAllXOK)
        //		{
        //			sameD = z + 1;
        //		}
        //		else
        //		{
        //			break;
        //		}
        //	}

        //	//チェックつける
        //	//Debug.Log("stX:" + stX + "  stZ:" + stZ + "  sameD:" + sameD + "  sameW:" + sameW);
        //	for (int z = 0; z < sameD; ++z)
        //	{
        //		for (int x = 0; x < sameW; ++x)
        //		{
        //			isXZ_[(stX + x) + (mapTips3.mapSizeX * (stZ + z))] = true;
        //		}
        //	}
        //	MapPathPlate res = new MapPathPlate(baseShape, baseY, stX, stZ, sameW, sameD);
        //	return res;
        //}

        ////コライダー
        // List<MapCollider> SearchMapCollider(MapTips mapTips3, int mapSizeX, int mapSizeZ, int mapSizeY)
        //{
        //	List<MapCollider> colliderList = new List<MapCollider>();
        //	for (int x = 0; x < mapSizeX; ++x)
        //	{
        //		for (int z = 0; z < mapSizeZ; ++z)
        //		{
        //			//方体コライダー
        //			int index = (x + (mapSizeX * z));
        //			if (!isXZ_[index])
        //			{
        //				var boxCollider = GetBoxCollider(x, z, mapTips3, mapSizeX, mapSizeY, mapSizeZ);
        //				if (boxCollider != null) colliderList.Add(boxCollider);
        //			}
        //			//三角柱コライダー
        //			var prismCollider = GetPrismCollider(x, z, mapTips3, mapSizeY);
        //			if (prismCollider != null) colliderList.Add(prismCollider);
        //		}
        //	}

        //	//坂コライダー
        //	for (int x = 0; x < mapSizeX; ++x)
        //	{
        //		for (int y = 0; y < mapSizeY; ++y)
        //		{
        //			int index = (x + (mapSizeX * y));
        //			var slopeCollider = GetSlopeCollider(x, y, mapTips3, mapSizeZ);
        //			if (slopeCollider != null) colliderList.Add(slopeCollider);
        //		}
        //	}

        //	return colliderList;
        //}
        // MapCollider GetBoxCollider(int stX, int stZ, MapTips mapTips3, int mapSizeX, int mapSizeY, int mapSizeZ)
        //{

        //	//基準点ブロック
        //	//真上からブロックにぶつかるまで探す
        //	int baseY = -1;
        //	int baseTip = 0;
        //	bool isFind = false;
        //	for (int y = 0; y < mapSizeY; ++y)
        //	{
        //		int revY = (mapSizeY - y - 1);
        //		Vector3Int pos = new Vector3Int(stX, revY, stZ);
        //		int tip = mapTips3.GetTip(pos);
        //		if (tip == 0) continue;//空チップ

        //		if (tip != 1) continue;//壁チップ以外は別で

        //		baseY = revY;
        //		baseTip = tip;
        //		isFind = true;
        //		break;
        //	}

        //	if (!isFind) return null;

        //	int sameW = 1;//Z方向が同じ幅
        //	int restX = (mapSizeX - stX);
        //	for (int x = 0; x < restX; ++x)
        //	{
        //		Vector3Int pos = new Vector3Int(stX + x, baseY, stZ);
        //		Vector3Int uppos = new Vector3Int(stX + x, baseY + 1, stZ);

        //		//範囲外	//違うチップ //上が空じゃない
        //		if (!mapTips3.IsSafePos(pos)
        //			|| (mapTips3.GetTip(pos) != baseTip)
        //			|| (mapTips3.GetTip(uppos) != 0)
        //			|| (isXZ_[(stX + x) + (mapSizeX * (stZ))])
        //			)
        //		{
        //			break;
        //		}
        //		else
        //		{
        //			sameW = x + 1;
        //		}
        //	}

        //	int sameD = 1;
        //	int restZ = (mapSizeZ - stZ);
        //	for (int z = 0; z < restZ; ++z)
        //	{
        //		if (z == 0) continue;
        //		bool isAllXOK = true;
        //		for (int x = 0; x < sameW; ++x)
        //		{
        //			Vector3Int pos = new Vector3Int((stX + x), baseY, (stZ + z));
        //			Vector3Int uppos = new Vector3Int((stX + x), baseY + 1, (stZ + z));
        //			//Debug.Log(i + "pos" + (pos));
        //			//範囲外	//違うチップ //上が空じゃない
        //			if (!mapTips3.IsSafePos(pos)
        //				|| (mapTips3.GetTip(pos) != baseTip)
        //				|| (mapTips3.GetTip(uppos) != 0)
        //				|| (isXZ_[(stX + x) + (mapSizeX * (stZ + z))])
        //				)
        //			{
        //				isAllXOK = false;
        //				break;
        //			}
        //		}

        //		if (isAllXOK)
        //		{
        //			sameD = z + 1;
        //		}
        //		else
        //		{
        //			break;
        //		}
        //	}

        //	//チェックつける
        //	//Debug.Log("stX:" + stX + "  stZ:" + stZ + "  sameD:" + sameD + "  sameW:" + sameW);
        //	for (int z = 0; z < sameD; ++z)
        //	{
        //		for (int x = 0; x < sameW; ++x)
        //		{
        //			//Debug.Log((stX + x) + (mapSizeX * (stZ + z)));
        //			isXZ_[(stX + x) + (mapSizeX * (stZ + z))] = true;
        //		}
        //	}
        //	MapCollider res = new MapCollider(baseTip, baseY, stX, stZ, sameW, sameD);
        //	return res;
        //}

        ////三角柱コライダー
        // MapCollider GetPrismCollider(int stX, int stZ, MapTips mapTips3, int mapSizeY)
        //{
        //	//真上からブロックにぶつかるまで探す
        //	int baseY = -1;
        //	int baseTip = 0;
        //	bool isFind = false;
        //	for (int y = 0; y < mapSizeY; ++y)
        //	{
        //		int revY = (mapSizeY - y - 1);
        //		Vector3Int pos = new Vector3Int(stX, revY, stZ);
        //		int tip = mapTips3.GetTip(pos);
        //		if (tip == 0) continue;//空チップ

        //		if (!IsTriFloor(tip)) continue;//三角床のみ

        //		baseY = revY;
        //		baseTip = tip;
        //		isFind = true;
        //		break;
        //	}

        //	if (isFind)
        //	{
        //		MapCollider res = new MapCollider(baseTip, baseY, stX, stZ, 1, 1);
        //		return res;
        //	}
        //	else
        //	{
        //		return null;
        //	}

        //}

        ////坂コライダー
        // MapCollider GetSlopeCollider(int stX, int stY, MapTips mapTips3, int mapSizeZ)
        //{
        //	//手前からブロックにぶつかるまで探す
        //	int baseZ = -1;
        //	int baseTip = 0;
        //	bool isFind = false;
        //	for (int z = 0; z < mapSizeZ; ++z)
        //	{
        //		Vector3Int pos = new Vector3Int(stX, stY, z);
        //		int tip = mapTips3.GetTip(pos);
        //		if (tip == 0) continue;//空チップ

        //		if (!IsTriWall(tip)) continue;//三角壁のみ

        //		baseZ = z;
        //		baseTip = tip;
        //		isFind = true;
        //		break;
        //	}

        //	if (isFind)
        //	{
        //		MapCollider res = new MapCollider(baseTip, stY, stX, baseZ, 1, mapSizeZ - baseZ);
        //		return res;
        //	}
        //	else
        //	{
        //		return null;
        //	}
        //}
    }
}