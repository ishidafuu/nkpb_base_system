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
                ComponentType.ReadWrite<CharaMap>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaMap> charaMaps = m_query.ToComponentDataArray<CharaMap>(Allocator.TempJob);

            NativeMapTips mapTips = Shared.m_mapTipList.m_MapTipList[0];
            var job = new PositionJob()
            {
                m_charaMaps = charaMaps,
                MapSizeX = mapTips.m_mapSizeX,
                MapSizeY = mapTips.m_mapSizeY,
                MapSizeZ = mapTips.m_mapSizeZ,
                Shapes = mapTips.shapes,
                Events = mapTips.events,
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

                    Debug.Log($"GetShape : {GetShape(charaMap.m_centerPos)}");

                    m_charaMaps[i] = charaMap;
                }
            }
            int ConvertVector3IntToIndex(Vector3Int pos)
            {
                int yy = (pos.y * MapSizeX);
                int zz = (pos.z * MapSizeX * MapSizeY);
                return pos.x + yy + zz;
            }

            bool IsSafePos(Vector3Int pos)
            {
                bool res = (pos.x >= 0) && (pos.y >= 0) && (pos.z >= 0)
                    && (pos.x < MapSizeX) && (pos.y < MapSizeY) && (pos.z < MapSizeZ);
                return res;
            }

            EnumShapeType GetShape(Vector3Int pos)
            {
                EnumShapeType res = EnumShapeType.Empty;
                if (IsSafePos(pos))
                {
                    res = Shapes[ConvertVector3IntToIndex(pos)];
                }
                return res;
            }

            int GetEvent(Vector3Int pos)
            {
                int res = 0;
                if (IsSafePos(pos))
                {
                    res = Events[ConvertVector3IntToIndex(pos)];
                }
                return res;
            }
        }


    }
}
