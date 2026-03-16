# Gopher

## Origins
The Gopher system was released in mid-1991 by Mark P. McCahill, Farhad Anklesaria, Paul Lindner, Daniel Torrey, 
and Bob Alberti of the University of Minnesota in the United States.  Its central goals were, as stated in RFC 1436:

A file-like hierarchical arrangement that would be familiar to users.
A simple syntax.
A system that can be created quickly and inexpensively.
Extensibility of the file system metaphor; allowing addition of searches for example.


Gopher combines document hierarchies with collections of services, including WAIS, the Archie 
and Veronica search engines, and gateways to other information systems such as File Transfer Protocol (FTP) and Usenet.

In February 1993, the University of Minnesota announced that it would charge licensing fees for the use of its 
implementation of the Gopher server. Users became concerned that fees might also be charged for independent implementations.
This in part was responsible for its decline. 


## Overview

With the HTTP protocol a simple request might look like `GET /customer/123 HTTP/1.1`.   
Which means GET the resource `/customer/123` using HTTP 1.1

The equivalent request in Gopher would be just `/customer/123` 

Gopher then returns a series of text lines.  The first character of the line indicates it's type.  
For example `iWelcome To Gopher` is an information line containing the text Welcome To Gopher.


## Getting Started

As shown above,  the HTTP and Gopher protocols are different so it' not possible to use your 
language's existing HttpClients to connect to a Gopher server.

Instead, you will need to connect at a socket level.   Unlike HTTP which typically uses port 80 (or 443), Gopher normally uses port 70.

Below shows some example code written in nodejs.


```js
let s = require('net').Socket();

const host =  'gopher.floodgap.com';
const port = 70;
s.connect(port, host);

const selector = ''
s.write(selector);

s.on('data', function(d){
    console.log(d.toString());
});

s.end();
```

Here we connect to  'gopher.floodgap.com' on port 70 and then send an empty selector.   This means return the root page.




