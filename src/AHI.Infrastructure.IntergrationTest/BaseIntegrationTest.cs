using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AHI.Infrastructure.IntegrationTest.Extension;
using AHI.Infrastructure.IntegrationTest.Models;

namespace AHI.Infrastructure.IntegrationTest
{
    public abstract class BaseIntegrationTest
    {
        protected ILogger _logger;
        protected abstract void SetLogger(ILogger<BaseIntegrationTest> logger);
        protected virtual async Task TestThisSubCollectionAsync(HttpClient httpClient, PostmanEnvironment environment, JArray collection)
        {
            await GetTokenAsync(environment);
            environment.Values.RemoveAll(x => x.Key == "$sessionId");
            environment.Values.Add(new EnvironmentValue("$sessionId", Guid.NewGuid().ToString(), true));
            foreach (var subCollection in collection)
            {
                if (subCollection["request"] is JObject requestObject)
                {
                    var postmanRequest = JObject.FromObject(subCollection).ToObject<PostmanCollectionRequest>();
                    var request = postmanRequest.Request;

                    _logger.LogInformation($"Run the request: {subCollection["name"]}");
                    AddPreRequestScript(subCollection, environment);

                    var testResponse = await SendRequestAsync(httpClient, request, environment);

                    if (!string.IsNullOrEmpty(testResponse))
                    {
                        try
                        {
                            dynamic jsonResponse = JValue.Parse(testResponse);
                            if (jsonResponse is JArray arrayTestResponse)
                            {
                                var responseArray = JsonConvert.DeserializeObject<JArray>(testResponse) as JArray;

                                Dictionary<string, string> outputDictionary = new Dictionary<string, string>();
                                for (int i = 0; i < responseArray.Count; i++)
                                {
                                    outputDictionary = (Dictionary<string, string>)ParseJObject($"_{i}", (JObject)responseArray[i], outputDictionary);
                                }

                                AddEnvironmentVariables(outputDictionary, environment, postmanRequest.Events);
                            }
                            else if (jsonResponse is JObject)
                            {
                                var responseObject = JsonConvert.DeserializeObject<JObject>(testResponse) as JObject;
                                Dictionary<string, string> outputDictionary = (Dictionary<string, string>)ParseJObject("", responseObject);
                                AddEnvironmentVariables(outputDictionary, environment, postmanRequest.Events);
                            }
                            else
                            {
                                Dictionary<string, string> outputDictionary = new Dictionary<string, string>();
                                outputDictionary.Add("_", jsonResponse.ToString());
                                AddEnvironmentVariables(outputDictionary, environment, postmanRequest.Events);
                            }

                        }
                        catch (System.Exception exc)
                        {
                            _logger.LogError(exc, exc.Message);
                        }
                    }
                }
                else if (subCollection["item"] is JArray array)
                {
                    await TestThisSubCollectionAsync(httpClient, environment, array);
                }
            }
        }
        private async Task<string> SendRequestAsync(HttpClient httpClient, PostmanRequest request, PostmanEnvironment environment)
        {
            string path = null;
            if (request.Url.Raw.StartsWith("{{idp}}"))
            {
                // do not run for idp request
                return null;
            }
            var targetHttpClient = httpClient;
            if (!request.Url.Raw.StartsWith("{{host}}"))
            {
                var regex = "{{(.+?)}}/";
                var match = System.Text.RegularExpressions.Regex.Match(request.Url.Raw, regex);
                var value = match.Groups[1].Value;
                var endpoint = environment.Values.First(x => x.Key == value).Value;
                targetHttpClient = new HttpClient()
                {
                    BaseAddress = new Uri(endpoint)
                };
            }
            string pathWithQueryString = null;
            if (request.Url.Path == null)
            {
                pathWithQueryString = request.Url.Raw;
            }
            else
            {
                pathWithQueryString = string.Join('/', request.Url.Path);
            }
            if (request.Url.Raw.Contains("?"))
            {
                pathWithQueryString += $"?{request.Url.Raw.Split('?')[1]}";
            }
            path = pathWithQueryString.ReplaceWithEnvironment(environment);
            targetHttpClient.DefaultRequestHeaders.Clear();
            foreach (var header in request.Header.Where(x => !x.Disabled && x.Key != "Content-Type"))
            {
                targetHttpClient.DefaultRequestHeaders.Add(header.Key, header.Value.ReplaceWithEnvironment(environment));
            }
            try
            {
                HttpResponseMessage result = null;
                switch (request.Method.ToUpper())
                {
                    case "GET":
                        if (request.Body != null)
                        {
                            HttpRequestMessage getRequest = new HttpRequestMessage
                            {
                                Content = GetContent(request, environment),
                                Method = HttpMethod.Get,
                                RequestUri = new Uri(path, UriKind.Relative)
                            };
                            result = await targetHttpClient.SendAsync(getRequest);
                        }
                        else
                        {
                            result = await targetHttpClient.GetAsync(path);
                        }

                        break;
                    case "POST":
                        result = await targetHttpClient.PostAsync(path, GetContent(request, environment));
                        break;
                    case "HEAD":
                        // send the head method
                        HttpRequestMessage headRequest = new HttpRequestMessage
                        {
                            Content = GetContent(request, environment),
                            Method = HttpMethod.Head,
                            RequestUri = new Uri(path, UriKind.Relative)
                        };
                        result = await targetHttpClient.SendAsync(headRequest);
                        break;
                    case "PUT":
                        result = await targetHttpClient.PutAsync(path, GetContent(request, environment));
                        break;
                    case "PATCH":
                        result = await targetHttpClient.PatchAsync(path, GetContent(request, environment));
                        break;
                    case "DELETE":
                        if (request.Body != null)
                        {
                            HttpRequestMessage deleteRequest = new HttpRequestMessage
                            {
                                Content = GetContent(request, environment),
                                Method = HttpMethod.Delete,
                                RequestUri = new Uri(path, UriKind.Relative)
                            };
                            result = await targetHttpClient.SendAsync(deleteRequest);
                        }
                        else
                        {
                            result = await targetHttpClient.DeleteAsync(path);
                        }
                        break;
                }
                var testResponse = await result.Content.ReadAsStringAsync();
                _logger.LogInformation($"{result.StatusCode.ToString()}- {testResponse}");
                return testResponse;
            }
            catch (System.Exception exc)
            {
                _logger.LogError(exc, exc.Message);
            }
            return null;
        }
        protected virtual void AddPreRequestScript(JToken subCollection, PostmanEnvironment environment)
        {
            var events = (subCollection["event"] as JArray)?.Where(x => x["listen"].ToString() == "prerequest").Select(x => x["script"]["exec"]);
            if (events != null && events.Any())
            {
                foreach (var evt in events)
                {
                    if (evt is JArray scripts)
                    {
                        foreach (var script in scripts)
                        {
                            var sScript = script.ToString().Replace("pm.environment.set(", "").Replace(");", "").Replace("\\", "").Replace("\"", "").Trim();
                            if (!string.IsNullOrEmpty(sScript) && sScript.Contains(","))
                            {
                                _logger.LogInformation($"Add prerequest sript {sScript}");
                                var sData = sScript.Split(',');
                                environment.Values.Add(new EnvironmentValue(sData[0], sData[1], true));
                            }
                        }
                    }
                }
            };
        }
        protected virtual HttpContent GetContent(PostmanRequest request, PostmanEnvironment environment)
        {
            if (request.Body == null)
            {
                return new StringContent("");
            }
            if (request.Body.Mode == "raw")
            {
                var content = request.Body.Raw.ReplaceWithEnvironment(environment);
                _logger.LogInformation(content);
                var contentType = request.Header.FirstOrDefault(x => x.Key == "Content-Type")?.Value;
                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = "application/json";
                }
                return new StringContent(content, System.Text.Encoding.UTF8, contentType);
            }
            else if (request.Body.Mode == "urlencoded")
            {
                var content = request.Body.Urlencoded.Select(x => KeyValuePair.Create(x.Key, x.Value.ReplaceWithEnvironment(environment)));
                _logger.LogInformation(JsonConvert.SerializeObject(content));
                return new FormUrlEncodedContent(content);
            }
            else if (request.Body.Mode == "formdata")
            {
                var multipartFormDataContent = new MultipartFormDataContent();
                foreach (var fileRequest in request.Body.FormData)
                {
                    var fileName = Path.GetFileName(fileRequest.Src);
                    var memoryStream = new MemoryStream(File.ReadAllBytes(Path.Combine("AppData", fileName)));
                    var streamContent = new StreamContent(memoryStream);
                    multipartFormDataContent.Add(streamContent, fileRequest.Key, fileName);
                }
                return multipartFormDataContent;
            }
            return new StringContent("");
        }

