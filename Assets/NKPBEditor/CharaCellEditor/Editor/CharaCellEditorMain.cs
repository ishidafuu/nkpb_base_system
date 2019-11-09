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
        CharaCellObject m_charCells;
        Dictionary<string, CharaCell> m_charCellDict;

        int m_selectedSprite = 0;
        bool m_isLoadSprite = false;

        bool m_isLoadData = false;

        Sprite[] m_bodySprites;
        Sprite[] m_kaoSprites;
        Sprite[] m_zuraSprites;
        Texture2D m_dummy;
        CharaCellEditorSub m_subWindow; // サブウィンドウ

        private Vector2 m_scrollPosition = Vector2.zero;

        [UnityEditor.MenuItem(MenuItemName)]
        static void ShowMainWindow()
        {
            EditorWindow.GetWindow<CharaCellEditorMain>();
        }

        public void Recording()
        {
            // Undoで戻る先を保存する.
            Undo.RecordObject(m_charCells, "charCellsUpdate");
        }

        public Sprite GetSelectedSprite()
        {
            return GetSprite(m_selectedSprite);
        }

        public Sprite GetSprite(int spriteNo)
        {
            return m_bodySprites[spriteNo];
        }

        public CharaCell GetSelectedCharCell()
        {
            return m_charCells[m_selectedSprite];
        }

        public Sprite GetSelectedKaoSprite()
        {
            //charCells_[selectedSprite_].faceNo = 0;
            return m_kaoSprites[GetSelectedCharCell().faceNo];
        }

        public Sprite GetSelectedZuraSprite()
        {
            //charCells_[selectedSprite_].faceNo = 0;
            return m_zuraSprites[ZURA_OF_KAO[GetSelectedCharCell().faceNo]];
        }

        public void SetFaceNo(int faceNo)
        {
            if (faceNo < 0) return;
            if (faceNo > KAO_MAX) return;

            m_charCells[m_selectedSprite].faceNo = faceNo;
        }
        public void IncFaceNo(bool isDec)
        {
            if (isDec)
            {
                if (m_charCells[m_selectedSprite].faceNo > 0) m_charCells[m_selectedSprite].faceNo--;
            }
            else
            {
                if (m_charCells[m_selectedSprite].faceNo < KAO_MAX) m_charCells[m_selectedSprite].faceNo++;
            }

        }
        public void SetFaceAngle(int faceAngle)
        {
            if (faceAngle < 0)
            {
                faceAngle = (faceAngle % (ANGLE_MAX + 1)) + (ANGLE_MAX + 1);
            }
            m_charCells[m_selectedSprite].faceAngle = (faceAngle % (ANGLE_MAX + 1));
        }

        public void SetFaceX(int faceX)
        {
            m_charCells[m_selectedSprite].faceX = faceX;
        }

        public void SetFaceY(int faceY)
        {
            m_charCells[m_selectedSprite].faceY = faceY;
        }

        public void SetFaceZ(int faceZ)
        {
            if (faceZ < 0) return;
            if (faceZ > 1) return;
            m_charCells[m_selectedSprite].faceZ = faceZ;
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
            m_isLoadSprite = false;
        }

        void OnGUI()
        {
            try
            {
                if (!m_isLoadSprite)
                    LoadSprite();

                if (!m_isLoadData)
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
            m_dummy = new Texture2D(TIPSIZE, TIPSIZE);
            {
                // 読み込み(Resources.LoadAllを使うのがミソ)
                Object[] list = Resources.LoadAll(BodyFilePath, typeof(Sprite));
                // listがnullまたは空ならエラーで返す
                m_bodySprites = new Sprite[list.Length];
                // listを回してDictionaryに格納
                for (int i = 0; i < list.Length; ++i) m_bodySprites[i] = list[i] as Sprite;
            }

            {
                // 読み込み(Resources.LoadAllを使うのがミソ)
                Object[] list = Resources.LoadAll(KaoFileName, typeof(Sprite));
                // listがnullまたは空ならエラーで返す
                m_kaoSprites = new Sprite[list.Length];
                Debug.Log(m_kaoSprites.Length);
                // listを回してDictionaryに格納
                for (int i = 0; i < list.Length; ++i) m_kaoSprites[i] = list[i] as Sprite;
            }

            {
                // 読み込み(Resources.LoadAllを使うのがミソ)
                Object[] list = Resources.LoadAll(ZuraFileName, typeof(Sprite));
                // listがnullまたは空ならエラーで返す
                m_zuraSprites = new Sprite[list.Length];
                Debug.Log(m_zuraSprites.Length);
                // listを回してDictionaryに格納
                for (int i = 0; i < list.Length; ++i) m_zuraSprites[i] = list[i] as Sprite;
            }

            LoadFile();

            AssetDatabase.Refresh();

            m_isLoadSprite = true;
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
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition, GUI.skin.box);

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
                foreach (var sp in m_bodySprites)
                {
                    string spname = CharaCell.ReplaceHyphen(sp.name);
                    GUIContent contents = new GUIContent();
                    if (!m_charCellDict.ContainsKey(spname))
                    {
                        Debug.Log($"NotFound {spname}");
                        m_isLoadSprite = false;
                        return;
                    }

                    contents.text = spname
                        + "\n No:" + m_charCellDict[spname].faceNo.ToString()
                        + " X:" + m_charCellDict[spname].faceX.ToString()
                        + " Y:" + m_charCellDict[spname].faceY.ToString();

                    if ((m_charCellDict[spname].faceNo == 0) && (m_charCellDict[spname].faceX == 0) && (m_charCellDict[spname].faceY == 0))
                    {
                        contents.text += " *AllZero*";
                    }

                    //GUILayout.FlexibleSpace();
                    if (m_selectedSprite == index) GUI.color = new Color(1f, 0.5f, 1f, 1f);
                    if (GUILayout.Button(contents, GUILayout.MaxWidth(TIPSIZE * 5), GUILayout.Height(TIPSIZE), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
                    {
                        m_selectedSprite = index;
                        RefreshEditorWindow();
                    }
                    if (m_selectedSprite == index) GUI.color = new Color(1f, 1f, 1f, 1f);

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
            if (m_dummy == null) m_dummy = new Texture2D(TIPSIZE, TIPSIZE);
            GUILayout.Box(m_dummy);
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

            //読み込み
            if (GUILayout.Button("RefreshFromSpriteFile"))
            {
                RefreshFromSpriteFile();
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
            if (m_subWindow != null) m_subWindow.Close();

            if (m_subWindow == null)
            {
                m_subWindow = CharaCellEditorSub.WillAppear(this);
            }
            else
            {
                m_subWindow.Focus();
            }
            m_subWindow.init();
        }

        void RefreshEditorWindow()
        {
            if (m_subWindow == null)
            {
                m_subWindow = CharaCellEditorSub.WillAppear(this);
            }
            else
            {
                m_subWindow.Focus();
            }
            m_subWindow.init();
        }

        void DrawButtonSaveLoad()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //保存
            if (GUILayout.Button("SaveFile"))
                SaveFile();

            //読み込み
            if (GUILayout.Button("LoadFile"))
                LoadFile();

            EditorGUILayout.EndHorizontal();
        }

        //入出力系///////////////////////////////

        // ファイルで出力
        public void SaveFile()
        {
            var savetips = m_charCells.GetClone();
            AssetDatabase.CreateAsset(savetips, ScriptableObjectFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("SaveMap " + ScriptableObjectFilePath);

        }

        void LoadFile()
        {
            Debug.Log("LoadFile");
            m_isLoadData = true;
            var loadtips = AssetDatabase.LoadAssetAtPath<CharaCellObject>(ScriptableObjectFilePath);
            if (loadtips != null)
            {
                m_charCells = loadtips.GetClone();

                UpdateCharaCellDict();
            }
            else
            {
                EditorUtility.DisplayDialog("LoadFile", "読み込めませんでした。\n" + ScriptableObjectFilePath, "ok");
            }

            AssetDatabase.Refresh();

        }

        private void UpdateCharaCellDict()
        {
            m_charCellDict = new Dictionary<string, CharaCell>();
            for (int i = 0; i < m_charCells.param.Count; ++i)
            {
                m_charCellDict[m_charCells[i].ID] = m_charCells[i];
            }
        }

        void RefreshFromSpriteFile()
        {
            if (EditorUtility.DisplayDialogComplex("RefreshFromSpriteFile",
                    "スプライトからCharaCellデータを再生しますか？",
                    "OK", "キャンセル", "") == 0)
            {
                Debug.Log("RefreshFromSpriteFile");
                var newCharCells = new CharaCellObject();

                // 現在のスプライトにあるものを抽出、ない場合は新たに作成
                for (int i = 0; i < m_bodySprites.Length; ++i)
                {
                    string id = CharaCell.ReplaceHyphen(m_bodySprites[i].name);
                    CharaCell item = m_charCells.param.FirstOrDefault(x => x.ID == id);
                    if (item == null)
                    {
                        item = new CharaCell();
                        item.ID = id;
                    }

                    newCharCells.param.Add(item);
                }

                m_charCells = newCharCells;

                UpdateCharaCellDict();

                AssetDatabase.Refresh();
            }

        }

    }
}
