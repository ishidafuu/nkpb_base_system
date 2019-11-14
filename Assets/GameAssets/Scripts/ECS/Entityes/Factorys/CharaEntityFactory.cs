using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
// using Unity.Transforms2D;
using Unity.Collections;
// using toinfiniityandbeyond.Rendering2D;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEditor;
using UnityEngine.SceneManagement;
namespace NKPB
{
    public static class CharaEntityFactory
    {
        public static Entity CreateEntity(int i, EntityManager entityManager)
        {
            var archetype = entityManager.CreateArchetype(ComponentTypes.CharaComponentType);
            var entity = entityManager.CreateEntity(archetype);

            // 必要なキャラのみインプットをつける
            if (i < Settings.Instance.Common.PlayerCount)
            {
                entityManager.AddComponent(entity, ComponentType.ReadWrite<PadScan>());
                var padScan = new PadScan();
                padScan.Init();
                entityManager.SetComponentData(entity, padScan);
            }

            // ID
            entityManager.SetComponentData(entity, new CharaId
            {
                m_myId = i,
            });

            var posL = 0;
            var posH = 0;

            entityManager.SetComponentData(entity, new Translation
            {
                Value = new float3(UnityEngine.Random.Range(posL, posH), UnityEngine.Random.Range(posL, posH), 0)
            });

            var charaPos = new CharaPos();
            charaPos.SetPosition(new Vector3Int(UnityEngine.Random.Range(posL, posH), UnityEngine.Random.Range(posL, posH), 0));
            entityManager.SetComponentData(entity, charaPos);

            entityManager.SetComponentData(entity, new CharaLastPos
            {

            });

            entityManager.SetComponentData(entity, new CharaDelta
            {
                m_delta = Vector3Int.zero
            });

            entityManager.SetComponentData(entity, new CharaMotion
            {
                m_motionType = EnumMotionType.Idle
            });

            entityManager.SetComponentData(entity, new CharaLook
            {
                m_isLeft = false,
            });

            entityManager.SetComponentData(entity, new CharaFlag
            {
                m_inputCheckFlag = FlagInputCheck.Jump | FlagInputCheck.Dash | FlagInputCheck.Walk,
                m_moveFlag = FlagMove.Stop,
                m_motionFlag = FlagMotion.None,
                m_mukiFlag = true,
            });

            entityManager.SetComponentData(entity, new CharaDash
            {
                m_dashMuki = EnumMuki.None
            });

            entityManager.SetComponentData(entity, new CharaMuki
            {
                m_muki = EnumMuki.Right
            });

            entityManager.SetComponentData(entity, new CharaPos
            {

            });


            return entity;
        }
    }
}
