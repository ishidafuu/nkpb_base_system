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
    public class ShiftCountMotionSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaMotion>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaMotion> charaMotions = m_query.ToComponentDataArray<CharaMotion>(Allocator.TempJob);
            var job = new ShiftCountMotionJob()
            {
                charaMotions = charaMotions
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.charaMotions);
            charaMotions.Dispose();
            return inputDeps;
        }

        [BurstCompileAttribute]
        struct ShiftCountMotionJob : IJob
        {
            public NativeArray<CharaMotion> charaMotions;

            public void Execute()
            {
                for (int i = 0; i < charaMotions.Length; i++)
                {
                    CharaMotion charaMotion = charaMotions[i];

                    switch (charaMotion.motionType)
                    {
                        case EnumMotion.Idle:
                            break;
                        case EnumMotion.Walk:
                            break;
                        case EnumMotion.Dash:
                            break;
                        case EnumMotion.Slip:
                            UpdateSlip(ref charaMotion);
                            break;
                        case EnumMotion.Jump:
                            break;
                        case EnumMotion.Fall:
                            break;
                        case EnumMotion.Land:
                            UpdateLand(ref charaMotion);
                            break;
                        case EnumMotion.Damage:
                            UpdateDamage(ref charaMotion);
                            break;
                        case EnumMotion.Fly:
                            break;
                        case EnumMotion.Down:
                            UpdateDown(ref charaMotion);
                            break;
                        case EnumMotion.Dead:
                            UpdateDead(ref charaMotion);
                            break;
                        case EnumMotion.Action:
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }

                    charaMotions[i] = charaMotion;
                }

            }

            void UpdateSlip(ref CharaMotion motion)
            {

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
