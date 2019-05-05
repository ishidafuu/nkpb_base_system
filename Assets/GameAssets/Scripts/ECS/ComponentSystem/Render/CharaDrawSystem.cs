// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Transforms;
// using UnityEngine;
// using UnityEngine.Experimental.PlayerLoop;

// namespace NKPB
// {

//     // 各パーツの描画位置決定および描画
//     [UpdateAfter(typeof(PreLateUpdate.ParticleSystemBeginUpdateAll))]
//     public class CharaDrawSystem : JobComponentSystem
//     {
//         ComponentGroup group;

//         protected override void OnCreateManager()
//         {
//             group = GetComponentGroup(
//                 ComponentType.ReadOnly<Position>(),
//                 ComponentType.ReadOnly<CharaMuki>(),
//                 ComponentType.ReadOnly<CharaLook>(),
//                 ComponentType.ReadOnly<CharaMotion>()
//             );
//         }

//         protected override JobHandle OnUpdate(JobHandle inputDeps)
//         {
//             var positions = group.GetComponentDataArray<Position>();
//             var charaMukis = group.GetComponentDataArray<CharaMuki>();
//             var charaLooks = group.GetComponentDataArray<CharaLook>();
//             var charaMotions = group.GetComponentDataArray<CharaMotion>();
//             var length = charaMotions.Length;

//             var isInCamera = new NativeArray<int>(length, Allocator.TempJob);
//             var lrQuaternion = new NativeArray<Quaternion>(2, Allocator.TempJob);
//             var position = new NativeArray<Position>(length, Allocator.TempJob);
//             var look = new NativeArray<CharaLook>(length, Allocator.TempJob);
//             var motion = new NativeArray<CharaMotion>(length, Allocator.TempJob);
//             var frame = new NativeArray<AniFrame>(length, Allocator.TempJob);

//             lrQuaternion[0] = Quaternion.Euler(new Vector3(-90, 0, 180));
//             lrQuaternion[1] = Quaternion.Euler(new Vector3(-90, 0, 0));
//             positions.CopyTo(position);
//             charaLooks.CopyTo(look);
//             charaMotions.CopyTo(motion);

//             for (int i = 0; i < length; i++)
//             {
//                 frame[i] = Shared.aniScriptSheet.scripts[(int)motion[i].motionType].frames[motion[i].count >> 2];
//             }

//             float cameraW = Cache.pixelPerfectCamera.refResolutionX >> 1;
//             float cameraH = (cameraW * 0.75f);
//             var jobCulling = new JobCulling()
//             {
//                 isInCamera = isInCamera,
//                 position = position,
//                 cameraXMax = Camera.main.transform.position.x + cameraW,
//                 cameraXMin = Camera.main.transform.position.x - cameraW,
//                 cameraYMax = Camera.main.transform.position.y + cameraH,
//                 cameraYMin = Camera.main.transform.position.y - cameraH,
//                 entitiesLength = length,
//             };

//             inputDeps = jobCulling.Schedule(length, 64, inputDeps);
//             // inputDeps.Complete();

//             var jobBody = new JobBody()
//             {
//                 bodyMatrix = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 leftArmMatrix = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 rightArmMatrix = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 leftHandMatrix = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 rightHandMatrix = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 leftLegMatrix = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 rightLegMatrix = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 leftFootMatrix = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 rightFootMatrix = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 isInCamera = isInCamera,
//                 matrixLength = 0,
//                 position = position,
//                 look = look,
//                 motion = motion,
//                 aniBasePos = Shared.aniBasePos,
//                 frame = frame,
//                 lrQuaternion = lrQuaternion,
//                 entitiesLength = length,
//                 one = Vector3.one,
//             };
//             // var jobHandle = job.Schedule(length, 50);
//             inputDeps = jobBody.Schedule(inputDeps);

