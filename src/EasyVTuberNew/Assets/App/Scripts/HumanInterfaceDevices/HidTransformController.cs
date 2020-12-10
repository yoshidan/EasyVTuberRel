using App.Main.Scripts.Interprocess;
using App.Main.Scripts.SettingAdjust;
using UnityEngine;
using Zenject;

namespace App.Main.Scripts.HumanInterfaceDevices
{
   public class HidTransformController : MonoBehaviour
    {
        [Inject]
        private ReceivedMessageHandler handler = null;

        [SerializeField] private KeyboardProvider keyboard = null;
        [SerializeField] private TouchPadProvider touchpad = null;
        
        //NOTE: この辺の基準値はMegumi Baxterさんの表示時にちょうどよくなる値
        [SerializeField] private Vector3 refKeyboardPosition = new Vector3(0, 0.9f, 0.28f);
        [SerializeField] private Vector3 refKeyboardRotation = Vector3.zero;
        [SerializeField] private Vector3 refKeyboardScale = new Vector3(0.5f, 1f, 0.5f);

        [SerializeField] private Vector3 refTouchpadPosition = new Vector3(0.25f, 0.98f, 0.25f);
        [SerializeField] private Vector3 refTouchpadRotation = new Vector3(60, 30, 0);
        [SerializeField] private Vector3 refTouchpadScale = new Vector3(0.15f, 0.15f, 1.0f);

        [SerializeField] private Vector3 refMidiPosition = new Vector3(0, 0.85f, 0.3f);
        [SerializeField] private Vector3 refMidiRotation = Vector3.zero;
        [SerializeField] private Vector3 refMidiScale = new Vector3(0.5f, 0.5f, 0.5f);
     
        /// <summary>
        /// 指定されたパラメータベースを用いてタッチパッドとキーボードの位置を初期化します。
        /// </summary>
        /// <param name="parameters"></param>
        public void SetHidLayoutByParameter(DeviceLayoutAutoAdjustParameters parameters)
        {
            SetKeyboardLayout(parameters);
            SetTouchPadLayout(parameters);
            
            void SetKeyboardLayout(DeviceLayoutAutoAdjustParameters p)
            {
                var keyboardTransform = keyboard.transform;

                keyboardTransform.localRotation = Quaternion.Euler(refKeyboardRotation);
                keyboardTransform.localPosition = new Vector3(
                    refKeyboardPosition.x * p.ArmLengthFactor,
                    refKeyboardPosition.y * p.HeightFactor,
                    refKeyboardPosition.z * p.ArmLengthFactor
                );
                keyboardTransform.localScale = new Vector3(
                    refKeyboardScale.x * p.ArmLengthFactor,
                    1.0f,
                    refKeyboardScale.z * p.ArmLengthFactor
                );
            }

            void SetTouchPadLayout(DeviceLayoutAutoAdjustParameters p)
            {
                var touchpadTransform = touchpad.transform;
            
                touchpadTransform.localRotation = Quaternion.Euler(refTouchpadRotation);
                touchpadTransform.localPosition = new Vector3(
                    refTouchpadPosition.x * p.ArmLengthFactor,
                    refTouchpadPosition.y * p.HeightFactor,
                    refTouchpadPosition.z * p.ArmLengthFactor
                );
                touchpadTransform.localScale = new Vector3(
                    refTouchpadScale.x * p.ArmLengthFactor,
                    refTouchpadScale.y * p.ArmLengthFactor,
                    1.0f
                );
            }

        }

    }
}