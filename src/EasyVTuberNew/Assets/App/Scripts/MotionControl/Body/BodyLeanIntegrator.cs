﻿using App.Main.Scripts.VRMLoad;
using UnityEngine;
using Zenject;

namespace App.Main.Scripts.MotionControl.Body
{
    /// <summary>
    /// 各コンポーネントの情報を総合して体の傾き具合(の提案値)に変換するすごいやつだよ
    /// </summary>
    public class BodyLeanIntegrator : MonoBehaviour
    {
        private const float ReferenceHipsHeight = 0.6f;
        
        [SerializeField] private FaceYawToBodyYaw _faceYawToBodyYaw = null;
    //    [SerializeField] private GamepadBasedBodyLean _gamepadBasedBodyLean = null;
        [SerializeField] private ImageBasedBodyMotion _imageBasedBodyMotion = null;
        [SerializeField] private FaceAttitudeController _faceAttitudeController = null;
        
        [Tooltip("腰が1度傾いてたら0.01m=1cmだけ腰をずらす、みたいな比率。" +
                 "適当に決めた基準モデル向けに調整し、他モデルについてはHipsの高さベースで倍率かけて適用。")]
        [SerializeField] private float _bodyRollDegToXOffsetFactor = 0.01f;
        [Tooltip("腰の並進を計算するときに腰ロールがこれ以上大きい場合は切り捨てるよ、という値。")]
        [SerializeField] private float _bodyRollReflectMaxDeg = 4.0f;

        //Xのズレを-Yとか-Zに反映する比率
        [Range(0f, 1f)] [SerializeField] private float x2y = 0f;
        
        public Quaternion BodyLeanSuggest { get; private set; }

        /// <summary> -1 ~ 1の範囲で体のロール度合いを取得します。この値を公開することで、ヒジを適宜開けるようにするのが狙いです。 </summary>
        public float BodyRollRate { get; private set; } = 0f;

        
        public float BodyHorizontalOffsetSuggest { get; private set; } = 0f;

        //腰を左右に動かすとき、ついでに高さも少し下げる: 多少自然にするため。
        public Vector3 BodyOffsetSuggest => new Vector3(
            BodyHorizontalOffsetSuggest,
            -Mathf.Abs(BodyHorizontalOffsetSuggest) * x2y,
            0
            );

        private float _hipsHeightRate = 1.0f;
        

        [Inject]
        public void Initialize(IVRMLoadable vrmLoadable)
        {
            vrmLoadable.VrmLoaded += OnVrmLoaded;
        }

        private void OnVrmLoaded(VrmLoadedInfo info)
        {
            BodyHorizontalOffsetSuggest = 0;
            float hipsHeight = info.animator.GetBoneTransform(HumanBodyBones.Hips).position.y;
            //あまり非常識な値が来たらもう適当に蹴ってしまう。別に蹴ってもそこまで危険でもないし。
            _hipsHeightRate = Mathf.Clamp(hipsHeight / ReferenceHipsHeight, 0.1f, 3f);
        }
        
        private void Update()
        {
            //NOTE:
            // faceAttitudeToBodyYaw: ヨー
            // faceAttitudeController: ロール
            // imageBasedBodyMotion: ロール
            // gamePadBasedBodyLean: ロールとピッチ
            // どれも角度は小さめなので雑にかけてます
            BodyLeanSuggest =
                _faceYawToBodyYaw.BodyYawSuggest *
                _faceAttitudeController.BodyLeanSuggest *
                _imageBasedBodyMotion.BodyLeanSuggest;
                //_gamepadBasedBodyLean.BodyLeanSuggest;
            
            //ロール角を拾う。
            BodyLeanSuggest.ToAngleAxis(out float angle, out Vector3 axis);

            //NOTE: これは近似計算になるハズだけど、まあ十分だろうと。
            float rollAngle = angle * axis.z;
            rollAngle = Mathf.Clamp(rollAngle, -_bodyRollReflectMaxDeg, _bodyRollReflectMaxDeg);

            BodyRollRate = rollAngle / _bodyRollReflectMaxDeg;
            BodyHorizontalOffsetSuggest = rollAngle * _bodyRollDegToXOffsetFactor * _hipsHeightRate;
        }
        
        
    }
}
