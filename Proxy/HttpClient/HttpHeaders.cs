// Copyright (c) Microsoft. All rights reserved.

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.HttpClient
{
    public class HttpHeaders : IEnumerable<KeyValuePair<string, List<string>>>
    {
        private readonly Dictionary<string, List<string>> data;

        public HttpHeaders()
        {
            this.data = new Dictionary<string, List<string>>();
        }

        public void Add(string name, string value)
        {
            if (!this.data.ContainsKey(name))
            {
                this.data[name] = new List<string>();
            }

            this.data[name].Add(value);
        }

        public void Add(string name, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                this.Add(name, value);
            }
        }

        public bool Contains(string name)
        {
            return this.data.ContainsKey(name);
        }

        public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, List<string>>>) this.data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}