using Spyder.Extensions.Web.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Spyder.Extensions.Web.Extensions
{
    /// <summary>
    /// Extension methods for Web Response
    /// </summary>
    public static class ResponseExtensions
    {
        #region Create Web Response Async

        /// <summary>
        /// Returns a <see cref="WebResponse{T}"/> pre-populated with the <see cref="HttpResponseMessage"/> information
        /// </summary>
        /// <typeparam name="TResponse">The type of response to create</typeparam>
        /// <param name="serverResponse">The response sent from the server</param>
        /// <returns>Web response of type {TResponse}</returns>
        public static async Task<WebResponse<TResponse>> CreateWebResponseAsync<TResponse>(this HttpResponseMessage? serverResponse)
        {
            // Return a new web request result
            WebResponse<TResponse> result = new()
            {
                Successful = serverResponse.IsSuccessStatusCode,

                // Content type
                ContentType = serverResponse.Content.Headers.ContentType.MediaType,

                // Response uri
                ResponseUri = serverResponse.Headers.Location,

                // Status Description
                StatusDescription = serverResponse.ReasonPhrase,

                // Headers
                Headers = serverResponse.Headers,

                // Status Code
                StatusCode = serverResponse.StatusCode
            };

            // If response is not successful
            if (!result.Successful)
            {
                /*
                * Read in the response body
                * NOTE: By reading to the end of the stream, the stream will also close
                *       for us (which we must do to release the request)
                */
                return result;
            }

            // Open the response stream...
            //await using Stream responseStream = await serverResponse.Content.ReadAsStreamAsync();

            // Get a read stream
            //using StreamReader streamReader = new StreamReader(responseStream!);

            /*
             * Read in the response body
             * NOTE: By reading to the end of the stream, the stream will also close
             *       for us (which we must do to release the request)
             */
            result.RawServerResponse = await serverResponse.Content.ReadAsStreamAsync();
            //result.RawServerResponse = await streamReader.ReadToEndAsync();

            return result;
        }

        #endregion
    }
}