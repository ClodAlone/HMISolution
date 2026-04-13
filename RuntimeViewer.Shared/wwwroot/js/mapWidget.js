// Map Widget â€” Leaflet.js interop for geographic & floor plan maps
const _maps = {};
let _leafletLoaded = false;
let _leafletLoading = null;

async function ensureLeaflet() {
    if (_leafletLoaded) return;
    if (_leafletLoading) { await _leafletLoading; return; }
    _leafletLoading = new Promise((resolve, reject) => {
        // CSS
        if (!document.querySelector('link[href*="leaflet.css"]')) {
            const css = document.createElement('link');
            css.rel = 'stylesheet';
            css.href = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.css';
            css.integrity = 'sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/miZyoHS5obTRR9BMY=';
            css.crossOrigin = '';
            document.head.appendChild(css);
        }
        // JS
        if (window.L) { _leafletLoaded = true; resolve(); return; }
        const script = document.createElement('script');
        script.src = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.js';
        script.integrity = 'sha256-20nQCchB9co0qIjJZRGuk2/Z9VM+kNiyxNV1lvTlZBo=';
        script.crossOrigin = '';
        script.onload = () => { _leafletLoaded = true; resolve(); };
        script.onerror = () => reject(new Error('Failed to load Leaflet.js'));
        document.head.appendChild(script);
    });
    await _leafletLoading;
}

function createMarkerIcon(shape, color, size, iconText) {
    const s = size || 24;
    const half = s / 2;
    let svg;
    switch (shape) {
        case 'circle':
            svg = `<svg xmlns="http://www.w3.org/2000/svg" width="${s}" height="${s}" viewBox="0 0 ${s} ${s}">
                <circle cx="${half}" cy="${half}" r="${half - 2}" fill="${color}" stroke="#0f172a" stroke-width="2" opacity="0.9"/>
                ${iconText ? `<text x="${half}" y="${half + 4}" text-anchor="middle" fill="white" font-size="${s * 0.4}">${iconText}</text>` : ''}
            </svg>`;
            break;
        case 'square':
            svg = `<svg xmlns="http://www.w3.org/2000/svg" width="${s}" height="${s}" viewBox="0 0 ${s} ${s}">
                <rect x="2" y="2" width="${s - 4}" height="${s - 4}" rx="3" fill="${color}" stroke="#0f172a" stroke-width="2" opacity="0.9"/>
                ${iconText ? `<text x="${half}" y="${half + 4}" text-anchor="middle" fill="white" font-size="${s * 0.4}">${iconText}</text>` : ''}
            </svg>`;
            break;
        case 'diamond':
            svg = `<svg xmlns="http://www.w3.org/2000/svg" width="${s}" height="${s}" viewBox="0 0 ${s} ${s}">
                <polygon points="${half},2 ${s - 2},${half} ${half},${s - 2} 2,${half}" fill="${color}" stroke="#0f172a" stroke-width="2" opacity="0.9"/>
                ${iconText ? `<text x="${half}" y="${half + 4}" text-anchor="middle" fill="white" font-size="${s * 0.35}">${iconText}</text>` : ''}
            </svg>`;
            break;
        default: // pin
            svg = `<svg xmlns="http://www.w3.org/2000/svg" width="${s}" height="${Math.round(s * 1.4)}" viewBox="0 0 24 34">
                <path d="M12 0C5.4 0 0 5.4 0 12c0 9 12 22 12 22s12-13 12-22C24 5.4 18.6 0 12 0z" fill="${color}" stroke="#0f172a" stroke-width="1.5"/>
                <circle cx="12" cy="12" r="5" fill="white" opacity="0.9"/>
                ${iconText ? `<text x="12" y="15" text-anchor="middle" fill="${color}" font-size="8" font-weight="bold">${iconText}</text>` : ''}
            </svg>`;
            break;
    }
    const iconUrl = 'data:image/svg+xml;base64,' + btoa(svg);
    const iconSize = shape === 'pin' ? [s, Math.round(s * 1.4)] : [s, s];
    const iconAnchor = shape === 'pin' ? [s / 2, Math.round(s * 1.4)] : [s / 2, s / 2];
    return L.icon({ iconUrl, iconSize, iconAnchor, popupAnchor: [0, -s / 2] });
}

