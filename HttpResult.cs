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
        public string ResponseUrl { get; set; }

        /// <summary>
        /// 302 Redirect url. or Header[location]
        /// </summary>
        public string RedirectUrl
        {
            get
            {
                if (Header != null && Header.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Header["location"]))
                    {
                        return Header["location"].ToString();
                    }
                    return string.Empty;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Cookies of response, not all cookies. All updated cookies are in your cookie container.
        /// </summary>
        public CookieCollection ResponseCookies { get; set; }

        /// <summary>
        /// All updated cookies
        /// </summary>
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// Response content. html/text/other data
        /// </summary>
        public string Html { get; set; } = string.Empty;

        /// <summary>
        /// Response byte array. Only when ResultType=ResultType.Byte
        /// </summary>
        public byte[] ResultByte { get; set; }

        /// <summary>
        /// Header of response
        /// </summary>
        public WebHeaderCollection Header { get; set; }

        /// <summary>
        /// Http status code (enum)
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Http status code (number)
        /// </summary>
        public int StatusCodeNum => (int)StatusCode;

        /// <summary>
        /// Description of http status
        /// </summary>
        public string StatusDescription { get; set; }
    }
}
