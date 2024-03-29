# LV-WebSockets
A library implementing the WebSocket protocol including support for resource names

## Description

This library is a fork of v1.6.0.22 of the WebSokets library by MediaMongrels Ltd.

**The main new feature is the introduction of support for resource names as per RFC 6455 and better adherence to the RFC standard.**

**As far as the author knows, this is the only LabVIEW WebSocket library supporting resource names.**

The library leverages on the TCP VIs for establishing a WebSocket connection and communicating through it. It allows, for example, a web browser to communicate with a LabVIEW application acting as a WebSocket server.

It contains VIs for performing WebSockets handshaking, reading/writing data and closing the connection. The API allows for you to write code to act as both a WebSocket server, and as a client.

## Changelog since the initial fork:
### v1.8.0
- Improved error handling in client handshake
- Implemented URI parsing for wss/https URIs (#4)
- Implemented support for Secure WebSocket  (Closes #4)
- Now if the client/server handshaking fails the TCP connection will be closed (Closes #5)
- Up to now, the Sec-WebSocket-Key was a value hard-coded in the code. Implemented the generation of a random 16 bytes string and then its encoding to a 24 bytes Base64 string as specified in the RFC 6455 . This will improve the security of the implementation (Closes #1).
- Minor improvement to the example code
- The previous implementation of the Base64 encoding was relatively slow and inefficient. We have replaced it with a highly optimized pure LabVIEW implementations available here https://forums.ni.com/t5/Example-Code/Fast-Base64-Encoder-Decoder-using-LabVIEW/ta-p/3503281 (Closes #3).

### v1.7.0
- As per RFC 2616 (referred in RFC 6455) headers should be matched in a case insensitive manner. This is was not done and caused a problem with certain WebSocket servers. We have implemented a case insensitive header matching (Fixes #2).
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