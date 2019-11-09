using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.InteropServices;

namespace NKPB
{
#pragma warning disable 0414
    public partial class HitboxManager
    {

#if UNITY_EDITOR
        public void BakeAnimation(int animationID)
        {
            var animation = m_Animations[animationID];

            if (animation.clip == null) return;

            var clip = animation.clip;
            var numFrames = GetNumFrames(animationID);
            var events = new List<AnimationEvent>(60);

            AnimationUtility.SetAnimationEvents(clip, new AnimationEvent[0]);

            for (int k = 0; k < numFrames; k++)
            {
                if (animation.frameData.Length <= k)
                    break;

                var framedata = animation.frameData[k];

                AnimationEvent _event = new AnimationEvent();

                _event.functionName = "UpdateHitbox";
                _event.stringParameter = new internalFrameData(animation, k).Serialize();
                _event.floatParameter = animationID;
                _event.intParameter = k;
                _event.time = framedata.time;
                events.Add(_event);

                if (animation.frameData[k].events != null)
                    for (int i = 0; i < animation.frameData[k].events.Length; i++)
                    {
                        AnimationEvent frame_event = new AnimationEvent();

                        frame_event.functionName = "EVENT_" + animation.frameData[k].events[i].id.ToString();
                        frame_event.intParameter = animation.frameData[k].events[i].intParam;
                        frame_event.floatParameter = animation.frameData[k].events[i].floatParam;
                        frame_event.stringParameter = animation.frameData[k].events[i].stringParam;
                        frame_event.time = framedata.time;
                        events.Add(frame_event);
                    }
            }

            AnimationUtility.SetAnimationEvents(clip, events.ToArray());
        }

        public void BakeAnimations()
        {
            for (int j = 0; j < m_Animations.Length; j++)
            {
                BakeAnimation(j);
            }
        }

        public int GetNumFrames(int animationID)
        {
            if (m_Animations == null || animationID >= m_Animations.Length)
                return 0;

            var animation = m_Animations[animationID];

            if (animation.clip == null)
                return 0;

            var curves = AnimationUtility.GetObjectReferenceCurveBindings(animation.clip);

            for (int i = 0; i < curves.Length; i++)
            {
                if (AnimClipUtils.CheckSpriteCurve(curves[i]))
                {
                    var keyframes = AnimationUtility.GetObjectReferenceCurve(animation.clip, curves[i]);

                    return keyframes.Length;
                }
            }

            Debug.LogWarning("No sprite keyframes have been found in the current animation.");

            return 0;
        }

        public void UpdatePreview()
        {
            if (m_Animations == null
                || m_Animations.Length == 0
                || m_CurrentAnimation >= m_Animations.Length)
                return;

            var animation = m_Animations[m_CurrentAnimation].clip;

            if (animation == null)
                return;

            var time = m_CurrentFrame * (animation.length / (GetNumFrames(m_CurrentAnimation) - 1));
            animation.SampleAnimation(gameObject, time);
        }

        void OnDrawGizmos()
        {
            if (m_Animations == null
                || m_CurrentAnimation >= m_Animations.Length
                || m_Animations[m_CurrentAnimation].frameData == null
                || m_CurrentFrame >= m_Animations[m_CurrentAnimation].frameData.Length)
                return;

            var framedata = m_Animations[m_CurrentAnimation].frameData[m_CurrentFrame];
            var collider = framedata.collider;

            for (int i = 0; i < collider.Length; i++)
            {
                var color = HitboxSettings.COLOR(collider[i].type);
                color.a = 0.75f;
                Gizmos.color = color;
                Rect rect = new Rect((collider[i].rect.x) * m_UPP,
                    (collider[i].rect.y) * m_UPP,
                    collider[i].rect.width * m_UPP,
                    collider[i].rect.height * m_UPP);

                if (m_Renderer != null && m_Renderer.flipX)
                {
                    rect.x *= -1;
                    rect.width *= -1;
                }

                Gizmos.DrawCube(new Vector3(transform.position.x + rect.x,
                        transform.position.y + rect.y, transform.position.z),
                    new Vector3(rect.width * m_Scale.x, rect.height * m_Scale.y, 1));
            }
        }
#endif
    }

}
