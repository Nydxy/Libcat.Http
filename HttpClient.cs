using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace Libcat.Http
{
    public class HttpClient
    {
        private static readonly System.Net.Http.HttpClient _Client = new System.Net.Http.HttpClient();
        private System.Net.Http.HttpClient Client;

        public HttpClient()
        {
            Client = new System.Net.Http.HttpClient();
        }
        ~HttpClient()
        {
            Client.Dispose();
        }

        public static async Task<HttpResponseMessage> GetAsync(string uri) => await _Client.GetAsync(uri);
        public static async Task<HttpResponseMessage> GetAsync(string uri,bool isAjax, int timeout)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.Add("x-requested-with", "XMLHttpRequest");
            return await _Client.SendAsync(req);
        }
        public static async Task<HttpResponseMessage> PostAsync(string uri, string postdata) => await _Client.PostAsync(uri, new StringContent(postdata));
        public static async Task<HttpResponseMessage> PostAsync(string uri, byte[] postdata) => await _Client.PostAsync(uri, new ByteArrayContent(postdata));

    }
}
