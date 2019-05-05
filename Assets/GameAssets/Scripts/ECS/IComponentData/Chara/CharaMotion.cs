using Unity.Entities;
using UnityEngine;

namespace NKPB
{
	/// <summary>
	/// キャラのモーション
	/// </summary>
	public struct CharaMotion : IComponentData
	{
		public EnumMotion motionType;
		public int count;
		public int totalCount;

		public EnumFlagMotion motionFlags;

		/// <summary>
		/// モーションセット
		/// </summary>
		/// <param name="_motionType"></param>
		public void SetMotion(EnumMotion _motionType)
		{
			motionType = _motionType;
			count = 0;
			totalCount = 0;
			var isLastDash = HasFlag(EnumFlagMotion.Dash);

			//モーションごとのフラグ
			switch (_motionType)
			{
				case EnumMotion.Idle:
				case EnumMotion.Walk:
				case EnumMotion.Land:
					motionFlags = EnumFlagMotion.None;
					break;
				case EnumMotion.Dash:
				case EnumMotion.Slip:
					motionFlags = EnumFlagMotion.Dash;
					break;
				case EnumMotion.Jump:
					motionFlags = EnumFlagMotion.Air;
					if (isLastDash)
						motionFlags |= EnumFlagMotion.Dash;
					break;
				case EnumMotion.Fall:
					motionFlags = EnumFlagMotion.Air;
					break;
				case EnumMotion.Damage:
				case EnumMotion.Down:
					motionFlags = EnumFlagMotion.Damage;
					break;
				case EnumMotion.Fly:
					motionFlags = EnumFlagMotion.Damage | EnumFlagMotion.Air;
					break;
				case EnumMotion.Dead:
					motionFlags = EnumFlagMotion.None;
					break;
				case EnumMotion.Action:
					motionFlags = EnumFlagMotion.None;
					if (isLastDash)
						motionFlags |= EnumFlagMotion.Dash;
					break;
				default:
					Debug.Assert(false);
					break;
			}

		}

		/// <summary>
		/// フラグ保持チェック
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		public bool HasFlag(EnumFlagMotion flag)
		{
			//motionFlags.HasFlagはバーストできない
			return (motionFlags & flag) != 0;
		}

		/// <summary>
		/// フラグON
		/// </summary>
		/// <param name="flag"></param>
		public void AddFlag(EnumFlagMotion flag)
		{
			motionFlags |= flag;
		}

		/// <summary>
		/// フラグOFF
		/// </summary>
		/// <param name="flag"></param>
		public void SubFlag(EnumFlagMotion flag)
		{
			motionFlags &= ~flag;
		}
	}
}
