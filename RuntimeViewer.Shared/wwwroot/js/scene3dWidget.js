// Scene3D Widget — Three.js interop for 3D shapes / glTF model viewer with animation
const _scenes = {};
let _threeLoaded = false;
let _threeLoading = null;

// Single shared requestAnimationFrame driver for ALL scene3d instances on the
// page. Running N independent rAF loops (one per widget) means N separate
// WebGL draw-call submissions interleaved by the browser every frame, which
// is far more expensive than batching them in one tick — this was the main
// cause of high CPU / frozen UI when a screen has several 3D widgets.
// Scenes are also paused when off-screen (IntersectionObserver) or when the
// browser tab is hidden (visibilitychange), and skip rendering entirely if
// nothing changed (no auto-rotate, no manual rotation, controls idle).
let _driverRunning = false;
let _lastDriverTime = performance.now();
const FRAME_INTERVAL_MS = 1000 / 24; // ~24fps is plenty for slow rotation/orbit

function driverTick(time) {
    if (Object.keys(_scenes).length === 0) { _driverRunning = false; return; }
    requestAnimationFrame(driverTick);

    if (time - _lastDriverTime < FRAME_INTERVAL_MS) return;
    const dt = (time - _lastDriverTime) / 1000;
    _lastDriverTime = time;

    for (const id in _scenes) {
        const s = _scenes[id];
        if (s.disposed || !s.visible || document.hidden) continue;

        let dirty = false;
        if (s.manualRotationDeg != null) {
            s.group.rotation.y = s.THREE.MathUtils.degToRad(s.manualRotationDeg);
            dirty = true;
        } else if (s.autoRotate) {
            s.group.rotation.y += s.THREE.MathUtils.degToRad(s.rotateSpeed * dt);
            dirty = true;
        }
        if (s.controls) {
            s.controls.update();
            dirty = true; // damping may still be settling
        }
        if (applyPartTargets(s, dt)) dirty = true;
        if (dirty || s.forceRender) {
            s.renderer.render(s.scene, s.camera);
            s.forceRender = false;
        }
    }
}

// Advances each bound part towards its target value (instant or smoothed via
// exponential easing) and applies it to the matching Object3D's rotation,
// position, scale, or material color. Returns true if anything changed so the
// caller knows a render is needed.
function applyPartTargets(s, dt) {
    if (!s.partTargets) return false;
    let changed = false;
    const SMOOTH_RATE = 6; // higher = snappier easing

    for (const bindingId in s.partTargets) {
        const t = s.partTargets[bindingId];
        const part = s.parts && s.parts[t.partNameLower];
        if (!part) continue;

        if (t.property === 'color') {
            if (t.color != null && part.colorableMaterial) {
                part.colorableMaterial.color.set(t.color);
                changed = true;
            }
            continue;
        }

        if (t.value == null) continue;
        if (t.current == null) t.current = t.value;
        const diff = t.value - t.current;
        if (t.smooth && Math.abs(diff) > 0.0001) {
            t.current += diff * Math.min(1, SMOOTH_RATE * dt);
        } else {
            t.current = t.value;
        }
        changed = true;

        const obj = part.object3D;
        switch (t.property) {
            case 'rotateX': obj.rotation.x = s.THREE.MathUtils.degToRad(t.current); break;
            case 'rotateY': obj.rotation.y = s.THREE.MathUtils.degToRad(t.current); break;
            case 'rotateZ': obj.rotation.z = s.THREE.MathUtils.degToRad(t.current); break;
            case 'posX': obj.position.x = t.current; break;
            case 'posY': obj.position.y = t.current; break;
            case 'posZ': obj.position.z = t.current; break;
            case 'scale': obj.scale.setScalar(t.current); break;
        }
    }
    return changed;
}

function ensureDriver() {
    if (_driverRunning) return;
    _driverRunning = true;
    _lastDriverTime = performance.now();
    requestAnimationFrame(driverTick);
}

document.addEventListener('visibilitychange', () => {
    if (!document.hidden) {
        for (const id in _scenes) _scenes[id].forceRender = true;
        ensureDriver();
    }
});

// Use esm.sh, which rewrites bare specifiers (e.g. "import ... from 'three'")
// inside addon modules into resolved absolute URLs. unpkg serves the raw
// source files unmodified, so addons importing the bare "three" specifier
// fail to resolve in the browser's native ES module loader.
const THREE_VERSION = '0.160.0';
const THREE_BASE = `https://esm.sh/three@${THREE_VERSION}`;
const ORBIT_CONTROLS_URL = `https://esm.sh/three@${THREE_VERSION}/examples/jsm/controls/OrbitControls.js`;
const GLTF_LOADER_URL = `https://esm.sh/three@${THREE_VERSION}/examples/jsm/loaders/GLTFLoader.js`;

