using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    public class PosMoveSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaDelta>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaDelta> charaMoves = m_query.ToComponentDataArray<CharaDelta>(Allocator.TempJob);
            var job = new PositionJob()
            {
                m_charaMoves = charaMoves,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaMoves);

            charaMoves.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct PositionJob : IJob
        {
            public NativeArray<CharaDelta> m_charaMoves;
            public void Execute()
            {
                for (int i = 0; i < m_charaMoves.Length; i++)
                {
                    CharaDelta charaMove = m_charaMoves[i];
                    charaMove.m_position += charaMove.m_delta;
                    m_charaMoves[i] = charaMove;
                }
            }
        }

    }
}
