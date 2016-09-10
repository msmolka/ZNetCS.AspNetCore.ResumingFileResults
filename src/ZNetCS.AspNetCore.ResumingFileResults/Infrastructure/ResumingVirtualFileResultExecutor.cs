// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingVirtualFileResultExecutor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The resuming virtual file result executor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults.Infrastructure
{
    #region Usings

    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Logging;

    #endregion

    /// <summary>
    /// The resuming virtual file result executor.
    /// </summary>
    public class ResumingVirtualFileResultExecutor : ResumingFileResultExecutorBase
    {
        #region Fields

        /// <summary>
        /// The hosting environment.
        /// </summary>
        private readonly IHostingEnvironment hostingEnvironment;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingVirtualFileResultExecutor"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The logger factory.
        /// </param>
        /// <param name="hostingEnvironment">
        /// The hosting environment.
        /// </param>
        public ResumingVirtualFileResultExecutor(ILoggerFactory loggerFactory, IHostingEnvironment hostingEnvironment)
            : base(CreateLogger<ResumingVirtualFileResultExecutor>(loggerFactory))
        {
            if (hostingEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostingEnvironment));
            }

            this.hostingEnvironment = hostingEnvironment;
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
        /// The action result to process.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to support cancellation.
        /// </param>
        public Task ExecuteAsync(ActionContext context, ResumingVirtualFileResult result, CancellationToken cancellationToken = default(CancellationToken))
        {
            IFileProvider fileProvider = this.GetFileProvider(result);

            string normalizedPath = result.FileName;
            if (normalizedPath.StartsWith("~", StringComparison.Ordinal))
            {
                normalizedPath = normalizedPath.Substring(1);
            }

            IFileInfo fileInfo = fileProvider.GetFileInfo(normalizedPath);
            if (fileInfo.Exists)
            {
                return this.ExecuteAsync(context, this.GetFileStream(fileInfo), result, cancellationToken);
            }

            throw new FileNotFoundException($"Could not find file: {result.FileName}");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets read file stream from file info.
        /// </summary>
        /// <param name="fileInfo">
        /// The file info to open stream.
        /// </param>
        protected virtual Stream GetFileStream(IFileInfo fileInfo)
        {
            return fileInfo.CreateReadStream();
        }

        /// <summary>
        /// Gets file provider related to result or current hosting environment.
        /// </summary>
        /// <param name="result">
        /// The result to find provider.
        /// </param>
        private IFileProvider GetFileProvider(ResumingVirtualFileResult result)
        {
            if (result.FileProvider != null)
            {
                return result.FileProvider;
            }

            result.FileProvider = this.hostingEnvironment.WebRootFileProvider;
            return result.FileProvider;
        }

        #endregion
    }
}