let THREE, OrbitControls, GLTFLoader;

async function ensureThree() {
    if (_threeLoaded) return;
    if (_threeLoading) { await _threeLoading; return; }
    _threeLoading = (async () => {
        THREE = await import(THREE_BASE);
        const [{ OrbitControls: OC }, { GLTFLoader: GL }] = await Promise.all([
            import(ORBIT_CONTROLS_URL),
            import(GLTF_LOADER_URL)
        ]);
        OrbitControls = OC;
        GLTFLoader = GL;
        _threeLoaded = true;
    })();
    await _threeLoading;
}

function createPrimitive(shape, color) {
    let geometry;
    switch (shape) {
        case 'sphere':
            geometry = new THREE.SphereGeometry(1, 32, 24);
            break;
        case 'cylinder':
            geometry = new THREE.CylinderGeometry(1, 1, 1.6, 32);
            break;
        case 'cone':
            geometry = new THREE.ConeGeometry(1, 1.6, 32);
            break;
        case 'torus':
            geometry = new THREE.TorusGeometry(0.8, 0.3, 16, 48);
            break;
        default: // box
            geometry = new THREE.BoxGeometry(1.4, 1.4, 1.4);
            break;
    }
    const material = new THREE.MeshStandardMaterial({ color, roughness: 0.4, metalness: 0.2 });
    return new THREE.Mesh(geometry, material);
}

export async function initScene(containerId, options) {
    await ensureThree();

    const container = document.getElementById(containerId);
    if (!container) return;

    if (container.offsetWidth === 0 || container.offsetHeight === 0) {
        await new Promise(r => setTimeout(r, 100));
    }

    const width = container.clientWidth || 300;
    const height = container.clientHeight || 200;

    const scene = new THREE.Scene();
    scene.background = new THREE.Color(options.background || '#0f172a');

    const camera = new THREE.PerspectiveCamera(45, width / height, 0.1, 1000);
    camera.position.set(2.6, 2.1, 3.2);

    // antialias off + capped pixel ratio (1x): with several scene3d widgets on
    // one screen each running its own WebGL context, full-AA + high DPR
    // multiplies GPU/CPU cost quickly and can starve the browser main thread
    // (freezing the rest of the Blazor UI). The shared driver above further
    // reduces load by batching all scenes into one rAF tick at ~24fps.
    const renderer = new THREE.WebGLRenderer({ antialias: false, alpha: false, powerPreference: 'low-power' });
    renderer.setSize(width, height);
    renderer.setPixelRatio(Math.min(window.devicePixelRatio || 1, 1));
    container.innerHTML = '';
    container.appendChild(renderer.domElement);

    // Lighting
    scene.add(new THREE.AmbientLight(0xffffff, 0.6));
    const dirLight = new THREE.DirectionalLight(0xffffff, 0.9);
    dirLight.position.set(3, 5, 2);
    scene.add(dirLight);
    const fillLight = new THREE.DirectionalLight(0x88aaff, 0.25);
    fillLight.position.set(-3, -2, -3);
    scene.add(fillLight);

    if (options.showGrid) {
        const grid = new THREE.GridHelper(6, 12, 0x334155, 0x1e293b);
        grid.position.y = -1.2;
        scene.add(grid);
    }

    let subject = null;      // THREE.Object3D being rotated/colored
    let colorable = null;    // mesh whose material color we can update (primitive only)
    let parts = {};          // lowercased node name -> { object3D, colorableMaterial }
    let partNames = [];      // original-case names, for discovery/editor UI

    const group = new THREE.Group();
    scene.add(group);

    if (options.modelUrl) {
        try {
            const loader = new GLTFLoader();
            const gltf = await loader.loadAsync(options.modelUrl);
            subject = gltf.scene;
            // Normalize scale/position so arbitrary models fit the view
            const box = new THREE.Box3().setFromObject(subject);
            const size = box.getSize(new THREE.Vector3());
            const center = box.getCenter(new THREE.Vector3());
            const maxDim = Math.max(size.x, size.y, size.z) || 1;
            const scale = 2.2 / maxDim;
            subject.scale.setScalar(scale);
            subject.position.sub(center.multiplyScalar(scale));
            group.add(subject);

            // Build a name -> node lookup so individual parts (meshes, groups,
            // bones) can be independently animated. Materials are cloned per-mesh
            // so a color binding on one part doesn't repaint every mesh sharing
            // the same material instance from the glTF file.
            subject.traverse(obj => {
                if (!obj.name) return;
                let colorableMaterial = null;
                if (obj.isMesh && obj.material) {
                    obj.material = Array.isArray(obj.material)
                        ? obj.material.map(m => m.clone())
                        : obj.material.clone();
                    colorableMaterial = Array.isArray(obj.material) ? obj.material[0] : obj.material;
                }
                const key = obj.name.toLowerCase();
                if (!parts[key]) partNames.push(obj.name);
                parts[key] = { object3D: obj, colorableMaterial };
            });
        } catch (err) {
            console.error('Scene3DWidget: failed to load model', err);
            subject = createPrimitive('box', options.color || '#3b82f6');
            colorable = subject;
            group.add(subject);
        }
    } else {
        subject = createPrimitive(options.shape, options.color || '#3b82f6');
        colorable = subject;
        group.add(subject);
    }

    let controls = null;
    if (options.orbitControls) {
        controls = new OrbitControls(camera, renderer.domElement);
        controls.enableDamping = true;
        controls.dampingFactor = 0.08;
        controls.minDistance = 1.5;
        controls.maxDistance = 12;
    } else {
        camera.lookAt(0, 0, 0);
    }

    const state = {
        THREE, scene, camera, renderer, group, subject, colorable, controls, parts, partNames,
        autoRotate: !!options.autoRotate,
        rotateSpeed: options.rotateSpeed || 30, // deg/sec
        manualRotationDeg: null,                // set via updateScene when bound to a variable
        partTargets: {},                        // bindingId -> { partNameLower, property, value/color, current, smooth }
        visible: true,                          // becomes false when scrolled off-screen
        forceRender: true,
        intersectionObserver: null,
        resizeObserver: null,
        disposed: false
    };

    // Render one frame immediately so the scene isn't blank until the shared
    // driver's next tick.
    renderer.render(scene, camera);
    ensureDriver();

    // Pause rendering/animation entirely for scenes scrolled out of view —
    // avoids paying full render cost for widgets the user can't see.
    const io = new IntersectionObserver((entries) => {
        for (const entry of entries) {
            state.visible = entry.isIntersecting;
            if (state.visible) state.forceRender = true;
        }
    }, { threshold: 0.01 });
    io.observe(container);
    state.intersectionObserver = io;

    // Responsive resize
    const ro = new ResizeObserver(() => {
        const w = container.clientWidth || width;
        const h = container.clientHeight || height;
        if (w === 0 || h === 0) return;
        camera.aspect = w / h;
        camera.updateProjectionMatrix();
        renderer.setSize(w, h);
        state.forceRender = true;
    });
    ro.observe(container);
    state.resizeObserver = ro;

    _scenes[containerId] = state;
}

