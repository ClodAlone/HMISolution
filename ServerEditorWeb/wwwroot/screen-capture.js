// QW-19: Copy SVG canvas as PNG to clipboard
window.screenCapture = {
    copyCanvasPng: async function () {
        const svg = document.querySelector('.screen-canvas');
        if (!svg) return;

        const svgData = new XMLSerializer().serializeToString(svg);
        const width = svg.width.baseVal.value || 800;
        const height = svg.height.baseVal.value || 600;

        const canvas = document.createElement('canvas');
        canvas.width = width;
        canvas.height = height;
        const ctx = canvas.getContext('2d');

        const img = new Image();
        const blob = new Blob([svgData], { type: 'image/svg+xml;charset=utf-8' });
        const url = URL.createObjectURL(blob);

        await new Promise((resolve, reject) => {
            img.onload = () => {
                ctx.drawImage(img, 0, 0);
                URL.revokeObjectURL(url);
                resolve();
            };
            img.onerror = reject;
            img.src = url;
        });

        canvas.toBlob(async (pngBlob) => {
            try {
                await navigator.clipboard.write([
                    new ClipboardItem({ 'image/png': pngBlob })
                ]);
            } catch {
                // Fallback: trigger download
                const a = document.createElement('a');
                a.href = URL.createObjectURL(pngBlob);
                a.download = 'screen-export.png';
                a.click();
                URL.revokeObjectURL(a.href);
            }
        }, 'image/png');
    }
};
