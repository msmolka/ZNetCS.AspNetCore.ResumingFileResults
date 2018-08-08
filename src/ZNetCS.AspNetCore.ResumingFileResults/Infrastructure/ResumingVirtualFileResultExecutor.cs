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
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;

    #endregion

    /// <summary>
    /// The resuming virtual file result executor.
    /// </summary>
    public class ResumingVirtualFileResultExecutor : VirtualFileResultExecutor, IActionResultExecutor<ResumingVirtualFileResult>
    {
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
        public ResumingVirtualFileResultExecutor(ILoggerFactory loggerFactory, IHostingEnvironment hostingEnvironment) : base(loggerFactory, hostingEnvironment)
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
        public virtual Task ExecuteAsync(ActionContext context, ResumingVirtualFileResult result)
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
}