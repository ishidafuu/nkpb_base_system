using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKPB
{
    [UpdateInGroup(typeof(ScanGroup))]
    // [UpdateBefore(typeof(CountMotionSystem))]
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

        EntityQuery m_query;
        ReadOnlyCollection<string>[] ButtonTypeName;
        ReadOnlyCollection<string> HorizontalName;
        ReadOnlyCollection<string> VerticalName;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<PadScan>()
            );

            var playerNum = Settings.Instance.Common.PlayerCount;
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
            var ButtonNum = Settings.Instance.Common.ButtonCount;
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
            var padScans = m_query.ToComponentDataArray<PadScan>(Allocator.TempJob);
            for (int i = 0; i < padScans.Length; i++)
            {
                var padScan = padScans[i];
                SetCross(ref padScan, i);
                SetButton(ref padScan, i);
                padScans[i] = padScan;
            }
            m_query.CopyFromComponentDataArray(padScans);
            padScans.Dispose();

        }

        void SetCross(ref PadScan padScan, int playerNo)
        {
            var nowAxis = new Vector2(Input.GetAxis(HorizontalName[playerNo]), Input.GetAxis(VerticalName[playerNo]));
            padScan.SetCross(nowAxis, Time.time);
            // if (nowAxis != Vector2.zero)
            //     Debug.Log(nowAxis);
        }

        void SetButton(ref PadScan padScan, int playerNo)
        {
            var ButtonNum = Settings.Instance.Common.ButtonCount;

            for (int i = 0; i < ButtonNum; i++)
            {
                var isPush = Input.GetButtonDown(ButtonTypeName[i][playerNo]);
                var isPress = Input.GetButton(ButtonTypeName[i][playerNo]);
                var isPop = Input.GetButtonUp(ButtonTypeName[i][playerNo]);

                switch ((EnumUnityButtonType)i)
                {
                    case EnumUnityButtonType.Fire1:
                        padScan.m_buttonA.SetButtonData(isPush, isPress, isPop, Time.time);
                        break;
                    case EnumUnityButtonType.Fire2:
                        padScan.m_buttonB.SetButtonData(isPush, isPress, isPop, Time.time);
                        break;
                    case EnumUnityButtonType.Fire3:
                        padScan.m_buttonX.SetButtonData(isPush, isPress, isPop, Time.time);
                        break;
                    case EnumUnityButtonType.Fire4:
                        padScan.m_buttonY.SetButtonData(isPush, isPress, isPop, Time.time);
                        break;
                }
                // if (isPush)
                //     Debug.Log(i);
            }
        }
    }
}
