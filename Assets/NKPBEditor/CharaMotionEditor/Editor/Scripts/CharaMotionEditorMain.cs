using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKPB
{
    public partial class CharaMotionEditorMain : EditorWindow
    {
        GameObject m_target;
        float m_toolbarWidth = 300f;
        float m_previewZoom = 2f;
        Vector2 m_previewPan = Vector2.zero;
        Vector2 m_editorScroll;
        bool m_showColliders = true;
        bool m_showGizmos = false;
        bool m_showMove;
        bool m_showAnimation;
        bool m_showAttackData;
        bool m_showFrameData;
        bool m_showEvents;
        Texture2D m_backgroundImage;
        Texture2D m_editorBackgroundImage;
        Texture2D m_BufferTexture;
        readonly float MINZOOM = 1f;
        readonly float MAXZOOM = 8f;
        HitBoxManagerInspector m_Inspector = null;
        HitboxManager m_hitboxManager = null;
        Event m_currentEvent;
        // ICharacter m_character;
        SpriteRenderer m_SpriteRenderer;

        [MenuItem("NKPBEditor/CharaMotion Editor", priority = 1000)]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            CharaMotionEditorMain window = (CharaMotionEditorMain)EditorWindow.GetWindow(typeof(CharaMotionEditorMain));
            window.titleContent = new GUIContent("CharaMotion Editor");
            window.Show();
        }

        void OnGUI()
        {
            wantsMouseMove = true;
            m_currentEvent = Event.current;
            InitGUI();
            InitHitBoxManager();

            Repainting();
            DrawBackGround();

            EditorGUILayout.BeginHorizontal();
            GUIEditView();
            GUIInspectorView();
            EditorGUILayout.EndHorizontal();

        }

        void InitGUI()
        {
            if (m_editorBackgroundImage == null)
                m_editorBackgroundImage = Resources.Load<Texture2D>("GrayCheckerBackground");

            if (m_backgroundImage == null)
                m_backgroundImage = MakeTex(1, 1, new Color(0.1f, 0.1f, 0.1f, 1f));
        }

        void Repainting()
        {
            if (m_currentEvent.type == EventType.MouseDrag
                || m_currentEvent.type == EventType.MouseDown
                || m_currentEvent.type == EventType.MouseMove)
                Repaint();
        }

        void DrawBackGround()
        {
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), m_backgroundImage, ScaleMode.StretchToFill);
        }

        void InitHitBoxManager()
        {
            m_hitboxManager = m_target == null ? null : m_hitboxManager = m_target.GetComponentInChildren<HitboxManager>();

            if (m_hitboxManager == null)
            {
                m_SpriteRenderer = null;
                m_Inspector = null;
                return;
            }

            // m_character = m_target.GetComponentInChildren<ICharacter>();
            if (m_SpriteRenderer == null)
                m_SpriteRenderer = m_target.GetComponentInChildren<SpriteRenderer>();
            if (m_Inspector == null)
            {
                m_Inspector = (HitBoxManagerInspector)Editor.CreateEditor(m_hitboxManager);
                m_target.GetComponentInChildren<SpriteRenderer>();
            }
        }

        void GUIInspectorView()
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(m_toolbarWidth));
            GUILayout.Label("Inspector View", EditorStyles.boldLabel);

            m_editorScroll = EditorGUILayout.BeginScrollView(m_editorScroll, GUILayout.Width(m_toolbarWidth));
            var previoustarget = m_target;
            m_target = (GameObject)EditorGUILayout.ObjectField(m_target, typeof(GameObject), false);

            // m_target = (RuntimeAnimatorController)EditorGUILayout.ObjectField(m_target, typeof(RuntimeAnimatorController), false);
            if (previoustarget != m_target)
            {
                m_hitboxManager = null;
                m_Inspector = null;
                m_SpriteRenderer = null;
                InitHitBoxManager();
                Repaint();
                return;
            }

            GUIRenderInspector();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void GUIRenderInspector()
        {
            EditorGUILayout.Separator();
            if (m_target != null && m_Inspector != null)
            {
                RenderInspector(m_Inspector);
            }
        }

        void RenderInspector(HitBoxManagerInspector inspector)
        {
            inspector.showColliders = m_showColliders;
            inspector.showAnimation = m_showAnimation;
            inspector.showAttackData = m_showAttackData;
            inspector.showFrameData = m_showFrameData;
            inspector.showMove = m_showMove;
            inspector.showEvents = m_showEvents;

            inspector.OnInspectorGUI();

            m_showColliders = inspector.showColliders;
            m_showAnimation = inspector.showAnimation;
            m_showAttackData = inspector.showAttackData;
            m_showFrameData = inspector.showFrameData;
            m_showMove = inspector.showMove;
            m_showEvents = inspector.showEvents;
        }

        Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}
