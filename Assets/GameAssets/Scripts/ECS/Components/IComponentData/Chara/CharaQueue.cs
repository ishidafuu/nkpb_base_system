using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    /// <summary>
    /// キャラの状態変化予約
    /// /// </summary>
    public struct CharaQueue : IComponentData
    {
        public boolean isQueue { get; private set; }
        public EnumMotion motionType { get; private set; }
        public EnumMuki muki { get; private set; }

        public void SetQueue(EnumMotion _motionType)
        {
            isQueue = true;
            motionType = _motionType;
        }

        public void SetQueueMuki(EnumMotion _motionType, EnumMuki _muki)
        {
            SetQueue(_motionType);
            muki = _muki;
        }

        public void ClearQueue()
        {
            isQueue = false;
        }
    }
}
