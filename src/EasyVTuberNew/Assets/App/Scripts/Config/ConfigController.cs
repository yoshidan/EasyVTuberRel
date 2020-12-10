using System.Collections;
using System.Threading.Tasks;
using App.Main.Scripts.HumanInterfaceDevices;
using App.Main.Scripts.Interprocess;
using App.Main.Scripts.Interprocess.Model;
using App.Main.Scripts.VRMLoad;
using UnityNativeWindow;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.UI;
using VRM;
using Zenject;

namespace App.Main.Scripts.Config
{
    public class ConfigController : MonoBehaviour
    {
        private readonly int _hashWaiting = Animator.StringToHash("Waitiing");
        
        [SerializeField] private GameObject menuPanel;
        
        [SerializeField] private Toggle backgroundImageWidthEnabled;
        
        [SerializeField] private Toggle backgroundTransparentEnabled;
        
        [SerializeField] private Toggle desktopShareEnabled;
        
        [SerializeField] private Toggle faceTrackingEnabled;
        [SerializeField] private Dropdown faceTrackingDevices;
        
        [SerializeField] private Toggle handTrackingEnabled;
        
        [SerializeField] private Toggle voiceLipSyncEnabled;
        [SerializeField] private Dropdown microphoneDevices;
        
        [SerializeField] private Toggle cameraControlEnabled;
       
        [SerializeField] private VRMLoadController vrmLoadController;
        
        // [SerializeField] private Toggle micModeEnabled;

        [Inject] private ReceivedMessageHandler _receivedMessageHandler;
        
        private bool _isInitialized = false;
        
        private void Awake()
        {
            menuPanel.SetActive(false);
        }
        
        private void Start()
        {
#if !UNITY_EDITOR
            //強制的に再生時のKeyNoteよりも前に持ってきます。
            WindowApi.Frontize();
            WindowApi.HideTitleBar();
            WindowApi.EnableWindowDrag();
#endif

            vrmLoadController.VrmLoaded += OnVRMLoaded;

            StartCoroutine(LoadAvatar());
        }
        
        private IEnumerator LoadAvatar()
        {
            yield return null;
            vrmLoadController.Load();
        }
        
        private async void OnVRMLoaded(VrmLoadedInfo vrm)
        {
            
            var lookAtHead = vrm.vrmRoot.GetComponent<VRMLookAtHead>();
            var lookAtIk= vrm.vrmRoot.GetComponent<LookAtIK>();
            var bodyIK = vrm.vrmRoot.GetComponent<FullBodyBipedIK>();
            var initialHeadIkTarget = lookAtHead.Target;
            var animator = vrm.animator;

            _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(MessageCommandNames.AutoBlinkDuringFaceTracking, false.ToString()));
            
