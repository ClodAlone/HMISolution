"""Create the CameraShowcase sample project demonstrating IP camera widgets."""
import json, os

BASE = r"C:\Work\samples\CameraShowcase"
os.makedirs(os.path.join(BASE, "screens"), exist_ok=True)
os.makedirs(os.path.join(BASE, "scripts"), exist_ok=True)
os.makedirs(os.path.join(BASE, "Logs"), exist_ok=True)
os.makedirs(os.path.join(BASE, "crash_reports"), exist_ok=True)

# ════════════════════════════════════════════════════════════
# 1. nodes.json — project definition
# ════════════════════════════════════════════════════════════
nodes = {
    "Database": {
        "Provider": "TimescaleDb",
        "ConnectionString": "Host=localhost;Port=5432;Database=camera_hda;Username=postgres;Password=secret",
        "TableName": "variable_data"
    },
    "Users": [
        {
            "Username": "admin",
            "Password": "",
            "PasswordHash": "",
            "Group": "Administrators",
            "AutoLogOffSeconds": 0,
            "PasswordExpiryDays": 0,
            "MustChangePasswordOnFirstLogin": False,
            "PasswordChangedDate": None,
            "HasLoggedInBefore": False
        },
        {
            "Username": "operator",
            "Password": "",
            "PasswordHash": "",
            "Group": "Operators",
            "AutoLogOffSeconds": 0,
            "PasswordExpiryDays": 0,
            "MustChangePasswordOnFirstLogin": False,
            "PasswordChangedDate": None,
            "HasLoggedInBefore": False
        }
    ],
    "UserGroups": [
        {
            "Name": "Administrators",
            "AccessLevel": "ReadWrite",
            "CanAccessEditor": True,
            "CanAccessRuntime": True
        },
        {
            "Name": "Operators",
            "AccessLevel": "ReadWrite",
            "CanAccessEditor": False,
            "CanAccessRuntime": True
        }
    ],
    "Scripts": [],
    "PlcPrograms": [],
    "Screens": [],
    "Recipes": [],
    "Folder": {
        "Name": "CameraDemo",
        "Folders": [
            {
                "Name": "Cameras",
                "Folders": [
                    {
                        "Name": "Entrance",
                        "Folders": [],
                        "Variables": [
                            {
                                "Name": "YoloEnabled",
                                "Type": "Boolean",
                                "Access": "ReadWrite",
                                "Value": True,
                                "InitialValue": "true",
                                "Retentive": True
                            },
                            {
                                "Name": "MotionDetected",
                                "Type": "Boolean",
                                "Access": "Read",
                                "Value": False,
                                "InitialValue": ""
                            }
                        ]
                    },
                    {
                        "Name": "Warehouse",
                        "Folders": [],
                        "Variables": [
                            {
                                "Name": "YoloEnabled",
                                "Type": "Boolean",
                                "Access": "ReadWrite",
                                "Value": True,
                                "InitialValue": "true",
                                "Retentive": True
                            }
                        ]
                    },
                    {
                        "Name": "ParkingLot",
                        "Folders": [],
                        "Variables": [
                            {
                                "Name": "YoloEnabled",
                                "Type": "Boolean",
                                "Access": "ReadWrite",
                                "Value": False,
                                "InitialValue": "false",
                                "Retentive": True
                            }
                        ]
                    }
                ],
                "Variables": []
            },
            {
                "Name": "Detection",
                "Folders": [
                    {
                        "Name": "Entrance",
                        "Folders": [],
                        "Variables": [
                            {
                                "Name": "Label",
                                "Type": "String",
                                "Access": "Read",
                                "Value": "",
                                "InitialValue": ""
                            },
                            {
                                "Name": "Confidence",
                                "Type": "Double",
                                "Access": "Read",
                                "Value": 0.0,
                                "InitialValue": ""
                            },
                            {
                                "Name": "Count",
                                "Type": "Int32",
                                "Access": "Read",
                                "Value": 0,
                                "InitialValue": ""
                            },
                            {
                                "Name": "Alive",
                                "Type": "Boolean",
                                "Access": "Read",
                                "Value": False,
                                "InitialValue": ""
                            }
                        ]
                    },
                    {
                        "Name": "Warehouse",
                        "Folders": [],
                        "Variables": [
                            {
                                "Name": "Label",
                                "Type": "String",
                                "Access": "Read",
                                "Value": "",
                                "InitialValue": ""
                            },
                            {
                                "Name": "Confidence",
                                "Type": "Double",
                                "Access": "Read",
                                "Value": 0.0,
                                "InitialValue": ""
                            },
                            {
                                "Name": "Count",
                                "Type": "Int32",
                                "Access": "Read",
                                "Value": 0,
                                "InitialValue": ""
                            },
                            {
                                "Name": "Alive",
                                "Type": "Boolean",
                                "Access": "Read",
                                "Value": False,
                                "InitialValue": ""
                            }
                        ]
                    },
                    {
                        "Name": "ParkingLot",
                        "Folders": [],
                        "Variables": [
                            {
                                "Name": "Label",
                                "Type": "String",
                                "Access": "Read",
                                "Value": "",
                                "InitialValue": ""
                            },
                            {
                                "Name": "Confidence",
                                "Type": "Double",
                                "Access": "Read",
                                "Value": 0.0,
                                "InitialValue": ""
                            },
                            {
                                "Name": "Count",
                                "Type": "Int32",
                                "Access": "Read",
                                "Value": 0,
                                "InitialValue": ""
                            },
                            {
                                "Name": "Alive",
                                "Type": "Boolean",
                                "Access": "Read",
                                "Value": False,
                                "InitialValue": ""
                            }
                        ]
                    }
                ],
                "Variables": []
            },
            {
                "Name": "Stats",
                "Folders": [],
                "Variables": [
                    {
                        "Name": "TotalPeopleDetected",
                        "Type": "Int32",
                        "Access": "ReadWrite",
                        "Value": 0,
                        "InitialValue": "0",
                        "Retentive": True,
                        "DataLogging": {
                            "Enabled": True,
                            "Hysteresis": 1,
                            "MaxAge": "00:05:00"
                        }
                    },
                    {
                        "Name": "TotalVehiclesDetected",
                        "Type": "Int32",
                        "Access": "ReadWrite",
                        "Value": 0,
                        "InitialValue": "0",
                        "Retentive": True,
                        "DataLogging": {
                            "Enabled": True,
                            "Hysteresis": 1,
                            "MaxAge": "00:05:00"
                        }
                    },
                    {
                        "Name": "ActiveCameras",
                        "Type": "Int32",
                        "Access": "Read",
                        "Value": 0,
                        "InitialValue": "0"
                    }
                ]
            }
        ],
        "Variables": []
    },
    "Strings": [
        {"Key": "app_title", "Translations": {"en": "Camera Showcase", "de": "Kamera-Demo", "it": "Demo Telecamere"}},
        {"Key": "overview", "Translations": {"en": "Camera Overview", "de": "Kameraübersicht", "it": "Panoramica Telecamere"}},
        {"Key": "entrance", "Translations": {"en": "Entrance", "de": "Eingang", "it": "Ingresso"}},
        {"Key": "warehouse", "Translations": {"en": "Warehouse", "de": "Lager", "it": "Magazzino"}},
        {"Key": "parking", "Translations": {"en": "Parking Lot", "de": "Parkplatz", "it": "Parcheggio"}},
        {"Key": "detection", "Translations": {"en": "Detection Results", "de": "Erkennungsergebnisse", "it": "Risultati Rilevamento"}},
        {"Key": "yolo_enabled", "Translations": {"en": "YOLO Enabled", "de": "YOLO Aktiviert", "it": "YOLO Attivato"}},
        {"Key": "no_detection", "Translations": {"en": "No detection", "de": "Keine Erkennung", "it": "Nessun rilevamento"}},
        {"Key": "single_camera", "Translations": {"en": "Single Camera", "de": "Einzelkamera", "it": "Singola Telecamera"}},
        {"Key": "yolo_settings", "Translations": {"en": "YOLO Settings", "de": "YOLO-Einstellungen", "it": "Impostazioni YOLO"}}
    ],
    "Images": [],
    "Cameras": [],
    "Schedulers": [],
    "Reports": [],
    "CalculatedVariables": [],
    "ProjectPasswordHash": "",
    "Server": {
        "EndpointUrl": "opc.tcp://localhost:14860/CameraShowcase",
        "EnableAnonymous": True,
        "EnableEditorLogin": False,
        "EnableRuntimeLogin": False,
        "RequireStrongPassword": False,
        "StartupScreen": "CameraOverview",
        "ShowNavigationBar": True,
        "NavigationStyle": "hamburger",
        "EnableTagBrowser": True,
        "EventLog": {
            "Enabled": True,
            "DbPath": "events.db",
            "MaxAgeDays": 30
        },
        "DiagnosticsPort": 14861
    }
}

