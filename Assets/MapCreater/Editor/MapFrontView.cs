using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MapFrontView : EditorWindow
{

    static readonly float WINDOW_W = 350.0f;
    static readonly float WINDOW_H = 350.0f;
    readonly int TIP_W = 8;
    readonly int TIP_H = 16;
    readonly int TIP_H_Harf = 8;

    MapEditorMain m_parent;
    List<Texture> m_mapShapeTex;
    List<Sprite> m_mapPaletteSprite;
    Texture2D m_dummy;
    Texture2D m_dummy2;
    Texture2D m_dummy3;
    bool m_isLoadSprite = false;

    // サブウィンドウを開く
    public static MapFrontView WillAppear(MapEditorMain _parent)
    {
        MapFrontView window = (MapFrontView)EditorWindow.GetWindow(typeof(MapFrontView), false);
        window.Show();
        window.minSize = new Vector2(WINDOW_W, WINDOW_H);
        window.SetParent(_parent);
        window.Init();
        return window;
    }

    private void SetParent(MapEditorMain _parent)
    {
        m_parent = _parent;
    }

    public void Init()
    {
        // SetRepaint();
    }

    void OnGUI()
    {
        try
        {
            if (!m_isLoadSprite)
                LoadMapTipSprite();

            DrawMapTip();

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

    //ロード
    void LoadMapTipSprite()
    {
        m_mapShapeTex = new List<Texture>();
        m_mapShapeTex = NonResources.LoadAll<Texture>("Assets/MapCreater/Editor/MapTipFront");

        m_mapPaletteSprite = new List<Sprite>();
        m_mapPaletteSprite.AddRange(Resources.LoadAll<Sprite>("MapTile"));

        AssetDatabase.Refresh();

        m_isLoadSprite = true;
    }

    //マップ奥行き（回転反映）
    private int GetMapD()
    {
        return m_parent.m_mapSizeZ;

    }
    //マップの幅（回転反映）
    private int GetMapH()
    {
        return m_parent.m_mapSizeY;
    }

    private int GetMapW()
    {
        return m_parent.m_mapSizeX;
    }
    void DrawMapTip()
    {
        Vector2 size = new Vector2(TIP_W, TIP_H);

        for (int z = 0; z < GetMapD(); z++)
        {
            //奥から
            int zz = (GetMapD() - z - 1);
            for (int y = 0; y < GetMapH(); y++)
            {
                for (int x = 0; x < GetMapW(); x++)
                {
                    enShapeType shape = m_parent.m_mapTips.GetShape(new Vector3Int(x, y, zz));
                    if (shape == enShapeType.Empty)
                        continue;

                    Texture sp = m_mapShapeTex[(int)shape];
                    Vector2 pos = new Vector2((x * TIP_W) + TIP_W, (GetMapH() - y + z) * TIP_H_Harf);
                    Rect drawRect = new Rect(pos, size);
                    GUI.DrawTextureWithTexCoords(drawRect, sp, new Rect(0, 0, 1, 1)); //描画
                }
            }
        }
    }
}
