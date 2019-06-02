using Unity.Entities;
using UnityEngine;
namespace NKPB
{
	public struct ToukiMeter : IComponentData
	{
		public EnumCrossType muki;
		public int value;
		public EnumToukiMaterState state;
		public int bgScroll;
		public float textureUl;
		public float textureUr;
	}
}
