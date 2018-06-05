using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using vtortola.WebSockets;

namespace WebSocketServer
{
	class WebSocketServer : BaseScript
	{
		private enum LogLevels { Debug, Info, Warn, Error };
		private LogLevels logLevel;

		private string authorization;
		private List<WebSocket> webSockets;

		private async Task<bool> CheckHttpHeaders(WebSocketHttpRequest request, WebSocketHttpResponse response)
		{
			await Task.Run(() =>
			{
				if (authorization.Length > 0)
				{
					var authHeader = request.Headers["Authorization"];
					if (authHeader == null || authHeader != authorization)
					{
						response.Status = HttpStatusCode.Unauthorized;
						Log("Rejected Authorization header: " + authHeader, LogLevels.Debug);
					}
				}
			});

			return true;
		}

		public WebSocketServer()
		{
			logLevel = Function.Call<string>(Hash.GET_CONVAR, "websocket_debug", "false") == "true" ? LogLevels.Debug : LogLevels.Info;
			authorization = Function.Call<string>(Hash.GET_CONVAR, "websocket_authorization", "");

			IPAddress listeningHost = IPAddress.Loopback;
			IPAddress.TryParse(Function.Call<string>(Hash.GET_CONVAR, "websocket_host", "127.0.0.1"), out listeningHost);
			int listeningPort = Function.Call<int>(Hash.GET_CONVAR_INT, "websocket_port", 80);
			var endpoint = new IPEndPoint(listeningHost, listeningPort);

			var server = new WebSocketEventListener (endpoint, new WebSocketListenerOptions () {
				SubProtocols = new String[]{ "text" },
				HttpAuthenticationHandler = CheckHttpHeaders
			});
			server.OnConnect += (ws) =>
			{
				Log ("Connection from " + ws.RemoteEndpoint.ToString (), LogLevels.Debug);

				TriggerEvent("WebSocketServer:onConnect", ws.RemoteEndpoint.ToString ());

				lock (webSockets)
				{
					webSockets.Add(ws);
				}
			};
			server.OnDisconnect += (ws) =>
			{
				Log ("Disconnection from " + ws.RemoteEndpoint.ToString (), LogLevels.Debug);

				TriggerEvent("WebSocketServer:onDisconnect", ws.RemoteEndpoint.ToString ());

				lock (webSockets)
				{
					webSockets.Remove(ws);
				}
			};
			server.OnError += (ws, ex) => Log("Error: " + ex.Message, LogLevels.Debug);
			server.OnMessage += (ws, msg) =>
			{
				Log("Received message: " + msg, LogLevels.Debug);

				TriggerEvent("WebSocketServer:onMessage", msg, ws.RemoteEndpoint.ToString ());
			};

			EventHandlers["onResourceStart"] += new Action<dynamic>((dynamic resourceName) =>
			{
				if ((string) resourceName == "WebSocketServer")
				{
					try
					{
						server.Start();
						Tick += server.ListenAsync;

						Log("Started at " + endpoint.ToString());
					}
					catch (Exception e)
					{
						Log("Can't start server at " + endpoint.ToString() + ": " + e.Message);
					}
				}
			});

			EventHandlers["onResourceStop"] += new Action<dynamic>((dynamic resourceName) =>
			{
				if ((string) resourceName == "WebSocketServer")
				{
					server.Stop();
					server.Dispose();
				}
			});

			webSockets = new List<WebSocket>();
			EventHandlers["WebSocketServer:broadcast"] += new Action<dynamic>((dynamic message) =>
			{
				lock (webSockets)
				{
					foreach (var webSocket in webSockets)
					{
						if (webSocket.IsConnected)
						{
							Task.Run(async () =>
							{
								try
								{
									await webSocket.WriteStringAsync((string) message, CancellationToken.None);
								}
								catch (Exception e)
								{
									Log("An error occurred while sending a message to " + webSocket.RemoteEndpoint.ToString() + ": " + e.Message, LogLevels.Debug);
								}
							});
						}
						else
						{
							webSockets.Remove(webSocket);
						}
					}
				}
			});

			EventHandlers["WebSocketServer:send"] += new Action<dynamic, dynamic>((dynamic message, dynamic client) =>
			{
				lock (webSockets)
				{
					foreach (var webSocket in webSockets)
					{
						if (webSocket.IsConnected)
						{
							if (webSocket.RemoteEndpoint.ToString () == client)
							{
								Task.Run(async () =>
								{
									try
									{
										await webSocket.WriteStringAsync((string) message, CancellationToken.None);
									}
									catch (Exception e)
									{
										Log("An error occurred while sending a message to " + webSocket.RemoteEndpoint.ToString() + ": " + e.Message, LogLevels.Debug);
									}
								});
                            }
						}
						else
						{
							webSockets.Remove(webSocket);
						}
					}
				}
			});



		}

		private void Log(string message, LogLevels level = LogLevels.Info)
		{
			if (logLevel > level)
			{
				return;
			}

			Console.WriteLine("[WebSocket Server] " + message);
		}
	}
}
