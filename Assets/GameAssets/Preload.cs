using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    static class Preload
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize() => PlayerLoopManager.RegisterDomainUnload(DomainUnloadShutdown, 10000);

        static void DomainUnloadShutdown()
        {
            World.DisposeAllWorlds();
            Shared.m_mapTipList.Dispose();
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(World.Active);
        }
    }
}