(() => {
    const duration = 250;
    const motion = matchMedia('(prefers-reduced-motion: reduce)');
    const canvases = new WeakMap();
    const lines = new Set();
    let frame = 0;

    function point(position, reject) {
        if (reject && position >= 64)
            return [788 + (position - 64) * 8, 395 + (position - 64) * 6.2];
        // Bridge the rounded branch anchor without a subpixel jump at the turn.
        if (reject && position > 62)
            return [769 + (position - 62) * 9.5, 398.1 - (position - 62) * 1.55];
        return [180 + position * 9.5, 488 - position * 1.45];
    }

    function draw(parcel, position) {
        parcel.displayed = position;
        const here = point(position, parcel.reject);
        const target = point(parcel.target, parcel.reject);
        parcel.root.setAttribute('transform', `translate(${here[0] - target[0]} ${here[1] - target[1]})`);
    }

    function advance(parcel, now) {
        const progress = Math.min(1, Math.max(0, (now - parcel.started) / duration));
        draw(parcel, parcel.from + (parcel.target - parcel.from) * progress);
        if (progress === 1) parcel.moving = false;
    }

    function tick(now) {
        frame = 0;
        let moving = false;
        for (const line of lines) {
            for (const parcel of line.parcels.values()) {
                if (!parcel.root.isConnected || !parcel.moving) continue;
                advance(parcel, now);
                moving ||= parcel.moving;
            }
        }
        if (moving) frame = requestAnimationFrame(tick);
    }

    function scan() {
        const now = performance.now();
        const seen = new Map();
        for (const root of document.querySelectorAll('[data-conveyor-parcel]')) {
            const canvas = root.ownerSVGElement?.parentElement?.closest('svg');
            if (!canvas) continue;
            const id = root.dataset.conveyorLine;
            let groups = canvases.get(canvas);
            if (!groups) canvases.set(canvas, groups = new Map());
            let line = groups.get(id);
            if (!line) {
                line = { canvas, id, parcels: new Map() };
                groups.set(id, line);
                lines.add(line);
            }
            let slots = seen.get(line);
            if (!slots) seen.set(line, slots = new Set());
            const slot = root.dataset.conveyorParcel;
            slots.add(slot);
            const target = Number(root.dataset.position);
            const reject = root.dataset.route === '1';
            const running = [...canvas.querySelectorAll('[data-conveyor-running]')]
                .some(marker => marker.dataset.conveyorRunning === id);
            let parcel = line.parcels.get(slot);
            const fresh = !parcel || parcel.reject !== reject || target < parcel.target;
            if (fresh) {
                parcel = { target, displayed: target, reject, moving: false };
                line.parcels.set(slot, parcel);
            } else if (parcel.moving) advance(parcel, now);
            const changed = parcel.target !== target;
            parcel.root = root;
            parcel.target = target;
            if (motion.matches || document.hidden || fresh) {
                parcel.moving = false;
                draw(parcel, target);
            } else if (!running) {
                parcel.moving = false;
                draw(parcel, parcel.displayed);
            } else if (changed || (!parcel.moving && parcel.displayed !== target)) {
                parcel.from = parcel.displayed;
                parcel.started = now;
                parcel.moving = true;
                draw(parcel, parcel.displayed);
            } else draw(parcel, parcel.displayed);
        }
        for (const line of lines) {
            const slots = seen.get(line);
            for (const slot of line.parcels.keys())
                if (!slots?.has(slot)) line.parcels.delete(slot);
            if (!line.parcels.size) {
                lines.delete(line);
                canvases.get(line.canvas)?.delete(line.id);
            }
        }
        if (!frame && [...lines].some(line => [...line.parcels.values()].some(p => p.moving)))
            frame = requestAnimationFrame(tick);
    }

    new MutationObserver(records => {
        if (records.some(record => [...record.addedNodes, ...record.removedNodes].some(node =>
            node instanceof Element && (node.matches('[data-conveyor-parcel],[data-conveyor-running]') ||
                node.querySelector('[data-conveyor-parcel],[data-conveyor-running]'))))) scan();
    }).observe(document.documentElement, { childList: true, subtree: true });
    motion.addEventListener('change', scan);
    document.addEventListener('visibilitychange', scan);
    scan();
})();
