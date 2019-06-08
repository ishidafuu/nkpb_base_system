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
    /// 入力による状態変化システム
    /// </summary>
    public class InputMotionJobSystem : JobComponentSystem
    {
        ComponentGroup m_group;

        protected override void OnCreateManager()
        {
            m_group = GetComponentGroup(
                ComponentType.Create<CharaQueue>(),
                ComponentType.ReadOnly<CharaFlag>(),
                ComponentType.ReadOnly<PadScan>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new InputJob()
            {
                m_charaQueues = m_group.GetComponentDataArray<CharaQueue>(),
                m_charaFlags = m_group.GetComponentDataArray<CharaFlag>(),
                m_PadScans = m_group.GetComponentDataArray<PadScan>(),
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            return inputDeps;
        }

        [BurstCompileAttribute]
        struct InputJob : IJob
        {
            public ComponentDataArray<CharaQueue> m_charaQueues;
            [ReadOnly]
            public ComponentDataArray<CharaFlag> m_charaFlags;
            [ReadOnly]
            public ComponentDataArray<PadScan> m_PadScans;

            public void Execute()
            {
                for (int i = 0; i < m_charaQueues.Length; i++)
                {
                    var charaFlag = m_charaFlags[i];

                    if (charaFlag.inputCheckFlag.IsFlag(FlagInputCheck.Jump))
                    {
                        if (CheckJump(i))
                            break;
                    }
                    else if (charaFlag.inputCheckFlag.IsFlag(FlagInputCheck.Dash))
                    {
                        if (CheckDash(i))
                            break;
                    }
                    else if (charaFlag.inputCheckFlag.IsFlag(FlagInputCheck.Walk))
                    {
                        if (CheckWalk(i))
                            break;
                    }
                    else if (charaFlag.inputCheckFlag.IsFlag(FlagInputCheck.Slip))
                    {
                        if (CheckSlip(i))
                            break;
                    }
                    else if (charaFlag.inputCheckFlag.IsFlag(FlagInputCheck.Idle))
                    {
                        if (CheckIdle(i))
                            break;
                    }
                }
            }

            bool CheckJump(int i)
            {
                if (m_PadScans[i].IsJumpPush())
                {
                    SetQueue(i, EnumMotion.Jump);
                    return true;
                }

                return false;
            }

            bool CheckDash(int i)
            {
                if (m_PadScans[i].crossLeft.isDouble || m_PadScans[i].crossRight.isDouble)
                {
                    var charaQueue = m_charaQueues[i];
                    EnumMuki muki = (m_PadScans[i].crossLeft.isDouble)
                        ? EnumMuki.Left
                        : EnumMuki.Right;
                    charaQueue.SetQueueMuki(EnumMotion.Dash, muki);
                    m_charaQueues[i] = charaQueue;

                    return true;
                }

                return false;
            }

            bool CheckWalk(int i)
            {
                if (m_PadScans[i].IsAnyCrossPress())
                {
                    SetQueue(i, EnumMotion.Walk);
                    return true;
                }

                return false;
            }

            bool CheckSlip(int i)
            {
                if (m_PadScans[i].IsAnyCrossPress())
                {
                    SetQueue(i, EnumMotion.Slip);
                    return true;
                }

                return false;
            }

            bool CheckIdle(int i)
            {
                if (!m_PadScans[i].IsAnyCrossPress())
                {
                    SetQueue(i, EnumMotion.Idle);
                    return true;
                }

                return false;
            }

            private void SetQueue(int i, EnumMotion motion)
            {
                var charaQueue = m_charaQueues[i];
                charaQueue.SetQueue(motion);
                m_charaQueues[i] = charaQueue;
            }
        }
    }
}
