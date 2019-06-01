using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace NKPB
{
#pragma warning disable 0414
    partial class HitBoxManagerInspector
    {
        #region Menu Functions
        void ResetMenu()
        {
            m_isEditorMenuOpen = true;
            m_EditorMenuItem.Clear();
        }

        void ResizeMenu(Vector2 mouse)
        {
            var menuWidth = 120f;

            for (int i = 0; i < m_EditorMenuItem.Count; i++)
            {
                if (string.IsNullOrEmpty(m_EditorMenuItem[i].label))continue;
                menuWidth = Mathf.Max(menuWidth, m_EditorMenuItem[i].label.Length * 7.5f);
            }

            for (int i = 0; i < m_EditorMenuItem.Count; i++)
                m_EditorMenuItem[i].width = menuWidth;

            m_EditorMenuRect = new Rect(mouse, new Vector2(menuWidth, 16 * m_EditorMenuItem.Count));
        }

        void CloseMenu()
        {
            m_isEditorMenuOpen = false;
        }

        void CreateEditGizmoMenu(Vector2 pivot, Vector2 mouse)
        {
            // ResetMenu();
            // m_EditorMenuItem.Add(new EditorMenuItem("Apply pivot to all frames", 0, (int i) => OnClickApplyPivot(pivot), mouse));
            // ResizeMenu(mouse);
        }

        void CreateEditFrameMenu(int frame, Vector2 mouse)
        {
            ResetMenu();

            m_EditorMenuItem.Add(new EditorMenuItem("Add Event", 0, null, mouse, FontStyle.Italic));

            for (int i = 0; i < m_EventLabels.Length; i++)
                m_EditorMenuItem.Add(new EditorMenuItem(m_EventLabels[i],
                    i + 1,
                    (int index) => OnClickEventLabel(frame, index - 1),
                    mouse));

            ResizeMenu(mouse);
        }

        void CreateColliderEditMenu(Vector2 mouse)
        {
            int i;

            ResetMenu();

            for (i = 0; i < m_TypeLabels.Length; i++)
                m_EditorMenuItem.Add(new EditorMenuItem("Set: " + m_TypeLabels[i], i, OnClickTypeLabel, mouse));

            m_EditorMenuItem.Add(new EditorMenuItem(null, i++, null, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem("Copy Collider", i++, OnClickCopy, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem("Delete Collider", i++, OnClickDelete, mouse));
            ResizeMenu(mouse);
        }

        void CreateEditMenu(Vector2 mouse)
        {
            int i;
            bool showPrev = SelectedFrame > 0 && Animations[SelectedAnimation].framedata[SelectedFrame - 1].collider.Length > 0,
                showNext = SelectedFrame + 1 < Animations[SelectedAnimation].framedata.Length
                && Animations[SelectedAnimation].framedata[SelectedFrame + 1].collider.Length > 0;

            ResetMenu();

            for (i = 0; i < m_TypeLabels.Length; i++)
                m_EditorMenuItem.Add(new EditorMenuItem("Create: " + m_TypeLabels[i], i, OnClickCreate, mouse));

            m_EditorMenuItem.Add(new EditorMenuItem(null, i++, null, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem("Paste Collider", i++, OnClickPaste, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem(null, i++, null, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem("Copy Frame", i++, OnClickCopyFrame, mouse));
            if (m_FrameClipboard != new Vector2Int(-1, -1))
                m_EditorMenuItem.Add(new EditorMenuItem("Paste Frame", i++, OnClickPasteFrame, mouse));

            if (showPrev || showNext)
                m_EditorMenuItem.Add(new EditorMenuItem(null, i++, null, mouse));

            if (showPrev)
                m_EditorMenuItem.Add(new EditorMenuItem("Copy from prev frame", i++, CopyCollidersFromPreviousFrame, mouse));

            if (showNext)
                m_EditorMenuItem.Add(new EditorMenuItem("Copy from next frame", i++, CopyCollidersFromNextFrame, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem(null, i++, null, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem("Delete All Collider", i++, OnClickAllDelete, mouse));
            ResizeMenu(mouse);
        }

        void OnClickTypeLabel(int index)
        {
            var type = (HitboxType)index;
            Animations[SelectedAnimation].framedata[SelectedFrame].collider[SelectedCollider].type = type;
            CloseMenu();
        }

        void OnClickEventLabel(int fId, int eId)
        {
            var e = (FrameEvent)eId;
            var list = new List<HitboxFrameEventData>(Animations[SelectedAnimation].framedata[fId].events);

            list.Add(new HitboxFrameEventData { id = e });
            Animations[SelectedAnimation].framedata[fId].events = list.ToArray();

            CloseMenu();
        }

        void OnClickCreate(int index)
        {
            var list = new List<HitboxColliderData>(Animations[SelectedAnimation].framedata[SelectedFrame].collider);

            list.Add(new HitboxColliderData
            {
                type = (HitboxType)index,
                    rect = new RectInt(new Vector2Int(Mathf.RoundToInt((m_EditorMenuRect.position.x - m_PreviewOrigin.x) / m_EditorScale),
                            Mathf.RoundToInt((m_PreviewOrigin.y - m_EditorMenuRect.position.y) / m_EditorScale - 16)),
                        new Vector2Int(16, 16))
            });

            Animations[SelectedAnimation].framedata[SelectedFrame].collider = list.ToArray();
            SelectedCollider = list.Count - 1;
            CloseMenu();
        }
#if UNITY_2017_4_OR_NEWER
        Vector2Int m_FrameClipboard = new Vector2Int(-1, -1);
#else
        Vector2 m_FrameClipboard = new Vector2(-1, -1);
#endif

        HitboxColliderData m_ColliderClipboard = new HitboxColliderData
        {
            rect = new RectInt(0, 0, 16, 16),
        };

        void OnClickCopyFrame(int index)
        {
#if UNITY_2017_4_OR_NEWER
            m_FrameClipboard = new Vector2Int(SelectedAnimation, SelectedFrame);
#else
            m_FrameClipboard = new Vector2(SelectedAnimation, SelectedFrame);
#endif
            CloseMenu();
        }

        void OnClickCopy(int index)
        {
            m_ColliderClipboard = Animations[SelectedAnimation].framedata[SelectedFrame].collider[SelectedCollider];
            CloseMenu();
        }

        void OnClickPasteFrame(int index)
        {
#if UNITY_2017_4_OR_NEWER
            if (m_FrameClipboard == new Vector2Int(-1, -1)) { CloseMenu(); return; }
            var other = new List<HitboxColliderData>(Animations[m_FrameClipboard.x].framedata[m_FrameClipboard.y].collider);
#else
            if (m_FrameClipboard == new Vector2(-1, -1)) { CloseMenu(); return; }
            var other = new List<HitboxColliderData>(Animation[(int)m_FrameClipboard.x].framedata[(int)m_FrameClipboard.y].collider);
#endif
            var list = new List<HitboxColliderData>(Animations[SelectedAnimation].framedata[SelectedFrame].collider);
            var keep = new List<HitboxColliderData>(other.Count);

            foreach (var colliderA in other)
            {
                bool hit = false;

                foreach (var colliderB in list)
                    if (colliderA.rect.position == colliderB.rect.position
                        && colliderA.rect.size == colliderB.rect.size
                        && colliderA.type == colliderB.type)
                    {
                        hit = true;
                        break;
                    }

                if (hit == false)
                    keep.Add(colliderA);
            }

            list.AddRange(keep);
            Animations[SelectedAnimation].framedata[SelectedFrame].collider = list.ToArray();
            CloseMenu();
        }

        void OnClickPaste(int index)
        {
            var list = new List<HitboxColliderData>(Animations[SelectedAnimation].framedata[SelectedFrame].collider);

            list.Add(m_ColliderClipboard);
            Animations[SelectedAnimation].framedata[SelectedFrame].collider = list.ToArray();

            CloseMenu();
        }

        void OnClickDelete(int index)
        {
            var list = new List<HitboxColliderData>(Animations[SelectedAnimation].framedata[SelectedFrame].collider);

            list.RemoveAt(SelectedCollider);
            Animations[SelectedAnimation].framedata[SelectedFrame].collider = list.ToArray();
            CloseMenu();
        }

        void OnClickAllDelete(int index)
        {
            var list = new List<HitboxColliderData>(Animations[SelectedAnimation].framedata[SelectedFrame].collider);

            list.Clear();
            Animations[SelectedAnimation].framedata[SelectedFrame].collider = list.ToArray();
            CloseMenu();
        }

        void OnClickApplyPivot(Vector2 pivot)
        {
            if (m_Renderer == null)
                m_Renderer = m_targetManager.GetComponent<SpriteRenderer>();

            var path = AssetDatabase.GetAssetPath(m_Renderer.sprite);

            TextureImporter import = (TextureImporter)AssetImporter.GetAtPath(path);
            if (import.spriteImportMode == SpriteImportMode.Multiple && import.spritesheet != null)
            {
                var metadata = import.spritesheet;

                for (int i = 0; i < metadata.Length; i++)
                    metadata[i].pivot = pivot;

                import.spritesheet = metadata;
                EditorUtility.SetDirty(import);
                import.SaveAndReimport();
            }

            CloseMenu();
        }

        void CopyCollidersFromPreviousFrame(int index)
        {
            CopyCollidersFromFrame(-1);
        }

        void CopyCollidersFromNextFrame(int index)
        {
            CopyCollidersFromFrame(1);
        }

        void CopyCollidersFromFrame(int delta)
        {
            var list = new List<HitboxColliderData>(Animations[SelectedAnimation].framedata[SelectedFrame].collider);
            var other = new List<HitboxColliderData>(Animations[SelectedAnimation].framedata[SelectedFrame + delta].collider);
            var keep = new List<HitboxColliderData>(other.Count);

            foreach (var colliderA in other)
            {
                bool hit = false;

                foreach (var colliderB in list)
                    if (colliderA.rect.position == colliderB.rect.position
                        && colliderA.rect.size == colliderB.rect.size
                        && colliderA.type == colliderB.type)
                    {
                        hit = true;
                        break;
                    }

                if (hit == false)
                    keep.Add(colliderA);
            }

            list.AddRange(keep);
            Animations[SelectedAnimation].framedata[SelectedFrame].collider = list.ToArray();
            CloseMenu();
        }
        #endregion
    }
}
