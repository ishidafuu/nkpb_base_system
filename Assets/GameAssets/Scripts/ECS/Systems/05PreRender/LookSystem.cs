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

    public class LookSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaLook>(),
                ComponentType.ReadOnly<CharaMuki>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaLook> charaLooks = m_query.ToComponentDataArray<CharaLook>(Allocator.TempJob);
            NativeArray<CharaMuki> charaMukis = m_query.ToComponentDataArray<CharaMuki>(Allocator.TempJob);

            var job = new ConvertJob()
            {
                m_charaLooks = charaLooks,
                m_charaMukis = charaMukis,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaLooks);
            charaLooks.Dispose();
            charaMukis.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct ConvertJob : IJob
        {
            public NativeArray<CharaLook> m_charaLooks;
            [ReadOnly] public NativeArray<CharaMuki> m_charaMukis;

            public void Execute()
            {
                for (int i = 0; i < m_charaLooks.Length; i++)
                {
                    var look = m_charaLooks[i];
                    look.isLeft = (m_charaMukis[i].muki == EnumMuki.Left)
                        ? 1
                        : 0;
                    m_charaLooks[i] = look;
                }
            }
        }

    }
}
