using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace NKPB
{
#pragma warning disable 0414
    partial class LabelDrawer
    {
        static GUIStyle m_LabelStyle = new GUIStyle();
        static public void DrawLabel(string text, Vector2 position, int fontSize = 12, FontStyle style = FontStyle.Normal, bool careIfPro = false)
        {
            m_LabelStyle.fontSize = fontSize;
            m_LabelStyle.fontStyle = style;
            if (EditorGUIUtility.isProSkin || careIfPro == false)
            {
                m_LabelStyle.normal.textColor = Color.black;
                GUI.Label(new Rect(position.x, position.y, 128, 16), text, m_LabelStyle);
                m_LabelStyle.normal.textColor = Color.white;
                GUI.Label(new Rect(position.x, position.y - 1f, 128, 16), text, m_LabelStyle);
            }
            else
            {
                m_LabelStyle.normal.textColor = Color.black;
                GUI.Label(new Rect(position.x, position.y - 1f, 128, 16), text, m_LabelStyle);
            }
        }

        static public void DrawLabelColor(string text, Vector2 position, Color color, int fontSize = 12, FontStyle style = FontStyle.Normal, bool careIfPro = false)
        {
            m_LabelStyle.fontSize = fontSize;
            m_LabelStyle.fontStyle = style;
            if (EditorGUIUtility.isProSkin || careIfPro == false)
            {
                m_LabelStyle.normal.textColor = Color.black;
                GUI.Label(new Rect(position.x, position.y, 128, 16), text, m_LabelStyle);
                m_LabelStyle.normal.textColor = color;
                GUI.Label(new Rect(position.x, position.y - 1f, 128, 16), text, m_LabelStyle);
            }
            else
            {
                m_LabelStyle.normal.textColor = color;
                GUI.Label(new Rect(position.x, position.y - 1f, 128, 16), text, m_LabelStyle);
            }
        }
    }
}
