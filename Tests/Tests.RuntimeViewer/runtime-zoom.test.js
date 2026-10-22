const { test } = require('node:test');
const assert = require('node:assert/strict');
const { readFileSync } = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');

const source = readFileSync(path.join(__dirname, '..', '..', 'RuntimeViewer.Shared', 'wwwroot', 'runtime-zoom.js'), 'utf8');

function runtime(pixelRatio = 1) {
    const properties = new Map();
    const attributes = new Set();
    const listeners = new Map();
    const window = {
        devicePixelRatio: pixelRatio, innerWidth: 1440, innerHeight: 900,
        addEventListener: (name, callback) => listeners.set(name, callback)
    };
    const document = {
        documentElement: {
            style: {
                setProperty: (name, value) => properties.set(name, value),
                removeProperty: name => properties.delete(name)
            },
            toggleAttribute: (name, enabled) => enabled ? attributes.add(name) : attributes.delete(name)
        }
    };
    vm.runInNewContext(source, { window, document });
    return {
        window, properties, attributes,
        resize(width, height, ratio) {
            Object.assign(window, { innerWidth: width, innerHeight: height, devicePixelRatio: ratio });
            listeners.get('resize')();
        }
    };
}

for (const initialRatio of [1, 1.25, 1.5, 2]) {
    test(`page zoom preserves logical layout at initial display scale ${initialRatio}`, () => {
        const page = runtime(initialRatio);
        assert.equal(page.properties.size, 0);
        assert.equal(page.attributes.size, 0);

        for (const zoom of [1.25, 1.5, 2, 0.75, 0.5, 1.5]) {
            page.resize(1440 / zoom, 900 / zoom, initialRatio * zoom);
            assert.equal(page.properties.get('--rv-viewport-width'), '1440px');
            assert.equal(page.properties.get('--rv-viewport-height'), '900px');
            assert.equal(page.attributes.has('data-rv-page-zoom'), true);

            // CSS content, including auto-fitted SVG, now follows native browser magnification.
            const physicalWidth = parseFloat(page.properties.get('--rv-viewport-width')) * page.window.devicePixelRatio;
            assert.equal(physicalWidth, 1440 * initialRatio * zoom);
        }

        page.resize(1440, 900, initialRatio);
        assert.equal(page.properties.size, 0);
        assert.equal(page.attributes.size, 0);
    });
}

test('real window resizing still changes the logical layout while zoomed', () => {
    const page = runtime();
    page.resize(960, 600, 1.5);
    page.resize(1200, 700, 1.5);
    assert.equal(page.properties.get('--rv-viewport-width'), '1800px');
    assert.equal(page.properties.get('--rv-viewport-height'), '1050px');
});

test('ordinary resizing and touch pinch zoom leave native viewport sizing intact', () => {
    const page = runtime(1.5);
    page.resize(800, 600, 1.5);
    page.window.visualViewport = { width: 400, height: 300, scale: 2 };
    page.resize(800, 600, 1.5);
    assert.equal(page.properties.size, 0);
    assert.equal(page.attributes.size, 0);
});