with open(os.path.join(BASE, "nodes.json"), "w", encoding="utf-8") as f:
    json.dump(nodes, f, indent=2, ensure_ascii=False)
print("1. Created nodes.json")


# ════════════════════════════════════════════════════════════
# Helper: default symbol props (matching WidgetShowcase format)
# ════════════════════════════════════════════════════════════
def default_sym():
    """Return a dict with all the default symbol properties."""
    return {
        "FontFamily": "",
        "FontSize": 0,
        "FontWeight": "",
        "FontStyle": "",
        "SvgContent": None,
        "GroupId": None,
        "Animations": [],
        "VariablePath": None,
        "HdaVariablePaths": [],
        "HdaTimeRangeMinutes": 60,
        "HdaMaxPoints": 500,
        "EventLogMaxRows": 200,
        "EventLogTimeRangeMinutes": 1440,
        "EventLogCategories": "",
        "EditBoxStep": 1,
        "EditBoxFormat": "",
        "EditBoxShowStatistics": False,
        "FillBinding": None,
        "VisibilityBinding": None,
        "RotationBinding": None,
        "LabelBinding": None,
        "MinValue": None,
        "MaxValue": None,
        "Commands": [],
        "RequiredAccess": "",
        "Camera": None,
        "RecipeName": "",
        "SchedulerName": "",
        "EmbeddedScreens": [],
    }

def sym(id, type, x, y, w, h, fill="#1e293b", stroke="#333333", label="", **kw):
    """Create a symbol dict with defaults + overrides."""
    d = {
        "Id": id,
        "Type": type,
        "X": x,
        "Y": y,
        "Width": w,
        "Height": h,
        "Fill": fill,
        "Stroke": stroke,
        "StrokeWidth": 1,
        "Label": label,
        "Rotation": 0,
        **default_sym()
    }
    d.update(kw)
    return d

def cam_config(cam_id, url, protocol="mjpeg", fps=5, enable_yolo=True,
               yolo_enable_var="", det_prefix="", confidence=0.5,
               draw=True, timeout=10, username="", password=""):
    """Create a CameraConfig dict."""
    return {
        "CameraId": cam_id,
        "Url": url,
        "Protocol": protocol,
        "Fps": fps,
        "EnableYolo": enable_yolo,
        "YoloEnableVariable": yolo_enable_var,
        "YoloModelPath": "",
        "YoloConfidence": confidence,
        "DetectionVariablePrefix": det_prefix,
        "DrawDetections": draw,
        "UseCuda": False,
        "CudaDeviceId": 0,
        "DetectionTimeoutSeconds": timeout,
        "Username": username,
        "Password": password
    }


