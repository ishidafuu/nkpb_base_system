using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace NKPB
{
    public partial class MapEditorMain : EditorWindow
    {
        public static readonly float WINDOW_W = 750.0f;
        public static readonly float WINDOW_H = 750.0f;

        public static readonly string MapTipsPath = "Assets/GameAssets/Resources/MapData/MapTips_";
        public static readonly string MapPathPath = "Assets/GameAssets/Resources/MapData/MapPath_";
        public static readonly string BasePath = "Assets/NKPBEditor/MapEditor";
        public static readonly string MapTipPaletteImagePath = BasePath + "/Image/MapTipPalette";
        public static readonly string MapTipFrontImagePath = BasePath + "/Image/MapTipFront";

        public static readonly string MapTileName = "MapTile";
        public static readonly string MapImageName = "Sprites/Map/map";
    }
}