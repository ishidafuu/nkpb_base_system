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

        protected override void OnCreate()
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

            charaPoses.Dispose();
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
                    charaLastPos.m_position = charaPos.m_position;
                    charaLastPos.m_mapXCenter = charaPos.m_mapXCenter;
                    charaLastPos.m_mapXLeft = charaPos.m_mapXLeft;
                    charaLastPos.m_mapXRight = charaPos.m_mapXRight;
                    charaLastPos.m_tipCenterX = charaPos.m_tipXCenter;
                    charaLastPos.m_tipLeftX = charaPos.m_tipXLeft;
                    charaLastPos.m_tipRightX = charaPos.m_tipXRight;
                    charaLastPos.m_mapY = charaPos.m_mapY;
                    charaLastPos.m_mapZ = charaPos.m_mapZ;
                    charaLastPos.m_tipY = charaPos.m_tipY;
                    charaLastPos.m_tipZ = charaPos.m_tipZ;
                    m_charaLastPoses[i] = charaLastPos;
                }
            }
        }

    }
}
