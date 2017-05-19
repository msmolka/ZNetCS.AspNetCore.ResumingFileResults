﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingFileResultExecutorBase.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The resuming file result executor base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults.Infrastructure
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Internal;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;

    #endregion

    /// <summary>
    /// The resuming file result executor base.
    /// </summary>
    public abstract class ResumingFileResultExecutorBase : FileResultExecutorBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingFileResultExecutorBase"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        protected ResumingFileResultExecutorBase(ILogger logger) : base(logger)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Evaluates the precondition header fields.
        /// </summary>
        /// <param name="context">
        /// The action context to access request.
        /// </param>
        /// <param name="result">
        /// The file result to perform precondition check.
        /// </param>
        /// <param name="statusCode">
        /// The additional status code for precondition.
        /// </param>
        /// <remarks>
        /// See RFC 7232 https://tools.ietf.org/html/rfc7232.
        /// See point 6 for precedence.
        /// </remarks>
        protected virtual bool CheckDefaultPrecondition(ActionContext context, ResumingFileResult result, out HttpStatusCode? statusCode)
        {
            // An origin server that receives an If-Match header field MUST evaluate
            // the condition prior to performing the method. If the field - value is "*",
            // the condition is false if the origin server does not have a current representation
            // for the target resource. If the field - value is a list of entity - tags,
            // the condition is false if none of the listed tags match the entity - tag of the selected
            // representation. An origin server MUST use the strong comparison function when
            // comparing entity-tags.
            string ifMatchValue = context.HttpContext.Request.Headers[HeaderNames.IfMatch];
            if (!string.IsNullOrWhiteSpace(ifMatchValue))
            {
                if (EntityTagHeaderValue.TryParseList(new List<string> { ifMatchValue }, out IList<EntityTagHeaderValue> entityTagHeaderValues))
                {
                    // only one element * is allowed
                    if ((entityTagHeaderValues.Count > 1) && entityTagHeaderValues.Any(e => e.Equals(EntityTagHeaderValue.Any)))
                    {
                        statusCode = HttpStatusCode.PreconditionFailed;
                        return false;
                    }

                    if ((result.EntityTag == null) ||
                        !(entityTagHeaderValues.Any(e => e.Equals(EntityTagHeaderValue.Any)) || entityTagHeaderValues.Any(e => e.Compare(result.EntityTag, true))))
                    {
                        statusCode = HttpStatusCode.PreconditionFailed;
                        return false;
                    }
                }
            }
            else if (result.LastModified.HasValue)
            {
                // When recipient is the origin server, If-Match is not present, and If-Unmodified-Since is present
                string ifUnmodifiedSinceValue = context.HttpContext.Request.Headers[HeaderNames.IfUnmodifiedSince];
                if (!string.IsNullOrWhiteSpace(ifUnmodifiedSinceValue))
                {
                    if (HeaderUtilities.TryParseDate(ifUnmodifiedSinceValue, out DateTimeOffset date))
                    {
                        // The origin server MUST NOT perform the requested method
                        // if the selected representation's last modification date is more
                        // recent than the date provided in the field-value
                        if (result.LastModified.Value > date)
                        {
                            statusCode = HttpStatusCode.PreconditionFailed;
                            return false;
                        }
                    }
                }
            }

            // An origin server MUST NOT perform the requested method if the
            // condition evaluates to false; instead, the origin server MUST respond
            // with either a) the 304(Not Modified) status code if the request
            // method is GET or HEAD or b) the 412(Precondition Failed) status code
            // for all other request methods. A recipient MUST use the weak comparison function when comparing
            // entity - tags.
            string ifNoneMatchValue = context.HttpContext.Request.Headers[HeaderNames.IfNoneMatch];
            if (!string.IsNullOrWhiteSpace(ifNoneMatchValue))
            {
                if (EntityTagHeaderValue.TryParseList(new List<string> { ifNoneMatchValue }, out IList<EntityTagHeaderValue> entityTagHeaderValues))
                {
                    // only one element * is allowed
                    if ((entityTagHeaderValues.Count > 1) && entityTagHeaderValues.Any(e => e.Equals(EntityTagHeaderValue.Any)))
                    {
                        statusCode = (context.HttpContext.Request.Method == "GET") || (context.HttpContext.Request.Method == "HEAD")
                            ? HttpStatusCode.NotModified
                            : HttpStatusCode.PreconditionFailed;
                        return false;
                    }

                    if ((result.EntityTag != null) &&
                        (entityTagHeaderValues.Any(e => e.Equals(EntityTagHeaderValue.Any)) || entityTagHeaderValues.Any(e => e.Compare(result.EntityTag, false))))
                    {
                        statusCode = (context.HttpContext.Request.Method == "GET") || (context.HttpContext.Request.Method == "HEAD")
                            ? HttpStatusCode.NotModified
                            : HttpStatusCode.PreconditionFailed;
                        return false;
                    }
                }
            }
            else if (result.LastModified.HasValue)
            {
                // When the method is GET or HEAD, If-None-Match is not present, and
                // If-Modified-Since is present, evaluate the If-Modified-Since precondition
                if ((context.HttpContext.Request.Method == "GET") || (context.HttpContext.Request.Method == "HEAD"))
                {
                    string ifModifiedSinceValue = context.HttpContext.Request.Headers[HeaderNames.IfModifiedSince];
                    if (!string.IsNullOrWhiteSpace(ifModifiedSinceValue))
                    {
                        if (HeaderUtilities.TryParseDate(ifModifiedSinceValue, out DateTimeOffset date))
                        {
                            // The origin server SHOULD NOT perform the requested
                            // method if the selected representation's last modification date is
                            // earlier than or equal to the date provided in the field-value
                            if (result.LastModified.Value <= date)
                            {
                                statusCode = HttpStatusCode.NotModified;
                                return false;
                            }
                        }
                    }
                }
            }

            statusCode = null;
            return true;
        }

        /// <summary>
        /// Checks if range is supported in current request and evaluate the range precondition header field.
        /// </summary>
        /// <param name="context">
        /// The action context to access request.
        /// </param>
        /// <param name="result">
        /// The file result to perform precondition check.
        /// </param>
        /// <remarks>
        /// See RFC 7232 https://tools.ietf.org/html/rfc7232.
        /// See point 6 for precedence.
        /// </remarks>
        protected virtual bool CheckRangePrecondition(ActionContext context, ResumingFileResult result)
        {
            // When the method is GET and both Range and If-Range are present, evaluate the If-Range precondition
            string ifRangeValue = context.HttpContext.Request.Headers[HeaderNames.IfRange];
            if ((context.HttpContext.Request.Method != "GET") || string.IsNullOrWhiteSpace(ifRangeValue)
                || string.IsNullOrWhiteSpace(context.HttpContext.Request.Headers[HeaderNames.Range]))
            {
                return true;
            }

            // The "If-Range" header field allows a client to "short-circuit" the
            // second request.  Informally, its meaning is as follows: if the
            // representation is unchanged, send me the part(s)that I am requesting
            // in Range; otherwise, send me the entire representation.
            if (RangeConditionHeaderValue.TryParse(ifRangeValue, out RangeConditionHeaderValue rangeConditionHeaderValue))
            {
                if (rangeConditionHeaderValue.LastModified.HasValue)
                {
                    if (!result.LastModified.HasValue || (result.LastModified.Value > rangeConditionHeaderValue.LastModified))
                    {
                        // the content changed so send whole content
                        return false;
                    }
                }
                else if (rangeConditionHeaderValue.EntityTag != null)
                {
                    // A client MUST NOT generate an If-Range header field containing an entity-tag that is marked as weak.
                    // A server that evaluates an If-Range precondition MUST use the strong comparison function when comparing entity-tags.
                    if ((result.EntityTag == null) || !rangeConditionHeaderValue.EntityTag.Compare(result.EntityTag, true))
                    {
                        // not matched so send whole content
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Executes context asynchronously.
        /// </summary>
        /// <param name="context">
        /// The action context to access request and response.
        /// </param>
        /// <param name="inputStream">
        /// The input stream to transfer. This is taking ownership so it will close it on the end.
        /// </param>
        /// <param name="result">
        /// The action result to process.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to support cancellation.
        /// </param>
        protected virtual async Task ExecuteAsync(
            ActionContext context,
            Stream inputStream,
            ResumingFileResult result,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.SetDefaultHeaders(context, result);

            if (!this.CheckDefaultPrecondition(context, result, out HttpStatusCode? statusCode))
            {
                // ReSharper disable once PossibleInvalidOperationException
                await new StatusCodeResult((int)statusCode.Value).ExecuteResultAsync(context);
                return;
            }

            if (!this.CheckRangePrecondition(context, result))
            {
                this.SetContentLengthHeader(context, inputStream.Length);

                // means if-range failed so just return whole file.
                using (inputStream)
                {
                    await this.WriteContentAsync(context, inputStream, 0, null, cancellationToken);
                }

                return;
            }

            long streamLength = inputStream.Length;
            var ranges = this.GetRequestRanges(context, streamLength);
            if (ranges.Count == 0)
            {
                this.SetContentLengthHeader(context, streamLength);

                // return whole file
                using (inputStream)
                {
                    await this.WriteContentAsync(context, inputStream, 0, null, cancellationToken);
                }
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.PartialContent;

                // here will be partial result
                using (inputStream)
                {
                    // multipart
                    if (ranges.Count > 1)
                    {
                        const string crlf = "\r\n";
                        string boundary = $"NextPart_{Guid.NewGuid():N}";

                        MediaTypeHeaderValue mediaTypeHeaderValue = MediaTypeHeaderValue.Parse("multipart/byteranges");
                        mediaTypeHeaderValue.Boundary = boundary;
                        context.HttpContext.Response.ContentType = mediaTypeHeaderValue.ToString();

                        string contentTypeBoundary = $"{HeaderNames.ContentType}: {result.ContentType}";

                        long contentLength = this.CalculateMultipartBoundaryLength(ranges, streamLength, boundary, result.ContentType);

                        this.SetContentLengthHeader(context, contentLength);

                        foreach (ProcessRange range in ranges)
                        {
                            await context.HttpContext.Response.WriteAsync($"--{boundary}", cancellationToken);
                            await context.HttpContext.Response.WriteAsync(crlf, cancellationToken);
                            await context.HttpContext.Response.WriteAsync(contentTypeBoundary, cancellationToken);
                            await context.HttpContext.Response.WriteAsync(crlf, cancellationToken);

                            var contentRangeHeaderValue = new ContentRangeHeaderValue(range.From, range.To, streamLength);
                            string contentRangeBoundary = $"{HeaderNames.ContentRange}: {contentRangeHeaderValue}";
                            await context.HttpContext.Response.WriteAsync(contentRangeBoundary, cancellationToken);
                            await context.HttpContext.Response.WriteAsync(crlf, cancellationToken);
                            await context.HttpContext.Response.WriteAsync(crlf, cancellationToken);

                            await this.WriteContentAsync(context, inputStream, range.From, range.Length, cancellationToken);
                            await context.HttpContext.Response.WriteAsync(crlf, cancellationToken);
                        }

                        await context.HttpContext.Response.WriteAsync($"--{boundary}--", cancellationToken);
                    }
                    else
                    {
                        ProcessRange range = ranges.Single();
                        var contentRangeHeaderValue = new ContentRangeHeaderValue(range.From, range.To, streamLength);
                        context.HttpContext.Response.Headers[HeaderNames.ContentRange] = contentRangeHeaderValue.ToString();

                        this.SetContentLengthHeader(context, range.Length);

                        await this.WriteContentAsync(context, inputStream, range.From, range.Length, cancellationToken);
                    }
                }
            }
        }

        /// <summary>
        /// Sets common headers for the response.
        /// </summary>
        /// <param name="context">
        /// The action context to access response.
        /// </param>
        /// <param name="length">
        /// The length of response.
        /// </param>
        protected virtual void SetContentLengthHeader(ActionContext context, long length)
        {
            context.HttpContext.Response.Headers[HeaderNames.ContentLength] = HeaderUtilities.FormatInt64(length);
        }

        /// <summary>
        /// Sets default headers for the response.
        /// </summary>
        /// <param name="context">
        /// The action context to access response.
        /// </param>
        /// <param name="result">
        /// The action result to process.
        /// </param>
        protected virtual void SetDefaultHeaders(ActionContext context, ResumingFileResult result)
        {
            this.SetHeadersAndLog(context, result);

            IHeaderDictionary headers = context.HttpContext.Response.Headers;

            // if there is no file name set Content-Disposition as inline
            if (string.IsNullOrEmpty(result.FileDownloadName))
            {
                var contentDisposition = new ContentDispositionHeaderValue("inline");

                if (!string.IsNullOrWhiteSpace(result.FileInlineName))
                {
                    contentDisposition.SetHttpFileName(result.FileInlineName);
                }

                headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
            }

            var nameValueHeader = new NameValueHeaderValue("bytes");
            headers[HeaderNames.AcceptRanges] = nameValueHeader.ToString();

            if (result.EntityTag != null)
            {
                headers[HeaderNames.ETag] = result.EntityTag.ToString();
            }

            if (result.LastModified.HasValue)
            {
                headers[HeaderNames.LastModified] = HeaderUtilities.FormatDate(result.LastModified.Value);
            }
        }

        /// <summary>
        /// Calculate multipart boundary length.
        /// </summary>
        /// <param name="ranges">
        /// The ranges used for calculation.
        /// </param>
        /// <param name="streamLength">
        /// The stream length for Content-Range line calculation.
        /// </param>
        /// <param name="boundary">
        /// The boundary text to calculate boundary delimiter length.
        /// </param>
        /// <param name="contentType">
        /// The content type for Content-Type line calculation.
        /// </param>
        /// <returns>
        /// Calculated multipart boundary length.
        /// </returns>
        private long CalculateMultipartBoundaryLength(IEnumerable<ProcessRange> ranges, long streamLength, string boundary, string contentType)
        {
            long contentLength = 0;
            string contentTypeBoundary = $"{HeaderNames.ContentType}: {contentType}";
            foreach (ProcessRange range in ranges)
            {
                var contentRangeHeaderValue = new ContentRangeHeaderValue(range.From, range.To, streamLength);
                string contentRangeBoundary = $"{HeaderNames.ContentRange}: {contentRangeHeaderValue}";

                contentLength += 2 /* -- */ + boundary.Length + contentTypeBoundary.Length + contentRangeBoundary.Length + range.Length + 10 /* 5*2 CRLF*/;
            }

            contentLength += 2 /* -- */ + boundary.Length + 2 /* -- */;
            return contentLength;
        }

        /// <summary>
        /// Parsing request for resuming request to get request ranges.
        /// </summary>
        /// <param name="context">
        /// The action context to access request.
        /// </param>
        /// <param name="contentLength">
        /// The content length to calculate allowed byte ranges.
        /// </param>
        /// <remarks>
        /// See RFC 7233 https://tools.ietf.org/html/rfc7233.
        /// </remarks>
        private IList<ProcessRange> GetRequestRanges(ActionContext context, long contentLength)
        {
            var headerRanges = new List<ProcessRange>();

            // first parse byte ranges header
            if (!RangeHeaderValue.TryParse(context.HttpContext.Request.Headers[HeaderNames.Range], out RangeHeaderValue rangeHeader))
            {
                return headerRanges;
            }

            // only byte ranges
            if (!rangeHeader.Unit.Equals("bytes", StringComparison.OrdinalIgnoreCase))
            {
                return headerRanges;
            }

            // Examples of byte-ranges-specifier values:
            // The first 500 bytes (byte offsets 0-499, inclusive):
            // bytes=0-499
            // The second 500 bytes (byte offsets 500-999, inclusive):
            // bytes=500-999
            // Additional examples, assuming a representation of length 10000:
            // The final 500 bytes (byte offsets 9500-9999, inclusive):
            // bytes=-500
            // bytes=9500-
            // The first and last bytes only (bytes 0 and 9999):
            // bytes=0-0,-1
            // Other valid (but not canonical) specifications of the second 500 bytes (byte offsets 500-999, inclusive):
            // bytes=500-600,601-999
            // bytes=500-700,601-999

            // process parsed bytes to check if in allowed ranges
            foreach (RangeItemHeaderValue range in rangeHeader.Ranges)
            {
                if (range.From.HasValue && range.To.HasValue)
                {
                    if ((range.From < contentLength) && (range.To < contentLength))
                    {
                        // all proper as requested
                        headerRanges.Add(new ProcessRange { From = range.From.Value, To = range.To.Value });
                    }
                    else if (range.From < contentLength)
                    {
                        // in case wrong end, just rest of file
                        headerRanges.Add(new ProcessRange { From = range.From.Value, To = contentLength - 1 });
                    }
                }
                else if (range.From.HasValue)
                {
                    // bytes from to the end
                    headerRanges.Add(new ProcessRange { From = range.From.Value, To = contentLength - 1 });
                }
                else if (range.To.HasValue)
                {
                    // final x bytes
                    headerRanges.Add(new ProcessRange { From = contentLength - range.To.Value, To = contentLength - 1 });
                }
            }

            var index = 0;
            var resultRanges = new List<ProcessRange>();

            // normalize to sort output and merge intersections
            foreach (ProcessRange range in headerRanges.OrderBy(r => r.From))
            {
                if (index == 0)
                {
                    resultRanges.Add(range.Clone());
                }
                else
                {
                    ProcessRange prevRange = resultRanges.Last();

                    if (prevRange.To >= range.From - 1)
                    {
                        // adjust previous range to current end to avoid multiple overlapping ranges.
                        prevRange.To = range.To;
                    }
                    else
                    {
                        resultRanges.Add(range.Clone());
                    }
                }

                index++;
            }

            return resultRanges;
        }

        /// <summary>
        /// The write content to the output.
        /// </summary>
        /// <param name="context">
        /// The action context to access response.
        /// </param>
        /// <param name="inputStream">
        /// The input stream to write.
        /// </param>
        /// <param name="offset">
        /// The offset to be adjusted.
        /// </param>
        /// <param name="count">
        /// The bytes remaining to send.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation Token.
        /// </param>
        private async Task WriteContentAsync(ActionContext context, Stream inputStream, long offset, long? count, CancellationToken cancellationToken)
        {
            HttpResponse response = context.HttpContext.Response;
            Stream outputStream = response.Body;

            if (offset > 0)
            {
                inputStream.Seek(offset, SeekOrigin.Begin);
            }

            await StreamCopyOperation.CopyToAsync(inputStream, outputStream, count, cancellationToken);
        }

        #endregion

        #region Nested Classes

        #region ProcessRange

        /// <summary>
        /// The process range that will be used to stream result.
        /// </summary>
        private class ProcessRange
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets the start byte (inclusion).
            /// </summary>
            public long From { get; set; }

            /// <summary>
            /// Gets the length.
            /// </summary>
            public long Length => (this.To - this.From) + 1;

            /// <summary>
            /// Gets or sets the end byte (inclusion).
            /// </summary>
            public long To { get; set; }

            #endregion

            #region Public Methods

            /// <summary>
            /// Clones current range object.
            /// </summary>
            /// <returns>
            /// New instance of cloned object,.
            /// </returns>
            public ProcessRange Clone()
            {
                return new ProcessRange
                {
                    From = this.From,
                    To = this.To
                };
            }

            /// <summary>
            /// For easier debugging.
            /// </summary>
            public override string ToString()
            {
                return $"{this.From}-{this.To}/{this.Length}";
            }

            #endregion
        }

        #endregion

        #endregion
    }
}