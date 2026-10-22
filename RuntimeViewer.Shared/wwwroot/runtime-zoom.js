const initialPixelRatio = window.devicePixelRatio;

function updateRuntimeViewport() {
    const root = document.documentElement;
    const zoom = window.devicePixelRatio / initialPixelRatio;
    const zoomed = Math.abs(zoom - 1) > 0.001;

    // Auto-fit must not cancel page zoom by fitting to the smaller CSS viewport.
    if (zoomed) {
        root.style.setProperty('--rv-viewport-width', `${window.innerWidth * zoom}px`);
        root.style.setProperty('--rv-viewport-height', `${window.innerHeight * zoom}px`);
    } else {
        root.style.removeProperty('--rv-viewport-width');
        root.style.removeProperty('--rv-viewport-height');
    }
    root.toggleAttribute('data-rv-page-zoom', zoomed);
}

window.addEventListener('resize', updateRuntimeViewport);
updateRuntimeViewport();
