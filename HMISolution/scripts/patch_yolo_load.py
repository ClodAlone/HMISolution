"""Patch CameraStreamService.cs: load YOLO session when EnableYolo OR YoloEnableVariable is set."""

path = r"C:\Users\cfior\source\repos\Server\CameraStreamService.cs"
with open(path, "r", encoding="utf-8") as f:
    content = f.read()

old = '''            if (config.EnableYolo)
            {
                try
                {
                    var modelPath = string.IsNullOrEmpty(config.YoloModelPath) ? "yolov8n.onnx" : config.YoloModelPath;'''

new = '''            // Load YOLO model if detection is enabled statically OR a dynamic variable is configured
            if (config.EnableYolo || !string.IsNullOrEmpty(config.YoloEnableVariable))
            {
                try
                {
                    var modelPath = string.IsNullOrEmpty(config.YoloModelPath) ? "yolov8n.onnx" : config.YoloModelPath;'''

assert old in content, "EnableYolo condition not found"
content = content.replace(old, new, 1)
print("Patched YOLO session loading condition to include YoloEnableVariable")

with open(path, "w", encoding="utf-8") as f:
    f.write(content)
print("   -> CameraStreamService.cs saved")
