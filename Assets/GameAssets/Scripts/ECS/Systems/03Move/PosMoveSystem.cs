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

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaPos>(),
                ComponentType.ReadOnly<CharaDelta>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaPos> charaPoses = m_query.ToComponentDataArray<CharaPos>(Allocator.TempJob);
            NativeArray<CharaDelta> charaDeltas = m_query.ToComponentDataArray<CharaDelta>(Allocator.TempJob);
            var job = new PositionJob()
            {
                m_charaPoses = charaPoses,
                m_charaDeltas = charaDeltas,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaPoses);

            charaPoses.Dispose();
            charaDeltas.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct PositionJob : IJob
        {
            public NativeArray<CharaPos> m_charaPoses;
            [ReadOnly] public NativeArray<CharaDelta> m_charaDeltas;
            public void Execute()
            {
                for (int i = 0; i < m_charaPoses.Length; i++)
                {
                    CharaPos charaPoses = m_charaPoses[i];
                    CharaDelta charaDelta = m_charaDeltas[i];
                    charaPoses.SetPosition(charaPoses.m_position + charaDelta.m_delta);
                    m_charaPoses[i] = charaPoses;
                }
            }
        }

    }
}
