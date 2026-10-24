"""MJPEG navigation regression: requires Python Playwright and Microsoft Edge.

Run: python Tests\Tests.RuntimeViewer\camera-navigation.browser.py
Use --native to reproduce the original unmanaged <img> connection leak.
"""

import base64
import sys
import threading
import time
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path

from playwright.sync_api import sync_playwright

image = b''
active = set()
lock = threading.Lock()
module_path = Path(__file__).resolve().parents[2] / 'RuntimeViewer.Shared' / 'wwwroot' / 'ipcamera.js'
managed = '--native' not in sys.argv


class Handler(BaseHTTPRequestHandler):
    protocol_version = 'HTTP/1.1'

    def log_message(self, *args):
        pass

    def handle(self):
        try:
            super().handle()
        except (ConnectionResetError, ConnectionAbortedError):
            pass  # Expected when the browser cancels a stream or closes a socket.

    def do_GET(self):
        if not self.path.startswith('/stream'):
            body = module_path.read_bytes() if self.path == '/ipcamera.js' else b'<html><body><div id="screen"></div></body></html>'
            self.send_response(200)
            self.send_header('Content-Type', 'text/javascript' if self.path == '/ipcamera.js' else 'text/html')
            self.send_header('Content-Length', str(len(body)))
            self.end_headers()
            self.wfile.write(body)
            return
        key = self.path
        with lock:
            active.add(key)
        try:
            self.send_response(200)
            self.send_header('Content-Type', 'multipart/x-mixed-replace; boundary=frame')
            self.send_header('Cache-Control', 'no-cache')
            self.end_headers()
            while True:
                self.wfile.write(b'--frame\r\nContent-Type: image/jpeg\r\nContent-Length: ' + str(len(image)).encode() + b'\r\n\r\n' + image + b'\r\n')
                self.wfile.flush()
                time.sleep(0.1)
        except (BrokenPipeError, ConnectionResetError, ConnectionAbortedError):
            pass  # Expected when a widget is removed, hidden, or retried.
        finally:
            with lock:
                active.discard(key)


def expect_connections(expected):
    deadline = time.monotonic() + 5
    while time.monotonic() < deadline:
        with lock:
            count = len(active)
        if count == expected:
            return
        time.sleep(0.05)
    raise AssertionError(f'Expected {expected} active streams; found {count}')


server = ThreadingHTTPServer(('127.0.0.1', 0), Handler)
server.daemon_threads = True
threading.Thread(target=server.serve_forever, daemon=True).start()
try:
    with sync_playwright() as p:
        browser = p.chromium.launch(channel='msedge', headless=True)
        page = browser.new_page()
        page.goto(f'http://127.0.0.1:{server.server_port}')
        image = base64.b64decode(page.evaluate('''() => {
            const canvas = document.createElement('canvas');
            canvas.width = canvas.height = 32;
            const context = canvas.getContext('2d');
            context.fillStyle = '#16a34a';
            context.fillRect(0, 0, 32, 32);
            return canvas.toDataURL('image/jpeg').split(',')[1];
        }'''))
        if managed:
            page.evaluate("import('/ipcamera.js').then(module => window.camera = module)")
        for visit in range(12):
            page.evaluate('''({ visit, managed }) => {
                const screen = document.getElementById('screen');
                screen.innerHTML = '<svg><foreignObject width="600" height="150"><div xmlns="http://www.w3.org/1999/xhtml"></div></foreignObject></svg>';
                const container = screen.querySelector('foreignObject div');
                for (let i = 0; i < 3; i++) {
                    const img = document.createElement('img');
                    img.width = img.height = 32;
                    container.appendChild(img);
                    const url = `/stream?visit=${visit}&cam=${i}`;
                    if (managed) camera.start(img, url);
                    else img.src = url;
                }
            }''', {'visit': visit, 'managed': managed})
            page.wait_for_function('Array.from(document.querySelectorAll("img")).every(img => img.naturalWidth === 32)', timeout=5000)
            if managed:
                expect_connections(3)
            print(f'Visit {visit}: all three camera images loaded', flush=True)
        if managed:
            for _ in range(3):
                page.evaluate('document.getElementById("screen").hidden = true')
                expect_connections(0)
                page.evaluate('document.getElementById("screen").hidden = false')
                page.wait_for_function('Array.from(document.querySelectorAll("img")).every(img => img.naturalWidth === 32)', timeout=5000)
                expect_connections(3)
            print('Cached-screen hide/show: streams stop and resume', flush=True)
            page.evaluate('''() => {
                const img = document.querySelector('img');
                camera.start(img, '/stream?retry=1');
            }''')
            page.wait_for_function('document.querySelector("img").naturalWidth === 32', timeout=5000)
            expect_connections(3)
            print('Retry: replacement image loads without leaking its previous stream', flush=True)
        page.evaluate('document.getElementById("screen").replaceChildren()')
        expect_connections(0)
        print('All widgets removed: zero active streams', flush=True)
        browser.close()
finally:
    server.shutdown()
