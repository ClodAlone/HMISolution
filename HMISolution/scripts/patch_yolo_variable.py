"""Patch CameraStreamService.cs and CameraHostedService.cs for YoloEnableVariable support."""

# ── 1. CameraStreamService.cs ──────────────────────────────
path = r"C:\Users\cfior\source\repos\Server\CameraStreamService.cs"
with open(path, "r", encoding="utf-8") as f:
    content = f.read()

# 1a. Update StartCamera signature and body
old = '''    public void StartCamera(CameraConfig config, Action<string, string, double, int>? onDetection = null)
    {
        if (_cameras.ContainsKey(config.CameraId)) return;
        var instance = new CameraInstance(config, _logger, onDetection);
        _cameras[config.CameraId] = instance;
        instance.Start();
    }'''

new = '''    public void StartCamera(CameraConfig config, Action<string, string, double, int>? onDetection = null, Func<string, bool>? readBoolVariable = null)
    {
        if (_cameras.ContainsKey(config.CameraId)) return;
        var instance = new CameraInstance(config, _logger, onDetection, readBoolVariable);
        _cameras[config.CameraId] = instance;
        instance.Start();
    }'''

assert old in content, "StartCamera not found"
content = content.replace(old, new, 1)
print("1a. Patched StartCamera signature")

# 1b. Add _readBoolVariable field
old = '''        private readonly Action<string, string, double, int>? _onDetection;
        private CancellationTokenSource? _cts;'''

new = '''        private readonly Action<string, string, double, int>? _onDetection;
        private readonly Func<string, bool>? _readBoolVariable;
        private CancellationTokenSource? _cts;'''

assert old in content, "_onDetection field not found"
content = content.replace(old, new, 1)
print("1b. Added _readBoolVariable field")

# 1c. Update CameraInstance constructor signature and assignment
old = '''        public CameraInstance(CameraConfig config, ILogger logger, Action<string, string, double, int>? onDetection)
        {
            _config = config;
            _logger = logger;
            _onDetection = onDetection;'''

new = '''        public CameraInstance(CameraConfig config, ILogger logger, Action<string, string, double, int>? onDetection, Func<string, bool>? readBoolVariable)
        {
            _config = config;
            _logger = logger;
            _onDetection = onDetection;
            _readBoolVariable = readBoolVariable;'''

assert old in content, "CameraInstance constructor not found"
content = content.replace(old, new, 1)
print("1c. Updated CameraInstance constructor")

# 1d. Add IsYoloEnabledAtRuntime method and modify ProcessFrameAsync check
old = '''        private async Task ProcessFrameAsync(byte[] jpegBytes)
        {
            if (_yoloSession != null)
            {'''

new = '''        /// <summary>
        /// Checks the YoloEnableVariable (if configured) to dynamically enable/disable detection.
        /// Returns true if detection should run.
        /// </summary>
        private bool IsYoloEnabledAtRuntime()
        {
            if (!string.IsNullOrEmpty(_config.YoloEnableVariable) && _readBoolVariable != null)
            {
                try { return _readBoolVariable(_config.YoloEnableVariable); }
                catch { return true; } // variable not found — default to enabled
            }
            return true; // no variable configured — always enabled
        }

        private async Task ProcessFrameAsync(byte[] jpegBytes)
        {
            if (_yoloSession != null && IsYoloEnabledAtRuntime())
            {'''

assert old in content, "ProcessFrameAsync not found"
content = content.replace(old, new, 1)
print("1d. Added IsYoloEnabledAtRuntime + patched ProcessFrameAsync")

with open(path, "w", encoding="utf-8") as f:
    f.write(content)
print("   -> CameraStreamService.cs saved")


# ── 2. CameraHostedService.cs ──────────────────────────────
path2 = r"C:\Users\cfior\source\repos\Server\CameraHostedService.cs"
with open(path2, "r", encoding="utf-8") as f:
    content2 = f.read()

# 2a. Update the StartCamera call to pass a variable-reader lambda
old2 = '''            _cameraService.StartCamera(cam, (prefix, label, confidence, count) =>
            {
                WriteDetectionVariables(prefix, label, confidence, count);
            });'''

new2 = '''            _cameraService.StartCamera(cam, (prefix, label, confidence, count) =>
            {
                WriteDetectionVariables(prefix, label, confidence, count);
            },
            varPath => ReadBoolVariable(varPath));'''

assert old2 in content2, "StartCamera call not found in CameraHostedService"
content2 = content2.replace(old2, new2, 1)
print("2a. Updated StartCamera call with readBoolVariable lambda")

# 2b. Add ReadBoolVariable method after WriteDetectionVariables
old2b = '''    private void WriteDetectionVariables(string prefix, string label, double confidence, int count)
    {
        if (_nodeManager == null) return;'''

new2b = '''    /// <summary>
    /// Reads a Boolean OPC variable by path. Used by CameraInstance to check YoloEnableVariable.
    /// </summary>
    private bool ReadBoolVariable(string variablePath)
    {
        if (_nodeManager == null) return true;
        try
        {
            var value = _nodeManager.ReadVariable(variablePath);
            return Convert.ToBoolean(value);
        }
        catch
        {
            return true; // variable not found — default to enabled
        }
    }

    private void WriteDetectionVariables(string prefix, string label, double confidence, int count)
    {
        if (_nodeManager == null) return;'''

assert old2b in content2, "WriteDetectionVariables not found in CameraHostedService"
content2 = content2.replace(old2b, new2b, 1)
print("2b. Added ReadBoolVariable method")

with open(path2, "w", encoding="utf-8") as f:
    f.write(content2)
print("   -> CameraHostedService.cs saved")

print("\nDone — CameraStreamService.cs and CameraHostedService.cs patched.")
