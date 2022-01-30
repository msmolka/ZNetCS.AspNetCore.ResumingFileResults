// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingFileHelper.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The resuming file helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults;

#region Usings

using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

#endregion

/// <summary>
/// The resuming file helper.
/// </summary>
public static class ResumingFileHelper
{
    #region Public Methods

    /// <summary>
    /// Sets content disposition as inline when there is no file download name specified.
    /// </summary>
    /// <param name="context">
    /// The action context.
    /// </param>
    /// <param name="result">
    /// The <see cref="IResumingFileResult"/>.
    /// </param>
    public static void SetContentDispositionHeaderInline(ActionContext context, IResumingFileResult result)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (!string.IsNullOrEmpty(result.FileDownloadName))
        {
            return;
        }

        var contentDisposition = new ContentDispositionHeaderValue("inline");

        if (!string.IsNullOrWhiteSpace(result.FileInlineName))
        {
            contentDisposition.SetHttpFileName(result.FileInlineName);
        }

        context.HttpContext.Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
    }

    #endregion
}