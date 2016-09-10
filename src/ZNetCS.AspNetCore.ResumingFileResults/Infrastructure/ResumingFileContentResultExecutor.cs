// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingFileContentResultExecutor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The resuming file result executor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults.Infrastructure
{
    #region Usings

    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    #endregion

    /// <summary>
    /// The resuming file contents result executor.
    /// </summary>
    public class ResumingFileContentResultExecutor : ResumingFileResultExecutorBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingFileContentResultExecutor"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The logger factory.
        /// </param>
        public ResumingFileContentResultExecutor(ILoggerFactory loggerFactory) : base(CreateLogger<ResumingFileContentResultExecutor>(loggerFactory))
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
        public Task ExecuteAsync(ActionContext context, ResumingFileContentResult result, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.ExecuteAsync(context, new MemoryStream(result.FileContents), result, cancellationToken);
        }

        #endregion
    }
}