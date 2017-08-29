# WebSocket Server

This library exposes a simple WebSocket server into FXServer.

## Features

* Async
* Text messages
* Authorization header support

## Prerequisites

* `System.Web.dll`
* `System.Security.dll`
* `System.ServiceModel.dll`
* `System.IdentityModel.dll`

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
-- Create a listener with a string parameter
AddEventHandler("my:listener", function(message)
    print("Received message: " .. message)
end)

-- Add your listener to WebSockerServer
TriggerEvent("WebSocketServer:addListener", "my:listener");
```

### Send a message to connected WebSocket clients

TODO

## Built With

* [vtortola/WebSocketListener](https://github.com/vtortola/WebSocketListener)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
