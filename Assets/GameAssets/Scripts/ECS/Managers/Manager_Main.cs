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

    /// <summary>
    /// ECSのセットアップを行うManager_Main Component
    /// </summary>
    sealed class Manager_Main : MonoBehaviour
    {
        // ワールドとシーン名を一致させる
        const string SCENE_NAME = "Main";

        // エンティティリスト
        List<Entity> m_playerEntityList = new List<Entity>();

        void Start()
        {
            // シーンの判定
            var scene = SceneManager.GetActiveScene();
            if (scene.name != SCENE_NAME)
                return;

            // ワールド生成
            var manager = InitializeWorld();

            // SharedComponentDataの準備
            ReadySharedComponentData();

            // コンポーネントのキャッシュ
            ComponentCache();

            // エンティティ生成
            InitializeEntities(manager);
        }

        /// <summary>
        /// ワールド生成
        /// </summary>
        /// <returns></returns>
        EntityManager InitializeWorld()
        {
            var worlds = new World[1];
            ref
            var world = ref worlds[0];

            world = new World(SCENE_NAME);
            var manager = world.CreateManager<EntityManager>();

            // ComponentSystemの初期化
            InitializeSystem(world);

            // PlayerLoopへのWorldの登録
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(worlds);

            return manager;
        }

        /// <summary>
        /// ComponentSystemの初期化
        /// </summary>
        /// <param name="world"></param>
        void InitializeSystem(World world)
        {
            // Systemは World.Active.GetOrCreateManager<EntityManager>() で明示的に作成できます。
            // World.Active.CreateManager<EntityManager>() もありますが、 
            // 既にEntityが存在すると問題になるので、 個人的には「 明示的な作成」 のような
            // 生成タイミングがフワっとしてるものにはオススメしません。（ Worldの初期化時に作るんだ!!!等ならCreateManagerでも）
            // world.CreateManager(typeof(MoveSystem));
            // world.CreateManager(typeof(EndFrameBarrier));
            // world.CreateManager(typeof(DanceSystem));
            // world.CreateManager(typeof(EndFrameTransformSystem));
            // world.CreateManager<MeshInstanceRendererSystem>().ActiveCamera = GetComponent<Camera>();

            // Player
            // Chara

            // 入力システム
            world.CreateManager(typeof(PadScanSystem));
            world.CreateManager(typeof(ToukiMeterInputJobSystem));
            world.CreateManager(typeof(ToukiMeterCountJobSystem));
            // world.CreateManager(typeof(BgScrollCountJobSystem));
            world.CreateManager(typeof(ToukiMeterDebubSystem));
            world.CreateManager(typeof(BGDrawSystem));

            // // モーションの時間進行システム
            // world.CreateManager(typeof(CountMotionJobSystem));
            // // 時間経過によるモーション変更システム
            // world.CreateManager(typeof(ShiftCountMotionJobSystem));
            // // 入力による状態変化システム
            // world.CreateManager(typeof(InputMotionJobSystem));
            // // 入力による向き変化システム
            // world.CreateManager(typeof(InputMukiSystem));
            // // 入力による座標変化システム
            // world.CreateManager(typeof(InputMoveSystem));
            // // 座標移動システム
            // world.CreateManager(typeof(MovePosJobSystem));
            // // 描画向き変換
            // world.CreateManager(typeof(LookJobSystem));
            // // 描画座標変換システム
            // world.CreateManager(typeof(ConvertDrawPosJobSystem));
            // // Renderer
            // // 各パーツの描画位置決定および描画
            // world.CreateManager(typeof(CharaDrawSystem));

        }

        // 各コンポーネントのキャッシュ
        void ComponentCache()
        {
            Cache.pixelPerfectCamera = FindObjectOfType<PixelPerfectCamera>();
            // var tileMaps = FindObjectsOfType<Tilemap>();
            // foreach (var item in tileMaps)
            // {
            //     // Debug.Log(item.layoutGrid.name);
            //     if (item.layoutGrid.name == "PheromGrid")
            //     {
            //         Cache.pheromMap = item;
            //         Cache.pheromMap.ClearAllTiles();
            //         Cache.pheromMap.size = new Vector3Int(Define.Instance.GRID_SIZE, Define.Instance.GRID_SIZE, 0);
            //     }
            // }
        }

        // SharedComponentDataの読み込み
        void ReadySharedComponentData()
        {
            Shared.ReadySharedComponentData();
        }

        /// <summary>
        /// エンティティ生成
        /// </summary>
        /// <param name="manager"></param>
        void InitializeEntities(EntityManager manager)
        {
            // プレーヤー作成
            // CreatePlayerEntity(manager);
            // キャラ作成
            CreateCharaEntity(manager);
        }

        /// <summary>
        /// プレーヤーエンティティ作成
        /// </summary>
        /// <param name="manager"></param>
        // void CreatePlayerEntity(EntityManager manager)
        // {
        //     for (int i = 0; i < Define.Instance.PLAYER_NUM; i++)
        //     {
        //         var entity = PlayerEntityFactory.CreateEntity(i, manager);
        //         // m_playerEntityList.Add(entity);
        //     }
        // }

        /// <summary>
        /// キャラエンティティ作成
        /// </summary>
        /// <param name="manager"></param>
        void CreateCharaEntity(EntityManager manager)
        {
            for (int i = 0; i < Define.Instance.Common.CharaNum; i++)
            {
                var playerEntity = (i < m_playerEntityList.Count)
                    ? m_playerEntityList[i]
                    : Entity.Null;

                var entity = CharaEntityFactory.CreateEntity(i, manager, ref Shared.charaMeshMat, ref Shared.aniScriptSheet, ref Shared.aniBasePos);
            }
        }

        // void InitializeEntity(ref Unity.Mathematics.Random random, EntityManager manager, Entity entity)
        // {
        //     var moves = manager.GetBuffer<DanceMove>(entity);
        //     DanceMove move = default;
        //     for (uint i = 0; i < danceLoopLength; ++i)
        //     {
        //         var values = random.NextFloat4() * 10f - 5f;
        //         move.Duration = values.w + 5f;
        //         move.Velocity = values.xyz;
        //         random.state = random.NextUInt();
        //         moves.Add(move);
        //     }
        //     manager.SetComponentData(entity, new Velocity { Value = (random.NextFloat3() - 0.5f) * 8f });
        // }
    }
}
