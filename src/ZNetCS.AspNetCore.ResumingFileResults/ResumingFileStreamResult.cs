// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingFileStreamResult.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The resuming file stream result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults;

#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

#endregion

/// <summary>
/// The resuming file stream result.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
public class ResumingFileStreamResult : FileStreamResult, IResumingFileResult
{
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
    /// The ETag header of the response.
    /// </param>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
    public ResumingFileStreamResult(Stream fileStream, string contentType, string? etag = null)
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
    /// The ETag header of the response.
    /// </param>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
    public ResumingFileStreamResult(Stream fileStream, MediaTypeHeaderValue contentType, EntityTagHeaderValue? etag = null) : base(fileStream, contentType)
    {
        this.EntityTag = etag;

        // enable range processing by default
        this.EnableRangeProcessing = true;
    }

    #endregion

    #region Public Properties

    /// <inheritdoc/>
    public string? FileInlineName { get; set; }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public override Task ExecuteResultAsync(ActionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var executor = context.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<ResumingFileStreamResult>>();
        return executor.ExecuteAsync(context, this);
    }

    #endregion
}