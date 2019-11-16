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
    public class MotionCountSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaMotion>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            NativeArray<CharaMotion> charaMotions = m_query.ToComponentDataArray<CharaMotion>(Allocator.TempJob);
            var job = new CountJob()
            {
                m_charaMotions = charaMotions
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaMotions);
            charaMotions.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct CountJob : IJob
        {
            public NativeArray<CharaMotion> m_charaMotions;

            public void Execute()
            {
                const int FRAMES_COUNT = 3;
                for (int i = 0; i < m_charaMotions.Length; i++)
                {
                    CharaMotion charaMotion = m_charaMotions[i];
                    //Shared.aniScriptSheet.scripts[(int)charaMotion.motionType].frames.Count;
                    charaMotion.m_count++;
                    //４カウントで１アニメカウント
                    if ((charaMotion.m_count >> 2) >= FRAMES_COUNT)
                    {
                        charaMotion.m_count = 0;
                        charaMotion.m_frame++;
                    }
                    m_charaMotions[i] = charaMotion;
                }
            }
        }
    }
}
