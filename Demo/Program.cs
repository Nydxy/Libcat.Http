using System;
using System.Net;
using Libcat.Http;


namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            
            //Console.WriteLine(result.StatusDescription);
            //result = client.Get("http://zccx.tyb.njupt.edu.cn/",true,null);
            //Console.WriteLine(result.StatusDescription);
            //result = client.Post("http://zccx.tyb.njupt.edu.cn/","");
            //Console.WriteLine(result.StatusDescription);
            //var container = new CookieContainer();
            //var result = HttpRequest.Get("http://zccx.tyb.njupt.edu.cn/", container);
            var s = new System.Diagnostics.Stopwatch();
            for (int i=0;i<10;i++)
            {
                s.Start();
                var result = client.Get("http://www.qq.com");
                //var result = HttpRequest.Get("http://www.qq.com");
                s.Stop();
                Console.WriteLine(s.ElapsedMilliseconds + " ms");
                s.Reset();
            }
            
            Console.ReadKey();
        }
    }
}
