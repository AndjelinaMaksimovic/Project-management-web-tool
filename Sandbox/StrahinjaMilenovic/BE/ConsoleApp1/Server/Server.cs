using ConsoleApp1.Server.controllers;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class SimpleHttpServer
{
    private readonly HttpListener _listener = new HttpListener();
    private readonly PostsManager postsManager = new PostsManager();

    public SimpleHttpServer(string[] prefixes)
    {
        if (!HttpListener.IsSupported)
        {
            throw new NotSupportedException("HTTP Listener is not supported on this platform.");
        }

        foreach (var prefix in prefixes)
        {
            _listener.Prefixes.Add(prefix);
        }
    }

    public void Start()
    {
        _listener.Start();
        Console.WriteLine("Server started. Listening for requests...");

        Task.Run(() =>
        {
            while (_listener.IsListening)
            {
                var context = _listener.GetContext(); // Blocking call, waiting for a request
                ProcessRequest(context);
            }
        });
    }

    public void Stop()
    {
        _listener.Stop();
        _listener.Close();
    }

    private void ProcessRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        // Set CORS headers for all responses
        response.Headers.Add("Access-Control-Allow-Origin", "*"); // Allow requests from any origin (you may want to restrict this in production)
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

        // Get the path from the request URL
        string path = request.Url.AbsolutePath;

        // Process different routes based on the path
        switch (path)
        {
            case "/":
                HandleHomeRoute(response);
                break;
            case "/posts/create":
                HandleCreatePostRoute(response, request);
                break;
            case "/posts/all":
                HandleGetPostsRoute(response);
                break;
            case "/posts/delete":
                HandleDeletePostRoute(response, request);
                break;
            case "/posts/update":
                HandleUpdatePostRoute(response, request);
                break;
            case "/posts":
                HandleGetPostRoute(response, request);
                break;
            default:
                HandleNotFoundRoute(response);
                break;
        }
    }

    private void HandleGetPostRoute(HttpListenerResponse response, HttpListenerRequest request)
    {
        string post = postsManager.GetPost(request.QueryString.Get("id"));
        WriteResponse(response, post);
    }
    private void HandleUpdatePostRoute(HttpListenerResponse response, HttpListenerRequest request)
    {
        postsManager.UpdatePost(request.QueryString.Get("id"), request.QueryString.Get("text"));
        WriteResponse(response, "post updated");
    }
    private void HandleDeletePostRoute(HttpListenerResponse response, HttpListenerRequest request)
    {
        postsManager.DeletePost(request.QueryString.Get("id"));
        WriteResponse(response, "post deleted");
    }
    private void HandleCreatePostRoute(HttpListenerResponse response, HttpListenerRequest request)
    {
        postsManager.CreatePost(request.QueryString.Get("text"));
        WriteResponse(response, "post created");
    }
    private void HandleGetPostsRoute(HttpListenerResponse response)
    {
        var postsData = postsManager.GetAllPosts();
        string res = postsData;
        WriteResponse(response, res);
    }
    private void HandleHomeRoute(HttpListenerResponse response)
    {
        // Construct the response for the home route
        string responseString = "<html><body><h1>Welcome to the Home Page!</h1></body></html>";
        WriteResponse(response, responseString);
    }

    private void HandleNotFoundRoute(HttpListenerResponse response)
    {
        // Construct the response for a not found route
        string responseString = "<html><body><h1>404 - Not Found</h1></body></html>";
        WriteResponse(response, responseString);
    }

    private void WriteResponse(HttpListenerResponse response, string responseString)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentType = "text/html";
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
}