using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Libcat.Http
{
    public class HttpClient
    {
        //private static readonly System.Net.Http.HttpClient _Client = new System.Net.Http.HttpClient();
        private System.Net.Http.HttpClient _Client;
        public CookieContainer _CookieContainer { get; set; }

        #region Constructor
        public HttpClient()
        {
#if NETCOREAPP2_1
            var handler = new SocketsHttpHandler();
#else
            var handler = new HttpClientHandler();
#endif
            _CookieContainer = new CookieContainer();
            handler.CookieContainer = _CookieContainer;
            _Client = new System.Net.Http.HttpClient(handler);

            _Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134");
            _Client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            _Client.DefaultRequestHeaders.Add("Accept", "*/*");
            _Client.Timeout = TimeSpan.FromMilliseconds(15000);
        }

        ~HttpClient()
        {
            _Client.Dispose();
        }
        #endregion


        public async Task<HttpResponseMessage> GetAsync(string uri) => await _Client.GetAsync(uri);
        public async Task<HttpResponseMessage> GetAsync(string uri, bool isAjax, int timeout)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.Add("x-requested-with", "XMLHttpRequest");
            return await _Client.SendAsync(req);
        }
        public async Task<string> GetStringAsync(string uri) => await _Client.GetStringAsync(uri);
        public async Task<byte[]> GetByteArrayAsync(string uri) => await _Client.GetByteArrayAsync(uri);
        public async Task<HttpResponseMessage> PostAsync(string uri, string postdata) => await _Client.PostAsync(uri, new StringContent(postdata));
        public async Task<HttpResponseMessage> PostAsync(string uri, byte[] postdata) => await _Client.PostAsync(uri, new ByteArrayContent(postdata));

        public HttpResponseMessage Get(string uri) => _Client.GetAsync(uri).Result;
        public string GetString(string uri) => _Client.GetStringAsync(uri).Result;
        public byte[] GetByteArray(string uri) => _Client.GetByteArrayAsync(uri).Result;
        public HttpResponseMessage Post(string uri, string postdata) => _Client.PostAsync(uri, new StringContent(postdata)).Result;
        public HttpResponseMessage Post(string uri, byte[] postdata) => _Client.PostAsync(uri, new ByteArrayContent(postdata)).Result;


        public string GetCookie(string name) => GetCookie(name, _CookieContainer);
        private static string GetCookie(string name, CookieContainer container)
        {
            var lstCookies = GetAllCookies(container);
            return lstCookies.Find(c => c.Name == name)?.Value;
        }
        private static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }

        private async Task<HttpResult> FromHttpResponseMessage(HttpResponseMessage response, ResultType resultType)
        {
            var result = new HttpResult();
            switch (resultType)
            {
                case ResultType.String:
                    result.Html = await response.Content.ReadAsStringAsync();
                    break;
                case ResultType.Byte:
                    result.ResultByte = await response.Content.ReadAsByteArrayAsync();
                    break;
                case ResultType.So:
                    break;
            }
            result.StatusCode = response.StatusCode;
            result.StatusDescription = response.ReasonPhrase;
           //result.Header=response.Headers.
           //foreach (var a in response.Headers)
           // {
           //     result.Header.Add();
           // }
            result.RedirectUrl = response.Headers.Location.ToString();
            result.ResponseUrl = response.RequestMessage.RequestUri.ToString();
            result.CookieContainer = _CookieContainer;
            return result;
        }
    }
}
