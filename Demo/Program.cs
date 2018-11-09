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

            var result = client.GetByHttpResult("www.qq.com");
            Console.ReadKey();
        }
    }
}
