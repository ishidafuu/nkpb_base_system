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
            World.Active = world;

            InitializationSystemGroup initializationSystemGroup = world.GetOrCreateSystem<InitializationSystemGroup>();
            initializationSystemGroup.SortSystemUpdateList();

            SimulationSystemGroup simulationSystemGroup = world.GetOrCreateSystem<SimulationSystemGroup>();
            AddScanGroup(world, simulationSystemGroup);
            AddCountGroup(world, simulationSystemGroup);
            AddInputGroup(world, simulationSystemGroup);
            AddMoveGroup(world, simulationSystemGroup);
            AddMapGroup(world, simulationSystemGroup);
            AddJudgeGroup(world, simulationSystemGroup);
            AddPreRenderGroup(world, simulationSystemGroup);
            simulationSystemGroup.SortSystemUpdateList();

            PresentationSystemGroup presentationSystemGroup = world.GetOrCreateSystem<PresentationSystemGroup>();
            presentationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CharaDrawSystem>());
            presentationSystemGroup.SortSystemUpdateList();

            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);

            return world.EntityManager;
        }
        private static void AddScanGroup(World world, SimulationSystemGroup simulationSystemGroup)
        {
            ScanGroup scanGroup = world.GetOrCreateSystem<ScanGroup>();
            simulationSystemGroup.AddSystemToUpdateList(scanGroup);
            scanGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PadScanSystem>());
            scanGroup.SortSystemUpdateList();
        }

        private static void AddCountGroup(World world, SimulationSystemGroup simulationSystemGroup)
        {
            CountGroup countGroup = world.GetOrCreateSystem<CountGroup>();
            simulationSystemGroup.AddSystemToUpdateList(countGroup);
            countGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MotionCountSystem>());
            countGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CountQueueSystem>());
            countGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MoveCountSystem>());
            countGroup.SortSystemUpdateList();
        }

        private static void AddInputGroup(World world, SimulationSystemGroup simulationSystemGroup)
        {
            InputGroup inputGroup = world.GetOrCreateSystem<InputGroup>();
            simulationSystemGroup.AddSystemToUpdateList(inputGroup);
            inputGroup.AddSystemToUpdateList(world.GetOrCreateSystem<InputQueueSystem>());
            inputGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MukiInputSystem>());
            inputGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MoveInputSystem>());
            inputGroup.SortSystemUpdateList();
        }

        private static void AddMoveGroup(World world, SimulationSystemGroup simulationSystemGroup)
        {
            MoveGroup moveGroup = world.GetOrCreateSystem<MoveGroup>();
            simulationSystemGroup.AddSystemToUpdateList(moveGroup);
            moveGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PosMoveSystem>());
            moveGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MapUpdateSystem>());
            moveGroup.SortSystemUpdateList();
        }

        private static void AddMapGroup(World world, SimulationSystemGroup simulationSystemGroup)
        {
            MapGroup mapGroup = world.GetOrCreateSystem<MapGroup>();
            simulationSystemGroup.AddSystemToUpdateList(mapGroup);
            mapGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MapJudgeSystem>());
            // judgeGroup.AddSystemToUpdateList(world.CreateSystem<MapUpdateSystem>());
            mapGroup.SortSystemUpdateList();
        }

        private static void AddJudgeGroup(World world, SimulationSystemGroup simulationSystemGroup)
        {
            JudgeGroup judgeGroup = world.GetOrCreateSystem<JudgeGroup>();
            simulationSystemGroup.AddSystemToUpdateList(judgeGroup);
            // judgeGroup.AddSystemToUpdateList(world.CreateSystem<MapUpdateSystem>());
            judgeGroup.AddSystemToUpdateList(world.GetOrCreateSystem<QueueMotionSystem>());
            judgeGroup.SortSystemUpdateList();
        }

        private static void AddPreRenderGroup(World world, SimulationSystemGroup simulationSystemGroup)
        {
            PreRenderGroup preRenderGroup = world.GetOrCreateSystem<PreRenderGroup>();
            simulationSystemGroup.AddSystemToUpdateList(preRenderGroup);
            preRenderGroup.AddSystemToUpdateList(world.GetOrCreateSystem<LookSystem>());
            preRenderGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ConvertDrawTranslationSystem>());
            preRenderGroup.AddSystemToUpdateList(world.GetOrCreateSystem<LastMapUpdateSystem>());
            preRenderGroup.SortSystemUpdateList();
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
                // var playerEntity = (i < m_playerEntityList.Count)
                //     ? m_playerEntityList[i]
                //     : Entity.Null;

                var entity = CharaEntityFactory.CreateEntity(i, manager);
            }
        }
    }
}
