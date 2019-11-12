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
    public class MoveCountSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaDelta>(),
                ComponentType.ReadWrite<CharaFlag>(),
                ComponentType.ReadOnly<CharaDash>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            NativeArray<CharaDelta> charaDeltas = m_query.ToComponentDataArray<CharaDelta>(Allocator.TempJob);
            NativeArray<CharaDash> charaDashes = m_query.ToComponentDataArray<CharaDash>(Allocator.TempJob);
            NativeArray<CharaFlag> charaFlags = m_query.ToComponentDataArray<CharaFlag>(Allocator.TempJob);

            var job = new MoveJob()
            {
                m_charaDeltas = charaDeltas,
                m_charaFlags = charaFlags,
                m_charaDashes = charaDashes,
                BrakeDelta = Settings.Instance.Move.BrakeDelta,
                Gravity = Settings.Instance.Move.Gravity,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaDeltas);
            m_query.CopyFromComponentDataArray(job.m_charaFlags);

            charaDeltas.Dispose();
            charaDashes.Dispose();
            charaFlags.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct MoveJob : IJob
        {
            public NativeArray<CharaDelta> m_charaDeltas;
            public NativeArray<CharaFlag> m_charaFlags;
            [ReadOnly] public NativeArray<CharaDash> m_charaDashes;
            [ReadOnly] public int BrakeDelta;
            [ReadOnly] public int Gravity;

            public void Execute()
            {

                for (int i = 0; i < m_charaFlags.Length; i++)
                {
                    var charaFlag = m_charaFlags[i];
                    var charaDelta = m_charaDeltas[i];

                    if (charaFlag.m_moveFlag.IsFlag(FlagMove.Friction))
                    {
                        UpdateFriction(ref charaDelta, BrakeDelta);
                    }

                    if (charaFlag.m_moveFlag.IsFlag(FlagMove.Air))
                    {
                        UpdateGravity(ref charaDelta, ref charaFlag, Gravity);
                    }

                    if (charaFlag.m_moveFlag.IsFlag(FlagMove.Stop))
                    {
                        ClearDelta(ref charaDelta);
                    }

                    m_charaFlags[i] = charaFlag;
                    m_charaDeltas[i] = charaDelta;
                }
            }

            void UpdateFriction(ref CharaDelta charaDelta, int brakeDelta)
            {
                if (charaDelta.m_delta.x > 0)
                {
                    charaDelta.m_delta.x = Mathf.Min(0, charaDelta.m_delta.x - brakeDelta);
                }
                else if (charaDelta.m_delta.x < 0)
                {
                    charaDelta.m_delta.x = Mathf.Max(0, charaDelta.m_delta.x + brakeDelta);
                }

                if (charaDelta.m_delta.z > 0)
                {
                    charaDelta.m_delta.z = Mathf.Min(0, charaDelta.m_delta.z - brakeDelta);
                }
                else if (charaDelta.m_delta.x < 0)
                {
                    charaDelta.m_delta.z = Mathf.Max(0, charaDelta.m_delta.z + brakeDelta);
                }
            }

            void UpdateGravity(ref CharaDelta charaDelta, ref CharaFlag charaFlag, int gravity)
            {
                charaDelta.m_delta.y -= gravity;

                if (!charaFlag.m_mapFlag.IsFlag(FlagMapCheck.Land)
                    && charaDelta.m_delta.y < 0)
                {
                    charaFlag.m_mapFlag.AddFlag(FlagMapCheck.Land);
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

        }

    }
}
