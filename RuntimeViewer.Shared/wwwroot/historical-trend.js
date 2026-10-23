export function revealSelectedRow(container) {
    if (!container || !container.isConnected) return;
    const row = container.querySelector('tbody tr.historical-selected');
    if (!row) return;

    const bounds = container.getBoundingClientRect();
    const scale = container.offsetHeight > 0 ? bounds.height / container.offsetHeight : 1;
    if (scale <= 0) return;
    const header = container.querySelector('thead');
    const headerHeight = header ? header.getBoundingClientRect().height : 0;
    const top = bounds.top + container.clientTop * scale + headerHeight;
    const bottom = bounds.top + (container.clientTop + container.clientHeight) * scale;
    const selected = row.getBoundingClientRect();
    if (selected.top < top) {
        container.scrollTop += (selected.top - top) / scale;
    } else if (selected.bottom > bottom) {
        container.scrollTop += (selected.bottom - bottom) / scale;
    }
}
