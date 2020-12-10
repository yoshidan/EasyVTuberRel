using UnityEngine;
using UnityNativeWindow;

namespace Example
{
    public class SampleController : MonoBehaviour
    {

        public bool transparent = false;
        public bool monitoring = false;

        void Start()
        {
#if !UNITY_EDITOR
            WindowApi.Frontize();
            WindowApi.HideTitleBar();
            WindowApi.EnableWindowDrag();
#endif
            if (KeyApi.IsKeyEventObservable())
            {
                Debug.Log("accessibility OK");
            }

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
#if !UNITY_EDITOR
                if (transparent)
                {
                    Camera.main.backgroundColor = Color.green;
                    WindowApi.Opacify();
                }
                else
                {
                    Camera.main.clearFlags = CameraClearFlags.Color;
                    Camera.main.backgroundColor = Color.clear;
                    WindowApi.Transparentize();
                }
#endif
                
                transparent = !transparent;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (monitoring)
                {
                    MouseApi.Release(); 
                    KeyApi.Release();
                }
                else
                {
                    MouseApi.ObserveLeftMouseDown(() => Debug.Log("left mouse down"));
                    MouseApi.ObserveLeftMouseUp(() => Debug.Log("left mouse up"));
                    MouseApi.ObserveRightMouseDown(() => Debug.Log("right mouse down"));
                    MouseApi.ObserveRightMouseUp(() => Debug.Log("right mouse up"));
                    
                    // accessibility permission is required 
                    KeyApi.ObserveKeyDown(code => Debug.Log($"key down: {code}"));
                    KeyApi.ObserveKeyUp(code => Debug.Log($"key up: {code}"));
                }

                monitoring = !monitoring;
            }
        }
    
        private void OnDestroy() {
            Debug.Log("Clear");
            MouseApi.Release(); 
            KeyApi.Release();
        }
    }
}