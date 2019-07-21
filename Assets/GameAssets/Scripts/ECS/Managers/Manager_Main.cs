using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
// using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

namespace NKPB
{
    sealed class Manager_Main : MonoBehaviour
    {
        const string SCENE_NAME = "Main";
        List<Entity> m_playerEntityList = new List<Entity>();

        void Start()
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.name != SCENE_NAME)
                return;
            var manager = InitializeWorld();
            ReadySharedComponentData();
            ComponentCache();
            InitializeEntities(manager);
        }

        EntityManager InitializeWorld()
        {
            World[] worlds = new World[1];
            ref World world = ref worlds[0];
            world = new World(SCENE_NAME);

            world = new World(SCENE_NAME);
            var manager = world.CreateManager<EntityManager>();

            InitializeSystem(world);
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(worlds);

            return manager;
        }

        void InitializeSystem(World world)
        {
            // 入力システム
            world.CreateManager(typeof(PadScanSystem));
            // world.CreateManager(typeof(ToukiMeterInputJobSystem));
            // world.CreateManager(typeof(ToukiMeterCountJobSystem));
            // world.CreateManager(typeof(ToukiMeterDebubSystem));
            // world.CreateManager(typeof(BGDrawSystem));

            // モーションの時間進行システム
            world.CreateManager(typeof(CountMotionSystem));
            // 時間経過によるモーション変更システム
            world.CreateManager(typeof(ShiftCountMotionSystem));
            // 入力による状態変化システム
            world.CreateManager(typeof(InputMotionSystem));
            // 入力による向き変化システム
            world.CreateManager(typeof(InputMukiSystem));
            // 入力による座標変化システム
            world.CreateManager(typeof(InputMoveSystem));
            // 座標移動システム
            world.CreateManager(typeof(MovePosSystem));
            // 描画向き変換
            world.CreateManager(typeof(LookSystem));
            // 描画座標変換システム
            world.CreateManager(typeof(ConvertDrawPosSystem));
            // Renderer
            // 各パーツの描画位置決定および描画
            world.CreateManager(typeof(CharaDrawSystem));

        }

        void ComponentCache()
        {
            Cache.pixelPerfectCamera = FindObjectOfType<PixelPerfectCamera>();
        }

        void ReadySharedComponentData()
        {
            Shared.ReadySharedComponentData();
        }

        void InitializeEntities(EntityManager manager)
        {
            CreateCharaEntity(manager);
        }

        void CreateCharaEntity(EntityManager manager)
        {
            for (int i = 0; i < Settings.Instance.Common.CharaCount; i++)
            {
                var playerEntity = (i < m_playerEntityList.Count)
                    ? m_playerEntityList[i]
                    : Entity.Null;

                var entity = CharaEntityFactory.CreateEntity(i, manager, ref Shared.charaMeshMat);
            }
        }
    }
}
