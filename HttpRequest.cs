/* Libcat.Http
 * 2018-10-1  v1.0.0
 * 
 * 
 */


using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Libcat.Http
{
    /// <summary>
    /// Http Request Class
    /// </summary>
    [Serializable]
    public class HttpRequest
    {

        #region 请求参数 Request Parametres

        /// <summary>
        /// 请求URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求方式默认为GET方式
        /// </summary>
        public string Method { get; set; } = "GET";

        /// <summary>
        ///     默认请求超时时间
        /// </summary>
        public int Timeout { get; set; } = 15000;

        /// <summary>
        ///  Accept
        /// </summary>
        public string Accept { get; set; } = "text/html,application/xhtml+xml,application/xml;q=0.9,image/jpeg,*/*;";

        /// <summary>
        ///  ContentType
        /// </summary>
        public string ContentType { get; set; } = "application/x-www-form-urlencoded";

        /// <summary>
        ///  UserAgent
        /// </summary>
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.9 Safari/537.36";

        /// <summary>
        ///  Referer
        /// </summary>
        public string Referer { get; set; } = string.Empty;

        /// <summary>
        ///     头数据
        /// </summary>
        public WebHeaderCollection Header { get; set; } = new WebHeaderCollection();

        /// <summary>
        ///     字符串头
        /// </summary>
        public string HeaderStr { get; set; }

        /// <summary>
        ///  CookieContainer
        /// </summary>
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        ///  string方式的cookie，会覆盖CookieContainer
        /// </summary>
        public string CookieString { get; set; }

        /// <summary>
        ///     是否增加异步请求头
        ///     对应协议 : x-requested-with: XMLHttpRequest
        /// </summary>
        public bool IsAjax { get; set; } = false;

        /// <summary>
        ///     支持跳转页面，查询结果将是跳转后的页面
        /// </summary>
        public bool AllowAutoRedirect { get; set; } = true;

        /// <summary>
        ///     如果返回内容为 : 尝试自动重定向的次数太多 请设置本属性为true
        ///     同时应注意设置超时时间(默认15s)
        /// </summary>
        public bool AutoRedirectMax { get; set; }

        public bool KeepAlive { get; set; } = true;

        /// <summary>
        ///     当该属性设置为 true 时，使用 POST 方法的客户端请求应该从服务器收到 100-Continue 响应，以指示客户端应该发送要发送的数据。此机制使客户端能够在服务器根据请求报头打算拒绝请求时，避免在网络上发送大量的数据
        ///     默认False
        /// </summary>
        public bool Expect100Continue { get; set; }

        /// <summary>
        ///     最大连接数
        /// </summary>
        public int Connectionlimit { get; set; } = 1024;

        /// <summary>
        ///  Post data encoding
        /// </summary>
        public Encoding PostEncoding { get; set; } = Encoding.Default;

        /// <summary>
        ///     Post的数据类型
        /// </summary>
        public PostDataType PostDataType { get; set; } = PostDataType.String;

        /// <summary>
        ///     Post请求时要发送的字符串Post数据
        /// </summary>
        public string Postdata { get; set; }

        /// <summary>
        ///     Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte { get; set; }

        /// <summary>
        ///     默认写入Post数据超时间
        /// </summary>
        public int ReadWriteTimeout { get; set; } = 15000;

        /// <summary>
        ///     设置返回类型String和Byte
        /// </summary>
        public ResultType ResultType { get; set; } = ResultType.String;

        /// <summary>
        ///     返回数据编码默认为NUll,可以自动识别
        /// </summary>
        public Encoding ResponseEncoding { get; set; }

        /// <summary>
        ///     文件上传路径
        /// </summary>
        public string UpLoadPath { get; set; }

        #endregion

        #region 主要函数 Main methods
        public HttpRequest() { }

        public HttpRequest(string url) => Url = url;

        private HttpWebRequest CreateHttpRequest()
        {
            //if the url does not contains http/https, add it
            if (!Url.Contains("http://")&&!Url.Contains("https://"))
            {
                Url = "http://" + Url;
            }

            var _request = WebRequest.CreateHttp(Url);

            if (Header != null && Header.Count > 0)
            {
                _request.Headers = Header;
            }

            if (AutoRedirectMax)
            {
                _request.MaximumAutomaticRedirections = 9999;
            }

            if (IsAjax)
            {
                _request.Headers.Add("x-requested-with: XMLHttpRequest");
            }

            //通用属性设置 
            _request.Method = Method;
            _request.Timeout = Timeout;
            _request.ReadWriteTimeout = ReadWriteTimeout;
            _request.Accept = Accept;
            _request.ContentType = ContentType;
            _request.UserAgent = UserAgent;
            _request.Referer = Referer;
            _request.AllowAutoRedirect = AllowAutoRedirect;
            _request.KeepAlive = KeepAlive;

            //设置Cookie    Set cookies
            if (CookieContainer != null)
            {
                //CookieContainer方式  if cookies are in a CookieContainer
                _request.CookieContainer = CookieContainer;
            }
            else if (!string.IsNullOrEmpty(CookieString))
            {
                //字符串方式   if cookies are in a string
                _request.Headers[HttpRequestHeader.Cookie] = CookieString;
            }


            //设置最大连接   Set max connections
            if (Connectionlimit > 0)
            {
                _request.ServicePoint.ConnectionLimit = Connectionlimit;
                System.Net.ServicePointManager.DefaultConnectionLimit = 1024;
            }
            //设置 post数据在大于1024时是否分包
            _request.ServicePoint.Expect100Continue = Expect100Continue;

            //验证在得到结果时是否有传入数据
            if (_request.Method.Trim().ToLower() == "post")
            {
                //写入Byte类型
                if (PostDataType == PostDataType.Byte)
                {
                    //验证在得到结果时是否有传入数据
                    if (PostdataByte != null && PostdataByte.Length > 0)
                    {
                        _request.ContentLength = PostdataByte.Length;
                        _request.GetRequestStream().Write(PostdataByte, 0, PostdataByte.Length);
                    }
                }//写入文件
                else if (PostDataType == PostDataType.FilePath)
                {
                    using (StreamReader r = new StreamReader(Postdata, PostEncoding))
                    {
                        byte[] buffer = PostEncoding.GetBytes(r.ReadToEnd());
                        _request.ContentLength = buffer.Length;
                        _request.GetRequestStream().Write(buffer, 0, buffer.Length);
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(Postdata))
                    {
                        //写入字符串数据.如果希望修改提交时的编码.请修改objHttpRequests.PostEncoding
                        byte[] buffer = PostEncoding.GetBytes(Postdata);
                        _request.ContentLength = buffer.Length;
                        _request.GetRequestStream().Write(buffer, 0, buffer.Length);
                    }
                }
            }
            return _request;
        }

        private HttpResult ParseHttpResult(HttpWebResponse response)
        {
            var result = new HttpResult();

            #region Set result headers 设置相应头数据
            result.StatusCode = response.StatusCode;
            result.StatusDescription = response.StatusDescription;
            result.Header = response.Headers;
            result.ResponseCookies = response.Cookies;
            result.ResponseUrl = response.ResponseUri != null ? response.ResponseUri.ToString() : string.Empty;
            result.CookieContainer = CookieContainer;

            //If only request header
            if (ResultType == ResultType.So)
            {
                return result;
            }
            #endregion

            #region Get response stream 应答结果转换为内存数据流

            //get response stream (bytes)  获得应答数据流
            Stream resStream = null;

            //gzip decompress  (if compressed)  处理gzip压缩
            if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
            {
                resStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
            }
            //deflate decompress  (if compressed)  处理deflate压缩
            else if (response.ContentEncoding != null && response.ContentEncoding.Equals("deflate", StringComparison.InvariantCultureIgnoreCase))
            {
                resStream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress);
            }
            // if not compressed  没有压缩
            else
            {
                resStream = response.GetResponseStream();
            }
            #endregion

            #region Return response byte array or text/html
            //if user request bytes, return it   如果用户要求bytes数据，不解析为文本
            if (ResultType == ResultType.Byte)
            {
                result.ResultByte = new byte[resStream.Length];
                resStream.Read(result.ResultByte, 0, (int)resStream.Length);
                resStream.Close();
                resStream.Dispose();
                return result;
            }


            //get response encoding
            if (ResponseEncoding == null)
            {
                //如果未设置应答数据编码
                if (string.IsNullOrEmpty(response.CharacterSet))
                {
                    //如果http response也没有指定编码
                    if (string.IsNullOrEmpty(response.ContentEncoding))
                        ResponseEncoding = Encoding.UTF8;  //使用默认编码
                    else
                        ResponseEncoding = Encoding.GetEncoding(response.ContentEncoding);
                }
                else
                {
                    ResponseEncoding = Encoding.GetEncoding(response.CharacterSet);
                }
            }

            //get response text/html
            try
            {
                var sr = new StreamReader(resStream, ResponseEncoding);
                result.Html = sr.ReadToEnd();
            }
            catch(Exception e)
            {
                result.Html = "Failed to decode data. Status num is " + result.StatusCodeNum.ToString() + "  " +e.Message;
            }
            resStream.Close();
            resStream.Dispose();
            #endregion

            return result;
        }

        /// <summary>
        /// Do request
        /// </summary>
        /// <returns></returns>
        public HttpResult GetResponse()
        {
            HttpResult result = null;
            HttpWebResponse response = null;
            try
            {
                //设置请求参数
                var request = CreateHttpRequest();

                //获得应答结果
                response = (HttpWebResponse)request.GetResponse();

                //解析应答数据
                result = ParseHttpResult(response);

                request.Abort();
                response.Close();
                response.Dispose();

                return result;
            }
            catch (WebException ex)
            {
                result = new HttpResult();

                //获取异常数据与结果
                result.Html = ex.Message;
                response = (HttpWebResponse)ex.Response;

                if (response != null)
                {
                    result.StatusCode = response.StatusCode;
                    result.StatusDescription = ex.Message;
                    try
                    {
                        return ParseHttpResult(response);
                    }
                    catch
                    {
                        return result;
                    }
                }
                else
                {
                    //Time out!
                    result.StatusCode = HttpStatusCode.NotFound;
                    result.StatusDescription = ex.Message;
                }
            }
            return result;
        }

        #endregion

        #region 快速请求函数  Methods for easily get/post
        public static HttpResult Get(string url) => new HttpRequest(url).GetResponse();
        public static HttpResult Get(string url, CookieContainer cookies)
        {
            var req = new HttpRequest(url)
            {
                CookieContainer = cookies
            };
            return req.GetResponse();
        }
        public static HttpResult Get(string url, CookieContainer cookies, int timeout=15000)
        {
            var req = new HttpRequest(url)
            {
                CookieContainer = cookies,
                Timeout = timeout
            };
            return req.GetResponse();
        }
        public static HttpResult Get(string url, CookieContainer cookies, bool isAjax, int timeout=15000)
        {
            var req = new HttpRequest(url)
            {
                CookieContainer = cookies,
                IsAjax = isAjax,
                Timeout = timeout
            };
            return req.GetResponse();
        }
        public static HttpResult Get(string url, string referer, int timeout = 15000)
        {
            var req = new HttpRequest(url)
            {
                Referer=referer,
                Timeout = timeout
            };
            return req.GetResponse();
        }
        public static HttpResult Get(string url, CookieContainer cookies, string referer, bool isAjax=false, int timeout = 15000)
        {
            var req = new HttpRequest(url)
            {
                CookieContainer = cookies,
                Referer = referer,
                IsAjax = isAjax,
                Timeout = timeout
            };
            return req.GetResponse();
        }

        public static HttpResult Post(string url, string postdata, int timeout=15000)
        {
            var req = new HttpRequest(url)
            {
                Method = "POST",
                Postdata = postdata,
                Timeout = timeout
            };
            return req.GetResponse();
        }
        public static HttpResult Post(string url, string postdata, CookieContainer cookies, int timeout = 15000)
        {
            var req = new HttpRequest(url)
            {
                Method = "POST",
                Postdata = postdata,
                CookieContainer = cookies,
                Timeout = timeout
            };
            return req.GetResponse();
        }
        #endregion
    }

    #region Enums
    /// <summary>
    ///     Post的数据格式默认为string
    /// </summary>
    public enum PostDataType
	{
		/// <summary>
		///     字符串
		/// </summary>
		String, //字符串

		/// <summary>
		///     字节流
		/// </summary>
		Byte, //字符串和字节流

		/// <summary>
		///     文件路径
		/// </summary>
		FilePath //表示传入的是文件
	}

	/// <summary>
	///     返回类型
	/// </summary>
	public enum ResultType
	{
		/// <summary>
		///     表示只返回字符串
		/// </summary>
		String,

		/// <summary>
		///     表示只返回字节流
		/// </summary>
		Byte,

		/// <summary>
		///     急速请求,仅返回数据头
		/// </summary>
		So
	}
    #endregion
}