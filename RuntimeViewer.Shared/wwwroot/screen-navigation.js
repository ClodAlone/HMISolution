const workspaces = new WeakMap();
const reducedMotion = () => window.matchMedia('(prefers-reduced-motion: reduce)').matches;
const interactive = 'button, a, input, select, textarea, [contenteditable], [role="slider"], [role="button"], [data-no-swipe], .rv-responsive-interactive, .rv-interactive';

export function attach(root, reference) {
    if (workspaces.has(root)) return;
    const state = { reference, animations: [], gesture: null, suppressUntil: 0, drawer: null, returnFocus: null };
    const direction = (dx, dy) => Math.abs(dx) > Math.abs(dy) ? (dx < 0 ? 'left' : 'right') : (dy < 0 ? 'up' : 'down');
    const opening = { top: 'down', bottom: 'up', left: 'right', right: 'left' };
    const closing = { top: 'up', bottom: 'down', left: 'left', right: 'right' };
    const canSwipe = dir => {
        const drawer = root.querySelector('.rv-layout-drawer');
        if (drawer) return closing[drawer.dataset.drawerEdge] === dir;
        return root.dataset['swipe' + dir[0].toUpperCase() + dir.slice(1)] === 'true' ||
            (state.gesture?.edges.some(edge => opening[edge] === dir && root.querySelector('.rv-layout-handles-' + edge)) ?? false);
    };
    const start = event => {
        state.gesture = null;
        state.suppressUntil = 0;
        if (event.touches.length !== 1 || !(event.target instanceof Element)) return;
        // Handles act as pull tabs: an inward swipe that starts on one opens that panel.
        const handle = event.target.closest('.rv-layout-handle');
        const handleEdge = handle?.closest('.rv-layout-handles')?.dataset.edge;
        if (handle && handleEdge && !root.querySelector('.rv-layout-drawer')) {
            const t = event.touches[0];
            state.gesture = { x: t.clientX, y: t.clientY, id: t.identifier, time: performance.now(), edges: [handleEdge], handle };
            return;
        }
        if (event.target.closest(interactive)) return;
        // Scrollable widgets and actual-size canvases retain their native gestures.
        for (let el = event.target; el && el !== root; el = el.parentElement) {
            const css = getComputedStyle(el);
            if ((/(auto|scroll)/.test(css.overflowY) && el.scrollHeight > el.clientHeight + 1) ||
                (/(auto|scroll)/.test(css.overflowX) && el.scrollWidth > el.clientWidth + 1)) return;
        }
        const touch = event.touches[0];
        const bounds = root.getBoundingClientRect();
        const edges = [];
        if (touch.clientY - bounds.top <= 28) edges.push('top');
        if (bounds.bottom - touch.clientY <= 28) edges.push('bottom');
        if (touch.clientX - bounds.left <= 28) edges.push('left');
        if (bounds.right - touch.clientX <= 28) edges.push('right');
        state.gesture = { x: touch.clientX, y: touch.clientY, id: touch.identifier, time: performance.now(), edges };
    };
    const move = event => {
        if (!state.gesture || event.touches.length !== 1) { state.gesture = null; return; }
        const touch = event.touches[0];
        const dx = touch.clientX - state.gesture.x, dy = touch.clientY - state.gesture.y;
        if (Math.max(Math.abs(dx), Math.abs(dy)) >= 12 && canSwipe(direction(dx, dy)) && event.cancelable)
            event.preventDefault();
    };
    const end = event => {
        const gesture = state.gesture;
        if (!gesture) return;
        const touch = [...event.changedTouches].find(t => t.identifier === gesture.id);
        if (!touch) return;
        const dx = touch.clientX - gesture.x, dy = touch.clientY - gesture.y;
        const major = Math.max(Math.abs(dx), Math.abs(dy)), minor = Math.min(Math.abs(dx), Math.abs(dy));
        const dir = direction(dx, dy);
        const accepted = major >= 64 && major >= minor * 1.35 && performance.now() - gesture.time <= 900 && canSwipe(dir);
        state.gesture = null;
        if (!accepted) return;
        if (event.cancelable) event.preventDefault();
        state.suppressUntil = Date.now() + 450;
        const edge = gesture.edges.find(e => opening[e] === dir) ?? '';
        if (gesture.handle) {
            if (edge && gesture.handle.isConnected) gesture.handle.click();
            return;
        }
        reference.invokeMethodAsync('OnSwipe', dir, edge).catch(error => console.error('Screen swipe navigation failed:', error));
    };
    const cancel = () => { state.gesture = null; };
    const click = event => {
        if (Date.now() < state.suppressUntil && (event.sourceCapabilities?.firesTouchEvents || event.pointerType === 'touch')) {
            event.preventDefault(); event.stopImmediatePropagation();
        }
    };
    const keydown = event => {
        const drawer = root.querySelector('.rv-layout-drawer');
        if (!drawer || event.key !== 'Tab') return;
        const focusable = [...drawer.querySelectorAll('button, a[href], input, select, textarea, [tabindex]')]
            .filter(el => !el.disabled && el.tabIndex >= 0 && el.getClientRects().length);
        const first = focusable[0], last = focusable.at(-1);
        if (event.shiftKey && (document.activeElement === first || document.activeElement === drawer)) {
            event.preventDefault(); (last ?? drawer).focus();
        } else if (!event.shiftKey && document.activeElement === last) {
            event.preventDefault(); (first ?? drawer).focus();
        }
    };
    const preference = window.matchMedia('(prefers-reduced-motion: reduce)');
    const motionChanged = () => {
        if (!preference.matches) return;
        state.animations.forEach(animation => animation.cancel());
        state.drawerAnimation?.cancel();
    };
    preference.addEventListener('change', motionChanged);
    root.addEventListener('touchstart', start, { passive: true });
    root.addEventListener('touchmove', move, { passive: false });
    root.addEventListener('touchend', end, { passive: false });
    root.addEventListener('touchcancel', cancel);
    root.addEventListener('click', click, true);
    root.addEventListener('keydown', keydown);
    state.cleanup = () => {
        preference.removeEventListener('change', motionChanged);
        root.removeEventListener('touchstart', start);
        root.removeEventListener('touchmove', move);
        root.removeEventListener('touchend', end);
        root.removeEventListener('touchcancel', cancel);
        root.removeEventListener('click', click, true);
        root.removeEventListener('keydown', keydown);
    };
    workspaces.set(root, state);
}

