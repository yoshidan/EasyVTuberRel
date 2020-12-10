namespace App.Main.Scripts.Interprocess.Model
{
    public static class MessageCommandNames
    {
        // Load VRM
        public const string RequestAutoAdjust = nameof(RequestAutoAdjust);
        public const string RequestAutoAdjustEyebrow = nameof(RequestAutoAdjustEyebrow);
        
        /// <summary>
        /// NOTE: 歴史的経緯により、このキーは名前に反してキーボード/マウスの動作反映のオンオフのみを指定します。
        /// (ゲームパッドやMIDIコンの単体指定はなく、これらのオンオフは現在デバイスの読み取りオンオフで設定します)
        /// </summary>
        public const string EnableHidArmMotion = nameof(EnableHidArmMotion);

        // Motion, Hand
        public const string LengthFromWristToTip = nameof(LengthFromWristToTip);
        public const string LengthFromWristToPalm = nameof(LengthFromWristToPalm);
        public const string HandYOffsetBasic = nameof(HandYOffsetBasic);
        public const string HandYOffsetAfterKeyDown = nameof(HandYOffsetAfterKeyDown);

        // Motion, Arm
        public const string EnableShoulderMotionModify = nameof(EnableShoulderMotionModify);
        public const string SetWaistWidth = nameof(SetWaistWidth);
        public const string SetElbowCloseStrength = nameof(SetElbowCloseStrength);
        public const string EnablePresenterMotion = nameof(EnablePresenterMotion);

        // Motion, Wait
        public const string EnableWaitMotion = nameof(EnableWaitMotion);
        public const string WaitMotionScale = nameof(WaitMotionScale);
        public const string WaitMotionPeriod = nameof(WaitMotionPeriod);
        
        // Motion, Body
        public const string EnableBodyLeanZ = nameof(EnableBodyLeanZ);

        // Motion, Face
        public const string EnableFaceTracking = nameof(EnableFaceTracking);
        public const string SetCameraDeviceName = nameof(SetCameraDeviceName);
        public const string AutoBlinkDuringFaceTracking = nameof(AutoBlinkDuringFaceTracking);
        public const string EnableHeadRotationBasedBlinkAdjust = nameof(EnableHeadRotationBasedBlinkAdjust);
        public const string EnableLipSyncBasedBlinkAdjust = nameof(EnableLipSyncBasedBlinkAdjust);
        public const string DisableFaceTrackingHorizontalFlip = nameof(DisableFaceTrackingHorizontalFlip);
        public const string CalibrateFace = nameof(CalibrateFace);
        public const string SetCalibrateFaceData = nameof(SetCalibrateFaceData);
        public const string FaceDefaultFun = nameof(FaceDefaultFun);

        // Motion, Face, Eyebrow
        public const string EyebrowLeftUpKey = nameof(EyebrowLeftUpKey);
        public const string EyebrowLeftDownKey = nameof(EyebrowLeftDownKey);
        public const string UseSeparatedKeyForEyebrow = nameof(UseSeparatedKeyForEyebrow);
        public const string EyebrowRightUpKey = nameof(EyebrowRightUpKey);
        public const string EyebrowRightDownKey = nameof(EyebrowRightDownKey);
        public const string EyebrowUpScale = nameof(EyebrowUpScale);
        public const string EyebrowDownScale = nameof(EyebrowDownScale);


        // Motion, Mouth
        public const string EnableLipSync = nameof(EnableLipSync);
        public const string SetMicrophoneDeviceName = nameof(SetMicrophoneDeviceName);

        // Motion, Eye
        public const string LookAtStyle = nameof(LookAtStyle);

        // Motion, Image-based Hand
        public const string EnableImageBasedHandTracking = nameof(EnableImageBasedHandTracking);

        //public const string EnableTouchTyping = nameof(EnableTouchTyping);
      // Layout, device layouts
        public const string SetDeviceLayout = nameof(SetDeviceLayout);
        public const string ResetDeviceLayout = nameof(ResetDeviceLayout);
        
        // Layout, HID (keyboard and mousepad)
        public const string EnableDeviceFreeLayout = nameof(EnableDeviceFreeLayout);
        
        // Layout, MIDI Controller
        public const string SetDeviceTypeToStartWordToMotion = nameof(SetDeviceTypeToStartWordToMotion);

        // Configuration 
        public const string ChangeBackground = nameof(ChangeBackground);
        public const string ChangeBackgroundImageDirection = nameof(ChangeBackgroundImageDirection);
        public const string CameraControl = nameof(CameraControl);
        public const string EnableDesktopShareMode = nameof(EnableDesktopShareMode);
    }
}

