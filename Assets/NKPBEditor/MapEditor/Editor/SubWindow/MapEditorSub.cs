﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKPB
{
    public partial class MapEditorSub : EditorWindow
    {
        readonly int GRID_SIZE = 16;
        readonly int GRID_SIZE_Z = 8;
        readonly int TIP_SIZE = 24;
        float m_mag = 2f;
        // グリッドの四角
        Rect[,] m_gridRect;
        // 親ウィンドウの参照を持つ
        MapEditorMain m_parent;

        Vector2 m_camPos = new Vector2(64, 128);
        Vector2 m_mouseStPos;
        Vector2Int m_cursorPos;

        int m_selectedDepth = 0;
        enRotate m_camRotate = enRotate.Front;
        int m_penDepth = 0;

        Vector3Int m_copySt;
        Vector3Int m_copyEd;

        Vector3Int m_copyLT;
        Vector3Int m_copyRB;
        bool m_isRepaint;

        bool m_isSelecting;
        enExpand m_expandVec;
        // MapTips m_copyTips;
        bool m_isSaveOk;

        // サブウィンドウを開く
        public static MapEditorSub WillAppear(MapEditorMain _parent)
        {
            MapEditorSub window = (MapEditorSub)EditorWindow.GetWindow(typeof(MapEditorSub), false);
            window.Show();
            window.minSize = new Vector2(MapEditorMain.WINDOW_W, MapEditorMain.WINDOW_H);
            window.SetParent(_parent);
            window.ResetUndoPerformed();
            window.Init();
            return window;
        }

        private void ResetUndoPerformed()
        {
            Undo.undoRedoPerformed -= MyUndoCallback;
            Undo.undoRedoPerformed += MyUndoCallback;
        }

        public void SetRepaint()
        {
            m_isRepaint = true;
        }

        private void SetParent(MapEditorMain _parent)
        {
            m_parent = _parent;
        }

        public void Recording()
        {
            // Undoで戻る先を保存する.
            // RecordObjectの引数はScriptableObjectのみ有効
            // その中の変数もPublicもしくは[SerializeField]がついたメンバのみ有効
            Undo.RecordObject(m_parent.m_mapTips, "maptips");
            m_isSaveOk = true;
        }

        void MyUndoCallback()
        {
            SetRepaint();
        }

        void OnGUI()
        {
            try
            {
                if (m_parent == null)
                {
                    Close();
                    return;
                }
                m_isRepaint = false;
                if (m_gridRect == null) Init();

                EditorGUI.BeginChangeCheck();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                int sd = EditorGUILayout.IntField("selectedDepth", m_selectedDepth);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                Vector2 cp = EditorGUILayout.Vector2Field("campos", m_camPos);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                enRotate ro = (enRotate)EditorGUILayout.EnumPopup("rotate", m_camRotate);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                int pd = EditorGUILayout.IntField("penDepth", m_penDepth);
                GUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    m_camPos = cp;
                    m_selectedDepth = sd;
                    m_camRotate = ro;
                    m_penDepth = pd;
                }

                //入力系
                Input();

                //描画系

                //画像の描画
                DrawMapTip();

                if (m_isRepaint)
                {
                    Repaint();
                    m_parent.RepaintFrontView();
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

        //前後面
        private bool IsFrontView()
        {
            return (m_camRotate == enRotate.Front);
        }

        //マップの幅（回転反映）
        private int GetMapW()
        {
            if (IsFrontView())
            {
                return m_parent.m_mapSizeX;
            }
            else
            {
                return m_parent.m_mapSizeZ;
            }
        }
        //マップ奥行き（回転反映）
        private int GetMapD()
        {
            if (IsFrontView())
            {
                return m_parent.m_mapSizeZ;
            }
            else
            {
                return m_parent.m_mapSizeX;
            }
        }
        //マップの幅（回転反映）
        private int GetMapH()
        {
            return m_parent.m_mapSizeY;
        }
        private Vector3Int GetPosVector3(int x, int y)
        {
            return GetPosVector3(x, y, m_selectedDepth + m_penDepth);
        }
        private Vector3Int GetPosVector3(Vector3Int vec)
        {
            return GetPosVector3(vec.x, vec.y, vec.z);
        }
        private Vector3Int GetPosVector3(int x, int y, int z)
        {
            Vector3Int res = new Vector3Int(0, y, 0);
            switch (m_camRotate)
            {
                case enRotate.Front:
                    res.x = x;
                    res.z = z;
                    break;
                case enRotate.Right:
                    res.x = GetMapD() - z - 1;
                    res.z = x;
                    break;
                // case enRotate.r180:
                //     res.x = GetMapW() - x - 1;
                //     res.z = GetMapD() - z - 1;
                //     break;
                case enRotate.Left:
                    res.x = z;
                    res.z = GetMapW() - x - 1;
                    break;
                default:
                    break;
            }
            return res;
        }

        // サブウィンドウの初期化
        public void Init()
        {
            wantsMouseMove = true; // マウス情報を取得.
                                   // グリッドデータを生成
            m_gridRect = CreateGrid();
            SetRepaint();
        }

        // グリッドデータを生成
        private Rect[,] CreateGrid()
        {

            float x = 0f;
            float y = 0f;

            Rect[,] resultRects = new Rect[GetMapW(), GetMapH()];

            for (int yy = 0; yy < GetMapH(); yy++)
            {
                x = 0f;
                for (int xx = 0; xx < GetMapW(); xx++)
                {
                    Vector2 pos = new Vector2(x, (y + GRID_SIZE_Z));
                    Vector2 size = new Vector2(GRID_SIZE + 0.1f, GRID_SIZE + 0.1f);
                    Rect r = new Rect(pos * m_mag, size * m_mag);
                    resultRects[xx, yy] = r;
                    x += GRID_SIZE;
                }
                y += GRID_SIZE;
            }

            return resultRects;
        }

        //マウスのグリッド座標
        private bool GetMouseGridPos(out Vector2Int resvec)
        {
            bool res = false;
            resvec = new Vector2Int(0, 0);

            Vector2 pos = Event.current.mousePosition - (m_camPos * m_mag);

            int xx = 0;
            for (xx = 0; xx < GetMapW(); xx++)
            {
                Rect r = m_gridRect[xx, 0];
                if (r.x <= pos.x && pos.x <= r.x + r.width)
                {
                    break;
                }
            }

            if (xx < GetMapW())
            {
                for (int yy = 0; yy < GetMapH(); yy++)
                {
                    if (m_gridRect[xx, yy].Contains(pos))
                    {
                        res = true;
                        resvec.x = xx;
                        resvec.y = GetMapH() - yy - 1;
                        break;
                    }
                }
            }

            return res;
        }

    }
}