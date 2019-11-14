using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    public class LastMapUpdateSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaLastPos>(),
                ComponentType.ReadOnly<CharaPos>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaPos> charaPoses = m_query.ToComponentDataArray<CharaPos>(Allocator.TempJob);
            NativeArray<CharaLastPos> charaLastPoses = m_query.ToComponentDataArray<CharaLastPos>(Allocator.TempJob);
            var job = new PositionJob()
            {
                m_charaLastPoses = charaLastPoses,
                m_charaPoses = charaPoses,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaPoses);

            charaLastPoses.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct PositionJob : IJob
        {
            public NativeArray<CharaLastPos> m_charaLastPoses;
            [ReadOnly] public NativeArray<CharaPos> m_charaPoses;
            public void Execute()
            {
                for (int i = 0; i < m_charaPoses.Length; i++)
                {
                    var charaLastPos = m_charaLastPoses[i];
                    var charaPos = m_charaPoses[i];
                    charaLastPos.m_lastCenterMapX = charaPos.m_tipCenterX;
                    charaLastPos.m_lastLeftMapX = charaPos.m_tipLeftX;
                    charaLastPos.m_lastRightMapX = charaPos.m_tipRightX;
                    charaLastPos.m_lastMapY = charaPos.m_tipY;
                    charaLastPos.m_lastMapZ = charaPos.m_tipZ;
                    m_charaLastPoses[i] = charaLastPos;
                }
            }
        }

    }
}
