// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Http;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy
{
    public class ApplicationRequestRouting
    {
        // See https://azure.microsoft.com/blog/disabling-arrs-instance-affinity-in-windows-azure-web-sites
        private const string HEADER_NAME = "Arr-Disable-Session-Affinity";
        private const string HEADER_VALUE = "True";

        // Disable IIS Application Request Routing (remove cookie)
        public static void DisableInstanceAffinity(HttpResponse responseOut)
        {
            if (!responseOut.Headers.ContainsKey(HEADER_NAME))
            {
                responseOut.Headers.Add(HEADER_NAME, HEADER_VALUE);
            }
        }
    }
}
