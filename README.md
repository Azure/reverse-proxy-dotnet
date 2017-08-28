[![Build][build-badge]][build-url]
[![Issues][issues-badge]][issues-url]
[![Gitter][gitter-badge]][gitter-url]

Reverse Proxy
=============

This service allows to expose private resources in a managed way, and
through a unique endpoint. Private resources are returned to the client
as if they originated from the proxy server itself.

The proxy is used to secure private resources with a valid SSL certificate,
and can be extended with other features like load balancing, caching,
A/B testing, version management, etc.

The service allows also to host a set of static files, which are served
from the local copy, without routing requests to the remote endpoint.

The project documentation is available
[here](https://azure.github.io/reverse-proxy-dotnet).

How to use the service
======================

## Build and Run from the command line

The [scripts](scripts) folder contains scripts for some frequent tasks:

* `build`: compile all the projects and run the tests.
* `run`: compile the projects and run the service. This will prompt for
  elevated privileges in Windows to run the application.

## Deployment

The service is designed to be deployed as an 
[Azure Web App](https://docs.microsoft.com/en-us/azure/app-service-web/app-service-web-tutorial-custom-SSL),
reusing the SSL encryption provided by the platform, to expose, for example,
private services hosted in Azure VMs, Cloud Apps, etc.

However, you can easily extend the code to use a custom certificate,
see the
[documentation](https://azure.github.io/reverse-proxy-dotnet/ssl-setup.html)
for more information.



[build-badge]: https://img.shields.io/travis/Azure/reverse-proxy-dotnet.svg
[build-url]: https://travis-ci.org/Azure/reverse-proxy-dotnet
[issues-badge]: https://img.shields.io/github/issues/azure/reverse-proxy-dotnet.svg
[issues-url]: https://github.com/azure/reverse-proxy-dotnet/issues
[gitter-badge]: https://img.shields.io/gitter/room/azure/iot-pcs.js.svg
[gitter-url]: https://gitter.im/azure/iot-pcs
