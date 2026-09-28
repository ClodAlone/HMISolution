# Augmented Reality Runtime Mode

## Overview

The RuntimeViewer supports a third startup mode (alongside normal and `--kiosk`): `--ar`,
which opens the device camera, streams frames to a **local** object-recognition (OR) server
for low-latency detection, and overlays the mapped screen beside each recognized object —
with a bounding box drawn around it and a transparent screen background so the operator still
sees the physical object through the overlay.

```
RuntimeViewer.exe nodes.json --ar
RuntimeViewer.Desktop.exe nodes.json --ar
```

Navigating to `/ar` directly also works if the AR config is enabled for the project.

## Why a separate local OR server?

Real-time AR requires per-frame object detection with minimal round-trip latency. Routing
frames through the cloud or the main OPC UA server process adds latency and network
dependency. `ArObjectServer` is a small, standalone ASP.NET Core process that:

- Runs on the same machine (or LAN) as the AR client.
- Loads a YOLOv8 ONNX model once at startup (CPU or CUDA).
- Exposes a single `/detect` endpoint: POST a JPEG frame, get bounding boxes back immediately.
- Has no camera-streaming/session state of its own — the AR client (browser camera capture)
  controls its own capture rate (`AugmentedRealityConfig.TargetFps`, default 10 fps) and
  simply posts frames as fast as it can process the previous response.

Run it once per site/workstation:

```
ArObjectServer.exe yolov8n.onnx --port=8098 --cuda
```

**Bundled default model**: `ArObjectServer` ships with a standard `yolov8n.onnx` (80 COCO
classes, ~12 MB) copied next to its executable at build time, so `/health` reports `ready`
immediately without any manual setup. It was exported with:

```
pip install ultralytics
python -c "from ultralytics import YOLO; YOLO('yolov8n.pt').export(format='onnx', imgsz=640, opset=12, simplify=True)"
```

Replace `ArObjectServer/yolov8n.onnx` with your own general-purpose or custom-trained model
(matching the same 640x640 input / 80-class-or-custom output shape) to override the default —
see "Custom object detection training" below for training a model on your own classes.

**Auto-start**: When `AugmentedRealityConfig.OrServerUrl` points at localhost (the default,
`http://127.0.0.1:8098`) and `AugmentedRealityConfig.AutoStartOrServer` is `true` (the default),
RuntimeViewer automatically launches `ArObjectServer.exe` as a child process on startup (using
`OrModelPath`/`OrServerUseCuda` from the same config) and stops it on shutdown — you don't need
to start it manually in that case. Set `AutoStartOrServer` to `false` if you run the OR server
yourself (e.g. on a separate machine reachable over the LAN, or managed as its own service).
RuntimeViewer looks for `ArObjectServer(.exe)` next to its own executable, in a sibling
`ArObjectServer` folder (installer/dev layout), or via the `HMI_ROOT` environment variable —
matching the same search strategy ServerEditorWeb uses to find RuntimeViewer itself.

## Data model

- `ServerSettings.AugmentedReality` (`AugmentedRealityConfig`) — global AR settings: OR server
  URL, target FPS, confidence threshold, bounding box color, screen overlay opacity/placement,
  detection-hold time (avoids flicker), and camera facing.
- `NodeModel.ArObjectMappings` (`List<ArObjectMapping>`) — associates a recognized object class
  (YOLO label, or a custom-trained class) with:
  - `ScreenName` — the screen to render beside the object.
  - `AliasMapName` — an existing `VariableAliasMap` for template screens.
  - `ParameterFilePath` — a project-relative JSON file mapping alias placeholders to real OPC
    variable paths for one specific physical instance (e.g. `params/motor1.json`).
  - `InstanceParameterFiles` — a dictionary from an *instance key* (QR code / AprilTag / tracker
    ID reported by the OR server) to a parameter file, so the *same* visual class (e.g. "motor")
    can resolve to a different physical instance depending on which specific unit is recognized.

Parameter file format (same shape as `VariableAliasMap`):

```json
{ "Mappings": { "Motor": "Plant.Line1.Motor1", "Status": "Plant.Line1.Motor1.Status" } }
```

## Client pipeline (`/ar` page)

1. `ar-camera.js` opens `getUserMedia` and grabs one JPEG frame per detection cycle.
2. `ArDetectionClient` posts the frame to the local OR server and parses bounding boxes.
3. Detections are matched against `ArObjectMappings` by object class + confidence.
4. `ArParameterFileService` resolves the alias map (instance parameter file > single parameter
   file > named alias map).
5. The matched `ScreenConfig` is rendered live via the existing `ScreenRenderer` component,
   positioned beside the detection's bounding box, with configurable transparency.
6. Detections are held briefly (`DetectionHoldSeconds`) after they disappear from a frame to
   avoid overlay flicker from momentary misses.

## Disambiguating multiple instances of the same class

When the camera sees two objects of the same YOLO class (e.g. two motors), classification alone
cannot tell them apart — both have the same label and similar confidence. Two independent
mechanisms work together to solve this:

