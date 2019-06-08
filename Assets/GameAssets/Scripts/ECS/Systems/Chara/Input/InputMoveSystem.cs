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
                ComponentType.ReadOnly<CharaFlag>(),
                ComponentType.ReadOnly<PadScan>());
        }

        ComponentDataArray<CharaMove> m_charaMoves;
        ComponentDataArray<CharaDash> m_charaDashs;
        ComponentDataArray<CharaFlag> m_charaFlags;
        ComponentDataArray<PadScan> padScans;

        protected override void OnUpdate()
        {
            m_charaMoves = group.GetComponentDataArray<CharaMove>();
            m_charaDashs = group.GetComponentDataArray<CharaDash>();
            m_charaFlags = group.GetComponentDataArray<CharaFlag>();
            padScans = group.GetComponentDataArray<PadScan>();

            for (int i = 0; i < m_charaFlags.Length; i++)
            {
                var charaFlag = m_charaFlags[i];

                if (charaFlag.moveFlag.IsFlag(FlagMove.Walk))
                {
                    Walk(i);
                }

                if (charaFlag.moveFlag.IsFlag(FlagMove.Friction))
                {
                    Friction(i);
                }

                if (charaFlag.moveFlag.IsFlag(FlagMove.Stop))
                {
                    Stop(i);
                }

                if (charaFlag.moveFlag.IsFlag(FlagMove.Dash))
                {
                    Dash(i);
                }
            }
        }

        void Friction(int i)
        {
            var charaMove = m_charaMoves[i];
            charaMove.Friction(Define.Instance.Move.BrakeDelta);
            m_charaMoves[i] = charaMove;
        }

        void Stop(int i)
        {
            var charaMove = m_charaMoves[i];
            charaMove.Stop();
            m_charaMoves[i] = charaMove;
        }

        void Walk(int i)
        {
            Debug.Log("Walk");
            var charaMove = m_charaMoves[i];
            charaMove.SetDelta(Define.Instance.Move.WalkSpeed, InputToMoveMuki(i));
            m_charaMoves[i] = charaMove;
        }

        void Dash(int i)
        {
            Debug.Log("Dash");
            var charaMove = m_charaMoves[i];
            charaMove.SetDelta(Define.Instance.Move.WalkSpeed, InputToMoveMukiDash(i));
            m_charaMoves[i] = charaMove;
        }

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

        public EnumMoveMuki InputToMoveMukiDash(int i)
        {
            var res = EnumMoveMuki.None;
            var charaDash = m_charaDashs[i];
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
