export function enterFullscreen() {
    const el = document.documentElement;
    const rfs = el.requestFullscreen
        || el.webkitRequestFullscreen
        || el.msRequestFullscreen;
    if (rfs) {
        return rfs.call(el).catch(() => { });
    }
}

export function exitFullscreen() {
    const efs = document.exitFullscreen
        || document.webkitExitFullscreen
        || document.msExitFullscreen;
    if (efs && document.fullscreenElement) {
        return efs.call(document).catch(() => { });
    }
}

export function isFullscreen() {
    return !!(document.fullscreenElement
        || document.webkitFullscreenElement
        || document.msFullscreenElement);
}
