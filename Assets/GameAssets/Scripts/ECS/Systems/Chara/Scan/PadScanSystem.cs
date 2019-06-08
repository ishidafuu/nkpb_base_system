using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HedgehogTeam.EasyTouch;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    /// <summary>
    /// 入力システム（Input.GetButtonDownはメインスレッドからのみ呼び出せるのでComponentSystemで呼び出す）
    /// </summary>
    [UpdateInGroup(typeof(ScanGroup))]
    [UpdateBefore(typeof(CountGroup))]
    public class PadScanSystem : ComponentSystem
    {
        enum EnumUnityButtonType
        {
            Fire1,
            Fire2,
            Fire3,
            Fire4,
            Fire5,
            Fire6,
        }

        ComponentGroup m_group;
        ReadOnlyCollection<string>[] ButtonTypeName;
        ReadOnlyCollection<string> HorizontalName;
        ReadOnlyCollection<string> VerticalName;

        protected override void OnCreateManager()
        {
            m_group = GetComponentGroup(
                ComponentType.Create<PadScan>());

            var playerNum = Define.Instance.Common.PlayerNum;
            var tmpPlayerNames = new List<string>();
            for (int i = 0; i < playerNum; i++)
            {
                tmpPlayerNames.Add($"P{i}");
            }

            InitButtonTypeName(tmpPlayerNames);
            InitHorizontalName(tmpPlayerNames);
            InitVerticalName(tmpPlayerNames);
        }

        private void InitButtonTypeName(List<string> tpmPlayerNames)
        {
            var ButtonNum = Define.Instance.Common.ButtonNum;
            ButtonTypeName = new ReadOnlyCollection<string>[ButtonNum];

            var buttonNames = new List<string>();
            for (int i = 0; i < ButtonNum; i++)
            {
                buttonNames.Add($"Fire{i + 1}");
            }

            for (int i = 0; i < ButtonNum; i++)
            {
                var playerButtonNames = new List<string>();
                foreach (var item in tpmPlayerNames)
                {
                    playerButtonNames.Add($"{item}{buttonNames[i]}");
                }
                ButtonTypeName[i] = Array.AsReadOnly(playerButtonNames.ToArray());
            }
        }

        private void InitHorizontalName(List<string> tpmPlayerNames)
        {
            var tmpHorizontalNames = new List<string>();
            foreach (var item in tpmPlayerNames)
            {
                tmpHorizontalNames.Add($"{item}Horizontal");
            }
            HorizontalName = Array.AsReadOnly(tmpHorizontalNames.ToArray());
        }

        private void InitVerticalName(List<string> tpmPlayerNames)
        {
            var tmpVerticalNames = new List<string>();
            foreach (var item in tpmPlayerNames)
            {
                tmpVerticalNames.Add($"{item}Vertical");
            }
            VerticalName = Array.AsReadOnly(tmpVerticalNames.ToArray());
        }

        protected override void OnUpdate()
        {
            var PadScans = m_group.ToComponentDataArray<PadScan>(Allocator.TempJob);
            for (int i = 0; i < PadScans.Length; i++)
            {
                var PadScan = PadScans[i];
                SetCross(ref PadScan, i);
                SetButton(ref PadScan, i);
                PadScans[i] = PadScan;
            }
            m_group.CopyFromComponentDataArray(PadScans);
            PadScans.Dispose();

        }

        void SetCross(ref PadScan PadScan, int playerNo)
        {
            var nowAxis = new Vector2(Input.GetAxis(HorizontalName[playerNo]), Input.GetAxis(VerticalName[playerNo]));
            PadScan.SetCross(nowAxis, Time.time);
            // if (nowAxis != Vector2.zero)
            // 	Debug.Log(nowAxis);
        }

        void SetButton(ref PadScan padScan, int playerNo)
        {
            var ButtonNum = Define.Instance.Common.ButtonNum;

            for (int i = 0; i < ButtonNum; i++)
            {
                var isPush = Input.GetButtonDown(ButtonTypeName[i][playerNo]);
                var isPress = Input.GetButton(ButtonTypeName[i][playerNo]);
                var isPop = Input.GetButtonUp(ButtonTypeName[i][playerNo]);

                switch ((EnumUnityButtonType)i)
                {
                    case EnumUnityButtonType.Fire1:
                        padScan.buttonA.SetButtonData(isPush, isPress, isPop, Time.time);
                        break;
                    case EnumUnityButtonType.Fire2:
                        padScan.buttonB.SetButtonData(isPush, isPress, isPop, Time.time);
                        break;
                    case EnumUnityButtonType.Fire3:
                        padScan.buttonX.SetButtonData(isPush, isPress, isPop, Time.time);
                        break;
                    case EnumUnityButtonType.Fire4:
                        padScan.buttonY.SetButtonData(isPush, isPress, isPop, Time.time);
                        break;
                }
                // if (isPush)
                // 	Debug.Log(buttonName);
            }
        }
    }
}
