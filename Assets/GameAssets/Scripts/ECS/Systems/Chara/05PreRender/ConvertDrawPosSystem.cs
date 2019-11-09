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
    public class ConvertDrawPosSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<CharaMove>(),
                ComponentType.ReadWrite<Translation>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaMove> charaMoves = m_query.ToComponentDataArray<CharaMove>(Allocator.TempJob);
            NativeArray<Translation> positions = m_query.ToComponentDataArray<Translation>(Allocator.TempJob);

            var job = new ConvertJob()
            {
                m_charaMoves = charaMoves,
                m_positions = positions,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_positions);

            charaMoves.Dispose();
            positions.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct ConvertJob : IJob
        {
            public NativeArray<Translation> m_positions;
            [ReadOnly] public NativeArray<CharaMove> m_charaMoves;
            public void Execute()
            {
                for (int i = 0; i < m_positions.Length; i++)
                {
                    var position = m_positions[i];
                    position.Value.x = m_charaMoves[i].position.x * 0.01f;
                    position.Value.y = (m_charaMoves[i].position.y + m_charaMoves[i].position.z) * 0.01f;
                    position.Value.z = 100f + position.Value.y * 0.01f;
                    m_positions[i] = position;
                }
            }
        }

    }
}
