using System;
using UnityEngine;

namespace NKPB
{
    [CreateAssetMenu(menuName = "Settings/DrawPosSettings", fileName = "DrawPosSettings")]
    public sealed class DrawPosSettings : ScriptableObject
    {
        public int ScreenWidth;
        public int ScreenHeight;
    }
}
