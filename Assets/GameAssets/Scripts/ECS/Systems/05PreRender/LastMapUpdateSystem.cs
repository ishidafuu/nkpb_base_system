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
                ComponentType.ReadWrite<CharaMap>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaMap> charaMaps = m_query.ToComponentDataArray<CharaMap>(Allocator.TempJob);
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
            public NativeArray<CharaMap> m_charaMaps;
            public void Execute()
            {
                for (int i = 0; i < m_charaMaps.Length; i++)
                {
                    var charaMap = m_charaMaps[i];
                    charaMap.m_lastCenterPos.Set(
                        charaMap.m_centerPos.x,
                        charaMap.m_centerPos.y,
                        charaMap.m_centerPos.z);

                    charaMap.m_lastLeftPos.Set(
                        charaMap.m_leftPos.x,
                        charaMap.m_leftPos.y,
                        charaMap.m_leftPos.z);

                    charaMap.m_lastRightPos.Set(
                        charaMap.m_rightPos.x,
                        charaMap.m_rightPos.y,
                        charaMap.m_rightPos.z);

                    m_charaMaps[i] = charaMap;
                }
            }
        }

    }
}
