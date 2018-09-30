# Libcat.Http
- A very simple & light http library. You can do get/post requests easily with this lib.
- Only frequently used functions included (but enough at most time),such as get/post.
- There is no need to warry about most of the annoying system apis, parametres, certificates, byte arrays, encodings, etc.

## Code structure (Classes)
- **HttpRequest**: important parametres of one request; methods to do the request, and methods to process the response.
- **HttpResult**: Response text/html,status code(200,404,...), etc.
- **Tools**: some useful tools, such as get timestamp, url decoding, and so on.

## How to do a GET/POST request ?
### Normal way
```
var request=new HttpRequest("http://www.google.com/");  //create a request
var result=request.GetResponse();

Console.WriteLine(result.Html);
Console.WriteLine(result.StatusCodeNum);
```

Set request headers like this
```
request.CookieContainer=myCookieContainer;
request.referer="https://www.youtube.com";
request.TimeOut=1000;
//discover by yourself!
```

### Simple way
```
var result=HttpRequest.Get("http://www.google.com/");
var result=HttpRequest.Post("http://www.google.com/","id=123&password=456");
//discover by yourself!
```

## Features
- Automaticly update cookiecontainer (set request.CookieContainer=yourContainer)
- 6 versions of Get method, 2 versions of Post method
- Support RESTful api partly (set request.Method=GET/POST/DELETE)
- Discover by yourself, I need sleep urgently.

## Future plan
Now this project is just a preview version, I make it in one day (some parts of it are written by others, without a liscense)  
And I did not fully test it. God knows how many bugs there are.  
  
Here are my plan:  
- Fix bugs
- Support async get/post methods
- Support all RESTful api
- Support download files and images (although you can download them in byte array now)

## End
- If you find any bug or have suggestions for me, raise issues.  
- I am not responsible for any problem caused by this lib (Libcat.http)  

Thank you! 