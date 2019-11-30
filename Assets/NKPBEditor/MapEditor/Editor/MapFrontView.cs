using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKPB
{
    [Serializable]
    public class MapFrontView : EditorWindow
    {

        static readonly float WINDOW_W = 350.0f;
        static readonly float WINDOW_H = 350.0f;
        readonly int TIP_W = 8;
        readonly int TIP_H = 16;
        readonly int TIP_H_Harf = 8;
        readonly int DRAW_OFFSET = 16;

        MapEditorMain m_parent;
        List<Texture> m_mapShapeTexList;
        List<Sprite> m_mapPaletteSprites;
        bool m_isLoadSprite = false;
        bool m_isDrawMap = true;

        Texture2D m_texture;

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

        void SetParent(MapEditorMain _parent)
        {
            m_parent = _parent;
        }

        public void Init()
        {
            m_texture = new Texture2D(
                GetMapBmpSprite().texture.width,
                GetMapBmpSprite().texture.height, TextureFormat.ARGB32, false);

        }

        void OnGUI()
        {
            try
            {
                if (!m_isLoadSprite)
                    LoadMapTipSprite();

                m_isDrawMap = GUI.Toggle(new Rect(10, 0, 100, 30), m_isDrawMap, "MapDraw");

                if (GUI.Button(new Rect(100, 0, 100, 30), "Output"))
                {
                    m_texture = new Texture2D(
                         GetMapBmpSprite().texture.width,
                        GetMapBmpSprite().texture.height + DRAW_OFFSET,
                        TextureFormat.ARGB32, false);
                    CreateOutputTexture();
                    var png = m_texture.EncodeToPNG();
                    File.WriteAllBytes(MapEditorMain.OutputPath, png);
                    AssetDatabase.Refresh();
                }

                DrawMapTip();

                DrawMap();
            }
            catch (System.Exception exception)
            {
                if (exception is ExitGUIException)
                {
                    throw exception;
                }
                else
                {
                    Debug.LogError(exception.ToString());
                }
            }
        }

        //ロード
        void LoadMapTipSprite()
        {
            m_mapShapeTexList = new List<Texture>();
            m_mapShapeTexList = NonResources.LoadAll<Texture>(MapEditorMain.MapTipFrontImagePath);

            m_mapPaletteSprites = new List<Sprite>();
            m_mapPaletteSprites.AddRange(Resources.LoadAll<Sprite>(MapEditorMain.MapTileName));

            AssetDatabase.Refresh();

            m_isLoadSprite = true;
        }

        //マップ奥行き（回転反映）
        int GetMapD()
        {
            return m_parent.m_mapSizeZ;

        }
        //マップの幅（回転反映）
        int GetMapH()
        {
            return m_parent.m_mapSizeY;
        }

        int GetMapW()
        {
            return m_parent.m_mapSizeX;
        }

        Sprite GetMapBmpSprite()
        {
            return m_parent.GetMapBmpSprite();
        }

        void DrawMapTip()
        {
            Vector2 size = new Vector2(TIP_W, TIP_H);

            float baseCol = 0.8f;
            int grid = 4;
            for (int z = 0; z < GetMapD(); z++)
            {
                //奥から
                int zz = (GetMapD() - z - 1);
                float colGreen = (zz % grid == 0)
                    ? 1
                    : baseCol;

                for (int y = 0; y < GetMapH(); y++)
                {
                    float colBlue = baseCol + (((float)y / GetMapH()) * (1f - baseCol));

                    for (int x = 0; x < GetMapW(); x++)
                    {
                        float colRed = (x % grid == 0)
                            ? 1
                            : baseCol;

                        EnumShapeType shape = m_parent.m_mapTips.GetShape(new Vector3Int(x, y, zz));
                        if (shape == EnumShapeType.Empty)
                            continue;

                        Texture sp = m_mapShapeTexList[(int)shape];
                        Vector2 pos = new Vector2((x * TIP_W) + DRAW_OFFSET, (GetMapH() - 1 - y + z) * TIP_H_Harf + DRAW_OFFSET);
                        Rect drawRect = new Rect(pos, size);
                        // GUI.DrawTextureWithTexCoords(drawRect, sp, new Rect(0, 0, 1, 1)); //描画
                        GUI.DrawTexture(drawRect, sp, ScaleMode.StretchToFill, true, 1, new Color(colRed, colGreen, colBlue, 1), 0, 0); //描画

                    }
                }
            }
        }

        void CreateOutputTexture()
        {
            Texture2D boxTexture = (Texture2D)m_mapShapeTexList[(int)EnumShapeType.Box];
            Color[] groundPixels = boxTexture.GetPixels(0, TIP_H_Harf, TIP_W, TIP_H_Harf);
            Color[] wallPixels = boxTexture.GetPixels(0, 0, TIP_W, TIP_H_Harf);
            for (int i = 0; i < groundPixels.Length; i++)
            {
                groundPixels[i] = groundPixels[i] * 0.9f;
            }

            for (int i = 0; i < wallPixels.Length; i++)
            {
                wallPixels[i] = wallPixels[i] * 0.9f;
            }

            for (int y = 0; y < GetMapH(); y++)
            {
                for (int x = 0; x < GetMapW(); x++)
                {
                    Vector2 pos = new Vector2((x * TIP_W), (y + GetMapD() - 2) * TIP_H_Harf);
                    m_texture.SetPixels((int)pos.x, (int)pos.y, TIP_W, TIP_H_Harf, wallPixels, 0);
                }
            }

            for (int z = 0; z < GetMapD(); z++)
            {
                int zz = (GetMapD() - z - 1);
                for (int x = 0; x < GetMapW(); x++)
                {
                    Vector2 pos = new Vector2((x * TIP_W), (+zz) * TIP_H_Harf);
                    m_texture.SetPixels((int)pos.x, (int)pos.y, TIP_W, TIP_H_Harf, groundPixels, 0);
                }
            }

            for (int z = 0; z < GetMapD(); z++)
            {
                int zz = (GetMapD() - z - 1);
                for (int y = 0; y < GetMapH(); y++)
                {
                    for (int x = 0; x < GetMapW(); x++)
                    {
                        EnumShapeType shape = m_parent.m_mapTips.GetShape(new Vector3Int(x, y, zz));
                        if (shape == EnumShapeType.Empty)
                            continue;

                        Texture sp = m_mapShapeTexList[(int)shape];
                        Vector2 pos = new Vector2((x * TIP_W), (y + zz) * TIP_H_Harf);
                        Texture2D mainTexture = (Texture2D)sp;
                        Color[] pixels = mainTexture.GetPixels(); ;
                        m_texture.SetPixels((int)pos.x, (int)pos.y, TIP_W, TIP_H, pixels, 0);
                    }
                }
            }
        }

        void DrawMap()
        {
            if (!m_isDrawMap)
                return;

            Rect drawRect = GetMapBmpSprite().rect;
            drawRect.x += DRAW_OFFSET;
            drawRect.y += DRAW_OFFSET * 2;

            GUI.DrawTexture(drawRect, GetMapBmpSprite().texture, ScaleMode.StretchToFill, true, 1, new Color(1, 1, 1, 0.5f), 0, 0); //描画
        }
    }
}