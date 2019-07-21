using System;
using UnityEngine;

namespace NKPB
{
    [CreateAssetMenu(menuName = "Settings/MoveSettings", fileName = "MoveSettings")]
    public sealed class MoveSettings : ScriptableObject
    {
        public int WalkSpeed;
        public int DashSpeed;
        public int BrakeDelta;
    }
}
