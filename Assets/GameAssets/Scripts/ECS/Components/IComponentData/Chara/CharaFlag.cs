using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    public struct CharaFlag : IComponentData
    {
        public boolean mukiFlag;
        public FlagInputCheck inputCheckFlag;
        public FlagMove moveFlag;
        public FlagMotion motionFlag;
        // public FlagUpdate updateFlag;

        // public bool HasFlag(EnumUpdateFlag flag)
        // {

        //     //motionFlags.HasFlagはバーストできないので != 0で代用
        //     return motionFlags.HasFlagB(flag);
        //     //(updateFlags & flag) != 0;
        // }

        // public void AddFlag(EnumUpdateFlag flag)
        // {
        //     updateFlags |= flag;
        // }

        // public void SubFlag(EnumUpdateFlag flag)
        // {
        //     updateFlags &= ~flag;
        // }

        // /// <summary>
        // /// モーションセット
        // /// </summary>
        // /// <param name="_motionType"></param>
        // public void SetMotion(EnumMotion _motionType)
        // {
        //     var isLastDash = HasFlag(EnumMotionFlag.Dash);

        //     //モーションごとのフラグ
        //     switch (_motionType)
        //     {
        //         case EnumMotion.Idle:
        //         case EnumMotion.Walk:
        //         case EnumMotion.Land:
        //             motionFlags = EnumMotionFlag.None;
        //             break;
        //         case EnumMotion.Dash:
        //         case EnumMotion.Slip:
        //             motionFlags = EnumMotionFlag.Dash;
        //             break;
        //         case EnumMotion.Jump:
        //             motionFlags = EnumMotionFlag.Air;
        //             if (isLastDash)
        //                 motionFlags |= EnumMotionFlag.Dash;
        //             break;
        //         case EnumMotion.Fall:
        //             motionFlags = EnumMotionFlag.Air;
        //             break;
        //         case EnumMotion.Damage:
        //         case EnumMotion.Down:
        //             motionFlags = EnumMotionFlag.Damage;
        //             break;
        //         case EnumMotion.Fly:
        //             motionFlags = EnumMotionFlag.Damage | EnumMotionFlag.Air;
        //             break;
        //         case EnumMotion.Dead:
        //             motionFlags = EnumMotionFlag.None;
        //             break;
        //         case EnumMotion.Action:
        //             motionFlags = EnumMotionFlag.None;
        //             if (isLastDash)
        //                 motionFlags |= EnumMotionFlag.Dash;
        //             break;
        //         default:
        //             Debug.Assert(false);
        //             break;
        //     }

        // }

        // public bool HasFlag(EnumMotionFlag flag)
        // {
        //     //motionFlags.HasFlagはバーストできない
        //     return (motionFlags & flag) != 0;
        // }

        // public void AddFlag(EnumMotionFlag flag)
        // {
        //     motionFlags |= flag;
        // }

        // public void SubFlag(EnumMotionFlag flag)
        // {
        //     motionFlags &= ~flag;
        // }
    }
}
