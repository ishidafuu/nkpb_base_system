using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace NKPB
{
#pragma warning disable 0414
    [CustomEditor(typeof(HitboxManager))]
    public partial class HitBoxManagerInspector : Editor
    {
        SerializedObject m_targetObject;
        HitboxManager m_targetManager;
        Color oldBGColor;
        Color oldFGColor;
        public bool showColliders;
        public bool showAudio;
        public bool showMove;
        public bool showAnimation;
        public bool showAttackData;
        public bool showFrameData;
        public bool showLightingData;
        public bool showEvents;

        SerializedProperty m_SelectedAnimation;
        SerializedProperty m_SelectedFrame;
        SerializedProperty m_SelectedCollider;
        SpriteRenderer m_Renderer;
        List<string> m_AnimationOptionLabels = new List<string>();

        int m_LastAnimationID = int.MaxValue;
        int m_LastFrameID = int.MaxValue;

        int SelectedAnimation { get { return m_SelectedAnimation.intValue; } set { m_SelectedAnimation.intValue = value; } }
        int SelectedFrame { get { return m_SelectedFrame.intValue; } set { m_SelectedFrame.intValue = value; } }
        int SelectedCollider { get { return m_targetManager.m_CurrentCollider; } set { m_targetManager.m_CurrentCollider = value; } }
        HitboxAnimation[] Animation { get { return m_targetManager.m_Animations; } }

        public void OnEnable()
        {
            oldBGColor = GUI.backgroundColor;
            oldFGColor = GUI.contentColor;
            if (target == null)
                return;

            m_targetManager = (HitboxManager)target;
            m_targetObject = new SerializedObject(target);
            m_SelectedAnimation = m_targetObject.FindProperty(nameof(m_targetManager.m_CurrentAnimation));
            m_SelectedFrame = m_targetObject.FindProperty(nameof(m_targetManager.m_CurrentFrame));
            m_SelectedCollider = m_targetObject.FindProperty(nameof(m_targetManager.m_CurrentCollider));
        }

        void ResetEditorColors()
        {
            GUI.backgroundColor = oldBGColor;
            GUI.contentColor = oldFGColor;
        }

        void SetEditorColors(Color background, Color content)
        {
            GUI.backgroundColor = background;
            GUI.contentColor = content;
        }

        public void UpdateSerializedObject()
        {
            m_targetObject.Update();
        }

        public override void OnInspectorGUI()
        {

            if (Application.isPlaying)
            {
                GUILayout.Label("Cannot edit animations while the game is running.", EditorStyles.miniTextField);
                return;
            }

            UpdateSerializedObject();

            // HitboxManager m_Target = (HitboxManager)target;
            SerializedProperty Animations = m_targetObject.FindProperty(nameof(m_targetManager.m_Animations));

            EditorGUILayout.BeginVertical();

            RefreshAnimationOptionLabels();

            // GUIAddNewAnimationButton(m_targetManager, Animations);
            GUIAddFromAttachedAnimatorButton(Animations);
            // GUIRemoveCurrentAnimationButton(m_targetManager, Animations);
            ResetEditorColors();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            GUISelectAnimationToEditPopup();
            EditorGUILayout.Separator();

            if (SelectedAnimation < Animations.arraySize)
            {
                SerializedProperty HitboxData = Animations.GetArrayElementAtIndex(SelectedAnimation);
                HitboxAnimation selectedAnimation = m_targetManager.m_Animations[SelectedAnimation];
                if (HitboxData != null)
                {

                    var clip = HitboxData.FindPropertyRelative(nameof(selectedAnimation.clip));
                    var framedata = HitboxData.FindPropertyRelative(nameof(selectedAnimation.framedata));
                    var oldclip = clip.objectReferenceValue;

                    GUIAnimationSettingsLayout(HitboxData, clip);
                    // GUIAttackDataLayout(HitboxData);
                    GUIFrameDataLayout(m_targetManager, clip, framedata, oldclip);
                }
            }

            EditorGUILayout.EndVertical();

            var maxHitboxes = 1;
            if (Animation != null)
                for (int i = 0; i < Animation.Length; i++)
                    if (Animation[i].framedata != null)
                        for (int j = 0; j < Animation[i].framedata.Length; j++)
                            if (Animation[i].framedata[j].collider != null)
                                maxHitboxes = Mathf.Max(maxHitboxes, Animation[i].framedata[j].collider.Length);

            m_targetObject.FindProperty(nameof(m_targetManager.m_MaxHitboxes)).intValue = maxHitboxes;
            ApplySerializedProperties();

            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.MouseMove && Event.current.type != EventType.Layout)
            {
                Profiler.BeginSample("Hitbox Inspector: Bake Animation");
                if (SelectedAnimation < Animations.arraySize)
                    m_targetManager.BakeAnimation(SelectedAnimation);
                //m_Target.BakeAnimations();
                Profiler.EndSample();
            }

            UpdatePreview();

            EditorGUI.indentLevel = 0;
        }

        private void GUIFrameDataLayout(HitboxManager m_Target, SerializedProperty clip, SerializedProperty framedata, UnityEngine.Object oldclip)
        {
            //New animations need their frame data populated
            if (oldclip != clip.objectReferenceValue && clip.objectReferenceValue != null || framedata.arraySize == 0)
            {
                framedata.ClearArray();
                framedata.arraySize = 0;
                ApplySerializedProperties();
                for (int i = 0, j = m_Target.GetNumFrames(SelectedAnimation); i < j; i++)
                {
                    framedata.InsertArrayElementAtIndex(0);
                }
                ApplySerializedProperties();
            }

            GUILayout.Label("Frame Data", EditorStyles.boldLabel);
            // showFrameData = EditorGUILayout.Foldout(showFrameData, "Frame Data", true);
            // if (showFrameData)
            {
                GUILayout.Label("Frames in clip: " + m_Target.GetNumFrames(SelectedAnimation));
                SelectedFrame = EditorGUILayout.IntSlider(SelectedFrame, 0, Mathf.Max(0, m_Target.GetNumFrames(SelectedAnimation) - 1));

                EditorGUI.indentLevel++;

                if (SelectedFrame < framedata.arraySize)
                {
                    var events = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("events");
                    EditorGUILayout.PropertyField(events, true);

                    // var collider = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("collider");
                }
                else
                {
                    for (int i = framedata.arraySize; i <= SelectedFrame; i++)
                    {
                        framedata.InsertArrayElementAtIndex(i);
                        ApplySerializedProperties();
                    }
                }
            }
        }

        private void GUIAnimationSettingsLayout(SerializedProperty HitboxData, SerializedProperty clip)
        {
            // showAnimation = EditorGUILayout.Foldout(showAnimation, "Animation Settings", true);
            // if (showAnimation)
            GUILayout.Label("Animation Settings", EditorStyles.boldLabel);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(clip);
                EditorGUI.indentLevel--;
            }
        }

        private void GUISelectAnimationToEditPopup()
        {
            var oldSelectedAnim = SelectedAnimation;
            SelectedAnimation = EditorGUILayout.Popup("Select animation to edit:",
                SelectedAnimation, m_AnimationOptionLabels.ToArray(), EditorStyles.popup);

            if (oldSelectedAnim != SelectedAnimation)
                SelectedFrame = 0;
        }

        // private void GUIRemoveCurrentAnimationButton(HitboxManager m_Target, SerializedProperty Animations)
        // {
        //     SetEditorColors(Color.red, Color.white);
        //     if (GUILayout.Button("Remove Current Animation")
        //         && EditorUtility.DisplayDialog("Remove " + m_AnimationOptionLabels[SelectedAnimation] + " from " + m_Target.transform.root.name + "?",
        //             "Are you sure you want to remove this animation from " + m_Target.transform.root.name + "? All collider data, movement data, and other custom data for this animation on this character will be deleted.", "Yes", "No"))
        //     {
        //         Animations.MoveArrayElement(SelectedAnimation, Mathf.Max(0, Animations.arraySize - 1));
        //         Animations.DeleteArrayElementAtIndex(Mathf.Max(0, Animations.arraySize - 1));
        //         SelectedAnimation = Mathf.Max(0, SelectedAnimation - 1);
        //     }
        // }

        private void GUIAddFromAttachedAnimatorButton(SerializedProperty Animations)
        {
            SetEditorColors(Color.yellow, Color.white);

            if (GUILayout.Button("Add All Animations From Attached Animator"))
            {
                var animator = m_targetManager.GetComponent<Animator>();
                var allclips = animator.runtimeAnimatorController.animationClips;
                var allclipslist = new List<AnimationClip>(allclips);

                for (int i = 0; i < Animations.arraySize; i++)
                {
                    var currentclip = (AnimationClip)Animations.GetArrayElementAtIndex(i).FindPropertyRelative("clip").objectReferenceValue;

                    allclipslist.Remove(currentclip);
                }

                for (int i = 0; i < allclipslist.Count; i++)
                {
                    Animations.InsertArrayElementAtIndex(Mathf.Max(0, Animations.arraySize - 1));

                    m_targetObject.ApplyModifiedProperties();

                    SerializedProperty HitboxData = Animations.GetArrayElementAtIndex(Animations.arraySize - 1);
                    var clip = HitboxData.FindPropertyRelative("clip");
                    var framedata = HitboxData.FindPropertyRelative("framedata");

                    clip.objectReferenceValue = allclipslist[i];
                    for (int k = 0, j = m_targetManager.GetNumFrames(Animations.arraySize - 1); k < j; k++)
                    {
                        framedata.InsertArrayElementAtIndex(0);
                        framedata.GetArrayElementAtIndex(0).FindPropertyRelative("collider").ClearArray();
                        framedata.GetArrayElementAtIndex(0).FindPropertyRelative("events").ClearArray();
                    }
                    m_targetObject.ApplyModifiedProperties();
                }
            }
        }

        private void GUIAddNewAnimationButton(HitboxManager m_Target, SerializedProperty Animations)
        {
            if (GUILayout.Button("Add New Animation"))
            {
                string path = EditorUtility.OpenFilePanel("Select Animation Clip", "", "anim");

                if (!string.IsNullOrEmpty(path))
                {
                    int indexOf = path.IndexOf("Assets/");
                    path = path.Substring(indexOf >= 0 ? indexOf : 0);
                    var loadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

                    if (loadedClip == null)
                    {
                        Debug.LogError("HITBOXMANAGER ERROR: Unable to load animation clip at path \"" + path + "\"\n\r Is this this animation located inside your project's assets folder?");
                    }
                    else
                    {
                        Animations.InsertArrayElementAtIndex(Mathf.Max(0, Animations.arraySize - 1));

                        m_targetObject.ApplyModifiedProperties();

                        SerializedProperty HitboxData = Animations.GetArrayElementAtIndex(Animations.arraySize - 1);
                        var clip = HitboxData.FindPropertyRelative("clip");
                        var framedata = HitboxData.FindPropertyRelative("framedata");

                        clip.objectReferenceValue = loadedClip;
                        for (int i = 0, j = m_Target.GetNumFrames(Animations.arraySize - 1); i < j; i++)
                        {
                            framedata.InsertArrayElementAtIndex(0);
                            framedata.GetArrayElementAtIndex(0).FindPropertyRelative("collider").ClearArray();
                            framedata.GetArrayElementAtIndex(0).FindPropertyRelative("events").ClearArray();
                        }

                        SelectedAnimation = Animations.arraySize - 1;
                        SelectedFrame = 0;
                        m_targetObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        private void RefreshAnimationOptionLabels()
        {
            EditorGUILayout.Separator();
            m_AnimationOptionLabels.Clear();
            if (m_targetManager.m_Animations != null && m_targetManager.m_Animations.Length > 0)
            {
                for (int i = 0; i < m_targetManager.m_Animations.Length; i++)
                {
                    if (m_targetManager.m_Animations[i].clip == null)
                    {
                        m_AnimationOptionLabels.Add("undefined_" + i);
                        continue;
                    }
                    m_AnimationOptionLabels.Add(m_targetManager.m_Animations[i].clip.name);
                }
            }
            else
            {
                m_AnimationOptionLabels.Add("none");
            }
        }

        void UpdatePreview()
        {
            if (m_LastAnimationID != SelectedAnimation || m_LastFrameID != SelectedFrame)
                m_targetManager.UpdatePreview();

            m_LastAnimationID = SelectedAnimation;
            m_LastFrameID = SelectedFrame;

        }

        public bool ApplySerializedProperties()
        {
            return m_targetObject.ApplyModifiedProperties();
        }
    }
}
