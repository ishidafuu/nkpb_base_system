using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    [UpdateAfter(typeof(PosMoveSystem))]
    public class MapJudgeSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaPos>(),
                ComponentType.ReadWrite<CharaQueue>(),
                ComponentType.ReadWrite<CharaDelta>(),
                ComponentType.ReadOnly<CharaFlag>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaPos> charaMaps = m_query.ToComponentDataArray<CharaPos>(Allocator.TempJob);
            NativeArray<CharaQueue> charaQueues = m_query.ToComponentDataArray<CharaQueue>(Allocator.TempJob);
            NativeArray<CharaFlag> charaFlags = m_query.ToComponentDataArray<CharaFlag>(Allocator.TempJob);

            NativeMapTips mapTips = Shared.m_mapTipList.m_MapTipList[0];
            var job = new PositionJob()
            {
                m_charaMaps = charaMaps,
                m_charaQueues = charaQueues,
                m_charaFlags = charaFlags,
                MapSizeX = mapTips.m_mapSizeX,
                MapSizeY = mapTips.m_mapSizeY,
                MapSizeZ = mapTips.m_mapSizeZ,
                Shapes = mapTips.shapes,
                Events = mapTips.events,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaQueues);
            m_query.CopyFromComponentDataArray(job.m_charaMaps);

            charaFlags.Dispose();
            charaQueues.Dispose();
            charaMaps.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct PositionJob : IJob
        {
            public NativeArray<CharaPos> m_charaMaps;
            public NativeArray<CharaQueue> m_charaQueues;
            [ReadOnly] public NativeArray<CharaFlag> m_charaFlags;
            public int MapSizeX;
            public int MapSizeY;
            public int MapSizeZ;
            public NativeArray<EnumShapeType> Shapes;
            public NativeArray<int> Events;

            public void Execute()
            {
                for (int i = 0; i < m_charaMaps.Length; i++)
                {
                    var charaMap = m_charaMaps[i];

                    // Debug.Log($"GetShape : {GetShape(charaMap.m_centerPos)}");

                    m_charaMaps[i] = charaMap;
                }
            }
            int ConvertVector3IntToIndex(int x, int y, int z)
            {
                return x + (y * MapSizeX) + (z * MapSizeX * MapSizeY);
            }

            bool IsSafePos(int x, int y, int z)
            {
                return (x >= 0) && (y >= 0) && (z >= 0)
                    && (x < MapSizeX) && (y < MapSizeY) && (z < MapSizeZ);
            }

            EnumShapeType GetShape(int x, int y, int z)
            {
                return IsSafePos(x, y, z)
                    ? Shapes[ConvertVector3IntToIndex(x, y, z)]
                    : EnumShapeType.Box;
            }

            int GetEvent(int x, int y, int z)
            {
                return IsSafePos(x, y, z)
                    ? Events[ConvertVector3IntToIndex(x, y, z)]
                    : 0;
            }
        }


    }
}