//             const int HEADNUM = 2;
//             var jobAntHead = new JobAntHead()
//             {
//                 antMatrix = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 antMatrix2 = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 headMatrix = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 headMatrix2 = new NativeArray<Matrix4x4>(length, Allocator.TempJob),
//                 matrixLength = new NativeArray<int>(HEADNUM, Allocator.TempJob),
//                 isInCamera = isInCamera,
//                 position = position,
//                 look = look,
//                 motion = motion,
//                 aniBasePos = Shared.aniBasePos,
//                 frame = frame,
//                 lrQuaternion = lrQuaternion,
//                 entitiesLength = length,
//                 one = Vector3.one,
//             };
//             inputDeps = jobAntHead.Schedule(inputDeps);
//             inputDeps.Complete();
//             // jobBodyHandle.Complete();
//             // jobAntHeadHandle.Complete();

//             // DrawMeshInstancedだとZソートがかからない（最初に描画されたやつに引っ張られてる？）
//             // 体
//             for (int i = 0; i < jobBody.bodyMatrix.Length; i++)
//             {
//                 var framesCount = Shared.charaMeshMat;

//                 Graphics.DrawMesh(Shared.charaMeshMat.meshs[0], jobBody.bodyMatrix[i],
//                     Shared.charaMeshMat.materials[0], 0);
//                 Graphics.DrawMesh(Shared.charaMeshMat.meshs[1], jobBody.leftArmMatrix[i],
//                     Shared.charaMeshMat.materials[1], 0);
//                 Graphics.DrawMesh(Shared.charaMeshMat.meshs[1], jobBody.rightArmMatrix[i],
//                     Shared.charaMeshMat.materials[1], 0);
//                 Graphics.DrawMesh(Shared.charaMeshMat.meshs[1], jobBody.leftLegMatrix[i],
//                     Shared.charaMeshMat.materials[1], 0);
//                 Graphics.DrawMesh(Shared.charaMeshMat.meshs[1], jobBody.rightLegMatrix[i],
//                     Shared.charaMeshMat.materials[1], 0);

//                 Graphics.DrawMesh(Shared.charaMeshMat.meshs[2], jobBody.leftHandMatrix[i],
//                     Shared.charaMeshMat.materials[2], 0);
//                 Graphics.DrawMesh(Shared.charaMeshMat.meshs[2], jobBody.rightHandMatrix[i],
//                     Shared.charaMeshMat.materials[2], 0);
//                 Graphics.DrawMesh(Shared.charaMeshMat.meshs[2], jobBody.leftFootMatrix[i],
//                     Shared.charaMeshMat.materials[2], 0);
//                 Graphics.DrawMesh(Shared.charaMeshMat.meshs[2], jobBody.rightFootMatrix[i],
//                     Shared.charaMeshMat.materials[2], 0);

//                 // Graphics.DrawMesh(Shared.ariMeshMat.meshs[3], jobBody.gasterMatrix[i],
//                 // 	Shared.ariMeshMat.materials[3], 0);
//             }

//             // 頭
//             for (int i = 0; i < jobAntHead.matrixLength[0]; i++)
//             {
//                 // Graphics.DrawMesh(Shared.ariMeshMat.meshs[4], jobAntHead.antMatrix[i],
//                 // 	Shared.ariMeshMat.materials[4], 0);
//                 Graphics.DrawMesh(Shared.charaMeshMat.meshs[5], jobAntHead.headMatrix[i],
//                     Shared.charaMeshMat.materials[5], 0);
//             }
//             for (int i = 0; i < jobAntHead.matrixLength[1]; i++)
//             {
//                 // Graphics.DrawMesh(Shared.ariMeshMat.meshs[6], jobAntHead.antMatrix2[i],
//                 // 	Shared.ariMeshMat.materials[6], 0);
//                 Graphics.DrawMesh(Shared.charaMeshMat.meshs[7], jobAntHead.headMatrix2[i],
//                     Shared.charaMeshMat.materials[7], 0);
//             }

