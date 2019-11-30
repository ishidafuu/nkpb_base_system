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
    public static class CameraEntityFactory
    {
        public static Entity CreateEntity(EntityManager entityManager)
        {
            var archetype = entityManager.CreateArchetype(ComponentTypes.CameraComponentType);
            var entity = entityManager.CreateEntity(archetype);

            entityManager.SetComponentData(entity, new CameraPos
            {

            });

            return entity;
        }
    }
}
