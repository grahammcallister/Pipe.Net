# Pipe.Net
A simple C# named pipe server implementation that can be used to share data with Node.js instances

Available as a NuGet package download on ![Nuget Gallery](https://www.nuget.org/packages/Pipe.Net/) here: [![https://www.nuget.org/packages/Pipe.Net/](https://www.nuget.org/packages/Pipe.Net/)](https://www.nuget.org/packages/Pipe.Net/) 

**Build status**

[![Build status](https://ci.appveyor.com/api/projects/status/x3y5ewmb6nud47am?svg=true)](https://ci.appveyor.com/project/grahammcallister/pipe-net)

**Example usage**

***Server***
The project is firstly designed for the Server side of the pipe to be on the .Net side.
See the demo project Pipe.Net.Demo for server example.

Run the Pipe.Demo project. "Open" the server, and then the client. Use the "send to client" and "send to server" functions to send messages though the pipe.

![Demo screen capture](https://github.com/grahammcallister/Pipe.Net/blob/master/Demo_screen_capture.png)

***Node client***
See the Node.Demo folder for the node client demo.

*Note: The demo code requires that the .Net server should already be running.*

Run the Pipe.Demo .Net project in Studio. "Open" the server.

Run the index.js script with node.

Send a message through the pipe using the "send to clients" feature in the .Net server.

![Node demo screen capture](https://github.com/grahammcallister/Pipe.Net/blob/master/Node_demo_screen_capture.png)



