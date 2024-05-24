import http.server
import socketserver
import os


class MyHTTPRequestHandler(http.server.SimpleHTTPRequestHandler):
    
    def do_GET(self):
        path = self.translate_path(self.path)

        if os.path.exists(path):
            return http.server.SimpleHTTPRequestHandler.do_GET(self)
        else:
            self.send_response(200)
            self.send_header('Content-type', 'text/html')
            self.end_headers()
            with open('index.html','rb') as f:
                chunk = f.read(8192)
                while chunk :
                    self.wfile.write(chunk)
                    chunk = f.read(8192)

PORT = 10123
with socketserver.TCPServer(("", PORT), MyHTTPRequestHandler) as httpd:
    print("Serving at port ", PORT)
    httpd.serve_forever()
        