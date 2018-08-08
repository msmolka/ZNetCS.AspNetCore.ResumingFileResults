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
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Net.Http.Headers;

    #endregion

    /// <summary>
    /// The resuming physical file result.
    /// </summary>
    public class ResumingPhysicalFileResult : PhysicalFileResult, IResumingFileResult
    {
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
        public ResumingPhysicalFileResult(string fileName, MediaTypeHeaderValue contentType, EntityTagHeaderValue etag = null) : base(fileName, contentType)
        {
            this.EntityTag = etag;

            // enable range processing by default
            this.EnableRangeProcessing = true;
        }

        #endregion

        #region Public Properties

        /// <inheritdoc/>
        public string FileInlineName { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = context.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<ResumingPhysicalFileResult>>();
            return executor.ExecuteAsync(context, this);
        }

        #endregion
    }
}