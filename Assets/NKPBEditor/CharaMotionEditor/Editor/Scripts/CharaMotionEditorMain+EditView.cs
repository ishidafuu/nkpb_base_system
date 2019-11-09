using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKPB
{

    public partial class CharaMotionEditorMain
    {
        void GUIEditView()
        {
            DrawCharacter();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Edit View", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            GUIHitboxAndGizmosToggle();
            GUISettingButton();

            GUIZoom();
            GUIResetViewButton();
            EditorGUILayout.EndHorizontal();
            GUICreateObjectButton();
            EditorGUILayout.EndVertical();
            GUITimeLine();
        }

        void GUIHitboxAndGizmosToggle()
        {
            m_showColliders = GUILayout.Toggle(m_showColliders && !m_showGizmos, "Edit Hitboxes");
            m_showGizmos = GUILayout.Toggle(m_showGizmos && !m_showColliders, "Edit Gizmos");
        }

        void GUISettingButton()
        {
            if (GUILayout.Button("Settings", GUILayout.Width(100f)))
                SettingsEditorWindow.Init();
        }

        void GUITimeLine()
        {
            if (m_Inspector != null)
                m_Inspector.DrawEditorTimeline(new Vector2(60, 90f),
                    position.width - m_toolbarWidth - 80,
                    Event.current.mousePosition);
        }

        void GUIZoom()
        {
            GUILayout.Label("Zoom", EditorStyles.boldLabel, GUILayout.MaxWidth(40));
            m_previewZoom = Mathf.Round(Mathf.Clamp(GUILayout.HorizontalSlider(m_previewZoom, MINZOOM, MAXZOOM, GUILayout.MaxWidth(100)), MINZOOM, MAXZOOM));

            if (m_currentEvent.type == EventType.ScrollWheel)
            {
                var delta = (m_currentEvent.delta.y * -1f / Mathf.Abs(m_currentEvent.delta.y));

                m_previewZoom = Mathf.Clamp(m_previewZoom + delta, MINZOOM, MAXZOOM);
                Repaint();
            }
            else if (m_currentEvent.type == EventType.MouseDrag && m_currentEvent.button == 2)
            {
                m_previewPan += m_currentEvent.delta;
            }
        }

        void GUIResetViewButton()
        {
            if (GUILayout.Button("Reset View", GUILayout.MaxWidth(80)))
            {
                m_previewZoom = 2f;
                m_previewPan = Vector2.zero;
                Repaint();
            }
        }

        void DrawCharacter()
        {
            if (m_SpriteRenderer == null)
                return;

            RenderSprite(m_SpriteRenderer);
        }

        void RenderSprite(SpriteRenderer renderer, int highlightIndex = -1)
        {
            if (renderer == null || renderer.sprite == null)
                return;

            float scale = m_previewZoom;
            float canvasSize = 64 * scale;
            float spriteWidth = renderer.sprite.rect.width * scale;
            float spriteHeight = renderer.sprite.rect.height * scale;

            Vector2 pivot = renderer.sprite.pivot * scale;
            pivot.y = spriteHeight - pivot.y;
            var pos = new Vector2(
                m_toolbarWidth + ((position.width - m_toolbarWidth) / 2f) - (canvasSize / 2f),
                position.height / 2f - (spriteHeight / 2f)) + pivot + m_previewPan;

            if (Event.current.type == EventType.Repaint)
            {
                // 背景描画
                var spriteSize = new Vector2(canvasSize, canvasSize);
                DrawBackGround(pos, spriteSize, highlightIndex);

                // キャラ描画
                Rect spriteRect = new Rect(renderer.sprite.rect.x / renderer.sprite.texture.width,
                    renderer.sprite.rect.y / renderer.sprite.texture.height,
                    renderer.sprite.rect.width / renderer.sprite.texture.width,
                    renderer.sprite.rect.height / renderer.sprite.texture.height);
                GUI.DrawTextureWithTexCoords(new Rect(pos.x - pivot.x, pos.y - pivot.y, spriteWidth, spriteHeight),
                    renderer.sprite.texture, spriteRect);

                // XY線描画
                DrawGridLine3(pos);
            }

            // ギズモ、コライダー描画
            DrawGizmoCollider(scale, pivot, canvasSize, pos);
        }

        private void DrawGizmoCollider(float scale, Vector2 pivot, float spriteHeight, Vector2 pos)
        {
            var normalizedY = pivot.y / spriteHeight;
            normalizedY = 1f - normalizedY;
            pivot.y = normalizedY * spriteHeight;
            var mouse = Event.current.mousePosition;

            if (m_showColliders)
                m_Inspector.DrawColliderRects(pos, pivot, mouse, scale);
            else if (m_showGizmos)
                m_Inspector.DrawEditorGizmos(pos, pivot, mouse, scale);
        }

        // グリッド線を描画
        private void DrawGridLine3(Vector2 pos)
        {
            // grid
            Handles.color = new Color(1f, 1f, 1f, 0.5f);
            //縦線
            {
                Vector2 st = new Vector2(0, -32) * m_previewZoom;
                Vector2 ed = new Vector2(0, 32) * m_previewZoom;
                Handles.DrawLine((pos + st), (pos + ed));
            }
            //横線
            {
                Vector2 st = new Vector2(-32, 0) * m_previewZoom;
                Vector2 ed = new Vector2(32, 0) * m_previewZoom;
                Handles.DrawLine((pos + st), (pos + ed));
            }
        }

        void DrawBackGround(Vector2 position, Vector2 size, int highlightIndex = -1)
        {

            Vector2 actualSize = size;

            var checkerSize = Mathf.CeilToInt(Mathf.Max(size.x, size.y) / 16);
            // var checkerWidth = Mathf.CeilToInt(size.x / 32f);
            // var checkerHeight = Mathf.CeilToInt(size.y / 32f);

            for (int x = 0; x < checkerSize; x++)
            {
                var posx = (x - (checkerSize / 2)) * 32f;
                for (int y = 0; y < checkerSize; y++)
                {
                    var posy = (y - (checkerSize / 2)) * 32f;
                    GUI.DrawTexture(new Rect(position.x + posx,
                        position.y + posy, 32f, 32f), m_editorBackgroundImage);

                }
            }

        }

        void GUICreateObjectButton()
        {
            if (GUILayout.Button("Create Scriptable Object", GUILayout.MaxWidth(180)))
            {
                if (m_hitboxManager == null)
                {
                    Debug.LogWarning("m_hitboxManager == null");
                    return;
                }
                CreateObjectPack();
            }
        }

        void CreateObjectPack()
        {
            CharacterMotionMaster obj = CreateInstance(typeof(CharacterMotionMaster)) as CharacterMotionMaster;

            foreach (var item in m_hitboxManager.m_Animations)
            {
                obj.motionData.Add(CreateMotionDataObject(item));
            }

            AssetDatabase.CreateAsset(obj, CharacterMotionMasterOutputPath);
            Debug.Log("CreateObject:" + CharacterMotionMasterOutputPath);
        }

        CharacterMotionData CreateMotionDataObject(HitboxAnimation hitbox)
        {
            CharacterMotionData res = new CharacterMotionData();

            res.motionName = hitbox.clip.name;
            res.frameData = hitbox.frameData
                .Select(x => new CharacterMotionFrame()
                {
                    collider = x.collider,
                    events = x.events,
                }).ToArray();

            var curves = AnimationUtility.GetObjectReferenceCurveBindings(hitbox.clip);
            foreach (var item in curves)
            {
                if (item.propertyName != "m_Sprite")
                    continue;

                string[] imageNames = AnimationUtility.GetObjectReferenceCurve(hitbox.clip, item)
                    .Select(x => x.value.ToString().Replace(" (UnityEngine.Sprite)", ""))
                    .ToArray();

                for (int i = 0; i < res.frameData.Length; i++)
                {
                    res.frameData[i].imageName = imageNames[i];
                }
            }

            return res;
        }

        // //If we don't have a palette assigned don't even bother with this function
        // void CopySpriteToTexture(Sprite sprite, int highlightIndex = -1)
        // {
        //     var source = sprite.texture;
        //     var rect = sprite.rect;
        //     int width = (int)rect.width;
        //     int height = (int)rect.height;

        //     if (m_BufferTexture == null)
        //     {
        //         m_BufferTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        //         m_BufferTexture.filterMode = FilterMode.Point;
        //         m_BufferTexture.wrapMode = TextureWrapMode.Clamp;
        //     }
        //     else if (m_BufferTexture.width != width || m_BufferTexture.height != height)
        //     {
        //         m_BufferTexture.Resize(width, height);
        //         m_BufferTexture.Apply();
        //     }

        //     var inPixels = source.GetPixels(Mathf.FloorToInt(rect.x), Mathf.FloorToInt(rect.y), width, height);
        //     var outPixels = new Color[width * height];

        //     for (int i = 0; i < width * height; i++)
        //     {
        //         var index = Mathf.Clamp(Mathf.RoundToInt(inPixels[i].r * 255f), 0, palette.Colors.Length - 1);

        //         outPixels[i] = index == highlightIndex ? new Color(1f, 0f, 1f, 1f) : palette.Colors[index];
        //         outPixels[i].a = inPixels[i].a;
        //     }

        //     m_BufferTexture.SetPixels(outPixels);
        //     m_BufferTexture.Apply();
        // }

        // void DrawTexture(Vector2 position, Sprite sprite, Vector2 size)
        // {
        //     Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
        //         sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
        //     Vector2 actualSize = size;

        //     actualSize.y *= (sprite.rect.height / sprite.rect.width);
        //     Graphics.DrawTexture(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), sprite.texture, spriteRect, 0, 0, 0, 0);
        // }

    }
}
