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

    [UpdateInGroup(typeof(CountGroup))]
    // [UpdateBefore(typeof(InputMotionSystem))]
    public class CountMotionSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaMotion>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            NativeArray<CharaMotion> charaMotions = m_query.ToComponentDataArray<CharaMotion>(Allocator.TempJob);
            var job = new CountJob()
            {
                charaMotions = charaMotions
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.charaMotions);
            charaMotions.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        //Sharedを使っているのでバーストできない
        //事前にNativeArrayにコピーすれば良いとは思う
        struct CountJob : IJob
        {
            public NativeArray<CharaMotion> charaMotions;

            public void Execute()
            {
                const int FRAMES_COUNT = 4;
                for (int i = 0; i < charaMotions.Length; i++)
                {
                    CharaMotion charaMotion = charaMotions[i];
                    //Shared.aniScriptSheet.scripts[(int)charaMotion.motionType].frames.Count;
                    charaMotion.count++;
                    charaMotion.totalCount++;
                    //４カウントで１アニメカウント
                    if ((charaMotion.count >> 2) >= FRAMES_COUNT)
                    {
                        charaMotion.count = 0;
                    }
                    charaMotions[i] = charaMotion;
                }
            }
        }
    }
}
