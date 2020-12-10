using System;
using App.Main.Scripts.Interprocess;
using App.Main.Scripts.Interprocess.Model;
using App.Main.Scripts.VRMLoad;
using UnityNativeWindow;
using UniRx;
using UnityEngine;
using VRM;
using Zenject;

namespace App.Scripts.Camera
{
    public class CameraController : MonoBehaviour
    {
        [Inject] private ReceivedMessageHandler _receivedMessageHandler;
        
        [Inject] private IVRMLoadable _loadable;

        private VRMFirstPerson _firstPerson;

        private Vector3 initialPosition = Vector3.zero;

        private float _prevRange = 0;
        
        private void Awake()
        {
            initialPosition = transform.position;
            _loadable.VrmLoaded += info => _firstPerson = info.vrmRoot.GetComponent<VRMFirstPerson>();
            _receivedMessageHandler.Commands.Subscribe(message =>
            {
                switch (message.Command)
                {
                    case MessageCommandNames.CameraControl:
                    {
                        enabled = message.ToBoolean();
                        break;
                    }
                }
            });
        }

        private void OnEnable()
        {
             WindowApi.DisableWindowDrag(); 
        }

        private void OnDisable()
        {
            WindowApi.EnableWindowDrag(); 
        }

        private void Update()
        {
            if (_firstPerson != null)
            {
                if (Input.GetMouseButton(0))
                {
                    var angle = new Vector3(Input.GetAxis("Mouse X") * 2, Input.GetAxis("Mouse Y") * 2, 0);

                    var head =
                        _firstPerson.FirstPersonBone.localToWorldMatrix.MultiplyPoint(_firstPerson.FirstPersonOffset);

                    if (Mathf.Abs(angle.x) > 0 || Mathf.Abs(angle.y) > 0)
                    {
                        var avatarPosition = head;
                        transform.RotateAround(avatarPosition, Vector3.up, Mathf.Clamp(angle.x, -30, 30));
                        transform.RotateAround(avatarPosition, Vector3.right, Mathf.Clamp(angle.y, -30, 30));
                    } 
                }else if (Input.GetMouseButton(1)) {
                    //福ボタン選択時には移動縦移動可能にする
                    var angle = new Vector3(0, Input.GetAxis("Mouse Y") * 2, 0);
                    if (Math.Abs(angle.y) > 0)
                    {
                        var currentPosition = transform.position;
                        currentPosition.y -= (angle.y * 0.1f);
                        transform.position = currentPosition;
                    }
                }
                else
                {
                    float scroll = Input.GetAxis("Mouse ScrollWheel");
                    if (Math.Abs(scroll) > 0) 
                    {
                        var t = transform;
                        var nextPosition = t.position + t.forward * scroll;
                        var range = (nextPosition - initialPosition).magnitude;
                        if (range < _prevRange || range <= (scroll > 0 ? 1.0f : .4f))
                        {
                            Debug.Log(range.ToString("F3") + ","+ _prevRange.ToString("F3"));
                            transform.position = nextPosition;
                            _prevRange = range;
                        }
                    }
                }
            }
        }
    }
}