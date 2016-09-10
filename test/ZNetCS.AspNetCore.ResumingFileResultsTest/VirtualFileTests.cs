// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualFileTests.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The virtual file tests.
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
    /// The virtual file tests.
    /// </summary>
    [TestClass]
    public class VirtualFileTests : FileTestBase
    {
        #region Public Methods

        /// <summary>
        /// The full virtual file attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullVirtualFileAttachmentEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/virtual/true/true");
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
        /// The full virtual file attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullVirtualFileAttachmentNoEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/virtual/true/false");
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
        /// The full virtual file inline with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullVirtualFileInlineEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/virtual/false/true");
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
        /// The full virtual file inline without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullVirtualFileInlineNoEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/virtual/false/false");
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
        /// The partial first byte virtual file attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial1VirtualFileAttachmentEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=0-0");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/virtual/true/true");
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
        /// The partial first byte virtual file attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial1VirtualFileAttachmentNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=0-0");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/virtual/true/false");
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
        /// The partial second byte virtual file attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2VirtualFileAttachmentEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/virtual/true/true");
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
        /// The partial second byte virtual file attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2VirtualFileAttachmentNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/virtual/true/false");
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
        /// The partial second byte virtual file inline with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2VirtualFileInlineEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/virtual/false/true");
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
        /// The partial second byte virtual file inline without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2VirtualFileInlineNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/virtual/false/false");
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