# ════════════════════════════════════════════════════════════
# 2. Screen: CameraOverview — 3 cameras in a grid with status
# ════════════════════════════════════════════════════════════
overview_screen = {
    "Name": "CameraOverview",
    "Width": 1920,
    "Height": 1080,
    "Background": "#0f172a",
    "BackgroundImageId": "",
    "LayoutMode": "svg",
    "GridColumns": 12,
    "GridGap": 8,
    "Group": "",
    "ShowInNavigation": True,
    "ShowGrid": False,
    "EditorGridSize": 20,
    "SnapToGrid": False,
    "SmartSnap": False,
    "Symbols": [
        # Title
        sym("title", "text", 40, 20, 700, 45, fill="#e2e8f0",
            Label="📷 Camera Showcase — Multi-Camera Monitoring",
            FontSize=22, FontWeight="bold"),

        # ── Camera 1: Entrance (MJPEG, YOLO on, dynamic variable) ──
        sym("lbl_entrance", "text", 40, 80, 280, 30, fill="#94a3b8",
            Label="🚪 Entrance Camera", FontSize=14, FontWeight="bold"),
        sym("cam_entrance", "ipcamera", 40, 115, 560, 380, fill="#000000", stroke="#334155",
            Camera=cam_config(
                cam_id="entrance",
                url="http://192.168.1.100/mjpeg",
                protocol="mjpeg",
                fps=10,
                enable_yolo=True,
                yolo_enable_var="CameraDemo.Cameras.Entrance.YoloEnabled",
                det_prefix="CameraDemo.Detection.Entrance",
                confidence=0.5,
                draw=True
            )),
        # Entrance detection status
        sym("entrance_det_label", "text", 40, 505, 150, 22, fill="#94a3b8",
            Label="Detected:", FontSize=11),
        sym("entrance_det_value", "numericdisplay", 195, 500, 200, 30,
            VariablePath="CameraDemo.Detection.Entrance.Label",
            NumericDisplayColor="#22c55e", NumericDisplayBackground="#1e293b"),
        sym("entrance_count", "numericdisplay", 410, 500, 100, 30,
            VariablePath="CameraDemo.Detection.Entrance.Count",
            Label="Count", NumericDisplayColor="#60a5fa", NumericDisplayBackground="#1e293b"),
        sym("entrance_alive", "indicator", 520, 505, 20, 20,
            VariablePath="CameraDemo.Detection.Entrance.Alive",
            Fill="#22c55e"),
        # YOLO toggle switch
        sym("lbl_entrance_yolo", "text", 40, 540, 120, 22, fill="#94a3b8",
            Label="YOLO Detection:", FontSize=11),
        sym("entrance_yolo_sw", "switch", 175, 535, 60, 30,
            VariablePath="CameraDemo.Cameras.Entrance.YoloEnabled",
            SwitchOnColor="#22c55e", SwitchOffColor="#64748b", SwitchStyle="apple"),

        # ── Camera 2: Warehouse (RTSP via FFmpeg, YOLO on) ──
        sym("lbl_warehouse", "text", 680, 80, 280, 30, fill="#94a3b8",
            Label="🏭 Warehouse Camera", FontSize=14, FontWeight="bold"),
        sym("cam_warehouse", "ipcamera", 680, 115, 560, 380, fill="#000000", stroke="#334155",
            Camera=cam_config(
                cam_id="warehouse",
                url="rtsp://192.168.1.101:554/stream1",
                protocol="rtsp",
                fps=15,
                enable_yolo=True,
                yolo_enable_var="CameraDemo.Cameras.Warehouse.YoloEnabled",
                det_prefix="CameraDemo.Detection.Warehouse",
                confidence=0.4,
                draw=True,
                username="admin",
                password="camera123"
            )),
        sym("warehouse_det_label", "text", 680, 505, 150, 22, fill="#94a3b8",
            Label="Detected:", FontSize=11),
        sym("warehouse_det_value", "numericdisplay", 835, 500, 200, 30,
            VariablePath="CameraDemo.Detection.Warehouse.Label",
            NumericDisplayColor="#22c55e", NumericDisplayBackground="#1e293b"),
        sym("warehouse_count", "numericdisplay", 1050, 500, 100, 30,
            VariablePath="CameraDemo.Detection.Warehouse.Count",
            Label="Count", NumericDisplayColor="#60a5fa", NumericDisplayBackground="#1e293b"),
        sym("warehouse_alive", "indicator", 1160, 505, 20, 20,
            VariablePath="CameraDemo.Detection.Warehouse.Alive",
            Fill="#22c55e"),
        sym("lbl_warehouse_yolo", "text", 680, 540, 120, 22, fill="#94a3b8",
            Label="YOLO Detection:", FontSize=11),
        sym("warehouse_yolo_sw", "switch", 815, 535, 60, 30,
            VariablePath="CameraDemo.Cameras.Warehouse.YoloEnabled",
            SwitchOnColor="#22c55e", SwitchOffColor="#64748b", SwitchStyle="apple"),

        # ── Camera 3: Parking Lot (HTTP snapshot, YOLO off by default) ──
        sym("lbl_parking", "text", 1320, 80, 280, 30, fill="#94a3b8",
            Label="🅿️ Parking Lot Camera", FontSize=14, FontWeight="bold"),
        sym("cam_parking", "ipcamera", 1320, 115, 560, 380, fill="#000000", stroke="#334155",
            Camera=cam_config(
                cam_id="parking",
                url="http://192.168.1.102/snapshot.jpg",
                protocol="http",
                fps=2,
                enable_yolo=False,
                yolo_enable_var="CameraDemo.Cameras.ParkingLot.YoloEnabled",
                det_prefix="CameraDemo.Detection.ParkingLot",
                confidence=0.6,
                draw=True
            )),
        sym("parking_det_label", "text", 1320, 505, 150, 22, fill="#94a3b8",
            Label="Detected:", FontSize=11),
        sym("parking_det_value", "numericdisplay", 1475, 500, 200, 30,
            VariablePath="CameraDemo.Detection.ParkingLot.Label",
            NumericDisplayColor="#22c55e", NumericDisplayBackground="#1e293b"),
        sym("parking_count", "numericdisplay", 1690, 500, 100, 30,
            VariablePath="CameraDemo.Detection.ParkingLot.Count",
            Label="Count", NumericDisplayColor="#60a5fa", NumericDisplayBackground="#1e293b"),
        sym("parking_alive", "indicator", 1800, 505, 20, 20,
            VariablePath="CameraDemo.Detection.ParkingLot.Alive",
            Fill="#22c55e"),
        sym("lbl_parking_yolo", "text", 1320, 540, 120, 22, fill="#94a3b8",
            Label="YOLO Detection:", FontSize=11),
        sym("parking_yolo_sw", "switch", 1455, 535, 60, 30,
            VariablePath="CameraDemo.Cameras.ParkingLot.YoloEnabled",
            SwitchOnColor="#22c55e", SwitchOffColor="#64748b", SwitchStyle="apple"),

        # ── Bottom: Statistics section ──
        sym("stats_title", "text", 40, 600, 400, 35, fill="#e2e8f0",
            Label="📊 Detection Statistics", FontSize=16, FontWeight="bold"),

        # Stats gauges
        sym("lbl_people", "text", 40, 650, 150, 22, fill="#94a3b8",
            Label="People Detected:", FontSize=12),
        sym("gauge_people", "numericdisplay", 200, 645, 120, 35,
            VariablePath="CameraDemo.Stats.TotalPeopleDetected",
            NumericDisplayDecimals=0, NumericDisplayColor="#f472b6",
            NumericDisplayBackground="#1e293b"),

        sym("lbl_vehicles", "text", 360, 650, 160, 22, fill="#94a3b8",
            Label="Vehicles Detected:", FontSize=12),
        sym("gauge_vehicles", "numericdisplay", 530, 645, 120, 35,
            VariablePath="CameraDemo.Stats.TotalVehiclesDetected",
            NumericDisplayDecimals=0, NumericDisplayColor="#60a5fa",
            NumericDisplayBackground="#1e293b"),

        sym("lbl_active_cams", "text", 700, 650, 150, 22, fill="#94a3b8",
            Label="Active Cameras:", FontSize=12),
        sym("gauge_active", "numericdisplay", 860, 645, 80, 35,
            VariablePath="CameraDemo.Stats.ActiveCameras",
            NumericDisplayDecimals=0, NumericDisplayColor="#22c55e",
            NumericDisplayBackground="#1e293b"),

        # ── Bottom: Event log for detection events ──
        sym("eventlog_title", "text", 40, 710, 300, 30, fill="#e2e8f0",
            Label="🔔 Detection Events", FontSize=14, FontWeight="bold"),
        sym("eventlog", "eventlog", 40, 750, 900, 300,
            Fill="#0f172a", Stroke="#334155",
            EventLogMaxRows=50, EventLogTimeRangeMinutes=60,
            EventLogCategories="Detection"),

        # ── Bottom right: confidence bars per camera ──
        sym("conf_title", "text", 1000, 710, 300, 30, fill="#e2e8f0",
            Label="📈 Confidence Levels", FontSize=14, FontWeight="bold"),

        sym("lbl_conf_entrance", "text", 1000, 755, 120, 22, fill="#94a3b8",
            Label="Entrance:", FontSize=11),
        sym("bar_conf_entrance", "progressbar", 1130, 750, 250, 28,
            VariablePath="CameraDemo.Detection.Entrance.Confidence",
            ProgressBarMin=0, ProgressBarMax=1,
            ProgressBarFillColor="#22c55e", ProgressBarTrackColor="#334155",
            ProgressBarShowText=True, ProgressBarBorderRadius=4),

        sym("lbl_conf_warehouse", "text", 1000, 800, 120, 22, fill="#94a3b8",
            Label="Warehouse:", FontSize=11),
        sym("bar_conf_warehouse", "progressbar", 1130, 795, 250, 28,
            VariablePath="CameraDemo.Detection.Warehouse.Confidence",
            ProgressBarMin=0, ProgressBarMax=1,
            ProgressBarFillColor="#3b82f6", ProgressBarTrackColor="#334155",
            ProgressBarShowText=True, ProgressBarBorderRadius=4),

        sym("lbl_conf_parking", "text", 1000, 845, 120, 22, fill="#94a3b8",
            Label="Parking:", FontSize=11),
        sym("bar_conf_parking", "progressbar", 1130, 840, 250, 28,
            VariablePath="CameraDemo.Detection.ParkingLot.Confidence",
            ProgressBarMin=0, ProgressBarMax=1,
            ProgressBarFillColor="#f97316", ProgressBarTrackColor="#334155",
            ProgressBarShowText=True, ProgressBarBorderRadius=4),

        # Navigation buttons
        sym("nav_single", "navbutton", 1500, 750, 200, 45,
            Label="Single Camera View",
            NavButtonTarget="SingleCamera", NavButtonStyle="filled",
            NavButtonIcon="🔍", NavButtonColor="#3b82f6", NavButtonTextColor="#ffffff"),
        sym("nav_yolo", "navbutton", 1500, 810, 200, 45,
            Label="YOLO Settings",
            NavButtonTarget="YoloSettings", NavButtonStyle="filled",
            NavButtonIcon="🤖", NavButtonColor="#facc15", NavButtonTextColor="#0f172a"),
    ]
}

