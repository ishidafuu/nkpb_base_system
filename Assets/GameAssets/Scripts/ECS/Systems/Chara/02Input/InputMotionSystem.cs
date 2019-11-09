using System;
using System.Collections.ObjectModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    // [UpdateBefore(typeof(MovePosSystem))]
    public class InputMotionSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaQueue>(),
                ComponentType.ReadOnly<CharaFlag>(),
                ComponentType.ReadOnly<PadScan>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaQueue> charaQueues = m_query.ToComponentDataArray<CharaQueue>(Allocator.TempJob);
            NativeArray<CharaFlag> charaFlags = m_query.ToComponentDataArray<CharaFlag>(Allocator.TempJob);
            NativeArray<PadScan> padScans = m_query.ToComponentDataArray<PadScan>(Allocator.TempJob);

            var job = new InputJob()
            {
                m_charaQueues = charaQueues,
                m_charaFlags = charaFlags,
                m_padScans = padScans,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaQueues);

            charaQueues.Dispose();
            charaFlags.Dispose();
            padScans.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct InputJob : IJob
        {
            public NativeArray<CharaQueue> m_charaQueues;
            [ReadOnly] public NativeArray<CharaFlag> m_charaFlags;
            [ReadOnly] public NativeArray<PadScan> m_padScans;

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

                    if (charaFlag.inputCheckFlag.IsFlag(FlagInputCheck.Dash))
                    {
                        if (CheckDash(i))
                            break;
                    }

                    if (charaFlag.inputCheckFlag.IsFlag(FlagInputCheck.Walk))
                    {
                        if (CheckWalk(i))
                            break;
                    }

                    if (charaFlag.inputCheckFlag.IsFlag(FlagInputCheck.Slip))
                    {
                        if (CheckSlip(i))
                            break;
                    }

                    if (charaFlag.inputCheckFlag.IsFlag(FlagInputCheck.Idle))
                    {
                        if (CheckIdle(i))
                            break;
                    }

                }
            }

            bool CheckJump(int i)
            {
                if (m_padScans[i].IsJumpPush())
                {
                    Debug.Log("asdfasdf");
                    SetQueue(i, EnumMotion.Jump);
                    return true;
                }

                return false;
            }

            bool CheckDash(int i)
            {
                if (m_padScans[i].m_crossLeft.m_isDouble || m_padScans[i].m_crossRight.m_isDouble)
                {
                    var charaQueue = m_charaQueues[i];
                    EnumMuki muki = (m_padScans[i].m_crossLeft.m_isDouble)
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

                if (m_padScans[i].IsAnyCrossPress())
                {
                    SetQueue(i, EnumMotion.Walk);
                    return true;
                }

                return false;
            }

            bool CheckSlip(int i)
            {
                if (m_padScans[i].IsAnyCrossPress())
                {
                    SetQueue(i, EnumMotion.Slip);
                    return true;
                }

                return false;
            }

            bool CheckIdle(int i)
            {
                if (!m_padScans[i].IsAnyCrossPress())
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
