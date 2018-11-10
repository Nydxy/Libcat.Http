using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace Libcat.Http
{
    public class HttpClient
    {
        private System.Net.Http.HttpClient _Client;
        public CookieContainer _CookieContainer { get; set; }

        #region Constructor
        public HttpClient()
        {

#if (NETCOREAPP2_1 || NETSTANDARD2_0)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  //Support more encodings (such as GB2312)
#endif

#if NETCOREAPP2_1
            var handler = new SocketsHttpHandler();
#else
            var handler = new HttpClientHandler();
#endif

            _CookieContainer = handler.CookieContainer;
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

#region Raw async Http method
        public async Task<HttpResponseMessage> GetAsync(string uri) => await _Client.GetAsync(uri);
        public async Task<string> GetStringAsync(string uri) => await _Client.GetStringAsync(uri);
        public async Task<byte[]> GetByteArrayAsync(string uri) => await _Client.GetByteArrayAsync(uri);
        public async Task<HttpResponseMessage> PostAsync(string uri, HttpContent content) => await _Client.PostAsync(uri, content);
        public async Task<HttpResponseMessage> PutAsync(string uri, HttpContent content) => await _Client.PutAsync(uri, content);
        public async Task<HttpResponseMessage> DeleteAsync(string uri) => await _Client.DeleteAsync(uri);
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => await _Client.SendAsync(request);
#endregion

        /// <summary>
        /// GET方法
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="resultType">返回类型</param>
        /// <returns></returns>
        public HttpResult Get(string uri) => FromHttpResponseMessage(GetAsync(EnsureValidHttpUri(uri)).Result);
        public HttpResult Get(string uri, bool ajax, string referer)
        {
            uri = EnsureValidHttpUri(uri);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ajax) request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            if (referer != null) request.Headers.Referrer = new Uri(referer);
            var response = SendAsync(request).Result;
            var result = FromHttpResponseMessage(response);
            return result;
        }
        public string GetString(string uri) => GetStringAsync(EnsureValidHttpUri(uri)).Result;
        public byte[] GetByteArray(string uri) => GetByteArrayAsync(EnsureValidHttpUri(uri)).Result;
        public HttpResult Post(string uri, string postdata) => FromHttpResponseMessage(PostAsync(EnsureValidHttpUri(uri), new StringContent(postdata)).Result);
        public HttpResult Post(string uri, byte[] postdata) => FromHttpResponseMessage(PostAsync(EnsureValidHttpUri(uri), new ByteArrayContent(postdata)).Result);
        public HttpResult Delete(string uri) => FromHttpResponseMessage(DeleteAsync(EnsureValidHttpUri(uri)).Result);
        public HttpResult Put(string uri, HttpContent content) => FromHttpResponseMessage(PutAsync(EnsureValidHttpUri(uri), content).Result);
        private HttpResult Send(string uri, HttpMethod method, ResultType resultType)
        {
            uri = EnsureValidHttpUri(uri);
            var request = new HttpRequestMessage(method, uri);
            var response = SendAsync(request).Result;
            var result = FromHttpResponseMessage(response, resultType);
            return result;
        }

        /// <summary>
        /// HttpResponseMessage to HttpResult
        /// </summary>
        /// <param name="response">HttpResponseMessage object</param>
        /// <param name="resultType">resultType enum</param>
        /// <returns></returns>
        private HttpResult FromHttpResponseMessage(HttpResponseMessage response, ResultType resultType=ResultType.String)
        {
            var result = new HttpResult();
            switch (resultType)
            {
                case ResultType.String:
                    result.Html =  response.Content.ReadAsStringAsync().Result;
                    break;
                case ResultType.Byte:
                    result.ResultByte =  response.Content.ReadAsByteArrayAsync().Result;
                    break;
                case ResultType.So:
                    break;
            }
            result.StatusCode = response.StatusCode;
            result.StatusDescription = response.ReasonPhrase;
            result.Header = new WebHeaderCollection();
            foreach (var header in response.Headers)
            {
                var value = string.Join(";", header.Value);
                result.Header.Add(header.Key, value);
            }
            result.RedirectUrl = response.Headers.Location?.ToString();
            result.ResponseUrl = response.RequestMessage.RequestUri.ToString();
            result.CookieContainer = _CookieContainer;
            return result;
        }

        /// <summary>
        /// Automaticly add http:// to uri (if not exist)
        /// </summary>
        /// <param name="uri">origin uri</param>
        /// <returns></returns>
        private string EnsureValidHttpUri(string uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");
            if (!(uri.Contains("http://") || uri.Contains("https://")))
            {
                uri = "http://" + uri;
            }
            return uri;
        }

#region Get cookie methods
        /// <summary>
        /// Get cookie value with specific name in despite of domain. If couldn't find, return null. 
        /// </summary>
        /// <param name="name">name of cookie</param>
        /// <returns></returns>
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
#endregion
    }

}
