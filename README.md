# LV-WebSockets
A library implementing the WebSocket protocol including support for resource names

## Description

This library is a fork of v1.6.0.22 of the WebSokets library by MediaMongrels Ltd.

**The main new feature is the introduction of support for resource names as per RFC 6455 and better adherence to the RFC standard.**

**As far as the author knows, this is the only LabVIEW WebSocket library supporting resource names.**

The library leverages on the TCP VIs for establishing a WebSocket connection and communicating through it. It allows, for example, a web browser to communicate with a LabVIEW application acting as a WebSocket server.

It contains VIs for performing WebSockets handshaking, reading/writing data and closing the connection. The API allows for you to write code to act as both a WebSocket server, and as a client.

**Since the fork the following changes have been implemented:**

* Added support for WebSocket resources names as per RFC 6455
* Fixed bugs in client handshake code
* Refactored  the client/server handshake code
* Now the server handshake checks if a resource is available as of the input array of resource and it answers with a 404 Not Found HTTP response according to the RFC 6455. Server handshake similarly handles the malformed client handshake by answering with a 400 Bad Request HTTP response according to RFC 6455.  We added a "ProcessHandshakeResponse.vi" that fully checks the server response as of RFC 6455, before there was only a partial check obtained by reusing "the ProcessHandshakeRequest.vi"server code.
* Data Framing Refactoring: Introduced a generic "SendDataFrame.vi" which sends a data frame according to all the possible Opcodes as defined "Opcodes.ctl" enum and also allows to set the Final Frame Bit (FIN Bit). This vi is now used in both the "Write.vi" and "Close.vi", as well as the newly introduced "Ping.vi" and "Pong.vi". 
* The "Close.vi" now correctly sets the FIN bit and allows to send a Close Status as defined in the "StatusCode.ctl" ring and a close reason
* A "DecodeCloseReason.vi" was introduced to decode the Close Status and the reason as delivered by the "Read.vi"
* The "Write.vi" now can send binary an text data frames. 
* The "Read.vi" was refactored so as to allow also binary data, to read the FIN bit and the close reason in case of receive "Connection Close" opcode. As a result now the calling code has the tools to handle the multi-framing and the Ping/Pong exchanges.
* Introduced an enum typedef defining the data format type either text or binary.
* Added a VI to create a valid WebSocket URI staring from IP address, port and service name. 
* The close method now filters error 6066 originating from the waiting Read method when the peer closes the connection. This allows now for a clean connection close