//             // NativeArrayの開放

//             jobBody.bodyMatrix.Dispose();
//             jobBody.leftArmMatrix.Dispose();
//             jobBody.rightArmMatrix.Dispose();
//             jobBody.leftHandMatrix.Dispose();
//             jobBody.rightHandMatrix.Dispose();
//             jobBody.leftLegMatrix.Dispose();
//             jobBody.rightLegMatrix.Dispose();
//             jobBody.leftFootMatrix.Dispose();
//             jobBody.rightFootMatrix.Dispose();

//             jobAntHead.antMatrix.Dispose();
//             jobAntHead.antMatrix2.Dispose();
//             jobAntHead.headMatrix.Dispose();
//             jobAntHead.headMatrix2.Dispose();
//             jobAntHead.matrixLength.Dispose();

//             lrQuaternion.Dispose();
//             position.Dispose();
//             look.Dispose();
//             motion.Dispose();
//             frame.Dispose();
//             isInCamera.Dispose();

//             return inputDeps;
//         }

//         // カリング
//         [BurstCompileAttribute]
//         struct JobCulling : IJobParallelFor
//         {
//             public NativeArray<int> isInCamera;
//             [ReadOnly] public float cameraXMin;
//             [ReadOnly] public float cameraXMax;
//             [ReadOnly] public float cameraYMin;
//             [ReadOnly] public float cameraYMax;
//             [ReadOnly] public NativeArray<Position> position;
//             [ReadOnly] public int entitiesLength;
//             public void Execute(int i)
//             {

//                 // isInCamera[i] = 1;
//                 if (cameraXMax < position[i].Value.x)
//                 {
//                     isInCamera[i] = 0;
//                 }
//                 else if (cameraXMin > position[i].Value.x)
//                 {
//                     isInCamera[i] = 0;
//                 }
//                 else if (cameraYMax < position[i].Value.y)
//                 {
//                     isInCamera[i] = 0;
//                 }
//                 else if (cameraYMin > position[i].Value.y)
//                 {
//                     isInCamera[i] = 0;
//                 }
//                 else
//                 {
//                     isInCamera[i] = 1;
//                 }
//             }
//         }

//         // 胸手足位置
//         [BurstCompileAttribute]
//         struct JobBody : IJob
//         {
//             public NativeArray<Matrix4x4> bodyMatrix;
//             public NativeArray<Matrix4x4> leftArmMatrix;
//             public NativeArray<Matrix4x4> rightArmMatrix;
//             public NativeArray<Matrix4x4> leftHandMatrix;
//             public NativeArray<Matrix4x4> rightHandMatrix;
//             public NativeArray<Matrix4x4> leftLegMatrix;
//             public NativeArray<Matrix4x4> rightLegMatrix;
//             public NativeArray<Matrix4x4> leftFootMatrix;
//             public NativeArray<Matrix4x4> rightFootMatrix;
//             public int matrixLength;
//             [ReadOnly] public NativeArray<int> isInCamera;
//             [ReadOnly] public NativeArray<Position> position;
//             [ReadOnly] public NativeArray<CharaLook> look;
//             [ReadOnly] public NativeArray<CharaMotion> motion;
//             [ReadOnly] public NativeArray<Quaternion> lrQuaternion;
//             [ReadOnly] public NativeArray<AniFrame> frame;
//             [ReadOnly] public AniBasePos aniBasePos;
//             [ReadOnly] public int entitiesLength;
//             [ReadOnly] public Vector3 one;

//             public void Execute()
//             {
//                 for (int i = 0; i < entitiesLength; i++)
//                 {
//                     // if (isInCamera[i] == 0)continue;

//                     bool isBack = (look[i].isBack != 0);
//                     bool isLeft = (look[i].isLeft != 0);