            //レイアウト調整
            _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(MessageCommandNames.RequestAutoAdjust));
            _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(MessageCommandNames.RequestAutoAdjustEyebrow));
            
            await Task.Delay(300);
            vrm.context.ShowMeshes();
            _isInitialized = true;
            
            cameraControlEnabled.onValueChanged.AddListener(value =>
            {
                _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(MessageCommandNames.CameraControl, value.ToString()));
            });
            
            backgroundImageWidthEnabled.onValueChanged.AddListener(value =>
            {
                _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(MessageCommandNames.ChangeBackgroundImageDirection, value.ToString()));
            });

            backgroundTransparentEnabled.onValueChanged.AddListener((value) =>
           {
               _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(MessageCommandNames.ChangeBackground, value.ToString()));
           });

            var firstPerson = vrm.vrmRoot.GetComponent<VRMFirstPerson>();
            
            //アバターの目線位置に合わせる
            desktopShareEnabled.onValueChanged.AddListener((value) =>
            {
                if (!value)
                {
                    SetVideoChatMode(lookAtHead, lookAtIk, firstPerson);
                }
                else
                { 
                    lookAtHead.Target = initialHeadIkTarget;
                    lookAtIk.solver.target = lookAtHead.Target;
                    _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(MessageCommandNames.RequestAutoAdjust));
                    _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(MessageCommandNames.RequestAutoAdjustEyebrow));
                    _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(MessageCommandNames.EnableDesktopShareMode, true.ToString()));
                }

                lookAtIk.enabled = value;
                bodyIK.enabled = value;
                animator.SetBool(_hashWaiting, !value);
            });

           faceTrackingEnabled.onValueChanged.AddListener((value) =>
           {
               if (value)
               {
                   var device = faceTrackingDevices.options[faceTrackingDevices.value].text;
                   _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(
                       MessageCommandNames.SetCameraDeviceName, device)
                   );
                   _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(
                       MessageCommandNames.EnableFaceTracking, true.ToString())
                   );
               }
               else
               {
                   _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(
                       MessageCommandNames.EnableFaceTracking,false.ToString())
                   );
               }
           });
           
           handTrackingEnabled.onValueChanged.AddListener((value) =>
           {
               _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(
                   MessageCommandNames.EnableImageBasedHandTracking, value.ToString())
               );
           });
           
           handTrackingEnabled.onValueChanged.AddListener((value) =>
           {
               _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(
                   MessageCommandNames.EnableImageBasedHandTracking, value.ToString())
               );
           });
           
           voiceLipSyncEnabled.onValueChanged.AddListener((value) =>
           {
               if (value)
               {
                   var device = microphoneDevices.options[microphoneDevices.value].text;
                   _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(
                       MessageCommandNames.SetMicrophoneDeviceName, device)
                   );
                   _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(
                       MessageCommandNames.EnableLipSync, true.ToString())
                   );
               }
               else
               {
                   _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(
                       MessageCommandNames.EnableLipSync,false.ToString())
                   );
               }
           });

           foreach (var device in WebCamTexture.devices)
           {
               faceTrackingDevices.options.Add(new Dropdown.OptionData(device.name));
           }

           //フェイストラッキングはデフォルトで有効
           faceTrackingDevices.value = faceTrackingDevices.options.FindIndex(c => c.text.StartsWith("FaceTime"));
           faceTrackingDevices.RefreshShownValue();
           var cameraDevice = faceTrackingDevices.options[faceTrackingDevices.value].text;
           _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(
               MessageCommandNames.SetCameraDeviceName, cameraDevice)
           );

           foreach (var device in Microphone.devices)
           {
               microphoneDevices.options.Add(new Dropdown.OptionData(device));
           }
           microphoneDevices.value = 0;
           microphoneDevices.RefreshShownValue();

           SetVideoChatMode(lookAtHead, lookAtIk, firstPerson);

        }

        public void SetVideoChatMode(VRMLookAtHead lookAtHead, LookAtIK lookAtIk , VRMFirstPerson firstPerson)
        {
            lookAtHead.Target = Camera.main.transform;
            lookAtIk.solver.target = lookAtHead.Target;
            var headCenter = firstPerson.FirstPersonBone.localToWorldMatrix.MultiplyPoint(firstPerson.FirstPersonOffset);
            headCenter.z = .9f;
            Camera.main.transform.position = headCenter;
            _receivedMessageHandler.ReceiveCommand(new ReceivedCommand(MessageCommandNames.EnableDesktopShareMode, false.ToString()));
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.M))
            {
                menuPanel.SetActive(!menuPanel.activeInHierarchy);
            }else if (Input.GetKeyDown(KeyCode.T))
            {
                backgroundTransparentEnabled.isOn = !backgroundTransparentEnabled.isOn;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                backgroundImageWidthEnabled.isOn = !backgroundImageWidthEnabled.isOn;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                desktopShareEnabled.isOn = !desktopShareEnabled.isOn;
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                cameraControlEnabled.isOn = !cameraControlEnabled.isOn;
            }
        }
        
    }
}