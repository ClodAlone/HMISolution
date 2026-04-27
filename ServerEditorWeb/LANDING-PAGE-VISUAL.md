# 🎨 Landing Page - Visual Reference

## Layout Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                 │
│                     [AI Core HMI Logo - Animated]               │
│                   AI Core HMI Server Editor                     │
│     Professional Industrial Automation Platform with AI         │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────┐  │
│  │  📄               │  │  📂              │  │  🎯          │  │
│  │ Create New       │  │ Open Existing    │  │ Sample       │  │
│  │ Project          │  │ Project          │  │ Projects     │  │
│  │                  │  │                  │  │              │  │
│  │ Start from       │  │ Browse and open  │  │ Explore pre- │  │
│  │ scratch          │  │ a project file   │  │ built demos  │  │
│  │                  │  │                  │  │              │  │
│  │ [New] Ctrl+N     │  │ [Open] Ctrl+O    │  │ [Browse]     │  │
│  └──────────────────┘  └──────────────────┘  └──────────────┘  │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│                      Recent Projects                            │
│                                                                 │
│  📋 FactoryAutomation.hmi    → C:\Projects\Factory\           │
│  📋 WaterTreatment.hmi       → C:\Projects\WaterPlant\        │
│  📋 BuildingHVAC.hmi         → C:\Projects\HVAC\              │
│  📋 PackagingLine.hmi        → C:\Projects\Packaging\         │
│  📋 EnergyMonitor.hmi        → C:\Projects\Energy\            │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│                      Getting Started                            │
│                                                                 │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐          │
│  │   🚀    │  │   📚    │  │   🎓    │  │   💬    │          │
│  │ Quick   │  │  Docs   │  │ Video   │  │Community│          │
│  │ Start   │  │         │  │Tutorial │  │& Support│          │
│  │ Guide   │  │         │  │         │  │         │          │
│  └─────────┘  └─────────┘  └─────────┘  └─────────┘          │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│     GitHub • FDA 21 CFR Part 11 • Keyboard Shortcuts           │
│              Version 1.0.0 • .NET 10 • OPC UA                   │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Color Scheme

```
Background:     Linear Gradient (#667eea → #764ba2)
Cards:          White (#ffffff)
Primary Text:   White on gradient, Dark Gray (#2d3748) on cards
Secondary Text: Light Gray (#718096)
Accent:         Purple (#667eea)
Hover:          Lifted with shadow
```

## Animation Effects

### On Page Load
```
Header:          Fade In Down (0.6s)
Action Cards:    Fade In Up (0.6s, delay 0.2s)
Recent Section:  Fade In Up (0.6s, delay 0.4s)
Getting Started: Fade In Up (0.6s, delay 0.4s)
Footer:          Fade In (0.6s, delay 0.6s)
```

### On Hover
```
Action Cards:    Lift up (-8px) + Shadow increase
Recent Items:    Slide right (8px) + Background gradient
Info Cards:      Lift up (-4px) + Shadow increase
Action Icons:    Faster bounce animation
```

### Continuous
```
Logo:            Subtle pulse (scale 1 → 1.05)
Action Icons:    Slow bounce (up 10px)
```

## Responsive Breakpoints

### Desktop (> 768px)
```
Container:       max-width: 1200px
Action Grid:     3 columns
Info Grid:       4 columns
Padding:         60px 40px
Title Size:      3rem
```

### Mobile (≤ 768px)
```
Container:       full width
Action Grid:     1 column
Info Grid:       1 column
Padding:         40px 20px
Title Size:      2rem
```

## Typography

```
Landing Title:      3rem, font-weight: 700, white
Landing Subtitle:   1.25rem, rgba(255,255,255,0.9)
Section Title:      1.75rem, font-weight: 600, white
Action Title:       1.5rem, font-weight: 600
Action Description: 0.95rem, color: #718096
Info Title:         1.1rem, font-weight: 600
Footer Links:       default, font-weight: 500
Footer Version:     0.9rem, rgba(255,255,255,0.7)
```

