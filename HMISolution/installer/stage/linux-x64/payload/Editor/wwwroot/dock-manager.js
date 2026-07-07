window.dockManager = {
    _handler: null,
    _mode: null, // 'drag', 'resize', 'splitter-right', 'splitter-bottom'
    _data: null,

    /** Trigger a browser file-download with arbitrary text content. */
    downloadText: function (filename, mimeType, content) {
        var blob = new Blob([content], { type: mimeType });
        var url = URL.createObjectURL(blob);
        var a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    },

    startDrag: function (dotNetRef, mode, data) {
        this._handler = dotNetRef;
        this._mode = mode;
        this._data = data;

        document.addEventListener('mousemove', this._onMouseMove);
        document.addEventListener('mouseup', this._onMouseUp);
        document.body.style.userSelect = 'none';
    },

    _onMouseMove: function (e) {
        var dm = window.dockManager;
        if (dm._handler && dm._mode) {
            dm._handler.invokeMethodAsync('OnJsMouseMove', e.clientX, e.clientY);
        }
    },

    _onMouseUp: function (e) {
        var dm = window.dockManager;
        if (dm._handler && dm._mode) {
            dm._handler.invokeMethodAsync('OnJsMouseUp', e.clientX, e.clientY);
        }
        dm.stop();
    },

    stop: function () {
        document.removeEventListener('mousemove', this._onMouseMove);
        document.removeEventListener('mouseup', this._onMouseUp);
        document.body.style.userSelect = '';
        this._handler = null;
        this._mode = null;
        this._data = null;
    },

    // ── Tree panel resize splitter (same pattern as dock splitters) ──────
    _treeHandler: null,
    _treeDragStartX: 0,
    _treeStartWidth: 0,
    _treeSplitterKey: 'tree-panel-width',

    /** Called from Blazor @onmousedown on the tree splitter. */
    startTreeSplitter: function (dotNetRef, startX, startWidth, storageKey) {
        this._treeHandler  = dotNetRef;
        this._treeDragStartX  = startX;
        this._treeStartWidth  = startWidth;
        this._treeSplitterKey = storageKey || 'tree-panel-width';
        document.addEventListener('mousemove', this._onTreeMove);
        document.addEventListener('mouseup',   this._onTreeUp);
        document.body.style.userSelect = 'none';
        document.body.style.cursor     = 'col-resize';
    },

    _onTreeMove: function (e) {
        var dm = window.dockManager;
        if (!dm._treeHandler) return;
        var delta    = e.clientX - dm._treeDragStartX;
        var newWidth = Math.max(160, Math.min(600, dm._treeStartWidth + delta));
        dm._treeHandler.invokeMethodAsync('OnTreeSplitterMove', newWidth);
    },

    _onTreeUp: function (e) {
        var dm = window.dockManager;
        var delta    = e.clientX - dm._treeDragStartX;
        var newWidth = Math.max(160, Math.min(600, dm._treeStartWidth + delta));
        localStorage.setItem(dm._treeSplitterKey, newWidth);
        document.removeEventListener('mousemove', dm._onTreeMove);
        document.removeEventListener('mouseup',   dm._onTreeUp);
        document.body.style.userSelect = '';
        document.body.style.cursor     = '';
        dm._treeHandler = null;
    },

    /** Restore persisted width on page load. Returns the saved pixel value or 0. */
    restoreTreeWidth: function (storageKey) {
        var saved = localStorage.getItem(storageKey || 'tree-panel-width');
        return saved ? parseInt(saved, 10) : 0;
    },

    /** Return the current pixel width of the center dock zone (used to size a new split pane). */
    getCenterZoneWidth: function () {
        var el = document.querySelector('.dock-zone-center');
        return el ? Math.round(el.getBoundingClientRect().width) : 0;
    }
};