//                     float bodyDepth = position[i].Value.z;
//                     float leftArmDepth = position[i].Value.z;
//                     float rightArmDepth = position[i].Value.z;
//                     float leftHandDepth = position[i].Value.z;
//                     float rightHandDepth = position[i].Value.z;
//                     float leftLegDepth = position[i].Value.z;
//                     float rightLegDepth = position[i].Value.z;
//                     float leftFootDepth = position[i].Value.z;
//                     float rightFootDepth = position[i].Value.z;

//                     if (isBack)
//                     {
//                         bodyDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.Thorax);
//                         leftArmDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.LeftArm);
//                         rightArmDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.RightArm);
//                         leftHandDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.LeftHand);
//                         rightHandDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.RightHand);
//                         leftLegDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.LeftLeg);
//                         rightLegDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.RightLeg);
//                         leftFootDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.LeftFoot);
//                         rightFootDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.RightFoot);
//                     }
//                     else
//                     {
//                         bodyDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.Thorax);
//                         leftArmDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.LeftArm);
//                         rightArmDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.RightArm);
//                         leftHandDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.LeftHand);
//                         rightHandDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.RightHand);
//                         leftLegDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.LeftLeg);
//                         rightLegDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.RightLeg);
//                         leftFootDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.LeftFoot);
//                         rightFootDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.RightFoot);
//                     }

//                     float bodyX;
//                     float lArmX;
//                     float rArmX;
//                     float lLegX;
//                     float rLegX;
//                     float lHandX;
//                     float rHandX;
//                     float lFootX;
//                     float rFootX;
//                     Quaternion q;
//                     if (isLeft)
//                     {
//                         bodyX = position[i].Value.x - aniBasePos.BODY_BASE.x - frame[i].body.x;
//                         if (isBack)
//                         {
//                             lArmX = position[i].Value.x - aniBasePos.R_ARM_BASE.x - frame[i].rightArm.x;
//                             rArmX = position[i].Value.x - aniBasePos.L_ARM_BASE.x - frame[i].leftArm.x;
//                             lHandX = position[i].Value.x - aniBasePos.R_HAND_BASE.x - frame[i].rightHand.x;
//                             rHandX = position[i].Value.x - aniBasePos.L_HAND_BASE.x - frame[i].leftHand.x;
//                             lLegX = position[i].Value.x - aniBasePos.R_LEG_BASE.x - frame[i].rightLeg.x;
//                             rLegX = position[i].Value.x - aniBasePos.L_LEG_BASE.x - frame[i].leftLeg.x;
//                             lFootX = position[i].Value.x - aniBasePos.R_FOOT_BASE.x - frame[i].rightFoot.x;
//                             rFootX = position[i].Value.x - aniBasePos.L_FOOT_BASE.x - frame[i].leftFoot.x;
//                         }
//                         else
//                         {
//                             lArmX = position[i].Value.x - aniBasePos.L_ARM_BASE.x - frame[i].leftArm.x;
//                             rArmX = position[i].Value.x - aniBasePos.R_ARM_BASE.x - frame[i].rightArm.x;
//                             lHandX = position[i].Value.x - aniBasePos.L_HAND_BASE.x - frame[i].leftHand.x;
//                             rHandX = position[i].Value.x - aniBasePos.R_HAND_BASE.x - frame[i].rightHand.x;
//                             lLegX = position[i].Value.x - aniBasePos.L_LEG_BASE.x - frame[i].leftLeg.x;
//                             rLegX = position[i].Value.x - aniBasePos.R_LEG_BASE.x - frame[i].rightLeg.x;
//                             lFootX = position[i].Value.x - aniBasePos.L_FOOT_BASE.x - frame[i].leftFoot.x;
//                             rFootX = position[i].Value.x - aniBasePos.R_FOOT_BASE.x - frame[i].rightFoot.x;
//                         }

