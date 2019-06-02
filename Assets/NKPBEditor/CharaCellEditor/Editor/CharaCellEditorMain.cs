using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKPB
{
    public partial class CharaCellEditorMain : EditorWindow
    {

        [SerializeField]
        CharaCellObject charCells_;
        Dictionary<string, CharaCell> charCellDict_;

        int selectedSprite_ = 0;
        bool isLoadSprite_ = false;

        bool isLoadData_ = false;

        Sprite[] bodySprites_;
        Sprite[] kaoSprites_;
        Sprite[] zuraSprites_;
        Texture2D dummy_;
        CharaCellEditorSub subWindow_; // サブウィンドウ

        private Vector2 scrollPosition = Vector2.zero;

        [UnityEditor.MenuItem(MenuItemName)]
        static void ShowMainWindow()
        {
            EditorWindow.GetWindow<CharaCellEditorMain>();
        }
        public Sprite GetSelectedSprite()
        {
            return GetSprite(selectedSprite_);
        }

        public Sprite GetSprite(int spriteNo)
        {
            return bodySprites_[spriteNo];
        }

        public CharaCell GetSelectedCharCell()
        {
            return charCells_[selectedSprite_];
        }

        public Sprite GetSelectedKaoSprite()
        {
            //charCells_[selectedSprite_].faceNo = 0;
            return kaoSprites_[GetSelectedCharCell().faceNo];
        }

        public Sprite GetSelectedZuraSprite()
        {
            //charCells_[selectedSprite_].faceNo = 0;
            return zuraSprites_[ZURA_OF_KAO[GetSelectedCharCell().faceNo]];
        }

        public void SetFaceNo(int faceNo)
        {
            if (faceNo < 0)return;
            if (faceNo > KAO_MAX)return;

            charCells_[selectedSprite_].faceNo = faceNo;
        }
        public void IncFaceNo(bool isDec)
        {
            if (isDec)
            {
                if (charCells_[selectedSprite_].faceNo > 0)charCells_[selectedSprite_].faceNo--;
            }
            else
            {
                if (charCells_[selectedSprite_].faceNo < KAO_MAX)charCells_[selectedSprite_].faceNo++;
            }

        }
        public void SetFaceAngle(int faceAngle)
        {
            if (faceAngle < 0)
            {
                faceAngle = (faceAngle % (ANGLE_MAX + 1)) + (ANGLE_MAX + 1);
            }
            charCells_[selectedSprite_].faceAngle = (faceAngle % (ANGLE_MAX + 1));
        }

        public void SetFaceX(int faceX)
        {
            charCells_[selectedSprite_].faceX = faceX;
        }

        public void SetFaceY(int faceY)
        {
            charCells_[selectedSprite_].faceY = faceY;
        }

        public void SetFaceZ(int faceZ)
        {
            if (faceZ < 0)return;
            if (faceZ > 1)return;
            charCells_[selectedSprite_].faceZ = faceZ;
        }

        public int GetKaoNoMax()
        {
            return KAO_MAX;
        }
        public int GetAngleMax()
        {
            return ANGLE_MAX;
        }

        void OnEnable()
        {
            isLoadSprite_ = false;
        }

        void OnGUI()
        {
            try
            {
                if (!isLoadSprite_)
                    LoadSprite();

                if (!isLoadData_)
                    LoadFile();

                EditorGUI.BeginChangeCheck();

                //テクスチャリロード
                DrawButtonReload();
                //新規マップ
                DrawButtonOpenSubWindow();
                //セーブロード
                DrawButtonSaveLoad();

                //アイコン＋名前
                DrawImageParts();
                //選択中
                DrawSelectedImage();
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

        void LoadSprite()
        {
            dummy_ = new Texture2D(TIPSIZE, TIPSIZE);
            {
                // 読み込み(Resources.LoadAllを使うのがミソ)
                Object[] list = Resources.LoadAll(BodyFilePath, typeof(Sprite));
                // listがnullまたは空ならエラーで返す
                bodySprites_ = new Sprite[list.Length];
                // listを回してDictionaryに格納
                for (int i = 0; i < list.Length; ++i)bodySprites_[i] = list[i] as Sprite;
            }

            {
                // 読み込み(Resources.LoadAllを使うのがミソ)
                Object[] list = Resources.LoadAll(KaoFileName, typeof(Sprite));
                // listがnullまたは空ならエラーで返す
                kaoSprites_ = new Sprite[list.Length];
                Debug.Log(kaoSprites_.Length);
                // listを回してDictionaryに格納
                for (int i = 0; i < list.Length; ++i)kaoSprites_[i] = list[i] as Sprite;
            }

            {
                // 読み込み(Resources.LoadAllを使うのがミソ)
                Object[] list = Resources.LoadAll(ZuraFileName, typeof(Sprite));
                // listがnullまたは空ならエラーで返す
                zuraSprites_ = new Sprite[list.Length];
                Debug.Log(zuraSprites_.Length);
                // listを回してDictionaryに格納
                for (int i = 0; i < list.Length; ++i)zuraSprites_[i] = list[i] as Sprite;
            }

            LoadFile();

            AssetDatabase.Refresh();

            isLoadSprite_ = true;
        }

        public static Rect GetSpriteNormalRect(Sprite sp)
        {
            // spriteの親テクスチャー上のRect座標を取得.
            Rect rectPosition = sp.textureRect;

            // 親テクスチャーの大きさを取得.
            float parentWith = sp.texture.width;
            float parentHeight = sp.texture.height;
            // spriteの座標を親テクスチャーに合わせて正規化.
            Rect NormalRect = new Rect(
                rectPosition.x / parentWith,
                rectPosition.y / parentHeight,
                rectPosition.width / parentWith,
                rectPosition.height / parentHeight
            );

            return NormalRect;
        }

        // 画像一覧をボタン選択出来る形にして出力
        void DrawImageParts()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUI.skin.box);

            //if(imgDirectory != null)
            {
                float x = 0.0f;
                float y = 00.0f;
                //float size = 48;
                //float h = 50.0f;
                float maxW = TIPSIZE * 5;

                EditorGUILayout.BeginVertical();
                int index = 0;
                bool isEnd = false;
                foreach (var sp in bodySprites_)
                {
                    string spname = CharaCell.ReplaceHyphen(sp.name);
                    GUIContent contents = new GUIContent();
                    if (!charCellDict_.ContainsKey(spname))
                    {
                        isLoadSprite_ = false;
                        return;
                    }

                    contents.text = spname
                        + "\n No:" + charCellDict_[spname].faceNo.ToString()
                        + " X:" + charCellDict_[spname].faceX.ToString()
                        + " Y:" + charCellDict_[spname].faceY.ToString();

                    if ((charCellDict_[spname].faceNo == 0) && (charCellDict_[spname].faceX == 0) && (charCellDict_[spname].faceY == 0))
                    {
                        contents.text += " *AllZero*";
                    }

                    //GUILayout.FlexibleSpace();
                    if (selectedSprite_ == index)GUI.color = new Color(1f, 0.5f, 1f, 1f);
                    if (GUILayout.Button(contents, GUILayout.MaxWidth(TIPSIZE * 5), GUILayout.Height(TIPSIZE), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
                    {
                        selectedSprite_ = index;
                        RefreshEditorWindow();
                    }
                    if (selectedSprite_ == index)GUI.color = new Color(1f, 1f, 1f, 1f);

                    Rect lastRect = GUILayoutUtility.GetLastRect();

                    lastRect.width = TIPSIZE;
                    lastRect.height = TIPSIZE;
                    GUI.DrawTextureWithTexCoords(lastRect, sp.texture, GetSpriteNormalRect(sp));
                    y += TIPSIZE;
                    index++;
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

        }

        // 選択した画像データを表示
        void DrawSelectedImage()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (dummy_ == null)dummy_ = new Texture2D(TIPSIZE, TIPSIZE);
            GUILayout.Box(dummy_);
            Rect lastRect = GUILayoutUtility.GetLastRect();
            lastRect.x += (lastRect.width - TIPSIZE) / 2;
            lastRect.y += (lastRect.height - TIPSIZE) / 2;
            lastRect.width = TIPSIZE;
            lastRect.height = TIPSIZE;
            GUI.DrawTextureWithTexCoords(lastRect, GetSelectedSprite().texture, GetSpriteNormalRect(GetSelectedSprite()));
            EditorGUILayout.LabelField(GetSelectedSprite().name); // いつまでも居座り続けるぜ！
            EditorGUILayout.EndVertical();
        }

        void DrawButtonReload()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reload"))
            {
                LoadSprite();
            }
            EditorGUILayout.EndVertical();
        }
        // エディタウィンドウを開くボタンを生成
        void DrawButtonOpenSubWindow()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("OpenSubWindow"))
            {
                OpenEditorWindow();
            }
            EditorGUILayout.EndVertical();
        }

        void OpenEditorWindow()
        {
            if (subWindow_ != null)subWindow_.Close();

            if (subWindow_ == null)
            {
                subWindow_ = CharaCellEditorSub.WillAppear(this);
            }
            else
            {
                subWindow_.Focus();
            }
            subWindow_.init();
        }

        void RefreshEditorWindow()
        {
            if (subWindow_ == null)
            {
                subWindow_ = CharaCellEditorSub.WillAppear(this);
            }
            else
            {
                subWindow_.Focus();
            }
            subWindow_.init();
        }

        void DrawButtonSaveLoad()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //保存
            if (GUILayout.Button("SaveFile"))SaveFile();

            //読み込み
            if (GUILayout.Button("LoadFile"))LoadFile();

            EditorGUILayout.EndHorizontal();
        }

        //入出力系///////////////////////////////

        // ファイルで出力
        public void SaveFile()
        {
            var savetips = charCells_.GetClone();
            AssetDatabase.CreateAsset(savetips, ScriptableObjectFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("SaveMap " + ScriptableObjectFilePath);

        }
        void LoadFile()
        {
            Debug.Log("LoadFile");
            isLoadData_ = true;
            var loadtips = AssetDatabase.LoadAssetAtPath<CharaCellObject>(ScriptableObjectFilePath);
            if (loadtips != null)
            {
                charCells_ = loadtips.GetClone();

                charCellDict_ = new Dictionary<string, CharaCell>();
                for (int i = 0; i < charCells_.param.Count; ++i)
                {
                    charCellDict_[charCells_[i].ID] = charCells_[i];
                }
            }
            else
            {
                EditorUtility.DisplayDialog("LoadFile", "読み込めませんでした。\n" + ScriptableObjectFilePath, "ok");
            }

            AssetDatabase.Refresh();

        }
    }
}
