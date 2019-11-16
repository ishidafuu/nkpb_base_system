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

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaMotion>(),
                ComponentType.ReadWrite<CharaFlag>(),
                ComponentType.ReadWrite<CharaQueue>(),
                ComponentType.ReadWrite<CharaDash>(),
                ComponentType.ReadWrite<CharaDelta>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaMotion> charaMotions = m_query.ToComponentDataArray<CharaMotion>(Allocator.TempJob);
            NativeArray<CharaFlag> charaFlags = m_query.ToComponentDataArray<CharaFlag>(Allocator.TempJob);
            NativeArray<CharaQueue> charaQueues = m_query.ToComponentDataArray<CharaQueue>(Allocator.TempJob);
            NativeArray<CharaDash> charaDashes = m_query.ToComponentDataArray<CharaDash>(Allocator.TempJob);
            NativeArray<CharaDelta> charaDeltas = m_query.ToComponentDataArray<CharaDelta>(Allocator.TempJob);

            var job = new InputJob()
            {
                m_charaMotions = charaMotions,
                m_charaFlags = charaFlags,
                m_charaQueues = charaQueues,
                m_charaDashes = charaDashes,
                m_charaDeltas = charaDeltas,
                JumpSpeed = Settings.Instance.Move.JumpSpeed,

            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaMotions);
            m_query.CopyFromComponentDataArray(job.m_charaFlags);
            m_query.CopyFromComponentDataArray(job.m_charaQueues);
            m_query.CopyFromComponentDataArray(job.m_charaDashes);
            m_query.CopyFromComponentDataArray(job.m_charaDeltas);

            charaMotions.Dispose();
            charaFlags.Dispose();
            charaQueues.Dispose();
            charaDashes.Dispose();
            charaDeltas.Dispose();
            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct InputJob : IJob
        {
            public NativeArray<CharaMotion> m_charaMotions;
            public NativeArray<CharaFlag> m_charaFlags;
            public NativeArray<CharaQueue> m_charaQueues;
            public NativeArray<CharaDash> m_charaDashes;
            public NativeArray<CharaDelta> m_charaDeltas;
            public int JumpSpeed;

            public void Execute()
            {
                for (int i = 0; i < m_charaQueues.Length; i++)
                {
                    var charaQueue = m_charaQueues[i];

                    if (charaQueue.m_isQueue)
                    {
                        var charaMotion = m_charaMotions[i];
                        var charaFlag = m_charaFlags[i];
                        var charaDelta = m_charaDeltas[i];
                        SwitchMotion(ref charaMotion, charaQueue.m_motionType);

                        switch (charaQueue.m_motionType)
                        {
                            case EnumMotionType.Idle:
                                IdleFlags(ref charaFlag);
                                break;
                            case EnumMotionType.Walk:
                                WalkFlags(ref charaFlag);
                                break;
                            case EnumMotionType.Dash:
                                DashFlags(ref charaFlag);
                                DashState(i, charaQueue.m_muki);
                                break;
                            case EnumMotionType.Slip:
                                SlipFlags(ref charaFlag);
                                break;
                            case EnumMotionType.Jump:
                                JumpFlags(ref charaFlag);
                                JumpState(ref charaDelta);
                                break;
                            case EnumMotionType.Fall:
                                break;
                            case EnumMotionType.Land:
                                LandFlags(ref charaFlag);
                                LandState(ref charaDelta);
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
                        m_charaDeltas[i] = charaDelta;
                    }

                    charaQueue.m_isQueue = false;
                    m_charaQueues[i] = charaQueue;
                }
            }

            void SwitchMotion(ref CharaMotion charaMotion, EnumMotionType motionType)
            {
                charaMotion.m_motionType = motionType;
                charaMotion.m_count = 0;
                charaMotion.m_frame = 0;
            }

            void IdleFlags(ref CharaFlag charaFlag)
            {
                charaFlag.m_inputCheckFlag = FlagInputCheck.Jump | FlagInputCheck.Dash | FlagInputCheck.Walk;
                charaFlag.m_moveFlag = FlagMove.Stop;
                charaFlag.m_motionFlag = FlagMotion.None;
                charaFlag.m_mapFlag = FlagMapCheck.Fall | FlagMapCheck.Stick;
                charaFlag.m_mukiFlag = true;
            }

            void WalkFlags(ref CharaFlag charaFlag)
            {
                charaFlag.m_inputCheckFlag = FlagInputCheck.Jump | FlagInputCheck.Dash | FlagInputCheck.Idle;
                charaFlag.m_moveFlag = FlagMove.Walk;
                charaFlag.m_motionFlag = FlagMotion.Move;
                charaFlag.m_mapFlag = FlagMapCheck.Fall | FlagMapCheck.Stick;
                charaFlag.m_mukiFlag = true;
            }

            void DashFlags(ref CharaFlag charaFlag)
            {
                charaFlag.m_inputCheckFlag = FlagInputCheck.Jump | FlagInputCheck.Slip;
                charaFlag.m_moveFlag = FlagMove.Dash;
                charaFlag.m_motionFlag = FlagMotion.Dash;
                charaFlag.m_mapFlag = FlagMapCheck.Fall | FlagMapCheck.Stick;
                charaFlag.m_mukiFlag = true;
            }

            void DashState(int i, EnumMuki muki)
            {
                var charaDashes = m_charaDashes[i];
                charaDashes.m_dashMuki = muki;
                m_charaDashes[i] = charaDashes;
            }

            void SlipFlags(ref CharaFlag charaFlag)
            {
                charaFlag.m_inputCheckFlag = FlagInputCheck.None;
                charaFlag.m_moveFlag = FlagMove.Friction;
                charaFlag.m_motionFlag = FlagMotion.Slip;
                charaFlag.m_mapFlag = FlagMapCheck.Fall;
                charaFlag.m_mukiFlag = false;
            }

            void JumpFlags(ref CharaFlag charaFlag)
            {
                charaFlag.m_inputCheckFlag = FlagInputCheck.Slip;
                charaFlag.m_moveFlag = FlagMove.Air;
                charaFlag.m_motionFlag = FlagMotion.Jump;
                charaFlag.m_mapFlag = FlagMapCheck.Stick;
                charaFlag.m_mukiFlag = true;
            }

            void JumpState(ref CharaDelta charaDelta)
            {
                charaDelta.m_delta.y = JumpSpeed;
            }

            void LandFlags(ref CharaFlag charaFlag)
            {
                charaFlag.m_inputCheckFlag = FlagInputCheck.None;
                charaFlag.m_moveFlag = FlagMove.None;
                charaFlag.m_motionFlag = FlagMotion.None;
                charaFlag.m_mapFlag = FlagMapCheck.Fall;
                charaFlag.m_mukiFlag = false;
            }

            void LandState(ref CharaDelta charaDelta)
            {
                charaDelta.m_delta.x = 0;
                charaDelta.m_delta.y = 0;
                charaDelta.m_delta.z = 0;
            }

        }
    }
}
