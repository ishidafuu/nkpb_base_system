using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomPreview(typeof(MapTips))]
public class MyPreview : ObjectPreview
{

    const int GRIDSIZE = 16;
    const int GRIDSIZE_Z = 8;
    const int TIPSIZE = 24;

    const int CAMPOS_DEFAULT = 32;

    static Sprite[] mapTipSprites_;
    MapTips mapTips3_;
    Vector2 camPos_;
    Vector2 mouseStPos_;
    float mag = 1f;
    bool isRepaint_;

    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void Initialize(Object[] targets)
    {
        base.Initialize(targets);

        LoadMapTipSprite();

        mapTips3_ = (MapTips)targets[0];
        SetDefaultCameraPos();
    }

    private void SetDefaultCameraPos()
    {
        camPos_ = new Vector2(CAMPOS_DEFAULT, (GetMapH() * GRIDSIZE) + (GetMapD() * GRIDSIZE_Z) + CAMPOS_DEFAULT);
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);
        isRepaint_ = false;
        // GUIの見た目を変える。
        GUIStyle guiStyle = new GUIStyle();
        GUIStyleState styleState = new GUIStyleState();

        //// GUI背景色のバックアップ
        //Color backColor = GUI.backgroundColor;

        //// GUI背景の色を設定
        //GUI.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);

        //// 背景用テクスチャを設定
        //styleState.background = Texture2D.whiteTexture;

        // テキストの色を設定
        styleState.textColor = Color.white;

        // スタイルの設定。
        guiStyle.normal = styleState;
        GUI.Label(r, camPos_.ToString(), guiStyle);

        MoveCamera(r);
        if (Event.current.type == EventType.Repaint)DrawMapTip(r);

        if (isRepaint_)
        {
            EditorUtility.SetDirty(target);
        }

    }

    //カメラ視点移動
    private void MoveCamera(Rect r)
    {

        Event e = Event.current;
        //if (e.button != 2) return;

        if (r.Contains(e.mousePosition) == false)return;

        if (e.button == 1)
        {
            if (e.type == EventType.MouseDown)
            {
                SetDefaultCameraPos();
                isRepaint_ = true;
            }
        }
        else
        {
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

    }

    static private void LoadMapTipSprite()
    {
        if (mapTipSprites_ != null)
            return;
        // 読み込み(Resources.LoadAllを使うのがミソ)
        Object[] list = Resources.LoadAll("MapTipPalette", typeof(Sprite));
        // listがnullまたは空ならエラーで返す
        mapTipSprites_ = new Sprite[list.Length];

        // listを回してDictionaryに格納
        for (int i = 0; i < list.Length; ++i)mapTipSprites_[i] = list[i] as Sprite;
    }

    //マップの幅
    private int GetMapW()
    {
        return mapTips3_.mapSizeX;
    }
    //マップ奥行き
    private int GetMapD()
    {
        return mapTips3_.mapSizeZ;
    }
    //マップの幅
    private int GetMapH()
    {
        return mapTips3_.mapSizeY;
    }
    public Sprite GetTipsSprite(Vector3Int pos, bool isEmpNull = false)
    {
        if (mapTips3_ == null)
            return null;

        if (isEmpNull && (mapTips3_[pos, false] == 0))
            return null;
        Debug.Log(pos);
        return mapTipSprites_[mapTips3_[pos, false]];
    }

    private void DrawMapTip(Rect r)
    {

        // 選択した画像を描画する
        for (int z = 0; z < GetMapD(); z++)
        {
            int zz = (GetMapD() - z - 1); //奥から

            int zpos = ((zz) * GRIDSIZE_Z); //奥行きのオフセット

            for (int y = 0; y < GetMapH(); y++)
            {
                // int yy = (GetMapH() - y - 1); //奥から
                int yy = y; //下から
                for (int x = 0; x < GetMapW(); x++)
                {
                    int xx = x; //奥から
                    Sprite sp = GetTipsSprite(new Vector3Int(xx, yy, zz), true);
                    Vector2 pos = new Vector2(r.x + (xx * GRIDSIZE) + zpos, r.y - (yy * GRIDSIZE) - zpos);
                    Vector2 size = new Vector2(TIPSIZE, TIPSIZE);
                    //Rect drawRect = new Rect((pos + camPos_) * mag, size * mag);
                    Rect drawRect = new Rect((pos + camPos_) * mag, size * mag);

                    if (sp != null)
                    {
                        Debug.Log(drawRect);
                        GUI.DrawTextureWithTexCoords(drawRect, sp.texture, MapCreater.GetSpriteNormalRect(sp)); //描画
                    }
                }
            }
        }
    }

}
