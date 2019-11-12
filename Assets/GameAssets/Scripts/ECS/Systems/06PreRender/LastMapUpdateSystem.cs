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
                ComponentType.ReadWrite<CharaPos>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaPos> charaMaps = m_query.ToComponentDataArray<CharaPos>(Allocator.TempJob);
            var job = new PositionJob()
            {
                m_charaMaps = charaMaps,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaMaps);

            charaMaps.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct PositionJob : IJob
        {
            public NativeArray<CharaPos> m_charaMaps;
            public void Execute()
            {
                for (int i = 0; i < m_charaMaps.Length; i++)
                {
                    var charaMap = m_charaMaps[i];
                    charaMap.m_lastCenterMapX = charaMap.m_tipCenterX;
                    charaMap.m_lastLeftMapX = charaMap.m_tipLeftX;
                    charaMap.m_lastRightMapX = charaMap.m_tipRightX;
                    charaMap.m_lastMapY = charaMap.m_tipY;
                    charaMap.m_lastMapZ = charaMap.m_tipZ;
                    m_charaMaps[i] = charaMap;
                }
            }
        }

    }
}
