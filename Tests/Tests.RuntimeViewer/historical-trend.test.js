const { test } = require('node:test');
const assert = require('node:assert/strict');
const { readFileSync } = require('node:fs');
const path = require('node:path');

const source = readFileSync(path.join(__dirname, '..', '..', 'RuntimeViewer.Shared', 'wwwroot', 'historical-trend.js'), 'utf8');
const modulePromise = import(`data:text/javascript;base64,${Buffer.from(source).toString('base64')}`);

function table(scale, rowTop, rowBottom) {
    return {
        isConnected: true, offsetHeight: 200, clientHeight: 198, clientTop: 1, scrollTop: 100,
        getBoundingClientRect: () => ({ top: 100, height: 200 * scale }),
        querySelector: selector => selector === 'thead'
            ? { getBoundingClientRect: () => ({ height: 20 * scale }) }
            : { getBoundingClientRect: () => ({ top: 100 + rowTop * scale, bottom: 100 + rowBottom * scale }) }
    };
}

for (const scale of [0.5, 1, 1.5, 2]) {
    test(`scrolls upward below sticky header at scale ${scale}`, async () => {
        const { revealSelectedRow } = await modulePromise;
        const container = table(scale, 10, 30);
        revealSelectedRow(container);
        assert.equal(container.scrollTop, 89);
    });
    test(`scrolls downward only far enough to reveal row at scale ${scale}`, async () => {
        const { revealSelectedRow } = await modulePromise;
        const container = table(scale, 190, 210);
        revealSelectedRow(container);
        assert.equal(container.scrollTop, 111);
    });
    test(`does not move a fully visible row at scale ${scale}`, async () => {
        const { revealSelectedRow } = await modulePromise;
        const container = table(scale, 21, 199);
        revealSelectedRow(container);
        assert.equal(container.scrollTop, 100);
    });
}

test('hidden unknown sample has no row and must not change scroll position', async () => {
    const { revealSelectedRow } = await modulePromise;
    const container = table(1, 0, 20);
    container.querySelector = () => null;
    revealSelectedRow(container);
    assert.equal(container.scrollTop, 100);
});

test('disconnected and hidden containers do not scroll', async () => {
    const { revealSelectedRow } = await modulePromise;
    revealSelectedRow(null);
    const container = table(1, 0, 20);
    container.isConnected = false;
    revealSelectedRow(container);
    assert.equal(container.scrollTop, 100);
    container.isConnected = true;
    container.getBoundingClientRect = () => ({ top: 100, height: 0 });
    revealSelectedRow(container);
    assert.equal(container.scrollTop, 100);
});