with open(os.path.join(BASE, "screens", "CameraOverview.json"), "w", encoding="utf-8") as f:
    json.dump(overview_screen, f, indent=2, ensure_ascii=False)
print("2. Created screens/CameraOverview.json")


# ════════════════════════════════════════════════════════════
# 3. Screen: SingleCamera — full-screen single camera with details
# ════════════════════════════════════════════════════════════
single_screen = {
    "Name": "SingleCamera",
    "Width": 1920,
    "Height": 1080,
    "Background": "#0f172a",
    "BackgroundImageId": "",
    "LayoutMode": "svg",
    "GridColumns": 12,
    "GridGap": 8,
    "Group": "Detail",
    "ShowInNavigation": True,
    "ShowGrid": False,
    "EditorGridSize": 20,
    "SnapToGrid": False,
    "SmartSnap": False,
    "Symbols": [
        # Title
        sym("title", "text", 40, 20, 500, 45, fill="#e2e8f0",
            Label="🔍 Entrance Camera — Full View", FontSize=20, FontWeight="bold"),

        # Full-size entrance camera
        sym("cam_full", "ipcamera", 40, 80, 1200, 800, fill="#000000", stroke="#475569",
            Camera=cam_config(
                cam_id="entrance",
                url="http://192.168.1.100/mjpeg",
                protocol="mjpeg",
                fps=15,
                enable_yolo=True,
                yolo_enable_var="CameraDemo.Cameras.Entrance.YoloEnabled",
                det_prefix="CameraDemo.Detection.Entrance",
                confidence=0.5,
                draw=True
            )),

        # Right sidebar — Detection info panel
        sym("info_bg", "rect", 1280, 80, 600, 800, fill="#1e293b", stroke="#334155"),
        sym("info_title", "text", 1300, 100, 280, 30, fill="#e2e8f0",
            Label="📋 Detection Information", FontSize=16, FontWeight="bold"),

        # Current detection
        sym("lbl_current", "text", 1300, 150, 200, 22, fill="#94a3b8",
            Label="Current Detection:", FontSize=12),
        sym("val_label", "numericdisplay", 1300, 180, 250, 40,
            VariablePath="CameraDemo.Detection.Entrance.Label",
            NumericDisplayColor="#22c55e", NumericDisplayBackground="#0f172a",
            FontSize=18),

        sym("lbl_conf", "text", 1300, 240, 200, 22, fill="#94a3b8",
            Label="Confidence:", FontSize=12),
        sym("gauge_conf", "gauge", 1300, 270, 250, 200,
            VariablePath="CameraDemo.Detection.Entrance.Confidence",
            MinValue=0, MaxValue=1,
            GaugeStyle="semicircle", GaugeTicks=5, GaugeShowValue=True,
            GaugeUnit="", GaugeValueFormat="P0",
            GaugeTrackColor="#334155", GaugeNeedleColor="#22c55e",
            Fill="#22c55e"),

        sym("lbl_objects", "text", 1300, 490, 200, 22, fill="#94a3b8",
            Label="Objects in Frame:", FontSize=12),
        sym("val_count", "numericdisplay", 1300, 520, 120, 45,
            VariablePath="CameraDemo.Detection.Entrance.Count",
            NumericDisplayDecimals=0, NumericDisplayColor="#60a5fa",
            NumericDisplayBackground="#0f172a", FontSize=24),

        # Alive indicator
        sym("lbl_alive", "text", 1300, 590, 200, 22, fill="#94a3b8",
            Label="Detection Alive:", FontSize=12),
        sym("led_alive", "indicator", 1300, 620, 40, 40,
            VariablePath="CameraDemo.Detection.Entrance.Alive",
            Fill="#22c55e"),
        sym("lbl_alive_status", "text", 1350, 627, 150, 22, fill="#94a3b8",
            VariablePath="CameraDemo.Detection.Entrance.Alive",
            LabelBinding="value == true ? 'Active' : 'Inactive'",
            FontSize=12),

        # YOLO toggle
        sym("lbl_yolo_ctrl", "text", 1300, 690, 200, 22, fill="#facc15",
            Label="🤖 YOLO Control:", FontSize=13, FontWeight="bold"),
        sym("yolo_switch", "switch", 1300, 720, 80, 35,
            VariablePath="CameraDemo.Cameras.Entrance.YoloEnabled",
            SwitchOnColor="#22c55e", SwitchOffColor="#64748b", SwitchStyle="apple"),
        sym("lbl_yolo_status", "text", 1400, 725, 160, 22, fill="#94a3b8",
            VariablePath="CameraDemo.Cameras.Entrance.YoloEnabled",
            LabelBinding="value == true ? 'Enabled' : 'Disabled'",
            FontSize=12),

        # Back button
        sym("nav_back", "navbutton", 1300, 810, 200, 40,
            Label="← Back to Overview",
            NavButtonTarget="CameraOverview", NavButtonStyle="outline",
            NavButtonIcon="", NavButtonColor="#64748b", NavButtonTextColor="#e2e8f0"),

        # Bottom: sparkline showing detection count over time
        sym("sparkline_title", "text", 40, 900, 300, 25, fill="#94a3b8",
            Label="Detection Count (realtime)", FontSize=12),
        sym("sparkline_count", "sparkline", 40, 930, 600, 120,
            VariablePath="CameraDemo.Detection.Entrance.Count",
            SparklineColor="#22c55e", SparklineMaxPoints=60,
            SparklineShowValue=True, SparklineFillArea=True,
            SparklineStrokeWidth=2, Fill="#0f172a", Stroke="#334155"),
    ]
}

