// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumingFileContentResultExecutor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The resuming file contents result executor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults.Infrastructure
{
    #region Usings

    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Internal;
    using Microsoft.Extensions.Logging;

    #endregion

    /// <summary>
    /// The resuming file contents result executor.
    /// </summary>
    public class ResumingFileContentResultExecutor : FileContentResultExecutor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumingFileContentResultExecutor"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The logger factory.
        /// </param>
        public ResumingFileContentResultExecutor(ILoggerFactory loggerFactory) : base(loggerFactory)
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
        public virtual Task ExecuteAsync(ActionContext context, ResumingFileContentResult result)
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