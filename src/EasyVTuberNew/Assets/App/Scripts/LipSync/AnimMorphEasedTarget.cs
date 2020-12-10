using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Main.Scripts.FaceControl;
using App.Main.Scripts.FaceTracking;
using App.Main.Scripts.Utils;
using App.Main.Scripts.VRMLoad;
using UnityEngine;
using VRM;
using Zenject;

namespace App.Main.Scripts.MotionControl
{
    public class AnimMorphEasedTarget : MonoBehaviour
    {
        [Tooltip("主要な母音音素(aa, E, ih, oh, ou)に対してBlendShapeを動かすカーブ")]
        public AnimationCurve transitionCurves = new AnimationCurve(new[]
        {
            new Keyframe(0.0f, 0.0f),
            new Keyframe(0.1f, 1.0f),
        });

        [Tooltip("発音しなくなった母音のBlendShapeをゼロに近づける際の速度を表す値")]
        public float cancelSpeedFactor = 8.0f;

        [Range(0.0f, 100.0f), Tooltip("この閾値未満の音素の重みは無視する")]
        public float weightThreshold = 2.0f;

        [Tooltip("BlendShapeの値を変化させるSkinnedMeshRenderer")]
        public VRMBlendShapeProxy blendShapeProxy;

        [Tooltip("OVRLipSyncに渡すSmoothing amountの値")]
        public int smoothAmount = 100;

        [SerializeField] private KeyboardBlendShapeController keyboardBlendShapeController;

        //このフラグがtrueだと何も言ってないときの口の形になる。
        public bool ForceClosedMouth { get; set; } = true;
        
        private readonly Dictionary<BlendShapeKey, float> _blendShapeWeights = new Dictionary<BlendShapeKey, float>
        {
            [new BlendShapeKey(BlendShapePreset.A)] = 0.0f,
            [new BlendShapeKey(BlendShapePreset.E)] = 0.0f,
            [new BlendShapeKey(BlendShapePreset.I)] = 0.0f,
            [new BlendShapeKey(BlendShapePreset.O)] = 0.0f,
            [new BlendShapeKey(BlendShapePreset.U)] = 0.0f,
        };

        private readonly BlendShapeKey[] _keys = new[]
        {
            new BlendShapeKey(BlendShapePreset.A),
            new BlendShapeKey(BlendShapePreset.E),
            new BlendShapeKey(BlendShapePreset.I),
            new BlendShapeKey(BlendShapePreset.O),
            new BlendShapeKey(BlendShapePreset.U),
        };

        private OVRLipSyncContextBase _context;
        private OVRLipSync.Viseme _previousViseme = OVRLipSync.Viseme.sil;
        private float _transitionTimer = 0.0f;
        
        [Inject] private IVRMLoadable _loadable = null;
        
        [Inject] private FaceTracker _faceTracker = null;

        public bool ShouldUpdateMouth { get; set; } = true;
        
        public bool EnableSinCurveOnly { get; set; } = false;

        private bool _animating = false;
        
        private void Start()
        {
            _context = GetComponent<OVRLipSyncContextBase>();
            if (_context == null)
            {
                LogOutput.Instance.Write("同じGameObjectにOVRLipSyncContextBaseを継承したクラスが見つかりません。");
            }
            _context.Smoothing = smoothAmount;

            _loadable.VrmLoaded += info =>
            {
                blendShapeProxy = info.blendShape;
            };
            _loadable.VrmDisposing += () => blendShapeProxy = null;
        }

