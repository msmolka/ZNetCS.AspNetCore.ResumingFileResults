// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileStreamTests.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The file stream tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults
{
    #region Usings

    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion

    /// <summary>
    /// The file stream tests.
    /// </summary>
    [TestClass]
    public class FileStreamTests : FileTestBase
    {
        #region Public Methods

        /// <summary>
        /// The full file stream attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullFileStreamAttachmentEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/stream/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual(62, response.Content.Headers.ContentLength, "Content-Length != 62");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The full file stream attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullFileStreamAttachmentNoEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/stream/true/false");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual(62, response.Content.Headers.ContentLength, "Content-Length != 62");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The full file stream inline with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullFileStreamInlineEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/stream/false/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual(62, response.Content.Headers.ContentLength, "Content-Length != 62");
            Assert.AreEqual("inline", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != inline");
        }

        /// <summary>
        /// The full file stream inline without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullFileStreamInlineNoEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/stream/false/false");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual(62, response.Content.Headers.ContentLength, "Content-Length != 62");
            Assert.AreEqual("inline", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != inline");
        }

        /// <summary>
        /// The partial first byte file stream attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial1FileStreamAttachmentEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=0-0");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/stream/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode, "StatusCode != PartialContent");
            Assert.AreEqual("0", responseString, "should be first byte");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 0-0/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 0-0/62");
            Assert.AreEqual(1, response.Content.Headers.ContentLength, "Content-Length != 1");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The partial first byte file stream attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial1FileStreamAttachmentNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=0-0");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/stream/true/false");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode, "StatusCode != PartialContent");
            Assert.AreEqual("0", responseString, "should be first byte");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 0-0/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 0-0/62");
            Assert.AreEqual(1, response.Content.Headers.ContentLength, "Content-Length != 1");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The partial second byte file stream attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2FileStreamAttachmentEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/stream/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode, "StatusCode != PartialContent");
            Assert.AreEqual("1", responseString, "should be second byte");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 1-1/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 1-1/62");
            Assert.AreEqual(1, response.Content.Headers.ContentLength, "Content-Length != 1");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The partial second byte file stream attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2FileStreamAttachmentNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/stream/true/false");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode, "StatusCode != PartialContent");
            Assert.AreEqual("1", responseString, "should be second byte");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 1-1/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 1-1/62");
            Assert.AreEqual(1, response.Content.Headers.ContentLength, "Content-Length != 1");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The partial second byte file stream inline with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2FileStreamInlineEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/stream/false/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode, "StatusCode != PartialContent");
            Assert.AreEqual("1", responseString, "should be second byte");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 1-1/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 1-1/62");
            Assert.AreEqual(1, response.Content.Headers.ContentLength, "Content-Length != 1");
            Assert.AreEqual("inline", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != inline");
        }

        /// <summary>
        /// The partial second byte file stream inline without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2FileStreamInlineNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/stream/false/false");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode, "StatusCode != PartialContent");
            Assert.AreEqual("1", responseString, "should be second byte");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 1-1/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 1-1/62");
            Assert.AreEqual(1, response.Content.Headers.ContentLength, "Content-Length != 1");
            Assert.AreEqual("inline", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != inline");
        }

        #endregion
    }
}