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
    [UpdateInGroup(typeof(InputGroup))]
    public class InputMukiSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaMuki>(),
                ComponentType.ReadOnly<CharaFlag>(),
                ComponentType.ReadOnly<PadScan>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaMuki> charaMukis = m_query.ToComponentDataArray<CharaMuki>(Allocator.TempJob);
            NativeArray<CharaFlag> charaFlags = m_query.ToComponentDataArray<CharaFlag>(Allocator.TempJob);
            NativeArray<PadScan> padScans = m_query.ToComponentDataArray<PadScan>(Allocator.TempJob);
            var job = new InputJob()
            {
                m_charaMukis = charaMukis,
                m_charaFlags = charaFlags,
                m_padScans = padScans,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.m_charaMukis);

            charaMukis.Dispose();
            charaFlags.Dispose();
            padScans.Dispose();

            return inputDeps;
        }

        [BurstCompileAttribute]
        struct InputJob : IJob
        {
            public NativeArray<CharaMuki> m_charaMukis;
            [ReadOnly] public NativeArray<CharaFlag> m_charaFlags;
            [ReadOnly] public NativeArray<PadScan> m_padScans;
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
                if (m_padScans[i].m_crossLeft.m_isPress
                    || m_padScans[i].m_crossRight.m_isPress)
                {
                    var charaMuki = m_charaMukis[i];
                    charaMuki.muki = m_padScans[i].m_crossLeft.m_isPress
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
