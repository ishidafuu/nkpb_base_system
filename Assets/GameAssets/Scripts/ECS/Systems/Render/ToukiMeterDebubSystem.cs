using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    [UpdateInGroup(typeof(RenderGroup))]
    public class ToukiMeterDebubSystem : ComponentSystem
    {
        ComponentGroup m_group;

        protected override void OnCreateManager()
        {
            m_group = GetComponentGroup(
                ComponentType.Create<PadScan>(),
                ComponentType.Create<ToukiMeter>()
            );
        }

        protected override void OnUpdate()
        {
            var toukiMeters = m_group.GetComponentDataArray<ToukiMeter>();
            for (int i = 0; i < toukiMeters.Length; i++)
            {
                var toukiMeter = toukiMeters[i];
                // Debug.Log(toukiMeter.value);
                // Debug.Log(toukiMeter.muki);
                // DebugPanel.Log("ToukiMeter.value", toukiMeter.value.ToString());
                DebugPanel.Log("ToukiMeter.bgScroll" + i.ToString(), toukiMeter.bgScroll.ToString());
                DebugPanel.Log("ToukiMeter.textureUl" + i.ToString(), toukiMeter.textureUl.ToString());
                DebugPanel.Log("ToukiMeter.textureUr" + i.ToString(), toukiMeter.textureUr.ToString());
                toukiMeters[i] = toukiMeter;
            }
        }

    }
}
