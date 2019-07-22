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
    public class QueueMotionSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaMotion>(),
                ComponentType.ReadWrite<CharaFlag>(),
                ComponentType.ReadWrite<CharaQueue>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaMotion> charaMotions = m_query.ToComponentDataArray<CharaMotion>(Allocator.TempJob);
            NativeArray<CharaFlag> charaFlags = m_query.ToComponentDataArray<CharaFlag>(Allocator.TempJob);
            NativeArray<CharaQueue> charaQueues = m_query.ToComponentDataArray<CharaQueue>(Allocator.TempJob);

            var job = new InputJob()
            {
                charaMotions = charaMotions,
                charaFlags = charaFlags,
                charaQueues = charaQueues,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.charaMotions);
            m_query.CopyFromComponentDataArray(job.charaFlags);
            m_query.CopyFromComponentDataArray(job.charaQueues);

            charaMotions.Dispose();
            charaFlags.Dispose();
            charaQueues.Dispose();
            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct InputJob : IJob
        {
            public NativeArray<CharaMotion> charaMotions;
            public NativeArray<CharaFlag> charaFlags;
            public NativeArray<CharaQueue> charaQueues;

            public void Execute()
            {
                for (int i = 0; i < charaQueues.Length; i++)
                {
                    var charaQueue = charaQueues[i];

                    if (charaQueue.isQueue)
                    {
                        var charaMotion = charaMotions[i];
                        charaMotion.SwitchMotion(charaQueue.motionType);

                        switch (charaQueue.motionType)
                        {
                            case EnumMotion.Idle:
                                UpdateIdleFlags(i);
                                break;
                            case EnumMotion.Walk:
                                break;
                            case EnumMotion.Dash:
                                break;
                            case EnumMotion.Slip:
                                break;
                            case EnumMotion.Jump:
                                break;
                            case EnumMotion.Fall:
                                break;
                            case EnumMotion.Land:
                                break;
                            case EnumMotion.Damage:
                                break;
                            case EnumMotion.Fly:
                                break;
                            case EnumMotion.Down:
                                break;
                            case EnumMotion.Dead:
                                break;
                            case EnumMotion.Action:
                                break;
                            default:
                                Debug.Assert(false);
                                break;
                        }
                    }

                    charaQueue.ClearQueue();
                    charaQueues[i] = charaQueue;
                }
            }
            // Jump = 0x0001,
            //     Dash = 0x0002,
            //     Walk = 0x0004,
            //     Slip = 0x0008,
            //     Idle = 0x0010,
            void UpdateIdleFlags(int i)
            {
                var charaFlag = charaFlags[i];
                charaFlag.inputCheckFlag = FlagInputCheck.Jump | FlagInputCheck.Dash | FlagInputCheck.Walk;
                charaFlag.moveFlag = FlagMove.Stop;
                charaFlag.motionFlag = FlagMotion.None;
                charaFlag.mukiFlag = true;
                charaFlags[i] = charaFlag;
            }

        }
    }
}
