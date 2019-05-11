using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MapCreater : EditorWindow
{
    const int TIPSIZE = 48;
    // マップエディタのマスの数
    public int mapId_ = 0;
    public int palId_ = 0;
    public int mapSizeX_ = 10;
    public int mapSizeY_ = 10;
    public int mapSizeZ_ = 10;
    [SerializeField]
    MapTips mapTips3_;
    MapPalette palette_;

    int selectedShape_ = 0;
    int selectedPalette_ = 0;
    bool isLoadSprite_ = false;
    bool isLoadMapTip_ = false;
    bool isPalMode_ = false;

    List<Texture> mapShapeTex_;
    List<Sprite> mapPaletteSprite_;
    Texture2D dummy_;
    Texture2D dummy2_;
    Texture2D dummy3_;
    MapEditor subWindow_; // サブウィンドウ
    MapObjMaker objMaker_;

    [UnityEditor.MenuItem("Window/MapCreater")]
    static void ShowMainWindow()
    {
        EditorWindow.GetWindow(typeof(MapCreater));
    }

    void OnGUI()
    {
        try
        {

            if (!isLoadSprite_)LoadMapTipSprite();

            if (!isLoadMapTip_)NewMap();

            if (palette_ == null)
            {
                palette_ = CreateInstance<MapPalette>();
                palette_.Init(false);
            }

            if (dummy_ == null)dummy_ = new Texture2D(TIPSIZE, TIPSIZE);
            if (dummy2_ == null)dummy2_ = new Texture2D((int)(TIPSIZE * 2.5f), (int)(TIPSIZE * 1.5f));
            if (dummy3_ == null)dummy3_ = new Texture2D((int)(TIPSIZE * 1.5f), (int)(TIPSIZE * 1.5f));
            EditorGUI.BeginChangeCheck();

            // GUI
            GUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            int mapId = EditorGUILayout.IntField("Map ID", mapId_);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            int palId = EditorGUILayout.IntField("Palette ID", palId_);
            GUILayout.EndHorizontal();

            GUILayout.Label("Map Size");
            GUILayout.BeginVertical();
            //GUILayout.FlexibleSpace();

            int mapSizeX = EditorGUILayout.IntField("X", mapSizeX_);
            int mapSizeY = EditorGUILayout.IntField("Y", mapSizeY_);
            int mapSizeZ = EditorGUILayout.IntField("Z", mapSizeZ_);
            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                mapId_ = Mathf.Clamp(mapId, 0, 999);
                palId_ = Mathf.Clamp(palId, 0, 999);
                mapSizeX_ = Mathf.Clamp(mapSizeX, 1, 99);
                mapSizeY_ = Mathf.Clamp(mapSizeY, 1, 99);
                mapSizeZ_ = Mathf.Clamp(mapSizeZ, 1, 99);
            }

            DrawButtonOpenMapEditor();
            DrawButtonLoadShapeAndPal();

            //リサイズ
            DrawButtonResize();
            //オブジェクト作成
            DrawButtonCreateObject();

            GUILayout.Label("Shape / Palette");

            bool tmpisPalMode_ = GUILayout.Toggle(isPalMode_, isPalMode_ ? "Palette" : "Shape", "button");

            if (tmpisPalMode_ != isPalMode_)
            {
                subWindow_.SwitchShapePal();
                isPalMode_ = tmpisPalMode_;
            }

            if (isPalMode_)
            {
                //新規パレット
                DrawButtonNewPal();
                //セーブロード
                DrawButtonPalSaveLoad();
                DrawButtonAddRmvPal();
                DrawPalette();
                DrawPaletteChanger();
            }
            else
            {
                //新規マップ
                DrawButtonNewMap();
                //セーブロード
                DrawButtonSaveLoad();
                DrawShapeTips();
                DrawSelectedShape();
            }

        }
        catch (System.Exception exeption)
        {
            if (exeption is ExitGUIException)
            {
                throw exeption;
            }
            else
            {
                Debug.LogError(exeption.ToString());
            }
        }
    }

    //スプライトの取得
    public Texture GetSprite(int spriteNo, enRotate rotate)
    {
        return mapShapeTex_[spriteNo + ((mapShapeTex_.Count / 4) * (int)rotate)];
    }
    public Texture GetTipsSprite(Vector3Int pos, enRotate rotate, bool isEmpNull = false)
    {
        if (mapTips3_ == null)return null;
        if (isEmpNull && (mapTips3_[pos, false] == 0))return null;

        return GetSprite(mapTips3_[pos, false], rotate);
    }
    public Texture GetSelectedSprite(enRotate rotate)
    {
        return GetSprite(selectedShape_, rotate);
    }

    public int GetTipsPal(Vector3Int pos)
    {
        return mapTips3_[pos, true];
    }

    void SetMapShapeGen(int value, int x, int y, int z)
    {
        mapTips3_[x, y, z, false] = value;
    }
    public void SetMapShape(Vector3Int pos)
    {
        mapTips3_[pos, false] = selectedShape_;
    }
    public void SetMapPalette(Vector3Int pos)
    {
        mapTips3_[pos, true] = selectedPalette_;
    }
    public MapTips GetCopyMapTip(Vector3Int pos, Vector3Int size)
    {
        return mapTips3_.GetCopy(pos, size);
    }
    public void SetPasteMapTip(Vector3Int pos, MapTips tips)
    {
        mapTips3_.SetPaste(pos, tips);
    }

    //スポイト
    public void Spuit(Vector3Int pos, bool isPal)
    {
        if (isPal)
        {
            selectedPalette_ = mapTips3_[pos, isPal];
        }
        else
        {
            selectedShape_ = mapTips3_[pos, isPal];
        }

        Repaint();
    }

    public bool IsPalMode()
    {
        return isPalMode_;
    }

    public int GetSelectedPal()
    {
        return selectedPalette_;
    }

    //ロード
    void LoadMapTipSprite()
    {
        // 読み込み(Resources.LoadAllを使うのがミソ)
        objMaker_ = new MapObjMaker();
        dummy_ = new Texture2D(TIPSIZE, TIPSIZE);
        dummy2_ = new Texture2D((int)(TIPSIZE * 2.5f), (int)(TIPSIZE * 1.5f));
        dummy3_ = new Texture2D((int)(TIPSIZE * 1.5f), (int)(TIPSIZE * 1.5f));

        mapShapeTex_ = new List<Texture>();
        mapShapeTex_ = NonResources.LoadAll<Texture>("Assets/MapCreater/Editor/MapTipPalette");

        mapPaletteSprite_ = new List<Sprite>();
        mapPaletteSprite_.AddRange(Resources.LoadAll<Sprite>("MapTile"));

        AssetDatabase.Refresh();

        isLoadSprite_ = true;
    }

    //マップ作成
    void NewMap()
    {
        //palette_ = CreateInstance<MapPalette>();
        mapTips3_ = CreateInstance<MapTips>();
        mapTips3_.Init(mapSizeX_, mapSizeY_, mapSizeZ_);

        for (int x = 0; x < mapSizeX_; ++x)
        {
            for (int y = 0; y < mapSizeY_; ++y)
            {
                for (int z = 0; z < mapSizeZ_; ++z)
                {
                    //mapTips2[x, y, z] = 0;

                    if ((y == 0) || (z == mapSizeZ_ - 1))SetMapShapeGen(1, x, y, z);
                }
            }
        }
        isLoadMapTip_ = true;
    }

    void NewPalette()
    {
        palette_ = CreateInstance<MapPalette>();
        palette_.Init(false);
    }

    //マップサイズ変更
    void ResizeMap()
    {
        MapTips newMapTips3 = CreateInstance<MapTips>();
        newMapTips3.Init(mapSizeX_, mapSizeY_, mapSizeZ_);

        for (int x = 0; x < mapTips3_.mapSizeX; ++x)
        {
            if (x >= mapSizeX_)continue;

            for (int y = 0; y < mapTips3_.mapSizeY; ++y)
            {
                if (y >= mapSizeY_)continue;

                for (int z = 0; z < mapTips3_.mapSizeZ; ++z)
                {
                    if (z >= mapSizeZ_)continue;

                    newMapTips3[x, y, z, true] = mapTips3_[x, y, z, true];
                    newMapTips3[x, y, z, false] = mapTips3_[x, y, z, false];
                }
            }
        }

        mapTips3_ = newMapTips3;

        OpenMapEditor();
    }

    // spriteの親テクスチャー上のRect座標を取得
    public static Rect GetSpriteNormalRect(Sprite sp)
    {
        Rect rectPosition = sp.textureRect;

        // 親テクスチャーの大きさを取得.
        float parentWith = sp.texture.width;
        float parentHeight = sp.texture.height;
        // spriteの座標を親テクスチャーに合わせて正規化.
        Rect NormalRect = new Rect(
            rectPosition.x / parentWith,
            rectPosition.y / parentHeight,
            rectPosition.width / parentWith,
            rectPosition.height / parentHeight
        );

        return NormalRect;
    }

    // 画像一覧をボタン選択出来る形にして出力
    void DrawShapeTips()
    {
        float x = 0.0f;
        float y = 00.0f;
        float maxW = TIPSIZE * 5;

        //EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        int index = 0;
        for (int i = 0; i < (mapShapeTex_.Count / 4); ++i)
        {
            var sp = mapShapeTex_[i];
            if (x >= maxW)
            {
                x = 0.0f;
                y += TIPSIZE;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            if (selectedShape_ == index)
            {
                GUI.contentColor = Color.magenta;
            }
            else
            {
                GUI.contentColor = Color.white;
            }

            if (GUILayout.Button(dummy_, GUILayout.MaxWidth(TIPSIZE), GUILayout.MaxHeight(TIPSIZE), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
            {
                selectedShape_ = index;
            }

            Rect lastRect = GUILayoutUtility.GetLastRect();
            lastRect.width = TIPSIZE;
            lastRect.height = TIPSIZE;
            GUI.DrawTextureWithTexCoords(lastRect, sp, new Rect(0, 0, 1, 1));
            x += TIPSIZE;

            index++;
        }
        EditorGUILayout.EndHorizontal();

    }

    // 画像一覧をボタン選択出来る形にして出力
    void DrawPalette()
    {

        float x = 0.0f;
        float y = 00.0f;
        //float size = 48;
        //float h = 50.0f;
        float maxW = TIPSIZE * 5;

        //EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        //int index = 0;
        for (int i = 0; i < palette_.Count(); i++)
        {
            int wallType = palette_.GetWall(i);
            int floorType = palette_.GetFloor(i);

            if (selectedPalette_ == i)
            {
                GUI.contentColor = Color.magenta;
            }
            else
            {
                GUI.contentColor = Color.white;
            }

            if (GUILayout.Button(dummy2_, GUILayout.MaxWidth(TIPSIZE * 2.5f), GUILayout.MaxHeight(TIPSIZE * 1.5f), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
            {
                selectedPalette_ = i;
                subWindow_.SwitchShapePal();
            }

            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect wallRect = new Rect(lastRect.x + (TIPSIZE / 4), lastRect.y + (TIPSIZE / 4), TIPSIZE, TIPSIZE);
            Rect floorRect = new Rect(lastRect.x + (TIPSIZE / 4) + TIPSIZE, lastRect.y + (TIPSIZE / 4), TIPSIZE, TIPSIZE);
            Sprite wallsp = mapPaletteSprite_[wallType * 2];
            Sprite floorsp = mapPaletteSprite_[(floorType * 2) + 1];
            GUI.DrawTextureWithTexCoords(wallRect, wallsp.texture, GetSpriteNormalRect(wallsp));
            GUI.DrawTextureWithTexCoords(floorRect, floorsp.texture, GetSpriteNormalRect(floorsp));
            //GUI.DrawTextureWithTexCoords(lastRect, sp.texture, new Rect(0, 0, 1, 1));
            //GUILayout.FlexibleSpace();
            x += TIPSIZE;

            if ((i + 1) % 3 == 0)
            {
                x = 0.0f;
                y += TIPSIZE * 1.5f;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            //index++;
        }
        //EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

    }

    // 選択中の形状
    void DrawSelectedShape()
    {

        //Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath (selectedImagePath, typeof(Texture2D));
        EditorGUILayout.BeginVertical();
        //GUILayout.FlexibleSpace();

        GUI.contentColor = Color.white;
        GUILayout.Box(dummy_);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        lastRect.x += (lastRect.width - TIPSIZE) / 2;
        lastRect.y += (lastRect.height - TIPSIZE) / 2;
        lastRect.width = TIPSIZE;
        lastRect.height = TIPSIZE;
        //GUI.DrawTextureWithTexCoords(lastRect, GetSelectedSprite().texture, GetSpriteNormalRect(GetSelectedSprite()));
        GUI.DrawTextureWithTexCoords(lastRect, GetSelectedSprite(enRotate.r0), new Rect(0, 0, 1, 1));
        EditorGUILayout.EndVertical();

    }

    //void DrawSelectedPalette()
    //{

    //	MapTipPalette nowPal = palette_[selectedPalette_];

    //	EditorGUILayout.BeginVertical();
    //	//GUILayout.FlexibleSpace();
    //	GUI.contentColor = Color.white;
    //	GUILayout.Box(dummy2_);
    //	Rect lastRect = GUILayoutUtility.GetLastRect();
    //	Rect wallRect = new Rect(lastRect.x + (TIPSIZE / 4), lastRect.y + (TIPSIZE / 4), TIPSIZE, TIPSIZE);
    //	Rect floorRect = new Rect(lastRect.x + (TIPSIZE / 4) + TIPSIZE, lastRect.y + (TIPSIZE / 4), TIPSIZE, TIPSIZE);
    //	Sprite wallsp = mapPaletteSprite_[nowPal.wallType_ * 2];
    //	Sprite floorsp = mapPaletteSprite_[(nowPal.floorType_ * 2) + 1];
    //	//GUI.DrawTextureWithTexCoords(lastRect, GetSelectedSprite().texture, GetSpriteNormalRect(GetSelectedSprite()));
    //	GUI.DrawTextureWithTexCoords(wallRect, wallsp.texture, GetSpriteNormalRect(wallsp));
    //	GUI.DrawTextureWithTexCoords(floorRect, floorsp.texture, GetSpriteNormalRect(floorsp));
    //	EditorGUILayout.EndVertical();
    //}

    // パレット変更ボタン
    void DrawPaletteChanger()
    {

        GUILayout.Label("Wall");
        EditorGUILayout.BeginHorizontal();
        //MapTipPalette nowPal = palette_.[selectedPalette_];

        int wallType = palette_.GetWall(selectedPalette_);
        int floorType = palette_.GetFloor(selectedPalette_);

        for (int i = 0; i < mapPaletteSprite_.Count / 2; i++)
        {
            Sprite wallsp = mapPaletteSprite_[i * 2];
            if (wallType == i)
            {
                GUI.contentColor = Color.magenta;
            }
            else
            {
                GUI.contentColor = Color.white;
            }

            if (GUILayout.Button(dummy3_, GUILayout.MaxWidth(TIPSIZE * 1.5f), GUILayout.MaxHeight(TIPSIZE * 1.5f), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
            {
                palette_.SetWall(selectedPalette_, i);
                subWindow_.SwitchShapePal();
            }
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect wallRect = new Rect(lastRect.x + (TIPSIZE / 4), lastRect.y + (TIPSIZE / 4), TIPSIZE, TIPSIZE);
            GUI.DrawTextureWithTexCoords(wallRect, wallsp.texture, GetSpriteNormalRect(wallsp));
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Floor");
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < mapPaletteSprite_.Count / 2; i++)
        {
            if (floorType == i)
            {
                GUI.contentColor = Color.magenta;
            }
            else
            {
                GUI.contentColor = Color.white;
            }

            Sprite floorsp = mapPaletteSprite_[(i * 2) + 1];
            if (GUILayout.Button(dummy3_, GUILayout.MaxWidth(TIPSIZE * 1.5f), GUILayout.MaxHeight(TIPSIZE * 1.5f), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
            {
                palette_.SetFloor(selectedPalette_, i);
                subWindow_.SwitchShapePal();
            }
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect floorRect = new Rect(lastRect.x + (TIPSIZE / 4), lastRect.y + (TIPSIZE / 4), TIPSIZE, TIPSIZE);
            GUI.DrawTextureWithTexCoords(floorRect, floorsp.texture, GetSpriteNormalRect(floorsp));
        }
        EditorGUILayout.EndHorizontal();
    }

    //リサイズボタン
    void DrawButtonResize()
    {
        EditorGUILayout.BeginVertical();
        //GUILayout.FlexibleSpace();
        if (GUILayout.Button("Resize"))
        {
            if ((mapSizeX_ != mapTips3_.mapSizeX)
                || (mapSizeY_ != mapTips3_.mapSizeY)
                || (mapSizeZ_ != mapTips3_.mapSizeZ))
            {
                if ((mapSizeX_ < mapTips3_.mapSizeX)
                    || (mapSizeY_ < mapTips3_.mapSizeY)
                    || (mapSizeZ_ < mapTips3_.mapSizeZ))
                {
                    if (EditorUtility.DisplayDialog("Resize", "サイズが小さくなるため、切り捨てが発生します。よろしいですか？", "ok", "cancel"))
                    {
                        ResizeMap();
                    }
                }
                else
                {
                    ResizeMap();
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    void DrawButtonReloadTipTex()
    {
        EditorGUILayout.BeginVertical();
        //GUILayout.FlexibleSpace();
        if (GUILayout.Button("ReloadTipTex"))
        {
            LoadMapTipSprite();
        }
        EditorGUILayout.EndVertical();
    }
    // マップウィンドウを開くボタンを生成
    void DrawButtonOpenMapEditor()
    {
        EditorGUILayout.BeginVertical();
        //GUILayout.FlexibleSpace();
        if (GUILayout.Button("OpenMapEditor"))
        {
            LoadPalette();
            LoadMap();
            OpenMapEditor();
        }
        EditorGUILayout.EndVertical();
    }

    void OpenMapEditor()
    {
        if (subWindow_ != null)subWindow_.Close();

        if (subWindow_ == null)
        {
            subWindow_ = MapEditor.WillAppear(this);
        }
        else
        {
            subWindow_.Focus();
        }
        subWindow_.Init();

        AssetDatabase.Refresh();
    }

    void DrawButtonNewMap()
    {
        EditorGUILayout.BeginVertical();
        //GUILayout.FlexibleSpace();
        if (GUILayout.Button("NewMap"))
        {
            // 完了ポップアップ
            if (EditorUtility.DisplayDialog("NewMap", "新規マップを作成しますか？", "ok", "cancel"))
            {
                NewMap();
                OpenMapEditor();
            }
        }
        EditorGUILayout.EndVertical();
    }

    void DrawButtonSaveLoad()
    {
        EditorGUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();
        //保存
        if (GUILayout.Button("SaveMap"))SaveMap();

        //読み込み
        if (GUILayout.Button("LoadMap"))LoadMap();

        EditorGUILayout.EndHorizontal();
    }

    void DrawButtonNewPal()
    {
        EditorGUILayout.BeginVertical();
        //GUILayout.FlexibleSpace();
        if (GUILayout.Button("NewPalette"))
        {
            // 完了ポップアップ
            if (EditorUtility.DisplayDialog("NewPalette", "新規パレットを作成しますか？", "ok", "cancel"))
            {
                NewPalette();
            }
        }
        EditorGUILayout.EndVertical();
    }

    void DrawButtonPalSaveLoad()
    {
        EditorGUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();
        //保存
        if (GUILayout.Button("SavePal"))SavePalette();

        //読み込み
        if (GUILayout.Button("LoadPal"))LoadPalette();

        EditorGUILayout.EndHorizontal();
    }
    void DrawButtonAddRmvPal()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("AddPalette"))palette_.Add();
        if (GUILayout.Button("RemovePalette"))palette_.Remove();
        EditorGUILayout.EndHorizontal();
    }

    void DrawButtonCreateObject()
    {
        EditorGUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();
        //オブジェクト作成
        if (GUILayout.Button("CreateObject"))CreateObject();

        EditorGUILayout.EndHorizontal();
    }
    void DrawButtonLoadShapeAndPal()
    {
        EditorGUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();
        //オブジェクト作成
        if (GUILayout.Button("LoadShapeAndPal"))
        {
            LoadMap();
            LoadPalette();
        }

        EditorGUILayout.EndHorizontal();
    }

    //入出力系///////////////////////////////
    string GetFilePath()
    {
        return "Assets/Resources/MapTips_" + mapId_.ToString("d3") + ".asset";
    }

    // ファイルで出力
    void SaveMap()
    {
        var savetips = mapTips3_.GetClone();

        AssetDatabase.CreateAsset(savetips, GetFilePath());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //// 完了ポップアップ
        //EditorUtility.DisplayDialog("SaveFile", "マップファイルを保存しました。", "ok");
    }
    void LoadMap()
    {

        // 完了ポップアップ
        //if (EditorUtility.DisplayDialog("LoadFile", "マップファイルを読み込みますか？", "ok", "cancel"))
        {
            var loadtips = AssetDatabase.LoadAssetAtPath<MapTips>(GetFilePath());
            if (loadtips != null)
            {
                mapTips3_ = loadtips.GetClone();
                mapSizeX_ = mapTips3_.mapSizeX;
                mapSizeY_ = mapTips3_.mapSizeY;
                mapSizeZ_ = mapTips3_.mapSizeZ;
            }
            else
            {
                EditorUtility.DisplayDialog("LoadFile", "読み込めませんでした。\n" + GetFilePath(), "ok");
            }

            //OpenMapEditor();
        }
        AssetDatabase.Refresh();

    }

    string GetPaletteFilePath()
    {
        return "Assets/Resources/MapPalette_" + palId_.ToString("d3") + ".asset";
    }
    // ファイルで出力
    void SavePalette()
    {
        var savePal = palette_.GetClone();
        AssetDatabase.CreateAsset(savePal, GetPaletteFilePath());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    void LoadPalette()
    {
        var loadPal = AssetDatabase.LoadAssetAtPath<MapPalette>(GetPaletteFilePath());
        if (loadPal != null)
        {
            palette_ = loadPal.GetClone();
        }
        else
        {
            EditorUtility.DisplayDialog("LoadFile", "ファイルが読み込めませんでした。\n" + GetPaletteFilePath(), "ok");
        }

        //OpenMapEditor();

        AssetDatabase.Refresh();

    }

    //オブジェクト作成
    void CreateObject()
    {
        if (objMaker_ == null)objMaker_ = new MapObjMaker();

        objMaker_.CreateObject(mapTips3_, mapId_, palette_);

        // 完了ポップアップ
        EditorUtility.DisplayDialog("CreateObject", "マップオブジェクトを生成しました。", "ok");

    }
}
