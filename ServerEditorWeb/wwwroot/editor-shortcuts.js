window.editorShortcuts = {
    _dotNetRef: null,
    _hasUnsavedChanges: false,

    init: function (dotNetRef) {
        this._dotNetRef = dotNetRef;
        // Use capture phase so we can preventDefault before the browser opens its own Find dialog
        document.addEventListener('keydown', this._onKeyDown, { capture: true });
        window.addEventListener('beforeunload', this._onBeforeUnload);
    },

    dispose: function () {
        document.removeEventListener('keydown', this._onKeyDown, { capture: true });
        window.removeEventListener('beforeunload', this._onBeforeUnload);
        this._dotNetRef = null;
    },

    setUnsavedChanges: function (hasChanges) {
        this._hasUnsavedChanges = hasChanges;
    },

    scrollIntoView: function (element) {
        if (element) element.scrollIntoView({ block: 'nearest', behavior: 'smooth' });
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
                    if (!isEditable) {
                        e.preventDefault();
                        ref.invokeMethodAsync('OnKeyboardUndo');
                    }
                    break;
                case 'y':
                    if (!isEditable) {
                        e.preventDefault();
                        ref.invokeMethodAsync('OnKeyboardRedo');
                    }
                    break;
                case 'o':
                    e.preventDefault();
                    ref.invokeMethodAsync('OnKeyboardOpen');
                    break;
                case 'f':
                    e.preventDefault();
                    if (e.shiftKey) {
                        ref.invokeMethodAsync('OnKeyboardGlobalFind');
                    } else {
                        ref.invokeMethodAsync('OnKeyboardFind');
                    }
                    break;
                case 'h':
                    e.preventDefault();
                    ref.invokeMethodAsync('OnKeyboardFindReplace');
                    break;
            }
        }

        // Escape — restore maximized or close auto-hide flyout
        if (e.key === 'Escape' && !isEditable) {
            ref.invokeMethodAsync('OnKeyboardEscape');
        }

        // Delete key — delete selected symbol or tree node
        if (e.key === 'Delete' && !isEditable) {
            e.preventDefault();
            ref.invokeMethodAsync('OnKeyboardDelete');
        }

        // Tab \u2013 cycle selection through screen objects by z-order
        if (e.key === 'Tab' && !isEditable) {
            e.preventDefault();
            if (e.shiftKey) {
                ref.invokeMethodAsync('OnKeyboardShiftTab');
            } else {
                ref.invokeMethodAsync('OnKeyboardTab');
            }
        }
    }
};
