How to add a set of static files
================================

The reverse proxy service by default routes all incoming requests to the
[configured host](ProxyAgent/appsettings.ini).

It's also possible to host some **static** files locally, e.g. HTML pages,
images, CSS stylesheets, which will be served directly, without any request
to the backend.

Static files are stored in the [wwwroot](ProxyAgent/wwwroot) folder. The
folder already contains some examples:

* [proxy-status.html](ProxyAgent/wwwroot/proxy-status.html)
* [robots.txt](ProxyAgent/wwwroot/robots.txt)
* [favicon.ico](ProxyAgent/wwwroot/favicon.ico)

When requesting `/` the service will try to locate and serve one of
`default.htm`, `default.html`, `index.htm` or `index.html`.

Adding a full web site to the reverse proxy
-------------------------------------------

The following instructions show how to add a full static web site to the
reverse proxy.

As an example target, we will use Azure IoT Remote Monitoring web user
interface, a Node.js application that needs to be compiled in order
to generate the static files optimized for the web.

1. Clone the target project in a temporary folder:
   ```
   mkdir temp
   cd temp
   git clone git@github.com:Azure/pcs-remote-monitoring-webui.git 
   ```
2. Remove the hardcoded hostname: (TODO: remove this step)
   ```
   cd pcs-remote-monitoring-webui
   echo "REACT_APP_BASE_SERVICE_URL=" > .env
   ```
3. Build the Node.js code to get an optimized production build:
   ```
   cd pcs-remote-monitoring-webui
   npm install
   npm run build
   ```
4. Copy the optimized site files into `wwwroot`:
   ```
   cd build
   rm -fR {REVERSE PROXY FOLDER}/ProxyAgent/wwwroot/*
   mv * {REVERSE PROXY FOLDER}/ProxyAgent/wwwroot/
   ```
5. To test the web site:
   ```
   cd {REVERSE PROXY FOLDER}
   scripts/run
   ```
   then point your browser to URL shown (e.g. http://localhost:9000)
