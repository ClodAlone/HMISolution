(() => {
    const duration = 400;
    const hosts = new WeakMap();
    const tanks = new Set();
    const active = new Set();
    const motion = matchMedia('(prefers-reduced-motion: reduce)');
    const numbers = /-?\d*\.?\d+(?:e[-+]?\d+)?/gi;
    let frame = 0;

    function current(tank, now) {
        const progress = Math.min(1, Math.max(0, (now - tank.started) / duration));
        const eased = progress * progress * (3 - 2 * progress);
        return tank.from + (tank.target - tank.from) * eased;
    }

    function draw(tank, level) {
        const root = tank.root;
        root.dataset.displayedLevel = String(level);
        const label = root.querySelector('.tank-level');
        if (label) {
            const text = level === tank.target ? tank.label : `${Math.round(level * 100)}%`;
            if (label.textContent !== text) label.textContent = text;
        }
        let liquid = root.querySelector('.tank-liquid');
        if (level <= 0) {
            if (liquid) liquid.remove();
            return;
        }
        if (!liquid) {
            liquid = tank.template.cloneNode(true);
            root.insertBefore(liquid, root.querySelector('.tank-sheen, .tank-outline, .tank-rim, .tank-level'));
        }
        const keys = tank.keys;
        let next = 1;
        while (next < keys.length - 1 && level > keys[next].level) next++;
        const start = keys[next - 1], end = keys[next];
        const fraction = (level - start.level) / (end.level - start.level);
        for (let part = 0; part < liquid.children.length; part++) {
            for (const [name, value] of Object.entries(start.parts[part])) {
                const a = value.match(numbers).map(Number);
                const b = end.parts[part][name].match(numbers).map(Number);
                let index = 0;
                const result = value.replace(numbers, () => {
                    const n = index++;
                    return String(a[n] + (b[n] - a[n]) * fraction);
                });
                liquid.children[part].setAttribute(name, result);
            }
        }
    }

    function tick(now) {
        frame = 0;
        for (const tank of active) {
            if (!tank.root.isConnected) {
                active.delete(tank);
                continue;
            }
            const done = now - tank.started >= duration;
            draw(tank, done ? tank.target : current(tank, now));
            if (done) active.delete(tank);
        }
        if (active.size) frame = requestAnimationFrame(tick);
    }

    function attach(root) {
        const host = root.parentElement;
        const id = root.dataset.tankId;
        if (!root.isConnected || !host || !id) return;
        let instances = hosts.get(host);
        if (!instances) hosts.set(host, instances = new Map());
        let tank = instances.get(id);
        if (tank?.root === root) return;
        const now = performance.now();
        const target = Number(root.dataset.level);
        const ready = root.dataset.state === 'ready';
        const previous = tank && tank.ready ? (active.has(tank) ? current(tank, now) : tank.target) : target;
        const changed = tank && tank.target !== target;
        const animate = tank && tank.ready && ready && !motion.matches && !document.hidden;
        if (!tank) {
            tank = { from: target, target, started: now };
            instances.set(id, tank);
            tanks.add(tank);
        }
        tank.root = root;
        tank.host = host;
        tank.id = id;
        tank.ready = ready;
        tank.label = root.querySelector('.tank-level')?.textContent;
        tank.keys = JSON.parse(root.dataset.tankKeyframes);
        tank.template = new DOMParser().parseFromString(root.dataset.tankTemplate, 'image/svg+xml').documentElement;
        if (!animate) {
            tank.from = tank.target = target;
            active.delete(tank);
        } else if (changed) {
            tank.from = previous;
            tank.target = target;
            tank.started = now;
            active.add(tank);
        }
        if (ready) draw(tank, active.has(tank) ? current(tank, now) : target);
        if (active.size && !frame) frame = requestAnimationFrame(tick);
    }

    function find(node) {
        if (!(node instanceof Element)) return;
        if (node.matches('.hmi-tank')) attach(node);
        node.querySelectorAll('.hmi-tank').forEach(attach);
    }

    // MarkupString replaces the SVG on a live update; keep progress on its stable host, not on the removed SVG.
    new MutationObserver(records => {
        for (const record of records) record.addedNodes.forEach(find);
        for (const tank of tanks) {
            if (tank.root.isConnected) continue;
            active.delete(tank);
            tanks.delete(tank);
            hosts.get(tank.host).delete(tank.id);
        }
        if (!active.size && frame) {
            cancelAnimationFrame(frame);
            frame = 0;
        }
    }).observe(document.documentElement, { childList: true, subtree: true });

    function finish() {
        if (!motion.matches && !document.hidden) return;
        for (const tank of active) draw(tank, tank.target);
        active.clear();
        cancelAnimationFrame(frame);
        frame = 0;
    }
    motion.addEventListener('change', finish);
    document.addEventListener('visibilitychange', finish);
    find(document.documentElement);
})();