export async function initMap(containerId, options, dotNetRef) {
    await ensureLeaflet();

    const container = document.getElementById(containerId);
    if (!container) return;

    // Ensure container has size
    if (container.offsetWidth === 0 || container.offsetHeight === 0) {
        await new Promise(r => setTimeout(r, 100));
    }

    const mapOptions = {
        zoomControl: options.showZoomControl,
        dragging: options.interactive,
        scrollWheelZoom: options.interactive,
        doubleClickZoom: options.interactive,
        touchZoom: options.interactive,
        boxZoom: options.interactive,
        keyboard: options.interactive,
        attributionControl: false
    };

    const map = L.map(containerId, mapOptions);
    const markers = {};

    if (options.mode === 'floorplan') {
        // Floor plan mode â€” CRS.Simple with image overlay
        const bounds = [[0, 0], [options.floorPlanHeight, options.floorPlanWidth]];
        map.setView([options.floorPlanHeight / 2, options.floorPlanWidth / 2], options.zoom);

        // Use simple CRS for pixel coordinates
        map.options.crs = L.CRS.Simple;
        // Re-initialize view after CRS change
        map.setMaxBounds([[-100, -100], [options.floorPlanHeight + 100, options.floorPlanWidth + 100]]);

        if (options.floorPlanImage) {
            L.imageOverlay(options.floorPlanImage, bounds).addTo(map);
        } else {
            // Draw a grid as placeholder
            const gridBg = L.rectangle(bounds, {
                fillColor: '#1e293b', fillOpacity: 0.8,
                color: '#475569', weight: 1
            }).addTo(map);
        }

        map.fitBounds(bounds);

        // Add markers using PosX/PosY (mapped to [y, x] in CRS.Simple)
        for (const m of (options.markers || [])) {
            const icon = createMarkerIcon(m.shape, m.color, m.size, m.icon);
            const marker = L.marker([m.posY, m.posX], { icon }).addTo(map);
            if (m.showPopup && m.label) {
                marker.bindPopup(`<b>${m.label}</b>`, {
                    className: 'map-widget-popup'
                });
            }
            if (m.label) marker.bindTooltip(m.label, { direction: 'top', offset: [0, -m.size / 2] });
            // Click handler for popup screen
            if (m.hasPopupScreen && dotNetRef) {
                marker.on('click', () => {
                    dotNetRef.invokeMethodAsync('OnMarkerClicked', m.id);
                });
            }
            markers[m.id] = { marker, data: m };
        }
    } else {
        // Geographic mode â€” standard tile layer
        map.setView([options.centerLat, options.centerLng], options.zoom);

        L.tileLayer(options.tileUrl, {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OSM</a>'
        }).addTo(map);

        // Add markers using Lat/Lng
        for (const m of (options.markers || [])) {
            const icon = createMarkerIcon(m.shape, m.color, m.size, m.icon);
            const marker = L.marker([m.lat, m.lng], { icon }).addTo(map);
            if (m.showPopup && m.label) {
                marker.bindPopup(`<b>${m.label}</b>`, {
                    className: 'map-widget-popup'
                });
            }
            if (m.label) marker.bindTooltip(m.label, { direction: 'top', offset: [0, -m.size / 2] });
            // Click handler for popup screen
            if (m.hasPopupScreen && dotNetRef) {
                marker.on('click', () => {
                    dotNetRef.invokeMethodAsync('OnMarkerClicked', m.id);
                });
            }
            markers[m.id] = { marker, data: m };
        }
    }

    _maps[containerId] = { map, markers, options, dotNetRef };

    // Invalidate size after a short delay (Blazor rendering)
    setTimeout(() => map.invalidateSize(), 200);
}

export function updateMarkers(containerId, updates) {
    const entry = _maps[containerId];
    if (!entry) return;

    for (const u of updates) {
        const m = entry.markers[u.id];
        if (!m) continue;

        // Update icon color
        if (u.color !== m.data.color) {
            const newIcon = createMarkerIcon(m.data.shape, u.color, m.data.size, m.data.icon);
            m.marker.setIcon(newIcon);
            m.data.color = u.color;
        }

        // Update popup content
        if (u.label && m.data.showPopup !== false) {
            m.marker.setPopupContent(`<b>${u.label}</b>${u.value ? `<br/>Value: ${u.value}` : ''}`);
            m.marker.setTooltipContent(u.label);
        }

        // Update position if dynamic variables provided values
        if (entry.options.mode === 'floorplan') {
            if (u.posX != null && u.posY != null) {
                m.marker.setLatLng([u.posY, u.posX]);
            }
        } else {
            if (u.lat != null && u.lng != null) {
                m.marker.setLatLng([u.lat, u.lng]);
            }
        }
    }
}

export function destroyMap(containerId) {
    const entry = _maps[containerId];
    if (entry) {
        entry.map.remove();
        delete _maps[containerId];
    }
}