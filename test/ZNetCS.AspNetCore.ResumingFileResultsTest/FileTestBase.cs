// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileTestBase.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The file test base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResultsTest;

#region Usings

using System;
using System.Net.Http;
using System.Net.Http.Headers;

#endregion

/// <summary>
/// The file test base.
/// </summary>
public abstract class FileTestBase : IDisposable
{
    #region Fields

    private readonly TestWebApplicationFactory factory;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="FileTestBase"/> class.
    /// </summary>
    protected FileTestBase()
    {
        this.factory = new TestWebApplicationFactory();
        this.Client = this.factory.CreateClient();
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the client.
    /// </summary>
    protected HttpClient Client { get; }

    /// <summary>
    /// Gets the entity tag.
    /// </summary>
    protected EntityTagHeaderValue EntityTag { get; } = new("\"TestFile\"");

    /// <summary>
    /// Gets the last modified.
    /// </summary>
    protected DateTimeOffset LastModified { get; } = new(2016, 1, 1, 0, 0, 0, TimeSpan.Zero);

    #endregion

    #region Implemented Interfaces

    #region IDisposable

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The disposing method.
    /// </summary>
    /// <param name="disposing">The disposing.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.Client.Dispose();
            this.factory.Dispose();
        }
    }

    #endregion
}