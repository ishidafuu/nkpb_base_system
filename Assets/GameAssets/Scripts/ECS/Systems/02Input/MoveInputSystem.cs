using System;
using System.Collections.ObjectModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    public class MoveInputSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaDelta>(),
                ComponentType.ReadOnly<CharaDash>(),
                ComponentType.ReadOnly<CharaFlag>(),
                ComponentType.ReadOnly<PadScan>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            NativeArray<CharaDelta> charaDeltas = m_query.ToComponentDataArray<CharaDelta>(Allocator.TempJob);
            NativeArray<CharaDash> charaDashes = m_query.ToComponentDataArray<CharaDash>(Allocator.TempJob);
            NativeArray<CharaFlag> charaFlags = m_query.ToComponentDataArray<CharaFlag>(Allocator.TempJob);
            NativeArray<PadScan> padScans = m_query.ToComponentDataArray<PadScan>(Allocator.TempJob);

            var job = new MoveJob()
            {
                m_charaDeltas = charaDeltas,
                m_charaDashes = charaDashes,
                m_charaFlags = charaFlags,
                m_padScans = padScans,
                BrakeDelta = Settings.Instance.Move.BrakeDelta,
                WalkSpeed = Settings.Instance.Move.WalkSpeed,
                DashSpeed = Settings.Instance.Move.DashSpeed,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaDeltas);

            charaDeltas.Dispose();
            charaDashes.Dispose();
            charaFlags.Dispose();
            padScans.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct MoveJob : IJob
        {
            public NativeArray<CharaDelta> m_charaDeltas;
            [ReadOnly] public NativeArray<CharaDash> m_charaDashes;
            [ReadOnly] public NativeArray<CharaFlag> m_charaFlags;
            [ReadOnly] public NativeArray<PadScan> m_padScans;
            [ReadOnly] public int BrakeDelta;
            [ReadOnly] public int WalkSpeed;
            [ReadOnly] public int DashSpeed;

            public void Execute()
            {

                for (int i = 0; i < m_charaFlags.Length; i++)
                {
                    var charaFlag = m_charaFlags[i];
                    var charaDelta = m_charaDeltas[i];

                    if (charaFlag.m_moveFlag.IsFlag(FlagMove.Walk))
                    {
                        SetDelta(ref charaDelta, WalkSpeed, InputToMoveMuki(i));
                    }

                    if (charaFlag.m_moveFlag.IsFlag(FlagMove.Dash))
                    {
                        SetDelta(ref charaDelta, DashSpeed, InputToMoveMukiDash(i));
                    }

                    m_charaDeltas[i] = charaDelta;
                }
            }

            void SetDelta(ref CharaDelta charaDelta, int _delta, EnumMoveMuki _moveMuki)
            {
                var delta = DeltaToVector3Int(_delta, _moveMuki);
                charaDelta.m_deltaX = delta.x;
                charaDelta.m_deltaY = delta.y;
                charaDelta.m_deltaZ = delta.z;
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

                if (m_padScans[i].m_crossLeft.m_isPress)
                {
                    if (m_padScans[i].m_crossUp.m_isPress)
                    {
                        res = EnumMoveMuki.LeftUp;
                    }
                    else if (m_padScans[i].m_crossDown.m_isPress)
                    {
                        res = EnumMoveMuki.LeftDown;
                    }
                    else
                    {
                        res = EnumMoveMuki.Left;
                    }
                }
                else if (m_padScans[i].m_crossRight.m_isPress)
                {
                    if (m_padScans[i].m_crossUp.m_isPress)
                    {
                        res = EnumMoveMuki.RightUp;
                    }
                    else if (m_padScans[i].m_crossDown.m_isPress)
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
                    if (m_padScans[i].m_crossUp.m_isPress)
                    {
                        res = EnumMoveMuki.Up;
                    }
                    else if (m_padScans[i].m_crossDown.m_isPress)
                    {
                        res = EnumMoveMuki.Down;
                    }
                }

                return res;
            }

            EnumMoveMuki InputToMoveMukiDash(int i)
            {
                var res = EnumMoveMuki.None;
                var charaDash = m_charaDashes[i];
                if (charaDash.m_dashMuki == EnumMuki.Left)
                {
                    if (m_padScans[i].m_crossLeft.m_isPress)
                    {
                        if (m_padScans[i].m_crossUp.m_isPress)
                        {
                            res = EnumMoveMuki.LeftLeftUp;
                        }
                        else if (m_padScans[i].m_crossDown.m_isPress)
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
                        if (m_padScans[i].m_crossUp.m_isPress)
                        {
                            res = EnumMoveMuki.LeftUp;
                        }
                        else if (m_padScans[i].m_crossDown.m_isPress)
                        {
                            res = EnumMoveMuki.LeftDown;
                        }
                        else
                        {
                            res = EnumMoveMuki.Left;
                        }
                    }
                }
                else if (charaDash.m_dashMuki == EnumMuki.Right)
                {
                    if (m_padScans[i].m_crossRight.m_isPress)
                    {
                        if (m_padScans[i].m_crossUp.m_isPress)
                        {
                            res = EnumMoveMuki.RightRightUp;
                        }
                        else if (m_padScans[i].m_crossDown.m_isPress)
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
                        if (m_padScans[i].m_crossUp.m_isPress)
                        {
                            res = EnumMoveMuki.RightUp;
                        }
                        else if (m_padScans[i].m_crossDown.m_isPress)
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
