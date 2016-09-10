// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalFileTests.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The physical file tests.
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
    /// The physical file tests.
    /// </summary>
    [TestClass]
    public class PhysicalFileTests : FileTestBase
    {
        #region Public Methods

        /// <summary>
        /// The full physical file attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullPhysicalFileAttachmentEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
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
        /// The full physical file attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullPhysicalFileAttachmentNoEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/false");
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
        /// The full physical file inline with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullPhysicalFileInlineEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/false/true");
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
        /// The full physical file inline without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task FullPhysicalFileInlineNoEtagTest()
        {
            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/false/false");
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
        /// The partial first byte physical file attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial1PhysicalFileAttachmentEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=0-0");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
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
        /// The partial first byte physical file attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial1PhysicalFileAttachmentNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=0-0");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/false");
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
        /// The partial second byte physical file attachment with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2PhysicalFileAttachmentEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
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
        /// The partial second byte physical file attachment without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2PhysicalFileAttachmentNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/false");
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
        /// The partial second byte physical file inline with entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2PhysicalFileInlineEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/false/true");
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
        /// The partial second byte physical file inline without entity tag test.
        /// </summary>
        [TestMethod]
        public async Task Partial2PhysicalFileInlineNoEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/false/false");
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