## Card Specifications

### Main Action Cards
```
Dimensions:  300px min-width, auto height
Padding:     32px
Border:      None
Radius:      16px
Shadow:      0 4px 20px rgba(0,0,0,0.1)
Top Border:  4px gradient (hidden, shown on hover)
```

### Primary Action Card (New Project)
```
Background:  Linear gradient (#667eea → #764ba2)
Text Color:  White
Button BG:   rgba(255,255,255,0.2)
```

### Recent File Items
```
Padding:     16px 20px
Radius:      12px
Background:  White
Shadow:      0 2px 8px rgba(0,0,0,0.1)
Hover:       Transform translateX(8px)
```

### Info Cards
```
Padding:     24px
Radius:      12px
Background:  rgba(255,255,255,0.95)
Shadow:      0 2px 8px rgba(0,0,0,0.1)
Text Align:  Center
```

## Icon Sizes

```
Logo SVG:       80x80px
Action Icons:   3rem (48px)
Recent Icons:   1.5rem (24px)
Info Icons:     2.5rem (40px)
Arrow Icon:     1.5rem (24px)
```

## Spacing

```
Section Margin:     60px bottom
Grid Gap:           24px (actions), 20px (info), 12px (recent)
Header Margin:      60px bottom
Footer Margin:      80px top
Footer Padding:     40px top
```

## Shadows

```
Card Default:   0 4px 20px rgba(0,0,0,0.1)
Card Hover:     0 12px 40px rgba(102,126,234,0.3)
Primary Hover:  0 12px 40px rgba(102,126,234,0.5)
Recent Default: 0 2px 8px rgba(0,0,0,0.1)
Recent Hover:   0 4px 16px rgba(102,126,234,0.2)
Info Default:   0 2px 8px rgba(0,0,0,0.1)
Info Hover:     0 8px 24px rgba(102,126,234,0.2)
```

## User Interactions

### Click Actions
```
Create New Project    → NewFileMenu()
Open Project          → OpenFileBrowser()
Sample Projects       → OpenSamplesDialog()
Recent File           → QuickLoadFile(path)
Quick Start Guide     → Opens GitHub Wiki
Documentation         → Opens GitHub README
Video Tutorials       → Opens GitHub Wiki Tutorials
Community & Support   → Opens GitHub Discussions
Keyboard Shortcuts    → Opens shortcuts guide
```

### Hover States
```
Action Cards          → Lift animation + shadow
Recent Items          → Slide animation + gradient
Info Cards            → Lift animation
Links                 → Opacity 0.8 + underline
```

## Accessibility

```
Semantic HTML:     ✅ Proper heading hierarchy
Keyboard Nav:      ✅ All interactive elements focusable
Screen Readers:    ✅ Meaningful text descriptions
Color Contrast:    ✅ WCAG AA compliant
Focus Indicators:  ✅ Browser default focus rings
```

## Browser Support

```
Modern Browsers:   ✅ Chrome, Firefox, Edge, Safari (latest 2 versions)
CSS Features:      ✅ Grid, Flexbox, Gradients, Transforms
Animations:        ✅ CSS Keyframes, Transitions
Fallbacks:         ✅ Graceful degradation for older browsers
```

## Performance

```
Initial Load:      Fast (scoped CSS, no heavy images)
Animations:        Hardware accelerated (transform, opacity)
Images:            SVG only (logo)
No External:       No external image dependencies
Bundle Size:       < 10KB (CSS + component)
```

## State Management

```
Project List:      From Editor.RecentFiles
Active Check:      Editor.ActiveProject == null
Browser State:     !_showFileBrowser
Authentication:    Handled before landing page
```

---

**Visual Style**: Modern, clean, professional  
**Inspiration**: VS Code, GitHub, modern SaaS apps  
**Feel**: Welcoming, helpful, efficient
