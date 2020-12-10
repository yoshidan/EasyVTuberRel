using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Main.Scripts.SettingAdjust;
using App.Main.Scripts.VRMLoad;
using RootMotion;
using RootMotion.FinalIK;
using UniHumanoid;
using UniJSON;
using UnityEngine;
using UnityEngine.Rendering;
using VRM;

namespace App.Main.Scripts.VRMLoad
{
    
    public class VRMLoadController : MonoBehaviour, IVRMLoadable
    {
        
        [Serializable]
        public struct VrmLoadSetting
        {
            public Transform bodyTarget;
            public Transform leftHandTarget;
            public Transform rightHandTarget;
            public Transform rightIndexTarget;
            public Transform headTarget;
        }

        [SerializeField] 
        private VrmLoadSetting loadSetting;
        
        [SerializeField] 
        private SettingAutoAdjuster settingAdjuster = null;
        
        [SerializeField] 
        private RuntimeAnimatorController animatorController = null;
        
        public event Action<VrmLoadedInfo> PreVrmLoaded;
        public event Action<VrmLoadedInfo> VrmLoaded; 
        public event Action VrmDisposing;
        
        private HumanPoseTransfer _humanPoseTransferTarget;

        public void Load()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/avatar.vrm";
            if (!File.Exists(path))
            {
                throw new Exception("no file found");
            }

            var data = File.ReadAllBytes(path);
            var importerContext = new VRMImporterContext();
            importerContext.ParseGlb(data);
            importerContext.Load();
            var root = importerContext.Root;
            importerContext.EnableUpdateWhenOffscreen();
            root.GetComponentsInChildren<Renderer>().ToList().ForEach( e =>
            {
                e.shadowCastingMode = ShadowCastingMode.Off;
                e.receiveShadows = false; 
            });

            var lookAt = root.GetComponent<VRMLookAtHead>();
            _humanPoseTransferTarget = root.AddComponent<HumanPoseTransfer>();
            _humanPoseTransferTarget.SourceType = HumanPoseTransfer.HumanPoseTransferSourceType.None;
            lookAt.UpdateType = UpdateType.LateUpdate;
            
            //Spring boneのスケジュール

            SetIK(root, loadSetting);
            
            var animator = root.GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            
            settingAdjuster.AssignModelRoot(root.transform);
            
            var info = new VrmLoadedInfo()
            {
                context = importerContext,
                vrmRoot = root.transform,
                animator = animator,
                blendShape = root.GetComponent<VRMBlendShapeProxy>(),
            };

            PreVrmLoaded?.Invoke(info);
            VrmLoaded?.Invoke(info);
        }
        
         public static void SetIK(GameObject root, VrmLoadSetting setting)
        {
            var animator = root.GetComponent<Animator>();
            animator.applyRootMotion = false;

            var bipedReferences = LoadReferencesFromVrm(root.transform, animator);
            var ik = AddFBBIK(root, setting, bipedReferences);

            var vrmLookAt = root.GetComponent<VRMLookAtHead>();
            vrmLookAt.Target = setting.headTarget;

            var vrmLookAtBoneApplier = root.GetComponent<VRMLookAtBoneApplyer>();

//            vrmLookAtBoneApplier.HorizontalInner.CurveYRangeDegree = 15;
 //           vrmLookAtBoneApplier.HorizontalOuter.CurveYRangeDegree = 15;
            //vrmLookAtBoneApplier.VerticalDown.CurveYRangeDegree = 10;
            //vrmLookAtBoneApplier.VerticalUp.CurveYRangeDegree = 20;
            
            AddLookAtIK(root, setting.headTarget, animator, bipedReferences.root);
            AddFingerRigToRightIndex(animator, setting);
        }

