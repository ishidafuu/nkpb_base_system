using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace NKPB
{
	[ExecuteInEditMode]
	public class SpriteSetter : MonoBehaviour
	{

		public int CharaNo;
		public int BGNo;

		public GameObject Character;
		public GameObject BackGround;
		public GameObject Effect;
		public GameObject SpriteRC;

		public void InitObject()
		{
			FindObject();
			InitPosition();
			DeleteOldObject(Character);
			DeleteOldObject(BackGround);
			DeleteOldObject(Effect);
			CreateNewCharaObject();
			CreateNewBGObject();
			CreateNewEffectObject(GetCharaPath());
			CreateNewEffectObject(GetBackGroundPath());
			LoadObject();
		}

		private void FindObject()
		{
			Character = GameObject.Find("Character");
			BackGround = GameObject.Find("BackGround");
			Effect = GameObject.Find("Effect");
			SpriteRC = GameObject.Find("SpriteRC");
		}

		private void InitPosition()
		{
			Vector3 BASE_POS = new Vector3(0, 48, 0);
			Character.transform.localPosition = BASE_POS;
			BackGround.transform.localPosition = BASE_POS;
			Effect.transform.localPosition = BASE_POS;
		}

		private void CreateNewBGObject()
		{
			// 生成
			string path = GetBackGroundPath();
			UnityEngine.Object[] spriteList = Resources.LoadAll(path, typeof(Sprite));
			foreach (var item in spriteList)
			{
				if (!CheckBG(item.name))
					continue;

				if (CheckEffect(item.name))
					continue;

				var newSprite = Instantiate(SpriteRC);
				newSprite.transform.SetParent(BackGround.transform);
				newSprite.name = item.name;
			}
		}

		private void CreateNewEffectObject(string path)
		{
			UnityEngine.Object[] spriteList = Resources.LoadAll(path, typeof(Sprite));
			foreach (var item in spriteList)
			{
				if (!CheckEffect(item.name))
					continue;

				var newSprite = Instantiate(SpriteRC);
				newSprite.transform.SetParent(Effect.transform);
				newSprite.name = item.name;
			}
		}

		private void CreateNewCharaObject()
		{
			// 生成
			string path = GetCharaPath();
			UnityEngine.Object[] spriteList = Resources.LoadAll(path, typeof(Sprite));
			foreach (var item in spriteList)
			{
				if (CheckEffect(item.name))
					continue;

				var newSprite = Instantiate(SpriteRC);
				newSprite.transform.SetParent(Character.transform);
				newSprite.name = item.name;
			}
		}

		private void DeleteOldObject(GameObject baseObject)
		{
			// 削除
			var transforms = baseObject.GetComponentsInChildren<Transform>();
			foreach (var item in transforms)
			{
				if (item == baseObject.transform)
					continue;
				DestroyImmediate(item.gameObject);
			}
		}

		public bool CheckEffect(string itemName)
		{
			return (itemName.IndexOf("effect") >= 0);
		}

		public bool CheckBG(string itemName)
		{
			return (itemName.IndexOf("bg") >= 0);
		}

		public void LoadObject()
		{
			LoadSprite(GetCharaPath());
			LoadSprite(GetBackGroundPath());
		}

		private void LoadSprite(string path)
		{
			UnityEngine.Object[] list = Resources.LoadAll(path, typeof(Sprite));

			// listを回してDictionaryに格納
			for (var i = 0; i < list.Length; ++i)
			{
				Debug.Log(list[i].name);
				var sprite = list[i] as Sprite;

				var targetObj = GameObject.Find(sprite.name);
				if (targetObj == null)
				{
					// Debug.LogError(sprite.name + "GameObject Not Found");
					continue;
				}

				var targetSpriteRenderer = targetObj.GetComponent<SpriteRenderer>();

				if (targetSpriteRenderer == null)
				{
					Debug.LogError(sprite.name + "SpriteRenderer Not Found");
					continue;
				}

				targetSpriteRenderer.sprite = sprite;

				if (CheckBG(targetSpriteRenderer.name))
				{
					targetSpriteRenderer.sprite = sprite;
					targetSpriteRenderer.sortingOrder = -20;
					targetSpriteRenderer.transform.localPosition = Vector3.zero;
				}
				else
				{
					if (targetSpriteRenderer.name.IndexOf("a_") >= 0)
					{
						targetSpriteRenderer.sortingOrder = +10;
					}
					else if (targetSpriteRenderer.name.IndexOf("b_") >= 0)
					{
						targetSpriteRenderer.sortingOrder = -10;
					}
				}
				targetSpriteRenderer.transform.localPosition = Vector3.zero;
			}
		}

		private string GetCharaPath()
		{
			return "Sprites/Character/chara" + CharaNo.ToString("d2");
		}

		private string GetBackGroundPath()
		{
			return "Sprites/BackGround/bg" + BGNo.ToString("d2");
		}

		// private string GetEffectPath()
		// {
		// 	return "Sprites/BackGround/effect";
		// }
	}

	[CustomEditor(typeof(SpriteSetter))] // 拡張するクラスを指定
	public class SpriteSetterEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// 元のInspector部分を表示
			base.OnInspectorGUI();

			// ボタンを表示

			if (GUILayout.Button("InitObject"))
				(target as SpriteSetter).InitObject();

			if (GUILayout.Button("LoadObject"))
				(target as SpriteSetter).LoadObject();

		}

	}
}
