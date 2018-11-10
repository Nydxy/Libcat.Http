using System;
using System.Net;

namespace Libcat.Http
{
    /// <summary>
    /// Http response/result
    /// 请求结果均包含在此类中
    /// </summary>
    [Serializable]
    public class HttpResult
    {
        /// <summary>
        ///ResponseUrl
        /// </summary>
        public string ResponseUrl { get; internal set; }

        /// <summary>
        /// 302 Redirect url. or Header[location]
        /// </summary>
        public string RedirectUrl { get; internal set; }

        /// <summary>
        /// All updated cookies
        /// </summary>
        public CookieContainer CookieContainer { get; internal set; }

        /// <summary>
        /// Response content. html/text/other data
        /// </summary>
        public string Html { get; internal set; } = string.Empty;

        /// <summary>
        /// Response byte array. Only when ResultType=ResultType.Byte
        /// </summary>
        public byte[] ResultByte { get; internal set; }

        /// <summary>
        /// Header of response
        /// </summary>
        public WebHeaderCollection Header { get; internal set; }

        /// <summary>
        /// Http status code (enum)
        /// </summary>
        public HttpStatusCode StatusCode { get; internal set; }

        /// <summary>
        /// Http status code (number)
        /// </summary>
        public int StatusCodeNum => (int)StatusCode;

        /// <summary>
        /// Description of http status
        /// </summary>
        public string StatusDescription { get; internal set; }
    }
}