//                         q = lrQuaternion[0];
//                     }
//                     else
//                     {
//                         bodyX = position[i].Value.x + aniBasePos.BODY_BASE.x + frame[i].body.x;
//                         if (isBack)
//                         {
//                             lArmX = position[i].Value.x + aniBasePos.R_ARM_BASE.x + frame[i].rightArm.x;
//                             rArmX = position[i].Value.x + aniBasePos.L_ARM_BASE.x + frame[i].leftArm.x;
//                             lHandX = position[i].Value.x + aniBasePos.R_HAND_BASE.x + frame[i].rightHand.x;
//                             rHandX = position[i].Value.x + aniBasePos.L_HAND_BASE.x + frame[i].leftHand.x;
//                             lLegX = position[i].Value.x + aniBasePos.R_LEG_BASE.x + frame[i].rightLeg.x;
//                             rLegX = position[i].Value.x + aniBasePos.L_LEG_BASE.x + frame[i].leftLeg.x;
//                             lFootX = position[i].Value.x + aniBasePos.R_FOOT_BASE.x + frame[i].rightFoot.x;
//                             rFootX = position[i].Value.x + aniBasePos.L_FOOT_BASE.x + frame[i].leftFoot.x;
//                         }
//                         else
//                         {
//                             lArmX = position[i].Value.x + aniBasePos.L_ARM_BASE.x + frame[i].leftArm.x;
//                             rArmX = position[i].Value.x + aniBasePos.R_ARM_BASE.x + frame[i].rightArm.x;
//                             lHandX = position[i].Value.x + aniBasePos.L_HAND_BASE.x + frame[i].leftHand.x;
//                             rHandX = position[i].Value.x + aniBasePos.R_HAND_BASE.x + frame[i].rightHand.x;
//                             lLegX = position[i].Value.x + aniBasePos.L_LEG_BASE.x + frame[i].leftLeg.x;
//                             rLegX = position[i].Value.x + aniBasePos.R_LEG_BASE.x + frame[i].rightLeg.x;
//                             lFootX = position[i].Value.x + aniBasePos.L_FOOT_BASE.x + frame[i].leftFoot.x;
//                             rFootX = position[i].Value.x + aniBasePos.R_FOOT_BASE.x + frame[i].rightFoot.x;
//                         }

//                         q = lrQuaternion[1];
//                     }

//                     float lArmY;
//                     float rArmY;
//                     float lHandY;
//                     float rHandY;
//                     float lLegY;
//                     float rLegY;
//                     float lFootY;
//                     float rFootY;

//                     if (isBack)
//                     {
//                         lArmY = position[i].Value.y + aniBasePos.R_ARM_BASE.y + frame[i].rightArm.y;
//                         rArmY = position[i].Value.y + aniBasePos.L_ARM_BASE.y + frame[i].leftArm.y;
//                         lHandY = position[i].Value.y + aniBasePos.R_HAND_BASE.y + frame[i].rightHand.y;
//                         rHandY = position[i].Value.y + aniBasePos.L_HAND_BASE.y + frame[i].leftHand.y;
//                         lLegY = position[i].Value.y + aniBasePos.R_LEG_BASE.y + frame[i].rightLeg.y;
//                         rLegY = position[i].Value.y + aniBasePos.L_LEG_BASE.y + frame[i].leftLeg.y;
//                         lFootY = position[i].Value.y + aniBasePos.R_FOOT_BASE.y + frame[i].rightFoot.y;
//                         rFootY = position[i].Value.y + aniBasePos.L_FOOT_BASE.y + frame[i].leftFoot.y;
//                     }
//                     else
//                     {
//                         lArmY = position[i].Value.y + aniBasePos.L_ARM_BASE.y + frame[i].leftArm.y;
//                         rArmY = position[i].Value.y + aniBasePos.R_ARM_BASE.y + frame[i].rightArm.y;
//                         lHandY = position[i].Value.y + aniBasePos.L_HAND_BASE.y + frame[i].leftHand.y;
//                         rHandY = position[i].Value.y + aniBasePos.R_HAND_BASE.y + frame[i].rightHand.y;
//                         lLegY = position[i].Value.y + aniBasePos.L_LEG_BASE.y + frame[i].leftLeg.y;
//                         rLegY = position[i].Value.y + aniBasePos.R_LEG_BASE.y + frame[i].rightLeg.y;
//                         lFootY = position[i].Value.y + aniBasePos.L_FOOT_BASE.y + frame[i].leftFoot.y;
//                         rFootY = position[i].Value.y + aniBasePos.R_FOOT_BASE.y + frame[i].rightFoot.y;
//                     }

