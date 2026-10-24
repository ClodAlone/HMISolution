// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

const cameras = new Map();
let observer;

function clearImage(image) {
    // Chromium keeps detached MJPEG requests alive unless their source is cleared.
    image.removeAttribute('src');
}

function update() {
    for (const [image, state] of cameras) {
        if (!image.isConnected) {
            stop(image);
            continue;
        }
        const visible = !image.closest('[hidden]');
        if (visible === state.active) continue;
        state.active = visible;
        if (visible) image.src = state.url;
        else clearImage(image);
    }
}

export function start(image, url) {
    if (!image || !image.isConnected) return;
    const previous = cameras.get(image);
    if (previous?.url === url) return;
    stop(image);
    cameras.set(image, { url, active: false });
    if (!observer) {
        observer = new MutationObserver(update);
        observer.observe(document.documentElement, {
            childList: true, subtree: true, attributes: true, attributeFilter: ['hidden']
        });
    }
    update();
}

export function stop(image) {
    if (!cameras.delete(image)) return;
    clearImage(image);
    if (cameras.size === 0) {
        observer?.disconnect();
        observer = null;
    }
}
