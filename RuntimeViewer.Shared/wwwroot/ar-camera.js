// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.
//
// Camera capture helper for Augmented Reality mode. Opens the device camera via
// getUserMedia, draws frames to an offscreen canvas, and returns encoded JPEG bytes to
// .NET for submission to the local object-recognition server. Kept intentionally simple
// (no WebRTC/streaming) since only single-frame snapshots are needed per detection cycle.

let _stream = null;
let _video = null;
let _canvas = null;
let _ctx = null;

export async function startCamera(videoElementId, facingMode) {
    _video = document.getElementById(videoElementId);
    if (!_video) throw new Error(`Video element '${videoElementId}' not found`);

    _stream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: facingMode || "environment", width: { ideal: 1280 }, height: { ideal: 720 } },
        audio: false
    });
    _video.srcObject = _stream;
    await _video.play();

    _canvas = document.createElement("canvas");
    _ctx = _canvas.getContext("2d", { willReadFrequently: true });

    return { width: _video.videoWidth, height: _video.videoHeight };
}

export function stopCamera() {
    if (_stream) {
        _stream.getTracks().forEach(t => t.stop());
        _stream = null;
    }
    if (_video) {
        _video.srcObject = null;
    }
}

// Pauses the live video feed so the currently displayed frame stays static on screen
// (used by the "Freeze" toolbar button so a user can stabilize the view without moving
// the camera/device while inspecting an overlay). The camera stream itself keeps running
// in the background so resuming is instant.
export function pauseVideo() {
    if (_video) _video.pause();
}

export function resumeVideo() {
    if (_video) _video.play();
}

// Captures the current video frame and returns it as a JPEG Blob (for streaming to .NET via
// IJSStreamReference). Returning a Blob instead of a plain byte array is important for Blazor
// Server: a raw byte[] gets base64-encoded and pushed through the SignalR circuit's JS-interop
// channel, which has a small default message-size limit (~32KB) — a 1280x720 JPEG easily exceeds
// that and the call hangs/fails with no visible exception. Streaming avoids the limit entirely.
export async function captureFrameJpeg(quality) {
    if (!_video || _video.readyState < 2) return null;

    _canvas.width = _video.videoWidth;
    _canvas.height = _video.videoHeight;
    _ctx.drawImage(_video, 0, 0, _canvas.width, _canvas.height);

    const blob = await new Promise(resolve => _canvas.toBlob(resolve, "image/jpeg", quality || 0.7));
    return blob;
}

export function getVideoSize() {
    if (!_video) return { width: 0, height: 0 };
    return { width: _video.videoWidth, height: _video.videoHeight };
}
