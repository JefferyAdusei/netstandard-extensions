using Spyder.Extensions.Web.Mimes;
using Spyder.Extensions.Web.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Spyder.Extensions.Web.Extensions
{
    public static class RequestExtensions
    {
        #region Properties

        /// <summary>
        /// Sets the general serialization options to web for deserializing json from web api's.
        /// </summary>
        private static readonly JsonSerializerOptions SerializerOptions =
            new(JsonSerializerDefaults.Web);

        #endregion

        #region Get Request

        /// <summary>
        /// Send a GET request to an URL and returns the raw http web response
        /// </summary>
        /// <remarks>IMPORTANT: Remember to close the returned <see cref="HttpResponseMessage"/> stream once done</remarks>
        /// <param name="client"></param>
        /// <param name="url">The URL to make the request to</param>
        /// <param name="returnMimeType">The format to deserialize the content of the response into.</param>
        /// <param name="configureRequest">
        ///     Allows caller to customize and configure the request prior to the request being sent</param>
        /// <param name="bearerToken">
        ///     Provides the Authorization header with 'bearer token-here' for things like JWT bearer tokens
        /// </param>
        /// <returns>An <see cref="HttpResponseMessage"/></returns>
        public static async Task<HttpResponseMessage> GetAsync(this HttpClient client, string url, MimeTypes returnMimeType = MimeTypes.Json, Action<HttpRequestMessage>? configureRequest = null,
            string? bearerToken = null)
        {
            // Set the request message
            using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            // If we have a bearer token...
            if (bearerToken != null)
            {
                // Add the bearer token to the request
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            // Set the response header's mime type
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(returnMimeType.ToMimeText()));


            // Any custom configuration on the request message.
            configureRequest?.Invoke(requestMessage);

            try
            {
                // Return the raw server response as an HttpResponseMessage
                return await client.SendAsync(requestMessage);
            }
            catch (HttpRequestException exception)
            {
                // Log the exception if we have a logger
                FabricDi.Logger?.LogErrorSource(exception.Message);

                // Otherwise, we don't have any information to be able to return
                throw;
            }
        }

        #endregion

        #region Get Request with Type

        /// <summary>
        /// Sends a POST request to an URL and returns a response of the expected data type TResponse
        /// </summary>
        /// <typeparam name="TResponse">Expected type of the web response</typeparam>
        /// <param name="client"></param>
        /// <param name="url">The URL to make the request to</param>
        /// <param name="returnMimeType">The expected type of content to be returned from the server</param>
        /// <param name="configureRequest">Allows caller to customize and configure the request prior to the content being written and sent</param>
        /// <param name="bearerToken">Provides the Authorization header with 'Bearer {token}' for JWT bearer tokens</param>
        /// <returns>Web response of Type TResponse/></returns>
        public static async Task<WebResponse<TResponse>> GetAsync<TResponse>(this HttpClient client, string url,
            MimeTypes returnMimeType = MimeTypes.Json, Action<HttpRequestMessage>? configureRequest = null,
            string? bearerToken = null)
        {
            // Create server response holder
            HttpResponseMessage serverResponse;

            try
            {
                // Make the standard POST call first
                serverResponse = await client.GetAsync(url, returnMimeType, configureRequest,
                                                 bearerToken);
            }
            catch (Exception exception)
            {
                // Log the exception
                FabricDi.Logger?.LogErrorSource(exception.Source, exception: exception);

                // If we got unexpected error, return that
                return new WebResponse<TResponse>
                {
                    // Include exception message
                    ErrorMessage = exception.Message
                };
            }

            // Create a result
            WebResponse<TResponse> result = await serverResponse.CreateWebResponseAsync<TResponse>();

            // If the response status code is not 200...
            if (!result.Successful)
            {
                /*
                 * Call failed
                 * Return no error message so the client can display its own based on the status code
                 */
                return result;
            }

            // Deserialize a raw response
            try
            {
                // If the server response type was not the expected type...
                if (!result.ContentType.Contains(returnMimeType.ToMimeText().ToLower()))
                {
                    // Fail due to unexpected return type
                    result.ErrorMessage =
                        $"Server did not return data in expected type. Expected {returnMimeType.ToMimeText()}, but received {result.ContentType}";
                    return result;
                }

                switch (returnMimeType)
                {
                    // JSON
                    case MimeTypes.Json:
                        await using (result.RawServerResponse)
                        {
                            result.ServerResponse = await JsonSerializer.DeserializeAsync<TResponse>(result.RawServerResponse, SerializerOptions);
                        }
                        break;

                    // XML
                    case MimeTypes.Xml:
                        {
                            // Create XML serializer
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TResponse));

                            await using (result.RawServerResponse)
                            {
                                // Deserialize XML string
                                result.ServerResponse = (TResponse)xmlSerializer.Deserialize(result.RawServerResponse);
                            }

                            break;
                        }

                    // TEXT
                    case MimeTypes.Text:
                        break;

                    // UNKNOWN
                    default:
                        {
                            // If deserialize failed, then set error message
                            result.ErrorMessage =
                                "Unknown return type, cannot deserialize server response to the expected type";

                            return result;
                        }
                }
            }
            catch (Exception exception)
            {
                // If deserialize failed then set error message
                result.ErrorMessage = exception.Message;

                return result;
            }

            // Return result
            return result;
        }

        #endregion

        #region General Post Request

        /// <summary>
        /// Sends a POST request to an URL and returns the raw <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <remarks>IMPORTANT: Remember to close the returned <see cref="HttpResponseMessage"/> once done.</remarks>
        /// <param name="client"></param>
        /// <param name="url">The URL to make the request to</param>
        /// <param name="content">The content to post</param>
        /// <param name="sendMimeType">The format to serialize the content to</param>
        /// <param name="returnMimeType">The expected type of content to be returned from the server</param>
        /// <param name="configureRequest">Allows caller to customize and configure the request prior to the content being written and sent</param>
        /// <param name="bearerToken">Provides the Authorization header with 'Bearer {token}' for JWT bearer tokens</param>
        /// <returns><see cref="HttpResponseMessage"/></returns>
        public static async Task<HttpResponseMessage?> PostAsync(this HttpClient client, string url, object? content = null,
            MimeTypes sendMimeType = MimeTypes.Json,
            MimeTypes returnMimeType = MimeTypes.Json, Action<HttpRequestMessage>? configureRequest = null,
            string? bearerToken = null)
        {
            // Set the request message
            using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            // Set the response header's mime type
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(returnMimeType.ToMimeText()));

            // If we have a bearer token...
            if (bearerToken != null)
            {
                // Add the bearer token to the request
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            // Any custom configuration on the request message.
            configureRequest?.Invoke(requestMessage);

            #region Write Content

            // Set the content length
            string contentString;

            switch (sendMimeType)
            {
                case MimeTypes.Json:

                    // Serialize the content object into a string
                    contentString = JsonSerializer.Serialize(content!);
                    break;

                case MimeTypes.Xml:
                    {
                        // Create XML serializer
                        XmlSerializer xmlSerializer = new XmlSerializer(content?.GetType()!);

                        // Create a string writer to receive the serialized string
                        await using StringWriter stringWriter = new StringWriter();

                        // Serialize the object to a string
                        xmlSerializer.Serialize(stringWriter, content!);

                        // Extract the string from the writer
                        contentString = stringWriter.ToString();
                        break;
                    }
                default:
                    return null;
            }

            // Set the Http
            using HttpContent httpContent = new StringContent(contentString, Encoding.UTF8, sendMimeType.ToMimeText());

            requestMessage.Content = httpContent;

            #endregion

            try
            {
                // Return the raw server response
                return await client.SendAsync(requestMessage);
            }
            catch (HttpRequestException exception)
            {
                // Log exception
                FabricDi.Logger?.LogErrorSource(exception.Message, exception: exception);

                // If there is no information, throw an exception
                return new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
            }
            catch (ArgumentNullException exception)
            {
                // Log exception
                FabricDi.Logger?.LogErrorSource(exception.Message, exception: exception);

                return new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
            }
            catch (InvalidOperationException exception)
            {
                // Log exception
                FabricDi.Logger?.LogErrorSource(exception.Message, exception: exception);

                return new HttpResponseMessage(HttpStatusCode.PreconditionRequired);
            }
        }

        #endregion

        #region Post Request with Type

        /// <summary>
        /// Sends a POST request to an URL and returns a response of the expected data type TResponse
        /// </summary>
        /// <typeparam name="TResponse">Expected type of the web response</typeparam>
        /// <param name="client"></param>
        /// <param name="url">The URL to make the request to</param>
        /// <param name="content">The content to post</param>
        /// <param name="sendMimeType">The format to serialize the content to</param>
        /// <param name="returnMimeType">The expected type of content to be returned from the server</param>
        /// <param name="configureRequest">Allows caller to customize and configure the request prior to the content being written and sent</param>
        /// <param name="bearerToken">Provides the Authorization header with 'Bearer {token}' for JWT bearer tokens</param>
        /// <returns>Web response of Type TResponse/></returns>
        public static async Task<WebResponse<TResponse>> PostAsync<TResponse>(this HttpClient client, string url, object? content = null,
            MimeTypes sendMimeType = MimeTypes.Json,
            MimeTypes returnMimeType = MimeTypes.Json, Action<HttpRequestMessage>? configureRequest = null,
            string? bearerToken = null)
        {
            // Create server response holder
            HttpResponseMessage? serverResponse;

            try
            {
                // Make the standard POST call first
                serverResponse = await client.PostAsync(url, content, sendMimeType, returnMimeType, configureRequest,
                                                 bearerToken);
            }
            catch (Exception exception)
            {
                // Log the exception
                FabricDi.Logger?.LogErrorSource(exception.Source, exception: exception);

                // If we got unexpected error, return that
                return new WebResponse<TResponse>
                {
                    // Include exception message
                    ErrorMessage = exception.Message
                };
            }

            // Create a result
            WebResponse<TResponse> result = await serverResponse.CreateWebResponseAsync<TResponse>();

            // If the response status code is not 200...
            if (!result.Successful)
            {
                /*
                 * Call failed
                 * Return no error message so the client can display its own based on the status code
                 */
                return result;
            }

            // If we have no content to deserialize...
            if (result.RawServerResponse.Length <= 0)
            {
                return result;
            }

            // Deserialize a raw response
            try
            {
                // If the server response type was not the expected type...
                if (!result.ContentType.Contains(returnMimeType.ToMimeText().ToLower()))
                {
                    // Fail due to unexpected return type
                    result.ErrorMessage =
                        $"Server did not return data in expected type. Expected {returnMimeType.ToMimeText()}, but received {result.ContentType}";
                    return result;
                }

                switch (returnMimeType)
                {
                    // JSON
                    case MimeTypes.Json:

                        await using (result.RawServerResponse)
                        {
                            result.ServerResponse = await JsonSerializer.DeserializeAsync<TResponse>(result.RawServerResponse, SerializerOptions);
                        }

                        break;

                    // XML
                    case MimeTypes.Xml:
                        {
                            // Create XML serializer
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TResponse));

                            await using (result.RawServerResponse)
                            {
                                // Deserialize XML string
                                result.ServerResponse = (TResponse)xmlSerializer.Deserialize(result.RawServerResponse);
                            }

                            break;
                        }

                    // TEXT
                    case MimeTypes.Text:
                        break;

                    // UNKNOWN
                    default:
                        {
                            // If deserialize failed, then set error message
                            result.ErrorMessage =
                                "Unknown return type, cannot deserialize server response to the expected type";

                            return result;
                        }
                }
            }
            catch (JsonException exception)
            {
                // Log the error.
                FabricDi.Logger.LogErrorSource($"An error occurred while deserializing{nameof(TResponse)}", exception: exception);

                // If deserializing JSON failed, then set JSON Exception
                result.ErrorMessage = exception.Message;

                return result;
            }
            catch (Exception exception)
            {
                // If deserialize failed then set error message
                result.ErrorMessage = exception.Message;

                return result;
            }

            // Return result
            return result;
        }

        #endregion
    }
}