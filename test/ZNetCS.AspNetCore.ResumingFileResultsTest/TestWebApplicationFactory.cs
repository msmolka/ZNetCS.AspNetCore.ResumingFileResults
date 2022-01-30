// -----------------------------------------------------------------------
// <copyright file="TestWebApplicationFactory.cs" company="Marcin Smółka">
// Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResultsTest;

#region Usings

using System.IO;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#endregion

/// <inheritdoc/>
internal class TestWebApplicationFactory : WebApplicationFactory<EmptyStartup>
{
    #region Methods

    /// <inheritdoc/>
    protected override IHostBuilder CreateHostBuilder() => Host.CreateDefaultBuilder().ConfigureWebHostDefaults(_ => { });

    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(GetPath() ?? string.Empty);
        builder.Configure(
            app =>
            {
                app.UseRouting();
                app.UseEndpoints(o => o.MapControllers());
            });
        builder.ConfigureServices(
            s =>
            {
                s.AddControllers();
                s.AddResumingFileResult();
            });
    }

    /// <summary>
    /// Get root path for test web server.
    /// </summary>
    private static string? GetPath()
    {
        string path = Path.GetDirectoryName(typeof(EmptyStartup).GetTypeInfo().Assembly.Location)!;

        // ReSharper disable PossibleNullReferenceException
        DirectoryInfo? di = new DirectoryInfo(path).Parent?.Parent?.Parent;

        return di?.FullName;
    }

    #endregion
}