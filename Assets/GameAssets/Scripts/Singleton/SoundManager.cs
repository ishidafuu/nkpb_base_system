using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{

    public class SoundManager : SingletonMonoBehaviourDontDestroy<SoundManager>
    {
        readonly string BGMPath = "Audio/BGM";
        readonly string SEPath = "Audio/SE";
        readonly int SeResourceCount = 5;

        [SerializeField, Range(0, 1)] float m_bgmVolume = 0.5f;
        [SerializeField, Range(0, 1)] float m_seVolume = 0.5f;

        AudioSource m_bgmSource;
        List<AudioSource> m_seSourceList = new List<AudioSource>();

        AudioClip[] m_bgmArray;
        AudioClip[] m_seArray;

        int m_nextSESourceIndex = 0;

        override protected void Awake()
        {
            base.Awake();

            InitAudioSource();
            LoadAudioResource();
        }

        private void LoadAudioResource()
        {
            m_bgmArray = Resources.LoadAll<AudioClip>(BGMPath);
            m_seArray = Resources.LoadAll<AudioClip>(SEPath);
        }

        private void InitAudioSource()
        {
            m_bgmSource = gameObject.AddComponent<AudioSource>();
            m_bgmSource.loop = true;

            for (int i = 0; i < SeResourceCount; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.loop = false;
                m_seSourceList.Add(source);
            }
        }

        public void PlayBgm(int index)
        {
            if (index >= m_bgmArray.Length)
            {
                Debug.LogError($"Out Of m_bgmArray {index}");
                return;
            }
            m_bgmSource.clip = m_bgmArray[index];
            m_bgmSource.volume = m_bgmVolume;
            m_bgmSource.Play();

            Debug.Log($"PlayBgm:{index}");
        }

        public void StopBgm()
        {
            m_bgmSource.Stop();
        }

        public void PlaySE(int index)
        {
            if (index >= m_seArray.Length)
            {
                Debug.LogError($"Out Of m_seArray {index}");
                return;
            }
            m_seSourceList[m_nextSESourceIndex].PlayOneShot(m_seArray[index], m_seVolume);
            m_nextSESourceIndex++;
            if (m_nextSESourceIndex >= SeResourceCount)
            {
                m_nextSESourceIndex = 0;
            }
        }

        public void StopSE()
        {
            foreach (var item in m_seSourceList)
            {
                item.Stop();
            }
        }

        public void SetBgmVolume(float value)
        {
            m_bgmVolume = Mathf.Clamp01(value);
            m_bgmSource.volume = m_bgmVolume;
        }
        public float GetBgmVolume() => m_bgmVolume;

        public void SetSEVolume(float value)
        {
            m_seVolume = Mathf.Clamp01(value);

            foreach (var item in m_seSourceList)
            {
                item.volume = m_seVolume;
            }
        }

        public float GetSEVolume() => m_seVolume;
    }

}