- **Instance identity (which physical unit is it?)** — `ArObjectServer` optionally decodes a
  QR/DataMatrix/Code128 marker (via ZXing.Net) cropped from each YOLO bounding box (with a margin)
  and returns it as `Detection.InstanceKey`. This is the *only* reliable way to know "this is
  Motor #1 vs Motor #2" — printed instance markers are the recommended approach for any screen
  that needs `InstanceParameterFiles` lookups. Pass `?decodeMarkers=false` to `/detect` to skip
  this step if markers aren't used (saves CPU).
- **Overlay tracking stability (keeping boxes from colliding on screen)** — independent of
  markers, the `/ar` client assigns each detection a per-frame track using: (a) the reported
  `InstanceKey` when present, or (b) nearest-centroid matching against the previous frame's boxes
  of the same class otherwise. This guarantees two simultaneous same-class detections always get
  separate overlay entries and never overwrite one another, even with no marker present.

If no marker is decoded, both instances render the *same* screen/parameters (the mapping's
`ParameterFilePath` or `AliasMapName`) — visually distinct overlays, but not instance-specific
data. Attach a printed marker to each physical unit whenever per-instance parameters matter.

## Freezing the view at runtime

The AR toolbar (top-right of `/ar`) has a **❄ Freeze** button. It pauses the live camera feed
(`<video>.pause()`) and stops sending new frames to the OR server, leaving the current bounding
boxes and screen overlays exactly as they were. This lets an operator stabilize the picture —
e.g. hold the tablet steady, or read a screen at leisure — without the overlay drifting or
disappearing due to hand shake or having to keep the object in frame. Press **▶ Resume** to
restart live capture immediately; the camera stream itself is never stopped, so resuming has no
startup delay.

## Returning to the AR view (screen navigation history)

Screens rendered as AR overlays never leave the `/ar` route — but a symbol on an overlaid screen
can still fire a `NavigateScreen` command to jump to a full-page screen (e.g. a maintenance log
or a detailed settings page). To get back afterwards, every Runtime Viewer session tracks a
lightweight navigation history (`ScreenHistoryService`, scoped per circuit) of the routes visited,
including `/ar` itself.

- A new `GoBackScreen` command action is available on any symbol/button (bind it via the
  **Commands** editor in ServerEditorWeb, under the "Navigation" group). Firing it returns to
  whatever screen — or `/ar` — preceded the current one.
- The main Runtime Viewer topbar also shows a **← Back** button automatically whenever there is
  history to go back to, so operators don't need a screen author to have added one.
- If `GoBackScreen` is fired from a button on a screen that is itself rendered as an AR overlay
  (rather than a full page), it simply dismisses that overlay instead of navigating — overlays
  never change the browser route, so "going back" means closing the panel.
- History has no fixed relationship to popups/modals (`OpenScreenPopup`/`OpenScreenModal`) since
  those don't change the route; only full-page `NavigateScreen` transitions and entering `/ar`
  are recorded.

## Editing AR Object Mappings (ServerEditorWeb)

The **AR Object Mapping** panel (View ▸ Panels ▸ AR Object Mapping in ServerEditorWeb) provides a
CRUD editor for `NodeModel.ArObjectMappings`, following the same list+detail pattern as the
Automation Rules editor:

- Left sidebar lists all mappings; ➕ adds one, 🗑 deletes one.
- Detail pane edits `Enabled`, `MinConfidence`, `ObjectClass` (with autocomplete against any
  custom-trained classes below), the target `ScreenName` and `AliasMapName` (populated from the
  project's existing screens/alias maps), the default `ParameterFilePath`, and a table of
  per-instance parameter file overrides (`InstanceParameterFiles`) keyed by the marker/instance
  key the OR server decodes.
- Changes mark the project dirty (`Editor.HasUnsavedChanges`) and save with the rest of the
  project (`Ctrl+S` / auto-save), same as every other panel.

## Custom object detection training

Clicking **🧠 Custom Model Training** in the sidebar footer switches the detail pane to a training
form for teaching the local OR server to recognize plant-specific objects the stock YOLO model
doesn't know:

1. Point it at a dataset folder in standard Ultralytics YOLO layout
   (`images/train`, `images/val`, `labels/train`, `labels/val`, `classes.txt`).
2. Optionally record a new class name (for reference/autocomplete in the mapping editor above).
3. **Start Training** launches `yolo detect train ...` as an external process via
   `ArTrainingService` — training itself is delegated entirely to Python + Ultralytics (must be
   installed separately: `pip install ultralytics`), since reimplementing detector training in
   C# would be reinventing a well-solved wheel. The service only manages the process lifecycle,
   streams stdout into a live log, and parses `epoch/total` lines to drive a progress bar.
4. On success, the best weights are exported to ONNX and copied next to the `ArObjectServer`
   executable as `custom-<name>.onnx`. Point `ArObjectServer.exe` at that file (as its model-path
   argument) to start recognizing the new class, then add/point an `ArObjectMapping` at it.
5. **Cancel** kills the training process tree immediately if a run needs to be aborted.

This keeps training decoupled from the always-on OR server process — a training run never
affects live AR detection latency, and the newly exported model is swapped in only when the
operator explicitly restarts `ArObjectServer` with the new model file.
