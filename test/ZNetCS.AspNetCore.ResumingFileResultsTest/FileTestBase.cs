// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileTestBase.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The file test base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResultsTest
{
    #region Usings

    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;

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
            var path = Path.GetDirectoryName(typeof(Startup).GetTypeInfo().Assembly.Location);

            // ReSharper disable PossibleNullReferenceException
            var di = new DirectoryInfo(path).Parent.Parent.Parent;

            // Arrange
            this.Server = new TestServer(
                new WebHostBuilder()
                    .UseStartup<Startup>()
                    .UseContentRoot(di.FullName));

            // ReSharper restore PossibleNullReferenceException
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