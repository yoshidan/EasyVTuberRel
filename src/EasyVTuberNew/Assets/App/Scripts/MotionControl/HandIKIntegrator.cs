using App.Main.Scripts.VRMLoad;
using UnityEngine;
using Zenject;

namespace App.Main.Scripts.MotionControl
{
    /// <summary>
    /// ユーザーの入力と設定に基づいて、実際にIKを適用していくやつ
    /// </summary>
    public class HandIKIntegrator : MonoBehaviour
    {
        //NOTE: ステートパターンがめんどくさいときのステートマシンの実装です。まあステート数少ないので…

        /// <summary> IK種類が変わるときのブレンディングに使う時間。IK自体の無効化/有効化もこの時間で行う </summary>
        private const float HandIkToggleDuration = 0.25f;
        private const float HandIkTypeChangeCoolDown = 0.3f;

        [SerializeField] private Transform rightHandTarget = null;
        [SerializeField] private Transform leftHandTarget = null;

        [SerializeField] private TypingHandIKGenerator typing = null;
        public TypingHandIKGenerator Typing => typing;

        [SerializeField] private MouseMoveHandIKGenerator mouseMove = null;
        public MouseMoveHandIKGenerator MouseMove => mouseMove;
        
        [SerializeField] private FingerController fingerController = null;
        
        private float _leftHandStateBlendCount = 0f;
        private float _rightHandStateBlendCount = 0f;

        private float _leftHandIkChangeCoolDown = 0f;
        private float _rightHandIkChangeCoolDown = 0f;

        private bool _enableHidArmMotion = true;

        public bool EnableHidArmMotion
        {
            get => _enableHidArmMotion;
            set
            {
                _enableHidArmMotion = value;
                mouseMove.EnableUpdate = value;
            }
        }
        
        //NOTE: このフラグではキーボードのみならずマウス入力も無視することに注意
        /// <summary> NOTE: 歴史的経緯によって「受け取ってるけど使わないフラグ」になってます。 </summary>
        public bool UseKeyboardForWordToMotion { get; set; } = false;
        public bool EnablePresentationMode { get; set; }

        public bool IsLeftHandGripGamepad => false;
        public bool IsRightHandGripGamepad => false;

        public Vector3 RightHandPosition => rightHandTarget.position;
        public Vector3 LeftHandPosition => leftHandTarget.position;
        
        
        [Inject] private IVRMLoadable _vrmLoadable = null;

        //NOTE: 初めて手がキーボードから離れるまではnull
        private IIKGenerator _prevRightHand = null;

        //NOTE: Start以降はnullにならない
        private IIKGenerator _currentRightHand = null;

        private IIKGenerator _prevLeftHand = null;
        private IIKGenerator _currentLeftHand = null;

        private HandTargetType _leftTargetType = HandTargetType.Keyboard;
        private HandTargetType _rightTargetType = HandTargetType.Keyboard;
        
        #region API

        #region Keyboard and Mouse
        
        public void PressKey(string keyName)
        {
            if (!EnableHidArmMotion)
            {
                return;
            }
            
            var (hand, pos) = typing.PressKey(keyName, EnablePresentationMode);
            if (!CheckCoolDown(hand, HandTargetType.Keyboard))
            {
                return;
            }
            
            if (hand == ReactedHand.Left)
            {
                SetLeftHandIk(HandTargetType.Keyboard);
            }
            else if (hand == ReactedHand.Right)
            {
                SetRightHandIk(HandTargetType.Keyboard);
            }
            
            fingerController.StartPressKeyMotion(keyName, EnablePresentationMode);	
            
            if (hand != ReactedHand.None && EnableHidArmMotion)
            {
               // particleStore.RequestKeyboardParticleStart(pos);
            }
        }

        public void MoveMouse(Vector3 mousePosition)
        {
            if (!EnableHidArmMotion)
            {
                return;
            }
            
            if (!CheckCoolDown(
                ReactedHand.Right, 
                EnablePresentationMode ? HandTargetType.Presentation : HandTargetType.Mouse
                ))
            {
                return;
            }
            
            SetRightHandIk(EnablePresentationMode ? HandTargetType.Presentation : HandTargetType.Mouse);
           
        }

        public void ClickMouse(string button)
        {
            if (!EnablePresentationMode && EnableHidArmMotion)
            {
                fingerController.StartClickMotion(button);
                SetRightHandIk(HandTargetType.Mouse);   
              
            }
        }

        #endregion
        
        #region Image Base Hand

        //画像処理の手検出があったらそっちのIKに乗り換える
        private void CheckHandUpdates()
        {
        }
        
        #endregion
        
        /// <summary> 既定の秒数をかけて手のIKを無効化します。 </summary>
        public void DisableHandIk()
        {
        }

        /// <summary> 既定の秒数をかけて手のIKを有効化します。 </summary>
        public void EnableHandIk()
        {
        }

        #endregion

