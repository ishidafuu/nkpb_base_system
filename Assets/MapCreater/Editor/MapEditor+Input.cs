using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Map creater sub window.
/// </summary>
public partial class MapEditor : EditorWindow
{
    //入力系///////////////////////////////

    void Input()
    {
        //保存
        SaveMap();

        //カメラ位置移動
        MoveCamera();
        //ローテーション
        Rotation();
        //奥行き移動
        ChangeSelectedDepth();
        // ペン奥行き
        CangePenDepth();

        //伸び縮み
        ExtendTips();
        //スポイト
        Spuit();
        //チップを置く
        PutTip();
    }

    //保存
    void SaveMap()
    {
        Event e = Event.current;
        if (m_isSaveOk
            && e.type == EventType.KeyDown
            && e.keyCode == KeyCode.S
            && e.control)
        {
            m_parent.SaveMap();
            m_isSaveOk = false;
        }
    }

    //カメラ視点移動
    void MoveCamera()
    {
        Event e = Event.current;
        if (e.button != 2)
            return;

        if (e.type == EventType.MouseDown)
        {
            m_mouseStPos = e.mousePosition;
        }
        else if (e.type == EventType.MouseDrag) ///e.button 0:左ボタン、1:右ボタン、2:中ボタン
        {
            Vector2 dist = (e.mousePosition - m_mouseStPos);
            m_camPos += (dist / m_mag);
            m_mouseStPos = e.mousePosition;
            SetRepaint();
        }
    }

    //ペン深さ
    void CangePenDepth()
    {
        Event e = Event.current;

        if (!e.shift && !m_isSeleting)
            return; //シフト

        if (e.type == EventType.ScrollWheel)
        {
            if (Event.current.delta.y > 0)
            {
                if (m_penDepth > 0)
                    m_penDepth--;
            }
            else
            {
                if (m_penDepth < GetMapD() - 1)
                    m_penDepth++;
            }
            SetRepaint();
        }
    }

    //ローテーション変更
    void Rotation()
    {
        Event e = Event.current;

        if (m_isSeleting)return;
        if (!e.control)return;

        if (e.type == EventType.ScrollWheel)
        {

            if (Event.current.delta.y < 0)
            {
                switch (m_camRotate)
                {
                    case enRotate.Front:
                        m_camRotate = enRotate.Right;
                        m_camPos = Vector2.zero;
                        break;
                    case enRotate.Right:
                        // 一周しないように
                        // m_camRotate = enRotate.Left;
                        break;
                    case enRotate.Left:
                        m_camRotate = enRotate.Front;
                        m_camPos = Vector2.zero;
                        break;
                }
            }

            else
            {
                switch (m_camRotate)
                {
                    case enRotate.Front:
                        m_camRotate = enRotate.Left;
                        m_camPos = Vector2.zero;
                        break;
                    case enRotate.Right:
                        m_camRotate = enRotate.Front;
                        m_camPos = Vector2.zero;
                        break;
                    case enRotate.Left:
                        // 一周しないように
                        // m_camRotate = enRotate.Right;
                        break;
                }
            }
            SetRepaint();
        }
    }

    //奥行き変更
    void ChangeSelectedDepth()
    {
        Event e = Event.current;

        if (m_isSeleting)return;
        if (e.control)return;
        if (e.shift)return;

        if (e.type == EventType.ScrollWheel)
        {

            if (Event.current.delta.y > 0)
                m_selectedDepth--;
            else
                m_selectedDepth++;

            if (m_selectedDepth >= GetMapD())
                m_selectedDepth = (GetMapD() - 1);

            if (m_selectedDepth < 0)
                m_selectedDepth = 0;

            SetRepaint();
        }
    }

