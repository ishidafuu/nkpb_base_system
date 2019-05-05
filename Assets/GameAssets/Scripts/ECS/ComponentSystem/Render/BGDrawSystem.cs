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
    public class BGDrawSystem : JobComponentSystem
    {
        ComponentGroup m_group;
        Quaternion m_quaternion;

        protected override void OnCreateManager()
        {
            m_group = GetComponentGroup(
                ComponentType.ReadOnly<ToukiMeter>()
                // ComponentType.ReadOnly<BgScroll>()
            );
            m_quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_group.AddDependency(inputDeps);

            var toukiMeters = m_group.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            var toukiMeterMatrixs = new NativeArray<Matrix4x4>(toukiMeters.Length, Allocator.TempJob);
            ToukiMeterJob toukiMeterJob = DoToukiMeterJob(ref inputDeps, toukiMeters, toukiMeterMatrixs);

            // var bgScrolls = m_group.ToComponentDataArray<BgScroll>(Allocator.TempJob);
            // var bgScrollsMatrixs = new NativeArray<Matrix4x4>(toukiMeters.Length, Allocator.TempJob);
            // BgScrollJob bgScrollJob = DoBgScrollJob(ref inputDeps, toukiMeters, bgScrollsMatrixs);

            // 描画のためCompleteする
            inputDeps.Complete();

            DrawBgScroll(toukiMeters);
            DrawFrame();
            DrawToukiMeter(toukiMeterJob);

            // NativeArrayの開放
            toukiMeters.Dispose();
            toukiMeterMatrixs.Dispose();
            // bgScrollsMatrixs.Dispose();

            return inputDeps;
        }

        private ToukiMeterJob DoToukiMeterJob(ref JobHandle inputDeps, NativeArray<ToukiMeter> toukiMeters, NativeArray<Matrix4x4> toukiMeterMatrixs)
        {
            var toukiMeterJob = new ToukiMeterJob()
            {
                toukiMeters = toukiMeters,
                toukiMeterMatrixs = toukiMeterMatrixs,
                q = m_quaternion,
                ToukiMeterX = Define.Instance.DrawPos.ToukiMeterX,
                ToukiMeterY = Define.Instance.DrawPos.ToukiMeterY,
            };
            inputDeps = toukiMeterJob.Schedule(inputDeps);
            m_group.AddDependency(inputDeps);
            return toukiMeterJob;
        }

        // private BgScrollJob DoBgScrollJob(ref JobHandle inputDeps, NativeArray<ToukiMeter> toukiMeters, NativeArray<Matrix4x4> bgScrollMatrixs)
        // {
        //     var bgScrollJob = new BgScrollJob()
        //     {
        //         toukiMeters = toukiMeters,
        //         bgScrollMatrixs = bgScrollMatrixs,
        //         q = m_quaternion,
        //         BgScrollWidth = Define.Instance.DrawPos.BgScrollWidth,
        //         BgScrollRangeFactor = Define.Instance.DrawPos.BgScrollRangeFactor,
        //         BgScrollX = -Define.Instance.DrawPos.BgScrollX,
        //         BgScrollY = Define.Instance.DrawPos.BgScrollY,
        //     };
        //     inputDeps = bgScrollJob.Schedule(inputDeps);
        //     m_group.AddDependency(inputDeps);
        //     return bgScrollJob;
        // }

        private void DrawBgScroll(NativeArray<ToukiMeter> toukiMeters)
        {
            for (int i = 0; i < toukiMeters.Length; i++)
            {
                Matrix4x4 bgScrollMatrixs = Matrix4x4.TRS(new Vector3(-Define.Instance.DrawPos.BgScrollX,
                        Define.Instance.DrawPos.BgScrollY, 0),
                    m_quaternion, new Vector3(0.5f, 1, 1));

                Mesh baseMesh = Shared.bgFrameMeshMat.meshs["bg00"];
                Mesh mesh = new Mesh()
                {
                    vertices = baseMesh.vertices,
                    uv = new Vector2[]
                    {
                    new Vector2(toukiMeters[i].textureUl, baseMesh.uv[0].y),
                    new Vector2(toukiMeters[i].textureUr, baseMesh.uv[1].y),
                    new Vector2(toukiMeters[i].textureUl, baseMesh.uv[2].y),
                    new Vector2(toukiMeters[i].textureUr, baseMesh.uv[3].y),
                    },
                    triangles = baseMesh.triangles,
                };
                Graphics.DrawMesh(mesh,
                    bgScrollMatrixs,
                    Shared.bgFrameMeshMat.material, 0);
            }
        }

        private void DrawFrame()
        {
            Matrix4x4 frameMatrix = Matrix4x4.TRS(new Vector3(0, 0, 0), m_quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.bgFrameMeshMat.meshs["frame"],
                frameMatrix,
                Shared.bgFrameMeshMat.material, 0);
        }

        private void DrawToukiMeter(ToukiMeterJob toukiMeterJob)
        {
            for (int i = 0; i < toukiMeterJob.toukiMeterMatrixs.Length; i++)
            {
                Graphics.DrawMesh(Shared.bgFrameMeshMat.meshs["meter02"],
                    toukiMeterJob.toukiMeterMatrixs[i],
                    Shared.bgFrameMeshMat.material, 0);
            }
        }

        [BurstCompileAttribute]
        struct ToukiMeterJob : IJob
        {
            [ReadOnly]
            public NativeArray<ToukiMeter> toukiMeters;
            [ReadOnly]
            public Quaternion q;
            [ReadOnly]
            public int ToukiMeterX;
            [ReadOnly]
            public int ToukiMeterY;
            public NativeArray<Matrix4x4> toukiMeterMatrixs;

            public void Execute()
            {
                for (int i = 0; i < toukiMeters.Length; i++)
                {
                    var width = (float)toukiMeters[i].value / 100;
                    Matrix4x4 tmpMatrix = Matrix4x4.TRS(
                        new Vector3(ToukiMeterX, ToukiMeterY, 0),
                        q, new Vector3(width, 1, 1));

                    toukiMeterMatrixs[i] = tmpMatrix;
                }
            }
        }

        // [BurstCompileAttribute]
        // struct BgScrollJob : IJob
        // {
        //     [ReadOnly]
        //     // [DeallocateOnJobCompletion]
        //     public NativeArray<ToukiMeter> toukiMeters;
        //     [ReadOnly]
        //     public Quaternion q;
        //     [ReadOnly]
        //     public int BgScrollWidth;
        //     [ReadOnly]
        //     public int BgScrollRangeFactor;
        //     [ReadOnly]
        //     public int BgScrollX;
        //     [ReadOnly]
        //     public int BgScrollY;
        //     public NativeArray<Matrix4x4> bgScrollMatrixs;

        //     public void Execute()
        //     {
        //         for (int i = 0; i < toukiMeters.Length; i++)
        //         {
        //             // var index = i * 2;
        //             var posX = BgScrollX + (toukiMeters[i].bgScroll >> BgScrollRangeFactor);
        //             var posX2 = (posX - BgScrollWidth);

        //             Matrix4x4 tmpMatrix = Matrix4x4.TRS(new Vector3(posX, BgScrollY, 0),
        //                 q, Vector3.one);

        //             bgScrollMatrixs[i] = tmpMatrix;

        //             // Matrix4x4 tmpMatrix2 = Matrix4x4.TRS(new Vector3(posX2, BgScrollY, 0),
        //             //     q, Vector3.one);

        //             // bgScrollMatrixs[index + 1] = tmpMatrix2;
        //         }
        //     }
        // }

    }
}
