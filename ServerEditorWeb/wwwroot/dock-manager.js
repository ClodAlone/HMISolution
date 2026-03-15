window.dockManager = {
    _handler: null,
    _mode: null, // 'drag', 'resize', 'splitter-right', 'splitter-bottom'
    _data: null,

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
    }
};
