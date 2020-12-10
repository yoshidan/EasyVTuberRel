//
//  NativePlugin.m
//  UnityNativeWindow
//
//  Created by Naohiro Yoshida on 2020/06/26.
//  Copyright © 2020 yoshidan. All rights reserved.
//

#include <stdio.h>
#import <Foundation/Foundation.h>
#import <Cocoa/Cocoa.h>
#import "NativePlugin.h"


id onLeftMouseDownHandler = NULL;
id onLeftMouseUpHandler = NULL;
id onRightMouseDownHandler = NULL;
id onRightMouseUpHandler = NULL;
id onKeyDownHandler = NULL;
id onKeyUpHandler = NULL;

void enableWindowDrag() {
    NSWindow *window = [NSApp orderedWindows][0];
    window.movableByWindowBackground = true;
}

void disableWindowDrag() {
    NSWindow *window = [NSApp orderedWindows][0];
    window.movableByWindowBackground = false;
}

void frontize() {
    NSWindow *window = [NSApp orderedWindows][0];
    window.collectionBehavior = NSWindowCollectionBehaviorCanJoinAllSpaces | NSWindowCollectionBehaviorFullScreenAuxiliary;
    window.level = NSPopUpMenuWindowLevel;

}

void hideTitleBar() {
    NSWindow* window = [NSApp orderedWindows][0];
    window.styleMask = NSWindowStyleMaskBorderless | NSWindowStyleMaskResizable;
    window.titlebarAppearsTransparent = false;
    window.titleVisibility = NSWindowTitleHidden;
}

void transparentize() {
    NSWindow* window = [NSApp orderedWindows][0];
    
    window.backgroundColor = [NSColor clearColor];
    window.opaque = false;
    window.hasShadow = false;
    
    NSView* view = window.contentView;
    if(view) {
        view.wantsLayer = true;
        view.layer.backgroundColor = CGColorCreateGenericRGB(0, 0, 0, 0);
        view.layer.opaque = false;
    }
}

void opacify() {
    NSWindow* window = [NSApp orderedWindows][0];
    NSView* view = window.contentView;
    if(view) {
        view.wantsLayer = false;
        view.layer.backgroundColor = CGColorCreateGenericRGB(1,1,1,1);
        view.layer.opaque = true;
    }
}

bool isKeyEventObservable(){
    NSDictionary* options = @{(__bridge id) kAXTrustedCheckOptionPrompt: @YES};
    return AXIsProcessTrustedWithOptions((__bridge CFDictionaryRef )options);
}

void removeOnKeyDown() {
    if (onKeyDownHandler != NULL) {
        [NSEvent removeMonitor:onKeyDownHandler];
        onKeyDownHandler = NULL;
    }
}

void onKeyDown(void* methodHandle, OnKeyDown callback) {
    removeOnKeyDown();
    onKeyDownHandler = [NSEvent addGlobalMonitorForEventsMatchingMask:(NSEventMaskKeyDown) handler:^(NSEvent *event){
        callback([[event characters] UTF8String], methodHandle);
    }];
}

void removeOnKeyUp() {
    if (onKeyUpHandler != NULL) {
        [NSEvent removeMonitor:onKeyUpHandler];
        onKeyUpHandler = NULL;
    }
}

void onKeyUp(void* methodHandle, OnKeyUp callback) {
    removeOnKeyUp();
    onKeyUpHandler = [NSEvent addGlobalMonitorForEventsMatchingMask:(NSEventMaskKeyUp) handler:^(NSEvent *event){
        callback([[event characters] UTF8String], methodHandle);
    }];
}

void removeOnLeftMouseDown() {
    if (onLeftMouseDownHandler != NULL) {
        [NSEvent removeMonitor:onLeftMouseDownHandler];
        onLeftMouseDownHandler = NULL;
    }
}

void onLeftMouseDown(void* methodHandle, OnLeftMouseDown callback) {
    removeOnLeftMouseDown();
    onLeftMouseDownHandler = [NSEvent addGlobalMonitorForEventsMatchingMask:(NSEventMaskLeftMouseDown) handler:^(NSEvent *event){
        callback(methodHandle);
    }];
}

void removeOnLeftMouseUp() {
    if (onLeftMouseUpHandler != NULL) {
        [NSEvent removeMonitor:onLeftMouseUpHandler];
        onLeftMouseUpHandler = NULL;
    }
}

void onLeftMouseUp(void* methodHandle, OnLeftMouseUp callback) {
    removeOnLeftMouseUp();
    onLeftMouseUpHandler = [NSEvent addGlobalMonitorForEventsMatchingMask:(NSEventMaskLeftMouseUp) handler:^(NSEvent *event){
        callback(methodHandle);
    }];
    
}

void removeOnRightMouseDown() {
    if (onRightMouseDownHandler != NULL) {
        [NSEvent removeMonitor:onRightMouseDownHandler];
        onRightMouseDownHandler = NULL;
    }
}

void onRightMouseDown(void* methodHandle, OnRightMouseDown callback) {
    removeOnRightMouseDown();
    onRightMouseDownHandler = [NSEvent addGlobalMonitorForEventsMatchingMask:(NSEventMaskRightMouseDown) handler:^(NSEvent *event){
        callback(methodHandle);
    }];
}

void removeOnRightMouseUp() {
    if (onRightMouseUpHandler != NULL) {
        [NSEvent removeMonitor:onRightMouseUpHandler];
        onRightMouseUpHandler = NULL;
    }
}

void onRightMouseUp(void* methodHandle, OnRightMouseUp callback) {
    removeOnRightMouseUp();
    onRightMouseUpHandler = [NSEvent addGlobalMonitorForEventsMatchingMask:(NSEventMaskRightMouseUp) handler:^(NSEvent *event){
        callback(methodHandle);
    }];
}


