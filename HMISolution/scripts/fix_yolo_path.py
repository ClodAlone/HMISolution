import os

path = r'C:\Users\cfior\source\repos\Server\CameraHostedService.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

old = '''    private List<CameraConfig> LoadCameraConfigs()
    {
        try
        {
            if (!File.Exists(_serverConfig.NodesConfigFile)) return [];

            using var stream = File.OpenRead(_serverConfig.NodesConfigFile);
            var model = JsonSerializer.Deserialize(stream, ServerJsonContext.Default.NodeModel);
            if (model?.Cameras != null && model.Cameras.Count > 0)
                return model.Cameras;

            // Also scan screens for ipcamera symbols with inline camera configs
            var cameras = new List<CameraConfig>();
            if (model?.Screens != null)
            {
                foreach (var screen in model.Screens)
                {
                    foreach (var sym in screen.Symbols)
                    {
                        if (sym.Type == "ipcamera" && sym.Camera != null && !string.IsNullOrEmpty(sym.Camera.CameraId))
                        {
                            if (!cameras.Any(c => c.CameraId == sym.Camera.CameraId))
                                cameras.Add(sym.Camera);
                        }
                    }
                }
            }

            return cameras;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load camera configs");
            return [];
        }
    }'''

new = '''    private List<CameraConfig> LoadCameraConfigs()
    {
        try
        {
            if (!File.Exists(_serverConfig.NodesConfigFile)) return [];

            using var stream = File.OpenRead(_serverConfig.NodesConfigFile);
            var model = JsonSerializer.Deserialize(stream, ServerJsonContext.Default.NodeModel);

            List<CameraConfig> cameras;
            if (model?.Cameras != null && model.Cameras.Count > 0)
            {
                cameras = model.Cameras;
            }
            else
            {
                // Also scan screens for ipcamera symbols with inline camera configs
                cameras = new List<CameraConfig>();
                if (model?.Screens != null)
                {
                    foreach (var screen in model.Screens)
                    {
                        foreach (var sym in screen.Symbols)
                        {
                            if (sym.Type == "ipcamera" && sym.Camera != null && !string.IsNullOrEmpty(sym.Camera.CameraId))
                            {
                                if (!cameras.Any(c => c.CameraId == sym.Camera.CameraId))
                                    cameras.Add(sym.Camera);
                            }
                        }
                    }
                }
            }

            // Resolve YOLO model paths relative to the project directory
            var projectDir = Path.GetDirectoryName(Path.GetFullPath(_serverConfig.NodesConfigFile));
            if (projectDir != null)
            {
                foreach (var cam in cameras)
                {
                    var modelFile = string.IsNullOrEmpty(cam.YoloModelPath) ? "yolov8n.onnx" : cam.YoloModelPath;
                    if (!Path.IsPathRooted(modelFile))
                    {
                        // Check project directory first, then fall back to working directory
                        var projectRelative = Path.Combine(projectDir, modelFile);
                        if (File.Exists(projectRelative))
                        {
                            cam.YoloModelPath = Path.GetFullPath(projectRelative);
                        }
                        else if (File.Exists(modelFile))
                        {
                            cam.YoloModelPath = Path.GetFullPath(modelFile);
                        }
                        // else leave as-is; CameraStreamService will log "not found"
                    }
                }
            }

            return cameras;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load camera configs");
            return [];
        }
    }'''

count = content.count(old)
if count == 0:
    # Try with \r\n
    old_crlf = old.replace('\n', '\r\n')
    count = content.count(old_crlf)
    if count == 1:
        new_crlf = new.replace('\n', '\r\n')
        content = content.replace(old_crlf, new_crlf)
        print('Replaced using CRLF line endings')
    else:
        print(f'ERROR: Found {count} matches with CRLF')
elif count == 1:
    content = content.replace(old, new)
    print('Replaced using LF line endings')
else:
    print(f'ERROR: Found {count} matches')

if count == 1:
    with open(path, 'w', encoding='utf-8-sig') as f:
        f.write(content)
    print('File saved successfully.')
