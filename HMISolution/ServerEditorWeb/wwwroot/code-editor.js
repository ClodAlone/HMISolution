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

    setDebugAnnotations: function (element, annotations, executedLines, writeLines) {
        var editor = element._cm;
        if (!editor) return;

        // Clear previous annotations
        if (element._debugWidgets) {
            for (var i = 0; i < element._debugWidgets.length; i++) {
                element._debugWidgets[i].clear();
            }
        }
        element._debugWidgets = [];

        if (element._debugLineClasses) {
            for (var i = 0; i < element._debugLineClasses.length; i++) {
                var lc = element._debugLineClasses[i];
                editor.removeLineClass(lc.line, 'wrap', lc.cls);
            }
        }
        element._debugLineClasses = [];

        // Apply executed / write line background classes
        if (executedLines) {
            for (var i = 0; i < executedLines.length; i++) {
                var ln = executedLines[i];
                if (ln >= 0 && ln < editor.lineCount()) {
                    editor.addLineClass(ln, 'wrap', 'debug-line-executed');
                    element._debugLineClasses.push({ line: ln, cls: 'debug-line-executed' });
                }
            }
        }
        if (writeLines) {
            for (var i = 0; i < writeLines.length; i++) {
                var ln = writeLines[i];
                if (ln >= 0 && ln < editor.lineCount()) {
                    editor.addLineClass(ln, 'wrap', 'debug-line-write');
                    element._debugLineClasses.push({ line: ln, cls: 'debug-line-write' });
                }
            }
        }

        // Add inline annotation text markers after each line's content
        if (annotations) {
            for (var lineStr in annotations) {
                var line = parseInt(lineStr);
                var text = annotations[lineStr];
                if (line < 0 || line >= editor.lineCount() || !text) continue;

                var lineContent = editor.getLine(line);
                var endCh = lineContent.length;

                // Create a bookmark widget at end of line
                var span = document.createElement('span');
                span.className = 'debug-inline-annotation';
                span.textContent = '  \u00AB ' + text + ' \u00BB';
                span.title = text;

                var marker = editor.setBookmark({ line: line, ch: endCh }, { widget: span, insertLeft: true });
                element._debugWidgets.push(marker);
            }
        }
    },

    clearDebugAnnotations: function (element) {
        var editor = element._cm;
        if (!editor) return;
        if (element._debugWidgets) {
            for (var i = 0; i < element._debugWidgets.length; i++) {
                element._debugWidgets[i].clear();
            }
            element._debugWidgets = [];
        }
        if (element._debugLineClasses) {
            for (var i = 0; i < element._debugLineClasses.length; i++) {
                var lc = element._debugLineClasses[i];
                editor.removeLineClass(lc.line, 'wrap', lc.cls);
            }
            element._debugLineClasses = [];
        }
    },

    destroy: function (element) {
        var editor = element._cm;
        if (!editor) return;
        codeEditor.clearDebugAnnotations(element);
        var wrapper = editor.getWrapperElement();
        if (wrapper && wrapper.parentNode) wrapper.parentNode.removeChild(wrapper);
        element._cm = null;
        element._cmSuppress = null;
    }
};
