using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Libcat.Http
{
    /// <summary>
    /// 快捷工具
    /// </summary>
    public class Tools
    {
        #region 时间操作与转换
        /// <summary>
        /// 时间戳转DateTime[Local]
        /// </summary>
        /// <param name="timeStamp">Unix时间戳(second)</param>
        /// <returns>DateTime[Local]</returns>
        public static DateTime GetDateTime(int timeStamp) => new DateTime(1970, 1, 1).ToLocalTime().AddSeconds(timeStamp);

        /// <summary>
        /// DateTime转10位时间戳
        /// </summary>
        /// <param name="time">DateTime[Local]</param>
        /// <returns></returns>
        public static int GetTimeStamp(DateTime time) => (int)(time - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;

        /// <summary>
        /// 获取13位时间戳(millisecond)
        /// </summary>
        /// <param name="nAddSecond"></param>
        /// <returns></returns>
        public static long GetTimeStamp13() => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

        /// <summary>  
        /// 获取10位时间戳(second)
        /// </summary>  
        /// <returns></returns>  
        public static int GetTimeStamp() => (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;

        #endregion

        #region 字符串处理方法
        /// <summary>
        /// 取文本中间
        /// </summary>
        /// <param name="allStr">原字符</param>
        /// <param name="firstStr">前面的文本</param>
        /// <param name="lastStr">后面的文本</param>
        /// <returns>返回获取的值</returns>
        public static string GetStringMid(string allStr, string firstStr, string lastStr)
        {
            //取出前面的位置
            int index1 = allStr.IndexOf(firstStr);
            //取出后面的位置
            int index2 = allStr.IndexOf(lastStr, index1 + firstStr.Length);

            if (index1 < 0 || index2 < 0)
            {
                return "";
            }
            //定位到前面的位置
            index1 = index1 + firstStr.Length;
            //判断要取的文本的长度
            index2 = index2 - index1;

            if (index1 < 0 || index2 < 0)
            {
                return "";
            }
            //取出文本
            return allStr.Substring(index1, index2);
        }

        /// <summary>
        /// 取文本中间 正则方式
        /// </summary>
        /// <param name="source">原始字符串</param>
        /// <param name="left">开始字符串</param>
        /// <param name="right">结束字符串</param>
        /// <returns>返回获取结果</returns>
        public static string GetStringMidByRegex(string source, string left, string right)
        {
            string rx = left + "([\\s\\S]*?)" + right;
            Match match = Regex.Match(source, rx);
            if (match != null && match.Groups.Count > 0)
            {
                return match.Groups[1].Value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Url编码,encoding默认为utf8编码
        /// </summary>
        /// <param name="str">需要编码的字符串</param>
        /// <param name="encoding">指定编码类型</param>
        /// <returns>编码后的字符串</returns>
        public static string UrlEncoding(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                return System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            }
            else
            {
                return System.Web.HttpUtility.UrlEncode(str, encoding);
            }
        }

        /// <summary>
        /// Url解码,encoding默认为utf8编码
        /// </summary>
        /// <param name="str">需要解码的字符串</param>
        /// <param name="encoding">指定解码类型</param>
        /// <returns>解码后的字符串</returns>
        public static string UrlDecoding(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                return System.Web.HttpUtility.UrlDecode(str, Encoding.UTF8);
            }
            else
            {
                return System.Web.HttpUtility.UrlDecode(str, encoding);
            }
        }

        /// <summary>
        /// Html解码
        /// </summary>
        /// <param name="str">需要解码的字符</param>
        /// <returns></returns>
        public static string HtmlDecode(string str)
        {
            string[] strsx = str.Split('△');
            if (strsx.Length > 1)
            {
                return FromUnicodeString(strsx[0], strsx[1].Trim());//Decode2Html(strsx[0], strsx[1].Trim());
            }
            else
            {
                return Decode2Html(str);
            }
        }

        /// <summary>
        /// 解析任意符号开头的编码后续数据符合Hex编码
        /// </summary>
        /// <param name="param"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        private static string Decode2Html(string param, string sp = "&#")
        {

            string[] paramstr = param.Replace(sp, sp + " ").Replace(sp, "").Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string str = string.Empty;
            foreach (string item in paramstr)
            {
                try
                {

                    str += (char)int.Parse(item, System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    str += item;
                    continue;
                }
            }
            // 如果失败请尝试这种办法 可解决变异HTML 开头非&# :->  StringWriter myWriter = new StringWriter(); System.Web.HttpUtility.HtmlDecode(param,myWriter); return myWriter.ToString(); 
            return str;
        }

        /// <summary>
        /// Html编码 
        /// </summary>
        /// <param name="param">需要编码的字符</param>
        /// <returns>返回编码后数据</returns>
        public static string HtmlEncode(string param)
        {
            string str = string.Empty;
            foreach (char item in param.ToCharArray())
            {
                try
                {
                    str += "&#" + Convert.ToInt32(item).ToString("x4") + " ";
                }
                catch
                {
                    str += "ToHtml Error";
                }
            }

            return str;
        }

        /// <summary>
        /// 取文本右边 
        /// 默认取出右边所有文本,如果需要取固定长度请设置 length参数
        /// 异常则返回空字符串
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="right">需要确认位置的字符串</param>
        /// <param name="length">默认0,如果设置按照设置的值取出数据</param>
        /// <returns>返回结果</returns>
        public static string Right(string str, string right, int length = 0)
        {
            int pos = str.IndexOf(right, StringComparison.Ordinal);
            if (pos < 0) return "";
            int len = str.Length;
            if (len - pos - right.Length <= 0) return "";
            string result = "";
            if (length == 0)
            {
                result = str.Substring(pos + right.Length, len - (pos + right.Length));
            }
            else
            {
                result = str.Substring(pos + right.Length, length);
            }
            return result;
        }

        /// <summary>
        ///  取文本左边
        ///  默认取出左边所有文本,如果需要取固定长度请设置 length参数
        /// 异常则返回空字符串
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="left">需要确认位置的字符串</param>
        /// <param name="length">默认0,如果设置按照设置的值取出数据</param>
        /// <returns>返回结果</returns>
        public static string Left(string str, string left, int length = 0)
        {
            var pos = str.IndexOf(left, StringComparison.Ordinal);
            if (pos < 0) return "";
            string result = "";
            if (length == 0)
            {
                result = str.Substring(0, pos);
            }
            else
            {
                result = str.Substring(length, pos);
            }
            return result;
        }

        /// <summary>
        /// Unicode字符转汉字 允许自定义分隔字符
        /// </summary>
        /// <param name="str">需要转换的字符串</param>
        /// <param name="SplitString">分隔字符</param>
        /// <param name="TrimStr">如果有尾部数据则填写尾部</param>
        /// <returns>处理后结果</returns>
        public static string FromUnicodeString(string str, string SplitString = "u", string TrimStr = ";")
        {
            string regexCode = SplitString == "u" ? "\\\\u(\\w{1,4})" : SplitString + "(\\w{1,4})";
            string reString = str;
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regexCode);
            System.Text.RegularExpressions.MatchCollection mc = reg.Matches(reString);
            for (int i = 0; i < mc.Count; i++)
            {
                try
                {
                    var outs = (char)int.Parse(mc[i].Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
                    if (str.IndexOf(mc[i].Groups[0].Value + TrimStr) > 0)
                    {
                        //如果出现(封号);结尾则连带符号替换
                        str = str.Replace(mc[i].Groups[0].Value + TrimStr, outs.ToString());
                    }
                    else
                    {
                        str = str.Replace(mc[i].Groups[0].Value, outs.ToString());
                    }
                }
                catch
                {
                    continue;
                }
            }
            return str;
        }

        /// <summary>
        /// 汉字转Unicode字符 默认\u1234 
        /// </summary>
        /// <param name="param">需要转换的字符</param>
        /// <param name="SplitString">分隔结果</param>
        /// <returns>转换后结果</returns>
        public static string GetUnicodeString(string param, string SplitString = "u")
        {
            string outStr = "";
            for (int i = 0; i < param.Length; i++)
            {
                try
                {
                    outStr += "\\" + SplitString + ((int)param[i]).ToString("x4");
                }
                catch
                {
                    outStr += param[i];
                    continue;
                }

            }

            return outStr;
        }

        /// <summary>
        /// 将字符串转换为base64格式 默认UTF8编码
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>结果</returns>
        public static string GetString2Base64(string str, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            return Convert.ToBase64String(encoding.GetBytes(str));
        }

        /// <summary>
        /// base64字符串转换为普通格式 默认UTF8编码
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>结果</returns>
        public static string GetStringbyBase64(string str, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            byte[] buffer = Convert.FromBase64String(str);
            return encoding.GetString(buffer);
        }

        /// <summary>
        /// 将byte数组转换为AscII字符
        /// </summary>
        /// <param name="b">需要操作的数组</param>
        /// <returns>结果</returns>
        public static string BytesToAscii(byte[] b)
        {
            string str = "";
            for (int i = 7; i < 19; i++)
            {
                str += (char)b[i];
            }
            return str;
        }

        /// <summary>
        /// 将字节数组转化为十六进制字符串，每字节表示为两位
        /// </summary>
        /// <param name="bytes">需要操作的数组</param>
        /// <param name="start">起始位置</param>
        /// <param name="len">长度</param>
        /// <returns>字符串结果</returns>
        public static string BytesToHexString(byte[] bytes, int start, int len)
        {
            string tmpStr = "";
            for (int i = start; i < (start + len); i++) tmpStr += bytes[i].ToString("X2");
            return tmpStr;
        }

        /// <summary>
        /// 字符串转16进制
        /// </summary>
        /// <param name="mHex">需要转换的字符串</param>
        /// <returns>返回十六进制代表的字符串</returns>
        public static string HexToStr(string mHex) // 返回十六进制代表的字符串 
        {
            byte[] bTemp = System.Text.Encoding.Default.GetBytes(mHex);
            string strTemp = "";
            for (int i = 0; i < bTemp.Length; i++)
            {
                strTemp += bTemp[i].ToString("X");
            }
            return strTemp;
        }

        /// <summary>
        /// 将十六进制字符串转化为字节数组
        /// </summary>
        /// <param name="src">需要转换的字符串</param>
        /// <returns>结果数据</returns>
        public static byte[] HexStringToBytes(string src)
        {
            byte[] retBytes = new byte[src.Length / 2];

            for (int i = 0; i < src.Length / 2; i++)
            {
                retBytes[i] = byte.Parse(src.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return retBytes;
        }
        #endregion

        #region 反射执行JS/调用IE/默认浏览器打开URL/MD5加密 等
        /// <summary>
        /// 执行js代码(JS代码,参数,调用方法名,方法名[默认Eval 可选Run])
        /// </summary>
        /// <param name="reString">JS代码</param>
        /// <param name="para">参数</param>
        /// <param name="MethodName">调用方法名</param>
        /// <param name="Method">方法名:默认Eval 可选Run</param>
        /// <returns></returns>
        public static string RunJsMethod(string reString, string para, string MethodName, string Method = "Eval")
        {
            try
            {
                Type obj = Type.GetTypeFromProgID("ScriptControl");
                if (obj == null) return string.Empty;
                object ScriptControl = Activator.CreateInstance(obj);
                obj.InvokeMember("Language", BindingFlags.SetProperty, null, ScriptControl, new object[] { "JScript" });
                obj.InvokeMember("AddCode", BindingFlags.InvokeMethod, null, ScriptControl, new object[] { reString });
                object objx = obj.InvokeMember(Method, BindingFlags.InvokeMethod, null, ScriptControl, new object[] { string.Format("{0}({1})", MethodName, para) }).ToString();//执行结果
                if (objx == null)
                {
                    return string.Empty;
                }
                return objx.ToString();
            }
            catch (Exception ex)
            {
                string ErrorInfo = string.Format("执行JS出现错误:   \r\n 错误描述: {0} \r\n 错误原因: {1} \r\n 错误来源:{2}", ex.Message, ex.InnerException.Message, ex.InnerException.Source);//异常信息
                return ErrorInfo;
            }
        }

        /// <summary>
        /// 打开指定URL openType:0使用IE打开,!=0 使用默认浏览器打开
        /// </summary>
        /// <param name="url">需要打开的地址</param>
        /// <param name="openType">0使用IE,其他使用默认</param>
        public static void OpenUrl(string url, bool useIE = false)
        {
            // 调用ie打开网页
            if (useIE) System.Diagnostics.Process.Start("IEXPLORE.EXE", url);
            else System.Diagnostics.Process.Start(url);
        }

        /// <summary>
        /// 字符串MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncryptMD5String(string str)
        {
            using (MD5 md5String = MD5.Create())
            {
                StringBuilder sb = new StringBuilder();
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                byte[] md5Encrypt = md5String.ComputeHash(bytes);
                for (int i = 0; i < md5Encrypt.Length; i++)
                {
                    sb.Append(md5Encrypt[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        #endregion
    }

    /// <summary>
    /// 从流识别编码
    /// </summary>
    public class StreamEncoder
    {
        /// <summary>   
        /// 取得一个文本文件的编码方式。如果无法在文件头部找到有效的前导符，Encoding.Default将被返回。   
        /// </summary>   
        /// <param name="fileName">文件名。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(string fileName)
        {
            return GetEncoding(fileName, Encoding.Default);
        }
        /// <summary>   
        /// 取得一个文本文件流的编码方式。   
        /// </summary>   
        /// <param name="stream">文本文件流。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(FileStream stream)
        {
            return GetEncoding(stream, Encoding.Default);
        }
        /// <summary>   
        /// 取得一个文本文件的编码方式。   
        /// </summary>   
        /// <param name="fileName">文件名。</param>   
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            Encoding targetEncoding = GetEncoding(fs, defaultEncoding);
            fs.Close();
            return targetEncoding;
        }
        /// <summary>   
        /// 取得一个文本文件流的编码方式。   
        /// </summary>   
        /// <param name="stream">文本文件流。</param>   
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(FileStream stream, Encoding defaultEncoding)
        {
            Encoding targetEncoding = defaultEncoding;
            if (stream != null && stream.Length >= 2)
            {
                //保存文件流的前4个字节   
                byte byte1 = 0;
                byte byte2 = 0;
                byte byte3 = 0;
                byte byte4 = 0;
                //保存当前Seek位置   
                long origPos = stream.Seek(0, SeekOrigin.Begin);
                stream.Seek(0, SeekOrigin.Begin);

                int nByte = stream.ReadByte();
                byte1 = Convert.ToByte(nByte);
                byte2 = Convert.ToByte(stream.ReadByte());
                if (stream.Length >= 3)
                {
                    byte3 = Convert.ToByte(stream.ReadByte());
                }
                if (stream.Length >= 4)
                {
                    byte4 = Convert.ToByte(stream.ReadByte());
                }
                //根据文件流的前4个字节判断Encoding   
                //Unicode {0xFF, 0xFE};   
                //BE-Unicode {0xFE, 0xFF};   
                //UTF8 = {0xEF, 0xBB, 0xBF};   
                if (byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe   
                {
                    targetEncoding = Encoding.BigEndianUnicode;
                }
                if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode   
                {
                    targetEncoding = Encoding.Unicode;
                }
                if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)//UTF8   
                {
                    targetEncoding = Encoding.UTF8;
                }
                //恢复Seek位置         
                stream.Seek(origPos, SeekOrigin.Begin);
            }
            return targetEncoding;
        }
        /// <summary>
        /// 从字节数组中返回字符编码
        /// </summary>
        /// <param name="sen">字节数组</param>
        /// <returns>编码结果</returns>
        public static System.Text.Encoding GetEncodingFromBytes(byte[] sen)
        {
            if (sen.Length == 0)
            {
                return null;
            }
            else
            {
                Stream stream = new MemoryStream(sen);
                return GetEncoding(stream);
            }
        }

        // 新增加一个方法，解决了不带BOM的 UTF8 编码问题    
        /// <summary>   
        /// 通过给定的文件流，判断文件的编码类型   
        /// </summary>   
        /// <param name="fs">文件流</param>   
        /// <returns>文件的编码类型</returns>   
        public static System.Text.Encoding GetEncoding(Stream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM   
            Encoding reVal = Encoding.Default;
            using (BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default))
            {
                byte[] ss = r.ReadBytes(4);
                if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
                {
                    reVal = Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
                {
                    reVal = Encoding.Unicode;
                }
                else
                {
                    if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                    {
                        reVal = Encoding.UTF8;
                    }
                    else
                    {
                        int i;
                        int.TryParse(fs.Length.ToString(), out i);
                        ss = r.ReadBytes(i);

                        if (IsUTF8Bytes(ss))
                            reVal = Encoding.UTF8;
                    }
                }
                r.Close();
                return reVal;
            }
        }

        /// <summary>   
        /// 判断是否是不带 BOM 的 UTF8 格式   
        /// </summary>   
        /// <param name="data"></param>   
        /// <returns></returns>   
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数   
            byte curByte; //当前分析的字节.   
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前   
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　   
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1   
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                return false;
            }
            return true;
        }
    }
}