    //範囲選択
    void ExtendTips()
    {
        // クリックされた位置を探して、その場所に画像データを入れる
        Event e = Event.current;

        if ((e.button != 1)
            && !(m_isSeleting && (e.type == EventType.ScrollWheel)))
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
                    m_isSeleting = true;
                    // m_copyTips = null;
                    m_copySt = new Vector3Int(resvec.x, resvec.y, 0);
                    m_expandVec = enExpand.None;
                }
                else if (e.type == EventType.MouseDrag)
                {
                    if (m_expandVec == enExpand.None)
                    {
                        if (m_copySt.x != resvec.x
                            || m_copySt.y != resvec.y)
                        {
                            if (Mathf.Abs(m_copySt.x - resvec.x) < Mathf.Abs(m_copySt.y - resvec.y))
                            {
                                m_expandVec = enExpand.Vertical;
                                Debug.Log(m_expandVec);
                            }
                            else
                            {
                                m_expandVec = enExpand.Horizontal;
                            }
                        }

                    }
                }
                else if (e.type == EventType.ScrollWheel) {}
                else if (e.type == EventType.MouseUp)
                {
                    m_isSeleting = false;
                }

                m_copyEd = new Vector3Int(resvec.x, resvec.y, 0);

                m_copyLT = new Vector3Int(0, 0, m_selectedDepth);
                m_copyRB = new Vector3Int(0, 0, m_selectedDepth + m_penDepth);

                //選択範囲の左上点と右下点
                if (m_copySt.x < m_copyEd.x)
                {
                    m_copyLT.x = m_copySt.x;
                    m_copyRB.x = m_copyEd.x;
                }
                else
                {
                    m_copyLT.x = m_copyEd.x;
                    m_copyRB.x = m_copySt.x;
                }

                if (m_copySt.y < m_copyEd.y)
                {
                    m_copyLT.y = m_copySt.y;
                    m_copyRB.y = m_copyEd.y;
                }
                else
                {
                    m_copyLT.y = m_copyEd.y;
                    m_copyRB.y = m_copySt.y;
                }

                if (m_copyRB.z >= GetMapD())
                    m_copyRB.z = GetMapD() - 1;