//                     bodyMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(bodyX, position[i].Value.y + aniBasePos.BODY_BASE.y + frame[i].body.y,
//                             bodyDepth),
//                         q, one);
//                     leftArmMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(lArmX, lArmY, leftArmDepth),
//                         q, one);
//                     rightArmMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(rArmX, rArmY, rightArmDepth),
//                         q, one);
//                     leftHandMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(lHandX, lHandY, leftHandDepth),
//                         q, one);
//                     rightHandMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(rHandX, rHandY, rightHandDepth),
//                         q, one);

//                     leftLegMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(lLegX, lLegY, leftLegDepth),
//                         q, one);
//                     rightLegMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(rLegX, rLegY, rightLegDepth),
//                         q, one);
//                     leftFootMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(lFootX, lFootY, leftFootDepth),
//                         q, one);
//                     rightFootMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(rFootX, rFootY, rightFootDepth),
//                         q, one);
//                     matrixLength++;
//                 }
//             }
//         }

//         // 触角頭位置
//         [BurstCompileAttribute]
//         struct JobAntHead : IJob
//         {
//             public NativeArray<Matrix4x4> antMatrix;
//             public NativeArray<Matrix4x4> antMatrix2;
//             public NativeArray<Matrix4x4> headMatrix;
//             public NativeArray<Matrix4x4> headMatrix2;
//             public NativeArray<int> matrixLength;
//             [ReadOnly] public NativeArray<int> isInCamera;
//             [ReadOnly] public NativeArray<Position> position;
//             [ReadOnly] public NativeArray<CharaLook> look;
//             [ReadOnly] public NativeArray<CharaMotion> motion;
//             [ReadOnly] public NativeArray<Quaternion> lrQuaternion;
//             [ReadOnly] public NativeArray<AniFrame> frame;
//             [ReadOnly] public AniBasePos aniBasePos;
//             [ReadOnly] public int entitiesLength;
//             [ReadOnly] public Vector3 one;
//             public void Execute()
//             {
//                 int matIndex = 0;
//                 int matIndex2 = 0;
//                 for (int i = 0; i < entitiesLength; i++)
//                 {

//                     // if (isInCamera[i] == 0)continue;

//                     bool isBack = (look[i].isBack != 0);
//                     bool isLeft = (look[i].isLeft != 0);

//                     float antDepth = position[i].Value.z;
//                     float headDepth = position[i].Value.z;
//                     if (isBack)
//                     {
//                         antDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.Ant);
//                         headDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.Head);
//                     }
//                     else
//                     {
//                         antDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.Ant);
//                         headDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.Head);
//                     }

//                     float antX;
//                     float headX;
//                     Quaternion q;
//                     if (isLeft)
//                     {
//                         antX = position[i].Value.x - aniBasePos.ANT_BASE.x - frame[i].ant.x;
//                         headX = position[i].Value.x - aniBasePos.HEAD_BASE.x - frame[i].head.x;
//                         q = lrQuaternion[0];
//                     }
//                     else
//                     {
//                         antX = position[i].Value.x + aniBasePos.ANT_BASE.x + frame[i].ant.x;
//                         headX = position[i].Value.x + aniBasePos.HEAD_BASE.x + frame[i].head.x;
//                         q = lrQuaternion[1];
//                     }

