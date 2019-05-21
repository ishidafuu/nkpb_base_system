using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MapEditorMain : EditorWindow
{
    const int TIPSIZE = 48;
    // マップエディタのマスの数
    public int m_mapId = 0;
    public int m_palId = 0;
    public int m_mapSizeX = 10;
    public int m_mapSizeY = 10;
    public int m_mapSizeZ = 10;
    public MapTips m_mapTips;

    enShapeType m_selectedShape = enShapeType.Empty;
    int m_selectedEvent = 0;
    bool m_isLoadSprite = false;
    bool m_isLoadMapTip = false;

    List<Texture> m_mapShapeTex;
    List<Sprite> m_mapPaletteSprite;
    Sprite m_mapSprite;
    Texture2D m_dummy;
    Texture2D m_dummy2;
    Texture2D m_dummy3;
    MapEditor m_subWindow; // サブウィンドウ
    MapFrontView m_frontWindow; // フロントウィンドウ
    MapObjMaker m_objMaker;

    [UnityEditor.MenuItem("Window/MapEditorMain")]
    static void ShowMainWindow()
    {
        EditorWindow.GetWindow(typeof(MapEditorMain));
    }

    void OnGUI()
    {
        try
        {
            if (!m_isLoadSprite)
                LoadMapTipSprite();

            CreateDummyTexture();

            EditorGUI.BeginChangeCheck();

            GUIDrawButtonOpenMapEditor();
            GUILayout.Label($"MapID:{m_mapId} Size:[{m_mapSizeX},{m_mapSizeY},{m_mapSizeZ}]");
            GUIDrawButtonNewMap();
            GUIDrawButtonSaveLoad();

            // GUI
            GUILayout.BeginHorizontal();
            int mapId = EditorGUILayout.IntField("Map ID", m_mapId);
            GUILayout.EndHorizontal();

            GUIDrawMapSize(mapId);

            //オブジェクト作成
            GUIDrawButtonCreateObject();

            GUIDrawShapeTips();
            GUIDrawSelectedShape();

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

    private void GUIDrawMapSize(int mapId)
    {
        GUILayout.Label("Map Size");
        GUIDrawButtonResize();
        GUILayout.BeginVertical();
        int mapSizeX = EditorGUILayout.IntField("X", m_mapSizeX);
        int mapSizeY = EditorGUILayout.IntField("Y", m_mapSizeY);
        int mapSizeZ = EditorGUILayout.IntField("Z", m_mapSizeZ);
        GUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            m_mapId = Mathf.Clamp(mapId, 0, 999);
            m_mapSizeX = Mathf.Clamp(mapSizeX, 1, 99);
            m_mapSizeY = Mathf.Clamp(mapSizeY, 1, 99);
            m_mapSizeZ = Mathf.Clamp(mapSizeZ, 1, 99);
        }
    }

    void CreateDummyTexture()
    {
        if (m_dummy == null)
            m_dummy = new Texture2D(TIPSIZE, TIPSIZE);

        if (m_dummy2 == null)
            m_dummy2 = new Texture2D((int)(TIPSIZE * 2.5f), (int)(TIPSIZE * 1.5f));

        if (m_dummy3 == null)
            m_dummy3 = new Texture2D((int)(TIPSIZE * 1.5f), (int)(TIPSIZE * 1.5f));
    }

    //スプライトの取得
    public Texture GetSprite(enShapeType spriteNo, enRotate rotate)
    {
        // Debug.Log(rotate);
        return m_mapShapeTex[(int)spriteNo + ((m_mapShapeTex.Count / 4) * (int)rotate)];
    }

    public enShapeType GetSelectedShape()
    {
        return m_selectedShape;
    }

    public Texture GetTipsSprite(Vector3Int pos, enRotate rotate, bool isEmpNull = false)
    {
        if (m_mapTips == null)
            return null;

        if (isEmpNull && (m_mapTips.GetShape(pos) == 0))
            return null;

        return GetSprite(m_mapTips.GetShape(pos), rotate);
    }

    public Texture GetSelectedSprite(enRotate rotate)
    {
        return GetSprite(m_selectedShape, rotate);
    }

    public Sprite GetMapBmpSprite()
    {
        return m_mapSprite;
    }

    public void SetMapShape(enShapeType value, int x, int y, int z)
    {
        m_mapTips.SetShape(value, x, y, z);
    }
    public void SetMapShape(enShapeType value, Vector3Int pos)
    {
        m_mapTips.SetShape(value, pos);
    }

    public enShapeType GetMapShape(Vector3Int pos)
    {
        return m_mapTips.GetShape(pos);
    }
    // public void SetMapShape(Vector3Int pos)
    // {
    //     m_mapTips.SetShape(m_selectedShape, pos);
    // }

    public MapTips GetCopyMapTip(Vector3Int pos, Vector3Int size)
    {
        return m_mapTips.GetCopy(pos, size);
    }

    public void ShrinkX(int lineX, int length)
    {
        m_subWindow.Recording();
        m_mapTips.ShrinkX(lineX, length);
    }

    public void ExpandX(int lineX, int length)
    {
        m_subWindow.Recording();
        m_mapTips.ExpandX(lineX, length);
    }

    public void ShrinkY(int lineY, int length)
    {
        m_subWindow.Recording();
        m_mapTips.ShrinkY(lineY, length);
    }

    public void ExpandY(int lineY, int length)
    {
        m_subWindow.Recording();
        m_mapTips.ExpandY(lineY, length);
    }

    public void ShrinkZ(int lineZ, int length)
    {
        m_subWindow.Recording();
        Debug.Log("ShrinkZ lineZ" + lineZ + " length" + length);
        m_mapTips.ShrinkZ(lineZ, length);
    }

    public void ExpandZ(int lineZ, int length)
    {
        m_subWindow.Recording();
        Debug.Log("ExpandZ lineZ" + lineZ + " length" + length);
        m_mapTips.ExpandZ(lineZ, length);
    }

    public void SetPasteMapTip(Vector3Int pos, MapTips tips)
    {
        m_mapTips.SetPaste(pos, tips);
    }

    //スポイト
    public void Spuit(Vector3Int pos)
    {
        // m_selectedEvent = m_mapTips.GetEvent(pos);
        m_selectedShape = m_mapTips.GetShape(pos);

        Repaint();
    }

    //ロード
    void LoadMapTipSprite()
    {
        // 読み込み(Resources.LoadAllを使うのがミソ)
        m_objMaker = new MapObjMaker();
        m_dummy = new Texture2D(TIPSIZE, TIPSIZE);
        m_dummy2 = new Texture2D((int)(TIPSIZE * 2.5f), (int)(TIPSIZE * 1.5f));
        m_dummy3 = new Texture2D((int)(TIPSIZE * 1.5f), (int)(TIPSIZE * 1.5f));

        m_mapShapeTex = new List<Texture>();
        m_mapShapeTex = NonResources.LoadAll<Texture>("Assets/MapCreater/Editor/MapTipPalette");

        m_mapPaletteSprite = new List<Sprite>();
        m_mapPaletteSprite.AddRange(Resources.LoadAll<Sprite>("MapTile"));

        AssetDatabase.Refresh();

        m_isLoadSprite = true;
    }

    //マップ作成
    void CreateNewMap()
    {
        //palette_ = CreateInstance<MapPalette>();
        m_mapTips = CreateInstance<MapTips>();
        m_mapTips.Init(m_mapSizeX, m_mapSizeY, m_mapSizeZ);

        for (int x = 0; x < m_mapSizeX; ++x)
        {
            for (int y = 0; y < m_mapSizeY; ++y)
            {
                for (int z = 0; z < m_mapSizeZ; ++z)
                {
                    //mapTips2[x, y, z] = 0;

                    if ((y == 0) || (z == m_mapSizeZ - 1))
                        SetMapShape(enShapeType.Box, x, y, z);
                }
            }
        }
        m_isLoadMapTip = true;
    }

    //マップサイズ変更
    void ResizeMap()
    {
        MapTips newMapTips3 = CreateInstance<MapTips>();
        newMapTips3.Init(m_mapSizeX, m_mapSizeY, m_mapSizeZ);

        for (int x = 0; x < m_mapTips.mapSizeX; ++x)
        {
            if (x >= m_mapSizeX)
                continue;

            for (int y = 0; y < m_mapTips.mapSizeY; ++y)
            {
                if (y >= m_mapSizeY)
                    continue;

                for (int z = 0; z < m_mapTips.mapSizeZ; ++z)
                {
                    if (z >= m_mapSizeZ)
                        continue;

                    newMapTips3.SetEvent(m_mapTips.GetEvent(x, y, z), x, y, z);
                    newMapTips3.SetShape(m_mapTips.GetShape(x, y, z), x, y, z);
                }
            }
        }

        m_mapTips = newMapTips3;

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
    void GUIDrawShapeTips()
    {
        float x = 0.0f;
        float y = 00.0f;
        float maxW = TIPSIZE * 5;

        //EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        int index = 0;
        for (int i = 0; i < (m_mapShapeTex.Count / 4); ++i)
        {
            var sp = m_mapShapeTex[i];
            if (x >= maxW)
            {
                x = 0.0f;
                y += TIPSIZE;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            if (m_selectedShape == (enShapeType)index)
            {
                GUI.contentColor = Color.magenta;
            }
            else
            {
                GUI.contentColor = Color.white;
            }

            if (GUILayout.Button(m_dummy, GUILayout.MaxWidth(TIPSIZE), GUILayout.MaxHeight(TIPSIZE), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
            {
                m_selectedShape = (enShapeType)index;
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

    // 選択中の形状
    void GUIDrawSelectedShape()
    {

        //Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath (selectedImagePath, typeof(Texture2D));
        EditorGUILayout.BeginVertical();
        //GUILayout.FlexibleSpace();

        GUI.contentColor = Color.white;
        GUILayout.Box(m_dummy);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        lastRect.x += (lastRect.width - TIPSIZE) / 2;
        lastRect.y += (lastRect.height - TIPSIZE) / 2;
        lastRect.width = TIPSIZE;
        lastRect.height = TIPSIZE;
        //GUI.DrawTextureWithTexCoords(lastRect, GetSelectedSprite().texture, GetSpriteNormalRect(GetSelectedSprite()));
        GUI.DrawTextureWithTexCoords(lastRect, GetSelectedSprite(enRotate.Front), new Rect(0, 0, 1, 1));
        EditorGUILayout.EndVertical();

    }

    //リサイズボタン
    void GUIDrawButtonResize()
    {
        EditorGUILayout.BeginVertical();
        //GUILayout.FlexibleSpace();
        if (GUILayout.Button("Resize"))
        {
            if ((m_mapSizeX != m_mapTips.mapSizeX)
                || (m_mapSizeY != m_mapTips.mapSizeY)
                || (m_mapSizeZ != m_mapTips.mapSizeZ))
            {
                if ((m_mapSizeX < m_mapTips.mapSizeX)
                    || (m_mapSizeY < m_mapTips.mapSizeY)
                    || (m_mapSizeZ < m_mapTips.mapSizeZ))
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
        if (GUILayout.Button("ReloadTipTex"))
        {
            LoadMapTipSprite();
        }
        EditorGUILayout.EndVertical();
    }

    // マップウィンドウを開くボタンを生成
    void GUIDrawButtonOpenMapEditor()
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("OpenMapEditor"))
        {
            LoadMap();
            if (!m_isLoadMapTip)
            {
                CreateNewMap();
            }
            OpenMapEditor();
            OpenFrontView();
        }
        EditorGUILayout.EndVertical();
    }

    void OpenMapEditor()
    {
        if (m_subWindow != null)m_subWindow.Close();

        if (m_subWindow == null)
        {
            m_subWindow = MapEditor.WillAppear(this);
        }
        else
        {
            m_subWindow.Focus();
        }
        m_subWindow.Init();

        AssetDatabase.Refresh();
    }

    void OpenFrontView()
    {
        if (m_frontWindow != null)m_frontWindow.Close();

        if (m_frontWindow == null)
        {
            m_frontWindow = MapFrontView.WillAppear(this);
        }

        m_frontWindow.Init();

        AssetDatabase.Refresh();
    }

    void GUIDrawButtonNewMap()
    {
        EditorGUILayout.BeginVertical();
        //GUILayout.FlexibleSpace();
        if (GUILayout.Button("NewMap"))
        {
            // 完了ポップアップ
            if (EditorUtility.DisplayDialog("NewMap", "新規マップを作成しますか？", "ok", "cancel"))
            {
                CreateNewMap();
                OpenMapEditor();
            }
        }
        EditorGUILayout.EndVertical();
    }

    void GUIDrawButtonSaveLoad()
    {
        EditorGUILayout.BeginHorizontal();
        //保存
        if (GUILayout.Button("SaveMap"))
            SaveMap();

        //読み込み
        if (GUILayout.Button("LoadMap"))
            LoadMap();

        EditorGUILayout.EndHorizontal();
    }

    void GUIDrawButtonCreateObject()
    {
        EditorGUILayout.BeginHorizontal();
        //オブジェクト作成
        if (GUILayout.Button("CreateObject"))
            CreateObject();

        EditorGUILayout.EndHorizontal();
    }

    //入出力系///////////////////////////////
    string GetFilePath()
    {
        return "Assets/Resources/MapTips_" + m_mapId.ToString("d3") + ".asset";
    }

    // ファイルで出力
    public void SaveMap()
    {
        var savetips = m_mapTips.GetClone();
        AssetDatabase.CreateAsset(savetips, GetFilePath());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("SaveMap " + GetFilePath());

    }
    void LoadMap()
    {

        var loadtips = AssetDatabase.LoadAssetAtPath<MapTips>(GetFilePath());
        if (loadtips != null)
        {
            m_mapTips = loadtips.GetClone();
            m_mapSizeX = m_mapTips.mapSizeX;
            m_mapSizeY = m_mapTips.mapSizeY;
            m_mapSizeZ = m_mapTips.mapSizeZ;
            m_isLoadMapTip = true;
            m_mapSprite = Resources.Load<Sprite>("map" + m_mapId.ToString("d3"));
        }
        else
        {
            EditorUtility.DisplayDialog("LoadFile", "読み込めませんでした。\n" + GetFilePath(), "ok");
        }

        AssetDatabase.Refresh();

    }

    string GetPaletteFilePath()
    {
        return "Assets/Resources/MapPalette_" + m_palId.ToString("d3") + ".asset";
    }

    //オブジェクト作成
    void CreateObject()
    {
        if (m_objMaker == null)
            m_objMaker = new MapObjMaker();

        m_objMaker.CreateObject(m_mapTips, m_mapId);

        // 完了ポップアップ
        EditorUtility.DisplayDialog("CreateObject", "マップオブジェクトを生成しました。", "ok");

    }
    public void RepaintFrontView()
    {
        m_frontWindow.Repaint();
    }

}
