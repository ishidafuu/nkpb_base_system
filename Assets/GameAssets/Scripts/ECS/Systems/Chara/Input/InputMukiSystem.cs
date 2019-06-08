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
    /// 入力による向き変化システム
    /// </summary>
    public class InputMukiSystem : JobComponentSystem
    {
        ComponentGroup m_group;

        protected override void OnCreateManager()
        {
            m_group = GetComponentGroup(
                ComponentType.Create<CharaMuki>(),
                ComponentType.ReadOnly<CharaFlag>(),
                ComponentType.ReadOnly<PadScan>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new InputJob()
            {
                m_charaMukis = m_group.GetComponentDataArray<CharaMuki>(),
                m_charaFlags = m_group.GetComponentDataArray<CharaFlag>(),
                m_PadScans = m_group.GetComponentDataArray<PadScan>(),
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            return inputDeps;
        }

        [BurstCompileAttribute]
        struct InputJob : IJob
        {
            public ComponentDataArray<CharaMuki> m_charaMukis;
            [ReadOnly]
            public ComponentDataArray<CharaFlag> m_charaFlags;
            [ReadOnly]
            public ComponentDataArray<PadScan> m_PadScans;
            public void Execute()
            {

                for (int i = 0; i < m_charaFlags.Length; i++)
                {
                    //モーションごとの入力
                    if (m_charaFlags[i].mukiFlag)
                    {
                        CheckCrossX(i);
                    }
                }
            }

            //左右チェック
            bool CheckCrossX(int i)
            {
                if (m_PadScans[i].crossLeft.isPress
                    || m_PadScans[i].crossRight.isPress)
                {
                    var charaMuki = m_charaMukis[i];
                    charaMuki.muki = m_PadScans[i].crossLeft.isPress
                        ? EnumMuki.Left
                        : EnumMuki.Right;
                    m_charaMukis[i] = charaMuki;
                    return true;
                }

                return false;
            }
        }

    }
}
