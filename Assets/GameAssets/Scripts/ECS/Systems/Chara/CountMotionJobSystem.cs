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
    /// モーションの時間進行システム
    /// </summary>
    public class CountMotionJobSystem : JobComponentSystem
    {
        ComponentGroup m_group;

        protected override void OnCreateManager()
        {
            m_group = GetComponentGroup(
                ComponentType.Create<CharaMotion>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new CountJob()
            {
                m_charaMotions = m_group.GetComponentDataArray<CharaMotion>()
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            return inputDeps;
        }

        // [BurstCompileAttribute]
        //Sharedを使っているのでバーストできない
        //事前にNativeArrayにコピーすれば良いとは思う
        struct CountJob : IJob
        {
            public ComponentDataArray<CharaMotion> m_charaMotions;

            public void Execute()
            {
                const int FRAMES_COUNT = 4;
                for (int i = 0; i < m_charaMotions.Length; i++)
                {
                    CharaMotion charaMotion = m_charaMotions[i];
                    //Shared.aniScriptSheet.scripts[(int)charaMotion.motionType].frames.Count;
                    charaMotion.count++;
                    charaMotion.totalCount++;
                    //４カウントで１アニメカウント
                    if ((charaMotion.count >> 2) >= FRAMES_COUNT)
                    {
                        charaMotion.count = 0;
                    }
                    m_charaMotions[i] = charaMotion;
                }
            }
        }
    }
}
