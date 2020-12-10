//
//  NativePlugin.h
//  UnityNativeWindow
//
//  Created by Naohiro Yoshida on 2020/06/26.
//  Copyright © 2020 yoshidan. All rights reserved.
//

#ifndef NativePlugin_h
#define NativePlugin_h

#ifdef __cplusplus
extern "C" {
#endif

typedef void (*OnLeftMouseDown)(void *methodHandle);

typedef void (*OnLeftMouseUp)(void *methodHandle);

typedef void (*OnRightMouseDown)(void *methodHandle);

typedef void (*OnRightMouseUp)(void *methodHandle);

typedef void (*OnKeyDown)(const char *keyCode, void *methodHandle);

typedef void (*OnKeyUp)(const char *keyCode, void *methodHandle);

/**
 * Makes window movable by dragging.
 */
void enableWindowDrag(void);

/**
 * Cancel window drag.
 */
void disableWindowDrag(void);

/**
 * Makes window always infront of any other apps.
 */
void frontize(void);
    
/**
 * Hide title bar.
 */
void hideTitleBar(void);

/**
 * Makes window background transparent.
 */
void transparentize(void);

/**
 * Cancel window transparency.
 */
void opacify(void);

/**
 * Check if accessibility permissions are granted.
 */
bool isKeyEventObservable(void);

/**
 * Remove monitor for key down event.
 */
void removeOnKeyDown(void);

/**
 * Register background key down event handler.
 *
 * @param methodHandle pointer of callee
 * @param callback callback function
 */
void onKeyDown(void *methodHandle, OnKeyDown callback);

/**
 * Remove monitor for key up event.
 */
void removeOnKeyUp(void);

/**
 * Register background key up event handler.
 *
 * @param methodHandle pointer of callee
 * @param callback callback function
 */
void onKeyUp(void *methodHandle, OnKeyUp callback);

/**
 * Remove monitor for left mouse down event.
 */
void removeOnLeftMouseDown(void);

/**
 * Register background left mouse down event handler.
 *
 * @param methodHandle pointer of callee
 * @param callback callback function
 */
void onLeftMouseDown(void *methodHandle, OnLeftMouseDown callback);

/**
 * Remove monitor for left mouse up event.
 */
void removeOnLeftMouseUp(void);

/**
 * Register background left mouse up event handler.
 *
 * @param methodHandle pointer of callee
 * @param callback callback function
 */
void onLeftMouseUp(void *methodHandle, OnLeftMouseUp callback);

/**
 * Remove monitor for right mouse down event.
 */
void removeOnRightMouseDown(void);

/**
 * Register background right mouse down event handler.
 *
 * @param methodHandle pointer of callee
 * @param callback callback function
 */
void onRightMouseDown(void *methodHandle, OnRightMouseDown callback);

/**
 * Remove monitor for right mouse up event.
 */
void removeOnRightMouseUp(void);

/**
 * Register background right mouse down event handler.
 *
 * @param methodHandle pointer of callee
 * @param callback callback function
 */
void onRightMouseUp(void *methodHandle, OnRightMouseUp callback);

#ifdef __cplusplus
}
#endif

#endif /* NativePlugin_h */
