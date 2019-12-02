using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    [UpdateInGroup(typeof(RenderGroup))]
    // [AlwaysUpdateSystem]
    public class MapDrawSystem : ComponentSystem
    {
        EntityQuery m_query;
        Quaternion m_Quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        Quaternion m_QuaternionRev = Quaternion.Euler(new Vector3(-90, 180, 0));

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<CameraPos>()
            );
        }

        protected override void OnUpdate()
        {
            DrawFrame();

        }

        private void DrawFrame()
        {
            Matrix4x4 frameTopMatrix = Matrix4x4.TRS(new Vector3(
                Shared.m_mapMeshMat.m_sizeXHalf,
                Shared.m_mapMeshMat.m_sizeYHalf,
                (int)800), m_Quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.m_mapMeshMat.m_mesh,
                frameTopMatrix,
                Shared.m_mapMeshMat.m_material, 0);
        }
    }
}
