using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKPB
{
    public partial class CharaCellEditorMain
    {
        const string MenuItemName = "NKPBEditor/CharaCell Editor";
        const int TIPSIZE = 48;
        const int KAO_MAX = 9;
        const int ANGLE_MAX = 3;
        const float DOT_PER_UNIT = 16f;
        readonly int[] ZURA_OF_KAO = new int[] { 0, 1, 2, 5, 5, 4, 3, 1, 1, 0 }; //顔ずら対応表
        const string BodyFilePath = "Sprites/nm2body";
        //Assets/GameAssets/Resources/Sprites/nm2body.png
        const string KaoFileName = "kao";
        const string ZuraFileName = "zura";
        const string ScriptableObjectFilePath = "Assets/GameAssets/Resources/CharaCell/CharaCell.asset";

    }
}
