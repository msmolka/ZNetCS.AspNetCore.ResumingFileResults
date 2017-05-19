// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingFileResult.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The resuming file result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults
{
    #region Usings

    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;

    #endregion

    /// <summary>
    /// The resuming file result class.
    /// </summary>
    public abstract class ResumingFileResult : FileResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingFileResult"/> class.
        /// </summary>
        /// <param name="contentType">
        /// The Content-Type header of the response.
        /// </param>
        protected ResumingFileResult(string contentType) : base(contentType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingFileResult"/> class.
        /// </summary>
        /// <param name="contentType">
        /// The Content-Type header of the response.
        /// </param>
        /// <param name="etag">
        /// The ETag header of the response.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
        protected ResumingFileResult(string contentType, string etag = null) : this(contentType, !string.IsNullOrEmpty(etag) ? EntityTagHeaderValue.Parse(etag) : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingFileResult"/> class.
        /// </summary>
        /// <param name="contentType">
        /// The Content-Type header of the response.
        /// </param>
        /// <param name="etag">
        /// The ETag header of the response.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
        protected ResumingFileResult(string contentType, EntityTagHeaderValue etag = null) : base(contentType)
        {
            this.EntityTag = etag;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the entity tag.
        /// </summary>
        public EntityTagHeaderValue EntityTag { get; set; }

        /// <summary>
        /// Gets or sets the last modified date.
        /// </summary>
        public DateTimeOffset? LastModified { get; set; }

        /// <summary>
        /// Gets or sets the file name that will be used in the Content-Disposition inline header of the response.
        /// </summary>
        public string FileInlineName { get; set; } = string.Empty;

        #endregion
    }
}