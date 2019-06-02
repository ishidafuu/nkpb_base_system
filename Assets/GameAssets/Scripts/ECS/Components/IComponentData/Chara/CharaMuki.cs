using Unity.Entities;
using UnityEngine;
namespace NKPB
{
	/// <summary>
	/// キャラの向き
	/// </summary>
	public struct CharaMuki : IComponentData
	{
		public EnumMuki muki;
	}
}
