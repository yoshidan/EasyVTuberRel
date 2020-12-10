# Unity Native Window

macOS Window Manager for Unity Application.

* Always infront of any other apps including 'Keynote'.
* Observe Keyboard event even if application is background. (accessibility permission is required)
* Transparentize the window.

## Demo 

<img src="./images/demo.gif" width="480" />

## Requirement

* Mojave(macOS 10.14.x) or later.

## How To Use
* download [UnityNativeWindow.unitypackage](./UnityNativeWindow.unitypackage) and import all.

## API 

| class |  method  |  description |
|-------|----------|-----------|
| WindowApi | Frontize | Makes window always infront of any other apps. |
| WindowApi | Transparentize | Makes window background transparent. |
| WindowApi | Opacify | Cancel window transparency. |
| WindowApi | EnableWindowDrag | Makes window movable by dragging. |
| WindowApi | DisableWindowDrag | Cancel window drag. |
| WindowApi | HideTitleBar | Hide title bar. |
| KeyApi | IsKeyEventObservable | Check if accessibility permissions are granted. |
| KeyApi | ObserveKeyDown | Register background key down event handler. |
| KeyApi | ObserveKeyUp | Register background key up event handler. |
| KeyApi | RemoveOnKeyDown | Remove monitor for key down event. |
| KeyApi | RemoveOnKeyUp | Remove monitor for key up event. |
| MouseApi | ObserveLeftMouseDown | Register background left mouse down event handler. |
| MouseApi | ObserveLeftMouseUp | Register background left mouse up event handler. |
| MouseApi | ObserveRightMouseDown | Register background right mouse down event handler. |
| MouseApi | ObserveRightMouseUp | Register background right mouse up event handler. |
| MouseApi | RemoveOnLeftMouseyDown | Remove monitor for left mouse down event. |
| MouseApi | RemoveOnLeftMouseUp | Remove monitor for left mouse up event. |
| MouseApi | RemoveOnRightMouseyDown | Remove monitor for right mouse down event. |
| MouseApi | RemoveOnRightMouseUp | Remove monitor for right mouse up event. |

### Example 

open [Example](./Unity/) by Unity.

```c#
using UnityEngine;
using UnityNativeWindow;

public class SampleController : MonoBehavior {

     private void Start() {
           WindowApi.Frontize();
           WindowApi.HideTitleBar();
           WindowApi.EnableWindowDrag();
           
           Camera.main.clearFlags = CameraClearFlags.Color;
           Camera.main.backgroundColor = Color.clear;
           WindowApi.Transparentize();
            
           MouseApi.ObserveLeftMouseDown(() => Debug.Log("left mouse down"));
           MouseApi.ObserveLeftMouseUp(() => Debug.Log("left mouse up"));
           MouseApi.ObserveRightMouseDown(() => Debug.Log("right mouse down"));
           MouseApi.ObserveRightMouseUp(() => Debug.Log("right mouse up"));
   
           if (KeyApi.IsKeyEventObservable())
           {
               KeyApi.ObserveKeyDown(code => Debug.Log($"key down: {code}"));
               KeyApi.ObserveKeyUp(code => Debug.Log($"key up: {code}"));
           }
     }
     
      private void OnDestroy() {
            MouseApi.Release(); 
            KeyApi.Release();
      }
}

