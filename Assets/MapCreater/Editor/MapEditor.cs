using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Map creater sub window.
/// </summary>
public class MapEditor : EditorWindow
{
    static readonly float WINDOW_W = 750.0f;
    static readonly float WINDOW_H = 750.0f;
    readonly int GRIDSIZE = 16;
    readonly int GRIDSIZE_Z = 8;
    readonly int TIPSIZE = 24;
    float mag = 2f;
    // グリッドの四角
    Rect[, ] gridRect_;
    // 親ウィンドウの参照を持つ
    MapCreater parent_;

    Vector2 camPos_ = new Vector2(64, 128);
    Vector2 mouseStPos_;
    Vector2Int cursorPos_;

    int selectedDepth_ = 0;
    enRotate camRotate_ = enRotate.r0;
    int penDepth_ = 0;

    Vector3Int copySt_;
    Vector3Int copyEd_;

    Vector3Int copyLT_;
    Vector3Int copyRB_;
    bool isRepaint_;

    bool isSeleting_;
    MapTips copyTips_;

    // サブウィンドウを開く
    public static MapEditor WillAppear(MapCreater _parent)
    {
        MapEditor window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor), false);
        window.Show();
        window.minSize = new Vector2(WINDOW_W, WINDOW_H);
        window.SetParent(_parent);
        window.Init();
        return window;
    }

    public void SetRepaint()
    {
        isRepaint_ = true;
    }

    public void SwitchShapePal()
    {
        copyTips_ = null;
        penDepth_ = 0;
        Repaint();
    }

    private void SetParent(MapCreater _parent)
    {
        parent_ = _parent;
    }

    private void Recording()
    {
        // Undoで戻る先を保存する.
        Undo.RecordObject(parent_, "maptips");
    }

    void OnGUI()
    {
        try
        {
            if (parent_ == null)
            {
                Close();
                return;
            }
            isRepaint_ = false;
            if (gridRect_ == null)Init();

            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int sd = EditorGUILayout.IntField("selectedDepth", selectedDepth_);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Vector2 cp = EditorGUILayout.Vector2Field("campos", camPos_);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            enRotate ro = (enRotate)EditorGUILayout.EnumPopup("rotate", camRotate_);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int pd = EditorGUILayout.IntField("penDepth", penDepth_);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                //// Undoで戻る先を保存する.
                //Recording();
                camPos_ = cp;
                // そのあと、変更を適用
                selectedDepth_ = sd;
                camRotate_ = ro;
                penDepth_ = pd;
            }

            //GUILayout.EndHorizontal();

            //入力系

            //カメラ位置移動
            MoveCamera();
            //ローテーション
            Rotation();
            //奥行き移動
            ChangeSelectedDepth();
            //ペン奥行き
            CangePenDepth();

            //範囲選択
            SelectTips();
            //スポイト
            Spuit();
            //チップを置く
            PutTip();

            UndoButton();

            //描画系

            //画像の描画
            DrawMapTip();
            //DrawPutTip();

            // 出力ボタン
            //DrawOutputButton();
            if (isRepaint_)Repaint();

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
        return ((camRotate_ == enRotate.r0) || (camRotate_ == enRotate.r180));
    }

    //マップの幅（回転反映）
    private int GetMapW()
    {
        if (IsFrontView())
        {
            return parent_.mapSizeX_;
        }
        else
        {
            return parent_.mapSizeZ_;
        }
    }
    //マップ奥行き（回転反映）
    private int GetMapD()
    {
        if (IsFrontView())
        {
            return parent_.mapSizeZ_;
        }
        else
        {
            return parent_.mapSizeX_;
        }
    }
    //マップの幅（回転反映）
    private int GetMapH()
    {
        return parent_.mapSizeY_;
    }
    private Vector3Int GetPosVector3(int x, int y)
    {
        return GetPosVector3(x, y, selectedDepth_);
    }
    private Vector3Int GetPosVector3(Vector3Int vec)
    {
        return GetPosVector3(vec.x, vec.y, vec.z);
    }
    private Vector3Int GetPosVector3(int x, int y, int z)
    {
        Vector3Int res = new Vector3Int(0, y, 0);
        switch (camRotate_)
        {
            case enRotate.r0:
                res.x = x;
                res.z = z;
                break;
            case enRotate.r90:
                res.x = GetMapD() - z - 1;
                res.z = x;
                break;
            case enRotate.r180:
                res.x = GetMapW() - x - 1;
                res.z = GetMapD() - z - 1;
                break;
            case enRotate.r270:
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
        gridRect_ = CreateGrid();
        SetRepaint();
    }

    // グリッドデータを生成
    private Rect[, ] CreateGrid()
    {

        float x = 0f;
        float y = 0f;

        Rect[, ] resultRects = new Rect[GetMapW(), GetMapH()];

        for (int yy = 0; yy < GetMapH(); yy++)
        {
            x = 0f;
            for (int xx = 0; xx < GetMapW(); xx++)
            {
                Vector2 pos = new Vector2(x, (y + GRIDSIZE_Z));
                Vector2 size = new Vector2(GRIDSIZE + 0.1f, GRIDSIZE + 0.1f);
                Rect r = new Rect(pos * mag, size * mag);
                resultRects[xx, yy] = r;
                x += GRIDSIZE;
            }
            y += GRIDSIZE;
        }

        return resultRects;
    }

    //マウスのグリッド座標
    private bool GetMouseGridPos(out Vector2Int resvec)
    {
        bool res = false;
        resvec = new Vector2Int(0, 0);

        Vector2 pos = Event.current.mousePosition - (camPos_ * mag);

        int xx = 0;
        for (xx = 0; xx < GetMapW(); xx++)
        {
            Rect r = gridRect_[xx, 0];
            if (r.x <= pos.x && pos.x <= r.x + r.width)
            {
                break;
            }
        }

        if (xx < GetMapW())
        {
            for (int yy = 0; yy < GetMapH(); yy++)
            {
                if (gridRect_[xx, yy].Contains(pos))
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

    //入力系///////////////////////////////

    private void UndoButton()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.Escape)
            {
                Undo.PerformUndo();
                //Repaint();
                SetRepaint();
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
            SetRepaint();

        }
    }
    //ペン深さ
    private void CangePenDepth()
    {
        Event e = Event.current;

        if (!e.shift && !isSeleting_)return; //シフト

        if (e.type == EventType.ScrollWheel)
        {
            if (Event.current.delta.y > 0)
            {
                if (penDepth_ > 0)penDepth_--;
            }
            else
            {
                if (penDepth_ < GetMapD())penDepth_++;
            }
            SetRepaint();
        }
    }
    //ローテーション変更
    private void Rotation()
    {
        Event e = Event.current;

        if (isSeleting_)return;
        if (!e.control)return;

        if (e.type == EventType.ScrollWheel)
        {

            if (Event.current.delta.y > 0)
            {
                if (camRotate_ == enRotate.r270)
                {
                    camRotate_ = enRotate.r0;
                }
                else
                {
                    camRotate_++;
                }
            }

            else
            {
                if (camRotate_ == enRotate.r0)
                {
                    camRotate_ = enRotate.r270;
                }
                else
                {
                    camRotate_--;
                }
            }
            SetRepaint();
        }
    }
    //奥行き変更
    private void ChangeSelectedDepth()
    {
        Event e = Event.current;

        if (isSeleting_)return;
        if (e.control)return;
        if (e.shift)return;

        if (e.type == EventType.ScrollWheel)
        {

            if (Event.current.delta.y > 0)
                selectedDepth_--;
            else
                selectedDepth_++;

            if (selectedDepth_ >= GetMapD())selectedDepth_ = (GetMapD() - 1);
            if (selectedDepth_ < 0)selectedDepth_ = 0;

            //parent.asdf = parent.selectedZ;
            SetRepaint();
        }
    }

    //範囲選択
    private void SelectTips()
    {
        if (parent_.IsPalMode())return;

        // クリックされた位置を探して、その場所に画像データを入れる
        Event e = Event.current;

        if ((e.button != 1)
            && !(isSeleting_ && (e.type == EventType.ScrollWheel)))
            return; //右クリック

        if ((e.type == EventType.MouseDown) //マウス押した
            || (e.type == EventType.MouseUp) //マウス離した
            || (e.type == EventType.MouseDrag) //ドラッグ
            || (e.type == EventType.ScrollWheel)) //ホイール
        {

            Vector2Int resvec;
            if (GetMouseGridPos(out resvec))
            {
                if (e.type == EventType.MouseDown)
                {
                    isSeleting_ = true;
                    copyTips_ = null;
                    copySt_ = new Vector3Int(resvec.x, resvec.y, 0);
                    copyEd_ = new Vector3Int(resvec.x, resvec.y, 0);
                }
                else if (e.type == EventType.MouseDrag)
                {
                    copyEd_ = new Vector3Int(resvec.x, resvec.y, 0);
                }
                else if (e.type == EventType.ScrollWheel)
                {
                    copyEd_ = new Vector3Int(resvec.x, resvec.y, 0);
                }
                else if (e.type == EventType.MouseUp)
                {
                    isSeleting_ = false;
                    copyEd_ = new Vector3Int(resvec.x, resvec.y, 0);
                }

                copyLT_ = new Vector3Int(0, 0, selectedDepth_);
                copyRB_ = new Vector3Int(0, 0, selectedDepth_ + penDepth_);

                //選択範囲の左上点と右下点
                if (copySt_.x < copyEd_.x)
                {
                    copyLT_.x = copySt_.x;
                    copyRB_.x = copyEd_.x;
                }
                else
                {
                    copyLT_.x = copyEd_.x;
                    copyRB_.x = copySt_.x;
                }

                if (copySt_.y < copyEd_.y)
                {
                    copyLT_.y = copySt_.y;
                    copyRB_.y = copyEd_.y;
                }
                else
                {
                    copyLT_.y = copyEd_.y;
                    copyRB_.y = copySt_.y;
                }

                if (copyRB_.z >= GetMapD())copyRB_.z = GetMapD() - 1;

                if (e.type == EventType.MouseUp)
                {
                    if (((copyRB_.x - copyLT_.x) >= 1) || ((copyRB_.y - copyLT_.y) >= 1))
                    {
                        Vector3Int size = GetPosVector3(copyRB_) - GetPosVector3(copyLT_);
                        size.x += 1;
                        size.y += 1;
                        size.z += 1;
                        Vector3Int st = new Vector3Int(copyLT_.x, copyLT_.y, copyLT_.z);
                        copyTips_ = parent_.GetCopyMapTip(GetPosVector3(st), size);
                    }
                }
            }
            else
            {
                //範囲外
                isSeleting_ = false;
                copyTips_ = null;
            }

            //入力があったときは再描画入れる
            SetRepaint();
        }
    }

    //スポイト
    private void Spuit()
    {
        Event e = Event.current;
        if (e.button != 1)return;

        if (e.type == EventType.MouseUp) //クリック
        {
            if (copySt_ == null)return;
            if (copyEd_ == null)return;
            if ((copySt_.x != copyEd_.x) || (copySt_.y != copyEd_.y))return;

            Vector2 pos = Event.current.mousePosition - (camPos_ * mag);
            int xx = 0;
            for (xx = 0; xx < GetMapW(); xx++)
            {
                Rect r = gridRect_[xx, 0];
                if (r.x <= pos.x && pos.x <= r.x + r.width)
                {
                    break;
                }
            }

            if (xx < GetMapW())
            {
                for (int yy = 0; yy < GetMapH(); yy++)
                {
                    if (gridRect_[xx, yy].Contains(pos))
                    {
                        int revyy = GetMapH() - yy - 1;
                        parent_.Spuit(GetPosVector3(xx, revyy), parent_.IsPalMode());
                        break;
                    }
                }
            }
        }

    }
    //チップを置く
    private void PutTip()
    {
        // クリックされた位置を探して、その場所に画像データを入れる
        Event e = Event.current;
        if (e.button != 0)return;
        if ((e.type == EventType.MouseDown) //クリック
            || (e.type == EventType.MouseDrag)) //ドラッグ
        {

            Vector2Int resvec;
            if (GetMouseGridPos(out resvec))
            {
                // Undoで戻る先を保存する.
                Recording();

                //範囲貼り付け
                if (copyTips_ != null)
                {
                    parent_.SetPasteMapTip(GetPosVector3(resvec.x, resvec.y, selectedDepth_), copyTips_);
                }
                else
                {
                    //ペンの深さ分
                    for (int zz = 0; zz < penDepth_ + 1; zz++)
                    {
                        if ((selectedDepth_ + zz) >= GetMapD())break;

                        if (parent_.IsPalMode())
                        {
                            parent_.SetMapPalette(GetPosVector3(resvec.x, resvec.y, selectedDepth_ + zz));
                        }
                        else
                        {
                            parent_.SetMapShape(GetPosVector3(resvec.x, resvec.y, selectedDepth_ + zz));
                        }
                    }
                }

                //入力があったときは再描画入れる
                SetRepaint();
            }
        }
    }

    //描画系///////////////////////////////

    private void DrawMapTip()
    {

        bool isPutTip = false;
        Texture putSprite = (parent_.IsPalMode())
            ? parent_.GetSprite(0, enRotate.r0) //とりあえず空白スプライト
            : parent_.GetSelectedSprite(camRotate_);

        if ((putSprite != null) && !isSeleting_)
        {
            Vector2Int resvec;
            if (GetMouseGridPos(out resvec))
            {
                //ペン表示
                isPutTip = true;
                if (cursorPos_ != resvec)
                {
                    cursorPos_ = resvec;
                    SetRepaint();
                }
            }
            //else
            //{
            //	if (cursorPos_ != null)
            //	{
            //		cursorPos_ = null;
            //		SetRepaint();
            //	}
            //}
        }

        // 選択した画像を描画する
        for (int z = 0; z < GetMapD(); z++)
        {
            int zz = (GetMapD() - z - 1); //奥から

            int zpos = ((zz - selectedDepth_) * GRIDSIZE_Z); //奥行きのオフセット
            bool blockDraw = true;
            //奥プレート描画
            if ((zz == (GetMapD() - 1)))DrawPlateFR(zpos + GRIDSIZE_Z);
            //選択面グリッド描画
            if (zz == selectedDepth_ + penDepth_)DrawGridLine2();

            if ((zz >= selectedDepth_) && (zz <= selectedDepth_ + penDepth_))
            {
                GUI.color = new Color(1f, 1f, 1f, 1f);
            }
            else if (zz < selectedDepth_)
            {
                const float COL = 0.6f;
                GUI.color = new Color(COL, COL, COL, 1f);
            }
            else
            {
                const float MINCOL = 0.4f;
                float col = 0.7f - (0.05f * Mathf.Abs(selectedDepth_ - zz));
                if (col < MINCOL)col = MINCOL;
                GUI.color = new Color(col, col, col, 1f);
            }

            if (blockDraw)
            {
                for (int y = 0; y < GetMapH(); y++)
                {
                    int yy = (GetMapH() - y - 1); //奥から

                    for (int xx = 0; xx < GetMapW(); xx++)
                    {
                        int revyy = (GetMapH() - yy - 1);
                        Vector2 pos = new Vector2((xx * GRIDSIZE) + zpos, (yy * GRIDSIZE) - zpos);
                        Vector2 size = new Vector2(TIPSIZE, TIPSIZE);
                        Rect drawRect = new Rect((pos + camPos_) * mag, size * mag);

                        //左端プレート
                        if ((xx == 0) && (y == 0) && (z == 0))
                        {
                            DrawPlateLR(pos + camPos_, true);
                        }

                        //ペン先描画判定
                        bool isPutTip2 = false;
                        Texture copysprite = null;

                        if (cursorPos_ != null)
                        {
                            if (copyTips_ != null)
                            {
                                Vector3Int copypos = new Vector3Int((xx - (int)cursorPos_.x), (revyy - (int)cursorPos_.y), (zz - selectedDepth_));
                                if (copyTips_.IsSafePos(copypos))
                                {
                                    copysprite = parent_.GetSprite(copyTips_[copypos, false], camRotate_);
                                }
                            }
                            else
                            {
                                if (isPutTip
                                    && (zz >= selectedDepth_)
                                    && (zz <= (penDepth_ + selectedDepth_)))
                                {
                                    Vector2Int pos2 = new Vector2Int(xx, revyy);
                                    isPutTip2 = (pos2 == cursorPos_); // (pos2.Equal(cursorPos_));
                                }
                            }
                        }

                        //選択範囲描画
                        if (copysprite != null)
                        {
                            Color tempcolor = GUI.color;
                            GUI.color = new Color(1f, 0.5f, 1f, 1f);
                            GUI.DrawTextureWithTexCoords(drawRect, copysprite, new Rect(0, 0, 1, 1)); //描画
                            GUI.color = tempcolor;
                        }
                        else if (isPutTip2)
                        {
                            Color tempcolor = GUI.color;
                            GUI.color = new Color(0f, 1f, 1f, 1f);
                            GUI.DrawTextureWithTexCoords(drawRect, putSprite, new Rect(0, 0, 1, 1)); //描画
                            GUI.color = tempcolor;
                        }
                        else
                        {

                            Texture sp = parent_.GetTipsSprite(GetPosVector3(xx, y, zz), camRotate_, true);

                            bool tipDraw = true;

                            if (cursorPos_ != null)
                            {
                                //カーソルより手前側は表示しない
                                tipDraw = (isPutTip)
                                    ? (xx < cursorPos_.x) || (zz >= (selectedDepth_))
                                    : (zz >= (selectedDepth_));
                            }
                            //else
                            //{
                            //	tipDraw = (zz >= (selectedDepth_));
                            //}

                            //カーソルより手前側は表示しない
                            float alp = (!tipDraw)
                                ? 0.1f
                                : 1f;

                            GUI.color = new Color(1f, 1f, 1f, alp);

                            //float alp = (!tipDraw) GUI.color = new Color(1f, 1f, 1f, 0.1f);
                            //選択範囲
                            if (isSeleting_)
                            {
                                bool betweenX = ((copyLT_.x <= xx) && (copyRB_.x >= xx));
                                bool betweenY = ((copyLT_.y <= revyy) && (copyRB_.y >= revyy));
                                bool betweenZ = ((copyLT_.z <= zz) && (copyRB_.z >= zz));
                                if ((copyLT_.x == xx) && betweenY && betweenZ)DrawTipPlate(pos + camPos_, enFace.fLeft);
                                if ((copyLT_.y == revyy) && betweenX && betweenZ)DrawTipPlate(pos + camPos_, enFace.fBottom);
                                if ((copyRB_.z == zz) && betweenX && betweenY)DrawTipPlate(pos + camPos_, enFace.fRear);

                                if (sp != null)
                                {
                                    if (betweenX && betweenY && betweenZ)
                                    {
                                        Color tmpCol = GUI.color;
                                        GUI.color = new Color(1f, 0f, 1f, 1f);
                                        GUI.DrawTextureWithTexCoords(drawRect, sp, new Rect(0, 0, 1, 1)); //描画
                                        GUI.color = tmpCol;
                                    }
                                    else
                                    {
                                        GUI.DrawTextureWithTexCoords(drawRect, sp, new Rect(0, 0, 1, 1)); //描画
                                    }
                                }
                                if ((copyRB_.x == xx) && betweenY && betweenZ)DrawTipPlate(pos + camPos_, enFace.fRight);
                                if ((copyRB_.y == revyy) && betweenX && betweenZ)DrawTipPlate(pos + camPos_, enFace.fTop);
                            }
                            else
                            {
                                if (sp != null)
                                {
                                    if (parent_.IsPalMode())
                                    {
                                        if (parent_.GetSelectedPal() != parent_.GetTipsPal(GetPosVector3(xx, y, zz)))
                                        {
                                            //GUI.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                                            GUI.color = new Color(0.5f, 1f, 0.5f, alp);
                                        }
                                    }
                                    GUI.DrawTextureWithTexCoords(drawRect, sp, new Rect(0, 0, 1, 1)); //描画
                                }
                            }

                        }
                    }
                }
            }

            //手前プレート描画
            if ((zz == 0))DrawPlateFR(zpos);

            //左端プレート
            if (zz == 0)
            {
                Vector2 pos = new Vector2((GetMapW() * GRIDSIZE) + zpos, (GetMapH() * GRIDSIZE) - zpos);
                DrawPlateLR(pos + camPos_, false);
            }
        }

        //色元に戻す
        GUI.color = new Color(1f, 1f, 1f, 1f);
    }

    // グリッド線を描画
    private void DrawGridLine2()
    {
        if (isSeleting_)return;

        if (cursorPos_ == null)return;

        // grid
        Handles.color = new Color(1f, 1f, 1f, 0.5f);
        float penpos = (penDepth_ * GRIDSIZE_Z);
        float fx = camPos_.x + penpos + GRIDSIZE_Z;
        float fy = camPos_.y - penpos;

        //縦線
        for (int i = 0; i < GetMapW() + 1; i++)
        {
            bool isPenX = (i == cursorPos_.x);

            Handles.color = isPenX
                ? new Color(1f, 1f, 1f, 1f)
                : new Color(1f, 1f, 1f, 0.5f);
            float x = fx + (i * GRIDSIZE);

            Vector2 st = new Vector2(x, fy);
            Vector2 ed = new Vector2(x, fy + (GetMapH() * GRIDSIZE));
            Handles.DrawLine(st * mag, ed * mag);
        }

        //横線
        for (int i = 0; i < GetMapH() + 1; i++)
        {
            bool isPenY = (i == GetMapH() - cursorPos_.y - 1);

            Handles.color = isPenY
                ? new Color(1f, 1f, 1f, 1f)
                : new Color(1f, 1f, 1f, 0.5f);
            float y = fy + (i * GRIDSIZE);
            Vector2 st = new Vector2(fx, y);
            Vector2 ed = new Vector2(fx + (GetMapW() * GRIDSIZE), y);
            Handles.DrawLine(st * mag, ed * mag);
        }
    }

    // 末端面を描画
    private void DrawPlateFR(int zpos)
    {
        // grid
        Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        float fx = zpos;
        float fy = GRIDSIZE_Z - zpos;
        Vector2 pos = new Vector2(fx, fy);
        Vector2 size = new Vector2((GetMapW() * GRIDSIZE), (GetMapH() * GRIDSIZE));
        Rect rect = new Rect((camPos_ + pos) * mag, size * mag);

        Handles.DrawSolidRectangleWithOutline(rect, Handles.color, Handles.color);
    }
    // 面を描画
    private void DrawPlateLR(Vector2 pos, bool isLeft)
    {

        //pos.y += GRIDSIZE_Z;
        int deplen = (GetMapD()) * GRIDSIZE_Z;
        pos.y -= (GetMapH() * GRIDSIZE);
        if (isLeft)
        {
            pos.x += -(deplen - GRIDSIZE_Z);
            pos.y += (deplen + GRIDSIZE);
        }
        else
        {
            pos.y += (GRIDSIZE_Z);
        }
        Vector2 revR = new Vector2(+deplen, -deplen);
        Vector2 revD = new Vector2(0, (GetMapH() * GRIDSIZE));

        Vector2 v0 = pos;
        Vector2 v1 = pos + revR;
        Vector2 v2 = pos + revR + revD;
        Vector2 v3 = pos + revD;

        Vector3[] verts = { v0 * mag, v1 * mag, v2 * mag, v3 * mag };
        Color tmpcol = Handles.color;
        Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);
        Handles.color = tmpcol;
    }

    // 面を描画
    private void DrawTipPlate(Vector2 pos, enFace face)
    {
        Vector2 revR = new Vector2();
        Vector2 revD = new Vector2();

        switch (face)
        {
            case enFace.fLeft:
                pos.y += GRIDSIZE_Z;
                revR = new Vector2(GRIDSIZE_Z, -GRIDSIZE_Z);
                revD = new Vector2(0, GRIDSIZE);
                break;
            case enFace.fRight:
                pos.x += GRIDSIZE;
                pos.y += GRIDSIZE_Z;
                revR = new Vector2(GRIDSIZE_Z, -GRIDSIZE_Z);
                revD = new Vector2(0, GRIDSIZE);
                break;
            case enFace.fTop:
                pos.x += GRIDSIZE_Z;
                revR = new Vector2(GRIDSIZE, 0);
                revD = new Vector2(-GRIDSIZE_Z, GRIDSIZE_Z);
                break;
            case enFace.fBottom:
                pos.x += GRIDSIZE_Z;
                pos.y += GRIDSIZE;
                revR = new Vector2(GRIDSIZE, 0);
                revD = new Vector2(-GRIDSIZE_Z, GRIDSIZE_Z);
                break;
            case enFace.fRear:
                pos.x += GRIDSIZE_Z;
                //pos.y -= GRIDSIZE_Z;
                revR = new Vector2(GRIDSIZE, 0);
                revD = new Vector2(0, GRIDSIZE);
                break;
            default:
                break;
        }

        Vector2 v0 = pos;
        Vector2 v1 = pos + revR;
        Vector2 v2 = pos + revR + revD;
        Vector2 v3 = pos + revD;

        Vector3[] verts = { v0 * mag, v1 * mag, v2 * mag, v3 * mag };
        Color tmpcol = Handles.color;
        Handles.color = new Color(1f, 1f, 1f, 0.5f);
        Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);
        Handles.color = tmpcol;
    }

}
