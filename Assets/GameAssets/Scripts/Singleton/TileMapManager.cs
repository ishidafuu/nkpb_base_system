// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Unity.Collections;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.Tilemaps;

// namespace NKPB
// {
// 	[ExecuteInEditMode]
// 	public class TileMapManager : SingletonMonoBehaviour<TileMapManager>
// 	{
// 		// public int mapSize = 100;
// 		public float fDiv = 50f;
// 		public int fillPercent = 10;
// 		public int smoothCount = 1;
// 		public int surfaceNum = 6;
// 		public int holeR = 20;
// 		public Tilemap tilemap;
// 		public TileBase holeTile;
// 		public List<TileBase> groundTile;
// 		public List<TileBase> tile2;

// 		const int HOLENO = -99;
// 		// public List<int> surfType;
// 		public void LoadTileMap()
// 		{
// 			// オートセット
// 			if (tilemap == null)
// 			{
// 				var tileMaps = FindObjectsOfType<Tilemap>();
// 				foreach (var item in tileMaps)
// 				{
// 					if (item.layoutGrid.name == "GeoGrid")
// 					{
// 						tilemap = item;
// 					}
// 				}
// 			}

// 			tilemap.layoutGrid.cellSize = new Vector3(Define.Instance.TIP_SIZE, Define.Instance.TIP_SIZE, 0);
// 		}

// 		public int MapTipNum()
// 		{
// 			return ((Define.Instance.MAP_GRID_NUM * Define.Instance.GRID_SIZE) / Define.Instance.TIP_SIZE);
// 		}

// 		// マップタイル作成
// 		public void CreatePerlinNoise()
// 		{

// 			LoadTileMap();

// 			var tipTypeNum = groundTile.Count;
// 			if (tipTypeNum == 0)
// 			{
// 				Debug.LogWarning("tipTypeNum Zero");
// 				return;
// 			}

// 			// マップを初期化する
// 			var mapSize = MapTipNum();
// 			int[, ] map = new int[mapSize, mapSize];

// 			//// 乱数生成器にシード値を設定する
// 			// System.Random rand = new System.Random(seed.GetHashCode());
// 			// パーリンノイズでマップ生成
// 			map = PerlinNoiseSurface(map, groundTile.Count);

// 			// チップをランダムで配置（割合指定）
// 			var mapFilter = GenerateCellularAutomata(mapSize, fillPercent, false);

// 			// ムーア近傍
// 			mapFilter = SmoothMooreCellularAutomata(mapFilter, false, smoothCount);
// 			map = SetFilter(map, mapFilter, true);
// 			// ムーア近傍
// 			mapFilter = GenerateCellularAutomata(mapSize, fillPercent, false);
// 			mapFilter = SmoothMooreCellularAutomata(mapFilter, false, smoothCount);
// 			map = SetFilter(map, mapFilter, false);

// 			// ムーア近傍
// 			mapFilter = GenerateCellularAutomata(mapSize, fillPercent, false);
// 			mapFilter = SmoothMooreCellularAutomata(mapFilter, false, smoothCount);
// 			map = SetFilter(map, mapFilter, true);
// 			// 巣穴周り
// 			map = MakeHomeHole2(map, new Vector2Int(mapSize / 2, mapSize / 2), holeR);
// 			// ムーア近傍
// 			mapFilter = GenerateCellularAutomata(mapSize, fillPercent, false);
// 			mapFilter = SmoothMooreCellularAutomata(mapFilter, false, smoothCount);
// 			map = SetFilter(map, mapFilter, false);

// 			// 本巣穴
// 			map = MakeHomeHole2(map, new Vector2Int(mapSize / 2, mapSize / 2), holeR * 0.6f);
// 			// 範囲チェック
// 			map = CheckMapBetween(map);
// 			// tilemap.size = new Vector3Int(mapSize, mapSize, 0);
// 			// int cellsize = (int)tilemap.layoutGrid.cellSize.x;

// 			// タイルすべて置く
// 			SetTileFromMapArray(map, mapSize);
// 		}

// 		int[, ] CheckMapBetween(int[, ] map)
// 		{
// 			var mapSize = MapTipNum();
// 			for (int x = 0; x < mapSize; x++)
// 			{
// 				for (int y = 0; y < mapSize; y++)
// 				{
// 					var tileNo = map[x, y];
// 					if (map[x, y] == HOLENO)continue;
// 					else if (map[x, y] < 0)map[x, y] = 0;
// 					else if (map[x, y] >= groundTile.Count)map[x, y] = groundTile.Count - 1;
// 				}
// 			}
// 			return map;
// 		}

// 		int[, ] MakeHomeHole2(int[, ] map, Vector2Int pos, float holeR)
// 		{
// 			var mapSize = MapTipNum();