//                     Matrix4x4 tmpAntMatrix = Matrix4x4.TRS(new Vector3(antX,
//                             position[i].Value.y + aniBasePos.ANT_BASE.y + frame[i].ant.y,
//                             antDepth),
//                         q, one);
//                     Matrix4x4 tmpHeadMatrix = Matrix4x4.TRS(new Vector3(headX,
//                             position[i].Value.y + aniBasePos.HEAD_BASE.y + frame[i].head.y,
//                             headDepth),
//                         q, one);

//                     if (isBack)
//                     {
//                         antMatrix2[matIndex2] = tmpAntMatrix;
//                         headMatrix2[matIndex2] = tmpHeadMatrix;
//                         matIndex2++;
//                     }
//                     else
//                     {
//                         antMatrix[matIndex] = tmpAntMatrix;
//                         headMatrix[matIndex] = tmpHeadMatrix;
//                         matIndex++;
//                     }
//                 }
//                 matrixLength[0] = matIndex;
//                 matrixLength[1] = matIndex2;
//             }
//         }

//     }
// }

// //// 胸
// // Graphics.DrawMeshInstanced(Shared.ariMeshMat.meshs[0], 0,
// // 	Shared.ariMeshMat.materials[0], job.thoraxMatrix.ToArray(), job.thoraxMatrix.Length);
// //// 腕
// // Graphics.DrawMeshInstanced(Shared.ariMeshMat.meshs[1], 0,
// // 	Shared.ariMeshMat.materials[1], job.leftArmMatrix.ToArray(), job.leftArmMatrix.Length);
// // Graphics.DrawMeshInstanced(Shared.ariMeshMat.meshs[1], 0,
// // 	Shared.ariMeshMat.materials[1], job.rightArmMatrix.ToArray(), job.rightArmMatrix.Length);
// //// 足
// // Graphics.DrawMeshInstanced(Shared.ariMeshMat.meshs[2], 0,
// // 	Shared.ariMeshMat.materials[2], job.leftLegMatrix.ToArray(), job.leftLegMatrix.Length);
// // Graphics.DrawMeshInstanced(Shared.ariMeshMat.meshs[2], 0,
// // 	Shared.ariMeshMat.materials[2], job.rightLegMatrix.ToArray(), job.rightLegMatrix.Length);

// //// 腹
// // Graphics.DrawMeshInstanced(Shared.ariMeshMat.meshs[3], 0,
// // 	Shared.ariMeshMat.materials[3], jobGaster.gasterMatrix.ToArray(), jobGaster.matrixLength[0]);
// //// 腹
// // Graphics.DrawMeshInstanced(Shared.ariMeshMat.meshs[4], 0,
// // 	Shared.ariMeshMat.materials[4], jobGaster.gasterMatrix2.ToArray(), jobGaster.matrixLength[1]);

// //// アンテナと顔
// // Graphics.DrawMeshInstanced(Shared.ariMeshMat.meshs[6], 0,
// // 	Shared.ariMeshMat.materials[6], jobAntHead.antMatrix.ToArray(), jobAntHead.matrixLength[0]);
// // Graphics.DrawMeshInstanced(Shared.ariMeshMat.meshs[7], 0,
// // 	Shared.ariMeshMat.materials[7], jobAntHead.headMatrix.ToArray(), jobAntHead.matrixLength[0]);

// //// アンテナと顔
// // Graphics.DrawMeshInstanced(Shared.ariMeshMat.meshs[8], 0,
// // 	Shared.ariMeshMat.materials[8], jobAntHead.antMatrix2.ToArray(), jobAntHead.matrixLength[1]);
// // Graphics.DrawMeshInstanced(Shared.ariMeshMat.meshs[9], 0,
// // 	Shared.ariMeshMat.materials[9], jobAntHead.headMatrix2.ToArray(), jobAntHead.matrixLength[1]);
