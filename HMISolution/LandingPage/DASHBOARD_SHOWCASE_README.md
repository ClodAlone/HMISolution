# 🎛️ Dashboard Showcase — AI Core HMI

## Overview

A brand-new interactive dashboard showcase has been added to the AI Core HMI GitHub Pages landing page, featuring fancy real-time widgets, gauges, charts, alarms, and industrial symbols.

## Files Added

### 1. **`dashboard-showcase.html`** — Main Demo Page
A fully interactive, standalone dashboard demonstration featuring:

#### 📊 **Production Overview Tab**
- **🌡️ Temperature Control** — Gauge widget with min/max ranges
- **📊 Pressure Level** — Real-time pressure monitoring with flow rate
- **⚙️ Production Rate** — Weekly production bar chart
- **💚 System Health Status** — CPU, memory, network I/O, uptime metrics
- **📈 Real-time Trends** — Throughput, efficiency, and SLA uptime indicators
- **🔗 Connected Devices** — OPC UA servers and active variables count

#### ⚡ **Energy Monitoring Tab**
- **⚡ Power Consumption** — Live kW gauge with peak metrics
- **🌱 Energy Efficiency** — Optimization score and CO₂ saved tracking
- **📡 Power Distribution** — Area-based power usage charts
- **💰 Cost Analysis** — Daily/weekly/monthly breakdown with budget status

#### 🚨 **Alarm Management Tab**
- **Alarm Summary** — Critical, Warning, Info, and Resolved counts
- **Recent Alarms Table** — Detailed alarm history with timestamps, sources, levels, and status

#### ⚙️ **Industrial Symbols Tab**
- 16 industrial equipment icons including:
  - Power sources, batteries, motors
  - Compressors, pumps, heaters, coolers
  - Turbines, sensors, valves, fans
  - Factories, meters, and control devices

## Features

### 🎨 **Beautiful Design**
- **Dark theme** with gradient accents matching AI Core HMI branding
- **Smooth animations** with hover effects and transitions
- **Responsive layout** — works seamlessly on desktop, tablet, and mobile

### ⚡ **Interactive Elements**
- **Tab switching** — organize dashboards by category
- **Live updates** — simulated real-time metrics that change every 2 seconds
- **Gauge animations** — conic-gradient gauges with animated pulses
- **Status indicators** — colored status lights with pulse animations

### 📊 **Rich Widgets**
- **Gauges** — Circular progress indicators with min/max ranges
- **Bar Charts** — Weekly/area-based data visualization
- **Metric Lists** — Key performance indicators with icons
- **Status Grids** — Multi-item status displays
- **Alarm Tables** — Detailed incident logs with severity levels
- **Symbols** — Interactive industrial equipment icons

## Integration with Main Landing Page

Two new links have been added to **`index.html`**:

1. **Hero Section** — "🎛️ Live Dashboard Demo" button
   - Location: Header hero-actions
   - Encourages immediate engagement

2. **New Dashboard Section** — Full showcase section with:
   - **Production Overview** — Real-time monitoring capabilities
   - **Energy Monitoring** — Cost tracking and efficiency metrics
   - **Alarm Management** — Comprehensive incident handling
   - **Call-to-Action Button** — Links to full interactive demo

## Styling Updates

**`style.css`** enhancements:
- `.card-icon` — Large emoji icons for dashboard feature cards
- `.dashboard-cta` — Call-to-action container with gradient background
- Maintains existing color scheme and typography

## Live Features

### Real-Time Simulation
```javascript
// Metrics update every 2 seconds
setInterval(updateMetrics, 2000);

// Temperature fluctuates ±0.25°C
// CPU usage changes ±3%
// Realistic operational behavior
```

### Navigation
- **Smooth tab switching** with fade animations
- **Back to home** button on showcase page
- **Mobile-optimized** navigation

## Color Scheme

Uses the established AI Core HMI palette:
- **Primary Accent**: `#58a6ff` (Blue)
- **Secondary Accent**: `#3fb950` (Green)
- **Success**: `#3fb950` (Green)
- **Warning**: `#d29922` (Amber)
- **Danger**: `#f85149` (Red)
- **Background**: `#0b0f19` (Dark)
- **Surface**: `#131927` (Medium-Dark)

## File Structure

```
LandingPage/
├── index.html                    (Updated with dashboard section & links)
├── style.css                     (Updated with dashboard styles)
├── dashboard-showcase.html       (New interactive dashboard)
├── DASHBOARD_SHOWCASE_README.md  (This file)
├── pricing.html                  (Existing)
└── googlea3d3c998ffd42a83.html  (Google verification)
```

## How to Use

1. **On GitHub Pages**: Visit https://clodalone.github.io/HMISolution/
2. **Click** "🎛️ Live Dashboard Demo" in the hero section
3. **Explore tabs**: Production, Energy, Alarms, Symbols
4. **Watch metrics update** in real-time
5. **Hover widgets** to see animations and visual feedback

## Browser Compatibility

- ✅ Chrome/Edge 90+
- ✅ Firefox 88+
- ✅ Safari 14+
- ✅ Mobile browsers (iOS Safari, Chrome Mobile)

## Performance

- **Zero external dependencies** — Pure HTML/CSS/JavaScript
- **Lightweight** — ~15 KB uncompressed
- **Fast animations** — GPU-accelerated CSS transforms
- **Responsive** — Efficient media queries for all screen sizes

## Future Enhancements

Potential additions:
- WebSocket integration for real live data from actual OPC UA servers
- More industrial symbols (compressors, heat exchangers, etc.)
- Advanced charting with Chart.js or similar
- Export capabilities (PNG, PDF)
- Customizable dashboards
- Dark/light mode toggle

## Notes

- All styling is inline or in `style.css` for easy customization
- No JavaScript frameworks required (Vanilla JS only)
- Metrics are simulated; ready to integrate with real data sources
- Industrial symbols use Unicode emoji for broad compatibility

---

**Created**: January 2025
**For**: AI Core HMI — AI-Powered Open-Source Industrial HMI/SCADA Platform
**GitHub**: https://github.com/ClodAlone/HMISolution