// 			for (int x = 0; x < mapSize; x++)
// 			{
// 				for (int y = 0; y < mapSize; y++)
// 				{

// 					if (x < pos.x - holeR)continue;
// 					if (x > pos.x + holeR)continue;
// 					if (y < pos.y - holeR)continue;
// 					if (y > pos.y + holeR)continue;

// 					var p = new Vector2Int(x, y);
// 					var dist = Vector2Int.Distance(pos, p);
// 					if (dist < (holeR))
// 					{
// 						map[x, y] = Mathf.FloorToInt((dist / holeR) * (groundTile.Count / 2));
// 					}

// 				}
// 			}

// 			// 巣穴
// 			map[mapSize / 2, mapSize / 2] = HOLENO;
// 			// map[mapSize / 2 - 1, mapSize / 2] = HOLENO;
// 			// map[mapSize / 2, mapSize / 2 - 1] = HOLENO;
// 			// map[mapSize / 2 - 1, mapSize / 2 - 1] = HOLENO;

// 			return map;
// 		}

// 		// タイルすべて置く
// 		public void SetTileFromMapArray(int[, ] map, int mapSize)
// 		{
// 			tilemap.ClearAllTiles();
// 			for (int x = 0; x < mapSize; x++)
// 			{
// 				for (int y = 0; y < mapSize; y++)
// 				{
// 					// tilemap.AddTileFlags(position, TileFlags.LockAll);
// 					// int index = (y * mapSize) + x;
// 					var tileNo = map[x, y];
// 					if (tileNo == HOLENO)
// 					{
// 						tilemap.SetTile(new Vector3Int(x, y, 0), holeTile);
// 						continue;
// 					}

// 					tilemap.SetTile(new Vector3Int(x, y, 0), groundTile[tileNo]);
// 				}
// 			}
// 		}

// 		int[, ] PerlinNoiseSurface(int[, ] map, int tipTypeNum)
// 		{
// 			var mapSize = MapTipNum();
// 			var surfType = new List<int>(mapSize * mapSize);
// 			float xf = Random.value * (float)mapSize;
// 			float yf = Random.value * (float)mapSize;
// 			for (int x = 0; x < mapSize; x++)
// 			{
// 				for (int y = 0; y < mapSize; y++)
// 				{
// 					// int index = (y * mapSize) + x;
// 					float noise = Mathf.PerlinNoise(((float)x + xf) / fDiv, ((float)y + yf) / fDiv);
// 					var surf = Mathf.FloorToInt(noise * tipTypeNum);
// 					if (surf < 0)surf = 0;
// 					else if (surf >= tipTypeNum)surf = tipTypeNum - 1;
// 					map[x, y] = surf;
// 					// surfType.Add(surf);
// 				}
// 			}

// 			return map;
// 		}

// 		//// マップタイル作成
// 		// public void CreateSmoothMoore() {

// 		// 	LoadTileMap();
// 		// 	//// 乱数生成器にシード値を設定する
// 		// 	// System.Random rand = new System.Random(seed.GetHashCode());
// 		// 	// List<int> surfType = PerlinNoiseSurface(surfaceNum);

// 		// 	// チップをランダムで配置（割合指定）
// 		// 	var mapFilter = GenerateCellularAutomata(mapSize, fillPercent, false);
// 		// 	// ムーア近傍
// 		// 	SmoothMooreCellularAutomata(mapFilter, false, smoothCount);

// 		// 	// for (int x = 0; x < mapSize; x++) {
// 		// 	// 	for (int y = 0; y < mapSize; y++) {
// 		// 	// 		var position = new Vector3Int(x, y, 0);
// 		// 	// 		var thisTile = tilemap.GetTile(position);
// 		// 	// 		tilemap.SetTile(position, tile2[map[x, y]]);
// 		// 	// 	}
// 		// 	// }
// 		// }

// 		// チップをランダムで配置（割合指定）
// 		int[, ] GenerateCellularAutomata(int mapSize, int fillPercent, bool edgesAreWalls)
// 		{

// 			// マップを初期化する
// 			int[, ] map = new int[mapSize, mapSize];

// 			for (int x = 0; x < map.GetUpperBound(0); x++)
// 			{
// 				for (int y = 0; y < map.GetUpperBound(1); y++)
// 				{
// 					// エッジが壁に設定されている場合は、セルが on（1）に設定されるようにする
// 					if (edgesAreWalls && (x == 0 || x == map.GetUpperBound(0) - 1 || y == 0 || y == map.GetUpperBound(1) - 1))
// 					{
// 						map[x, y] = 1;
// 					}
// 					else
// 					{
// 						// グリッドをランダムに生成する
// 						map[x, y] = (Random.Range(0, 100) < fillPercent) ? 1 : 0;
// 					}
// 				}
// 			}
// 			return map;
// 		}