        protected virtual void AddEnvironmentVariables(Dictionary<string, string> outputDictionary, PostmanEnvironment environment, IEnumerable<PostmanEvent> events)
        {
            foreach (var line in events.SelectMany(x => x.Script.Code).Where(x => x.Contains("pm.environment.set")).Select(x => x.Replace("pm.environment.set", "").Trim()))
            {
                var lineInformation = line.Replace("\n", "").Replace(";", "").Replace("\"", "").TrimStart('(').TrimEnd(')');
                _logger.LogInformation(lineInformation);
                var keypair = lineInformation.Split(',');
                var environmentKey = keypair[0].Trim().Trim('\'');
                var environmentValueKey = keypair[1].Trim();
                var environmentValue = outputDictionary.ContainsKey(environmentValueKey) ? outputDictionary[environmentValueKey] : "";
                if (!string.IsNullOrEmpty(environmentKey))
                {
                    environment.Values.RemoveAll(e => string.Equals(e.Key, environmentKey, System.StringComparison.InvariantCultureIgnoreCase));
                    environment.Values.Add(new EnvironmentValue(environmentKey, environmentValue.ToString(), true));
                }
                else
                {
                    throw new System.Exception($"Key is not found {environmentValueKey}");
                }
            }
            var dictionary = new Dictionary<string, string>();
            foreach (var e in environment.Values)
            {
                dictionary[e.Key] = e.Value;
            }

            environment.Values.Clear();
            foreach (var e in dictionary)
            {
                environment.Values.Add(new EnvironmentValue(e.Key, e.Value, true));
            }
        }
        protected virtual IDictionary<string, string> ParseJObject(string parentToken, JObject json, Dictionary<string, string> outputAppend = null)
        {
            var result = new Dictionary<string, string>();
            foreach (var item in json)
            {
                var key = $"{parentToken}_{item.Key}";
                if (!parentToken.StartsWith("_"))
                {
                    key = key.Trim('_');
                }
                if (item.Value is JObject o)
                {
                    var objectResult = ParseJObject(key, o);
                    MergeDictionary(objectResult, result);
                }
                else if (item.Value is JArray a)
                {
                    int index = 0;
                    foreach (var jitem in a)
                    {
                        var itemKey = $"{key}_{index}";
                        index++;
                        if (jitem is JObject jo)
                        {
                            var objectResult = ParseJObject(itemKey, jo);
                            MergeDictionary(objectResult, result);
                        }
                        else if (jitem is JToken jt)
                        {
                            result[itemKey] = jitem.ToString();
                        }
                    }
                }
                else if (item.Value is JToken token)
                {
                    result[key] = token.ToString();
                }
            }
            if (outputAppend != null)
            {
                foreach (var item in result)
                {
                    outputAppend.Add(item.Key, item.Value);
                }
            }
            return outputAppend is null ? result : outputAppend;
        }
        private void MergeDictionary(IDictionary<string, string> sources, IDictionary<string, string> target)
        {
            foreach (var item in sources)
            {
                target[item.Key] = item.Value;
            }
        }
        protected virtual async Task GetTokenAsync(PostmanEnvironment environment)
        {
            #region Preparing access_token
            using (var httpClient = new HttpClient())
            {
                var tokenEndpoint = environment.Values.First((x => x.Key == "idp" || x.Key == "identity-service" || x.Key == "identity")).Value;
                var clientId = environment.Values.First((x => x.Key == "sa_client_id" || x.Key == "client_id")).Value;
                var clientSecret = environment.Values.First((x => x.Key == "sa_client_secret" || x.Key == "client_secret")).Value;
                var content = new FormUrlEncodedContent(
                         new List<KeyValuePair<string, string>>()
                         {
                            KeyValuePair.Create("grant_type","client_credentials"),
                            KeyValuePair.Create("client_id", clientId),
                            KeyValuePair.Create("client_secret", clientSecret)
                         });
                var tokenRequest = await httpClient.PostAsync($"{tokenEndpoint}/connect/token", content);
                tokenRequest.EnsureSuccessStatusCode();
                var tokenResponse = await tokenRequest.Content.ReadAsStringAsync();
                var accessToken = JsonConvert.DeserializeObject<JObject>(tokenResponse)["access_token"].ToString();
                var accessTokenKey = "access_token";
                var accessTokenEnvironment = environment.Values.FirstOrDefault(x => x.Key == accessTokenKey);
                if (accessTokenEnvironment != null)
                {
                    environment.Values.Remove(accessTokenEnvironment);
                }
                environment.Values.Add(new EnvironmentValue(accessTokenKey, accessToken, true));
                accessTokenKey = "sa_access_token";
                accessTokenEnvironment = environment.Values.FirstOrDefault(x => x.Key == accessTokenKey);
                if (accessTokenEnvironment != null)
                {
                    environment.Values.Remove(accessTokenEnvironment);
                }
                environment.Values.Add(new EnvironmentValue(accessTokenKey, accessToken, true));
            }
            #endregion

        }
    }
}