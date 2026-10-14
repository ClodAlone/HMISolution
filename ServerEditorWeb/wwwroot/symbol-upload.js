window.symbolUpload = {
    _dotNetRef: null,
    _files: [],
    _reading: false,

    init: function (dotNetRef) {
        this._dotNetRef = dotNetRef;
        this._files = [];
        this._reading = false;
    },

    pickFolder: function (inputEl) {
        var self = this;
        inputEl.value = '';
        inputEl.onchange = function () {
            self._onInputChange(inputEl);
        };
        inputEl.click();
    },

    pickFiles: function (inputEl) {
        var self = this;
        inputEl.value = '';
        inputEl.onchange = function () {
            self._onInputChange(inputEl);
        };
        inputEl.click();
    },

    _onInputChange: function (inputEl) {
        var svgFiles = [];
        for (var i = 0; i < inputEl.files.length; i++) {
            var f = inputEl.files[i];
            if (f.name.toLowerCase().endsWith('.svg')) {
                // When a folder is picked (webkitdirectory), webkitRelativePath looks like
                // "SelectedFolder/SubFolder/file.svg". Use the immediate parent folder name
                // as the category so importing a tree of subfolders creates/uses one category
                // per subfolder instead of dumping everything into a single typed category.
                var relPath = f.webkitRelativePath || '';
                var parts = relPath.split('/');
                var category = parts.length >= 2 ? parts[parts.length - 2] : null;
                svgFiles.push({ file: f, category: category });
            }
        }
        this._files = svgFiles;
        if (this._dotNetRef) {
            this._dotNetRef.invokeMethodAsync('OnFilesSelected', svgFiles.length);
        }
        // Reading is deferred until the user clicks Import (readFiles is invoked
        // explicitly from Blazor's DoUpload). Triggering it here as well used to
        // start a second, concurrent read pass racing with the one kicked off by
        // DoUpload, which corrupted batch ordering and left the progress bar stuck.
    },

    readFiles: function () {
        var self = this;
        if (this._reading) {
            // Already reading (e.g. triggered once already) — avoid starting a
            // duplicate, concurrent pass over the same files.
            return;
        }
        this._reading = true;

        var files = this._files;
        var total = files.length;
        if (total === 0) {
            this._reading = false;
            if (self._dotNetRef) {
                self._dotNetRef.invokeMethodAsync('OnAllFilesRead');
            }
            return;
        }

        // Read and send files in batches to avoid flooding SignalR/JSInterop with
        // thousands of individual round-trips when importing large folders.
        var BATCH_SIZE = 50;
        var index = 0;

        function readOne(entry) {
            return new Promise(function (resolve) {
                var reader = new FileReader();
                reader.onload = function () {
                    var bytes = new Uint8Array(reader.result);
                    var binary = '';
                    for (var i = 0; i < bytes.length; i++) {
                        binary += String.fromCharCode(bytes[i]);
                    }
                    resolve({ name: entry.file.name, category: entry.category, base64: btoa(binary) });
                };
                reader.onerror = function () {
                    resolve(null);
                };
                reader.readAsArrayBuffer(entry.file);
            });
        }

        function processNextBatch() {
            if (index >= total) {
                self._reading = false;
                if (self._dotNetRef) {
                    self._dotNetRef.invokeMethodAsync('OnAllFilesRead');
                }
                return;
            }

            var batch = files.slice(index, index + BATCH_SIZE);
            index += batch.length;

            Promise.all(batch.map(readOne)).then(function (results) {
                var names = [];
                var datas = [];
                var categories = [];
                results.forEach(function (r) {
                    if (r) {
                        names.push(r.name);
                        datas.push(r.base64);
                        categories.push(r.category || '');
                    }
                });
                if (self._dotNetRef && names.length > 0) {
                    self._dotNetRef.invokeMethodAsync('OnFileBatch', names, datas, categories).then(function () {
                        processNextBatch();
                    });
                } else {
                    processNextBatch();
                }
            });
        }

        processNextBatch();
    }
};
