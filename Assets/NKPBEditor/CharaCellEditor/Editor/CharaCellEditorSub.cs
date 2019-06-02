using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKPB
{
    public class CharaCellEditorSub : EditorWindow
    {
        enum enRotate
        {
            r0,
            r90,
            r180,
            r270,
        }

        enum enFace
        {
            fLeft,
            fRight,
            fTop,
            fBottom,
            fRear,
        }

        const int GRIDSIZE = 16;
        const int GRIDSIZE_Z = 8;
        const int TIPSIZE = 24;
        const int MAXPOS = 32;
        int mag = 8;

        // グリッドの四角
        // 親ウィンドウの参照を持つ
        CharaCellEditorMain m_parent;

        Vector2 m_camPos = new Vector2(32, 64);
        Vector2 m_mouseStPos;
        Vector2Int m_stFacePos;

        Vector2 m_mouseRStPos;
        int m_stFaceAngle;
        int m_stFaceZ;

        bool m_isRepaint;

        // サブウィンドウを開く
        public static CharaCellEditorSub WillAppear(CharaCellEditorMain _parent)
        {
            CharaCellEditorSub window = (CharaCellEditorSub)EditorWindow.GetWindow(typeof(CharaCellEditorSub), false);
            window.Show();
            //window.minSize = new Vector2(WINDOW_W, WINDOW_H);
            window.SetParent(_parent);
            window.init();
            return window;
        }

        private void SetParent(CharaCellEditorMain _parent)
        {
            m_parent = _parent;
        }

        private void Recording()
        {
            // Undoで戻る先を保存する.
            m_parent.Recording();
        }

        void OnGUI()
        {
            mag = 8;

            if (m_parent == null)
            {
                Close();
                return;
            }
            m_isRepaint = false;

            //入力フォーム
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int faceNo = EditorGUILayout.IntSlider("faceNo", m_parent.GetSelectedCharCell().faceNo, 0, m_parent.GetKaoNoMax());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int faceAngle = EditorGUILayout.IntSlider("faceAngle", m_parent.GetSelectedCharCell().faceAngle, 0, m_parent.GetAngleMax());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int faceX = EditorGUILayout.IntSlider("faceX", m_parent.GetSelectedCharCell().faceX, -MAXPOS, MAXPOS);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int faceY = EditorGUILayout.IntSlider("faceY", m_parent.GetSelectedCharCell().faceY, -MAXPOS, MAXPOS);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int faceZ = EditorGUILayout.IntSlider("faceZ", m_parent.GetSelectedCharCell().faceZ, 0, 1);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Vector2 cp = EditorGUILayout.Vector2Field("campos", m_camPos);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                // Undoで戻る先を保存する.
                Recording();
                m_parent.SetFaceNo(faceNo);
                m_parent.SetFaceAngle(faceAngle);

                m_parent.SetFaceX(faceX);
                m_parent.SetFaceY(faceY);
                m_parent.SetFaceZ(faceZ);
            }

            //GUILayout.EndHorizontal();

            //入力系

            //顔番号
            //顔プライオリティ
            //顔角度

            //カメラ位置移動
            MoveCamera();

            UndoButton();
            //顔位置移動
            MoveFacePos();
            //顔番号
            ChangeFaceNo();
            //顔角度
            ChangeFaceAngle();

            //描画系
            DrawChar();

            if (m_isRepaint)
                Repaint();
        }

        // サブウィンドウの初期化
        public void init()
        {
            wantsMouseMove = true; // マウス情報を取得.
            m_isRepaint = true;
        }

        //入力系///////////////////////////////

        private void UndoButton()
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Escape)
                {
                    Undo.PerformUndo();
                    m_isRepaint = true;
                }
            }
        }
        //カメラ視点移動
        private void MoveCamera()
        {
            Event e = Event.current;
            if (e.button != 2)return;
            if (e.type == EventType.MouseDown)
            {
                m_mouseStPos = e.mousePosition;
            }
            else if (e.type == EventType.MouseDrag) ///e.button 0:左ボタン、1:右ボタン、2:中ボタン
            {
                Vector2 dist = (e.mousePosition - m_mouseStPos);
                m_camPos += (dist / mag);
                m_mouseStPos = e.mousePosition;
                m_isRepaint = true;

            }
        }

        //顔位置移動
        private void MoveFacePos()
        {
            // クリックされた位置を探して、その場所に画像データを入れる
            Event e = Event.current;
            if (e.button != 0)return;

            if (e.type == EventType.MouseDown) //クリック
            {
                m_mouseStPos = Event.current.mousePosition;
                m_stFacePos = new Vector2Int(m_parent.GetSelectedCharCell().faceX, m_parent.GetSelectedCharCell().faceY);
            }

            if (e.type == EventType.MouseDrag) //ドラッグ
            {
                Vector2 mousePos = Event.current.mousePosition;

                // Undoで戻る先を保存する.
                Recording();

                int faceX = (int)(mousePos.x - m_mouseStPos.x) / mag;
                int faceY = (int)(mousePos.y - m_mouseStPos.y) / mag;

                m_parent.SetFaceX(m_stFacePos.x + faceX);
                m_parent.SetFaceY(m_stFacePos.y + faceY);
                //parent_.SetFaceZ(faceZ);

                //入力があったときは再描画入れる
                m_isRepaint = true;

            }
        }

        //顔位置移動
        private void ChangeFaceNo()
        {
            // クリックされた位置を探して、その場所に画像データを入れる
            Event e = Event.current;

            //ホイール
            if ((e.type != EventType.MouseDown) && (e.type == EventType.ScrollWheel) && (Event.current.delta.y != 0))
            {
                // Undoで戻る先を保存する.
                Recording();
                m_parent.IncFaceNo(Event.current.delta.y > 0);
                //入力があったときは再描画入れる
                m_isRepaint = true;

            }
        }

        //顔角度変更
        private void ChangeFaceAngle()
        {
            // クリックされた位置を探して、その場所に画像データを入れる
            Event e = Event.current;
            if (e.button != 1)return;

            if (e.type == EventType.MouseDown) //クリック
            {
                m_mouseRStPos = Event.current.mousePosition;
                m_stFaceAngle = m_parent.GetSelectedCharCell().faceAngle;
                //stFaceZ_ = parent_.GetSelectedCharCell().faceZ;
            }

            if (e.type == EventType.MouseDrag) //ドラッグ
            {
                Vector2 mousePos = Event.current.mousePosition;

                // Undoで戻る先を保存する.
                Recording();

                int faceAngle = (int)(mousePos.x - m_mouseStPos.x) / mag / 4;
                //int faceZ = (int)(mousePos.y - mouseStPos_.y) / mag / 4;

                m_parent.SetFaceAngle(m_stFaceAngle + faceAngle);
                //parent_.SetFaceZ(stFaceAngle_ + faceZ);

                //入力があったときは再描画入れる
                m_isRepaint = true;

            }
        }

        //描画系///////////////////////////////
        // グリッド線を描画
        private void DrawGridLine3()
        {
            //if (isSeleting_) return;

            //if (cursorPos_ == null) return;

            // grid
            Handles.color = new Color(1f, 1f, 1f, 0.5f);
            //float penpos = (penDepth_ * GRIDSIZE_Z);
            float fx = m_camPos.x + GRIDSIZE_Z;
            float fy = m_camPos.y;

            //縦線
            {
                Vector2 st = new Vector2(0, -32);
                Vector2 ed = new Vector2(0, 32);
                Handles.DrawLine((m_camPos + st) * mag, (m_camPos + ed) * mag);
            }

            //横線
            {
                Vector2 st = new Vector2(-32, 0);
                Vector2 ed = new Vector2(32, 0);
                Handles.DrawLine((m_camPos + st) * mag, (m_camPos + ed) * mag);
            }
        }

        //顔描画
        private void DrawFace(Vector2 pos, CharaCell cell)
        {
            const int BASEY = -32;
            float angle = cell.faceAngle * 90;
            Vector2 kaoRev = Vector2.zero;
            switch (cell.faceAngle)
            {
                case 0:
                    angle = 0;
                    kaoRev.y += 1;
                    break;
                case 1:
                    kaoRev.x -= 1;
                    break;
                case 2:
                    kaoRev.y -= 1;
                    break;
                case 3:
                    kaoRev.x += 1;
                    break;
            }

            //顔
            {
                Sprite kao = m_parent.GetSelectedKaoSprite();

                Vector2 kaopos = new Vector2(-kao.pivot.x + cell.faceX, cell.faceY + BASEY);
                kaopos += kaoRev;
                Vector2 size = new Vector2(kao.rect.width, kao.rect.height);
                Rect drawRect = new Rect((m_camPos + kaopos) * mag, size * mag);
                Vector2 rotatePivot = new Vector2(drawRect.center.x, drawRect.center.y - drawRect.height / 2);
                GUIUtility.RotateAroundPivot(angle, rotatePivot);
                // RotateAroundPivot等は行列の掛け算なので、一旦初期値に戻す

                GUI.DrawTextureWithTexCoords(drawRect, kao.texture, CharaCellEditorMain.GetSpriteNormalRect(kao)); //描画

                GUI.matrix = Matrix4x4.identity;
            }

            //ずら
            {
                Sprite zura = m_parent.GetSelectedZuraSprite();
                Vector2 zurapos = new Vector2(-zura.pivot.x + cell.faceX, cell.faceY + BASEY);
                Vector2 size = new Vector2(zura.rect.width, zura.rect.height);
                Rect drawRect = new Rect((m_camPos + zurapos) * mag, size * mag);

                Vector2 rotatePivot = new Vector2(drawRect.center.x, drawRect.center.y - drawRect.height / 2);
                GUIUtility.RotateAroundPivot(angle, rotatePivot);
                // RotateAroundPivot等は行列の掛け算なので、一旦初期値に戻す

                GUI.DrawTextureWithTexCoords(drawRect, zura.texture, CharaCellEditorMain.GetSpriteNormalRect(zura)); //描画

                GUI.matrix = Matrix4x4.identity;
            }

        }

        //キャラ描画
        private void DrawChar()
        {

            Sprite sp = m_parent.GetSelectedSprite();
            Vector2 pos = new Vector2(-sp.pivot.x, +sp.pivot.y - sp.rect.height);
            CharaCell cell = m_parent.GetSelectedCharCell();

            //顔奥表示
            if (cell.faceZ == 1)DrawFace(pos, cell);
            {
                //DebugPanel.Log("sp.rect", sp.rect);
                //Vector2 pos = new Vector2((1 * GRIDSIZE) + 0, (1 * GRIDSIZE) - 0);
                Vector2 size = new Vector2(sp.rect.width, sp.rect.height);
                Rect drawRect = new Rect((pos + m_camPos) * mag, size * mag);
                //DebugPanel.Log("drawRect", drawRect);
                //DebugPanel.Log("sp.pivot.", sp.pivot);
                if (sp != null)
                {
                    GUI.DrawTextureWithTexCoords(drawRect, sp.texture, CharaCellEditorMain.GetSpriteNormalRect(sp)); //描画
                }
            }

            //顔手前表示
            if (cell.faceZ == 0)DrawFace(pos, cell);

            DrawGridLine3();

        }

    }
}
