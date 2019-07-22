using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace NKPB
{
    [UpdateInGroup(typeof(RenderGroup))]
    public class CharaDrawSystem : JobComponentSystem
    {
        EntityQuery m_query;
        Quaternion m_quaternion;

        protected override void OnCreateManager()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<CharaMuki>(),
                ComponentType.ReadOnly<CharaLook>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<CharaMotion>()
            );
            m_quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_query.AddDependency(inputDeps);
            NativeArray<Matrix4x4> charaMatrixes = new NativeArray<Matrix4x4>(Settings.Instance.Common.CharaCount, Allocator.TempJob);
            BodyJob job = DoBodyJob(ref inputDeps, charaMatrixes);

            Draw(charaMatrixes, job);

            charaMatrixes.Dispose();
            return inputDeps;
        }

        private static NativeArray<Matrix4x4> Draw(NativeArray<Matrix4x4> charaMatrixes, BodyJob bodyJob)
        {
            for (int i = 0; i < charaMatrixes.Length; i++)
            {
                Graphics.DrawMesh(Shared.charaMeshMat.meshes["nm2body_01_01"],
                    bodyJob.charaMatrixes[i],
                    Shared.charaMeshMat.material, 0);
            }

            return charaMatrixes;
        }

        private BodyJob DoBodyJob(ref JobHandle inputDeps,
            NativeArray<Matrix4x4> charaMatrixes)
        {
            NativeArray<CharaMuki> charaMukis = m_query.ToComponentDataArray<CharaMuki>(Allocator.TempJob);
            NativeArray<CharaLook> charaLooks = m_query.ToComponentDataArray<CharaLook>(Allocator.TempJob);
            NativeArray<Translation> positions = m_query.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<CharaMotion> charaMotions = m_query.ToComponentDataArray<CharaMotion>(Allocator.TempJob);
            var job = new BodyJob()
            {
                charaMatrixes = charaMatrixes,
                charaMukis = charaMukis,
                charaLooks = charaLooks,
                positions = positions,
                charaMotions = charaMotions,
                one = Vector3.one,
                q = m_quaternion,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();

            m_query.AddDependency(inputDeps);

            charaMukis.Dispose();
            charaLooks.Dispose();
            positions.Dispose();
            charaMotions.Dispose();

            return job;
        }

        [BurstCompileAttribute]
        struct BodyJob : IJob
        {
            public NativeArray<Matrix4x4> charaMatrixes;
            [ReadOnly] public NativeArray<Translation> positions;
            [ReadOnly] public NativeArray<CharaMuki> charaMukis;
            [ReadOnly] public NativeArray<CharaLook> charaLooks;
            [ReadOnly] public NativeArray<CharaMotion> charaMotions;
            [ReadOnly] public Vector3 one;
            [ReadOnly] public Quaternion q;

            public void Execute()
            {
                for (int i = 0; i < charaMukis.Length; i++)
                {
                    bool isBack = (charaLooks[i].isBack != 0);
                    bool isLeft = (charaLooks[i].isLeft != 0);
                    float bodyDepth = positions[i].Value.z;
                    float bodyX = positions[i].Value.x;
                    charaMatrixes[i] = Matrix4x4.TRS(
                        new Vector3(bodyX, positions[i].Value.y, bodyDepth),
                        q, one);
                }
            }
        }
    }
}
