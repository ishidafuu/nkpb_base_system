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
    /// <summary>
    /// 入力による状態変化システム
    /// </summary>
    public class InputMotionJobSystem : JobComponentSystem
    {
        ComponentGroup m_group;

        protected override void OnCreateManager()
        {
            m_group = GetComponentGroup(
                ComponentType.Create<CharaMotion>(),
                ComponentType.Create<CharaDash>(),
                ComponentType.ReadOnly<PadScan>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new InputJob()
            {
                m_charaMotions = m_group.GetComponentDataArray<CharaMotion>(),
                m_charaDashs = m_group.GetComponentDataArray<CharaDash>(),
                m_PadScans = m_group.GetComponentDataArray<PadScan>(),
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            return inputDeps;
        }

        [BurstCompileAttribute]
        struct InputJob : IJob
        {
            public ComponentDataArray<CharaMotion> m_charaMotions;
            public ComponentDataArray<CharaDash> m_charaDashs;
            [ReadOnly]
            public ComponentDataArray<PadScan> m_PadScans;

            public void Execute()
            {

                for (int i = 0; i < m_charaMotions.Length; i++)
                {
                    //モーションごとの入力
                    switch (m_charaMotions[i].motionType)
                    {
                        case EnumMotion.Idle:
                            UpdateIdle(i);
                            break;
                        case EnumMotion.Walk:
                            UpdateWalk(i);
                            break;
                        case EnumMotion.Dash:
                            UpdateDash(i);
                            break;
                        case EnumMotion.Slip:
                            UpdateSlip(i);
                            break;
                        case EnumMotion.Jump:
                            break;
                        case EnumMotion.Fall:
                            break;
                        case EnumMotion.Land:
                            break;
                        case EnumMotion.Damage:
                            break;
                        case EnumMotion.Fly:
                            break;
                        case EnumMotion.Down:
                            break;
                        case EnumMotion.Dead:
                            break;
                        case EnumMotion.Action:
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }
                }
            }

            void UpdateIdle(int i)
            {
                if (CheckWalk(i))
                    return;
            }
            void UpdateWalk(int i)
            {
                // if (CheckJump(i))
                // 	return;
                // if (CheckRun(i))
                // 	return;
                if (CheckIdle(i))
                    return;
            }

            void UpdateDash(int i)
            {
                if (CheckJump(i))
                    return;
                if (CheckSlip(i))
                    return;
            }

            void UpdateSlip(int i)
            {

            }

            void UpdateLand(int i)
            {

            }

            void UpdateDamage(int i)
            {

            }

            void UpdateDown(int i)
            {

            }

            void UpdateDead(int i)
            {

            }

            /// <summary>
            /// アイドルチェック
            /// </summary>
            /// <param name="motion"></param>
            /// <param name="PadScan"></param>
            /// <returns></returns>
            bool CheckIdle(int i)
            {
                if (!m_PadScans[i].IsAnyCrossPress())
                {
                    var charaMotion = m_charaMotions[i];
                    charaMotion.SetMotion(EnumMotion.Idle);
                    m_charaMotions[i] = charaMotion;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// 歩きチェック
            /// </summary>
            /// <param name="motion"></param>
            /// <param name="PadScan"></param>
            /// <returns></returns>
            bool CheckWalk(int i)
            {
                if (m_PadScans[i].IsAnyCrossPress())
                {
                    var charaMotion = m_charaMotions[i];
                    charaMotion.SetMotion(EnumMotion.Walk);
                    m_charaMotions[i] = charaMotion;
                    return true;
                }

                return false;
            }
            /// <summary>
            /// ジャンプチェック
            /// </summary>
            /// <param name="motion"></param>
            /// <param name="PadScan"></param>
            /// <returns></returns>
            bool CheckJump(int i)
            {
                if (m_PadScans[i].IsJumpPush())
                {
                    var charaMotion = m_charaMotions[i];
                    charaMotion.SetMotion(EnumMotion.Jump);
                    m_charaMotions[i] = charaMotion;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// ダッシュチェック
            /// </summary>
            /// <param name="motion"></param>
            /// <param name="PadScan"></param>
            /// /// <returns></returns>
            bool CheckDash(int i)
            {
                if (m_PadScans[i].crossLeft.isDouble || m_PadScans[i].crossRight.isDouble)
                {
                    var charaMotion = m_charaMotions[i];
                    charaMotion.SetMotion(EnumMotion.Dash);
                    m_charaMotions[i] = charaMotion;

                    // ダッシュ向き
                    var charaDash = m_charaDashs[i];
                    charaDash.dashMuki = (m_PadScans[i].crossLeft.isDouble)
                        ? EnumMuki.Left
                        : EnumMuki.Right;
                    m_charaDashs[i] = charaDash;

                    return true;
                }

                return false;
            }

            /// <summary>
            /// 滑りチェック
            /// </summary>
            /// <param name="motion"></param>
            /// <param name="PadScan"></param>
            /// <returns></returns>
            bool CheckSlip(int i)
            {
                if (m_PadScans[i].IsAnyCrossPress())
                {
                    var charaMotion = m_charaMotions[i];
                    charaMotion.SetMotion(EnumMotion.Slip);
                    m_charaMotions[i] = charaMotion;
                    return true;
                }

                return false;
            }
        }
    }
}
