// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingPhysicalFileResultExecutor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The resuming physical file result executor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults.Infrastructure
{
    #region Usings

    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    #endregion

    /// <summary>
    /// The resuming physical file result executor.
    /// </summary>
    public class ResumingPhysicalFileResultExecutor : ResumingFileResultExecutorBase
    {
        #region Constants

        /// <summary>
        /// The default buffer size.
        /// </summary>
        private const int DefaultBufferSize = 0x1000;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingPhysicalFileResultExecutor"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The logger factory.
        /// </param>
        public ResumingPhysicalFileResultExecutor(ILoggerFactory loggerFactory) : base(CreateLogger<ResumingPhysicalFileResultExecutor>(loggerFactory))
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
        /// The action result to process.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to support cancellation.
        /// </param>
        public Task ExecuteAsync(ActionContext context, ResumingPhysicalFileResult result, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!Path.IsPathRooted(result.FileName))
            {
                throw new NotSupportedException($"Path '{result.FileName}' was not rooted.");
            }

            if (File.Exists(result.FileName))
            {
                return this.ExecuteAsync(context, this.GetFileStream(result.FileName), result, cancellationToken);
            }

            throw new FileNotFoundException($"Could not find file: {result.FileName}");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets read file stream from file info.
        /// </summary>
        /// <param name="path">
        /// The path to the file.
        /// </param>
        protected virtual Stream GetFileStream(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, DefaultBufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
        }

        #endregion
    }
}