        private void Start()
        {
            _currentRightHand = Typing.RightHand;
            _currentLeftHand = Typing.LeftHand;
            _leftHandStateBlendCount = HandIkToggleDuration;
            _rightHandStateBlendCount = HandIkToggleDuration;

            _vrmLoadable.VrmLoaded += OnVrmLoaded;
            _vrmLoadable.VrmDisposing += OnVrmDisposing;
        }
        
        private void OnVrmLoaded(VrmLoadedInfo info)
        {
            fingerController.Initialize(info.animator);
            
        }

        private void OnVrmDisposing()
        {
            fingerController.Dispose();
        }
        
        private void Update()
        {
            CheckHandUpdates();
            
            //ねらい: 前のステートと今のステートをブレンドしながら実際にIKターゲットの位置、姿勢を更新する
            UpdateLeftHand();
            UpdateRightHand();
        }

        //TODO: IKオン/オフとの兼ね合いがアレなのでどうにかしてね。

        private void UpdateLeftHand()
        {
            if (_leftHandIkChangeCoolDown > 0)
            {
                _leftHandIkChangeCoolDown -= Time.deltaTime;
            }
            
            //普通の状態: 複数ステートのブレンドはせず、今のモードをそのまま通す
            if (_leftHandStateBlendCount >= HandIkToggleDuration)
            {
                leftHandTarget.localPosition = _currentLeftHand.Position;
                leftHandTarget.localRotation = _currentLeftHand.Rotation;
                return;
            }

            //NOTE: ここの下に来る時点では必ず_prevLeftHandに非null値が入る実装になってます

            _leftHandStateBlendCount += Time.deltaTime;
            //prevStateと混ぜるための比率
            float t = CubicEase(_leftHandStateBlendCount / HandIkToggleDuration);
            leftHandTarget.localPosition = Vector3.Lerp(
                _prevLeftHand.Position,
                _currentLeftHand.Position,
                t
            );

            leftHandTarget.localRotation = Quaternion.Slerp(
                _prevLeftHand.Rotation,
                _currentLeftHand.Rotation,
                t
            );
        }

        private void UpdateRightHand()
        {
            if (_rightHandIkChangeCoolDown > 0f)
            {
                _rightHandIkChangeCoolDown -= Time.deltaTime;
            }
            
            //普通の状態: 複数ステートのブレンドはせず、今のモードをそのまま通す
            if (_rightHandStateBlendCount >= HandIkToggleDuration)
            {
                rightHandTarget.localPosition = _currentRightHand.Position;
                rightHandTarget.localRotation = _currentRightHand.Rotation;
                return;
            }

            //NOTE: 実装上ここの下に来る時点で_prevRightHandが必ず非nullなのでnullチェックはすっ飛ばす
            
            _rightHandStateBlendCount += Time.deltaTime;
            //prevStateと混ぜるための比率
            float t = CubicEase(_rightHandStateBlendCount / HandIkToggleDuration);
            
            rightHandTarget.localPosition = Vector3.Lerp(
                _prevRightHand.Position,
                _currentRightHand.Position,
                t
            );

            rightHandTarget.localRotation = Quaternion.Slerp(
                _prevRightHand.Rotation,
                _currentRightHand.Rotation,
                t
            );
        }

        private void SetLeftHandIk(HandTargetType targetType)
        {
            if (_leftTargetType == targetType)
            {
                return;
            }

            _leftHandIkChangeCoolDown = HandIkTypeChangeCoolDown;

            var prevType = _leftTargetType;
            _leftTargetType = targetType;

            var ik = Typing.LeftHand;

            _prevLeftHand = _currentLeftHand;
            _currentLeftHand = ik;
            _leftHandStateBlendCount = 0f;

        }

        private void SetRightHandIk(HandTargetType targetType)
        {
            if (_rightTargetType == targetType)
            {
                return;
            }

            _rightHandIkChangeCoolDown = HandIkTypeChangeCoolDown;

            var prevType = _rightTargetType;
            _rightTargetType = targetType;

            var ik =
                (targetType == HandTargetType.Mouse) ? MouseMove.RightHand : Typing.RightHand;

            _prevRightHand = _currentRightHand;
            _currentRightHand = ik;
            _rightHandStateBlendCount = 0f;

            fingerController.RightHandPresentationMode = (targetType == HandTargetType.Presentation);
            
        }

        //クールダウンタイムを考慮したうえで、モーションを適用してよいかどうかを確認します。
        private bool CheckCoolDown(ReactedHand hand, HandTargetType targetType)
        {
            if ((hand == ReactedHand.Left && targetType == _leftTargetType) ||
                (hand == ReactedHand.Right && targetType == _rightTargetType)
            )
            {
                //同じデバイスを続けて触っている -> 素通しでOK
                return true;
            }

            return
                (hand == ReactedHand.Left && _leftHandIkChangeCoolDown <= 0) ||
                (hand == ReactedHand.Right && _rightHandIkChangeCoolDown <= 0);
        }
        
        /// <summary>
        /// x in [0, 1] を y in [0, 1]へ3次補間するやつ
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        private static float CubicEase(float rate) 
            => 2 * rate * rate * (1.5f - rate);

        enum HandTargetType
        {
            Mouse,
            Keyboard,
            Presentation,
        }

    }
}