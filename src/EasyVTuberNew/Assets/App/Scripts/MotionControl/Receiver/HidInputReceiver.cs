using System;
using App.Main.Scripts.Interprocess;
using App.Main.Scripts.Interprocess.Model;
using App.Main.Scripts.VRMLoad;
using UniRx;
using UnityNativeWindow;
using UnityEngine;
using Zenject;

namespace App.Main.Scripts.MotionControl
{
    
    /// <summary>
    /// プロセス内外の両方から飛んできたHID(キーボード、マウス、コントローラ、MIDI)の入力を集約して投げるクラス
    /// </summary>
    public class HidInputReceiver : MonoBehaviour
    {
        [SerializeField] private HandIKIntegrator handIkIntegrator = null;
        [SerializeField] private HeadIkIntegrator headIkIntegrator = null;
        [SerializeField] private VRMLoadController vrmLoadController = null;
        
        private bool _mousePositionInitialized = false;
        private int _mouseX = 0;
        private int _mouseY = 0;

        [Inject] private ReceivedMessageHandler _receivedMessageHandler;
        
        private void Start()
        {
            if (KeyApi.IsKeyEventObservable())
            {
                KeyApi.ObserveKeyDown(code =>
                {
                    if (enabled) ReceiveKeyPressed(code.ToUpper());
                });
            }
           
            MouseApi.ObserveLeftMouseDown(() =>
            {
                if(enabled) ReceiveMouseButton(FingerController.LDown);
            });
            MouseApi.ObserveRightMouseDown(() =>
            {
                if(enabled) ReceiveMouseButton(FingerController.RDown);
            });
            
            _receivedMessageHandler.Commands.Subscribe((message) =>
            {
                switch (message.Command)
                {
                    case MessageCommandNames.EnableDesktopShareMode:
                    {
                        enabled = message.ToBoolean();
                        break;
                    }
                }
            });
        }

        private void Update()
        {
            //NOTE: Unityが非アクティブのときの値取得については Run in Backgroundを前提として
            // - マウス位置: OK, Input.mousePositionで取れる。
            // - マウスクリック: NG, グローバルフック必須
            // - キーボード: NG, グローバルフック必須
            var pos = Input.mousePosition;
            if (!_mousePositionInitialized)
            {
                _mouseX = (int)pos.x;
                _mouseY = (int)pos.y;
                _mousePositionInitialized = true;
            }

            if (_mouseX != (int)pos.x || 
                _mouseY != (int)pos.y
                )
            {
                _mouseX = (int)pos.x;
                _mouseY = (int)pos.y;
                handIkIntegrator.MoveMouse(pos);
                headIkIntegrator.MoveMouse(_mouseX, _mouseY);
            }

            if (Input.GetMouseButtonDown(0))
            {
                ReceiveMouseButton(FingerController.LDown);
            }else if (Input.GetMouseButtonDown(1))
            {
                ReceiveMouseButton(FingerController.RDown);
            }else if (Input.GetMouseButtonDown(2))
            {
                ReceiveMouseButton(FingerController.MDown);
            }else if (Input.anyKeyDown)
            {
                //フォアグラウンド
                foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(code))
                    {
                        var strCode = code.ToString();
                        ReceiveKeyPressed(strCode);
                        break;
                    }
                }
            }
        }
        
        private static Vector2 NormalizedStickPos(Vector2Int v)
        {
            const float factor = 1.0f / 32768.0f;
            return new Vector2(v.x * factor, v.y * factor);
        }
        
        private void ReceiveKeyPressed(string keyCodeName)
        {
            handIkIntegrator.PressKey(keyCodeName);
        }

        private void ReceiveMouseButton(string info)
        {
            if (info.Contains("Down"))
            {
                handIkIntegrator.ClickMouse(info);
            }
        }

        private void OnDestroy()
        {
            KeyApi.Release();
            MouseApi.Release();
            
        }
    }
}

