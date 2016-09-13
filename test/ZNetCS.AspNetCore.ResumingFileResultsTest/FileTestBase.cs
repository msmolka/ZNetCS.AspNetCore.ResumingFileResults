// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileTestBase.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The file test base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults
{
    #region Usings

    using System;
    using System.IO;
    using System.Net.Http;

    // TODO: Replace with Microsoft headers ones new version of TestServer is released.
    using System.Net.Http.Headers;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;

    using ZNetCS.AspNetCore.ResumingFileResults.TestWebSite;

    #endregion

    /// <summary>
    /// The file test base.
    /// </summary>
    public abstract class FileTestBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTestBase"/> class.
        /// </summary>
        protected FileTestBase()
        {
            // Arrange
            this.Server = new TestServer(
                new WebHostBuilder()
                    .UseStartup<Startup>()
                    .UseContentRoot(Directory.GetCurrentDirectory()));

            this.Client = this.Server.CreateClient();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the client.
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// Gets the entity tag.
        /// </summary>
        public EntityTagHeaderValue EntityTag { get; } = new EntityTagHeaderValue("\"TestFile\"");

        /// <summary>
        /// Gets the last modified.
        /// </summary>
        public DateTimeOffset LastModified { get; } = new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero);

        /// <summary>
        /// Gets the server.
        /// </summary>
        public TestServer Server { get; }

        #endregion
    }
}