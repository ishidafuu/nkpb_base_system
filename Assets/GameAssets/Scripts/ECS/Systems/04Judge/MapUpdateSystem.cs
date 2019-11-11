using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    public class MapUpdateSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<CharaDelta>(),
                ComponentType.ReadWrite<CharaMap>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaDelta> charaMoves = m_query.ToComponentDataArray<CharaDelta>(Allocator.TempJob);
            NativeArray<CharaMap> charaMaps = m_query.ToComponentDataArray<CharaMap>(Allocator.TempJob);
            var job = new PositionJob()
            {
                m_charaMoves = charaMoves,
                m_charaMaps = charaMaps,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaMaps);

            charaMoves.Dispose();
            charaMaps.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct PositionJob : IJob
        {
            public NativeArray<CharaDelta> m_charaMoves;
            public NativeArray<CharaMap> m_charaMaps;
            public void Execute()
            {
                for (int i = 0; i < m_charaMoves.Length; i++)
                {
                    var charaMove = m_charaMoves[i];
                    var charaMap = m_charaMaps[i];
                    charaMap.m_centerPos.Set(
                        charaMove.m_position.x >> 11,
                        charaMove.m_position.y >> 11,
                        charaMove.m_position.z >> 11);

                    charaMap.m_lastLeftPos.Set(
                        (charaMove.m_position.x - 7) >> 11,
                        (charaMove.m_position.y - 7) >> 11,
                        (charaMove.m_position.z - 7) >> 11);

                    charaMap.m_rightPos.Set(
                        (charaMove.m_position.x + 7) >> 11,
                        (charaMove.m_position.y + 7) >> 11,
                        (charaMove.m_position.z + 7) >> 11);

                    // Debug.Log($"charaMap.m_centerPos : {charaMap.m_centerPos}");

                    m_charaMaps[i] = charaMap;
                }
            }
        }

    }
}
