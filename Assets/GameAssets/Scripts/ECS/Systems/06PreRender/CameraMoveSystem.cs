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
    [UpdateAfter(typeof(ConvertDrawTranslationSystem))]
    public class CameraMoveSystem : ComponentSystem
    {
        EntityQuery m_query;
        EntityQuery m_queryCamera;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<Translation>());

            m_queryCamera = GetEntityQuery(
                ComponentType.ReadWrite<CameraPos>());
        }

        protected override void OnUpdate()
        {
            NativeArray<CameraPos> camreaPoses = m_queryCamera.ToComponentDataArray<CameraPos>(Allocator.TempJob);
            NativeArray<Translation> translations = m_query.ToComponentDataArray<Translation>(Allocator.TempJob);

            var cameraPos = camreaPoses[0];

            int mapWidth = (int)Shared.m_mapMeshMat.m_meshDict["map000"].bounds.size.x;
            float allX = 0;
            for (int i = 0; i < translations.Length; i++)
            {
                var charaPos = translations[i];
                allX += charaPos.Value.x;
            }
            allX /= translations.Length;

            int ScreenWidthHalf = Settings.Instance.DrawPos.ScreenWidth >> 1;
            int minX = ScreenWidthHalf;
            int maxX = (int)Shared.m_mapMeshMat.m_meshDict["map000"].bounds.size.x - ScreenWidthHalf;

            cameraPos.m_position = math.clamp(allX, minX, maxX);

            Camera.main.transform.position = new Vector3(cameraPos.m_position, 0, 0);
            camreaPoses[0] = cameraPos;
            m_queryCamera.CopyFromComponentDataArray(camreaPoses);
            translations.Dispose();
            camreaPoses.Dispose();
        }
    }
}
