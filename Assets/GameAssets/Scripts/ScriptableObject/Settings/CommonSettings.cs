using System;
using UnityEngine;

namespace NKPB
{
    [CreateAssetMenu(menuName = "Settings/CommonSettings", fileName = "CommonSettings")]
    public sealed class CommonSettings : ScriptableObject
    {
        public int PlayerCount;
        public int CharaCount;
        public int ButtonCount;
    }
}
