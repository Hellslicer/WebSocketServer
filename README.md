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
-- Add a new listener with a string parameter
AddEventHandler("WebSocketServer:onMessage", function(message)
    print("Received message: " .. message)
end)
```

### Send a message to connected WebSocket clients

```lua
TriggerEvent("WebSocketServer:broadcast", "This message will be broadcasted to all connected webSocket clients.");
```

## Built With

* [deniszykov/WebSocketListener](https://github.com/deniszykov/WebSocketListener)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
