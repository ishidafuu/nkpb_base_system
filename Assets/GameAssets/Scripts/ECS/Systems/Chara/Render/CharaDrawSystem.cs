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
    [UpdateAfter(typeof(CountGroup))]
    [UpdateAfter(typeof(PreLateUpdate.ParticleSystemBeginUpdateAll))]
    public class CharaDrawSystem : JobComponentSystem
    {
        ComponentGroup m_group;
        Quaternion m_quaternion;

        protected override void OnCreateManager()
        {
            m_group = GetComponentGroup(
                ComponentType.ReadOnly<CharaMuki>(),
                ComponentType.ReadOnly<CharaLook>(),
                ComponentType.ReadOnly<Position>(),
                ComponentType.ReadOnly<CharaMotion>()
            );
            m_quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_group.AddDependency(inputDeps);

            NativeArray<CharaMuki> charaMukis = m_group.ToComponentDataArray<CharaMuki>(Allocator.TempJob);
            NativeArray<CharaLook> charaLooks = m_group.ToComponentDataArray<CharaLook>(Allocator.TempJob);
            NativeArray<Position> positions = m_group.ToComponentDataArray<Position>(Allocator.TempJob);
            NativeArray<CharaMotion> charaMotions = m_group.ToComponentDataArray<CharaMotion>(Allocator.TempJob);

            NativeArray<Matrix4x4> charaMatrixes = new NativeArray<Matrix4x4>(charaMukis.Length, Allocator.TempJob);
            BodyJob bodyJob = DoBodyJob(ref inputDeps, charaMukis, charaLooks, positions, charaMotions, charaMatrixes);

            // var bgScrolls = m_group.ToComponentDataArray<BgScroll>(Allocator.TempJob);
            // var bgScrollsMatrixs = new NativeArray<Matrix4x4>(toukiMeters.Length, Allocator.TempJob);
            // BgScrollJob bgScrollJob = DoBgScrollJob(ref inputDeps, toukiMeters, bgScrollsMatrixs);

            // 描画のためCompleteする
            inputDeps.Complete();

            // JobBody(toukiMeters);
            // DrawFrame();
            // DrawToukiMeter(toukiMeterJob);
            for (int i = 0; i < charaMatrixes.Length; i++)
            {
                Graphics.DrawMesh(Shared.charaMeshMat.meshes["nm2body_01_01"],
                    bodyJob.charaMatrixes[i],
                    Shared.charaMeshMat.material, 0);
            }

            // NativeArrayの開放
            charaMukis.Dispose();
            charaLooks.Dispose();
            positions.Dispose();
            charaMotions.Dispose();
            charaMatrixes.Dispose();
            // bgScrollsMatrixs.Dispose();

            return inputDeps;
        }

        private BodyJob DoBodyJob(ref JobHandle inputDeps,
            NativeArray<CharaMuki> charaMukis,
            NativeArray<CharaLook> charaLooks,
            NativeArray<Position> positions,
            NativeArray<CharaMotion> charaMotions,
            NativeArray<Matrix4x4> charaMatrixes)
        {
            var bodyJob = new BodyJob()
            {
                charaMatrixes = charaMatrixes,
                charaMukis = charaMukis,
                charaLooks = charaLooks,
                positions = positions,
                charaMotions = charaMotions,
                one = Vector3.one,
                q = m_quaternion,
            };
            inputDeps = bodyJob.Schedule(inputDeps);
            m_group.AddDependency(inputDeps);
            return bodyJob;
        }

        [BurstCompileAttribute]
        struct BodyJob : IJob
        {
            public NativeArray<Matrix4x4> charaMatrixes;
            [ReadOnly] public NativeArray<Position> positions;
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
