// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestController.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The test controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults.TestWebSite.Controllers
{
    #region Usings

    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    using ZNetCS.AspNetCore.ResumingFileResults.Extensions;

    #endregion

    /// <summary>
    /// The test controller.
    /// </summary>
    [Route("test/file")]
    public class TestController : Controller
    {
        #region Constants

        /// <summary>
        /// The entity tag.
        /// </summary>
        private const string EntityTag = "\"TestFile\"";

        #endregion

        #region Fields

        /// <summary>
        /// The hosting environment.
        /// </summary>
        private readonly IHostingEnvironment hostingEnvironment;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">
        /// The hosting environment.
        /// </param>
        public TestController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The file contents.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpGet("contents/{fileName}/{etag}")]
        public IActionResult FileContents(bool fileName, bool etag)
        {
            string webRoot = this.hostingEnvironment.WebRootPath;
            var contents = System.IO.File.ReadAllBytes(Path.Combine(webRoot, "TestFile.txt"));

            return this.ResumingFile(contents, "text/plain", fileName ? "TestFile.txt" : null, etag ? EntityTag : null);
        }

        /// <summary>
        /// The file stream.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpGet("stream/{fileName}/{etag}")]
        public IActionResult FileStream(bool fileName, bool etag)
        {
            string webRoot = this.hostingEnvironment.WebRootPath;
            FileStream stream = System.IO.File.OpenRead(Path.Combine(webRoot, "TestFile.txt"));
            return this.ResumingFile(stream, "text/plain", fileName ? "TestFile.txt" : null, etag ? EntityTag : null);
        }

        /// <summary>
        /// The hello action.
        /// </summary>
        [HttpGet("hello")]
        public IActionResult Hello()
        {
            return this.Content("Hello");
        }

        /// <summary>
        /// The physical file.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpGet("physical/{fileName}/{etag}")]
        public IActionResult PhysicalFile(bool fileName, bool etag)
        {
            string webRoot = this.hostingEnvironment.WebRootPath;
            return this.ResumingPhysicalFile(Path.Combine(webRoot, "TestFile.txt"), "text/plain", fileName ? "TestFile.txt" : null, etag ? EntityTag : null);
        }

        /// <summary>
        /// The virtual file.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpGet("virtual/{fileName}/{etag}")]
        public IActionResult VirtualFile(bool fileName, bool etag)
        {
            return this.ResumingFile("TestFile.txt", "text/plain", fileName ? "TestFile.txt" : null, etag ? EntityTag : null);
        }

        #endregion
    }
}