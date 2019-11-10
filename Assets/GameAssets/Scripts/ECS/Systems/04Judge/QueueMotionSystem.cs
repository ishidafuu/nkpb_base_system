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
    public class QueueMotionSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaMotion>(),
                ComponentType.ReadWrite<CharaFlag>(),
                ComponentType.ReadWrite<CharaQueue>(),
                ComponentType.ReadWrite<CharaDash>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaMotion> charaMotions = m_query.ToComponentDataArray<CharaMotion>(Allocator.TempJob);
            NativeArray<CharaFlag> charaFlags = m_query.ToComponentDataArray<CharaFlag>(Allocator.TempJob);
            NativeArray<CharaQueue> charaQueues = m_query.ToComponentDataArray<CharaQueue>(Allocator.TempJob);
            NativeArray<CharaDash> charaDashes = m_query.ToComponentDataArray<CharaDash>(Allocator.TempJob);

            var job = new InputJob()
            {
                m_charaMotions = charaMotions,
                m_charaFlags = charaFlags,
                m_charaQueues = charaQueues,
                m_charaDashes = charaDashes,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaMotions);
            m_query.CopyFromComponentDataArray(job.m_charaFlags);
            m_query.CopyFromComponentDataArray(job.m_charaQueues);
            m_query.CopyFromComponentDataArray(job.m_charaDashes);

            charaMotions.Dispose();
            charaFlags.Dispose();
            charaQueues.Dispose();
            charaDashes.Dispose();
            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct InputJob : IJob
        {
            public NativeArray<CharaMotion> m_charaMotions;
            public NativeArray<CharaFlag> m_charaFlags;
            public NativeArray<CharaQueue> m_charaQueues;
            public NativeArray<CharaDash> m_charaDashes;

            public void Execute()
            {
                for (int i = 0; i < m_charaQueues.Length; i++)
                {
                    var charaQueue = m_charaQueues[i];

                    if (charaQueue.m_isQueue)
                    {
                        var charaMotion = m_charaMotions[i];
                        var charaFlag = m_charaFlags[i];
                        SwitchMotion(ref charaMotion, charaQueue.m_motionType);

                        switch (charaQueue.m_motionType)
                        {
                            case EnumMotionType.Idle:
                                UpdateIdleFlags(ref charaFlag);
                                break;
                            case EnumMotionType.Walk:
                                UpdateWalkFlags(ref charaFlag);
                                break;
                            case EnumMotionType.Dash:
                                UpdateDashFlags(ref charaFlag);
                                UpdateDashState(i, charaQueue.m_muki);
                                break;
                            case EnumMotionType.Slip:
                                break;
                            case EnumMotionType.Jump:
                                break;
                            case EnumMotionType.Fall:
                                break;
                            case EnumMotionType.Land:
                                break;
                            case EnumMotionType.Damage:
                                break;
                            case EnumMotionType.Fly:
                                break;
                            case EnumMotionType.Down:
                                break;
                            case EnumMotionType.Dead:
                                break;
                            case EnumMotionType.Action:
                                break;
                            default:
                                Debug.Assert(false);
                                break;
                        }


                        m_charaMotions[i] = charaMotion;
                        m_charaFlags[i] = charaFlag;
                    }

                    charaQueue.m_isQueue = false;
                    m_charaQueues[i] = charaQueue;
                }
            }

            private void UpdateDashState(int i, EnumMuki muki)
            {
                var charaDashes = m_charaDashes[i];
                charaDashes.dashMuki = muki;
                m_charaDashes[i] = charaDashes;
            }

            void SwitchMotion(ref CharaMotion charaMotion, EnumMotionType motionType)
            {
                charaMotion.m_motionType = motionType;
                charaMotion.m_count = 0;
                charaMotion.m_frame = 0;
            }

            void UpdateIdleFlags(ref CharaFlag charaFlag)
            {
                charaFlag.inputCheckFlag = FlagInputCheck.Jump | FlagInputCheck.Dash | FlagInputCheck.Walk;
                charaFlag.moveFlag = FlagMove.Stop;
                charaFlag.motionFlag = FlagMotion.None;
                charaFlag.mukiFlag = true;
            }

            void UpdateWalkFlags(ref CharaFlag charaFlag)
            {
                charaFlag.inputCheckFlag = FlagInputCheck.Jump | FlagInputCheck.Dash | FlagInputCheck.Idle;
                charaFlag.moveFlag = FlagMove.Walk;
                charaFlag.motionFlag = FlagMotion.Move;
                charaFlag.mukiFlag = true;
            }

            void UpdateDashFlags(ref CharaFlag charaFlag)
            {
                charaFlag.inputCheckFlag = FlagInputCheck.Jump | FlagInputCheck.Slip;
                charaFlag.moveFlag = FlagMove.Dash;
                charaFlag.motionFlag = FlagMotion.Dash;
                charaFlag.mukiFlag = true;
            }

        }
    }
}
