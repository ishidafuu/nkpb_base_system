using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    [UpdateInGroup(typeof(RenderGroup))]
    public class CharaDrawSystem : JobComponentSystem
    {
        EntityQuery m_query;
        Quaternion m_Quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        Quaternion m_QuaternionRev = Quaternion.Euler(new Vector3(-90, 180, 0));

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<CharaMuki>(),
                ComponentType.ReadOnly<CharaLook>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<CharaMotion>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_query.AddDependency(inputDeps);
            NativeArray<Matrix4x4> charaMatrixes = new NativeArray<Matrix4x4>(Settings.Instance.Common.CharaCount, Allocator.TempJob);
            NativeArray<CharaMotion> charaMotions = m_query.ToComponentDataArray<CharaMotion>(Allocator.TempJob);
            BodyJob job = DoBodyJob(ref inputDeps, charaMatrixes, charaMotions);

            Draw(charaMatrixes, charaMotions, job);
            charaMotions.Dispose();
            charaMatrixes.Dispose();
            return inputDeps;
        }

        private static NativeArray<Matrix4x4> Draw(NativeArray<Matrix4x4> charaMatrixes, NativeArray<CharaMotion> charaMotions,
            BodyJob bodyJob)
        {
            for (int i = 0; i < charaMatrixes.Length; i++)
            {
                string imageName = Shared.m_charaMotionList.GetImageName(charaMotions[i]);
                Graphics.DrawMesh(Shared.m_charaMeshMat.m_meshDict[imageName],
                    bodyJob.m_charaMatrixes[i],
                    Shared.m_charaMeshMat.m_materialDict[imageName], 0);
            }

            return charaMatrixes;
        }

        private BodyJob DoBodyJob(ref JobHandle inputDeps,
            NativeArray<Matrix4x4> charaMatrixes,
            NativeArray<CharaMotion> charaMotions)
        {
            NativeArray<CharaMuki> charaMukis = m_query.ToComponentDataArray<CharaMuki>(Allocator.TempJob);
            NativeArray<CharaLook> charaLooks = m_query.ToComponentDataArray<CharaLook>(Allocator.TempJob);
            NativeArray<Translation> transrations = m_query.ToComponentDataArray<Translation>(Allocator.TempJob);

            var job = new BodyJob()
            {
                m_charaMatrixes = charaMatrixes,
                m_charaMukis = charaMukis,
                m_charaLooks = charaLooks,
                m_translations = transrations,
                m_charaMotions = charaMotions,
                One = Vector3.one,
                Q = m_Quaternion,
                QRev = m_QuaternionRev,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.AddDependency(inputDeps);

            charaMukis.Dispose();
            charaLooks.Dispose();
            transrations.Dispose();


            return job;
        }

        // [BurstCompileAttribute]
        struct BodyJob : IJob
        {
            public NativeArray<Matrix4x4> m_charaMatrixes;
            [ReadOnly] public NativeArray<Translation> m_translations;
            [ReadOnly] public NativeArray<CharaMuki> m_charaMukis;
            [ReadOnly] public NativeArray<CharaLook> m_charaLooks;
            [ReadOnly] public NativeArray<CharaMotion> m_charaMotions;
            [ReadOnly] public Vector3 One;
            [ReadOnly] public Quaternion Q;
            [ReadOnly] public Quaternion QRev;

            public void Execute()
            {
                for (int i = 0; i < m_charaMukis.Length; i++)
                {
                    bool isLeft = m_charaLooks[i].m_isLeft;
                    float bodyDepth = m_translations[i].Value.z;
                    float bodyX = m_translations[i].Value.x;
                    m_charaMatrixes[i] = Matrix4x4.TRS(
                        new Vector3(bodyX, m_translations[i].Value.y, bodyDepth),
                         isLeft ? QRev : Q,
                         Vector3.one);
                }
            }
        }
    }
}
