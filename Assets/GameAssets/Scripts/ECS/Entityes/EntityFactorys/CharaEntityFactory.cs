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
        public static Entity CreateEntity(int _i, EntityManager _entityManager)
        {
            var archetype = _entityManager.CreateArchetype(ComponentTypes.CharaComponentType);
            var entity = _entityManager.CreateEntity(archetype);

            // 必要なキャラのみインプットをつける
            if (_i < Settings.Instance.Common.PlayerCount)
            {
                _entityManager.AddComponent(entity, ComponentType.ReadWrite<PadScan>());
                var padScan = new PadScan();
                padScan.Init();
                _entityManager.SetComponentData(entity, padScan);
            }

            // ID
            _entityManager.SetComponentData(entity, new CharaId
            {
                myId = _i,
            });

            var posL = 0;
            var posH = 0;

            _entityManager.SetComponentData(entity, new Translation
            {
                Value = new float3(UnityEngine.Random.Range(posL, posH), UnityEngine.Random.Range(posL, posH), 0)
            });

            _entityManager.SetComponentData(entity, new CharaDelta
            {
                m_position = new Vector3Int(UnityEngine.Random.Range(posL, posH), UnityEngine.Random.Range(posL, posH), 0),
                m_delta = Vector3Int.zero
            });

            _entityManager.SetComponentData(entity, new CharaMotion
            {
                m_motionType = EnumMotionType.Idle
            });

            _entityManager.SetComponentData(entity, new CharaLook
            {
                isLeft = 0,
                isBack = 0
            });

            _entityManager.SetComponentData(entity, new CharaFlag
            {
                inputCheckFlag = FlagInputCheck.Jump | FlagInputCheck.Dash | FlagInputCheck.Walk,
                moveFlag = FlagMove.Stop,
                motionFlag = FlagMotion.None,
                mukiFlag = true,
            });

            _entityManager.SetComponentData(entity, new CharaDash
            {
                dashMuki = EnumMuki.None
            });


            return entity;
        }
    }
}
