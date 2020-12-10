﻿using System.Linq;
using App.Main.Scripts.Interprocess;
using App.Main.Scripts.Interprocess.Model;
using UnityEngine;
using UniRx;
using Zenject;

namespace App.Main.Scripts.FaceTracking
{
    [RequireComponent(typeof(FaceTracker))]
    public class FaceTrackerReceiver : MonoBehaviour
    {
        [Inject] private ReceivedMessageHandler handler = null;

        private FaceTracker _faceTracker;
        private bool _enableFaceTracking = true;
        private string _cameraDeviceName = "";

        private void Start()
        {
            _faceTracker = GetComponent<FaceTracker>();

            handler.Commands.Subscribe(message =>
            {
                switch (message.Command)
                {
                    case MessageCommandNames.SetCameraDeviceName:
                        SetCameraDeviceName(message.Content);
                        break;
                    case MessageCommandNames.EnableFaceTracking:
                        EnableFaceTracking(message.ToBoolean());
                        break;
                    case MessageCommandNames.CalibrateFace:
                        _faceTracker.StartCalibration();
                        break;
                    case MessageCommandNames.SetCalibrateFaceData:
                        _faceTracker.SetCalibrateData(message.Content);
                        break;
                    case MessageCommandNames.DisableFaceTrackingHorizontalFlip:
                        _faceTracker.DisableHorizontalFlip = message.ToBoolean();
                        break;
                }
            });

        }

        public void EnableFaceTracking(bool enable)
        {
            if (_enableFaceTracking == enable)
            {
                return;
            }

            _enableFaceTracking = enable;
            UpdateFaceDetectorState();
        }

        public void SetCameraDeviceName(string content)
        {
            if (content != _cameraDeviceName)
            {
                _cameraDeviceName = content;
                UpdateFaceDetectorState();
            }
        }
        
        private void UpdateFaceDetectorState()
        {
            if (_enableFaceTracking && !string.IsNullOrWhiteSpace(_cameraDeviceName))
            {
                _faceTracker.ActivateCamera(_cameraDeviceName);
            }
            else
            {
                _faceTracker.StopCamera();
            }
        }

        private string[] GetCameraDeviceNames()
            => WebCamTexture.devices
            .Select(d => d.name)
            .ToArray();
        
    }
}
