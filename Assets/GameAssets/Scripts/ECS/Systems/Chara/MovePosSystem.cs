using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    public class MovePosSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaMove>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaMove> charaMoves = m_query.ToComponentDataArray<CharaMove>(Allocator.TempJob);
            var job = new PositionJob()
            {
                charaMoves = charaMoves,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.charaMoves);

            charaMoves.Dispose();

            return inputDeps;
        }

        [BurstCompileAttribute]
        struct PositionJob : IJob
        {
            public NativeArray<CharaMove> charaMoves;
            public void Execute()
            {
                for (int i = 0; i < charaMoves.Length; i++)
                {
                    CharaMove charaMove = charaMoves[i];
                    charaMove.position += charaMove.delta;
                    charaMoves[i] = charaMove;
                }
            }
        }

    }
}
