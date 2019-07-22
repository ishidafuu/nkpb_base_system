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
    [UpdateInGroup(typeof(InputGroup))]
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
                charaQueues = charaQueues,
                charaFlags = charaFlags,
                padScans = padScans,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.charaQueues);

            charaQueues.Dispose();
            charaFlags.Dispose();
            padScans.Dispose();

            return inputDeps;
        }

        [BurstCompileAttribute]
        struct InputJob : IJob
        {
            public NativeArray<CharaQueue> charaQueues;
            [ReadOnly] public NativeArray<CharaFlag> charaFlags;
            [ReadOnly] public NativeArray<PadScan> padScans;

            public void Execute()
            {
                for (int i = 0; i < charaQueues.Length; i++)
                {
                    var charaFlag = charaFlags[i];

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
                if (padScans[i].IsJumpPush())
                {
                    SetQueue(i, EnumMotion.Jump);
                    return true;
                }

                return false;
            }

            bool CheckDash(int i)
            {
                if (padScans[i].crossLeft.isDouble || padScans[i].crossRight.isDouble)
                {
                    var charaQueue = charaQueues[i];
                    EnumMuki muki = (padScans[i].crossLeft.isDouble)
                        ? EnumMuki.Left
                        : EnumMuki.Right;
                    charaQueue.SetQueueMuki(EnumMotion.Dash, muki);
                    charaQueues[i] = charaQueue;

                    return true;
                }

                return false;
            }

            bool CheckWalk(int i)
            {
                if (padScans[i].IsAnyCrossPress())
                {
                    SetQueue(i, EnumMotion.Walk);
                    return true;
                }

                return false;
            }

            bool CheckSlip(int i)
            {
                if (padScans[i].IsAnyCrossPress())
                {
                    SetQueue(i, EnumMotion.Slip);
                    return true;
                }

                return false;
            }

            bool CheckIdle(int i)
            {
                if (!padScans[i].IsAnyCrossPress())
                {
                    SetQueue(i, EnumMotion.Idle);
                    return true;
                }

                return false;
            }

            private void SetQueue(int i, EnumMotion motion)
            {
                var charaQueue = charaQueues[i];
                charaQueue.SetQueue(motion);
                charaQueues[i] = charaQueue;
            }
        }
    }
}
