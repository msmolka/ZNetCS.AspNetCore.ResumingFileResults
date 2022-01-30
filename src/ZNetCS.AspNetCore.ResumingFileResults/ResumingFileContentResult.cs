﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingFileContentResult.cs" company="Marcin Smółka">
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
public class ResumingFileContentResult : FileContentResult, IResumingFileResult
{
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ResumingFileContentResult"/> class.
    /// </summary>
    /// <param name="fileContents">
    /// The bytes that represent the file contents.
    /// </param>
    /// <param name="contentType">
    /// The Content-Type header of the response.
    /// </param>
    /// <param name="etag">
    /// The ETag header of the response.
    /// </param>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
    public ResumingFileContentResult(byte[] fileContents, string contentType, string? etag = null)
        : this(fileContents, MediaTypeHeaderValue.Parse(contentType), !string.IsNullOrEmpty(etag) ? EntityTagHeaderValue.Parse(etag) : null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResumingFileContentResult"/> class.
    /// </summary>
    /// <param name="fileContents">
    /// The bytes that represent the file contents.
    /// </param>
    /// <param name="contentType">
    /// The Content-Type header of the response.
    /// </param>
    /// <param name="etag">
    /// The ETag header of the response.
    /// </param>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
    public ResumingFileContentResult(byte[] fileContents, MediaTypeHeaderValue contentType, EntityTagHeaderValue? etag = null) : base(fileContents, contentType)
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

        var executor = context.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<ResumingFileContentResult>>();
        return executor.ExecuteAsync(context, this);
    }

    #endregion
}