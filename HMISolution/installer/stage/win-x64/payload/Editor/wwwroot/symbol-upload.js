window.symbolUpload = {
    _dotNetRef: null,
    _files: [],

    init: function (dotNetRef) {
        this._dotNetRef = dotNetRef;
        this._files = [];
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
                svgFiles.push(f);
            }
        }
        this._files = svgFiles;
        if (this._dotNetRef) {
            this._dotNetRef.invokeMethodAsync('OnFilesSelected', svgFiles.length);
        }
        // Immediately read the files
        this.readFiles();
    },

    readFiles: function () {
        var self = this;
        var remaining = this._files.length;
        if (remaining === 0) {
            if (self._dotNetRef) {
                self._dotNetRef.invokeMethodAsync('OnAllFilesRead');
            }
            return;
        }

        this._files.forEach(function (file) {
            var reader = new FileReader();
            reader.onload = function () {
                var bytes = new Uint8Array(reader.result);
                if (self._dotNetRef) {
                    self._dotNetRef.invokeMethodAsync('OnFileData', file.name, Array.from(bytes));
                }
                remaining--;
                if (remaining === 0 && self._dotNetRef) {
                    self._dotNetRef.invokeMethodAsync('OnAllFilesRead');
                }
            };
            reader.readAsArrayBuffer(file);
        });
    }
};
