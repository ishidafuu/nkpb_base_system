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

    public static readonly string MapTipsPath = "Assets/GameAssets/ScriptableObjects/MapData/MapTips_";
    public static readonly string MapPathPath = "Assets/GameAssets/ScriptableObjects/MapData/MapPath_";
    public static readonly string BasePath = "Assets/NKPBEditor/MapEditor";
    public static readonly string MapTipPaletteImagePath = BasePath + "/Image/MapTipPalette";
    public static readonly string MapTipFrontImagePath = BasePath + "/Image/MapTipFront";

    public static readonly string MapTileName = "MapTile";
    public static readonly string MapImageName = "Sprites/Map/map";

}
