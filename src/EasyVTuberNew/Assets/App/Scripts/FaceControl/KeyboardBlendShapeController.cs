using System;
using System.Collections.Generic;
using System.Linq;
using App.Main.Scripts.VRMLoad;
using RootMotion.FinalIK;
using UnityEngine;
using VRM;
using Zenject;

namespace App.Main.Scripts.FaceControl
{
    public class KeyboardBlendShapeController : MonoBehaviour
    {
        [Inject] private IVRMLoadable _loadable;

        private VRMBlendShapeProxy _proxy;
        
        private Dictionary<KeyCode, bool> _keyActive = new Dictionary<KeyCode, bool>();

        private void Awake()
        {
            _keyActive[KeyCode.A] = false;
            _keyActive[KeyCode.J] = false;
            _keyActive[KeyCode.F] = false;
            _keyActive[KeyCode.S] = false;
            _keyActive[KeyCode.N] = false;
            _keyActive[KeyCode.E] = false;
            
        }
        
        private void Start()
        {
            _loadable.VrmLoaded += info => _proxy = info.blendShape;
        }

        private void Clear(KeyCode exclude)
        {
            _proxy.ImmediatelySetValue(BlendShapePreset.Neutral, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.A, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.A, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.I, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.U, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.E, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.O, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.Blink, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.Blink_L, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.Blink_R, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.Angry, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.Joy, 0.0f);
            _proxy.ImmediatelySetValue(BlendShapePreset.Fun, 0.0f);
            _proxy.ImmediatelySetValue("Surprised", 0.0f);
            _proxy.ImmediatelySetValue("Extra", 0.0f);
            
            var keyList = new List<KeyCode>(_keyActive.Keys);
            
            foreach (var key in keyList)
            {
                if (key == exclude) continue;
                _keyActive[key] = false;
            }
        }


        public bool IsKeyPressing()
        {
            return _keyActive.Values.Contains(true);
        }
        
        private void LateUpdate()
        {
            if (_proxy == null)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Clear(KeyCode.A);
                if(!_keyActive[KeyCode.A]) _proxy.ImmediatelySetValue(BlendShapePreset.Angry, 1.0f);
                _keyActive[KeyCode.A] = !_keyActive[KeyCode.A];

            }else if (Input.GetKeyDown(KeyCode.J))
            {
                Clear(KeyCode.J);
                if(!_keyActive[KeyCode.J]) _proxy.ImmediatelySetValue(BlendShapePreset.Joy, 1.0f);
                _keyActive[KeyCode.J] = !_keyActive[KeyCode.J];
            }else if (Input.GetKeyDown(KeyCode.F))
            {
                Clear(KeyCode.F);
                if(!_keyActive[KeyCode.F]) _proxy.ImmediatelySetValue(BlendShapePreset.Fun, 1.0f);
                _keyActive[KeyCode.F] = !_keyActive[KeyCode.F];
            }else if (Input.GetKeyDown(KeyCode.S))
            {
                Clear(KeyCode.S);
                if(!_keyActive[KeyCode.S]) _proxy.ImmediatelySetValue("Surprised", 1.0f);
                _keyActive[KeyCode.S] = !_keyActive[KeyCode.S];
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                Clear(KeyCode.E);
                if(!_keyActive[KeyCode.E]) _proxy.ImmediatelySetValue("Extra", 1.0f);
                _keyActive[KeyCode.E] = !_keyActive[KeyCode.E];
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                Clear(KeyCode.N);
                if(!_keyActive[KeyCode.N]) _proxy.ImmediatelySetValue(BlendShapePreset.Neutral, 1.0f);
                _keyActive[KeyCode.N] = !_keyActive[KeyCode.N];
            }
        }
        
    }
}