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
                charaMukis = charaMukis,
                charaFlags = charaFlags,
                padScans = padScans,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.charaMukis);

            charaMukis.Dispose();
            charaFlags.Dispose();
            padScans.Dispose();

            return inputDeps;
        }

        [BurstCompileAttribute]
        struct InputJob : IJob
        {
            public NativeArray<CharaMuki> charaMukis;
            [ReadOnly] public NativeArray<CharaFlag> charaFlags;
            [ReadOnly] public NativeArray<PadScan> padScans;
            public void Execute()
            {

                for (int i = 0; i < charaFlags.Length; i++)
                {
                    //モーションごとの入力
                    if (charaFlags[i].mukiFlag)
                    {
                        CheckCrossX(i);
                    }
                }
            }

            //左右チェック
            bool CheckCrossX(int i)
            {
                if (padScans[i].crossLeft.isPress
                    || padScans[i].crossRight.isPress)
                {
                    var charaMuki = charaMukis[i];
                    charaMuki.muki = padScans[i].crossLeft.isPress
                        ? EnumMuki.Left
                        : EnumMuki.Right;
                    charaMukis[i] = charaMuki;
                    return true;
                }

                return false;
            }
        }

    }
}
