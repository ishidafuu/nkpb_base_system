using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    public class MapJudgeSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
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
            NativeArray<CharaPos> charaPoses = m_query.ToComponentDataArray<CharaPos>(Allocator.TempJob);
            NativeArray<CharaLastPos> charaLastPoses = m_query.ToComponentDataArray<CharaLastPos>(Allocator.TempJob);
            NativeArray<CharaQueue> charaQueues = m_query.ToComponentDataArray<CharaQueue>(Allocator.TempJob);
            NativeArray<CharaFlag> charaFlags = m_query.ToComponentDataArray<CharaFlag>(Allocator.TempJob);

            NativeMapTips mapTips = Shared.m_mapTipList.m_MapTipList[0];
            var job = new MapJob()
            {
                m_charaPoses = charaPoses,
                m_charaLastPoses = charaLastPoses,
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
            charaPoses.Dispose();
            charaLastPoses.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct MapJob : IJob
        {
            public NativeArray<CharaPos> m_charaPoses;
            public NativeArray<CharaQueue> m_charaQueues;
            [ReadOnly] public NativeArray<CharaLastPos> m_charaLastPoses;
            [ReadOnly] public NativeArray<CharaFlag> m_charaFlags;
            public int MapSizeX;
            public int MapSizeY;
            public int MapSizeZ;
            public NativeArray<EnumShapeType> Shapes;
            public NativeArray<int> Events;

            const int PIX_MAP = 3;
            const int SIDE_OFFSET = 7;
            const int TIP_SIZE = 8;
            const int TIP_SIZE_HALF = 4;

            enum XPos
            {
                Left,
                Center,
                Right,
            }

            public void Execute()
            {
                for (int i = 0; i < m_charaPoses.Length; i++)
                {
                    var charaPos = m_charaPoses[i];
                    var charaLastPos = m_charaLastPoses[i];
                    var charaQueue = m_charaQueues[i];
                    var charaFlag = m_charaFlags[i];

                    // Y壁・斜め壁チェック
                    CheckYWall(XPos.Left, ref charaPos, ref charaLastPos, ref charaFlag, ref charaQueue);
                    CheckYWall(XPos.Right, ref charaPos, ref charaLastPos, ref charaFlag, ref charaQueue);
                    CheckYWall(XPos.Center, ref charaPos, ref charaLastPos, ref charaFlag, ref charaQueue);

                    // 右サイド
                    // 直前のチップ横か手前かで判定優先度を変える
                    // 斜め壁はX,Zの移動量を比較して大きい方と垂直にずれるように補正する

                    // XZに関しては、斜め壁と壁は独立して補正を行う


                    // 坂の補正は中心のみ行う
                    // Z壁チェック
                    CheckZBox(XPos.Left, ref charaPos, ref charaLastPos, ref charaFlag, ref charaQueue);
                    CheckZBox(XPos.Right, ref charaPos, ref charaLastPos, ref charaFlag, ref charaQueue);
                    CheckZBox(XPos.Center, ref charaPos, ref charaLastPos, ref charaFlag, ref charaQueue);
                    // X壁チェック
                    CheckXBox(XPos.Left, ref charaPos, ref charaLastPos, ref charaFlag, ref charaQueue);
                    CheckXBox(XPos.Right, ref charaPos, ref charaLastPos, ref charaFlag, ref charaQueue);

                    // Y坂チェック
                    CheckYSlope(XPos.Right, ref charaPos, ref charaLastPos, ref charaFlag, ref charaQueue);

                    // XZ斜め壁チェック
                    CheckXZSlashWall(XPos.Right, ref charaPos, ref charaLastPos, ref charaFlag, ref charaQueue);
                    CheckXZSlashWall(XPos.Left, ref charaPos, ref charaLastPos, ref charaFlag, ref charaQueue);

                    // ３点
                    // 浮きチェック

                    m_charaQueues[i] = charaQueue;
                    m_charaPoses[i] = charaPos;
                }
            }

            private void CheckYWall(XPos xPos, ref CharaPos charaPos, ref CharaLastPos charaLastPos,
                ref CharaFlag charaFlag, ref CharaQueue charaQueue)
            {
                // MapX移動が無いときはチェックしない
                if (charaPos.m_mapY == charaLastPos.m_mapY)
                    return;

                int mapX = 0;
                switch (xPos)
                {
                    case XPos.Left:
                        mapX = charaPos.m_mapXLeft;
                        break;
                    case XPos.Center:
                        mapX = charaPos.m_mapXCenter;
                        break;
                    case XPos.Right:
                        mapX = charaPos.m_mapXRight;
                        break;
                }

                EnumShapeType topShape = GetShape(mapX, charaPos.m_mapY + 1, charaPos.m_mapZ);
                EnumShapeType shape = GetShape(mapX, charaPos.m_mapY, charaPos.m_mapZ);

                //ひとつ上と同じチップの場合はチェックしない
                if (shape == topShape)
                    return;

                // Yめり込みチェック
                bool isHit = false;
                switch (shape)
                {
                    case EnumShapeType.Box:
                        isHit = true;
                        break;
                    case EnumShapeType.BSlashWall:
                        isHit = (xPos == XPos.Right) && (charaPos.m_tipXRight >= (TIP_SIZE - 1 - charaPos.m_tipZ));
                        break;
                    case EnumShapeType.SlashWall:
                        isHit = (xPos == XPos.Left) && (charaPos.m_tipXLeft <= charaPos.m_tipZ);
                        break;
                }

                if (isHit)
                {
                    int newY = (charaPos.m_mapY + 1) << PIX_MAP;
                    charaPos.SetPixY(newY);
                    LandQueue(ref charaFlag, ref charaQueue);
                }
            }

            private void CheckXBox(XPos xPos, ref CharaPos charaPos, ref CharaLastPos charaLastPos,
                ref CharaFlag charaFlag, ref CharaQueue charaQueue)
            {
                int mapX = 0;
                int lastMapX = 0;
                int sideMapX = 0;
                int offsetX = 0;

                switch (xPos)
                {
                    case XPos.Left:
                        mapX = charaPos.m_mapXLeft;
                        lastMapX = charaLastPos.m_mapXLeft;
                        sideMapX = mapX + 1;
                        offsetX = +1 + SIDE_OFFSET;
                        break;
                    case XPos.Center:
                        mapX = charaPos.m_mapXCenter;
                        lastMapX = charaLastPos.m_mapXCenter;
                        break;
                    case XPos.Right:
                        mapX = charaPos.m_mapXRight;
                        lastMapX = charaLastPos.m_mapXRight;
                        sideMapX = mapX - 1;
                        offsetX = -1 - SIDE_OFFSET;
                        break;
                }

                // MapX移動が無いときはチェックしない
                if (mapX == lastMapX)
                    return;

                // 横が空の時のみ、壁チェック
                EnumShapeType sideShape = GetShape(sideMapX, charaPos.m_mapY, charaPos.m_mapZ);

                if (!sideShape.IsEmpty())
                    return;

                // ブロックのみ補正行う、斜め壁は別途
                EnumShapeType shape = GetShape(mapX, charaPos.m_mapY, charaPos.m_mapZ);

                if (!shape.IsBox())
                    return;

                // 中心位置への補正SIDE_OFFSET
                int newX = (mapX << PIX_MAP) + offsetX;
                charaPos.SetPixX(newX);
                CrashQueue(ref charaFlag, ref charaQueue);
            }


            private void CheckZBox(XPos xPos, ref CharaPos charaPos, ref CharaLastPos charaLastPos,
                ref CharaFlag charaFlag, ref CharaQueue charaQueue)
            {

                // MapZ移動が無いときはZチェックしない
                if (charaPos.m_mapZ != charaLastPos.m_mapZ)
                    return;

                int mapX = 0;
                switch (xPos)
                {
                    case XPos.Left:
                        mapX = charaPos.m_mapXLeft;
                        break;
                    case XPos.Center:
                        mapX = charaPos.m_mapXCenter;
                        break;
                    case XPos.Right:
                        mapX = charaPos.m_mapXRight;
                        break;
                }

                // 手前が壁以外のときのみ壁チェック
                EnumShapeType frontShape = GetShape(mapX, charaPos.m_mapY, charaPos.m_mapZ - 1);

                if (frontShape.IsBox())
                    return;

                // ブロックのみ補正行う、斜め壁は別途
                EnumShapeType shape = GetShape(mapX, charaPos.m_mapY, charaPos.m_mapZ);

                if (!shape.IsBox())
                    return;

                int newRZ = (charaPos.m_mapZ << PIX_MAP) - 1;
                charaPos.SetPixZ(newRZ);
            }


            private void CheckYSlope(XPos xPos, ref CharaPos charaPos, ref CharaLastPos charaLastPos,
                ref CharaFlag charaFlag, ref CharaQueue charaQueue)
            {
                EnumShapeType shape = GetShape(charaPos.m_mapXCenter, charaPos.m_mapY, charaPos.m_mapZ);

                if (!shape.IsSlope())
                    return;

                // 坂座標
                int borderY = 0;
                switch (shape)
                {
                    case EnumShapeType.LUpSlope:
                        borderY = (TIP_SIZE - 1 - charaPos.m_tipXCenter);
                        break;
                    case EnumShapeType.RUpSlope:
                        borderY = charaPos.m_tipXCenter;
                        break;
                    case EnumShapeType.LUpSlope2L:
                        borderY = TIP_SIZE_HALF - 1 - (charaPos.m_tipXCenter >> 1);
                        break;
                    case EnumShapeType.LUpSlope2H:
                        borderY = TIP_SIZE - 1 - (charaPos.m_tipXCenter >> 1);
                        break;
                    case EnumShapeType.RUpSlope2L:
                        borderY = charaPos.m_tipXCenter >> 1;
                        break;
                    case EnumShapeType.RUpSlope2H:
                        borderY = TIP_SIZE_HALF + charaPos.m_tipXCenter >> 1;
                        break;
                }

                if (charaPos.m_tipY <= borderY)
                {
                    charaPos.SetPixY(borderY + 1);
                    LandQueue(ref charaFlag, ref charaQueue);
                }
            }

            private void CheckXZSlashWall(XPos xPos, ref CharaPos charaPos, ref CharaLastPos charaLastPos,
                ref CharaFlag charaFlag, ref CharaQueue charaQueue)
            {

                int mapX = 0;
                int tipX = 0;
                int offsetX = 0;
                switch (xPos)
                {
                    case XPos.Left:
                        mapX = charaPos.m_mapXLeft;
                        tipX = charaPos.m_tipXLeft;
                        offsetX = +1 + SIDE_OFFSET;
                        break;
                    case XPos.Center:
                        mapX = charaPos.m_mapXCenter;
                        tipX = charaPos.m_tipXCenter;
                        break;
                    case XPos.Right:
                        mapX = charaPos.m_mapXRight;
                        tipX = charaPos.m_tipXRight;
                        offsetX = -1 - SIDE_OFFSET;
                        break;
                }

                EnumShapeType shape = GetShape(mapX, charaPos.m_mapY, charaPos.m_mapZ);

                if (!shape.IsSlashWall())
                    return;


                int moveX = math.abs(charaPos.m_position.x - charaLastPos.m_position.x);
                int moveZ = math.abs(charaPos.m_position.z - charaLastPos.m_position.z);
                bool isZSlide = (moveX > moveZ);

                if (isZSlide)
                {
                    // 斜め壁座標
                    int borderX = 0;
                    int shiftZ = 0;
                    bool isHit = false;
                    switch (shape)
                    {
                        case EnumShapeType.SlashWall:
                            borderX = charaPos.m_tipZ;
                            shiftZ = -(tipX + 1);
                            isHit = tipX <= borderX;
                            break;
                        case EnumShapeType.BSlashWall:
                            borderX = TIP_SIZE - 1 - charaPos.m_tipZ;
                            shiftZ = -(TIP_SIZE - tipX);
                            isHit = tipX >= borderX;
                            break;
                    }

                    if (isHit)
                    {
                        int newZ = ((charaPos.m_mapZ + 1) << PIX_MAP) + shiftZ;
                        charaPos.SetPixZ(newZ);
                    }
                }
                else
                {
                    int borderZ = 0;
                    int shiftX = 0;
                    switch (shape)
                    {
                        case EnumShapeType.SlashWall:
                            borderZ = tipX;
                            shiftX = charaPos.m_tipZ + 1;
                            break;
                        case EnumShapeType.BSlashWall:
                            borderZ = TIP_SIZE - 1 - tipX;
                            shiftX = -(TIP_SIZE - charaPos.m_tipZ);
                            break;
                    }

                    if (charaPos.m_tipZ >= borderZ)
                    {
                        int newX = (mapX << PIX_MAP) + shiftX + offsetX;
                        charaPos.SetPixX(newX);
                    }
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
