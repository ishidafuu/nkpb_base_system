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
    public class ConvertDrawTranslationSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<CharaPos>(),
                ComponentType.ReadWrite<Translation>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaPos> charaPoses = m_query.ToComponentDataArray<CharaPos>(Allocator.TempJob);
            NativeArray<Translation> positions = m_query.ToComponentDataArray<Translation>(Allocator.TempJob);

            var job = new ConvertJob()
            {
                m_charaPoses = charaPoses,
                m_positions = positions,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_positions);

            charaPoses.Dispose();
            positions.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct ConvertJob : IJob
        {
            public NativeArray<Translation> m_positions;
            [ReadOnly] public NativeArray<CharaPos> m_charaPoses;
            public void Execute()
            {
                const int SHIFT_PIXEL = 8;
                for (int i = 0; i < m_positions.Length; i++)
                {
                    var position = m_positions[i];
                    position.Value.x = m_charaPoses[i].m_position.x >> SHIFT_PIXEL;
                    position.Value.y = (m_charaPoses[i].m_position.y + m_charaPoses[i].m_position.z) >> SHIFT_PIXEL;
                    position.Value.z = 100 + (m_charaPoses[i].m_position.z >> SHIFT_PIXEL) * 0.01f;
                    m_positions[i] = position;
                }
            }
        }

    }
}
