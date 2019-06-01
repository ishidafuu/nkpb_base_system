using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;

namespace NKPB
{
#pragma warning disable 0414
    [DisallowMultipleComponent]
    public partial class HitboxManager : MonoBehaviour
    {

        #region Properties
        [SerializeField]
        public HitboxAnimation[] m_Animations;
        public int UID { get; set; }

        [SerializeField]
        public int m_MaxHitboxes = 1;

        SpriteRenderer m_Renderer;
        float m_UPP = 1f / 32f;
        int m_UnitsToPixel = 32;
        Vector2 m_Scale = Vector2.one;
        int m_UIDCounter = 0;
        #endregion

        void Awake()
        {
            UID = m_UIDCounter++;

            m_Scale = transform.localScale;

            GetSpriteBPP();
        }

        void OnValidate()
        {
            GetSpriteBPP();
        }

        void GetSpriteBPP()
        {
            if (m_Renderer == null)m_Renderer = GetComponent<SpriteRenderer>();

            var sprite = m_Renderer.sprite;

            if (sprite != null)
            {
                m_UPP = 1 / m_Renderer.sprite.pixelsPerUnit;
                m_UnitsToPixel = (int)m_Renderer.sprite.pixelsPerUnit;
            }
            else
            {
                Debug.LogWarning("HITBOX MANAGER WARNING: No sprite is assigned during Awake(). Unable to retreive sprite.pixelsPerUnit. Movement and hitbox location calculations will be incorrect!");
            }
        }

        // int m_CurrentHitCount;
        AnimationClip m_LastClip;
        int m_CurrentId;
        int m_UID;
        int m_LastFrame;
        void UpdateHitbox(AnimationEvent _event)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                return;
#endif

            if (!string.IsNullOrEmpty(_event.stringParameter))
            {

                internalFrameData animData = internalFrameData.Deserialize(_event.stringParameter);
                if (m_LastClip != _event.animatorClipInfo.clip)
                {
                    m_LastClip = _event.animatorClipInfo.clip;
                    m_CurrentId = m_UID++;
                }

                m_LastFrame = _event.intParameter;

                UpdateHitbox(animData, Mathf.RoundToInt(_event.floatParameter), _event.intParameter);
            }
        }

        Vector2Int m_NextOffset;
        Vector2 m_OffsetStep;

        void FixedUpdate()
        {
            if (m_OffsetStep != Vector2.zero)
            {
                var capsule = m_OffsetStep;
                transform.root.localPosition += new Vector3(capsule.x * m_UPP, capsule.y * m_UPP);
            }
        }

        void UpdateHitbox(internalFrameData animdata, int anim, int frame)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                return;
#endif

            m_CurrentAnimation = anim;
            m_CurrentFrame = frame;
            internalHitboxData framedata = animdata.frame;
            // var nextframedata = animdata.nextframe;

            if (framedata.collider != null)
                for (int i = 0; i < framedata.collider.Length; i++)
                {
                    var collider = framedata.collider[i];
                    var rect = collider.rect;
                }

        }

        // int m_NumFrames;
        [SerializeField]
        public int m_CurrentAnimation;
        [SerializeField]
        public int m_CurrentFrame;
        [SerializeField]
        public int m_CurrentCollider;

        void OnEnable()
        {
            m_Scale = transform.localScale;
            m_Animations = null;
        }

        /// <summary>
        /// Decode the data from an animation event to an origin -> direction.
        /// </summary>
        /// <param name="intParam">Int parameter passed by event</param>
        /// <param name="floatParam">Float parameter passed by event</param>
        /// <param name="origin">The local space origin of the gizmo</param>
        /// <param name="direction"> The forward direction of the gizmo</param>
        /// <param name="normalizeDirection"> Whether or not the direction vector output will be normalized</param>
        public void DecodeOriginAndDirection(int intParam, float floatParam, out Vector2 origin, out Vector2 direction, bool normalizeDirection = true)
        {
            origin = IntConverter.DecodeIntToVector2(intParam);
            direction = IntConverter.DecodeIntToVector2((int)floatParam) - origin;
            if (normalizeDirection)
                direction = Vector3.Normalize(direction);
            origin /= m_Renderer.sprite.pixelsPerUnit;
            direction.y *= -1f;
            origin.y *= -1f;
        }

    }

}
