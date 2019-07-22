using System;
using System.Collections.ObjectModel;
using HedgehogTeam.EasyTouch;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    public class InputMoveSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaMove>(),
                ComponentType.ReadOnly<CharaDash>(),
                ComponentType.ReadOnly<CharaFlag>(),
                ComponentType.ReadOnly<PadScan>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            NativeArray<CharaMove> charaMoves = m_query.ToComponentDataArray<CharaMove>(Allocator.TempJob);
            NativeArray<CharaDash> charaDashs = m_query.ToComponentDataArray<CharaDash>(Allocator.TempJob);
            NativeArray<CharaFlag> charaFlags = m_query.ToComponentDataArray<CharaFlag>(Allocator.TempJob);
            NativeArray<PadScan> padScans = m_query.ToComponentDataArray<PadScan>(Allocator.TempJob);

            var job = new MoveJob()
            {
                charaMoves = charaMoves,
                charaDashs = charaDashs,
                charaFlags = charaFlags,
                padScans = padScans,
                brakeDelta = Settings.Instance.Move.BrakeDelta,
                walkSpeed = Settings.Instance.Move.WalkSpeed,
                dashSpeed = Settings.Instance.Move.DashSpeed,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.charaMoves);

            charaMoves.Dispose();
            charaDashs.Dispose();
            charaFlags.Dispose();
            padScans.Dispose();

            return inputDeps;
        }

        [BurstCompileAttribute]
        struct MoveJob : IJob
        {
            public NativeArray<CharaMove> charaMoves;
            [ReadOnly] public NativeArray<CharaDash> charaDashs;
            [ReadOnly] public NativeArray<CharaFlag> charaFlags;
            [ReadOnly] public NativeArray<PadScan> padScans;
            [ReadOnly] public int brakeDelta;
            [ReadOnly] public int walkSpeed;
            [ReadOnly] public int dashSpeed;

            public void Execute()
            {

                for (int i = 0; i < charaFlags.Length; i++)
                {
                    var charaFlag = charaFlags[i];

                    if (charaFlag.moveFlag.IsFlag(FlagMove.Walk))
                    {
                        Walk(i);
                    }

                    if (charaFlag.moveFlag.IsFlag(FlagMove.Friction))
                    {
                        Friction(i);
                    }

                    if (charaFlag.moveFlag.IsFlag(FlagMove.Stop))
                    {
                        Stop(i);
                    }

                    if (charaFlag.moveFlag.IsFlag(FlagMove.Dash))
                    {
                        Dash(i);
                    }
                }
            }

            void Friction(int i)
            {
                CharaMove charaMove = charaMoves[i];
                UpdateFriction(ref charaMove, brakeDelta);
                charaMoves[i] = charaMove;
            }

            // XZ方向減速
            void UpdateFriction(ref CharaMove charaMove, int _brakeDelta)
            {
                if (charaMove.delta.x > 0)
                {
                    charaMove.delta.x = Mathf.Min(0, charaMove.delta.x - _brakeDelta);
                }
                else if (charaMove.delta.x < 0)
                {
                    charaMove.delta.x = Mathf.Max(0, charaMove.delta.x + _brakeDelta);
                }

                if (charaMove.delta.z > 0)
                {
                    charaMove.delta.z = Mathf.Min(0, charaMove.delta.z - _brakeDelta);
                }
                else if (charaMove.delta.x < 0)
                {
                    charaMove.delta.z = Mathf.Max(0, charaMove.delta.z + _brakeDelta);
                }
            }

            void Stop(int i)
            {
                CharaMove charaMove = charaMoves[i];
                ClearDelta(ref charaMove);
                charaMoves[i] = charaMove;
            }

            //XZ方向停止
            void ClearDelta(ref CharaMove charaMove)
            {
                charaMove.delta.x = 0;
                charaMove.delta.z = 0;
            }

            void Walk(int i)
            {
                // Debug.Log("Walk");
                CharaMove charaMove = charaMoves[i];
                SetDelta(ref charaMove, walkSpeed, InputToMoveMuki(i));
                charaMoves[i] = charaMove;
            }

            void SetDelta(ref CharaMove charaMove, int _delta, EnumMoveMuki _moveMuki)
            {
                charaMove.delta = DeltaToVector3Int(_delta, _moveMuki);
            }

            Vector3Int DeltaToVector3Int(int _delta, EnumMoveMuki _moveMuki)
            {
                Vector3 res = Vector3.zero;
                const float NANAME45 = 0.7f;
                const float NANAME30 = 0.5f;
                const float NANAME30L = 0.87f;
                switch (_moveMuki)
                {
                    case EnumMoveMuki.Left:
                        res = new Vector3(-1, 0, 0);
                        break;
                    case EnumMoveMuki.LeftLeftDown:
                        res = new Vector3(-NANAME30L, 0, -NANAME30);
                        break;
                    case EnumMoveMuki.LeftDown:
                        res = new Vector3(-NANAME45, 0, -NANAME45);
                        break;
                    case EnumMoveMuki.LeftLeftUp:
                        res = new Vector3(-NANAME30L, 0, +NANAME30);
                        break;
                    case EnumMoveMuki.LeftUp:
                        res = new Vector3(-NANAME45, 0, +NANAME45);
                        break;
                    case EnumMoveMuki.Right:
                        res = new Vector3(1, 0, 0);
                        break;
                    case EnumMoveMuki.RightRightDown:
                        res = new Vector3(+NANAME30L, 0, -NANAME30);
                        break;
                    case EnumMoveMuki.RightDown:
                        res = new Vector3(+NANAME45, 0, -NANAME45);
                        break;
                    case EnumMoveMuki.RightRightUp:
                        res = new Vector3(+NANAME30L, 0, +NANAME30);
                        break;
                    case EnumMoveMuki.RightUp:
                        res = new Vector3(+NANAME45, 0, +NANAME45);
                        break;
                    case EnumMoveMuki.Up:
                        res = new Vector3(0, 0, 1);
                        break;
                    case EnumMoveMuki.Down:
                        res = new Vector3(0, 0, -1);
                        break;
                }
                res *= _delta;
                return new Vector3Int((int)res.x, 0, (int)res.z);
            }

            EnumMoveMuki InputToMoveMuki(int i)
            {
                var res = EnumMoveMuki.None;

                if (padScans[i].crossLeft.isPress)
                {
                    if (padScans[i].crossUp.isPress)
                    {
                        res = EnumMoveMuki.LeftUp;
                    }
                    else if (padScans[i].crossDown.isPress)
                    {
                        res = EnumMoveMuki.LeftDown;
                    }
                    else
                    {
                        res = EnumMoveMuki.Left;
                    }
                }
                else if (padScans[i].crossRight.isPress)
                {
                    if (padScans[i].crossUp.isPress)
                    {
                        res = EnumMoveMuki.RightUp;
                    }
                    else if (padScans[i].crossDown.isPress)
                    {
                        res = EnumMoveMuki.RightDown;
                    }
                    else
                    {
                        res = EnumMoveMuki.Right;
                    }
                }
                else
                {
                    if (padScans[i].crossUp.isPress)
                    {
                        res = EnumMoveMuki.Up;
                    }
                    else if (padScans[i].crossDown.isPress)
                    {
                        res = EnumMoveMuki.Down;
                    }
                }

                return res;
            }

            void Dash(int i)
            {
                // Debug.Log("Dash");
                CharaMove charaMove = charaMoves[i];
                SetDelta(ref charaMove, dashSpeed, InputToMoveMukiDash(i));
                charaMoves[i] = charaMove;
            }

            EnumMoveMuki InputToMoveMukiDash(int i)
            {
                var res = EnumMoveMuki.None;
                var charaDash = charaDashs[i];
                if (charaDash.dashMuki == EnumMuki.Left)
                {
                    if (padScans[i].crossLeft.isPress)
                    {
                        if (padScans[i].crossUp.isPress)
                        {
                            res = EnumMoveMuki.LeftLeftUp;
                        }
                        else if (padScans[i].crossDown.isPress)
                        {
                            res = EnumMoveMuki.LeftLeftDown;
                        }
                        else
                        {
                            res = EnumMoveMuki.Left;
                        }
                    }
                    else
                    {
                        if (padScans[i].crossUp.isPress)
                        {
                            res = EnumMoveMuki.LeftUp;
                        }
                        else if (padScans[i].crossDown.isPress)
                        {
                            res = EnumMoveMuki.LeftDown;
                        }
                        else
                        {
                            res = EnumMoveMuki.Left;
                        }
                    }
                }
                else if (charaDash.dashMuki == EnumMuki.Right)
                {
                    if (padScans[i].crossRight.isPress)
                    {
                        if (padScans[i].crossUp.isPress)
                        {
                            res = EnumMoveMuki.RightRightUp;
                        }
                        else if (padScans[i].crossDown.isPress)
                        {
                            res = EnumMoveMuki.RightRightDown;
                        }
                        else
                        {
                            res = EnumMoveMuki.Right;
                        }
                    }
                    else
                    {
                        if (padScans[i].crossUp.isPress)
                        {
                            res = EnumMoveMuki.RightUp;
                        }
                        else if (padScans[i].crossDown.isPress)
                        {
                            res = EnumMoveMuki.RightDown;
                        }
                        else
                        {
                            res = EnumMoveMuki.Right;
                        }
                    }
                }
                return res;
            }
        }

    }
}
