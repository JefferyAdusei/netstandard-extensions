using System.IO;
using System.Net.Http.Headers;
using System.Net;
using System;

namespace Spyder.Extensions.Web.Models
{
    #region Generic Response

    /// <summary>
    /// A web response from a call to get generic object data from a HTTP server.
    /// </summary>
    public class WebResponse
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the call was successful.
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// Gets or sets the error message when a call has failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the status description
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// Gets or sets the raw server response body.
        /// </summary>
        public Stream RawServerResponse { get; set; }

        /// <summary>
        /// Gets or sets the content type returned by the server
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the response uri sent by the server.
        /// </summary>
        public Uri ResponseUri { get; set; }

        /// <summary>
        /// Gets or sets the actual server response as an object
        /// </summary>
        public object ServerResponse { get; set; }

        /// <summary>
        /// Gets or sets the values of status codes defined for HTTP
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the collection headers associated with web request or response
        /// </summary>
        public HttpResponseHeaders Headers { get; set; }

        #endregion
    }

    #endregion

    #region Type Response

    /// <inheritdoc cref="WebResponse"/>
    /// <typeparam name="T">The type of data to deserialize the raw body into</typeparam>
    public class WebResponse<T> : WebResponse
    {
        /// <summary>
        /// Gets or sets the server response and casts the underlying object to
        /// the specified type.
        /// </summary>
        public new T ServerResponse
        {
            get => (T)base.ServerResponse;
            set => base.ServerResponse = value;
        }
    }

    #endregion
}