using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace HeroServer
{
    public static class LeapServerHelper
    {
        static String leapServerUrl;
        static String leapServerPrefix;

        private static String leapServerUserName;
        private static String leapServerUserPassword;

        private static String leapToken = null;
        private static DateTime leapTokenExpiration = DateTime.Now;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions() { Converters = { new LeapResultJsonConverter() } };

        public static async void Initialize()
        {
            leapServerUrl = await new SystemParamDB().GetValue("LeapServerUrl");
            leapServerPrefix = await new SystemParamDB().GetValue("LeapServerPrefix");

            leapServerUserName = await new SystemParamDB().GetValue("LeapServerUserName");
            leapServerUserPassword = await new SystemParamDB().GetValue("LeapServerPassword");
        }

        public static async Task<String> GetToken()
        {
            SignInRequest signInRequest = new SignInRequest(new UserCredential(leapServerUserName, leapServerUserPassword));

            SignInResponse response = await LeapServerHelper.PostAnonymous<SignInRequest, SignInResponse>("access/SignIn", signInRequest);

            if (response.Granted == 0)
                return null;

            return response.AccessToken;
        }

        public static async Task<String> UpdateToken()
        {
            if (leapTokenExpiration > DateTime.Now)
                return leapToken;

            leapToken = await GetToken();
            leapTokenExpiration = leapTokenExpiration.AddHours(1);

            return leapToken;
        }

        public static async Task<U> Get<U>(String endpoint, Dictionary<String, String> query = null)
        {
            String token = await UpdateToken();

            using (HttpClient client = new HttpClient())
            {
                String address = leapServerUrl + leapServerPrefix + endpoint;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (query != null)
                    address = QueryHelpers.AddQueryString(address, query);

                HttpResponseMessage httpResponse = await client.GetAsync(address);

                if (httpResponse.StatusCode != HttpStatusCode.OK)
                    throw new Exception(httpResponse.ReasonPhrase);

                if (typeof(U) == typeof(String))
                    return (U)Convert.ChangeType(await httpResponse.Content.ReadAsStringAsync(), typeof(U));

                return await JsonSerializer.DeserializeAsync<U>(await httpResponse.Content.ReadAsStreamAsync(), jsonSerializerOptions);
            }
        }

        public static async Task<U> Post<U>(String endpoint, Dictionary<String, String> query = null)
        {
            String token = await UpdateToken();

            using (HttpClient client = new HttpClient())
            {
                String address = leapServerUrl + leapServerPrefix + endpoint;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (query != null)
                    address = QueryHelpers.AddQueryString(address, query);

                HttpResponseMessage httpResponse = await client.PostAsync(address, null);

                if (httpResponse.StatusCode != HttpStatusCode.OK)
                    throw new Exception(httpResponse.ReasonPhrase);

                if (typeof(U) == typeof(String))
                    return (U)Convert.ChangeType(await httpResponse.Content.ReadAsStringAsync(), typeof(U));

                return await JsonSerializer.DeserializeAsync<U>(await httpResponse.Content.ReadAsStreamAsync(), jsonSerializerOptions);
            }
        }

        public static async Task<U> PostAnonymous<T, U>(String endpoint, T request, Dictionary<String, String> query = null)
        {
            using (HttpClient client = new HttpClient())
            {
                String address = leapServerUrl + leapServerPrefix + endpoint;

                if (query != null)
                    address = QueryHelpers.AddQueryString(address, query);

                using (StringContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage httpResponse = await client.PostAsync(address, content);

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                        throw new Exception(httpResponse.ReasonPhrase);

                    if (typeof(U) == typeof(String))
                        return (U)Convert.ChangeType(await httpResponse.Content.ReadAsStringAsync(), typeof(U));

                    return await JsonSerializer.DeserializeAsync<U>(await httpResponse.Content.ReadAsStreamAsync());
                }
            }
        }

        public static async Task<U> Post<T, U>(String endpoint, T request, Dictionary<String, String> query = null)
        {
            String token = await UpdateToken();

            using (HttpClient client = new HttpClient())
            {
                String address = leapServerUrl + leapServerPrefix + endpoint;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (query != null)
                    address = QueryHelpers.AddQueryString(address, query);

                using (StringContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage httpResponse = await client.PostAsync(address, content);

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                        throw new Exception(httpResponse.ReasonPhrase);

                    if (typeof(U) == typeof(String))
                        return (U)Convert.ChangeType(await httpResponse.Content.ReadAsStringAsync(), typeof(U));

                    return await JsonSerializer.DeserializeAsync<U>(await httpResponse.Content.ReadAsStreamAsync(), jsonSerializerOptions);
                }
            }
        }

        public static async Task<U> Put<U>(String endpoint, Dictionary<String, String> query = null)
        {
            String token = await UpdateToken();

            using (HttpClient client = new HttpClient())
            {
                String address = leapServerUrl + leapServerPrefix + endpoint;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (query != null)
                    address = QueryHelpers.AddQueryString(address, query);

                HttpResponseMessage httpResponse = await client.PutAsync(address, null);

                if (httpResponse.StatusCode != HttpStatusCode.OK)
                    throw new Exception(httpResponse.ReasonPhrase);

                if (typeof(U) == typeof(String))
                    return (U)Convert.ChangeType(await httpResponse.Content.ReadAsStringAsync(), typeof(U));

                return await JsonSerializer.DeserializeAsync<U>(await httpResponse.Content.ReadAsStreamAsync(), jsonSerializerOptions);
            }
        }

        public static async Task<U> Put<T, U>(String endpoint, T request, Dictionary<String, String> query = null)
        {
            String token = await UpdateToken();

            using (HttpClient client = new HttpClient())
            {
                String address = leapServerUrl + leapServerPrefix + endpoint;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (query != null)
                    address = QueryHelpers.AddQueryString(address, query);

                using (StringContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage httpResponse = await client.PutAsync(address, content);

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                        throw new Exception(httpResponse.ReasonPhrase);

                    if (typeof(U) == typeof(String))
                        return (U)Convert.ChangeType(await httpResponse.Content.ReadAsStringAsync(), typeof(U));

                    return await JsonSerializer.DeserializeAsync<U>(await httpResponse.Content.ReadAsStreamAsync(), jsonSerializerOptions);
                }
            }
        }
    }
}
