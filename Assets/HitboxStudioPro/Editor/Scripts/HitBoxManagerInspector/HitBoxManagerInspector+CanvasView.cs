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
        readonly string PROJECTILE = "PROJECTILE";

        /// <summary>
        /// Setup a matrix of values to multiply by; so dragging moves the rect properly
        /// Starts at top left, then travels clockwise, and center is last
        /// </summary>
        EditorHandle[] m_ColliderDragHandle = new EditorHandle[9]
        {
            new EditorHandle(new Vector4(1f, 0f, -1f, -1f)),
            new EditorHandle(new Vector4(0f, 0f, 0f, -1f)),
            new EditorHandle(new Vector4(0f, 0f, 1f, -1f)),
            new EditorHandle(new Vector4(0f, 0f, 1f, 0f)),
            new EditorHandle(new Vector4(0f, -1f, 1f, 1f)),
            new EditorHandle(new Vector4(0f, -1f, 0f, 1f)),
            new EditorHandle(new Vector4(1f, -1f, -1f, 1f)),
            new EditorHandle(new Vector4(1f, 0f, -1f, 0f)),
            new EditorHandle(new Vector4(1f, -1f, 0f, 0f))
        };

        /// <summary>
        /// Just assuming the user will never have more than 12 gizmos in a single frame here. If you're an absolute madman you can increase this.
        /// </summary>
        EditorHandle[] m_GizmoDragHandle = new EditorHandle[24]
        {
            new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle(),
            new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle(),
            new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle(),
            new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle(), new EditorHandle()
        };

        EditorHandle m_ActiveHandle = null;
        EditorHandle m_MouseOverHandle = null;
        EditorHandle m_PivotHandle = new EditorHandle();

        string[] m_TypeLabels = Enum.GetNames(typeof(HitboxType));
        string[] m_EventLabels = Enum.GetNames(typeof(FrameEvent));
        bool m_isEditorMenuOpen = false;
        Rect m_EditorMenuRect = Rect.zero;
        List<EditorMenuItem> m_EditorMenuItem = new List<EditorMenuItem>(18);
        float m_EditorScale = 1f;
        Vector2 m_PreviewOrigin = Vector2.zero;
        bool m_isDragTimeline = false;

        /// <summary>
        /// タイムライン描画
        /// </summary>
        /// <param name="position"></param>
        /// <param name="width"></param>
        /// <param name="mouse"></param>
        public void DrawEditorTimeline(Vector2 position, float width, Vector2 mouse)
        {
            UpdateSerializedObject();

            if (Animations == null
                || SelectedAnimation >= Animations.Length
                || Animations[SelectedAnimation].framedata == null
                || SelectedFrame >= Animations[SelectedAnimation].framedata.Length)
            {
                return;
            }

            RepairFrameData();

            EventType eventType = Event.current.type;
            int eventButton = Event.current.button;
            bool isRepaint = (eventType == EventType.Repaint);

            if (eventType == EventType.MouseDown
                && m_isEditorMenuOpen
                && eventButton == 0)
            {
                if (m_EditorMenuRect.Contains(mouse))
                {
                    for (int i = 0; i < m_EditorMenuItem.Count; i++)
                    {
                        m_EditorMenuItem[i].ProcessEvent(mouse, eventType, eventButton);
                    }
                    //Since they clicked inside the menu we're going to assume they chose an option
                    //and return so we don't accidentally click something beneath the menu.
                    Event.current.button = 3;
                    return;
                }
                else
                {
                    m_isEditorMenuOpen = false;
                    return;
                }
            }

            DrawTimeLine(position, width, mouse, isRepaint);

            DrawTimeLineTack(position, width, mouse, eventType, eventButton, isRepaint);

            ApplySerializedProperties();
        }

        /// <summary>
        /// タイムライン描画
        /// </summary>
        /// <param name="position"></param>
        /// <param name="width"></param>
        /// <param name="mouse"></param>
        /// <param name="isRepaint"></param>
        private void DrawTimeLine(Vector2 position, float width, Vector2 mouse, bool isRepaint)
        {
            if (!isRepaint)
                return;

            LineDrawer.DrawLine(position, position + new Vector2(width, 0f), 6, Color.black);
            LineDrawer.DrawLine(position, position + new Vector2(width, 0f), 4, Color.gray);
            LabelDrawer.DrawLabel("Colliders", new Vector2(position.x - 40, position.y - 15), 8);
            LabelDrawer.DrawLabel("Frame", new Vector2(position.x - 40, position.y - 5), 8);
            LabelDrawer.DrawLabel("Events", new Vector2(position.x - 40, position.y + 5), 8);

            //Draw right click menu
            if (m_isEditorMenuOpen)
            {
                EditorGUI.DrawRect(new Rect(m_EditorMenuRect.x - 1,
                    m_EditorMenuRect.y - 1,
                    m_EditorMenuRect.width + 2,
                    m_EditorMenuRect.height + 2), Color.black);

                for (int i = 0; i < m_EditorMenuItem.Count; i++)
                {
                    m_EditorMenuItem[i].Draw(mouse);
                }
            }
        }

        /// <summary>
        /// タイムラインタック描画
        /// </summary>
        /// <param name="position"></param>
        /// <param name="width"></param>
        /// <param name="mouse"></param>
        /// <param name="eventType"></param>
        /// <param name="eventButton"></param>
        /// <param name="isRepaint"></param>
        private void DrawTimeLineTack(Vector2 position, float width, Vector2 mouse, EventType eventType, int eventButton, bool isRepaint)
        {
            var animation = Animations[SelectedAnimation];
            var frames = animation.framedata;
            var length = frames.Length;

            const int TACK_SIZE = 15;
            const int EVENT_SIZE = 10;

            for (int i = 0; i < length; i++)
            {
                var normalizedTime = frames[i].time / animation.clip.length;
                var frameRect = new Rect(position.x + normalizedTime * width, position.y - 5, TACK_SIZE, TACK_SIZE);
                var innerframeRect = new Rect(frameRect.x + 1, frameRect.y + 1, frameRect.width - 2, frameRect.height - 2);
                bool isFrameContains = frameRect.Contains(mouse);

                if (isRepaint)
                {
                    EditorGUI.DrawRect(frameRect, isFrameContains ? new Color(0.2f, 0.05f, 0.2f, 1f) : Color.black);
                    EditorGUI.DrawRect(innerframeRect, isFrameContains ? Color.cyan : (SelectedFrame == i ? Color.magenta : Color.white));
                    EditorGUIUtility.AddCursorRect(frameRect, MouseCursor.Link);

                    if (frames[i].collider != null)
                    {
                        int count = frames[i].collider.Length;
                        if (count > 0)
                        {
                            LabelDrawer.DrawLabel(count.ToString(), new Vector2(position.x + 2f + normalizedTime * width, position.y - 15), 8);
                        }
                    }
                }
                else if (eventType == EventType.MouseDown
                    && isFrameContains)
                {
                    if (eventButton == 0)
                    {
                        SelectedFrame = i;
                        m_isDragTimeline = true;
                    }
                    else if (eventButton == 1)
                    {
                        CreateEditFrameMenu(i, mouse);
                    }
                }
                else if (eventType == EventType.MouseUp
                    && eventButton == 0)
                {
                    m_isDragTimeline = false;
                }
                else if (eventType == EventType.MouseDrag
                    && m_isDragTimeline
                    && isFrameContains
                    && eventButton == 0)
                {
                    SelectedFrame = i;
                }

                if (frames[i].events != null)
                {
                    for (int j = 0; j < frames[i].events.Length; j++)
                    {
                        var eventRect = new Rect(position.x + normalizedTime * width, position.y - 1 + EVENT_SIZE * (j + 1) * 1.5f, EVENT_SIZE, EVENT_SIZE);
                        var innereventRect = new Rect(eventRect.x + 1, eventRect.y + 1, eventRect.width - 2, eventRect.height - 2);
                        bool eventContains = eventRect.Contains(mouse);

                        if (isRepaint)
                        {
                            EditorGUI.DrawRect(eventRect, eventContains ? new Color(0.2f, 0.05f, 0.2f, 1f) : Color.black);
                            EditorGUI.DrawRect(innereventRect, eventContains ? Color.cyan : (SelectedFrame == i ? Color.yellow : Color.white));

                            EditorGUIUtility.AddCursorRect(eventRect, MouseCursor.ArrowMinus);

                            Color col = (eventContains)
                                ? Color.cyan
                                : Color.white;
                            // if (eventContains)
                            {
                                LabelDrawer.DrawLabelColor(frames[i].events[j].id.ToString(), new Vector2(eventRect.position.x + 10f, eventRect.position.y), col, 8);
                            }
                        }
                        else if (eventType == EventType.MouseDown && eventContains && eventButton == 1)
                        {
                            var list = new List<HitboxFrameEventData>(Animations[SelectedAnimation].framedata[i].events);

                            list.RemoveAt(j);
                            Animations[SelectedAnimation].framedata[i].events = list.ToArray();
                            //If we accidentally opened a menu while deleting this event lets close it.
                            if (m_isEditorMenuOpen)
                            {
                                CloseMenu();
                            }
                        }
                    }
                }
            }
        }

        void RepairFrameData()
        {
            var length = m_targetManager.GetNumFrames(SelectedAnimation);
            var diff = length - m_targetManager.m_Animations[SelectedAnimation].framedata.Length;

            if (diff != 0)
            {
                var list = new List<HitboxAnimationFrame>(m_targetManager.m_Animations[SelectedAnimation].framedata);

                while (diff < 0)
                {
                    list.RemoveAt(list.Count - 1);
                    diff++;
                }

                while (diff > 0)
                {
                    list.Add(new HitboxAnimationFrame { collider = new HitboxColliderData[0], events = new HitboxFrameEventData[0] });
                    diff--;
                }

                m_targetManager.m_Animations[SelectedAnimation].framedata = list.ToArray();
            }
            else
            {
                EditorCurveBinding[] curves = AnimationUtility.GetObjectReferenceCurveBindings(m_targetManager.m_Animations[SelectedAnimation].clip);

                for (int i = 0; i < curves.Length; i++)
                {
                    if (AnimClipUtils.CheckSpriteCurve(curves[i]))
                    {
                        var keyframes = AnimationUtility.GetObjectReferenceCurve(m_targetManager.m_Animations[SelectedAnimation].clip, curves[i]);

                        for (int j = 0; j < m_targetManager.m_Animations[SelectedAnimation].framedata.Length && j < keyframes.Length; j++)
                            m_targetManager.m_Animations[SelectedAnimation].framedata[j].time = keyframes[j].time;
                    }
                }

            }
        }

        /// <summary>
        /// ギズモ描画
        /// </summary>
        /// <param name="position"></param>
        /// <param name="pivot"></param>
        /// <param name="mouse"></param>
        /// <param name="scale"></param>
        public void DrawEditorGizmos(Vector2 position, Vector2 pivot, Vector2 mouse, float scale)
        {
            UpdateSerializedObject();

            if (Animations == null
                || SelectedAnimation >= Animations.Length
                || Animations[SelectedAnimation].framedata == null
                || SelectedFrame >= Animations[SelectedAnimation].framedata.Length)
            {
                return;
            }

            var framedata = Animations[SelectedAnimation].framedata[SelectedFrame];
            var eventType = Event.current.type;
            var eventButton = Event.current.button;

            m_EditorScale = scale;
            m_PreviewOrigin = position;

            if (framedata.events != null)
            {
                int currGizmo = 0;

                for (int i = 0; i < framedata.events.Length; i++)
                {
                    if (framedata.events[i].id.ToString().Contains(PROJECTILE))
                    {
                        var origin = IntConverter.DecodeIntToVector2(framedata.events[i].intParam) * scale;
                        var dest = IntConverter.DecodeIntToVector2((int)framedata.events[i].floatParam) * scale;

                        m_MouseOverHandle = null;
                        //If the dots are overlapping we're going to force them apart
                        if (dest == origin)
                            dest.x += 20 * scale;

                        if (eventType == EventType.Repaint)
                        {
                            var a = new Vector2(-1f, 1f);
                            var b = new Vector2(-1f, -1f);
                            var direction = Vector3.Normalize(dest - origin);
#if UNITY_2017_1_OR_NEWER
                            var rotation = Quaternion.AngleAxis(Vector2.SignedAngle(direction, Vector2.right), -Vector3.forward);
#else
                            var angle = Vector2.Angle(direction, Vector2.right);
                            var cross = Vector3.Cross(direction, Vector3.right);
                            var rotation = Quaternion.AngleAxis(cross.z < 0 ? -angle : angle, -Vector3.forward);
#endif
                            a = rotation * a;
                            b = rotation * b;

                            LineDrawer.DrawLine(origin + position, dest + position, 6, Color.black);
                            LineDrawer.DrawLine(dest + position, dest + position + a * 11f, 6, Color.black);
                            LineDrawer.DrawLine(dest + position, dest + position + b * 11f, 6, Color.black);

                            LineDrawer.DrawLine(origin + position, dest + position, 4, Color.white);
                            LineDrawer.DrawLine(dest + position, dest + position + a * 10f, 4, Color.white);
                            LineDrawer.DrawLine(dest + position, dest + position + b * 10f, 4, Color.white);

                            LabelDrawer.DrawLabel(framedata.events[i].id.ToString(), origin + position + new Vector2(0f, 6f));
                        }

                        if (m_GizmoDragHandle[currGizmo * 2 + 0].Draw(position + origin, mouse, MouseCursor.Pan))
                        {
                            var index = i;
                            m_GizmoDragHandle[currGizmo * 2 + 0].action = (Vector2 v) =>
                                Animations[SelectedAnimation].framedata[SelectedFrame].events[index].intParam = IntConverter.EncodeVector2ToInt(v / scale);
                            m_MouseOverHandle = m_GizmoDragHandle[currGizmo * 2 + 0];
                        }
                        if (m_GizmoDragHandle[currGizmo * 2 + 1].Draw(position + dest, mouse, MouseCursor.Pan))
                        {
                            var index = i;
                            m_GizmoDragHandle[currGizmo * 2 + 1].action = (Vector2 v) =>
                                Animations[SelectedAnimation].framedata[SelectedFrame].events[index].floatParam = IntConverter.EncodeVector2ToInt(v / scale);
                            m_MouseOverHandle = m_GizmoDragHandle[currGizmo * 2 + 1];
                        }

                        if (eventType == EventType.MouseDown && m_MouseOverHandle != null && eventButton == 0)
                        {
                            m_ActiveHandle = m_MouseOverHandle;
                            //remove this click from the event so we don't click through other gizmos beneath us.
                            eventButton = 3;
                        }
                        else if (eventType == EventType.MouseUp && eventButton == 0)
                            m_ActiveHandle = null;
                        else if (eventType == EventType.MouseDrag && m_ActiveHandle != null && m_ActiveHandle.action != null)
                            m_ActiveHandle.action(mouse - position);

                        currGizmo++;
                    }
                }
            }

            if (m_PivotHandle.Draw(m_ActiveHandle == m_PivotHandle ? mouse : position, mouse, MouseCursor.Pan)
                && m_ActiveHandle == null)
            {
                m_MouseOverHandle = m_PivotHandle;
            }
            else if (m_MouseOverHandle == m_PivotHandle)
            {
                m_MouseOverHandle = null;
            }

            // if (m_ActiveHandle == null && m_MouseOverHandle == m_PivotHandle && eventType == EventType.MouseDown && eventButton == 0)
            // {
            //     m_ActiveHandle = m_MouseOverHandle;
            // }
            // else if (m_ActiveHandle != null && m_ActiveHandle == m_PivotHandle && eventType == EventType.MouseUp)
            // {
            //     if (m_Renderer == null)
            //         m_Renderer = m_targetManager.GetComponent<SpriteRenderer>();
            //     var path = AssetDatabase.GetAssetPath(m_Renderer.sprite);
            //     TextureImporter import = (TextureImporter)AssetImporter.GetAtPath(path);
            //     if (import.spriteImportMode == SpriteImportMode.Multiple && import.spritesheet != null)
            //     {
            //         var metadata = import.spritesheet;
            //         import.isReadable = true;
            //         for (int i = 0; i < metadata.Length; i++)
            //             if (metadata[i].name.Equals(m_Renderer.sprite.name))
            //             {
            //                 var p = (mouse - (position - pivot)) / scale;
            //                 p.x = Mathf.Round(p.x) / metadata[i].rect.width;
            //                 p.y = 1f - (Mathf.Round(p.y) / metadata[i].rect.height);
            //                 metadata[i].pivot = p;
            //                 break;
            //             }
            //         m_ActiveHandle = m_MouseOverHandle = null;
            //         import.spritesheet = metadata;
            //         EditorUtility.SetDirty(import);
            //         import.SaveAndReimport();
            //     }
            //     m_ActiveHandle = m_MouseOverHandle = null;
            // }

            Vector2 alignedPos = (m_ActiveHandle == m_PivotHandle)
                ? mouse - (position - pivot)
                : pivot;

            string labelText = "Pivot (" + Mathf.RoundToInt(alignedPos.x / scale)
                + ", " + Mathf.RoundToInt(alignedPos.y / scale) + ")";
            Vector2 labelPos = (m_ActiveHandle == m_PivotHandle)
                ? mouse
                : position;
            labelPos += new Vector2(-15f, 6f);
            LabelDrawer.DrawLabel(labelText, labelPos);

            if (m_isEditorMenuOpen == false && eventType == EventType.MouseDown && eventButton == 1)
            {
                if (m_Renderer == null)
                    m_Renderer = m_targetManager.GetComponent<SpriteRenderer>();

                var size = m_Renderer.sprite.rect.size;
                var pivotPixels = m_Renderer.sprite.pivot;

                CreateEditGizmoMenu(new Vector2(pivotPixels.x / size.x, pivotPixels.y / size.y), mouse);
            }

            ApplySerializedProperties();
        }

        /// <summary>
        /// コライダー描画
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="pivot"></param>
        /// <param name="pan"></param>
        /// <param name="mouse"></param>
        /// <param name="scale"></param>
        public void DrawColliderRects(Vector2 pos, Vector2 pivot, Vector2 mouse, float scale)
        {
            UpdateSerializedObject();

            if (Animations == null || SelectedAnimation >= Animations.Length
                || Animations[SelectedAnimation].framedata == null || SelectedFrame >= Animations[SelectedAnimation].framedata.Length)return;
            var framedata = Animations[SelectedAnimation].framedata[SelectedFrame];
            var colliderArray = framedata.collider;
            var eventType = Event.current.type;
            var eventButton = Event.current.button;

            m_EditorScale = scale;
            m_PreviewOrigin = pos;

            for (int i = 0; i < colliderArray.Length; i++)
            {
                var collider = colliderArray[i];
                var color = HitboxSettings.COLOR(collider.type);

                color.a = 0.75f;
                collider.rect.x *= (int)scale;
                collider.rect.y *= (int)scale;
                collider.rect.width *= (int)scale;
                collider.rect.height *= (int)scale;

                Rect rect = new Rect(collider.rect.x - collider.rect.width / 2,
                    collider.rect.y + collider.rect.height / 2,
                    collider.rect.width,
                    collider.rect.height);

                if (m_Renderer != null && m_Renderer.flipX)
                {
                    rect.x *= -1;
                    rect.width *= -1;
                }

                var cRect = new Rect(pos.x + (rect.x), pos.y - (rect.y), rect.width, rect.height);

                if (i != SelectedCollider)
                {
                    if (m_MouseOverHandle == null && m_ActiveHandle == null && cRect.Contains(mouse))
                    {
                        color.a = 0.5f;

                        if (eventType == EventType.MouseDown)
                        {
                            if (eventButton < 2)
                                SelectedCollider = i;
                            if (eventButton == 1)
                                CreateColliderEditMenu(mouse);
                            else if (eventButton == 0)
                                m_isEditorMenuOpen = false;

                            return;
                        }
                    }
                    else
                        color.a = 0.25f;
                }

                if (eventType == EventType.Repaint)
                    EditorGUI.DrawRect(cRect, color);

                //If we are editing this collider we need to display the editor doo dads
                if (SelectedCollider == i)
                {
                    m_MouseOverHandle = null;

                    if (m_ColliderDragHandle[0].Draw(new Vector2(cRect.x, cRect.y), mouse, MouseCursor.ResizeUpLeft))
                        m_MouseOverHandle = m_ColliderDragHandle[0];

                    if (m_ColliderDragHandle[1].Draw(new Vector2(cRect.x + cRect.width / 2f, cRect.y), mouse, MouseCursor.ResizeVertical))
                        m_MouseOverHandle = m_ColliderDragHandle[1];

                    if (m_ColliderDragHandle[2].Draw(new Vector2(cRect.x + cRect.width, cRect.y), mouse, MouseCursor.ResizeUpRight))
                        m_MouseOverHandle = m_ColliderDragHandle[2];

                    if (m_ColliderDragHandle[3].Draw(new Vector2(cRect.x + cRect.width, cRect.y + cRect.height / 2f), mouse, MouseCursor.ResizeHorizontal))
                        m_MouseOverHandle = m_ColliderDragHandle[3];

                    if (m_ColliderDragHandle[4].Draw(new Vector2(cRect.x + cRect.width, cRect.y + cRect.height), mouse, MouseCursor.ResizeUpLeft))
                        m_MouseOverHandle = m_ColliderDragHandle[4];

                    if (m_ColliderDragHandle[5].Draw(new Vector2(cRect.x + cRect.width / 2f, cRect.y + cRect.height), mouse, MouseCursor.ResizeVertical))
                        m_MouseOverHandle = m_ColliderDragHandle[5];

                    if (m_ColliderDragHandle[6].Draw(new Vector2(cRect.x, cRect.y + cRect.height), mouse, MouseCursor.ResizeUpRight))
                        m_MouseOverHandle = m_ColliderDragHandle[6];

                    if (m_ColliderDragHandle[7].Draw(new Vector2(cRect.x, cRect.y + cRect.height / 2f), mouse, MouseCursor.ResizeHorizontal))
                        m_MouseOverHandle = m_ColliderDragHandle[7];

                    if (m_ColliderDragHandle[8].Draw(new Vector2(cRect.x + cRect.width / 2f, cRect.y + cRect.height / 2f), mouse, MouseCursor.Pan))
                        m_MouseOverHandle = m_ColliderDragHandle[8];

                    if (eventType == EventType.Repaint)
                    {
                        LabelDrawer.DrawLabel(collider.type.ToString(), new Vector2(cRect.x + 4, cRect.y + 4));
                        LabelDrawer.DrawLabel("X: " + colliderArray[i].rect.x, new Vector2(cRect.x + 4, cRect.y + 22), 10);
                        LabelDrawer.DrawLabel("Y: " + colliderArray[i].rect.y, new Vector2(cRect.x + 4, cRect.y + 34), 10);
                        LabelDrawer.DrawLabel("W: " + colliderArray[i].rect.width, new Vector2(cRect.x + 4, cRect.y + 46), 10);
                        LabelDrawer.DrawLabel("H: " + colliderArray[i].rect.height, new Vector2(cRect.x + 4, cRect.y + 58), 10);
                    }

                    if (eventType == EventType.MouseDown)
                    {
                        if (m_MouseOverHandle != null && eventButton == 0)
                            m_ActiveHandle = m_MouseOverHandle;
                        else if (m_isEditorMenuOpen == false && eventButton == 1 && cRect.Contains(mouse))
                            CreateColliderEditMenu(mouse);
                    }
                    else if (eventType == EventType.MouseUp && eventButton == 0)
                    {
                        m_ActiveHandle = null;
                    }
                    else if (eventType == EventType.MouseDrag && m_ActiveHandle != null)
                    {
                        var delta = m_ActiveHandle.GetDragRect(mouse) / scale;

                        colliderArray[SelectedCollider].rect.x += Mathf.RoundToInt(delta.x);
                        colliderArray[SelectedCollider].rect.y += Mathf.RoundToInt(delta.y);
                        colliderArray[SelectedCollider].rect.width += Mathf.RoundToInt(delta.z);
                        colliderArray[SelectedCollider].rect.height += Mathf.RoundToInt(delta.w);
                    }
                }
            }

            if (m_isEditorMenuOpen == false && eventType == EventType.MouseDown && eventButton == 1)
                CreateEditMenu(mouse);

            ApplySerializedProperties();
        }

    }
}
