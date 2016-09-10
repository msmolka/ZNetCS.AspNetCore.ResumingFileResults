// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileContentsTests.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The file contents tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResults
{
    #region Usings

    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion

    /// <summary>
    /// The file contents tests.
    /// </summary>
    [TestClass]
    public class FileContentsTests : FileTestBase
    {
        #region Public Methods

        /// <summary>
        /// The full file contents attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullFileContentsAttachmentEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/contents/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.IsNotNull(response.Headers.AcceptRanges, "AcceptRanges != null");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The full file contents attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullFileContentsAttachmentNoEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/contents/true/false");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.IsNotNull(response.Headers.AcceptRanges, "AcceptRanges != null");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The full file contents inline with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullFileContentsInlineEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/contents/false/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.IsNotNull(response.Headers.AcceptRanges, "AcceptRanges != null");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("inline", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != inline");
        }

        /// <summary>
        /// The full file contents inline without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullFileContentsInlineNoEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/contents/false/false");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.IsNotNull(response.Headers.AcceptRanges, "AcceptRanges != null");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("inline", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != inline");
        }

        /// <summary>
        /// The partial first byte file contents attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial1FileContentsAttachmentEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=0-0");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/contents/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual("0", responseString, "should be first byte");
            Assert.IsNotNull(response.Headers.AcceptRanges, "AcceptRanges != null");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 0-0/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 0-0/62");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The partial first byte file contents attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial1FileContentsAttachmentNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=0-0");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/contents/true/false");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual("0", responseString, "should be first byte");
            Assert.IsNotNull(response.Headers.AcceptRanges, "AcceptRanges != null");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 0-0/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 0-0/62");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The partial second byte file contents attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2FileContentsAttachmentEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/contents/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual("1", responseString, "should be second byte");
            Assert.IsNotNull(response.Headers.AcceptRanges, "AcceptRanges != null");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 1-1/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 1-1/62");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The partial second byte file contents attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2FileContentsAttachmentNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/contents/true/false");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual("1", responseString, "should be second byte");
            Assert.IsNotNull(response.Headers.AcceptRanges, "AcceptRanges != null");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 1-1/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 1-1/62");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The partial second byte file contents inline with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2FileContentsInlineEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/contents/false/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual("1", responseString, "should be second byte");
            Assert.IsNotNull(response.Headers.AcceptRanges, "AcceptRanges != null");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 1-1/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 1-1/62");
            Assert.AreEqual("inline", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != inline");
        }

        /// <summary>
        /// The partial second byte file contents inline without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2FileContentsInlineNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/contents/false/false");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual("1", responseString, "should be second byte");
            Assert.IsNotNull(response.Headers.AcceptRanges, "AcceptRanges != null");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 1-1/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 1-1/62");
            Assert.AreEqual("inline", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != inline");
        }

        #endregion
    }
}