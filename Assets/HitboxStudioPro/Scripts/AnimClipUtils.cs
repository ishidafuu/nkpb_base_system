using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AnimClipUtils
{
    static public bool CheckSpriteCurve(EditorCurveBinding curve)
    {
        return (curve.propertyName.Equals("m_Sprite"));
    }
}
