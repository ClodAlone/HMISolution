window.codeEditor = {
    create: function (element, dotNetRef, value, language, height, enableBreakpoints, enableServerCompletions) {
        if (element._cm) return;

        var mode = language === 'csharp' ? 'text/x-csharp'
                 : language === 'vb' ? 'text/x-vb'
                 : { name: 'javascript', json: true };

        // Kind-to-CSS-class mapping for IntelliSense icons
        var kindClassMap = {
            method: 'cm-hint-function', property: 'cm-hint-property', field: 'cm-hint-field',
            event: 'cm-hint-event', 'class': 'cm-hint-class', 'interface': 'cm-hint-interface',
            namespace: 'cm-hint-namespace', keyword: 'cm-hint-keyword', variable: 'cm-hint-variable',
            constant: 'cm-hint-constant', text: ''
        };

        // Local completions hint (API functions + variable paths)
        var localHintFn = function (cm) {
            var cur = cm.getCursor();
            var line = cm.getLine(cur.line);
            var end = cur.ch;
            var start = end;
            while (start > 0 && /[\w.]/.test(line.charAt(start - 1))) start--;
            var token = line.substring(start, end).toLowerCase();

            var completions = element._completions || [];
            var filtered;
            if (token.length === 0) {
                filtered = completions.slice(0, 50);
            } else {
                var starts = [];
                var contains = [];
                for (var i = 0; i < completions.length; i++) {
                    var item = completions[i];
                    var textLower = item.text.toLowerCase();
                    if (textLower.indexOf(token) === 0) starts.push(item);
                    else if (textLower.indexOf(token) >= 0) contains.push(item);
                }
                filtered = starts.concat(contains).slice(0, 50);
            }

            return {
                list: filtered.map(function (item) {
                    return {
                        text: item.text,
                        displayText: item.displayText || item.text,
                        className: item.className || ''
                    };
                }),
                from: CodeMirror.Pos(cur.line, start),
                to: CodeMirror.Pos(cur.line, end)
            };
        };

        // Server-side (Roslyn) async hint function
        var serverHintFn = function (cm, callback) {
            var cur = cm.getCursor();
            var line = cm.getLine(cur.line);
            var end = cur.ch;
            var start = end;

            // Calculate cursor position in the full document
            var cursorPos = 0;
            for (var i = 0; i < cur.line; i++) {
                cursorPos += cm.getLine(i).length + 1; // +1 for newline
            }
            cursorPos += cur.ch;

            // Walk back for the token start (only word chars for Roslyn, stop at dot)
            var tokenStart = end;
            while (tokenStart > 0 && /[\w]/.test(line.charAt(tokenStart - 1))) tokenStart--;

            var code = cm.getValue();

            dotNetRef.invokeMethodAsync('OnRequestCompletions', code, cursorPos).then(function (items) {
                if (!items || items.length === 0) {
                    // Fall back to local completions
                    callback(localHintFn(cm));
                    return;
                }

                var list = items.map(function (item) {
                    return {
                        text: item.text,
                        displayText: item.displayText || item.text,
                        className: kindClassMap[item.kind] || ''
                    };
                });

                callback({
                    list: list,
                    from: CodeMirror.Pos(cur.line, tokenStart),
                    to: CodeMirror.Pos(cur.line, end)
                });
            }).catch(function () {
                callback(localHintFn(cm));
            });
        };
        serverHintFn.async = true;

        // Choose the appropriate hint function
        var hintFn = enableServerCompletions ? serverHintFn : localHintFn;

        var extraKeys = {
            'Ctrl-Space': function (cm) {
                cm.showHint({ hint: hintFn, completeSingle: false });
            }
        };

        var gutters = ['CodeMirror-linenumbers'];
        if (enableBreakpoints) gutters.unshift('breakpoints');

        var editor = CodeMirror(element, {
            value: value || '',
            mode: mode,
            lineNumbers: true,
            gutters: gutters,
            matchBrackets: true,
            autoCloseBrackets: true,
            tabSize: 4,
            indentWithTabs: false,
            indentUnit: 4,
            lineWrapping: false,
            theme: 'default',
            extraKeys: extraKeys,
            hintOptions: { completeSingle: false }
        });

        editor.setSize(null, height);

        // Store hint function for use by inputRead handler
        element._hintFn = hintFn;
        element._completions = [];

        var suppressed = false;
        editor.on('changes', function () {
            if (suppressed) return;
            dotNetRef.invokeMethodAsync('OnEditorChanged', editor.getValue());
        });

        // Auto-show completions when typing
        editor.on('inputRead', function (cm, change) {
            if (suppressed) return;
            if (cm.state.completionActive) return;
            var ch = change.text[change.text.length - 1];
            if (!ch) return;

            // If server completions are enabled, trigger on dot immediately
            if (enableServerCompletions && ch === '.') {
                cm.showHint({ hint: hintFn, completeSingle: false });
                return;
            }

            // If server completions are enabled, trigger on 'using' context
            if (enableServerCompletions && /[\w]/.test(ch)) {
                var cur = cm.getCursor();
                var line = cm.getLine(cur.line);
                var trimmed = line.substring(0, cur.ch).trimStart();
                if (/^using\s+\w/.test(trimmed)) {
                    cm.showHint({ hint: hintFn, completeSingle: false });
                    return;
                }
            }

            // Standard completions: trigger after 2+ chars typed
            var hasCompletions = enableServerCompletions || (element._completions && element._completions.length > 0);
            if (!hasCompletions) return;
            if (/[\w]/.test(ch)) {
                var cur = cm.getCursor();
                var line = cm.getLine(cur.line);
                var s = cur.ch;
                while (s > 0 && /[\w]/.test(line.charAt(s - 1))) s--;
                var tok = line.substring(s, cur.ch);
                if (tok.length >= 2) {
                    cm.showHint({ hint: hintFn, completeSingle: false });
                }
            }
        });

        element._breakpoints = new Set();
        element._enableBreakpoints = !!enableBreakpoints;

        if (enableBreakpoints) {
            editor.on('gutterClick', function (cm, line, gutter) {
                if (gutter !== 'breakpoints') return;
                var bps = element._breakpoints;
                if (bps.has(line)) {
                    bps.delete(line);
                    cm.setGutterMarker(line, 'breakpoints', null);
                } else {
                    bps.add(line);
                    cm.setGutterMarker(line, 'breakpoints', codeEditor._makeBreakpointMarker());
                }
                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync('OnBreakpointsChanged', Array.from(bps).sort(function(a,b){return a-b;}));
                }
            });
        }

        element._cm = editor;
        element._cmSuppress = function (fn) { suppressed = true; fn(); suppressed = false; };
    },

    setCompletions: function (element, completions) {
        element._completions = completions || [];
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

        // Add inline annotation text after each line's content
        if (annotations) {
            for (var lineStr in annotations) {
                var line = parseInt(lineStr);
                var text = annotations[lineStr];
                if (line < 0 || line >= editor.lineCount() || !text) continue;

                var lineContent = editor.getLine(line);
                var endCh = lineContent.length;

                var span = document.createElement("span");
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

    undo: function (element) {
        if (element._cm) element._cm.undo();
    },

    redo: function (element) {
        if (element._cm) element._cm.redo();
    },

    hasUndo: function (element) {
        return element._cm ? element._cm.historySize().undo > 0 : false;
    },

    hasRedo: function (element) {
        return element._cm ? element._cm.historySize().redo > 0 : false;
    },

    // Find the visible CodeMirror instance on the page (if any) and perform undo/redo.
    // Returns true if a CodeMirror was found and the operation was executed.
    activeUndo: function () {
        var cms = document.querySelectorAll('.CodeMirror');
        for (var i = 0; i < cms.length; i++) {
            if (cms[i].offsetParent !== null && cms[i].CodeMirror) {
                cms[i].CodeMirror.undo();
                return true;
            }
        }
        return false;
    },

    activeRedo: function () {
        var cms = document.querySelectorAll('.CodeMirror');
        for (var i = 0; i < cms.length; i++) {
            if (cms[i].offsetParent !== null && cms[i].CodeMirror) {
                cms[i].CodeMirror.redo();
                return true;
            }
        }
        return false;
    },

    // Returns true if there is a visible CodeMirror on the page.
    isCodeEditorVisible: function () {
        var cms = document.querySelectorAll('.CodeMirror');
        for (var i = 0; i < cms.length; i++) {
            if (cms[i].offsetParent !== null && cms[i].CodeMirror) return true;
        }
        return false;
    },

    setBreakpoints: function (element, lines) {
        var editor = element._cm;
        if (!editor) return;
        var bps = element._breakpoints || new Set();
        // Clear existing markers
        bps.forEach(function (ln) {
            editor.setGutterMarker(ln, 'breakpoints', null);
        });
        bps.clear();
        if (lines) {
            for (var i = 0; i < lines.length; i++) {
                var ln = lines[i];
                if (ln >= 0 && ln < editor.lineCount()) {
                    bps.add(ln);
                    editor.setGutterMarker(ln, 'breakpoints', codeEditor._makeBreakpointMarker());
                }
            }
        }
        element._breakpoints = bps;
    },

    getBreakpoints: function (element) {
        return element._breakpoints ? Array.from(element._breakpoints).sort(function(a,b){return a-b;}) : [];
    },

    setCurrentDebugLine: function (element, line) {
        var editor = element._cm;
        if (!editor) return;
        codeEditor.clearCurrentDebugLine(element);
        if (line >= 0 && line < editor.lineCount()) {
            editor.addLineClass(line, 'wrap', 'debug-current-line');
            element._currentDebugLine = line;
            editor.scrollIntoView({ line: line, ch: 0 }, 60);
        }
    },

    clearCurrentDebugLine: function (element) {
        var editor = element._cm;
        if (!editor) return;
        if (element._currentDebugLine != null) {
            editor.removeLineClass(element._currentDebugLine, 'wrap', 'debug-current-line');
            element._currentDebugLine = null;
        }
    },

    setExecutionLine: function (element, line) {
        var editor = element._cm;
        if (!editor) return;
        codeEditor.clearExecutionLine(element);
        if (line >= 0 && line < editor.lineCount()) {
            editor.addLineClass(line, 'wrap', 'debug-execution-line');
            element._executionLine = line;
        }
    },

    clearExecutionLine: function (element) {
        var editor = element._cm;
        if (!editor) return;
        if (element._executionLine != null) {
            editor.removeLineClass(element._executionLine, 'wrap', 'debug-execution-line');
            element._executionLine = null;
        }
    },

    _makeBreakpointMarker: function () {
        var marker = document.createElement('div');
        marker.className = 'breakpoint-marker';
        marker.innerHTML = '\u25CF';
        return marker;
    },

    destroy: function (element) {
        var editor = element._cm;
        if (!editor) return;
        codeEditor.clearDebugAnnotations(element);
        codeEditor.clearCurrentDebugLine(element);
        codeEditor.clearExecutionLine(element);
        var wrapper = editor.getWrapperElement();
        if (wrapper && wrapper.parentNode) wrapper.parentNode.removeChild(wrapper);
        element._cm = null;
        element._cmSuppress = null;
        element._completions = null;
        element._hintFn = null;
        element._breakpoints = null;
        element._enableBreakpoints = false;
    },

    // ─── Find & Replace helpers ─────────────────────────────
    // These operate on the first visible CodeMirror instance on the page.

    _getActiveEditor: function () {
        var cms = document.querySelectorAll('.CodeMirror');
        for (var i = 0; i < cms.length; i++) {
            if (cms[i].offsetParent !== null && cms[i].CodeMirror)
                return cms[i].CodeMirror;
        }
        return null;
    },

    _buildSearchQuery: function (text, caseSensitive, useRegex, wholeWord) {
        if (useRegex) {
            try {
                return new RegExp(text, caseSensitive ? 'g' : 'gi');
            } catch (e) {
                return null;
            }
        }
        if (wholeWord) {
            var escaped = text.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
            return new RegExp('\\b' + escaped + '\\b', caseSensitive ? 'g' : 'gi');
        }
        return null; // plain string search
    },

    _findMatches: function (editor, text, caseSensitive, useRegex, wholeWord) {
        var matches = [];
        if (!text) return matches;
        var doc = editor.getValue();
        var query = codeEditor._buildSearchQuery(text, caseSensitive, useRegex, wholeWord);
        if (query) {
            var m;
            while ((m = query.exec(doc)) !== null) {
                var from = editor.posFromIndex(m.index);
                var to = editor.posFromIndex(m.index + m[0].length);
                matches.push({ from: from, to: to });
                if (m[0].length === 0) break; // prevent infinite loop on zero-length match
            }
        } else {
            // Plain text search
            var searchStr = caseSensitive ? text : text.toLowerCase();
            var content = caseSensitive ? doc : doc.toLowerCase();
            var idx = 0;
            while (true) {
                var pos = content.indexOf(searchStr, idx);
                if (pos < 0) break;
                var from = editor.posFromIndex(pos);
                var to = editor.posFromIndex(pos + text.length);
                matches.push({ from: from, to: to });
                idx = pos + text.length;
            }
        }
        return matches;
    },

    _clearSearchOverlay: function (editor) {
        if (editor._frMarks) {
            for (var i = 0; i < editor._frMarks.length; i++) {
                editor._frMarks[i].clear();
            }
            editor._frMarks = [];
        }
        editor._frMatches = null;
        editor._frIndex = -1;
    },

    _highlightMatches: function (editor, matches) {
        codeEditor._clearSearchOverlay(editor);
        editor._frMarks = [];
        editor._frMatches = matches;
        editor._frIndex = -1;
        for (var i = 0; i < matches.length; i++) {
            var mark = editor.markText(matches[i].from, matches[i].to, {
                className: 'cm-fr-match'
            });
            editor._frMarks.push(mark);
        }
    },

    /// Start a search: highlight all matches and return count.
    startSearch: function (text, caseSensitive, useRegex, wholeWord) {
        var editor = codeEditor._getActiveEditor();
        if (!editor) return 0;
        var matches = codeEditor._findMatches(editor, text, caseSensitive, useRegex, wholeWord);
        codeEditor._highlightMatches(editor, matches);
        return matches.length;
    },

    /// Clear all search highlights from the active editor.
    clearSearch: function () {
        var editor = codeEditor._getActiveEditor();
        if (editor) codeEditor._clearSearchOverlay(editor);
    },

    /// Find the next match, select it, and scroll into view. Returns true if found.
    findNext: function (text, caseSensitive, useRegex, wholeWord) {
        var editor = codeEditor._getActiveEditor();
        if (!editor) return false;
        var matches = codeEditor._findMatches(editor, text, caseSensitive, useRegex, wholeWord);
        if (matches.length === 0) return false;

        codeEditor._highlightMatches(editor, matches);
        var cursor = editor.getCursor();
        var idx = 0;
        // Find next match after cursor
        for (var i = 0; i < matches.length; i++) {
            if (CodeMirror.cmpPos(matches[i].from, cursor) > 0 ||
                (CodeMirror.cmpPos(matches[i].from, cursor) === 0 && CodeMirror.cmpPos(matches[i].to, cursor) > 0)) {
                idx = i;
                break;
            }
            if (i === matches.length - 1) idx = 0; // wrap around
        }
        editor._frIndex = idx;
        editor.setSelection(matches[idx].from, matches[idx].to);
        editor.scrollIntoView({ from: matches[idx].from, to: matches[idx].to }, 60);
        return true;
    },

    /// Find the previous match, select it, and scroll into view. Returns true if found.
    findPrev: function (text, caseSensitive, useRegex, wholeWord) {
        var editor = codeEditor._getActiveEditor();
        if (!editor) return false;
        var matches = codeEditor._findMatches(editor, text, caseSensitive, useRegex, wholeWord);
        if (matches.length === 0) return false;

        codeEditor._highlightMatches(editor, matches);
        var cursor = editor.getCursor('from');
        var idx = matches.length - 1;
        for (var i = matches.length - 1; i >= 0; i--) {
            if (CodeMirror.cmpPos(matches[i].from, cursor) < 0) {
                idx = i;
                break;
            }
            if (i === 0) idx = matches.length - 1; // wrap around
        }
        editor._frIndex = idx;
        editor.setSelection(matches[idx].from, matches[idx].to);
        editor.scrollIntoView({ from: matches[idx].from, to: matches[idx].to }, 60);
        return true;
    },

    /// Replace the current selection if it matches, then move to next. Returns true if replaced.
    replaceCurrent: function (text, replacement, caseSensitive, useRegex, wholeWord) {
        var editor = codeEditor._getActiveEditor();
        if (!editor) return false;
        var sel = editor.getSelection();
        if (!sel) return false;

        // Check if current selection matches the search
        var isMatch = false;
        if (useRegex) {
            try {
                var flags = caseSensitive ? '' : 'i';
                isMatch = new RegExp('^(?:' + text + ')$', flags).test(sel);
            } catch (e) { return false; }
        } else if (wholeWord) {
            isMatch = caseSensitive ? sel === text : sel.toLowerCase() === text.toLowerCase();
        } else {
            isMatch = caseSensitive ? sel === text : sel.toLowerCase() === text.toLowerCase();
        }

        if (!isMatch) {
            // Try to find next match first
            return codeEditor.findNext(text, caseSensitive, useRegex, wholeWord);
        }

        editor.replaceSelection(replacement);
        // Find next match
        codeEditor.findNext(text, caseSensitive, useRegex, wholeWord);
        return true;
    },

    /// Replace all matches in the active editor. Returns the count of replacements.
    replaceAll: function (text, replacement, caseSensitive, useRegex, wholeWord) {
        var editor = codeEditor._getActiveEditor();
        if (!editor) return 0;
        var matches = codeEditor._findMatches(editor, text, caseSensitive, useRegex, wholeWord);
        if (matches.length === 0) return 0;

        // Replace from end to start to preserve positions
        editor.operation(function () {
            for (var i = matches.length - 1; i >= 0; i--) {
                editor.replaceRange(replacement, matches[i].from, matches[i].to);
            }
        });

        codeEditor._clearSearchOverlay(editor);
        return matches.length;
    }
};

// Helper to focus an element from Blazor
window.focusElement = function (element) {
    if (element && element.focus) element.focus();
};

