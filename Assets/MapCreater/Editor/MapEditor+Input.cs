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
    /// 
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

        if (!e.shift && !m_isSeleting)return; //シフト

        if (e.type == EventType.ScrollWheel)
        {
            if (Event.current.delta.y > 0)
            {
                if (m_penDepth > 0)m_penDepth--;
            }
            else
            {
                if (m_penDepth < GetMapD())m_penDepth++;
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

            if (Event.current.delta.y > 0)
            {
                if (m_camRotate == enRotate.r270)
                {
                    m_camRotate = enRotate.r0;
                }
                else
                {
                    m_camRotate++;
                }
            }

            else
            {
                if (m_camRotate == enRotate.r0)
                {
                    m_camRotate = enRotate.r270;
                }
                else
                {
                    m_camRotate--;
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

            if (m_selectedDepth >= GetMapD())m_selectedDepth = (GetMapD() - 1);
            if (m_selectedDepth < 0)m_selectedDepth = 0;

            //parent.asdf = parent.selectedZ;
            SetRepaint();
        }
    }

    //範囲選択
    void SelectTips()
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
                    m_copyTips = null;
                    m_copySt = new Vector3Int(resvec.x, resvec.y, 0);
                    m_copyEd = new Vector3Int(resvec.x, resvec.y, 0);
                }
                else if (e.type == EventType.MouseDrag)
                {
                    m_copyEd = new Vector3Int(resvec.x, resvec.y, 0);
                }
                else if (e.type == EventType.ScrollWheel)
                {
                    m_copyEd = new Vector3Int(resvec.x, resvec.y, 0);
                }
                else if (e.type == EventType.MouseUp)
                {
                    m_isSeleting = false;
                    m_copyEd = new Vector3Int(resvec.x, resvec.y, 0);
                }

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

                if (m_copyRB.z >= GetMapD())m_copyRB.z = GetMapD() - 1;

                if (e.type == EventType.MouseUp)
                {
                    if (((m_copyRB.x - m_copyLT.x) >= 1) || ((m_copyRB.y - m_copyLT.y) >= 1))
                    {
                        Vector3Int size = GetPosVector3(m_copyRB) - GetPosVector3(m_copyLT);
                        size.x += 1;
                        size.y += 1;
                        size.z += 1;
                        Vector3Int st = new Vector3Int(m_copyLT.x, m_copyLT.y, m_copyLT.z);
                        m_copyTips = m_parent.GetCopyMapTip(GetPosVector3(st), size);
                    }
                }
            }
            else
            {
                //範囲外
                m_isSeleting = false;
                m_copyTips = null;
            }

            //入力があったときは再描画入れる
            SetRepaint();
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

                //範囲貼り付け
                if (m_copyTips != null)
                {
                    m_parent.SetPasteMapTip(GetPosVector3(resvec.x, resvec.y, m_selectedDepth), m_copyTips);
                }
                else
                {
                    //ペンの深さ分
                    for (int zz = 0; zz < m_penDepth + 1; zz++)
                    {

                        if ((m_selectedDepth + zz) >= GetMapD())
                            break;

                        m_parent.SetMapShape(GetPosVector3(resvec.x, resvec.y, m_selectedDepth + zz));

                    }
                }

                //入力があったときは再描画入れる
                SetRepaint();
            }
        }
    }
}
