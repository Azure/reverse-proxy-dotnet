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
private services hosted in Azure VMs, Cloud Apps, etc. When deploying
via Azure Web Apps, remember to set the remote endpoint setting, under
"Application settings"

However, you can easily extend the code to use a custom certificate,
see the
[documentation](https://azure.github.io/reverse-proxy-dotnet/ssl-setup.html)
for more information.

## Configuration

The service has two mandatory values for the remote endpoint, and some optional
settings with a default value, that can be overridden if required:

Mandatory configuration settings:

* `endpoint`: this is the full URL of the remote endpoint where all requests
  are routed to.
  Environment variable: `REMOTE_ENDPOINT`.
* `ssl_cert_thumbprint`: the thumbprint of the SSL certificate used by the remote
  endpoint. This allows to use self-signed certificates, with fine-grained control
  on the exact certificate used by the remote server.
  Environment variable: `REMOTE_ENDPOINT_SSL_THUMBPRINT`.
  
Optional settings:

* `redirectHttpToHttps`: optional value to decide whether HTTP requests should
  be redirected to HTTPS (default: true).
  Environment variable: `REDIRECT_HTTP_TO_HTTPS`.
* `strictTransportSecurityEnabled`: whether to enable HSTS (default: true).
  Environment variable: `HSTS_ENABLED`.
* `strictTransportSecurityPeriod`: how long to persist HSTS rules in the
  clients, in seconds (default: 30 days).
  Environment variable: `HSTS_PERIOD`.
* `statusEndpointEnabled`: whether to expose extra information in the /status
  endpoint (default: false).
  Environment variable: `STATUS_ENDPOINT_ENABLED`.
* `maxPayloadSize`: the maximum size of requests' payload (default: 100Kb).
  Environment variable: `MAX_PAYLOAD_SIZE`.
* `loglevel`: application logging level (default: Warn).
  Environment variable: `LOG_LEVEL`.

[build-badge]: https://img.shields.io/travis/Azure/reverse-proxy-dotnet.svg
[build-url]: https://travis-ci.org/Azure/reverse-proxy-dotnet
[issues-badge]: https://img.shields.io/github/issues/azure/reverse-proxy-dotnet.svg
[issues-url]: https://github.com/azure/reverse-proxy-dotnet/issues
[gitter-badge]: https://img.shields.io/gitter/room/azure/iot-solutions.js.svg
[gitter-url]: https://gitter.im/azure/iot-solutions
