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
        CharaCellEditorMain parent_;

        Vector2 camPos_ = new Vector2(32, 64);
        Vector2 mouseStPos_;
        Vector2Int stFacePos_;

        Vector2 mouseRStPos_;
        int stFaceAngle_;
        int stFaceZ_;

        bool isRepaint_;

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
            parent_ = _parent;
        }

        private void Recording()
        {
            // Undoで戻る先を保存する.
            Undo.RecordObject(parent_, "characell");
        }

        void OnGUI()
        {
            mag = 8;

            if (parent_ == null)
            {
                Close();
                return;
            }
            isRepaint_ = false;

            //入力フォーム
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int faceNo = EditorGUILayout.IntSlider("faceNo", parent_.GetSelectedCharCell().faceNo, 0, parent_.GetKaoNoMax());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int faceAngle = EditorGUILayout.IntSlider("faceAngle", parent_.GetSelectedCharCell().faceAngle, 0, parent_.GetAngleMax());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int faceX = EditorGUILayout.IntSlider("faceX", parent_.GetSelectedCharCell().faceX, -MAXPOS, MAXPOS);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int faceY = EditorGUILayout.IntSlider("faceY", parent_.GetSelectedCharCell().faceY, -MAXPOS, MAXPOS);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int faceZ = EditorGUILayout.IntSlider("faceZ", parent_.GetSelectedCharCell().faceZ, 0, 1);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Vector2 cp = EditorGUILayout.Vector2Field("campos", camPos_);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                // Undoで戻る先を保存する.
                Recording();
                parent_.SetFaceNo(faceNo);
                parent_.SetFaceAngle(faceAngle);

                parent_.SetFaceX(faceX);
                parent_.SetFaceY(faceY);
                parent_.SetFaceZ(faceZ);
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

            if (isRepaint_)
                Repaint();
        }

        // サブウィンドウの初期化
        public void init()
        {
            wantsMouseMove = true; // マウス情報を取得.
            isRepaint_ = true;
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
                    isRepaint_ = true;
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
                mouseStPos_ = e.mousePosition;
            }
            else if (e.type == EventType.MouseDrag) ///e.button 0:左ボタン、1:右ボタン、2:中ボタン
            {
                Vector2 dist = (e.mousePosition - mouseStPos_);
                camPos_ += (dist / mag);
                mouseStPos_ = e.mousePosition;
                isRepaint_ = true;

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
                mouseStPos_ = Event.current.mousePosition;
                stFacePos_ = new Vector2Int(parent_.GetSelectedCharCell().faceX, parent_.GetSelectedCharCell().faceY);
            }

            if (e.type == EventType.MouseDrag) //ドラッグ
            {
                Vector2 mousePos = Event.current.mousePosition;

                // Undoで戻る先を保存する.
                Recording();

                int faceX = (int)(mousePos.x - mouseStPos_.x) / mag;
                int faceY = (int)(mousePos.y - mouseStPos_.y) / mag;

                parent_.SetFaceX(stFacePos_.x + faceX);
                parent_.SetFaceY(stFacePos_.y + faceY);
                //parent_.SetFaceZ(faceZ);

                //入力があったときは再描画入れる
                isRepaint_ = true;

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
                parent_.IncFaceNo(Event.current.delta.y > 0);
                //入力があったときは再描画入れる
                isRepaint_ = true;

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
                mouseRStPos_ = Event.current.mousePosition;
                stFaceAngle_ = parent_.GetSelectedCharCell().faceAngle;
                //stFaceZ_ = parent_.GetSelectedCharCell().faceZ;
            }

            if (e.type == EventType.MouseDrag) //ドラッグ
            {
                Vector2 mousePos = Event.current.mousePosition;

                // Undoで戻る先を保存する.
                Recording();

                int faceAngle = (int)(mousePos.x - mouseStPos_.x) / mag / 4;
                //int faceZ = (int)(mousePos.y - mouseStPos_.y) / mag / 4;

                parent_.SetFaceAngle(stFaceAngle_ + faceAngle);
                //parent_.SetFaceZ(stFaceAngle_ + faceZ);

                //入力があったときは再描画入れる
                isRepaint_ = true;

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
            float fx = camPos_.x + GRIDSIZE_Z;
            float fy = camPos_.y;

            //縦線
            {
                Vector2 st = new Vector2(0, -32);
                Vector2 ed = new Vector2(0, 32);
                Handles.DrawLine((camPos_ + st) * mag, (camPos_ + ed) * mag);
            }

            //横線
            {
                Vector2 st = new Vector2(-32, 0);
                Vector2 ed = new Vector2(32, 0);
                Handles.DrawLine((camPos_ + st) * mag, (camPos_ + ed) * mag);
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
                Sprite kao = parent_.GetSelectedKaoSprite();

                Vector2 kaopos = new Vector2(-kao.pivot.x + cell.faceX, cell.faceY + BASEY);
                kaopos += kaoRev;
                Vector2 size = new Vector2(kao.rect.width, kao.rect.height);
                Rect drawRect = new Rect((camPos_ + kaopos) * mag, size * mag);
                Vector2 rotatePivot = new Vector2(drawRect.center.x, drawRect.center.y - drawRect.height / 2);
                GUIUtility.RotateAroundPivot(angle, rotatePivot);
                // RotateAroundPivot等は行列の掛け算なので、一旦初期値に戻す

                GUI.DrawTextureWithTexCoords(drawRect, kao.texture, CharaCellEditorMain.GetSpriteNormalRect(kao)); //描画

                GUI.matrix = Matrix4x4.identity;
            }

            //ずら
            {
                Sprite zura = parent_.GetSelectedZuraSprite();
                Vector2 zurapos = new Vector2(-zura.pivot.x + cell.faceX, cell.faceY + BASEY);
                Vector2 size = new Vector2(zura.rect.width, zura.rect.height);
                Rect drawRect = new Rect((camPos_ + zurapos) * mag, size * mag);

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

            Sprite sp = parent_.GetSelectedSprite();
            Vector2 pos = new Vector2(-sp.pivot.x, +sp.pivot.y - sp.rect.height);
            CharaCell cell = parent_.GetSelectedCharCell();

            //顔奥表示
            if (cell.faceZ == 1)DrawFace(pos, cell);

            {

                //DebugPanel.Log("sp.rect", sp.rect);
                //Vector2 pos = new Vector2((1 * GRIDSIZE) + 0, (1 * GRIDSIZE) - 0);
                Vector2 size = new Vector2(sp.rect.width, sp.rect.height);
                Rect drawRect = new Rect((pos + camPos_) * mag, size * mag);
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
