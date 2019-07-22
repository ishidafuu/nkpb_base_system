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

    public class LookSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<CharaLook>(),
                ComponentType.ReadOnly<CharaMuki>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            NativeArray<CharaLook> charaLooks = m_query.ToComponentDataArray<CharaLook>(Allocator.TempJob);
            NativeArray<CharaMuki> charaMukis = m_query.ToComponentDataArray<CharaMuki>(Allocator.TempJob);

            var job = new ConvertJob()
            {
                charaLooks = charaLooks,
                charaMukis = charaMukis,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.CopyFromComponentDataArray(job.charaLooks);
            charaLooks.Dispose();
            charaMukis.Dispose();

            return inputDeps;
        }

        [BurstCompileAttribute]
        struct ConvertJob : IJob
        {
            public NativeArray<CharaLook> charaLooks;
            [ReadOnly] public NativeArray<CharaMuki> charaMukis;

            public void Execute()
            {
                for (int i = 0; i < charaLooks.Length; i++)
                {
                    var look = charaLooks[i];
                    look.isLeft = (charaMukis[i].muki == EnumMuki.Left)
                        ? 1
                        : 0;
                    charaLooks[i] = look;
                }
            }
        }

    }
}
