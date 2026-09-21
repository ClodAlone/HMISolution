#!/usr/bin/env python3
import argparse
import io
import time
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from threading import Lock

from PIL import Image, ImageDraw


class CameraStreamHandler(BaseHTTPRequestHandler):
    protocol_version = "HTTP/1.1"

    def do_GET(self):
        if self.path in {"/", "/mjpeg", "/mjpeg/"}:
            self._send_mjpeg()
            return

        if self.path in {"/snapshot", "/snapshot/"}:
            self._send_snapshot()
            return

        self.send_response(404)
        self.end_headers()

    def _send_snapshot(self):
        frame = make_frame(self.server.frame_index)
        self.send_response(200)
        self.send_header("Content-Type", "image/jpeg")
        self.send_header("Content-Length", str(len(frame)))
        self.send_header("Cache-Control", "no-cache")
        self.end_headers()
        self.wfile.write(frame)
        self.wfile.flush()

    def _send_mjpeg(self):
        self.send_response(200)
        self.send_header("Content-Type", "multipart/x-mixed-replace; boundary=--frame")
        self.send_header("Cache-Control", "no-cache, private")
        self.end_headers()

        try:
            while True:
                frame = make_frame(self.server.frame_index)
                self.server.frame_index += 1
                self.wfile.write(b"\r\n--frame\r\n")
                self.wfile.write(b"Content-Type: image/jpeg\r\n")
                self.wfile.write(f"Content-Length: {len(frame)}\r\n\r\n".encode("ascii"))
                self.wfile.write(frame)
                self.wfile.write(b"\r\n")
                self.wfile.flush()
                time.sleep(self.server.fps_interval)
        except (BrokenPipeError, ConnectionResetError, ConnectionAbortedError):
            pass

    def log_message(self, format, *args):
        return


def make_frame(index):
    width, height = 320, 240
    image = Image.new("RGB", (width, height), (30, 50, 80))
    draw = ImageDraw.Draw(image)
    draw.rectangle((15, 15, width - 15, height - 15), outline=(255, 255, 255), width=3)
    draw.text((28, 28), f"Frame {index}", fill=(255, 255, 255))
    draw.text((28, 58), time.strftime("%H:%M:%S"), fill=(255, 255, 255))
    draw.line((0, height // 2, width, height // 2), fill=(150, 220, 255), width=2)
    draw.line((width // 2, 0, width // 2, height), fill=(150, 220, 255), width=2)
    buffer = io.BytesIO()
    image.save(buffer, format="JPEG", quality=85)
    return buffer.getvalue()


class CameraTestServer(ThreadingHTTPServer):
    allow_reuse_address = True

    def __init__(self, server_address, handler_class, fps):
        super().__init__(server_address, handler_class)
        self.fps = fps
        self.fps_interval = 1.0 / fps
        self.frame_index = 0


def main():
    parser = argparse.ArgumentParser(description="Serve a valid MJPEG test camera stream")
    parser.add_argument("--host", default="127.0.0.1")
    parser.add_argument("--port", type=int, default=8092)
    parser.add_argument("--fps", type=float, default=5.0)
    args = parser.parse_args()

    server = CameraTestServer((args.host, args.port), CameraStreamHandler, args.fps)
    print(f"Valid MJPEG test camera running at http://{args.host}:{args.port}/mjpeg")
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        pass
    finally:
        server.server_close()


if __name__ == "__main__":
    main()
