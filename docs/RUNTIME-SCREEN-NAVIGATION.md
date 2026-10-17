# Runtime screen transitions, swipe navigation and edge layouts

Select a screen in the editor, then use **Properties > Runtime navigation** and
**Runtime layout**. The generic property grid exposes the same settings in
**Runtime Navigation** and **Runtime Layout**. Settings are saved with each screen
(including externally stored screen resources).

## Page transitions

| Property | Values / default | Meaning |
| --- | --- | --- |
| `NavigationTransition` | `none` (default), `fade`, `slide-left`, `slide-right`, `slide-up`, `slide-down` | Entrance effect when navigating to this page. Directions describe movement of the content. |
| `NavigationDurationMs` | 280; runtime clamps to 0-2000 | Animation duration in milliseconds. Zero is instant. |
| `SwipeUpScreen`, `SwipeDownScreen`, `SwipeLeftScreen`, `SwipeRightScreen` | Empty (disabled), a screen name, or `$back` | Explicit destination for each gesture; no implicit dependence on screen list order. |

Ordinary navigation uses the destination's effect. Back reverses the actual effect
used to enter the outgoing page (including swipe overrides). Browser history
returns to earlier screen routes are also recognized. Swipe navigation slides in
the gesture direction; `$back` reverses the previous entrance effect. The Back
command retains its existing startup-page fallback when history is empty.

Transitions work with screen preloading enabled or disabled. Only the incoming
page accepts interaction during a transition; outgoing pages are inert and hidden
from assistive technology. Rapid navigation cancels previous animations. Hidden
preloaded pages remain mounted, retaining their per-page alias context.

Existing projects retain instant navigation because `NavigationTransition` defaults
to `none`. System `prefers-reduced-motion` disables page and panel animation without
disabling navigation. No animation library is required.

## Persistent layouts and swipe panels

| Property | Values / default | Meaning |
| --- | --- | --- |
| `RuntimeDock` | `none` (default), `top`, `bottom`, `left`, `right` | `none` is a normal page. Other values make this a global layout screen. |
| `DockMode` | `fixed` (default), `swipe` | Fixed screens reserve space around the main page. Swipe panels overlay it on demand. |
| `DockSize` | 160; runtime clamps to 24-1200 | Height for top/bottom, width for left/right, in CSS pixels. |
| `DockOrder` | 0 | Ascending order within an edge, then screen name. |

Top/bottom layouts span the full workspace width; left/right layouts occupy the
remaining middle row. Fixed edge groups are capped at 35% of the viewport dimension
and scroll if their contents exceed that space. Swipe panels are capped at 85% of
the workspace dimension. These limits keep the main page or dismissal backdrop
reachable on small displays.

Layout screens appear across normal runtime pages, including kiosk mode. They do
not become startup pages or appear in page navigation even if `ShowInNavigation`
is true. Their `AllowedGroups` restrictions are applied before rendering, including
handles, fixed content and swipe panels. Group-restricted cached pages are also
excluded from rendering.

Swipe inward starting within 28 CSS pixels of an edge to open its first authorized
swipe panel (ordered by `DockOrder`, then name). If multiple panels share the edge,
each also has its own labeled handle. Handles act as pull tabs: tap one, or swipe
inward starting on it, to open that specific panel. Existing `NavigateScreen` commands and
navigation-button targets can open a swipe layout by name. Navigating to a fixed
layout does not replace the current page because that layout is already displayed.

Close an overlay with an outward swipe, the Close button, the backdrop, or Escape.
Opening a panel moves keyboard focus into it, traps Tab within the panel, and makes
the workspace background inert. Closing restores focus to the opener when it is
still present. Popup and modal commands continue to use their existing behavior.

## Gesture safety

- Touch swipes require at least 64 CSS pixels, a dominant axis ratio of 1.35, and a
  duration of at most 900ms. Multitouch and canceled gestures do not navigate.
- Interactive SVG symbols, input widgets, buttons, links, and scrollable content
  retain their normal gestures. Add `data-no-swipe` to custom content to opt out.
- Native scrolling is prevented only for a recognized mapped gesture on an eligible
  surface. No global `touch-action: none` rule is applied.
- Edge-panel gestures take priority over page destinations at the same edge.
- Destinations are checked for existence and group access; failures show a runtime
  notification. Editor project validation reports broken swipe links and unsupported
  presentation settings. The Screen Navigation Map includes explicit swipe links.

## Example

```json
{
  "Screens": [
    {
      "Name": "Overview",
      "NavigationTransition": "slide-left",
      "NavigationDurationMs": 300,
      "SwipeUpScreen": "Detail"
    },
    {
      "Name": "Detail",
      "NavigationTransition": "slide-up",
      "SwipeDownScreen": "$back"
    },
    {
      "Name": "Header",
      "RuntimeDock": "top",
      "DockMode": "fixed",
      "DockSize": 72
    },
    {
      "Name": "Tools",
      "RuntimeDock": "right",
      "DockMode": "swipe",
      "DockSize": 320
    }
  ]
}
```

Design each layout screen using the normal SVG or responsive screen editor. Its
existing fit settings and symbols still apply; the containing layout determines
its available runtime space. Layouts currently apply globally rather than being
assigned to individual pages. AR overlays and embedded screens are not runtime
page workspaces and retain their existing presentation.
