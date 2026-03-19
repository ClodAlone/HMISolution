window.imageDropZone = {
    init: function (element, dotNetRef) {
        if (!element) return;

        element.addEventListener('dragover', function (e) {
            e.preventDefault();
            e.stopPropagation();
            e.dataTransfer.dropEffect = 'copy';
            element.classList.add('drag-over');
        });

        element.addEventListener('dragleave', function (e) {
            e.preventDefault();
            e.stopPropagation();
            element.classList.remove('drag-over');
        });

        element.addEventListener('drop', function (e) {
            e.preventDefault();
            e.stopPropagation();
            element.classList.remove('drag-over');

            var files = e.dataTransfer.files;
            if (!files || files.length === 0) return;

            var imageFiles = [];
            for (var i = 0; i < files.length; i++) {
                var f = files[i];
                if (f.type && f.type.startsWith('image/')) {
                    imageFiles.push(f);
                }
            }

            if (imageFiles.length === 0) return;

            var processed = 0;
            var results = [];

            imageFiles.forEach(function (file) {
                var reader = new FileReader();
                reader.onload = function (ev) {
                    // Extract filename without extension as the ID
                    var name = file.name;
                    var dotIdx = name.lastIndexOf('.');
                    var id = dotIdx > 0 ? name.substring(0, dotIdx) : name;

                    results.push({ id: id, dataUri: ev.target.result, fileName: file.name });
                    processed++;

                    if (processed === imageFiles.length) {
                        dotNetRef.invokeMethodAsync('OnFilesDropped', results);
                    }
                };
                reader.readAsDataURL(file);
            });
        });
    }
};