export function updateScene(containerId, updates) {
    const s = _scenes[containerId];
    if (!s) return;

    if (updates.color != null && s.colorable && s.colorable.material) {
        s.colorable.material.color.set(updates.color);
    }
    if (updates.rotationDeg != null) {
        s.manualRotationDeg = updates.rotationDeg;
    }
    if (updates.partUpdates) {
        for (const pu of updates.partUpdates) {
            const target = s.partTargets[pu.id] || { smooth: pu.smooth !== false };
            target.partNameLower = (pu.partName || '').toLowerCase();
            target.property = pu.property;
            target.smooth = pu.smooth !== false;
            if (pu.property === 'color') {
                target.color = pu.color ?? null;
            } else if (pu.value != null) {
                target.value = pu.value;
            }
            s.partTargets[pu.id] = target;
            s.forceRender = true;
        }
    }
}

// Returns the list of named nodes/meshes found in the currently loaded model,
// so the editor can offer a picker instead of requiring manual name entry.
export function getPartNames(containerId) {
    const s = _scenes[containerId];
    return s ? s.partNames : [];
}

export function destroyScene(containerId) {
    const s = _scenes[containerId];
    if (!s) return;
    s.disposed = true;
    if (s.resizeObserver) s.resizeObserver.disconnect();
    if (s.intersectionObserver) s.intersectionObserver.disconnect();
    if (s.controls) s.controls.dispose();
    s.scene.traverse(obj => {
        if (obj.geometry) obj.geometry.dispose();
        if (obj.material) {
            const mats = Array.isArray(obj.material) ? obj.material : [obj.material];
            mats.forEach(m => m.dispose());
        }
    });
    s.renderer.dispose();
    delete _scenes[containerId];
}
