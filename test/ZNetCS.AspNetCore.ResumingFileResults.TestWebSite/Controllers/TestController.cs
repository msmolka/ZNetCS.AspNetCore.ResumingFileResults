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

    using System;
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
#if NETCOREAPP3_0
        private readonly IWebHostEnvironment hostingEnvironment;
#else
        private readonly IHostingEnvironment hostingEnvironment;
#endif

        /// <summary>
        /// The last modified.
        /// </summary>
        private readonly DateTimeOffset lastModified = new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">
        /// The hosting environment.
        /// </param>
#if NETCOREAPP3_0
        public TestController(IWebHostEnvironment hostingEnvironment)
#else
        public TestController(IHostingEnvironment hostingEnvironment)
#endif
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

            ResumingFileContentResult result = this.ResumingFile(contents, "text/plain", fileName ? "TestFile.txt" : null, etag ? EntityTag : null);
            result.LastModified = this.lastModified;
            return result;
        }

        /// <summary>
        /// The file contents.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpGet("contents/{fileName}")]
        public IActionResult FileContents(bool fileName)
        {
            string webRoot = this.hostingEnvironment.WebRootPath;
            var contents = System.IO.File.ReadAllBytes(Path.Combine(webRoot, "TestFile.txt"));

            var result = new ResumingFileContentResult(contents, "text/plain")
            {
                FileInlineName = "TestFile.txt",
                LastModified = this.lastModified
            };
            return result;
        }

        /// <summary>
        /// The file head.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpHead("file")]
        public IActionResult FileHead()
        {
            ResumingVirtualFileResult result = this.ResumingFile("TestFile.txt", "text/plain", "TestFile.txt", EntityTag);
            result.LastModified = this.lastModified;
            return result;
        }

        /// <summary>
        /// The file put.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpPut("file")]
        public IActionResult FilePut()
        {
            ResumingVirtualFileResult result = this.ResumingFile("TestFile.txt", "text/plain", "TestFile.txt", EntityTag);
            result.LastModified = this.lastModified;
            return result;
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

            ResumingFileStreamResult result = this.ResumingFile(stream, "text/plain", fileName ? "TestFile.txt" : null, etag ? EntityTag : null);
            result.LastModified = this.lastModified;
            return result;
        }

        /// <summary>
        /// The file stream.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpGet("stream/{fileName}")]
        public IActionResult FileStream(bool fileName)
        {
            string webRoot = this.hostingEnvironment.WebRootPath;
            FileStream stream = System.IO.File.OpenRead(Path.Combine(webRoot, "TestFile.txt"));

            var result = new ResumingFileStreamResult(stream, "text/plain")
            {
                FileInlineName = "TestFile.txt",
                LastModified = this.lastModified
            };

            return result;
        }

        /// <summary>
        /// The hello action.
        /// </summary>
        [HttpGet("hello")]
        public IActionResult Hello() => this.Content("Hello");

        /// <summary>
        /// The physical file.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpGet("physical/{fileName}/{etag}")]
        public IActionResult PhysicalFile(bool fileName, bool etag)
        {
            string webRoot = this.hostingEnvironment.WebRootPath;

            ResumingPhysicalFileResult result = this.ResumingPhysicalFile(
                Path.Combine(webRoot, "TestFile.txt"),
                "text/plain",
                fileName ? "TestFile.txt" : null,
                etag ? EntityTag : null);
            result.LastModified = this.lastModified;
            return result;
        }

        /// <summary>
        /// The physical file.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpGet("physical/{fileName}")]
        public IActionResult PhysicalFile(bool fileName)
        {
            string webRoot = this.hostingEnvironment.WebRootPath;

            var result = new ResumingPhysicalFileResult(Path.Combine(webRoot, "TestFile.txt"), "text/plain")
            {
                FileInlineName = "TestFile.txt",
                LastModified = this.lastModified
            };

            return result;
        }

        /// <summary>
        /// The virtual file.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpGet("virtual/{fileName}/{etag}")]
        public IActionResult VirtualFile(bool fileName, bool etag)
        {
            ResumingVirtualFileResult result = this.ResumingFile("TestFile.txt", "text/plain", fileName ? "TestFile.txt" : null, etag ? EntityTag : null);
            result.LastModified = this.lastModified;
            return result;
        }

        /// <summary>
        /// The virtual file.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "OK")]
        [HttpGet("virtual/{fileName}")]
        public IActionResult VirtualFile(bool fileName)
        {
            var result = new ResumingVirtualFileResult("TestFile.txt", "text/plain")
            {
                FileInlineName = "TestFile.txt",
                LastModified = this.lastModified
            };
            return result;
        }

        #endregion
    }
}