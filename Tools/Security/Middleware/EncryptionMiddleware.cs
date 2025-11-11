using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HeroServer.Middleware
{
    public class EncryptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly PathString middlewarePath = new PathString("/api/v1");

        public EncryptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke0(HttpContext httpContext)
        {
            await next(httpContext).ConfigureAwait(false);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.Path.StartsWithSegments(middlewarePath))
            {
                await next(httpContext).ConfigureAwait(false);
                return;
            }

            Stream responseBody = httpContext.Response.Body;
            Stream requestBody = httpContext.Request.Body;

            try
            {
                httpContext.Request.EnableBuffering();

                MemoryStream resStream = new MemoryStream();
                httpContext.Response.Body = resStream;

                MemoryStream reqStream = new MemoryStream();
                await requestBody.CopyToAsync(reqStream).ConfigureAwait(false);
                reqStream.Position = 0;
                httpContext.Request.Body = reqStream;

                //MiddlewareRequest middleware = new MiddlewareRequest()
                //{
                //    Verb = "GET",
                //    Uri = "https://leapsolutions.azurewebsites.net/api/v1",
                //    Headers = new Dictionary<String, String>() { { "header1", "value1" }, { "header2", "value2" } },
                //    Body = Encoding.UTF8.GetBytes(new String("Body"))
                //};

                MiddlewareRequest middleware = await JsonSerializer.DeserializeAsync<MiddlewareRequest>(reqStream);
                reqStream.Position = 0;

                await next(httpContext).ConfigureAwait(false);

                reqStream.Position = 0;
                String reqBody = new StreamReader(reqStream).ReadToEnd();

                resStream.Position = 0;
                String resBody = new StreamReader(resStream).ReadToEnd();

                resStream.Position = 0;
                resStream.SetLength(0);

                StreamWriter sw = new StreamWriter(resStream);
                await sw.WriteLineAsync("♦ Request Body   : " + reqBody);
                //await sw.WriteLineAsync("♦ Request Body   : " + middleware.Body);
                //await sw.WriteLineAsync("♦ Request Path   : " + middleware.Uri);
                //await sw.WriteLineAsync("♦ Request Header : ");
                //foreach (String header in middleware.Headers.Keys)
                //    await sw.WriteLineAsync("   > " + header + " : " + middleware.Headers[header]);
                //await sw.WriteLineAsync();
                //await sw.WriteLineAsync("♦ Response Body  : " + resBody);
                await sw.FlushAsync();

                resStream.Position = 0;
                await resStream.CopyToAsync(responseBody).ConfigureAwait(false);
            }
            finally
            {
                httpContext.Request.Body = requestBody;
                httpContext.Response.Body = responseBody;
            }
        }

        public async Task InvokeB(HttpContext httpContext)
        {
            Stream responseBody = httpContext.Response.Body;
            Stream requestBody = httpContext.Request.Body;

            try
            {
                httpContext.Request.EnableBuffering();

                MemoryStream resStream = new MemoryStream();
                httpContext.Response.Body = resStream;

                MemoryStream reqStream = new MemoryStream();
                await requestBody.CopyToAsync(reqStream).ConfigureAwait(false);
                reqStream.Position = 0;
                httpContext.Request.Body = reqStream;

                //httpContext.Request.Headers.Add("Authorization", new StringValues("Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6ImMxMGM5MGJhNGMzNjYzNTE2ZTA3MDdkMGU5YTg5NDgxMDYyODUxNTgiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vZGVtb3NpbmZpbiIsImF1ZCI6ImRlbW9zaW5maW4iLCJhdXRoX3RpbWUiOjE2NDM3NTg4NTUsInVzZXJfaWQiOiJod2hiajJndU9KTTR2NHlydkszb3VVeEl0ZmkyIiwic3ViIjoiaHdoYmoyZ3VPSk00djR5cnZLM291VXhJdGZpMiIsImlhdCI6MTY0Mzc1ODg1NSwiZXhwIjoxNjQzNzYyNDU1LCJlbWFpbCI6InJjaGN1c3RvbWVyQGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwiZmlyZWJhc2UiOnsiaWRlbnRpdGllcyI6eyJlbWFpbCI6WyJyY2hjdXN0b21lckBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.A5pYcASqCwh7iUHkHUkzWbhBdTs1QdWcM3ebSTLKV4CkyWq-Hx7fXDb8t-inXaEYLSPYDahnG8qYIHy0eT25a_gnudtZpe0pkfdcenOe6dXZ1WL4bmQvXC3DvAdXPKrsQX8otqJzfPtrS9crFQWI7MjpNYZLBgbslojxt8WgAh3htx9UfO5BigVhjzhkilQlyGD09m03ktA4cCgmPUH1VXRbr5Lo-c-4LVgHzPhEUo5i7BbKdQonHeRf9PV6cHJQSKSG10c4r6G52q6aWdFoRsaoRiiPxgAjx08cccoz55hGsZXE-H_kpKkmfMHriiwJGaJSCwJP-Gr5ArL4be6znw"));
                //String value = httpContext.Request.Headers["Authorization"];
                //httpContext.Request.Headers["Authorization"] = "Test";

                await next(httpContext).ConfigureAwait(false);

                reqStream.Position = 0;
                String reqBody = new StreamReader(reqStream).ReadToEnd();

                resStream.Position = 0;
                String resBody = new StreamReader(resStream).ReadToEnd();

                resStream.Position = 0;
                resStream.SetLength(0);

                StreamWriter sw = new StreamWriter(resStream);
                await sw.WriteLineAsync("♦ Request Body   : " + reqBody);
                await sw.WriteLineAsync("♦ Request Path   : " + httpContext.Request.Path.Value);
                await sw.WriteLineAsync("♦ Request Header : ");
                foreach (String header in httpContext.Request.Headers.Keys)
                    await sw.WriteLineAsync("   > " + header + " : " + httpContext.Request.Headers[header]);
                await sw.WriteLineAsync();
                await sw.WriteLineAsync("♦ Response Body  : " + resBody);
                await sw.FlushAsync();

                resStream.Position = 0;
                await resStream.CopyToAsync(responseBody).ConfigureAwait(false);
            }
            finally
            {
                httpContext.Request.Body = requestBody;
                httpContext.Response.Body = responseBody;
            }
        }

        public async Task InvokeC(HttpContext httpContext)
        {
            /*
            httpContext.Response.Body = EncryptDecrypt.EncryptStream(httpContext.Response.Body);
            
            httpContext.Request.Body = EncryptDecrypt.DecryptStream(httpContext.Request.Body);
            if (httpContext.Request.QueryString.HasValue)
            {
                string decryptedString = EncryptDecrypt.DecryptString(httpContext.Request.QueryString.Value.Substring(1));
                httpContext.Request.QueryString = new QueryString($"?{decryptedString}");
            }
            */

            httpContext.Response.Body = AesHelper.EncryptStream(httpContext.Response.Body);
            httpContext.Request.Body = AesHelper.DecryptStream(httpContext.Request.Body);

            await next(httpContext);

            await httpContext.Request.Body.DisposeAsync();
            await httpContext.Response.Body.DisposeAsync();
        }
    }
}
