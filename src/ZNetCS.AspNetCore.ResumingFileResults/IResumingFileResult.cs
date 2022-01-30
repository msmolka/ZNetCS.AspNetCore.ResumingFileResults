// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResumingFileResult.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The ResumingFileResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults;

#region Usings

using System.Diagnostics.CodeAnalysis;

#endregion

/// <summary>
/// The interface to support Content-Disposition inline.
/// </summary>
public interface IResumingFileResult
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the file name that will be used in the Content-Disposition attachment header of the response.
    /// </summary>
    [AllowNull]
    string FileDownloadName { get; set; }

    /// <summary>
    /// Gets or sets the file name that will be used in the Content-Disposition inline header of the response.
    /// </summary>
    string? FileInlineName { get; set; }

    #endregion
}