// Converts client (screen) coordinates to SVG viewBox coordinates.
export function clientToSvg(svgElement, clientX, clientY) {
    const pt = svgElement.createSVGPoint();
    pt.x = clientX;
    pt.y = clientY;
    const ctm = svgElement.getScreenCTM();
    if (!ctm) return { x: 0, y: 0 };
    const svgPt = pt.matrixTransform(ctm.inverse());
    return { x: svgPt.x, y: svgPt.y };
}

// --- Pointer-capture drag helper ---
// All pointer-move/up events are handled in JS during drag.
// Only SVG-space coordinates are sent back to .NET at a throttled rate,
// avoiding per-pixel SignalR traffic that causes Blazor Server sluggishness.

let _dragState = null;

export function startDrag(svgElement, dotnetHelper, throttleMs) {
    if (_dragState) stopDrag();
    _dragState = {
        svg: svgElement,
        helper: dotnetHelper,
        throttleMs: throttleMs || 30,
        lastSend: 0,
        pointerId: null
    };

    // Use document-level listeners to catch events even outside the SVG
    document.addEventListener('pointermove', onPointerMove, { passive: true });
    document.addEventListener('pointerup', onPointerUp);
    document.addEventListener('pointercancel', onPointerUp);
}

export function stopDrag() {
    if (!_dragState) return;
    document.removeEventListener('pointermove', onPointerMove);
    document.removeEventListener('pointerup', onPointerUp);
    document.removeEventListener('pointercancel', onPointerUp);
    _dragState = null;
}

function toSvgCoords(svg, clientX, clientY) {
    const pt = svg.createSVGPoint();
    pt.x = clientX;
    pt.y = clientY;
    const ctm = svg.getScreenCTM();
    if (!ctm) return { x: 0, y: 0 };
    const svgPt = pt.matrixTransform(ctm.inverse());
    return { x: svgPt.x, y: svgPt.y };
}

function onPointerMove(e) {
    if (!_dragState) return;
    const s = _dragState;
    const svgPt = toSvgCoords(s.svg, e.clientX, e.clientY);

    const now = performance.now();
    if (now - s.lastSend >= s.throttleMs) {
        s.lastSend = now;
        s.helper.invokeMethodAsync('JsDragMove', svgPt.x, svgPt.y);
    }
}

function onPointerUp(e) {
    if (!_dragState) return;
    const s = _dragState;
    const svgPt = toSvgCoords(s.svg, e.clientX, e.clientY);
    // Send final position then signal end
    s.helper.invokeMethodAsync('JsDragEnd', svgPt.x, svgPt.y);
    stopDrag();
}