using App.Main.Scripts.Interprocess;
using App.Main.Scripts.Interprocess.Model;
using UniRx;
using UnityEngine;
using Zenject;

namespace App.Main.Scripts.HumanInterfaceDevices
{
    public class TouchPadProvider : MonoBehaviour
    {

        private float _screenWidth = 1024f;
        private float _screenHeight = 768f;
        
        [Inject] private ReceivedMessageHandler _messageHandler;
        
        private void Start()
        {
            //var res = Screen.currentResolution;
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            
            foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = HIDMaterialUtil.Instance.GetPadMaterial();
            }
            
            var initialLocalScale = transform.localScale;
            _messageHandler.Commands.Subscribe(message =>
            {
                if (message.Command == MessageCommandNames.EnableDesktopShareMode)
                {
                    if (message.ToBoolean())
                    {
                        transform.localScale = initialLocalScale;
                    }
                    else
                    {
                        transform.localScale = Vector3.zero;
                    }
                }
            });
            transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Vector3 GetHandTipPosFromScreenPoint()
        {
            
            var mouse = Input.mousePosition;
            var x = Mathf.Clamp((mouse.x - _screenWidth / 2) / _screenWidth, -0.5f, 0.5f);
            var y = Mathf.Clamp((mouse.y - _screenHeight / 2) / _screenHeight, -0.5f, 0.5f);
            
            var cursorPosInVirtualScreen = new Vector2(x,y);
            //Debug.Log("mouse:" + cursorPosInVirtualScreen.ToString() + mouse + new Vector2(_screenWidth, _screenHeight));
            //NOTE: 0.95をかけて何が嬉しいかというと、パッドのギリギリのエリアを避けてくれるようになります
            return transform.TransformPoint(cursorPosInVirtualScreen * 0.8f);
        }

        /// <summary>
        /// <see cref="GetHandTipPosFromScreenPoint"/>の結果、またはその結果をローパスした座標を指定することで、
        /// その座標にVRMの右手を持っていくときの望ましいワールド回転値を計算し、取得します。
        /// </summary>
        /// <returns></returns>
        public Quaternion GetWristRotation(Vector3 pos)
            => transform.rotation *
               Quaternion.AngleAxis(-90, Vector3.right) * 
               Quaternion.AngleAxis(-90, Vector3.up);

        /// <summary>
        /// 手首から指までの位置を考慮するオフセットベクトルを、オフセットの量を指定して取得します。
        /// </summary>
        /// <param name="yOffset"></param>
        /// <param name="palmToTipLength"></param>
        /// <returns></returns>
        public Vector3 GetOffsetVector(float yOffset, float palmToTipLength)
        {
            var t = transform;
            return (-yOffset) * t.forward + (-palmToTipLength) * t.up;

        }
    }
}