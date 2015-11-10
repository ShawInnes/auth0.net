﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Auth0.Core.Models;
using Newtonsoft.Json;
using PortableRest;

namespace Auth0.Api.Management
{
    public class ApiConnection : RestClient, IApiConnection
    {
        private readonly string token;

        public ApiConnection(string token, string baseUrl)
        {
            this.token = token;
            BaseUrl = baseUrl;
        }

        public async Task<T> DeleteAsync<T>(string resource, IDictionary<string, string> urlSegments) where T : class
        {
            return await RunAsync<T>(resource,
                HttpMethod.Delete,
                ContentTypes.Json,
                urlSegments,
                null,
                null,
                null
                ).ConfigureAwait(false);
        }

        public async Task<T> GetAsync<T>(string resource, IDictionary<string, string> urlSegments, IDictionary<string, string> queryStrings) where T : class
        {
            return await RunAsync<T>(resource,
                HttpMethod.Get,
                ContentTypes.Json, 
                urlSegments,
                queryStrings,
                null,
                null
                ).ConfigureAwait(false);
        }

        public async Task<T> PostAsync<T>(string resource, object body) where T : class
        {
            return await RunAsync<T>(resource,
                HttpMethod.Post,
                ContentTypes.Json, 
                null,
                null,
                new Dictionary<string, object>
                {
                    { "body", body}
                }, 
                null
                ).ConfigureAwait(false);
        }

        public async Task<T> PatchAsync<T>(string resource, object body, Dictionary<string, string> urlSegments) where T : class
        {
            return await RunAsync<T>(resource,
                new HttpMethod("PATCH"), 
                ContentTypes.Json,
                urlSegments,
                null,
                new Dictionary<string, object>
                {
                    { "body", body}
                },
                null
                ).ConfigureAwait(false);
        }

        private async Task<T> RunAsync<T>(string resource,
            HttpMethod httpMethod,
            ContentTypes contentTypes,
            IDictionary<string, string> urlSegments,
            IDictionary<string, string> queryStrings,
            IDictionary<string, object> parameters,
            IDictionary<string, object> headers
            ) where T : class
        {
            var request = new RestRequest(resource, httpMethod)
            {
                ContentType = contentTypes,
                JsonSerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            };

            // Apply the URL Segments
            if (urlSegments != null)
            {
                foreach (var pair in urlSegments)
                {
                    request.AddUrlSegment(pair.Key, pair.Value);
                }
            }

            // Apply the Query strings
            if (queryStrings != null)
            {
                foreach (var pair in queryStrings)
                {
                    request.AddQueryString(pair.Key, pair.Value);
                }
            }

            // Apply the Parameters
            if (parameters != null)
            {
                foreach (var pair in parameters)
                {
                    request.AddParameter(pair.Key, pair.Value);
                }
            }

            // Set the authorization header
            request.AddHeader("Authorization", string.Format("Bearer {0}", token));

            // Apply other headers
            if (headers != null)
            {
                foreach (var pair in headers)
                {
                    request.AddHeader(pair.Key, pair.Value);
                }
            }

            // Send the request
            var response = await SendAsync<T>(request).ConfigureAwait(false);

            HandleErrors(response);

            return response.Content;
        }

        private void HandleErrors<T>(RestResponse<T> response) where T : class
        {
        }
    }
}