        private void Update()
        {
            if (blendShapeProxy == null)
            {
                return;
            }
            
            //口閉じの場合: とにかく閉じるのが良いので閉じて終わり
            if (ForceClosedMouth)
            {
                UpdateToClosedMouth();
                return;
            }

            if (_context == null || 
                !_context.enabled || 
                !(_context.GetCurrentPhonemeFrame() is OVRLipSync.Frame frame)
                )
            {
                return;
            }

            _transitionTimer += Time.deltaTime;

            // 最大の重みを持つ音素を探す
            int maxVisemeIndex = 0;
            float maxVisemeWeight = 0.0f;
            // 子音は無視する
            for (var i = (int)OVRLipSync.Viseme.aa; i < frame.Visemes.Length; i++)
            {
                if (frame.Visemes[i] > maxVisemeWeight)
                {
                    maxVisemeWeight = frame.Visemes[i];
                    maxVisemeIndex = i;
                }
            }

            // 音素の重みが小さすぎる場合は口を閉じる
            if (maxVisemeWeight * 100.0f < weightThreshold)
            {
                _transitionTimer = 0.0f;
            }

            // 音素の切り替わりでタイマーをリセットする
            if (_previousViseme != (OVRLipSync.Viseme)maxVisemeIndex)
            {
                _transitionTimer = 0.0f;
                _previousViseme = (OVRLipSync.Viseme)maxVisemeIndex;
            }

            int visemeIndex = maxVisemeIndex - (int)OVRLipSync.Viseme.aa;
            bool hasValidMaxViseme = (visemeIndex >= 0);

            for(int i = 0; i < _keys.Length; i++)
            {
                var key = _keys[i];

                _blendShapeWeights[key] = Mathf.Clamp(Mathf.Lerp(
                    _blendShapeWeights[key],
                    0.0f,
                    Time.deltaTime * cancelSpeedFactor
                    ),0,0.8f);

                //減衰中の値のほうが大きければそっちを採用する。
                //「あぁあぁぁあ」みたいな声の出し方した場合にEvaluateだけ使うとヘンテコになる可能性が高い為。
                if (hasValidMaxViseme && i == visemeIndex)
                {
                    _blendShapeWeights[key] = Mathf.Clamp(Mathf.Max(
                        _blendShapeWeights[key],
                        transitionCurves.Evaluate(_transitionTimer)
                        ),0,0.8f);
                }
            }
           
            /*
            if(keyboardBlendShapeController.IsKeyPressing())
            {
                return;
            }
            */

            if (EnableSinCurveOnly )
            {
                if (!_animating)
                {
                    var detected = _blendShapeWeights.Values.Where(s => s > 0.1).ToList().Count > 0;
               
                    if (detected)
                    {
                        _animating = true;
                        Animate();
                    }
                }

            }
            else
            {
                blendShapeProxy.SetValues(_blendShapeWeights);
            }
        }

        private async void Animate()
        {
            var elapsedTime = 0.0f;
            while (elapsedTime <= 0.5f)
            {
                elapsedTime += Time.deltaTime; 
                var nextVal = ( 0.8f * Mathf.Sin( 2 * Mathf.PI * 4 * elapsedTime) + 0.8f) / 2;
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.A, nextVal); 
                await Task.Yield();
            }
            blendShapeProxy.ImmediatelySetValue(BlendShapePreset.A, 0);
            _animating = false;
        }

        private void UpdateToClosedMouth()
        {
            foreach(var key in _keys)
            {
                _blendShapeWeights[key] = 0.0f;
            }
            blendShapeProxy.SetValues(_blendShapeWeights);
           
            //トラッキング結果を反映
          //  if(!keyboardBlendShapeController.IsKeyPressing()) {
                UpdateMouth();
           // }
        }
        
        
        private float _probA = 0.0f;
        private float _probI = 0.0f;

        private void UpdateMouth()
        {
            if (ShouldUpdateMouth)
            {
                var probA = _faceTracker.FaceParts.MouthOpen.mouthOpenYRatio;
                probA = probA >= 0.7f ? 1.0f : probA >= 0.25f ? 0.5f : 0.0f;
                _probA = Mathf.Lerp(_probA, probA, 0.6f);

                var probI = _faceTracker.FaceParts.MouthOpen.mouthOpenXRatio;
                probI = probI >= 0.8f ? 1.0f : probI >= 0.6f ? 0.5f : 0.0f;
                _probI = Mathf.Lerp(_probI, probI, 0.6f);
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.A, _probA);
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.I, _probI);
            }
            else
            {
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.A, 0);
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.I, 0);
            }
        }
    }
}
