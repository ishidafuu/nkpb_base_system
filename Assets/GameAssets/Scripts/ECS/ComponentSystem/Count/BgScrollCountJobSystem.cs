// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;

// namespace NKPB
// {
// 	[UpdateInGroup(typeof(CountGroup))]
// 	public class BgScrollCountJobSystem : JobComponentSystem
// 	{
// 		ComponentGroup m_group;

// 		protected override void OnCreateManager()
// 		{
// 			m_group = GetComponentGroup(
// 				ComponentType.ReadOnly<ToukiMeter>(),
// 				ComponentType.Create<BgScroll>()
// 			);
// 		}

// 		protected override JobHandle OnUpdate(JobHandle inputDeps)
// 		{
// 			m_group.AddDependency(inputDeps);

// 			var toukiMeters = m_group.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
// 			var bgScrolls = m_group.ToComponentDataArray<BgScroll>(Allocator.TempJob);
// 			JobHandle jobHandle = DoCountBgScrollJob(ref inputDeps, bgScrolls);

// 			// 開放
// 			inputDeps = new ReleaseJob
// 			{
// 				toukiMeters = toukiMeters,
// 					bgScrolls = bgScrolls,
// 			}.Schedule(jobHandle);

// 			return inputDeps;
// 		}

// 		private JobHandle DoCountBgScrollJob(ref JobHandle inputDeps, NativeArray<BgScroll> bgScrolls)
// 		{
// 			inputDeps = new CountBgScrollJob()
// 			{
// 				BgScrollWidth = Define.Instance.DrawPos.BgScrollWidth,
// 					bgScrolls = bgScrolls,
// 			}.Schedule(inputDeps);
// 			m_group.AddDependency(inputDeps);
// 			m_group.CopyFromComponentDataArray(bgScrolls, out JobHandle jobHandle);
// 			return jobHandle;
// 		}

// 		struct ReleaseJob : IJob
// 		{
// 			[DeallocateOnJobCompletion]
// 			public NativeArray<BgScroll> bgScrolls;
// 			[DeallocateOnJobCompletion]
// 			public NativeArray<ToukiMeter> toukiMeters;

// 			public void Execute() {}
// 		}

// 		[BurstCompileAttribute]
// 		struct CountBgScrollJob : IJob
// 		{
// 			[ReadOnly]
// 			public NativeArray<ToukiMeter> toukiMeters;
// 			[ReadOnly]
// 			public int BgScrollWidth;
// 			public NativeArray<BgScroll> bgScrolls;

// 			public void Execute()
// 			{
// 				for (int i = 0; i < bgScrolls.Length; i++)
// 				{
// 					var bgScroll = bgScrolls[i];
// 					var toukiMeter = toukiMeters[i];

// 					switch (toukiMeter.muki)
// 					{
// 						case EnumCrossType.Left:
// 						case EnumCrossType.Down:

// 							bgScroll.value--;
// 							if (bgScroll.value < 0)
// 							{
// 								bgScroll.value = BgScrollWidth;
// 							}
// 							break;
// 						case EnumCrossType.Right:
// 							bgScroll.value++;
// 							if (bgScroll.value > BgScrollWidth)
// 							{
// 								bgScroll.value = 0;
// 							}
// 							break;
// 					}

// 					bgScrolls[i] = bgScroll;
// 				}
// 			}

// 		}

// 	}
// }
