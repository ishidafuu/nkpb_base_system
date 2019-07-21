using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NMSpriteLoader : SingletonMonoBehaviour<NMSpriteLoader>
    {
        Dictionary<string, Sprite> m_dic = new Dictionary<string, Sprite>();
        List<string> m_loadPath = new List<string>();

        override protected void Awake()
        {
            base.Awake();
            Load("zura");
            Load("kao");
            Load("iconChest");
            Load("TPSprites/body");
        }

        public int Load(string path)
        {
            if (m_loadPath.Contains(path))
            {
                Debug.Log("NotFound : " + path);
                return 0;
            }

            m_loadPath.Add(path);
            Object[] list = Resources.LoadAll(path, typeof(Sprite));

            if (list == null || list.Length == 0)
            {
                Debug.Log("list null or 0");
                return -1;
            }

            int i, len = list.Length;
            for (i = 0; i < len; ++i)
            {
                m_dic.Add(list[i].name, list[i] as Sprite);
            }

            return len;
        }

        public Sprite GetSprite(string name)
        {
            if (!m_dic.ContainsKey(name))
                return null;

            return m_dic[name];
        }

        public void Dispose()
        {
            m_dic.Clear();
            m_dic = null;
        }
    }
