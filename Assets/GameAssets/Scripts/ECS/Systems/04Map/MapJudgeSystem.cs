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
                m_charaPoses = charaMaps,
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
            m_query.CopyFromComponentDataArray(job.m_charaPoses);

            charaFlags.Dispose();
            charaQueues.Dispose();
            charaMaps.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct PositionJob : IJob
        {
            public NativeArray<CharaPos> m_charaPoses;
            public NativeArray<CharaQueue> m_charaQueues;
            [ReadOnly] public NativeArray<CharaFlag> m_charaFlags;
            public int MapSizeX;
            public int MapSizeY;
            public int MapSizeZ;
            public NativeArray<EnumShapeType> Shapes;
            public NativeArray<int> Events;

            const int PIX_MAP = 3;
            const int SIDE_OFFSET = 7;
            const int TIP_SIZE = 8;

            public void Execute()
            {
                for (int i = 0; i < m_charaPoses.Length; i++)
                {
                    var charaPos = m_charaPoses[i];
                    var charaQueue = m_charaQueues[i];
                    var charaFlag = m_charaFlags[i];

                    // 中心
                    // Yチェック（坂）
                    CheckYCenter(ref charaPos, ref charaFlag, ref charaQueue);
                    // Zチェック

                    // サイド
                    // Xチェック
                    CheckXRight(ref charaPos, ref charaFlag, ref charaQueue);
                    // Yチェック
                    CheckYRight(ref charaPos, ref charaFlag, ref charaQueue);
                    // Zチェック

                    // ３点
                    // 浮きチェック

                    m_charaQueues[i] = charaQueue;
                    m_charaPoses[i] = charaPos;
                }
            }

            private void CheckYCenter(ref CharaPos charaPos, ref CharaFlag charaFlag, ref CharaQueue charaQueue)
            {
                EnumShapeType shape = GetShape(charaPos.m_centerMapX, charaPos.m_mapY, charaPos.m_mapZ);

                // Yめり込みチェック
                bool isHit = false;
                int newY = 0;
                switch (shape)
                {
                    case EnumShapeType.Box:
                        isHit = true;
                        newY = (charaPos.m_mapY + 1) << PIX_MAP;
                        break;
                    case EnumShapeType.LUpSlope:

                        break;
                    case EnumShapeType.RUpSlope:

                        break;
                    case EnumShapeType.LUpSlope2H:

                        break;
                    case EnumShapeType.LUpSlope2L:

                        break;
                    case EnumShapeType.RUpSlope2L:

                        break;
                    case EnumShapeType.RUpSlope2H:

                        break;
                    case EnumShapeType.SlashWall:

                        break;
                    case EnumShapeType.BSlashWall:

                        break;
                }

                if (isHit)
                {
                    charaPos.SetPixY(newY);

                    LandQueue(ref charaFlag, ref charaQueue);
                }

            }

            private void CheckYRight(ref CharaPos charaPos, ref CharaFlag charaFlag, ref CharaQueue charaQueue)
            {
                // Yめり込みチェック
                // 上が壁以外、横が空の時のみ、接地チェック
                EnumShapeType topShape = GetShape(charaPos.m_rightMapX, charaPos.m_mapY + 1, charaPos.m_mapZ);

                if (topShape == EnumShapeType.Box)
                    return;

                EnumShapeType sideShape = GetShape(charaPos.m_rightMapX - 1, charaPos.m_mapY, charaPos.m_mapZ);

                if (sideShape != EnumShapeType.Empty)
                    return;

                EnumShapeType shape = GetShape(charaPos.m_rightMapX, charaPos.m_mapY, charaPos.m_mapZ);

                bool isHit = false;
                // 坂は無視（centerで対応）
                switch (shape)
                {
                    case EnumShapeType.Box:
                        isHit = true;
                        break;
                    case EnumShapeType.BSlashWall:
                        isHit = (charaPos.m_tipRightX >= (TIP_SIZE - 1 - charaPos.m_tipZ));
                        break;
                }

                if (isHit)
                {
                    int newY = (charaPos.m_mapY + 1) << PIX_MAP;
                    charaPos.SetPixY(newY);

                    LandQueue(ref charaFlag, ref charaQueue);
                }
            }

            private void CheckXRight(ref CharaPos charaPos, ref CharaFlag charaFlag, ref CharaQueue charaQueue)
            {
                // X壁チェック
                // 上が壁、横が空の時のみ、壁チェック
                EnumShapeType topShape = GetShape(charaPos.m_rightMapX, charaPos.m_mapY + 1, charaPos.m_mapZ);

                if (topShape != EnumShapeType.Box)
                    return;

                EnumShapeType sideShape = GetShape(charaPos.m_rightMapX - 1, charaPos.m_mapY, charaPos.m_mapZ);

                if (sideShape != EnumShapeType.Empty)
                    return;

                EnumShapeType shape = GetShape(charaPos.m_rightMapX, charaPos.m_mapY, charaPos.m_mapZ);

                bool isHit = false;
                int newRX = 0;
                switch (shape)
                {
                    case EnumShapeType.Box:
                        isHit = true;
                        newRX = (charaPos.m_rightMapX << PIX_MAP) - 1;
                        break;
                    case EnumShapeType.BSlashWall:
                        int wallZ = (TIP_SIZE - 1 - charaPos.m_tipZ);
                        isHit = (charaPos.m_tipRightX >= wallZ);
                        newRX = (charaPos.m_rightMapX << PIX_MAP) + wallZ;
                        break;
                }

                // TODO: 壁に対するくっつきの仕様次第で、すべりの対応を決める

                if (isHit)
                {
                    charaPos.SetPixX(newRX - SIDE_OFFSET);

                    CrashQueue(ref charaFlag, ref charaQueue);
                }
            }

            private static void LandQueue(ref CharaFlag charaFlag, ref CharaQueue charaQueue)
            {
                if (charaFlag.m_mapFlag.IsFlag(FlagMapCheck.Land))
                {
                    charaQueue.SetQueue(EnumMotionType.Land);
                }
            }

            private static void CrashQueue(ref CharaFlag charaFlag, ref CharaQueue charaQueue)
            {
                if (charaFlag.m_mapFlag.IsFlag(FlagMapCheck.Crash))
                {
                    // charaQueue.SetQueue(EnumMotionType.Land);
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
                    : EnumShapeType.Empty;
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