                Extend(e);

            }
            else
            {
                //範囲外
                m_isSeleting = false;
                // m_copyTips = null;
            }

            //入力があったときは再描画入れる
            SetRepaint();
        }
    }

    private void Extend(Event e)
    {
        if (e.type == EventType.MouseUp)
        {
            // 横方向
            if (m_expandVec == enExpand.Horizontal)
            {
                int length = Mathf.Abs(m_copySt.x - m_copyEd.x);
                switch (m_camRotate)
                {
                    case enRotate.Front:
                        if (m_copyEd.x < m_copySt.x)
                        {
                            m_parent.ShrinkX(m_copyEd.x, length);
                        }
                        else if (m_copyEd.x > m_copySt.x)
                        {
                            m_parent.ExpandX(m_copySt.x, length);
                        }
                        break;
                    case enRotate.Left:
                        if (m_copyEd.x < m_copySt.x)
                        {
                            m_parent.ExpandZ(m_copySt.x, length);
                        }
                        else if (m_copyEd.x > m_copySt.x)
                        {
                            m_parent.ShrinkZ(m_copyEd.x, length);
                        }
                        break;
                    case enRotate.Right:
                        if (m_copyEd.x < m_copySt.x)
                        {
                            m_parent.ShrinkZ(m_copyEd.x, length);
                        }
                        else if (m_copyEd.x > m_copySt.x)
                        {
                            m_parent.ExpandZ(m_copySt.x, length);
                        }
                        break;
                }

            }
            else if (m_expandVec == enExpand.Vertical)
            {
                // 縦方向
                int length = Mathf.Abs(m_copySt.y - m_copyEd.y);
                if (m_copyEd.y < m_copySt.y)
                {
                    m_parent.ShrinkY(m_copyEd.y, length);
                }
                else if (m_copyEd.y > m_copySt.y)
                {
                    m_parent.ExpandY(m_copySt.y, length);

                }
            }

            m_expandVec = enExpand.None;
        }
    }

    //スポイト
    void Spuit()
    {
        Event e = Event.current;
        if (e.button != 1)return;

        if (e.type == EventType.MouseUp) //クリック
        {
            if (m_copySt == null)return;
            if (m_copyEd == null)return;
            if ((m_copySt.x != m_copyEd.x) || (m_copySt.y != m_copyEd.y))return;

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
                        int revyy = GetMapH() - yy - 1;
                        m_parent.Spuit(GetPosVector3(xx, revyy));
                        break;
                    }
                }
            }
        }

    }

    //チップを置く
    void PutTip()
    {
        // クリックされた位置を探して、その場所に画像データを入れる
        Event e = Event.current;
        if (e.button != 0)
            return;

        if ((e.type == EventType.MouseDown) //クリック
            || (e.type == EventType.MouseDrag)) //ドラッグ
        {

            Vector2Int resvec;
            if (GetMouseGridPos(out resvec))
            {
                // Undoで戻る先を保存する.
                Recording();

                PutSingle(resvec);

                //入力があったときは再描画入れる
                SetRepaint();
            }
        }
    }

    private void PutSingle(Vector2Int resvec)
    {
        enShapeType shape = m_parent.GetSelectedShape();
        int maxDepth = (m_camRotate == enRotate.Front)
            ? GetMapD()
            : GetMapW();

        int selectedDepth = (m_camRotate == enRotate.Front)
            ? m_selectedDepth + m_penDepth
            : resvec.x + m_penDepth;

        for (int index = 0; index < maxDepth; index++)
        {
            Vector3Int pos = Vector3Int.zero;
            bool isFrontFill = false;
            switch (m_camRotate)
            {
                case enRotate.Front:
                    pos = GetPosVector3(resvec.x, resvec.y, index);
                    isFrontFill = (index < selectedDepth);
                    break;
                case enRotate.Right:
                    pos = GetPosVector3(index, resvec.y, m_selectedDepth);
                    isFrontFill = (index < selectedDepth);
                    break;
                case enRotate.Left:
                    pos = GetPosVector3(index, resvec.y, m_selectedDepth);
                    isFrontFill = (index > selectedDepth);
                    break;
            }

            enShapeType drawShape = shape;
            bool isChanged = false;

            // 指定深さにはそのまま描画
            if (index == selectedDepth)
            {
                m_parent.SetMapShape(drawShape, pos);
                isChanged = true;
            }
            else if (isFrontFill)
            {
                // 手前
                // 置いたブロックより手前はすべて空に
                m_parent.SetMapShape(enShapeType.Empty, pos);
                isChanged = true;
            }
            else
            {
                // 奥
                // 奥をすべて描画チップに置き換える（箱チップであれば置き換えない
                // 描画チップが空チップの場合は置き換えない
                if (shape != enShapeType.Empty
                    && m_parent.GetMapShape(pos) != enShapeType.Box)
                {
                    if (shape == enShapeType.SlashWall || shape == enShapeType.BSlashWall)
                    {
                        drawShape = enShapeType.Box;
                    }

                    m_parent.SetMapShape(drawShape, pos);
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                AutoFixSideTip(pos, drawShape, true);
                AutoFixSideTip(pos, drawShape, false);
                AutoFixBottomTip(pos, drawShape);
                AutoFixTopTip(pos);
            }

        }
    }

    // 上のチップは必ず空
    private void AutoFixTopTip(Vector3Int pos)
    {
        for (int y = 0; y < GetMapH(); y++)
        {
            int targetY = pos.y + y + 1;
            if (targetY >= GetMapH())
                break;
            Vector3Int topPos = new Vector3Int(pos.x, targetY, pos.z);
            m_parent.SetMapShape(enShapeType.Empty, topPos);
        }
    }

    private void AutoFixBottomTip(Vector3Int pos, enShapeType drawShape)
    {
        if (pos.y > 0)
        {
            Vector3Int bottomPos = new Vector3Int(pos.x, pos.y - 1, pos.z);
            // public enum enShapeType
            // {
            //     Empty,
            //     Box,
            //     LUpSlope,
            //     RUpSlope,
            //     LUpSlope2H,
            //     LUpSlope2L,
            //     RUpSlope2L,
            //     RUpSlope2H,
            //     SlashWall,
            //     BSlashWall,
            // }
            // 下のブロック置き換え
            switch (drawShape)
            {
                case enShapeType.Box:
                case enShapeType.LUpSlope:
                case enShapeType.RUpSlope:
                case enShapeType.LUpSlope2H:
                case enShapeType.LUpSlope2L:
                case enShapeType.RUpSlope2L:
                case enShapeType.RUpSlope2H:
                    if (m_parent.GetMapShape(bottomPos) != enShapeType.Box)
                    {
                        m_parent.SetMapShape(enShapeType.Box, bottomPos);
                        // 再帰的に書き換え
                        AutoFixBottomTip(bottomPos, enShapeType.Box);
                    }
                    break;
                case enShapeType.SlashWall:
                case enShapeType.BSlashWall:
                    if (m_parent.GetMapShape(bottomPos) != enShapeType.Box)
                    {
                        m_parent.SetMapShape(drawShape, bottomPos);
                        // 再帰的に書き換え
                        AutoFixBottomTip(bottomPos, drawShape);
                        AutoFixSideTip(pos, drawShape, true);
                        AutoFixSideTip(pos, drawShape, false);
                    }
                    break;
            }

        }
    }

    private void AutoFixSideTip(Vector3Int pos, enShapeType drawShape, bool isRight)
    {
        int posX = (isRight)
            ? pos.x + 1
            : pos.x - 1;

        if (posX >= GetMapW() || posX < 0)
            return;

        Vector3Int sidePos = new Vector3Int(posX, pos.y, pos.z);
        bool isChanged = false;
        enShapeType changedShape = enShapeType.Empty;
        if (isRight)
        {
            switch (drawShape)
            {
                case enShapeType.LUpSlope:
                case enShapeType.LUpSlope2L:
                case enShapeType.SlashWall:
                    changedShape = enShapeType.Empty;
                    isChanged = ChangeShape(sidePos, changedShape);
                    break;
                case enShapeType.RUpSlope:
                case enShapeType.RUpSlope2H:
                case enShapeType.BSlashWall:
                    changedShape = enShapeType.Box;
                    isChanged = ChangeShape(sidePos, changedShape);
                    break;
                case enShapeType.LUpSlope2H:
                    changedShape = enShapeType.LUpSlope2L;
                    isChanged = ChangeShape(sidePos, changedShape);
                    break;
                case enShapeType.RUpSlope2L:
                    changedShape = enShapeType.RUpSlope2H;
                    isChanged = ChangeShape(sidePos, changedShape);
                    break;
            }
        }
        else
        {
            switch (drawShape)
            {
                case enShapeType.RUpSlope:
                case enShapeType.BSlashWall:
                case enShapeType.RUpSlope2L:
                    changedShape = enShapeType.Empty;
                    isChanged = ChangeShape(sidePos, changedShape);
                    break;
                case enShapeType.LUpSlope:
                case enShapeType.SlashWall:
                case enShapeType.LUpSlope2H:
                    changedShape = enShapeType.Box;
                    isChanged = ChangeShape(sidePos, changedShape);
                    break;
                case enShapeType.LUpSlope2L:
                    changedShape = enShapeType.LUpSlope2H;
                    isChanged = ChangeShape(sidePos, changedShape);
                    break;
                case enShapeType.RUpSlope2H:
                    changedShape = enShapeType.RUpSlope2L;
                    isChanged = ChangeShape(sidePos, changedShape);
                    break;
            }
        }

        // 書き換えがあった場合は上下の書き換えも行う
        if (isChanged)
        {
            AutoFixTopTip(sidePos);
            AutoFixBottomTip(sidePos, drawShape);
        }

    }

    private bool ChangeShape(Vector3Int sidePos, enShapeType mastShape)
    {
        bool res = false;
        if (m_parent.GetMapShape(sidePos) != mastShape)
        {
            m_parent.SetMapShape(mastShape, sidePos);
            res = true;
        }
        return res;
    }

    // //範囲選択
    // void SelectTips()
    // {
    //     // クリックされた位置を探して、その場所に画像データを入れる
    //     Event e = Event.current;

    //     if ((e.button != 1)
    //         && !(m_isSeleting && (e.type == EventType.ScrollWheel)))
    //         return; //右クリック

    //     if ((e.type == EventType.MouseDown) //マウス押した
    //         || (e.type == EventType.MouseUp) //マウス離した
    //         || (e.type == EventType.MouseDrag) //ドラッグ
    //         || (e.type == EventType.ScrollWheel)) //ホイール
    //     {

    //         Vector2Int resvec;
    //         if (GetMouseGridPos(out resvec))
    //         {
    //             if (e.type == EventType.MouseDown)
    //             {
    //                 m_isSeleting = true;
    //                 m_copyTips = null;
    //                 m_copySt = new Vector3Int(resvec.x, resvec.y, 0);
    //                 m_copyEd = new Vector3Int(resvec.x, resvec.y, 0);
    //             }
    //             else if (e.type == EventType.MouseDrag)
    //             {
    //                 m_copyEd = new Vector3Int(resvec.x, resvec.y, 0);
    //             }
    //             else if (e.type == EventType.ScrollWheel)
    //             {
    //                 m_copyEd = new Vector3Int(resvec.x, resvec.y, 0);
    //             }
    //             else if (e.type == EventType.MouseUp)
    //             {
    //                 m_isSeleting = false;
    //                 m_copyEd = new Vector3Int(resvec.x, resvec.y, 0);
    //             }

    //             m_copyLT = new Vector3Int(0, 0, m_selectedDepth);
    //             m_copyRB = new Vector3Int(0, 0, m_selectedDepth + m_penDepth);

    //             //選択範囲の左上点と右下点
    //             if (m_copySt.x < m_copyEd.x)
    //             {
    //                 m_copyLT.x = m_copySt.x;
    //                 m_copyRB.x = m_copyEd.x;
    //             }
    //             else
    //             {
    //                 m_copyLT.x = m_copyEd.x;
    //                 m_copyRB.x = m_copySt.x;
    //             }

    //             if (m_copySt.y < m_copyEd.y)
    //             {
    //                 m_copyLT.y = m_copySt.y;
    //                 m_copyRB.y = m_copyEd.y;
    //             }
    //             else
    //             {
    //                 m_copyLT.y = m_copyEd.y;
    //                 m_copyRB.y = m_copySt.y;
    //             }

    //             if (m_copyRB.z >= GetMapD())m_copyRB.z = GetMapD() - 1;

    //             if (e.type == EventType.MouseUp)
    //             {
    //                 if (((m_copyRB.x - m_copyLT.x) >= 1) || ((m_copyRB.y - m_copyLT.y) >= 1))
    //                 {
    //                     Vector3Int size = GetPosVector3(m_copyRB) - GetPosVector3(m_copyLT);
    //                     size.x += 1;
    //                     size.y += 1;
    //                     size.z += 1;
    //                     Vector3Int st = new Vector3Int(m_copyLT.x, m_copyLT.y, m_copyLT.z);
    //                     m_copyTips = m_parent.GetCopyMapTip(GetPosVector3(st), size);
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             //範囲外
    //             m_isSeleting = false;
    //             m_copyTips = null;
    //         }

    //         //入力があったときは再描画入れる
    //         SetRepaint();
    //     }
    // }

}
