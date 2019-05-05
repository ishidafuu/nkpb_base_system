using System;
using UnityEngine;

namespace NKPB
{
    /// <summary>
    /// 座標移動設定
    /// </summary>
    [CreateAssetMenu(menuName = "Settings/CommonSettings", fileName = "CommonSettings")]
    public sealed class CommonSettings : ScriptableObject
    {
        public int PlayerNum;
        public int CharaNum;
        public int ButtonNum;
    }
}
