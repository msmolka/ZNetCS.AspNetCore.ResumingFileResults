// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipartTest.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   Testing multipart.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults
{
    #region Usings

    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion

    /// <summary>
    /// Testing multipart.
    /// </summary>
    [TestClass]
    public class MultipartTest : FileTestBase
    {
        #region Public Methods

        /// <summary>
        /// The partial multipart byte physical file attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task PartialMultipartPhysicalFileAttachementEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=0-0,1-4,-3");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            MediaTypeHeaderValue contentType = response.Content.Headers.ContentType;
            string boundary = contentType.Parameters.Single(p => p.Name.Equals("boundary")).Value;

            var expected = new StringBuilder();
            expected.AppendLine($"--{boundary}");
            expected.AppendLine("Content-Type: text/plain");
            expected.AppendLine("Content-Range: bytes 0-4/62");
            expected.AppendLine();
            expected.AppendLine("01234");
            expected.AppendLine($"--{boundary}");
            expected.AppendLine("Content-Type: text/plain");
            expected.AppendLine("Content-Range: bytes 59-61/62");
            expected.AppendLine();
            expected.AppendLine("XYZ");
            expected.AppendLine($"--{boundary}--");

            // Assert
            Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode, "StatusCode != PartialContent");
            Assert.AreEqual(expected.ToString(), responseString, "should be multipart boundary response");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The partial multipart byte virtual file attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task PartialMultipartVirtualFileAttachementEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=0-0,2-4,-3");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/virtual/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            MediaTypeHeaderValue contentType = response.Content.Headers.ContentType;
            string boundary = contentType.Parameters.Single(p => p.Name.Equals("boundary")).Value;

            var expected = new StringBuilder();
            expected.AppendLine($"--{boundary}");
            expected.AppendLine("Content-Type: text/plain");
            expected.AppendLine("Content-Range: bytes 0-0/62");
            expected.AppendLine();
            expected.AppendLine("0");
            expected.AppendLine($"--{boundary}");
            expected.AppendLine("Content-Type: text/plain");
            expected.AppendLine("Content-Range: bytes 2-4/62");
            expected.AppendLine();
            expected.AppendLine("234");
            expected.AppendLine($"--{boundary}");
            expected.AppendLine("Content-Type: text/plain");
            expected.AppendLine("Content-Range: bytes 59-61/62");
            expected.AppendLine();
            expected.AppendLine("XYZ");
            expected.AppendLine($"--{boundary}--");

            // Assert
            Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode, "StatusCode != PartialContent");
            Assert.AreEqual(expected.ToString(), responseString, "should be multipart boundary response");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        #endregion
    }
}