# WebSocket Server

This library exposes a simple WebSocket server into FXServer.

## Features

* Async
* Text messages
* Authorization header support

## Configuration

Convars available:

| Name                    	| Type   	| Default value 	| Description                                                 	|
|-------------------------	|--------	|---------------	|-------------------------------------------------------------	|
| websocket_debug         	| bool   	| false         	| Defines the verbosity of logs                               	|
| websocket_host          	| string 	| "127.0.0.1"   	| Defines listening host                                      	|
| websocket_port          	| int    	| 80            	| Defines listening port                                      	|
| websocket_authorization 	| string 	| ""            	| Defines accepted Authorization header value (auth disabled if empty) 	|

## Usage

### Add a listener to receive messages

```lua
AddEventHandler("WebSocketServer:onMessage", function(message, endpoint)
    print("Received message from " .. endpoint .. ": " .. message)
end)
```

### Add a listener to get new connected remote endpoints

```lua
AddEventHandler("WebSocketServer:onConnect", function(endpoint)
    print("New WS remote endpoint: " .. endpoint)
end)
```

### Add a listener to get disconnected remote endpoints

```lua
AddEventHandler("WebSocketServer:onDisconnect", function(endpoint)
    print("WS remote endpoint " .. endpoint .. " has been disconnected")
end)
```

### Send a message to connected WebSocket clients

```lua
TriggerEvent("WebSocketServer:broadcast", "This message will be broadcasted to all connected webSocket clients.");
```

### Send a message to a specific WebSocket client

```lua
TriggerEvent("WebSocketServer:send", "This message will be sent to a specific webSocket client.", someValidAndConnectedRemoteEndpoint);
```

## Built With

* [deniszykov/WebSocketListener](https://github.com/deniszykov/WebSocketListener)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
