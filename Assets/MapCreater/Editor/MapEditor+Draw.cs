using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Map creater sub window.
/// </summary>
public partial class MapEditor
{
    //描画系///////////////////////////////
    void DrawMapTip()
    {

        bool isPutTip = false;
        Texture putSprite = m_parent.GetSelectedSprite(m_camRotate);

        if ((putSprite != null) && !m_isSeleting)
        {
            Vector2Int resvec;
            if (GetMouseGridPos(out resvec))
            {
                //ペン表示
                isPutTip = true;
                if (m_cursorPos != resvec)
                {
                    m_cursorPos = resvec;
                    SetRepaint();
                }
            }
            //else
            //{
            //	if (m_cursorPos != null)
            //	{
            //		m_cursorPos = null;
            //		SetRepaint();
            //	}
            //}
        }

        // 選択した画像を描画する
        for (int z = 0; z < GetMapD(); z++)
        {
            int zz = (GetMapD() - z - 1); //奥から

            int zpos = ((zz - m_selectedDepth) * GRID_SIZE_Z); //奥行きのオフセット
            bool isBlockDraw = true;

            //奥プレート描画
            if (zz == (GetMapD() - 1))
                DrawPlateFR(zpos + GRID_SIZE_Z);

            //選択面グリッド描画
            if (zz == m_selectedDepth + m_penDepth)
                DrawGridLine2();

            if ((zz >= m_selectedDepth)
                && (zz <= m_selectedDepth + m_penDepth))
            {
                GUI.color = new Color(1f, 1f, 1f, 1f);
            }
            else if (zz < m_selectedDepth)
            {
                const float COL = 0.6f;
                GUI.color = new Color(COL, COL, COL, 1f);
            }
            else
            {
                const float MINCOL = 0.4f;
                float col = 0.7f - (0.05f * Mathf.Abs(m_selectedDepth - zz));
                if (col < MINCOL)col = MINCOL;
                GUI.color = new Color(col, col, col, 1f);
            }

            if (isBlockDraw)
            {
                for (int y = 0; y < GetMapH(); y++)
                {
                    int yy = (GetMapH() - y - 1); //奥から

                    for (int xx = 0; xx < GetMapW(); xx++)
                    {
                        int revyy = (GetMapH() - yy - 1);
                        Vector2 pos = new Vector2((xx * GRID_SIZE) + zpos, (yy * GRID_SIZE) - zpos);
                        Vector2 size = new Vector2(TIP_SIZE, TIP_SIZE);
                        Rect drawRect = new Rect((pos + m_camPos) * m_mag, size * m_mag);

                        //左端プレート
                        if ((xx == 0) && (y == 0) && (z == 0))
                        {
                            DrawPlateLR(pos + m_camPos, true);
                        }

                        //ペン先描画判定
                        bool isPutTip2 = false;
                        Texture copysprite = null;

                        if (m_cursorPos != null)
                        {
                            if (m_copyTips != null)
                            {
                                Vector3Int copypos = new Vector3Int((xx - (int)m_cursorPos.x), (revyy - (int)m_cursorPos.y), (zz - m_selectedDepth));
                                if (m_copyTips.IsSafePos(copypos))
                                {
                                    copysprite = m_parent.GetSprite(m_copyTips.GetShape(copypos), m_camRotate);
                                }
                            }
                            else
                            {
                                if (isPutTip
                                    && (zz >= m_selectedDepth)
                                    && (zz <= (m_penDepth + m_selectedDepth)))
                                {
                                    Vector2Int pos2 = new Vector2Int(xx, revyy);
                                    isPutTip2 = (pos2 == m_cursorPos); // (pos2.Equal(m_cursorPos));
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

                            Texture sp = m_parent.GetTipsSprite(GetPosVector3(xx, y, zz), m_camRotate, true);

                            bool isTipDraw = true;
                            bool isSelectedDepth = (zz == (m_selectedDepth));

                            if (m_cursorPos != null)
                            {
                                //カーソルより手前側は表示しない
                                // tipDraw = (isPutTip)
                                //     ? (xx < m_cursorPos.x) || (zz >= (m_selectedDepth))
                                //     : (zz >= (m_selectedDepth));
                                isTipDraw = (zz >= (m_selectedDepth));
                            }

                            float alp = (isTipDraw)
                                ? 1f
                                : 0.05f;

                            float col = (isSelectedDepth)
                                ? 1f
                                : 0.8f;

                            GUI.color = new Color(col, col, col, alp);

                            //選択範囲
                            if (m_isSeleting)
                            {
                                bool betweenX = ((m_copyLT.x <= xx) && (m_copyRB.x >= xx));
                                bool betweenY = ((m_copyLT.y <= revyy) && (m_copyRB.y >= revyy));
                                bool betweenZ = ((m_copyLT.z <= zz) && (m_copyRB.z >= zz));
                                if ((m_copyLT.x == xx) && betweenY && betweenZ)
                                    DrawTipPlate(pos + m_camPos, enFace.fLeft);
                                if ((m_copyLT.y == revyy) && betweenX && betweenZ)
                                    DrawTipPlate(pos + m_camPos, enFace.fBottom);
                                if ((m_copyRB.z == zz) && betweenX && betweenY)
                                    DrawTipPlate(pos + m_camPos, enFace.fRear);

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
                                if ((m_copyRB.x == xx) && betweenY && betweenZ)DrawTipPlate(pos + m_camPos, enFace.fRight);
                                if ((m_copyRB.y == revyy) && betweenX && betweenZ)DrawTipPlate(pos + m_camPos, enFace.fTop);
                            }
                            else
                            {
                                if (sp != null)
                                {
                                    GUI.DrawTextureWithTexCoords(drawRect, sp, new Rect(0, 0, 1, 1)); //描画
                                }
                            }

                        }
                    }
                }
            }

            if (zz == 0)
            {
                //手前プレート描画
                DrawPlateFR(zpos);
                //左端プレート
                Vector2 pos = new Vector2((GetMapW() * GRID_SIZE) + zpos, (GetMapH() * GRID_SIZE) - zpos);
                DrawPlateLR(pos + m_camPos, false);
            }
        }

        //色元に戻す
        GUI.color = new Color(1f, 1f, 1f, 1f);
    }

    // グリッド線を描画
    void DrawGridLine2()
    {
        if (m_isSeleting)return;

        if (m_cursorPos == null)return;

        // grid
        Handles.color = new Color(1f, 1f, 1f, 0.5f);
        float penpos = (m_penDepth * GRID_SIZE_Z);
        float fx = m_camPos.x + penpos + GRID_SIZE_Z;
        float fy = m_camPos.y - penpos;

        // 縦線
        for (int i = 0; i < GetMapW() + 1; i++)
        {
            bool isPenX = (i == m_cursorPos.x);

            Handles.color = isPenX
                ? new Color(1f, 1f, 1f, 1f)
                : new Color(1f, 1f, 1f, 0.5f);
            float x = fx + (i * GRID_SIZE);

            Vector2 st = new Vector2(x, fy);
            Vector2 ed = new Vector2(x, fy + (GetMapH() * GRID_SIZE));
            Handles.DrawLine(st * m_mag, ed * m_mag);
        }

        // 横線
        for (int i = 0; i < GetMapH() + 1; i++)
        {
            bool isPenY = (i == GetMapH() - m_cursorPos.y - 1);

            Handles.color = isPenY
                ? new Color(1f, 1f, 1f, 1f)
                : new Color(1f, 1f, 1f, 0.5f);
            float y = fy + (i * GRID_SIZE);
            Vector2 st = new Vector2(fx, y);
            Vector2 ed = new Vector2(fx + (GetMapW() * GRID_SIZE), y);
            Handles.DrawLine(st * m_mag, ed * m_mag);
        }
    }

    // 末端面を描画
    void DrawPlateFR(int zpos)
    {
        // grid
        Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        float fx = zpos;
        float fy = GRID_SIZE_Z - zpos;
        Vector2 pos = new Vector2(fx, fy);
        Vector2 size = new Vector2((GetMapW() * GRID_SIZE), (GetMapH() * GRID_SIZE));
        Rect rect = new Rect((m_camPos + pos) * m_mag, size * m_mag);

        Handles.DrawSolidRectangleWithOutline(rect, Handles.color, Handles.color);
    }

    // 面を描画
    void DrawPlateLR(Vector2 pos, bool isLeft)
    {

        //pos.y += GRIDSIZE_Z;
        int deplen = (GetMapD()) * GRID_SIZE_Z;
        pos.y -= (GetMapH() * GRID_SIZE);
        if (isLeft)
        {
            pos.x += -(deplen - GRID_SIZE_Z);
            pos.y += (deplen + GRID_SIZE);
        }
        else
        {
            pos.y += (GRID_SIZE_Z);
        }
        Vector2 revR = new Vector2(+deplen, -deplen);
        Vector2 revD = new Vector2(0, (GetMapH() * GRID_SIZE));

        Vector2 v0 = pos;
        Vector2 v1 = pos + revR;
        Vector2 v2 = pos + revR + revD;
        Vector2 v3 = pos + revD;

        Vector3[] verts = { v0 * m_mag, v1 * m_mag, v2 * m_mag, v3 * m_mag };
        Color tmpcol = Handles.color;
        Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);
        Handles.color = tmpcol;
    }

    // 面を描画
    void DrawTipPlate(Vector2 pos, enFace face)
    {
        Vector2 revR = new Vector2();
        Vector2 revD = new Vector2();

        switch (face)
        {
            case enFace.fLeft:
                pos.y += GRID_SIZE_Z;
                revR = new Vector2(GRID_SIZE_Z, -GRID_SIZE_Z);
                revD = new Vector2(0, GRID_SIZE);
                break;
            case enFace.fRight:
                pos.x += GRID_SIZE;
                pos.y += GRID_SIZE_Z;
                revR = new Vector2(GRID_SIZE_Z, -GRID_SIZE_Z);
                revD = new Vector2(0, GRID_SIZE);
                break;
            case enFace.fTop:
                pos.x += GRID_SIZE_Z;
                revR = new Vector2(GRID_SIZE, 0);
                revD = new Vector2(-GRID_SIZE_Z, GRID_SIZE_Z);
                break;
            case enFace.fBottom:
                pos.x += GRID_SIZE_Z;
                pos.y += GRID_SIZE;
                revR = new Vector2(GRID_SIZE, 0);
                revD = new Vector2(-GRID_SIZE_Z, GRID_SIZE_Z);
                break;
            case enFace.fRear:
                pos.x += GRID_SIZE_Z;
                //pos.y -= GRIDSIZE_Z;
                revR = new Vector2(GRID_SIZE, 0);
                revD = new Vector2(0, GRID_SIZE);
                break;
            default:
                break;
        }

        Vector2 v0 = pos;
        Vector2 v1 = pos + revR;
        Vector2 v2 = pos + revR + revD;
        Vector2 v3 = pos + revD;

        Vector3[] verts = { v0 * m_mag, v1 * m_mag, v2 * m_mag, v3 * m_mag };
        Color tmpcol = Handles.color;
        Handles.color = new Color(1f, 1f, 1f, 0.5f);
        Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);
        Handles.color = tmpcol;
    }

}
