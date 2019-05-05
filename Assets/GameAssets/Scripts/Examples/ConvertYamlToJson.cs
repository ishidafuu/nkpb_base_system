using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YamlDotNet.Samples.Helpers;
using YamlDotNet.Serialization;

namespace YamlDotNet.Samples
{
  public class ConvertYamlToJson
  {
    private readonly ITestOutputHelper output;

    public ConvertYamlToJson(ITestOutputHelper output)
    {
      this.output = output;
    }

    [Sample(
      Title = "Convert YAML to JSON",
      Description = "Shows how to convert a YAML document to JSON."
    )]
    public void Main()
    {
      // convert string/file to YAML object
      var r = new StringReader(@"
AnimationClip:
  m_Name: Punch00
  m_RotationCurves: []
  m_CompressedRotationCurves: []
  m_EulerCurves: []
  m_PositionCurves:
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: {x: 0, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 1
        value: {x: 0, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 1.1666666
        value: {x: 0, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 2.5
        value: {x: 0, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    path: Camera
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: {x: 0, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 1.1666666
        value: {x: 26, y: -46.17, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 1.6666666
        value: {x: 26, y: -30.68, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 2.5
        value: {x: 26, y: -30.68, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    path: Camera/Body/parts01
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: {x: 60.25, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 1
        value: {x: 41.34, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 1.1666666
        value: {x: 41.34, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 2.5
        value: {x: 41.34, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    path: Camera/Body/parts00
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: {x: 0, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 2.5
        value: {x: 0, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    path: Camera/Body/stand03
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: {x: 19.25, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 1
        value: {x: 0, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 1.1666666
        value: {x: 0, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      - serializedVersion: 3
        time: 2.5
        value: {x: 0, y: 0, z: 0}
        inSlope: {x: 0, y: 0, z: 0}
        outSlope: {x: 0, y: 0, z: 0}
        tangentMode: 0
        weightedMode: 0
        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    path: Camera/Body
  m_ScaleCurves: []
  m_FloatCurves:
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 1
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 1.1666666
        value: 0
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_IsActive
    path: Camera/Body/parts00
    classID: 1
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 1.1666666
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 2.5
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_IsActive
    path: Camera/Body/parts01
    classID: 1
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 1.1666666
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 2.5
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_FlipX
    path: Camera/Body/stand03
    classID: 212
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 1.1666666
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 2.5
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_FlipX
    path: Camera/Body/parts01
    classID: 212
    script: {fileID: 0}
  m_PPtrCurves: []
  m_SampleRate: 60
  m_WrapMode: 0
  m_Bounds:
    m_Center: {x: 0, y: 0, z: 0}
    m_Extent: {x: 0, y: 0, z: 0}
  m_ClipBindingConstant:
    genericBindings:
    - serializedVersion: 2
      path: 503603439
      attribute: 1
      script: {fileID: 0}
      typeID: 4
      customType: 0
      isPPtrCurve: 0
    - serializedVersion: 2
      path: 1761824889
      attribute: 1
      script: {fileID: 0}
      typeID: 4
      customType: 0
      isPPtrCurve: 0
    - serializedVersion: 2
      path: 2386540504
      attribute: 1
      script: {fileID: 0}
      typeID: 4
      customType: 0
      isPPtrCurve: 0
    - serializedVersion: 2
      path: 1761824889
      attribute: 2086281974
      script: {fileID: 0}
      typeID: 1
      customType: 0
      isPPtrCurve: 0
    - serializedVersion: 2
      path: 503603439
      attribute: 2086281974
      script: {fileID: 0}
      typeID: 1
      customType: 0
      isPPtrCurve: 0
    - serializedVersion: 2
      path: 2473879338
      attribute: 555744692
      script: {fileID: 0}
      typeID: 212
      customType: 0
      isPPtrCurve: 0
    - serializedVersion: 2
      path: 503603439
      attribute: 555744692
      script: {fileID: 0}
      typeID: 212
      customType: 0
      isPPtrCurve: 0
    - serializedVersion: 2
      path: 1018227507
      attribute: 1
      script: {fileID: 0}
      typeID: 4
      customType: 0
      isPPtrCurve: 0
    - serializedVersion: 2
      path: 2473879338
      attribute: 1
      script: {fileID: 0}
      typeID: 4
      customType: 0
      isPPtrCurve: 0
    pptrCurveMapping: []
  m_AnimationClipSettings:
    serializedVersion: 2
    m_AdditiveReferencePoseClip: {fileID: 0}
    m_AdditiveReferencePoseTime: 0
    m_StartTime: 0
    m_StopTime: 2.5
    m_OrientationOffsetY: 0
    m_Level: 0
    m_CycleOffset: 0
    m_HasAdditiveReferencePose: 0
    m_LoopTime: 0
    m_LoopBlend: 0
    m_LoopBlendOrientation: 0
    m_LoopBlendPositionY: 0
    m_LoopBlendPositionXZ: 0
    m_KeepOriginalOrientation: 0
    m_KeepOriginalPositionY: 1
    m_KeepOriginalPositionXZ: 0
    m_HeightFromFeet: 0
    m_Mirror: 0
  m_EditorCurves:
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.x
    path: Camera
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.y
    path: Camera
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.z
    path: Camera
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: 26
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.6666666
        value: 26
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 26
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.x
    path: Camera/Body/parts01
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: -46.17
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.6666666
        value: -30.68
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: -30.68
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.y
    path: Camera/Body/parts01
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.6666666
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.z
    path: Camera/Body/parts01
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 60.25
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1
        value: 41.34
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: 41.34
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 41.34
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.x
    path: Camera/Body/parts00
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.y
    path: Camera/Body/parts00
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.z
    path: Camera/Body/parts00
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.x
    path: Camera/Body/stand03
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.y
    path: Camera/Body/stand03
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.z
    path: Camera/Body/stand03
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 19.25
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.x
    path: Camera/Body
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.y
    path: Camera/Body
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 1.1666666
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: 0
        outSlope: 0
        tangentMode: 136
        weightedMode: 0
        inWeight: 0.33333334
        outWeight: 0.33333334
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_LocalPosition.z
    path: Camera/Body
    classID: 4
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 1
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 1.1666666
        value: 0
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 2.5
        value: 0
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_IsActive
    path: Camera/Body/parts00
    classID: 1
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 1.1666666
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 2.5
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_IsActive
    path: Camera/Body/parts01
    classID: 1
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 1.1666666
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 2.5
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_FlipX
    path: Camera/Body/stand03
    classID: 212
    script: {fileID: 0}
  - curve:
      serializedVersion: 2
      m_Curve:
      - serializedVersion: 3
        time: 0
        value: 0
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 1.1666666
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      - serializedVersion: 3
        time: 2.5
        value: 1
        inSlope: Infinity
        outSlope: Infinity
        tangentMode: 103
        weightedMode: 0
        inWeight: 0
        outWeight: 0
      m_PreInfinity: 2
      m_PostInfinity: 2
      m_RotationOrder: 4
    attribute: m_FlipX
    path: Camera/Body/parts01
    classID: 212
    script: {fileID: 0}
  m_EulerEditorCurves: []
  m_HasGenericRootTransform: 0
  m_HasMotionFloatCurves: 0
  m_Events:
  - time: 0.16666667
    functionName: PlaySe
    data: 
    objectReferenceParameter: {fileID: 0}
    floatParameter: 0
    intParameter: 10
    messageOptions: 0
  - time: 0.16666667
    functionName: 
    data: 
    objectReferenceParameter: {fileID: 0}
    floatParameter: 0
    intParameter: 0
    messageOptions: 0
");
      var deserializer = new DeserializerBuilder().Build();
      var yamlObject = deserializer.Deserialize(r);

      var serializer = new SerializerBuilder()
        .JsonCompatible()
        .Build();

      var json = serializer.Serialize(yamlObject);

      output.WriteLine(json);

      NKPB.YyhsAnimation asdf = JsonUtility.FromJson<NKPB.YyhsAnimation>(json);
      Debug.Log(json);
      Debug.Log(asdf.AnimationClip.m_Name);
      Debug.Log(asdf.AnimationClip.m_PositionCurves);
      foreach (var item in asdf.AnimationClip.m_PositionCurves)
      {
        Debug.Log(item.m_Curve.Length);
        Debug.Log(item.path);
      }
      // Debug.Log(asdf.AnimationClip.m_PositionCurves.m_Curve);
      // Debug.Log(asdf.AnimationClip.m_PositionCurves.path);
      // foreach (var item in asdf.AnimationClip.m_PositionCurves.m_Curve)
      // {
      //   Debug.Log(item.);
      // }

    }
  }
}
