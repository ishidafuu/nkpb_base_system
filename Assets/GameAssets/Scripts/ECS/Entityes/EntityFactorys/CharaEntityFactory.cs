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

        /// <summary>
        /// キャラエンティティ作成
        /// </summary>
        /// <param name="i"></param>
        /// <param name="entityManager"></param>
        /// <param name="ariMeshMat"></param>
        /// <param name="aniScriptSheet"></param>
        /// <param name="aniBasePos"></param>
        /// <returns></returns>
        public static Entity CreateEntity(int _i, EntityManager _entityManager,
            ref MeshMatList _meshMatList
            // ref AniScriptSheet _aniScriptSheet,
            // ref AniBasePos _aniBasePos
        )
        {
            var archetype = _entityManager.CreateArchetype(ComponentTypes.CharaComponentType);
            var entity = _entityManager.CreateEntity(archetype);

            // 必要なキャラのみインプットをつける
            if (_i < Define.Instance.Common.PlayerNum)
            {
                _entityManager.AddComponent(entity, ComponentType.Create<PadScan>());
            }

            // ID
            _entityManager.SetComponentData(entity, new CharaId
            {
                myId = _i,
            });

            // ComponentDataのセット
            var posL = 0;
            // Define.Instance.GetMapSize() / 2;
            var posH = 0;
            // Define.Instance.GetMapSize() / 2;

            // 位置
            _entityManager.SetComponentData(entity, new Position
            {
                Value = new float3(UnityEngine.Random.Range(posL, posH), UnityEngine.Random.Range(posL, posH), 0)
            });

            // 位置
            _entityManager.SetComponentData(entity, new CharaMove
            {
                position = new Vector3Int(UnityEngine.Random.Range(posL, posH), UnityEngine.Random.Range(posL, posH), 0),
                    delta = Vector3Int.zero
            });

            // モーション
            _entityManager.SetComponentData(entity, new CharaMotion
            {

            });

            // 見た目
            _entityManager.SetComponentData(entity, new CharaLook
            {
                isLeft = 0,
                    isBack = 0
            });

            // SharedComponentDataのセット
            _entityManager.AddSharedComponentData(entity, _meshMatList);

            return entity;
        }
    }
}
