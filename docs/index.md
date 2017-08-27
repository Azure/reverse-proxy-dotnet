Reverse Proxy
=============

This service allows to expose private resources in a managed way, and
through a unique endpoint. Private resources are returned to the client
as if they originated from the proxy server itself. The proxy is used
to secure private resources with a valid SSL certificate, and can be
extended with other features like load balance, caching, A/B testing,
version management, etc.

The service allows also to host a set of static files, which are served
from the local copy, without routing requests to the remote endpoint.
