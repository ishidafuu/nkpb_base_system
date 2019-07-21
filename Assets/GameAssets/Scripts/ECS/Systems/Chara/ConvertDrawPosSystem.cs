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
                charaMoves = charaMoves,
                positions = positions,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.positions);

            charaMoves.Dispose();
            positions.Dispose();

            return inputDeps;
        }

        [BurstCompileAttribute]
        struct ConvertJob : IJob
        {
            public NativeArray<Translation> positions;
            [ReadOnly] public NativeArray<CharaMove> charaMoves;
            public void Execute()
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    var position = positions[i];
                    position.Value.x = charaMoves[i].position.x * 0.01f;
                    position.Value.y = (charaMoves[i].position.y + charaMoves[i].position.z) * 0.01f;
                    position.Value.z = 100f + position.Value.y * 0.01f;
                    positions[i] = position;
                }
            }
        }

    }
}
