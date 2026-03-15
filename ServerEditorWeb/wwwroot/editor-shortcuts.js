window.editorShortcuts = {
    _dotNetRef: null,
    _hasUnsavedChanges: false,

    init: function (dotNetRef) {
        this._dotNetRef = dotNetRef;
        document.addEventListener('keydown', this._onKeyDown);
        window.addEventListener('beforeunload', this._onBeforeUnload);
    },

    dispose: function () {
        document.removeEventListener('keydown', this._onKeyDown);
        window.removeEventListener('beforeunload', this._onBeforeUnload);
        this._dotNetRef = null;
    },

    setUnsavedChanges: function (hasChanges) {
        this._hasUnsavedChanges = hasChanges;
    },

    _onBeforeUnload: function (e) {
        if (window.editorShortcuts._hasUnsavedChanges) {
            e.preventDefault();
            e.returnValue = '';
        }
    },

    _onKeyDown: function (e) {
        var ref = window.editorShortcuts._dotNetRef;
        if (!ref) return;

        // Skip if the user is typing in an input, textarea, or contenteditable
        var tag = e.target.tagName;
        var isEditable = (tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT' || e.target.isContentEditable);

        if (e.ctrlKey && !e.altKey) {
            switch (e.key.toLowerCase()) {
                case 'c':
                    if (!isEditable) {
                        e.preventDefault();
                        ref.invokeMethodAsync('OnKeyboardCopy');
                    }
                    break;
                case 'v':
                    if (!isEditable) {
                        e.preventDefault();
                        ref.invokeMethodAsync('OnKeyboardPaste');
                    }
                    break;
                case 's':
                    e.preventDefault();
                    ref.invokeMethodAsync('OnKeyboardSave');
                    break;
                case 'z':
                    e.preventDefault();
                    ref.invokeMethodAsync('OnKeyboardUndo');
                    break;
                case 'y':
                    e.preventDefault();
                    ref.invokeMethodAsync('OnKeyboardRedo');
                    break;
            }
        }

        // Escape — restore maximized or close auto-hide flyout
        if (e.key === 'Escape' && !isEditable) {
            ref.invokeMethodAsync('OnKeyboardEscape');
        }
    }
};
