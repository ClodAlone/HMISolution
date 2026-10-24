const { test } = require('node:test');
const assert = require('node:assert/strict');
const { readFileSync } = require('node:fs');
const path = require('node:path');

const source = readFileSync(path.join(__dirname, '..', '..', 'RuntimeViewer.Shared', 'wwwroot', 'ipcamera.js'), 'utf8');
let revision = 0;

async function setup(t) {
    const observers = [];
    t.mock.method(globalThis, 'MutationObserver', function (callback) {
        const observer = {
            callback,
            observe(target, options) { this.target = target; this.options = options; this.connected = true; },
            disconnect() { this.connected = false; }
        };
        observers.push(observer);
        return observer;
    });
    const camera = await import(`data:text/javascript;base64,${Buffer.from(source).toString('base64')}#${++revision}`);
    return { ...camera, observers, update: () => observers.filter(o => o.connected).forEach(o => o.callback()) };
}

// Node has no DOM; the browser regression test exercises real connection cancellation.
globalThis.document = { documentElement: {} };
globalThis.MutationObserver = class {};

function image() {
    return {
        isConnected: true, hidden: false, requests: [], clears: 0,
        set src(value) { this.requests.push(value); this.current = value; },
        removeAttribute(name) { assert.equal(name, 'src'); this.current = undefined; this.clears++; },
        closest(selector) { assert.equal(selector, '[hidden]'); return this.hidden ? {} : null; }
    };
}

test('unchanged component rerenders do not reopen a stream', async t => {
    const camera = await setup(t), img = image();
    camera.start(img, '/camera/one/stream');
    camera.start(img, '/camera/one/stream');
    camera.update();
    assert.deepEqual(img.requests, ['/camera/one/stream']);
    assert.equal(img.clears, 0);
    assert.equal(camera.observers.length, 1);
    assert.deepEqual(camera.observers[0].options.attributeFilter, ['hidden']);
});

test('navigation clears every detached stream and releases the observer', async t => {
    const camera = await setup(t), images = [image(), image(), image()];
    images.forEach((img, i) => camera.start(img, `/camera/${i}/stream`));
    images.forEach(img => { img.isConnected = false; });
    camera.update();
    for (const img of images) {
        assert.equal(img.current, undefined);
        assert.equal(img.clears, 1);
    }
    assert.equal(camera.observers[0].connected, false);
    const returned = image();
    camera.start(returned, '/camera/0/stream');
    assert.equal(returned.current, '/camera/0/stream');
    assert.equal(camera.observers.length, 2);
    assert.equal(camera.observers[1].connected, true);
});

test('cached screens pause while hidden and resume on return', async t => {
    const camera = await setup(t), img = image();
    camera.start(img, '/camera/one/stream');
    img.hidden = true;
    camera.update();
    assert.equal(img.current, undefined);
    assert.equal(img.clears, 1);
    assert.equal(camera.observers[0].connected, true);
    img.hidden = false;
    camera.update();
    assert.deepEqual(img.requests, ['/camera/one/stream', '/camera/one/stream']);
    assert.equal(img.current, '/camera/one/stream');
});

test('preloaded hidden screens do not open streams before they become visible', async t => {
    const camera = await setup(t), img = image();
    img.hidden = true;
    camera.start(img, '/camera/one/stream');
    assert.deepEqual(img.requests, []);
    img.hidden = false;
    camera.update();
    assert.deepEqual(img.requests, ['/camera/one/stream']);
});

test('retry or changed camera cancels the old source before opening the new source', async t => {
    const camera = await setup(t), img = image();
    camera.start(img, '/camera/one/stream?v=0');
    camera.start(img, '/camera/one/stream?v=1');
    assert.equal(img.clears, 1);
    assert.deepEqual(img.requests, ['/camera/one/stream?v=0', '/camera/one/stream?v=1']);
    assert.equal(img.current, '/camera/one/stream?v=1');
});

test('a DOM move in the same mutation batch does not interrupt a connected image', async t => {
    const camera = await setup(t), img = image();
    camera.start(img, '/camera/one/stream');
    camera.update();
    assert.equal(img.clears, 0);
    assert.deepEqual(img.requests, ['/camera/one/stream']);
});

test('explicit stop is idempotent and does not interrupt other widgets', async t => {
    const camera = await setup(t), first = image(), second = image();
    camera.start(first, '/camera/one/stream');
    camera.start(second, '/camera/two/stream');
    camera.stop(first);
    camera.stop(first);
    assert.equal(first.clears, 1);
    assert.equal(second.current, '/camera/two/stream');
    assert.equal(second.clears, 0);
    assert.equal(camera.observers[0].connected, true);
    camera.stop(second);
    assert.equal(camera.observers[0].connected, false);
});

test('missing or already removed images do not start streams', async t => {
    const camera = await setup(t), img = image();
    img.isConnected = false;
    camera.start(null, '/camera/one/stream');
    camera.start(img, '/camera/one/stream');
    assert.deepEqual(img.requests, []);
    assert.equal(camera.observers.length, 0);
});
