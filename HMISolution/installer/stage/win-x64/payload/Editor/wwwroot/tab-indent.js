// Handle Tab key in textareas marked with the "tab-enabled" class.
// Inserts 4 spaces at the caret instead of moving focus.
document.addEventListener("keydown", function (e) {
    if (e.key !== "Tab") return;
    var el = e.target;
    if (!el || el.tagName !== "TEXTAREA" || !el.classList.contains("tab-enabled")) return;

    e.preventDefault();

    var start = el.selectionStart;
    var end = el.selectionEnd;
    var indent = "    ";

    if (e.shiftKey) {
        // Shift+Tab: remove up to 4 leading spaces on the current line
        var before = el.value.substring(0, start);
        var lineStart = before.lastIndexOf("\n") + 1;
        var linePrefix = el.value.substring(lineStart, start);
        var removed = 0;
        for (var i = 0; i < 4 && i < linePrefix.length && linePrefix[i] === " "; i++) removed++;
        if (removed > 0) {
            el.value = el.value.substring(0, lineStart) + el.value.substring(lineStart + removed);
            el.selectionStart = el.selectionEnd = start - removed;
            el.dispatchEvent(new Event("input", { bubbles: true }));
            el.dispatchEvent(new Event("change", { bubbles: true }));
        }
    } else {
        // Tab: insert 4 spaces
        el.value = el.value.substring(0, start) + indent + el.value.substring(end);
        el.selectionStart = el.selectionEnd = start + indent.length;
        el.dispatchEvent(new Event("input", { bubbles: true }));
        el.dispatchEvent(new Event("change", { bubbles: true }));
    }
});
