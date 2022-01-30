﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The service collection extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

#region Usings

using System;

using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

using ZNetCS.AspNetCore.ResumingFileResults;
using ZNetCS.AspNetCore.ResumingFileResults.Infrastructure;

#endregion

/// <summary>
/// The service collection extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    #region Public Methods

    /// <summary>
    /// Adds resuming file result services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add services to.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddResumingFileResult(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddSingleton<IActionResultExecutor<ResumingPhysicalFileResult>, ResumingPhysicalFileResultExecutor>();
        services.TryAddSingleton<IActionResultExecutor<ResumingVirtualFileResult>, ResumingVirtualFileResultExecutor>();
        services.TryAddSingleton<IActionResultExecutor<ResumingFileStreamResult>, ResumingFileStreamResultExecutor>();
        services.TryAddSingleton<IActionResultExecutor<ResumingFileContentResult>, ResumingFileContentResultExecutor>();

        return services;
    }

    #endregion
}