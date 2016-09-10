// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingFileStreamResult.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The resuming file stream result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults
{
    #region Usings

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Net.Http.Headers;

    using ZNetCS.AspNetCore.ResumingFileResults.Infrastructure;

    #endregion

    /// <summary>
    /// The resuming file stream result.
    /// </summary>
    public class ResumingFileStreamResult : ResumingFileResult
    {
        #region Fields

        /// <summary>
        /// The file stream.
        /// </summary>
        private Stream fileStream;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingFileStreamResult"/> class.
        /// </summary>
        /// <param name="fileStream">
        /// The stream with the file.
        /// </param>
        /// <param name="contentType">
        /// The Content-Type header of the response.
        /// </param>
        /// <param name="etag">
        /// The Etag header of the response.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
        public ResumingFileStreamResult(Stream fileStream, string contentType, string etag = null)
            : this(fileStream, MediaTypeHeaderValue.Parse(contentType), !string.IsNullOrEmpty(etag) ? EntityTagHeaderValue.Parse(etag) : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingFileStreamResult"/> class.
        /// </summary>
        /// <param name="fileStream">
        /// The stream with the file.
        /// </param>
        /// <param name="contentType">
        /// The Content-Type header of the response.
        /// </param>
        /// <param name="etag">
        /// The Etag header of the response.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
        public ResumingFileStreamResult(Stream fileStream, MediaTypeHeaderValue contentType, EntityTagHeaderValue etag = null) : base(contentType?.ToString(), etag)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            this.FileStream = fileStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the stream with the file that will be sent back as the response.
        /// </summary>
        public Stream FileStream
        {
            get
            {
                return this.fileStream;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.fileStream = value;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = context.HttpContext.RequestServices.GetRequiredService<ResumingFileStreamResultExecutor>();
            return executor.ExecuteAsync(context, this);
        }

        #endregion
    }
}