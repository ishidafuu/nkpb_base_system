using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Map creater sub window.
/// </summary>
public partial class MapEditor : EditorWindow
{
    static readonly float WINDOW_W = 750.0f;
    static readonly float WINDOW_H = 750.0f;
    public static readonly string MapTipsPath = "Assets/Resources/MapTips_";
    public static readonly string MapPathPath = "Assets/Resources/MapPath_";
    public static readonly string MapTipPaletteImagePath = "Assets/MapEditor/Image/MapTipPalette";
    public static readonly string MapTipFrontImagePath = "Assets/MapEditor/Editor/MapTipFront";

    public static readonly string MapTileName = "MapTile";

}
