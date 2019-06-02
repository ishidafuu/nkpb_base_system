using System;
using System.Collections.ObjectModel;
using HedgehogTeam.EasyTouch;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    /// <summary>
    /// 入力による向き変化システム
    /// </summary>
    public class InputMoveSystem : ComponentSystem
    {
        ComponentGroup group;

        protected override void OnCreateManager()
        {
            group = GetComponentGroup(
                ComponentType.Create<CharaMove>(),
                ComponentType.ReadOnly<CharaDash>(),
                ComponentType.ReadOnly<CharaMotion>(),
                ComponentType.ReadOnly<PadScan>());
        }
        ComponentDataArray<CharaMove> charaMoves;
        ComponentDataArray<CharaDash> charaDashs;
        ComponentDataArray<CharaMotion> charaMotions;
        ComponentDataArray<PadScan> padScans;

        protected override void OnUpdate()
        {
            charaMoves = group.GetComponentDataArray<CharaMove>();
            charaDashs = group.GetComponentDataArray<CharaDash>();
            charaMotions = group.GetComponentDataArray<CharaMotion>();
            padScans = group.GetComponentDataArray<PadScan>();

            for (int i = 0; i < charaMotions.Length; i++)
            {
                //モーションごとの入力
                switch (charaMotions[i].motionType)
                {
                    case EnumMotion.Idle:
                        Friction(i);
                        break;
                    case EnumMotion.Walk:
                        Walk(i);
                        break;
                    case EnumMotion.Dash:
                        break;
                    case EnumMotion.Slip:
                        Friction(i);
                        break;
                    case EnumMotion.Jump:
                        break;
                    case EnumMotion.Fall:
                        break;
                    case EnumMotion.Land:
                        Stop(i);
                        break;
                    case EnumMotion.Damage:
                        break;
                    case EnumMotion.Fly:
                        break;
                    case EnumMotion.Down:
                        Friction(i);
                        break;
                    case EnumMotion.Dead:
                        Stop(i);
                        break;
                    case EnumMotion.Action:
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
        }

        /// <summary>
        /// 摩擦
        /// </summary>
        /// <param name="i"></param>
        void Friction(int i)
        {
            var charaMove = charaMoves[i];
            charaMove.Friction(Define.Instance.Move.BrakeDelta);
            charaMoves[i] = charaMove;
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="i"></param>
        void Stop(int i)
        {
            var charaMove = charaMoves[i];
            charaMove.Stop();
            charaMoves[i] = charaMove;
        }

        void Walk(int i)
        {
            Debug.Log("Walk");
            var charaMove = charaMoves[i];
            charaMove.SetDelta(Define.Instance.Move.WalkSpeed, InputToMoveMuki(i));
            charaMoves[i] = charaMove;
        }

        /// <summary>
        /// 入力向き
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public EnumMoveMuki InputToMoveMuki(int i)
        {
            var res = EnumMoveMuki.None;

            if (padScans[i].crossLeft.isPress)
            {
                if (padScans[i].crossUp.isPress)
                {
                    res = EnumMoveMuki.LeftUp;
                }
                else if (padScans[i].crossDown.isPress)
                {
                    res = EnumMoveMuki.LeftDown;
                }
                else
                {
                    res = EnumMoveMuki.Left;
                }
            }
            else if (padScans[i].crossRight.isPress)
            {
                if (padScans[i].crossUp.isPress)
                {
                    res = EnumMoveMuki.RightUp;
                }
                else if (padScans[i].crossDown.isPress)
                {
                    res = EnumMoveMuki.RightDown;
                }
                else
                {
                    res = EnumMoveMuki.Right;
                }
            }
            else
            {
                if (padScans[i].crossUp.isPress)
                {
                    res = EnumMoveMuki.Up;
                }
                else if (padScans[i].crossDown.isPress)
                {
                    res = EnumMoveMuki.Down;
                }
            }

            return res;
        }

        /// <summary>
        /// ダッシュの入力向き
        /// </summary>
        /// <param name="i"></param>
        /// <param name="charaDash"></param>
        /// <returns></returns>
        public EnumMoveMuki InputToMoveMukiDash(int i, CharaDash charaDash)
        {
            var res = EnumMoveMuki.None;

            if (charaDash.dashMuki == EnumMuki.Left)
            {
                if (padScans[i].crossLeft.isPress)
                {
                    if (padScans[i].crossUp.isPress)
                    {
                        res = EnumMoveMuki.LeftLeftUp;
                    }
                    else if (padScans[i].crossDown.isPress)
                    {
                        res = EnumMoveMuki.LeftLeftDown;
                    }
                    else
                    {
                        res = EnumMoveMuki.Left;
                    }
                }
                else
                {
                    if (padScans[i].crossUp.isPress)
                    {
                        res = EnumMoveMuki.LeftUp;
                    }
                    else if (padScans[i].crossDown.isPress)
                    {
                        res = EnumMoveMuki.LeftDown;
                    }
                    else
                    {
                        res = EnumMoveMuki.Left;
                    }
                }
            }
            else if (charaDash.dashMuki == EnumMuki.Right)
            {
                if (padScans[i].crossRight.isPress)
                {
                    if (padScans[i].crossUp.isPress)
                    {
                        res = EnumMoveMuki.RightRightUp;
                    }
                    else if (padScans[i].crossDown.isPress)
                    {
                        res = EnumMoveMuki.RightRightDown;
                    }
                    else
                    {
                        res = EnumMoveMuki.Right;
                    }
                }
                else
                {
                    if (padScans[i].crossUp.isPress)
                    {
                        res = EnumMoveMuki.RightUp;
                    }
                    else if (padScans[i].crossDown.isPress)
                    {
                        res = EnumMoveMuki.RightDown;
                    }
                    else
                    {
                        res = EnumMoveMuki.Right;
                    }
                }
            }
            return res;
        }
    }
}
