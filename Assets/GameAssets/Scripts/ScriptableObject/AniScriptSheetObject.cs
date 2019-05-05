using System;
using System.Collections.Generic;
using UnityEngine;

namespace NKPB
{
	[Serializable]
	public class AniScriptSheetObject : ScriptableObject
	{
		// 各モーションごとのアニメーション情報
		public List<AniScript> scripts;
	}
}
