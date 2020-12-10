using App.Main.Scripts.Interprocess;
using App.Main.Scripts.Interprocess.Model;
using UnityEngine;
using UniRx;
using Zenject;

namespace App.Main.Scripts.MotionControl
{
    /// <summary> モーション関係で、操作ではなく設定値を受け取るレシーバクラス </summary>
    public class MotionSettingReceiver : MonoBehaviour
    {
        //Word to Motionの専用入力に使うデバイスを指定する定数値
//        private const int DeviceTypeNone = -1;
        private const int DeviceTypeKeyboardWord = 0;
        private const int DeviceTypeGamepad = 1;
        private const int DeviceTypeKeyboardTenKey = 2;
        private const int DeviceTypeMidiController = 3;

        [Inject] private ReceivedMessageHandler _handler = null;

//        [SerializeField] private GamepadBasedBodyLean gamePadBasedBodyLean = null;
 //       [SerializeField] private SmallGamepadHandIKGenerator smallGamepadHandIk = null;

        [SerializeField] private HandIKIntegrator handIkIntegrator = null;

        [SerializeField] private HeadIkIntegrator headIkIntegrator = null;

//        [SerializeField] private IkWeightCrossFade ikWeightCrossFade = null;
        
        //[SerializeField] private StatefulXinputGamePad gamePad = null;

        private void Start()
        {
            _handler.Commands.Subscribe(message =>
            {
                switch (message.Command)
                {
                    case MessageCommandNames.EnableHidArmMotion:
                        handIkIntegrator.EnableHidArmMotion = message.ToBoolean();
                        //ikWeightCrossFade.ForceStopHandIk = !message.ToBoolean();
                        break;
                    case MessageCommandNames.LengthFromWristToPalm:
                        SetLengthFromWristToPalm(message.ParseAsCentimeter());
                        break;
                    case MessageCommandNames.LengthFromWristToTip:
                        SetLengthFromWristToTip(message.ParseAsCentimeter());
                        break;
                    case MessageCommandNames.HandYOffsetBasic:
                        SetHandYOffsetBasic(message.ParseAsCentimeter());
                        break;
                    case MessageCommandNames.HandYOffsetAfterKeyDown:
                        SetHandYOffsetAfterKeyDown(message.ParseAsCentimeter());
                        break;
                    case MessageCommandNames.EnablePresenterMotion:
                        handIkIntegrator.EnablePresentationMode = message.ToBoolean();
                        break;
                    case MessageCommandNames.LookAtStyle:
                        headIkIntegrator.SetLookAtStyle(message.Content);
                        break;
                    case MessageCommandNames.SetDeviceTypeToStartWordToMotion:
                        SetDeviceTypeForWordToMotion(message.ToInt());
                        break;
                }
            });
        }

        private void SetDeviceTypeForWordToMotion(int deviceType)
        {
            handIkIntegrator.UseKeyboardForWordToMotion = (deviceType == DeviceTypeKeyboardTenKey);
        }

        //以下については適用先が1つじゃないことに注意

        private void SetLengthFromWristToTip(float v)
        {
            handIkIntegrator.Typing.HandToTipLength = v;
            handIkIntegrator.MouseMove.HandToTipLength = v;
        }
        
        private void SetLengthFromWristToPalm(float v)
        {
            //handIkIntegrator.MouseMove.HandToPalmLength = v;
        }
        
        private void SetHandYOffsetBasic(float offset)
        {
            handIkIntegrator.Typing.YOffsetAlways = offset;
            handIkIntegrator.MouseMove.YOffset = offset;
        }

        private void SetHandYOffsetAfterKeyDown(float offset)
        {
            handIkIntegrator.Typing.YOffsetAfterKeyDown = offset;
        }
    }
}
