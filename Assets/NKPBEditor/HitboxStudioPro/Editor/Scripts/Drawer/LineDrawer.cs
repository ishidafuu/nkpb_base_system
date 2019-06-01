using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace NKPB
{
#pragma warning disable 0414
    partial class LineDrawer
    {
        static Texture2D m_LineTexture;
        public static void DrawLine(Vector2 start, Vector2 end, int width, Color color)
        {
            Vector2 d = end - start;
            float a = Mathf.Rad2Deg * Mathf.Atan(d.y / d.x);
            if (d.x < 0)
                a += 180;

            int width2 = (int)Mathf.Ceil(width / 2);

            if (m_LineTexture == null)
            {
                m_LineTexture = new Texture2D(1, 1);
                m_LineTexture.SetPixel(0, 0, Color.white);
                m_LineTexture.Apply();
            }

            var matrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(a, start);
#if UNITY_2017_4_OR_NEWER
            GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), m_LineTexture, ScaleMode.StretchToFill, true, 0f, color, 0f, 0f);
#else
            GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), m_LineTexture, ScaleMode.StretchToFill, true);
#endif
            GUI.matrix = matrix;
        }
    }
}
