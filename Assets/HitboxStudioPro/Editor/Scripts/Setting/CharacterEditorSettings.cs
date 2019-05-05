using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlackGardenStudios.HitboxStudioPro
{
    public static class CharacterEditorSettings
    {
        readonly public static string PaletteFileName = "palette.png";
        readonly public static string HitboxTypeTemplate = "Generation/HitboxType";
        readonly public static string FrameEventTemplate = "Generation/FrameEvent";
        readonly public static string HitboxTypeOutputPath = Application.dataPath + "/BlackGardenStudios/HitboxStudioPro/Scripts/Data/HitboxType.cs";
        readonly public static string FrameEventOutputPath = Application.dataPath + "/BlackGardenStudios/HitboxStudioPro/Scripts/Data/FrameEvent.cs";
    }
}
