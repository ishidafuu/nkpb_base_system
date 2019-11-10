﻿using System;
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

        protected override void OnCreateManager()
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

                    if (charaFlag.moveFlag.IsFlag(FlagMove.Walk))
                    {
                        SetDelta(ref charaDelta, WalkSpeed, InputToMoveMuki(i));
                        // Debug.Log("Walk" + i);
                    }

                    if (charaFlag.moveFlag.IsFlag(FlagMove.Friction))
                    {
                        UpdateFriction(ref charaDelta, BrakeDelta);
                    }

                    if (charaFlag.moveFlag.IsFlag(FlagMove.Stop))
                    {
                        ClearDelta(ref charaDelta);
                    }

                    if (charaFlag.moveFlag.IsFlag(FlagMove.Dash))
                    {
                        SetDelta(ref charaDelta, DashSpeed, InputToMoveMukiDash(i));
                        // Debug.Log("Dash");
                    }

                    m_charaDeltas[i] = charaDelta;
                }
            }

            void UpdateFriction(ref CharaDelta charaDelta, int _brakeDelta)
            {
                if (charaDelta.m_delta.x > 0)
                {
                    charaDelta.m_delta.x = Mathf.Min(0, charaDelta.m_delta.x - _brakeDelta);
                }
                else if (charaDelta.m_delta.x < 0)
                {
                    charaDelta.m_delta.x = Mathf.Max(0, charaDelta.m_delta.x + _brakeDelta);
                }

                if (charaDelta.m_delta.z > 0)
                {
                    charaDelta.m_delta.z = Mathf.Min(0, charaDelta.m_delta.z - _brakeDelta);
                }
                else if (charaDelta.m_delta.x < 0)
                {
                    charaDelta.m_delta.z = Mathf.Max(0, charaDelta.m_delta.z + _brakeDelta);
                }
            }

            void ClearDelta(ref CharaDelta charaPos)
            {
                charaPos.m_delta.x = 0;
                charaPos.m_delta.z = 0;
            }


            void SetDelta(ref CharaDelta charaDelta, int _delta, EnumMoveMuki _moveMuki)
            {
                charaDelta.m_delta = DeltaToVector3Int(_delta, _moveMuki);
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
                if (charaDash.dashMuki == EnumMuki.Left)
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
                else if (charaDash.dashMuki == EnumMuki.Right)
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