// 		// ムーア近傍
// 		int[, ] SmoothMooreCellularAutomata(int[, ] map, bool edgesAreWalls, int smoothCount)
// 		{
// 			for (int i = 0; i < smoothCount; i++)
// 			{
// 				for (int x = 0; x < map.GetUpperBound(0); x++)
// 				{
// 					for (int y = 0; y < map.GetUpperBound(1); y++)
// 					{
// 						int surroundingTiles = GetMooreSurroundingTiles(map, x, y, edgesAreWalls);

// 						if (edgesAreWalls && (x == 0 || x == (map.GetUpperBound(0) - 1) || y == 0 || y == (map.GetUpperBound(1) - 1)))
// 						{
// 							// edgesAreWalls が true であればエッジを壁として設定する
// 							map[x, y] = 1;
// 						}
// 						// デフォルトのムーア近傍の規則では 5 つ以上の近傍が必要
// 						else if (surroundingTiles > 4)
// 						{
// 							map[x, y] = 1;
// 						}
// 						else if (surroundingTiles < 4)
// 						{
// 							map[x, y] = 0;
// 						}
// 					}
// 				}
// 			}
// 			// 戻り値として修正されたマップを返す
// 			return map;
// 		}
// 		// ムーア近傍用周囲取得
// 		int GetMooreSurroundingTiles(int[, ] map, int x, int y, bool edgesAreWalls)
// 		{
// 			int tileCount = 0;

// 			for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
// 			{
// 				for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
// 				{
// 					if (neighbourX >= 0 && neighbourX < map.GetUpperBound(0) && neighbourY >= 0 && neighbourY < map.GetUpperBound(1))
// 					{
// 						// 周囲のチェックが行われている、中心のタイルはカウントしない
// 						if (neighbourX != x || neighbourY != y)
// 						{
// 							tileCount += map[neighbourX, neighbourY];
// 						}
// 					}
// 				}
// 			}
// 			return tileCount;
// 		}

// 		// フィルター
// 		int[, ] SetFilter(int[, ] map, int[, ] mapFilter, bool isPosi)
// 		{
// 			for (int x = 0; x < map.GetUpperBound(0); x++)
// 			{
// 				for (int y = 0; y < map.GetUpperBound(1); y++)
// 				{
// 					if (isPosi)
// 					{
// 						map[x, y] += mapFilter[x, y];
// 					}
// 					else
// 					{
// 						map[x, y] -= mapFilter[x, y];
// 					}
// 				}
// 			}

// 			// 戻り値として修正されたマップを返す
// 			return map;
// 		}

// 		// 	public void UpdateMap(int[,] map, Tilemap tilemap) { // マップとタイルマップを取得し、null タイルを必要箇所に設定する

// 		// 		// TileBase

// 		// 		for (int x = 0; x < map.GetUpperBound(0); x++) {
// 		// 			for (int y = 0; y < map.GetUpperBound(1); y++) {
// 		// 				// 再レンダリングではなく、マップの更新のみを行う
// 		// 				// これは、それぞれのタイル（および衝突データ）を再描画するのに比べて
// 		// 				// タイルを null に更新するほうが使用リソースが少なくて済むためです。
// 		// 				if (map[x, y] == 0) {
// 		// 					tilemap.SetTile(new Vector3Int(x, y, 0), null);
// 		// 				}
// 		// 			}
// 		// 		}
// 		// 	}

// 		// 	public int[,] PerlinNoise(int[,] map, float seed) {
// 		// 		int newPoint;
// 		// 		// パーリンノイズのポイントの位置を下げるために使用される
// 		// 		float reduction = 0.5f;
// 		// 		// パーリンノイズを生成する
// 		// 		for (int x = 0; x < map.GetUpperBound(0); x++) {
// 		// 			newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, seed) - reduction) * map.GetUpperBound(1));

// 		// 			// 高さの半分の位置付近からノイズが始まるようにする
// 		// 			newPoint += (map.GetUpperBound(1) / 2);
// 		// 			for (int y = newPoint; y >= 0; y--) {
// 		// 				map[x, y] = 1;
// 		// 			}
// 		// 		}
// 		// 		return map;
// 		// 	}
// 	}

// 	[CustomEditor(typeof(TileMapManager))] // 拡張するクラスを指定
// 	public class TileMapManagerEditor : Editor
// 	{
// 		public override void OnInspectorGUI()
// 		{
// 			// 元のInspector部分を表示
// 			base.OnInspectorGUI();

// 			// ボタンを表示
// 			if (GUILayout.Button("LoadTileMap"))(target as TileMapManager).LoadTileMap();
// 			if (GUILayout.Button("PerlinNoise"))(target as TileMapManager).CreatePerlinNoise();
// 			// if (GUILayout.Button("SmoothMoore")) (target as TileMapManager).CreateSmoothMoore();

// 		}

// 	}
// }