with open(os.path.join(BASE, "screens", "SingleCamera.json"), "w", encoding="utf-8") as f:
    json.dump(single_screen, f, indent=2, ensure_ascii=False)
print("3. Created screens/SingleCamera.json")


# ════════════════════════════════════════════════════════════
# 4. Screen: YoloSettings — configuration & control panel
# ════════════════════════════════════════════════════════════
yolo_screen = {
    "Name": "YoloSettings",
    "Width": 1920,
    "Height": 1080,
    "Background": "#0f172a",
    "BackgroundImageId": "",
    "LayoutMode": "svg",
    "GridColumns": 12,
    "GridGap": 8,
    "Group": "Settings",
    "ShowInNavigation": True,
    "ShowGrid": False,
    "EditorGridSize": 20,
    "SnapToGrid": False,
    "SmartSnap": False,
    "Symbols": [
        # Title
        sym("title", "text", 40, 20, 600, 45, fill="#e2e8f0",
            Label="🤖 YOLO Detection Settings", FontSize=22, FontWeight="bold"),
        sym("subtitle", "text", 40, 65, 700, 25, fill="#64748b",
            Label="Toggle object detection per camera. Uses server variables for runtime control.",
            FontSize=12),

        # ── Entrance camera control ──
        sym("entrance_bg", "rect", 40, 120, 580, 400, fill="#1e293b", stroke="#334155"),
        sym("entrance_title", "text", 60, 140, 300, 30, fill="#e2e8f0",
            Label="🚪 Entrance Camera", FontSize=16, FontWeight="bold"),

        sym("entrance_thumb", "ipcamera", 60, 185, 260, 180, fill="#000000", stroke="#475569",
            Camera=cam_config(
                cam_id="entrance",
                url="http://192.168.1.100/mjpeg",
                protocol="mjpeg",
                fps=5,
                enable_yolo=True,
                yolo_enable_var="CameraDemo.Cameras.Entrance.YoloEnabled",
                det_prefix="CameraDemo.Detection.Entrance",
                confidence=0.5,
                draw=True
            )),

        sym("lbl_e_yolo", "text", 340, 190, 200, 22, fill="#facc15",
            Label="YOLO Detection:", FontSize=13, FontWeight="bold"),
        sym("sw_e_yolo", "switch", 340, 220, 80, 35,
            VariablePath="CameraDemo.Cameras.Entrance.YoloEnabled",
            SwitchOnColor="#22c55e", SwitchOffColor="#64748b", SwitchStyle="apple"),
        sym("lbl_e_status", "text", 430, 225, 120, 22, fill="#94a3b8",
            VariablePath="CameraDemo.Cameras.Entrance.YoloEnabled",
            LabelBinding="value == true ? 'ON' : 'OFF'", FontSize=12),

        sym("lbl_e_proto", "text", 340, 275, 200, 22, fill="#94a3b8",
            Label="Protocol: MJPEG", FontSize=11),
        sym("lbl_e_fps", "text", 340, 300, 200, 22, fill="#94a3b8",
            Label="FPS: 10", FontSize=11),
        sym("lbl_e_conf", "text", 340, 325, 200, 22, fill="#94a3b8",
            Label="Confidence: 0.50", FontSize=11),
        sym("lbl_e_var", "text", 340, 350, 260, 22, fill="#64748b",
            Label="Var: CameraDemo.Cameras.Entrance.YoloEnabled", FontSize=9),
        sym("lbl_e_det", "text", 340, 375, 260, 22, fill="#64748b",
            Label="Detection Prefix: CameraDemo.Detection.Entrance", FontSize=9),

        sym("e_det_label", "text", 60, 390, 100, 22, fill="#94a3b8",
            Label="Last detection:", FontSize=10),
        sym("e_det_val", "numericdisplay", 170, 385, 150, 28,
            VariablePath="CameraDemo.Detection.Entrance.Label",
            NumericDisplayColor="#22c55e", NumericDisplayBackground="#0f172a"),
        sym("e_alive_led", "indicator", 60, 425, 16, 16,
            VariablePath="CameraDemo.Detection.Entrance.Alive", Fill="#22c55e"),
        sym("e_alive_text", "text", 85, 425, 100, 18, fill="#94a3b8",
            VariablePath="CameraDemo.Detection.Entrance.Alive",
            LabelBinding="value == true ? 'Alive' : 'Dead'", FontSize=10),
        sym("e_count_lbl", "text", 170, 425, 60, 18, fill="#94a3b8",
            Label="Count:", FontSize=10),
        sym("e_count_val", "numericdisplay", 230, 420, 60, 28,
            VariablePath="CameraDemo.Detection.Entrance.Count",
            NumericDisplayDecimals=0, NumericDisplayColor="#60a5fa",
            NumericDisplayBackground="#0f172a"),
        sym("e_conf_bar", "progressbar", 60, 460, 260, 20,
            VariablePath="CameraDemo.Detection.Entrance.Confidence",
            ProgressBarMin=0, ProgressBarMax=1,
            ProgressBarFillColor="#22c55e", ProgressBarTrackColor="#334155",
            ProgressBarShowText=True, ProgressBarBorderRadius=3),

        # ── Warehouse camera control ──
        sym("warehouse_bg", "rect", 660, 120, 580, 400, fill="#1e293b", stroke="#334155"),
        sym("warehouse_title", "text", 680, 140, 300, 30, fill="#e2e8f0",
            Label="🏭 Warehouse Camera", FontSize=16, FontWeight="bold"),

        sym("warehouse_thumb", "ipcamera", 680, 185, 260, 180, fill="#000000", stroke="#475569",
            Camera=cam_config(
                cam_id="warehouse",
                url="rtsp://192.168.1.101:554/stream1",
                protocol="rtsp",
                fps=5,
                enable_yolo=True,
                yolo_enable_var="CameraDemo.Cameras.Warehouse.YoloEnabled",
                det_prefix="CameraDemo.Detection.Warehouse",
                confidence=0.4,
                draw=True,
                username="admin",
                password="camera123"
            )),

        sym("lbl_w_yolo", "text", 960, 190, 200, 22, fill="#facc15",
            Label="YOLO Detection:", FontSize=13, FontWeight="bold"),
        sym("sw_w_yolo", "switch", 960, 220, 80, 35,
            VariablePath="CameraDemo.Cameras.Warehouse.YoloEnabled",
            SwitchOnColor="#22c55e", SwitchOffColor="#64748b", SwitchStyle="apple"),
        sym("lbl_w_status", "text", 1050, 225, 120, 22, fill="#94a3b8",
            VariablePath="CameraDemo.Cameras.Warehouse.YoloEnabled",
            LabelBinding="value == true ? 'ON' : 'OFF'", FontSize=12),

        sym("lbl_w_proto", "text", 960, 275, 200, 22, fill="#94a3b8",
            Label="Protocol: RTSP", FontSize=11),
        sym("lbl_w_fps", "text", 960, 300, 200, 22, fill="#94a3b8",
            Label="FPS: 15", FontSize=11),
        sym("lbl_w_conf", "text", 960, 325, 200, 22, fill="#94a3b8",
            Label="Confidence: 0.40", FontSize=11),
        sym("lbl_w_var", "text", 960, 350, 280, 22, fill="#64748b",
            Label="Var: CameraDemo.Cameras.Warehouse.YoloEnabled", FontSize=9),
        sym("lbl_w_det", "text", 960, 375, 280, 22, fill="#64748b",
            Label="Detection Prefix: CameraDemo.Detection.Warehouse", FontSize=9),

        sym("w_det_label", "text", 680, 390, 100, 22, fill="#94a3b8",
            Label="Last detection:", FontSize=10),
        sym("w_det_val", "numericdisplay", 790, 385, 150, 28,
            VariablePath="CameraDemo.Detection.Warehouse.Label",
            NumericDisplayColor="#22c55e", NumericDisplayBackground="#0f172a"),
        sym("w_alive_led", "indicator", 680, 425, 16, 16,
            VariablePath="CameraDemo.Detection.Warehouse.Alive", Fill="#22c55e"),
        sym("w_alive_text", "text", 705, 425, 100, 18, fill="#94a3b8",
            VariablePath="CameraDemo.Detection.Warehouse.Alive",
            LabelBinding="value == true ? 'Alive' : 'Dead'", FontSize=10),
        sym("w_count_lbl", "text", 790, 425, 60, 18, fill="#94a3b8",
            Label="Count:", FontSize=10),
        sym("w_count_val", "numericdisplay", 850, 420, 60, 28,
            VariablePath="CameraDemo.Detection.Warehouse.Count",
            NumericDisplayDecimals=0, NumericDisplayColor="#60a5fa",
            NumericDisplayBackground="#0f172a"),
        sym("w_conf_bar", "progressbar", 680, 460, 260, 20,
            VariablePath="CameraDemo.Detection.Warehouse.Confidence",
            ProgressBarMin=0, ProgressBarMax=1,
            ProgressBarFillColor="#3b82f6", ProgressBarTrackColor="#334155",
            ProgressBarShowText=True, ProgressBarBorderRadius=3),

        # ── Parking Lot camera control ──
        sym("parking_bg", "rect", 1280, 120, 580, 400, fill="#1e293b", stroke="#334155"),
        sym("parking_title", "text", 1300, 140, 300, 30, fill="#e2e8f0",
            Label="🅿️ Parking Lot Camera", FontSize=16, FontWeight="bold"),

        sym("parking_thumb", "ipcamera", 1300, 185, 260, 180, fill="#000000", stroke="#475569",
            Camera=cam_config(
                cam_id="parking",
                url="http://192.168.1.102/snapshot.jpg",
                protocol="http",
                fps=2,
                enable_yolo=False,
                yolo_enable_var="CameraDemo.Cameras.ParkingLot.YoloEnabled",
                det_prefix="CameraDemo.Detection.ParkingLot",
                confidence=0.6,
                draw=True
            )),

        sym("lbl_p_yolo", "text", 1580, 190, 200, 22, fill="#facc15",
            Label="YOLO Detection:", FontSize=13, FontWeight="bold"),
        sym("sw_p_yolo", "switch", 1580, 220, 80, 35,
            VariablePath="CameraDemo.Cameras.ParkingLot.YoloEnabled",
            SwitchOnColor="#22c55e", SwitchOffColor="#64748b", SwitchStyle="apple"),
        sym("lbl_p_status", "text", 1670, 225, 120, 22, fill="#94a3b8",
            VariablePath="CameraDemo.Cameras.ParkingLot.YoloEnabled",
            LabelBinding="value == true ? 'ON' : 'OFF'", FontSize=12),

        sym("lbl_p_proto", "text", 1580, 275, 200, 22, fill="#94a3b8",
            Label="Protocol: HTTP Snapshot", FontSize=11),
        sym("lbl_p_fps", "text", 1580, 300, 200, 22, fill="#94a3b8",
            Label="FPS: 2", FontSize=11),
        sym("lbl_p_conf", "text", 1580, 325, 200, 22, fill="#94a3b8",
            Label="Confidence: 0.60", FontSize=11),
        sym("lbl_p_var", "text", 1580, 350, 260, 22, fill="#64748b",
            Label="Var: CameraDemo.Cameras.ParkingLot.YoloEnabled", FontSize=9),
        sym("lbl_p_det", "text", 1580, 375, 260, 22, fill="#64748b",
            Label="Detection Prefix: CameraDemo.Detection.ParkingLot", FontSize=9),

        sym("p_det_label", "text", 1300, 390, 100, 22, fill="#94a3b8",
            Label="Last detection:", FontSize=10),
        sym("p_det_val", "numericdisplay", 1410, 385, 150, 28,
            VariablePath="CameraDemo.Detection.ParkingLot.Label",
            NumericDisplayColor="#22c55e", NumericDisplayBackground="#0f172a"),
        sym("p_alive_led", "indicator", 1300, 425, 16, 16,
            VariablePath="CameraDemo.Detection.ParkingLot.Alive", Fill="#22c55e"),
        sym("p_alive_text", "text", 1325, 425, 100, 18, fill="#94a3b8",
            VariablePath="CameraDemo.Detection.ParkingLot.Alive",
            LabelBinding="value == true ? 'Alive' : 'Dead'", FontSize=10),
        sym("p_count_lbl", "text", 1410, 425, 60, 18, fill="#94a3b8",
            Label="Count:", FontSize=10),
        sym("p_count_val", "numericdisplay", 1470, 420, 60, 28,
            VariablePath="CameraDemo.Detection.ParkingLot.Count",
            NumericDisplayDecimals=0, NumericDisplayColor="#60a5fa",
            NumericDisplayBackground="#0f172a"),
        sym("p_conf_bar", "progressbar", 1300, 460, 260, 20,
            VariablePath="CameraDemo.Detection.ParkingLot.Confidence",
            ProgressBarMin=0, ProgressBarMax=1,
            ProgressBarFillColor="#f97316", ProgressBarTrackColor="#334155",
            ProgressBarShowText=True, ProgressBarBorderRadius=3),

        # ── Bottom: Help / info text ──
        sym("help_bg", "rect", 40, 560, 1820, 200, fill="#1e293b", stroke="#334155"),
        sym("help_title", "text", 60, 580, 400, 30, fill="#e2e8f0",
            Label="📖 How Camera + YOLO Works", FontSize=16, FontWeight="bold"),
        sym("help1", "text", 60, 620, 860, 22, fill="#94a3b8",
            Label="1. Each camera captures frames via MJPEG, RTSP (FFmpeg), or HTTP snapshot polling.", FontSize=11),
        sym("help2", "text", 60, 645, 860, 22, fill="#94a3b8",
            Label="2. When YOLO is enabled, each frame is processed through a YOLOv8 ONNX model for object detection.", FontSize=11),
        sym("help3", "text", 60, 670, 860, 22, fill="#94a3b8",
            Label="3. Detection results (Label, Confidence, Count, Alive) are written to OPC server variables.", FontSize=11),
        sym("help4", "text", 60, 695, 860, 22, fill="#94a3b8",
            Label="4. The YoloEnableVariable lets you toggle detection ON/OFF at runtime via a Boolean variable.", FontSize=11),
        sym("help5", "text", 60, 720, 860, 22, fill="#94a3b8",
            Label="5. Bounding boxes are drawn directly on the MJPEG stream when DrawDetections is enabled.", FontSize=11),

        sym("help_r1", "text", 960, 620, 860, 22, fill="#94a3b8",
            Label="• Entrance: MJPEG stream, YOLO enabled, dynamic enable via variable.", FontSize=11),
        sym("help_r2", "text", 960, 645, 860, 22, fill="#94a3b8",
            Label="• Warehouse: RTSP via FFmpeg, YOLO enabled, authenticated stream.", FontSize=11),
        sym("help_r3", "text", 960, 670, 860, 22, fill="#94a3b8",
            Label="• Parking: HTTP snapshot polling at 2 FPS, YOLO off by default (toggle to enable).", FontSize=11),
        sym("help_r4", "text", 960, 695, 860, 22, fill="#facc15",
            Label="💡 Toggle the switches above to enable/disable YOLO per camera at runtime!", FontSize=11, FontWeight="bold"),

        # Nav back
        sym("nav_back", "navbutton", 40, 790, 200, 40,
            Label="← Back to Overview",
            NavButtonTarget="CameraOverview", NavButtonStyle="outline",
            NavButtonIcon="", NavButtonColor="#64748b", NavButtonTextColor="#e2e8f0"),
    ]
}

