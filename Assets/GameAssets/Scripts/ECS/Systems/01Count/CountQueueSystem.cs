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
    [UpdateAfter(typeof(MotionCountSystem))]
    public class CountQueueSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<CharaMotion>(),
                ComponentType.ReadWrite<CharaQueue>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaMotion> charaMotions = m_query.ToComponentDataArray<CharaMotion>(Allocator.TempJob);
            NativeArray<CharaQueue> charaQueues = m_query.ToComponentDataArray<CharaQueue>(Allocator.TempJob);
            var job = new ShiftCountMotionJob()
            {
                m_charaMotions = charaMotions,
                m_charaQueues = charaQueues,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaQueues);
            charaMotions.Dispose();
            charaQueues.Dispose();
            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct ShiftCountMotionJob : IJob
        {
            public NativeArray<CharaQueue> m_charaQueues;
            [ReadOnly] public NativeArray<CharaMotion> m_charaMotions;

            public void Execute()
            {
                for (int i = 0; i < m_charaMotions.Length; i++)
                {
                    var charaMotion = m_charaMotions[i];
                    var charaQueue = m_charaQueues[i];
                    // Debug.Log(charaMotion.motionType);
                    switch (charaMotion.m_motionType)
                    {
                        case EnumMotionType.Idle:
                            break;
                        case EnumMotionType.Walk:
                            break;
                        case EnumMotionType.Dash:
                            break;
                        case EnumMotionType.Slip:
                            UpdateSlip(ref charaMotion, ref charaQueue);
                            break;
                        case EnumMotionType.Jump:
                            break;
                        case EnumMotionType.Fall:
                            break;
                        case EnumMotionType.Land:
                            UpdateLand(ref charaMotion);
                            break;
                        case EnumMotionType.Damage:
                            UpdateDamage(ref charaMotion);
                            break;
                        case EnumMotionType.Fly:
                            break;
                        case EnumMotionType.Down:
                            UpdateDown(ref charaMotion);
                            break;
                        case EnumMotionType.Dead:
                            UpdateDead(ref charaMotion);
                            break;
                        case EnumMotionType.Action:
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }
                    m_charaQueues[i] = charaQueue;
                }

            }

            void UpdateSlip(ref CharaMotion motion, ref CharaQueue charaQueue)
            {
                charaQueue.SetQueue(EnumMotionType.Idle);
            }

            void UpdateLand(ref CharaMotion motion)
            {

            }

            void UpdateDamage(ref CharaMotion motion)
            {

            }

            void UpdateDown(ref CharaMotion motion)
            {

            }

            void UpdateDead(ref CharaMotion motion)
            {

            }
        }

    }
}