        private static FullBodyBipedIK AddFBBIK(GameObject go, VrmLoadSetting setting, BipedReferences reference)
        {
            var fbbik = go.AddComponent<FullBodyBipedIK>();
            fbbik.SetReferences(reference, null);

            //IK目標をロードしたVRMのspineに合わせることで、BodyIKがいきなり動いてしまうのを防ぐ。
            //bodyTargetは実際には多階層なので当て方に注意
            setting.bodyTarget.position = reference.spine[0].position;
            fbbik.solver.bodyEffector.target = setting.bodyTarget;
            fbbik.solver.bodyEffector.positionWeight = 0.5f;
            //Editorで "FBBIK > Body > Mapping > Maintain Head Rot"を選んだ時の値を↓で入れてる(デフォルト0、ある程度大きくするとLook Atの見栄えがよい)
            fbbik.solver.boneMappings[0].maintainRotationWeight = 0.7f;

            fbbik.solver.leftHandEffector.target = setting.leftHandTarget;
            fbbik.solver.leftHandEffector.positionWeight = 1.0f;
            fbbik.solver.leftHandEffector.rotationWeight = 1.0f;

            fbbik.solver.rightHandEffector.target = setting.rightHandTarget;
            fbbik.solver.rightHandEffector.positionWeight = 1.0f;
            fbbik.solver.rightHandEffector.rotationWeight = 1.0f;

            //small pull: プレゼンモード中にキャラが吹っ飛んでいかないための対策
            fbbik.solver.rightArmChain.pull = 0.1f;
            
            return fbbik;
        }

        private static void AddLookAtIK(GameObject go, Transform headTarget, Animator animator, Transform referenceRoot)
        {
            var lookAtIk = go.AddComponent<LookAtIK>();
            lookAtIk.solver.SetChain(
                new Transform[]
                {
                    animator.GetBoneTransform(HumanBodyBones.Hips),
                    animator.GetBoneTransform(HumanBodyBones.Spine),
                    animator.GetBoneTransform(HumanBodyBones.UpperChest),
                    animator.GetBoneTransform(HumanBodyBones.Neck),
                }
                    .Where(t => t != null)
                    .ToArray(),
                animator.GetBoneTransform(HumanBodyBones.Head),
                new Transform[0],
                referenceRoot
                );

            lookAtIk.solver.target = headTarget;
            lookAtIk.solver.bodyWeight = 0.2f;
            lookAtIk.solver.headWeight = 0.5f;
        }

        private static BipedReferences LoadReferencesFromVrm(Transform root, Animator animator)
        {
            return new BipedReferences()
            {
                root = root,
                pelvis = animator.GetBoneTransform(HumanBodyBones.Hips),

                leftThigh = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
                leftCalf = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg),
                leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot),

                rightThigh = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
                rightCalf = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg),
                rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot),

                leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm),
                leftForearm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm),
                leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand),

                rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm),
                rightForearm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm),
                rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand),

                head = animator.GetBoneTransform(HumanBodyBones.Head),

                spine = new Transform[]
                {
                    animator.GetBoneTransform(HumanBodyBones.Spine),
                },

                eyes = new Transform[0],
            };
        }

        private static void AddFingerRigToRightIndex(Animator animator, VrmLoadSetting setting)
        {
            //NOTE: FinalIKのサンプルにあるFingerRigを持ち込んでみる。
            var rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

            var fingerRig = rightHand.gameObject.AddComponent<FingerRig>();
            fingerRig.AddFinger(
                animator.GetBoneTransform(HumanBodyBones.RightIndexProximal),
                animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate),
                animator.GetBoneTransform(HumanBodyBones.RightIndexDistal),
                setting.rightIndexTarget
                );
            //とりあえず無効化し、有効にするのはInputDeviceToMotionの責務にする
            fingerRig.weight = 0.0f;
            fingerRig.fingers[0].rotationDOF = Finger.DOF.One;
            fingerRig.fingers[0].weight = 1.0f;
            fingerRig.fingers[0].rotationWeight = 0;
        }
    }
}