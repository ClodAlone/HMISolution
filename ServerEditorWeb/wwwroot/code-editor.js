window.codeEditor = {
    create: function (element, dotNetRef, value, language, height) {
        if (element._cm) return;

        var mode = language === 'csharp' ? 'text/x-csharp'
                 : language === 'vb' ? 'text/x-vb'
                 : { name: 'javascript', json: true };

        var editor = CodeMirror(element, {
            value: value || '',
            mode: mode,
            lineNumbers: true,
            matchBrackets: true,
            autoCloseBrackets: true,
            tabSize: 4,
            indentWithTabs: false,
            indentUnit: 4,
            lineWrapping: false,
            theme: 'default'
        });

        editor.setSize(null, height);

        var suppressed = false;
        editor.on('changes', function () {
            if (suppressed) return;
            dotNetRef.invokeMethodAsync('OnEditorChanged', editor.getValue());
        });

        element._cm = editor;
        element._cmSuppress = function (fn) { suppressed = true; fn(); suppressed = false; };
    },

    setValue: function (element, value) {
        var editor = element._cm;
        if (!editor) return;
        if (editor.getValue() === (value || '')) return;
        element._cmSuppress(function () {
            var scroll = editor.getScrollInfo();
            var cursor = editor.getCursor();
            editor.setValue(value || '');
            editor.setCursor(cursor);
            editor.scrollTo(scroll.left, scroll.top);
        });
    },

    destroy: function (element) {
        var editor = element._cm;
        if (!editor) return;
        var wrapper = editor.getWrapperElement();
        if (wrapper && wrapper.parentNode) wrapper.parentNode.removeChild(wrapper);
        element._cm = null;
        element._cmSuppress = null;
    }
};
