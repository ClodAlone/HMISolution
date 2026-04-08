"""Patch the REAL SharedModels/NodeModels.cs to add YoloEnableVariable."""

path = r"C:\Users\cfior\source\repos\SharedModels\NodeModels.cs"
with open(path, "r", encoding="utf-8") as f:
    content = f.read()

old = '''        /// <summary>Enable YOLO object detection on frames.</summary>
        public bool EnableYolo { get; set; }

        /// <summary>Path to the YOLO ONNX model file. Empty = use default bundled model.</summary>'''

new = '''        /// <summary>Enable YOLO object detection on frames.</summary>
        public bool EnableYolo { get; set; }

        /// <summary>
        /// OPC variable path (Boolean) that dynamically enables/disables YOLO detection at runtime.
        /// When set, the server reads this variable each frame; true = run detection, false = skip.
        /// When empty, detection follows the static <see cref="EnableYolo"/> flag.
        /// </summary>
        public string YoloEnableVariable { get; set; } = "";

        /// <summary>Path to the YOLO ONNX model file. Empty = use default bundled model.</summary>'''

assert old in content, "EnableYolo property block not found in real SharedModels"
content = content.replace(old, new, 1)
print("Added YoloEnableVariable property to real SharedModels/NodeModels.cs")

with open(path, "w", encoding="utf-8") as f:
    f.write(content)
print("   -> SharedModels/NodeModels.cs saved")
