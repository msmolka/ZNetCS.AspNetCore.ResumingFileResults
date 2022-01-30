﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingPhysicalFileResultExecutor.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The resuming physical file result executor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults.Infrastructure;

#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

#endregion

/// <summary>
/// The resuming physical file result executor.
/// </summary>
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Public API")]
public class ResumingPhysicalFileResultExecutor : PhysicalFileResultExecutor, IActionResultExecutor<ResumingPhysicalFileResult>
{
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ResumingPhysicalFileResultExecutor"/> class.
    /// </summary>
    /// <param name="loggerFactory">
    /// The logger factory.
    /// </param>
    public ResumingPhysicalFileResultExecutor(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Executes context asynchronously.
    /// </summary>
    /// <param name="context">
    /// The action context to access request and response.
    /// </param>
    /// <param name="result">
    /// The file result to process.
    /// </param>
    public virtual Task ExecuteAsync(ActionContext context, ResumingPhysicalFileResult result)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        ResumingFileHelper.SetContentDispositionHeaderInline(context, result);

        return base.ExecuteAsync(context, result);
    }

    #endregion
}