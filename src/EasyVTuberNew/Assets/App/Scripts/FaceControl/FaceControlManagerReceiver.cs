﻿using App.Main.Scripts.Interprocess;
using App.Main.Scripts.Interprocess.Model;
using UnityEngine;
using UniRx;
using Zenject;

namespace App.Main.Scripts.FaceControl
{
    [RequireComponent(typeof(FaceControlManager))]
    [RequireComponent(typeof(BehaviorBasedAutoBlinkAdjust))]
    public class FaceControlManagerReceiver : MonoBehaviour
    {
        [Inject] private ReceivedMessageHandler handler = null;

        private FaceControlManager _faceControlManager;
        private BehaviorBasedAutoBlinkAdjust _autoBlinkAdjust;

        private void Start()
        {
            _faceControlManager = GetComponent<FaceControlManager>();
            _autoBlinkAdjust = GetComponent<BehaviorBasedAutoBlinkAdjust>();
            handler.Commands.Subscribe(message =>
            {
                switch (message.Command)
                {
                    case MessageCommandNames.EnableFaceTracking:
                        _faceControlManager.IsFaceTrackingActive = message.ToBoolean();
                        break;
                    case MessageCommandNames.AutoBlinkDuringFaceTracking:
                        _faceControlManager.PreferAutoBlink = message.ToBoolean();
                        break;
                    case MessageCommandNames.FaceDefaultFun:
                        _faceControlManager.DefaultBlendShape.FaceDefaultFunValue = message.ParseAsPercentage();
                        break;
                    case MessageCommandNames.EnableHeadRotationBasedBlinkAdjust:
                        _autoBlinkAdjust.EnableHeadRotationBasedBlinkAdjust = message.ToBoolean();
                        break;
                    case MessageCommandNames.EnableLipSyncBasedBlinkAdjust:
                        _autoBlinkAdjust.EnableLipSyncBasedBlinkAdjust = message.ToBoolean();
                        break;
                }
            });
        }
    }
}