export function update(root) {
    const state = workspaces.get(root);
    if (!state) return;
    const drawer = root.querySelector('.rv-layout-drawer');
    const previousFocus = document.activeElement;
    if (!root.querySelector('.rv-screen-outgoing')) {
        state.animations.forEach(animation => animation.cancel());
        state.animations = [];
    }
    for (const child of root.children) {
        if (child.matches('.rv-screen-stage, .rv-layout-edge, .rv-layout-handles')) child.inert = !!drawer;
    }
    if (drawer !== state.drawer) {
        if (drawer) {
            state.returnFocus = previousFocus;
            drawer.querySelector('.rv-layout-close').focus({ preventScroll: true });
        } else if (state.returnFocus?.isConnected) {
            state.returnFocus.focus({ preventScroll: true });
            state.returnFocus = null;
        }
        state.drawer = drawer;
    }
}

export async function animate(root, transition, duration) {
    const state = workspaces.get(root);
    if (!state) return;
    state.animations.forEach(animation => animation.cancel());
    state.animations = [];
    const active = root.querySelector('.rv-screen-active');
    const outgoing = root.querySelector('.rv-screen-outgoing');
    if (!active || !outgoing || reducedMotion() || !duration || transition === 'none') return;
    const vectors = { 'slide-left': ['100%', '0', '-100%', '0'], 'slide-right': ['-100%', '0', '100%', '0'],
        'slide-up': ['0', '100%', '0', '-100%'], 'slide-down': ['0', '-100%', '0', '100%'] };
    const vector = vectors[transition];
    if (!vector && transition !== 'fade') return;
    const options = { duration, easing: 'cubic-bezier(0.22, 0.7, 0.25, 1)', fill: 'forwards' };
    const incomingFrames = vector ? [{ transform: `translate(${vector[0]}, ${vector[1]})` }, { transform: 'translate(0, 0)' }] : [{ opacity: 0 }, { opacity: 1 }];
    const outgoingFrames = vector ? [{ transform: 'translate(0, 0)' }, { transform: `translate(${vector[2]}, ${vector[3]})` }] : [{ opacity: 1 }, { opacity: 0 }];
    const animations = [active.animate(incomingFrames, options), outgoing.animate(outgoingFrames, options)];
    state.animations = animations;
    await Promise.all(animations.map(animation => animation.finished.then(() => {}, error => {
        if (error.name !== 'AbortError') throw error;
    })));
}

export async function closeDrawer(root) {
    const state = workspaces.get(root);
    const drawer = root.querySelector('.rv-layout-drawer');
    if (!state || !drawer || reducedMotion()) return;
    const duration = Math.max(0, Math.min(2000, parseInt(getComputedStyle(drawer).getPropertyValue('--rv-drawer-duration'), 10) || 0));
    if (!duration) return;
    const end = { top: 'translateY(-100%)', bottom: 'translateY(100%)', left: 'translateX(-100%)', right: 'translateX(100%)' }[drawer.dataset.drawerEdge];
    drawer.getAnimations().forEach(animation => animation.cancel());
    const animation = drawer.animate([{ transform: 'translate(0, 0)' }, { transform: end }], { duration, easing: 'ease-in', fill: 'forwards' });
    state.drawerAnimation = animation;
    await animation.finished.then(() => {}, error => { if (error.name !== 'AbortError') throw error; });
}

export function dispose(root) {
    const state = workspaces.get(root);
    if (!state) return;
    state.animations.forEach(animation => animation.cancel());
    state.drawerAnimation?.cancel();
    state.cleanup();
    workspaces.delete(root);
}
