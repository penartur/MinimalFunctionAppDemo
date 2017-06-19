namespace ConsoleApp
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    class Program
    {
        const string APPNAME = "yourappname";
        const string FUNCTIONKEY = "KR...g==";
        const string AUTHSESSION = "NT...w==";

        static void Main(string[] args)
        {
            Task.WaitAll(MainAsync(args));
        }

        static async Task MainAsync(string[] args)
        {
            var uri = new Uri($"https://{APPNAME}.azurewebsites.net/api/PostFunction?code={FUNCTIONKEY}");
            var cookie = $"AppServiceAuthSession={AUTHSESSION}";
            var data = Enumerable.Range(0, 1000).Select(i => (byte)i).ToArray();
            using (var client = new HttpClient())
            {
                await SendRequest(client, uri, data, cookie, "MyTestApp");

                // Firefox
                await SendRequest(client, uri, data, cookie, "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0");

                // Chrome
                await SendRequest(client, uri, data, cookie, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

                // Edge
                await SendRequest(client, uri, data, cookie, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240");

                // IE
                await SendRequest(client, uri, data, cookie, "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; .NET4.0C; .NET4.0E; InfoPath.3; rv:11.0) like Gecko");

                await SendRequest(client, uri, data, cookie, "MyTestApp/5.0 (MyTestOS 10.0; WOW64; rv:53.0) MyTestEngine/20100101 MyTestApp/53.0");
            }
        }

        private static async Task SendRequest(HttpClient client, Uri uri, byte[] data, string cookie, string userAgent)
        {
            Console.WriteLine($"Sending request for {userAgent}");
            using (var message = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                message.Headers.Add("User-Agent", userAgent);
                message.Headers.Add("Cookie", cookie);
                using (var requestContent = new ByteArrayContent(data))
                {
                    message.Content = requestContent;
                    using (var result = await client.SendAsync(message))
                    {
                        var resultContent = await result.Content.ReadAsStringAsync();
                        Console.WriteLine($"{result.StatusCode}: {resultContent}");
                    }
                }
            }
        }
    }
}
