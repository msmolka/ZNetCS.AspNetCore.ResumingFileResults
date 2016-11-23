// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingPhysicalFileResult.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The resuming physical file result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults
{
    #region Usings

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Net.Http.Headers;

    using ZNetCS.AspNetCore.ResumingFileResults.Infrastructure;

    #endregion

    /// <summary>
    /// The resuming physical file result.
    /// </summary>
    public class ResumingPhysicalFileResult : ResumingFileResult
    {
        #region Fields

        /// <summary>
        /// The file name.
        /// </summary>
        private string fileName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingPhysicalFileResult"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The path to the file. The path must be an absolute path.
        /// </param>
        /// <param name="contentType">
        /// The Content-Type header of the response.
        /// </param>
        /// <param name="etag">
        /// The ETag header of the response.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
        public ResumingPhysicalFileResult(string fileName, string contentType, string etag = null)
            : this(fileName, MediaTypeHeaderValue.Parse(contentType), !string.IsNullOrEmpty(etag) ? EntityTagHeaderValue.Parse(etag) : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingPhysicalFileResult"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The path to the file. The path must be an absolute path.
        /// </param>
        /// <param name="contentType">
        /// The Content-Type header of the response.
        /// </param>
        /// <param name="etag">
        /// The ETag header of the response.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
        public ResumingPhysicalFileResult(string fileName, MediaTypeHeaderValue contentType, EntityTagHeaderValue etag = null) : base(contentType?.ToString(), etag)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            this.FileName = fileName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the path to the file that will be sent back as the response.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.fileName = value;
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

            var executor = context.HttpContext.RequestServices.GetRequiredService<ResumingPhysicalFileResultExecutor>();
            return executor.ExecuteAsync(context, this);
        }

        #endregion
    }
}