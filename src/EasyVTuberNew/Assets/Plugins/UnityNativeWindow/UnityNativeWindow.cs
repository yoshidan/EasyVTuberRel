using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityNativeWindow {
    class Bundle {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        public const string Target = "UnityNativeWindow";
#endif
    }

    public class MouseApi
    {
        public delegate void OnLeftMouseDown();
        private delegate void OnLeftMouseDownNative(IntPtr methodHandle);
        
        public delegate void OnLeftMouseUp();
        private delegate void OnLeftMouseUpNative(IntPtr methodHandle);
        
        public delegate void OnRightMouseDown();
        private delegate void OnRightMouseDownNative(IntPtr methodHandle);
        
        public delegate void OnRightMouseUp();
        private delegate void OnRightMouseUpNative(IntPtr methodHandle);
        
        [DllImport(Bundle.Target)]
        private static extern void removeOnLeftMouseDown();
        
        [DllImport(Bundle.Target)]
        private static extern void removeOnLeftMouseUp();
        
        [DllImport(Bundle.Target)]
        private static extern void removeOnRightMouseDown();
        
        [DllImport(Bundle.Target)]
        private static extern void removeOnRightMouseUp();

        [DllImport(Bundle.Target)]
        private static extern void onLeftMouseDown(IntPtr methodHandle, OnLeftMouseDownNative callback);
        
        [DllImport(Bundle.Target)]
        private static extern void onLeftMouseUp(IntPtr methodHandle, OnLeftMouseUpNative callback);
        
        [DllImport(Bundle.Target)]
        private static extern void onRightMouseDown(IntPtr methodHandle, OnRightMouseDownNative callback);
        
        [DllImport(Bundle.Target)]
        private static extern void onRightMouseUp(IntPtr methodHandle, OnRightMouseUpNative callback); 
        
        private static GCHandle? _leftMouseDownGcHandle;
        private static GCHandle? _leftMouseUpGcHandle;
        private static GCHandle? _rightMouseDownGcHandle;
        private static GCHandle? _rightMouseUpGcHandle;

         /// <summary>Remove monitor for left mouse down event.</summary>
        public static void RemoveOnLeftMouseDown()
        {
            _leftMouseDownGcHandle?.Free();
            _leftMouseDownGcHandle = null;
           removeOnLeftMouseDown();
        }
        
         /// <summary>Remove monitor for left mouse up event.</summary>
        public static void RemoveOnLeftMouseUp()
        {
            _leftMouseUpGcHandle?.Free();
            _leftMouseUpGcHandle = null;
            removeOnLeftMouseUp();
        }
        
         /// <summary>Remove monitor for right mouse down event.</summary>
        public static void RemoveOnRightMouseDown()
        {
            _rightMouseDownGcHandle?.Free();
            _rightMouseDownGcHandle = null;
            removeOnRightMouseDown();
        }
        
         /// <summary>Remove monitor for right mouse up event.</summary>
        public static void RemoveOnRightMouseUp()
        {
            _rightMouseUpGcHandle?.Free();
            _rightMouseUpGcHandle = null;
            removeOnRightMouseUp();
        }

        /// <summary>Register background left mouse down event handler.</summary>
        /// <param name="callback">callback function </param>
        public static void ObserveLeftMouseDown(OnLeftMouseDown callback)
        {
            RemoveOnLeftMouseDown();
            _leftMouseDownGcHandle = GCHandle.Alloc(callback, GCHandleType.Normal);
            onLeftMouseDown((IntPtr)_leftMouseDownGcHandle.Value, OnLeftMouseDownFromNative);
        }
        
        [AOT.MonoPInvokeCallbackAttribute(typeof(OnLeftMouseDownNative))]
        private static void OnLeftMouseDownFromNative(IntPtr methodHandle){
            var handle = (GCHandle)methodHandle;
            var callback = handle.Target as OnLeftMouseDown;
            callback?.Invoke();
        }
        
        /// <summary>Register background left mouse up event handler.</summary>
        /// <param name="callback">callback function </param>
        public static void ObserveLeftMouseUp(OnLeftMouseUp callback)
        {
            RemoveOnLeftMouseUp();
            _leftMouseUpGcHandle = GCHandle.Alloc(callback, GCHandleType.Normal);
            onLeftMouseUp((IntPtr)_leftMouseUpGcHandle.Value, OnLeftMouseUpFromNative);
        }
        
        [AOT.MonoPInvokeCallbackAttribute(typeof(OnLeftMouseUpNative))]
        private static void OnLeftMouseUpFromNative(IntPtr methodHandle){
            var handle = (GCHandle)methodHandle;
            var callback = handle.Target as OnLeftMouseUp;
            callback?.Invoke();
        }
        
        /// <summary>Register background right mouse down event handler.</summary>
        /// <param name="callback">callback function </param>
        public static void ObserveRightMouseDown(OnRightMouseDown callback)
        {
            RemoveOnRightMouseDown();
            _rightMouseDownGcHandle = GCHandle.Alloc(callback, GCHandleType.Normal);
            onRightMouseDown((IntPtr)_rightMouseDownGcHandle.Value, OnRightMouseDownFromNative);
        }
        
        [AOT.MonoPInvokeCallbackAttribute(typeof(OnRightMouseDownNative))]
        private static void OnRightMouseDownFromNative(IntPtr methodHandle){
            var handle = (GCHandle)methodHandle;
            var callback = handle.Target as OnRightMouseDown;
            callback?.Invoke();
        }
        
        /// <summary>Register background right mouse up event handler.</summary>
        /// <param name="callback">callback function </param>
        public static void ObserveRightMouseUp(OnRightMouseUp callback)
        {
            RemoveOnRightMouseUp(); 
            _rightMouseUpGcHandle = GCHandle.Alloc(callback, GCHandleType.Normal);
            onRightMouseUp((IntPtr)_rightMouseUpGcHandle.Value, OnRightMouseUpFromNative);
        }
        
        [AOT.MonoPInvokeCallbackAttribute(typeof(OnRightMouseUpNative))]
        private static void OnRightMouseUpFromNative(IntPtr methodHandle){
            var handle = (GCHandle)methodHandle;
            var callback = handle.Target as OnRightMouseUp;
            callback?.Invoke();
        }

        /// <summary>Remove all motitors.</summary>
        public static void Release()
        {
            RemoveOnLeftMouseDown(); 
            RemoveOnLeftMouseUp(); 
            RemoveOnRightMouseDown(); 
            RemoveOnRightMouseUp(); 
        }
    }

    public class KeyApi
    {
        public delegate void OnKeyDown(string keyCode);
        private delegate void OnKeyDownNative(string keyCode, IntPtr methodHandle);
        
        public delegate void OnKeyUp(string keyCode);
        private delegate void OnKeyUpNative(string keyCode, IntPtr methodHandle); 
        
        [DllImport(Bundle.Target)]
        private static extern void removeOnKeyDown();
        
        [DllImport(Bundle.Target)]
        private static extern void removeOnKeyUp();
        
        [DllImport(Bundle.Target)]
        private static extern void onKeyDown(IntPtr methodHandle, OnKeyDownNative callback);
        
        [DllImport(Bundle.Target)]
        private static extern void onKeyUp(IntPtr methodHandle, OnKeyUpNative callback);
        
        private static GCHandle? _keyDownGcHandle;
        private static GCHandle? _keyUpGcHandle;

        [DllImport(Bundle.Target)]
        private static extern bool isKeyEventObservable();

         /// <summary>Check if accessibility permissions are granted.</summary>
         /// <returns>true: accessible</returns>
        public static bool IsKeyEventObservable()
        {
            return isKeyEventObservable();
        }
       
         /// <summary>Remove monitor for key down event.</summary>
        public static void RemoveOnKeyDown()
        {
            _keyDownGcHandle?.Free();
            _keyDownGcHandle = null;
            removeOnKeyDown();
        }
        
         /// <summary>Remove monitor for key up event.</summary>
        public static void RemoveOnKeyUp()
        {
            _keyUpGcHandle?.Free();
            _keyUpGcHandle = null;
            removeOnKeyUp();
        }

        /// <summary>Register background key down event handler.</summary>
        /// <param name="callback">callback function </param>
        public static void ObserveKeyDown(OnKeyDown callback)
        {
            RemoveOnKeyDown();
            _keyDownGcHandle = GCHandle.Alloc(callback, GCHandleType.Normal);
            onKeyDown((IntPtr)_keyDownGcHandle.Value, OnKeyKeyFromNative);
        }
        
        [AOT.MonoPInvokeCallbackAttribute(typeof(OnKeyDownNative))]
        private static void OnKeyKeyFromNative(string keyCode, IntPtr methodHandle){
            var handle = (GCHandle)methodHandle;
            var callback = handle.Target as OnKeyDown;
            callback?.Invoke(keyCode);
        }
        
        /// <summary>Register background key up event handler.</summary>
        /// <param name="callback">callback function </param>
        public static void ObserveKeyUp(OnKeyUp callback)
        {
            RemoveOnKeyUp();
            _keyUpGcHandle = GCHandle.Alloc(callback, GCHandleType.Normal);
            onKeyUp((IntPtr)_keyUpGcHandle.Value, OnKeyUpFromNative);
        }
        
        [AOT.MonoPInvokeCallbackAttribute(typeof(OnKeyUpNative))]
        private static void OnKeyUpFromNative(string keyCode, IntPtr methodHandle){
            var handle = (GCHandle)methodHandle;
            var callback = handle.Target as OnKeyUp;
            callback?.Invoke(keyCode);
        }
        
        /// <summary>Remove all monitors.</summary>
        public static void Release()
        {
            RemoveOnKeyDown();
            RemoveOnKeyUp();
        }
    }
    
    public class WindowApi
    {
        [DllImport(Bundle.Target)]
        private static extern void enableWindowDrag();
        
        [DllImport(Bundle.Target)]
        private static extern void disableWindowDrag();
        
        [DllImport(Bundle.Target)]
        private static extern void frontize();
        
        [DllImport(Bundle.Target)]
        private static extern void transparentize();
        
        [DllImport(Bundle.Target)]
        private static extern void opacify();
        
        [DllImport(Bundle.Target)]
        private static extern void hideTitleBar();
        
        /// <summary>Makes window movable by dragging.</summary>
        public static void EnableWindowDrag()
        {
            enableWindowDrag();
        }
        
        /// <summary> Cancel window drag.</summary>
        public static void DisableWindowDrag()
        {
            disableWindowDrag();
        }
        
        /// <summary> Makes window always infront of any other apps.</summary>
        public static void Frontize()
        {
            frontize();
        }
       
        /// <summary> Hide title bar. </summary>
        public static void HideTitleBar()
        {
            hideTitleBar();
        }
        
        /// <summary> Makes window background transparent.</summary>
        public static void Transparentize()
        {
            transparentize();
        }
        
        /// <summary> Cancel window transparency. </summary>
        public static void Opacify()
        {
            opacify();
        }
    }
}