with open(os.path.join(BASE, "screens", "YoloSettings.json"), "w", encoding="utf-8") as f:
    json.dump(yolo_screen, f, indent=2, ensure_ascii=False)
print("4. Created screens/YoloSettings.json")


# ════════════════════════════════════════════════════════════
# 5. Script: SimulateDetections — fake detection data for demo
# ════════════════════════════════════════════════════════════
sim_code = r"""// Simulate camera detection results for demo purposes
var rnd = new Random();

// --- Entrance camera (YOLO enabled by default) ---
if (ReadBool("CameraDemo.Cameras.Entrance.YoloEnabled"))
{
    var labels = new[] { "person", "car", "bicycle", "dog", "backpack", "truck" };
    var pick = labels[rnd.Next(labels.Length)];
    var conf = 0.4 + rnd.NextDouble() * 0.55; // 0.40 – 0.95
    var count = rnd.Next(0, 6);
    Write("CameraDemo.Detection.Entrance.Label", count > 0 ? pick : "");
    Write("CameraDemo.Detection.Entrance.Confidence", count > 0 ? Math.Round(conf, 4) : 0.0);
    Write("CameraDemo.Detection.Entrance.Count", count);
    Write("CameraDemo.Detection.Entrance.Alive", true);

    if (pick == "person") Write("CameraDemo.Stats.TotalPeopleDetected", ReadInt("CameraDemo.Stats.TotalPeopleDetected") + count);
    if (pick == "car" || pick == "truck") Write("CameraDemo.Stats.TotalVehiclesDetected", ReadInt("CameraDemo.Stats.TotalVehiclesDetected") + count);
}
else
{
    Write("CameraDemo.Detection.Entrance.Alive", false);
}

// --- Warehouse camera ---
if (ReadBool("CameraDemo.Cameras.Warehouse.YoloEnabled"))
{
    var wLabels = new[] { "person", "forklift", "box", "pallet", "truck" };
    var wPick = wLabels[rnd.Next(wLabels.Length)];
    var wConf = 0.35 + rnd.NextDouble() * 0.6;
    var wCount = rnd.Next(0, 4);
    Write("CameraDemo.Detection.Warehouse.Label", wCount > 0 ? wPick : "");
    Write("CameraDemo.Detection.Warehouse.Confidence", wCount > 0 ? Math.Round(wConf, 4) : 0.0);
    Write("CameraDemo.Detection.Warehouse.Count", wCount);
    Write("CameraDemo.Detection.Warehouse.Alive", true);
}
else
{
    Write("CameraDemo.Detection.Warehouse.Alive", false);
}

// --- Parking lot camera ---
if (ReadBool("CameraDemo.Cameras.ParkingLot.YoloEnabled"))
{
    var pLabels = new[] { "car", "truck", "motorcycle", "person", "bicycle" };
    var pPick = pLabels[rnd.Next(pLabels.Length)];
    var pConf = 0.5 + rnd.NextDouble() * 0.45;
    var pCount = rnd.Next(0, 8);
    Write("CameraDemo.Detection.ParkingLot.Label", pCount > 0 ? pPick : "");
    Write("CameraDemo.Detection.ParkingLot.Confidence", pCount > 0 ? Math.Round(pConf, 4) : 0.0);
    Write("CameraDemo.Detection.ParkingLot.Count", pCount);
    Write("CameraDemo.Detection.ParkingLot.Alive", true);

    if (pPick == "car" || pPick == "truck" || pPick == "motorcycle") Write("CameraDemo.Stats.TotalVehiclesDetected", ReadInt("CameraDemo.Stats.TotalVehiclesDetected") + pCount);
    if (pPick == "person") Write("CameraDemo.Stats.TotalPeopleDetected", ReadInt("CameraDemo.Stats.TotalPeopleDetected") + pCount);
}
else
{
    Write("CameraDemo.Detection.ParkingLot.Alive", false);
}

// Active camera count
int active = 0;
if (ReadBool("CameraDemo.Cameras.Entrance.YoloEnabled")) active++;
if (ReadBool("CameraDemo.Cameras.Warehouse.YoloEnabled")) active++;
if (ReadBool("CameraDemo.Cameras.ParkingLot.YoloEnabled")) active++;
Write("CameraDemo.Stats.ActiveCameras", active);
"""

script = {
    "Name": "SimulateDetections",
    "Code": sim_code.strip(),
    "Enabled": True,
    "IntervalMs": 2000,
    "Language": "CSharp",
    "Group": "Simulation",
    "Breakpoints": []
}

with open(os.path.join(BASE, "scripts", "SimulateDetections.json"), "w", encoding="utf-8") as f:
    json.dump(script, f, indent=2, ensure_ascii=False)
print("5. Created scripts/SimulateDetections.json")


# ════════════════════════════════════════════════════════════
# 6. retentive.json (empty initial state)
# ════════════════════════════════════════════════════════════
with open(os.path.join(BASE, "retentive.json"), "w", encoding="utf-8") as f:
    json.dump({}, f, indent=2)
print("6. Created retentive.json")

print("\n✅ CameraShowcase sample created at:", BASE)
print("   - nodes.json (project with 3 cameras, detection variables, stats)")
print("   - screens/CameraOverview.json (3 cameras side-by-side with detection status)")
print("   - screens/SingleCamera.json (full-size entrance camera with detection details)")
print("   - screens/YoloSettings.json (per-camera YOLO toggle panel with help text)")
print("   - scripts/SimulateDetections.json (simulation script for demo)")
