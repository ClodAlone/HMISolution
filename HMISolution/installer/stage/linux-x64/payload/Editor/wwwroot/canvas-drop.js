window.canvasDrop = {
    _dotNetRef: null,
    _svgEl: null,
    _wrapEl: null,

    init: function (dotNetRef) {
        this._dotNetRef = dotNetRef;
        this._svgEl = document.querySelector('.screen-canvas');
        this._wrapEl = document.querySelector('.screen-canvas-wrap');

        if (this._svgEl) {
            this._svgEl.addEventListener('dragover', this._onDragOver);
            this._svgEl.addEventListener('drop', this._onDrop);
        }
        if (this._wrapEl) {
            this._wrapEl.addEventListener('dragover', this._onDragOver);
            this._wrapEl.addEventListener('drop', this._onDrop);
        }
    },

    dispose: function () {
        if (this._svgEl) {
            this._svgEl.removeEventListener('dragover', this._onDragOver);
            this._svgEl.removeEventListener('drop', this._onDrop);
        }
        if (this._wrapEl) {
            this._wrapEl.removeEventListener('dragover', this._onDragOver);
            this._wrapEl.removeEventListener('drop', this._onDrop);
        }
        this._dotNetRef = null;
        this._svgEl = null;
        this._wrapEl = null;
    },

    reinit: function () {
        // Re-attach after Blazor re-renders (new screen selected, etc.)
        if (this._svgEl) {
            this._svgEl.removeEventListener('dragover', this._onDragOver);
            this._svgEl.removeEventListener('drop', this._onDrop);
        }
        if (this._wrapEl) {
            this._wrapEl.removeEventListener('dragover', this._onDragOver);
            this._wrapEl.removeEventListener('drop', this._onDrop);
        }
        this._svgEl = document.querySelector('.screen-canvas');
        this._wrapEl = document.querySelector('.screen-canvas-wrap');
        if (this._svgEl) {
            this._svgEl.addEventListener('dragover', this._onDragOver);
            this._svgEl.addEventListener('drop', this._onDrop);
        }
        if (this._wrapEl) {
            this._wrapEl.addEventListener('dragover', this._onDragOver);
            this._wrapEl.addEventListener('drop', this._onDrop);
        }
    },

    _onDragOver: function (e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'copy';
    },

    _onDrop: function (e) {
        e.preventDefault();
        e.stopPropagation();

        var ref = window.canvasDrop._dotNetRef;
        if (!ref) return;

        // Compute position relative to the SVG element
        var svg = window.canvasDrop._svgEl;
        var dropX = e.offsetX;
        var dropY = e.offsetY;

        if (svg) {
            var rect = svg.getBoundingClientRect();
            dropX = e.clientX - rect.left;
            dropY = e.clientY - rect.top;
        }

        ref.invokeMethodAsync('JsOnCanvasDrop', dropX, dropY);
    }
};
