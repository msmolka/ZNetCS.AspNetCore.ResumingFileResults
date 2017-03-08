// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreconditionTests.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The precondition tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.ResumingFileResultsTest
{
    #region Usings

    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Net.Http.Headers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion

    /// <summary>
    /// The precondition tests.
    /// </summary>
    [TestClass]
    public class PreconditionTests : FileTestBase
    {
        #region Public Methods

        /// <summary>
        /// The precondition if match fail test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfMatchFailTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Match", "\"xyzzy\"");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PreconditionFailed, response.StatusCode, "StatusCode != PreconditionFailed");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if match empty fail test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfMatchEmptyFailTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Match", "\"xyzzy\"");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/false");

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PreconditionFailed, response.StatusCode, "StatusCode != PreconditionFailed");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if match fail weak test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfMatchFailWeakTest()
        {
            // Arrange
            string entityTag = this.EntityTag.ToString();
            var tmpNewEntityTag = new EntityTagHeaderValue(entityTag, true);
            this.Client.DefaultRequestHeaders.Add("If-Match", tmpNewEntityTag.ToString());

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PreconditionFailed, response.StatusCode, "StatusCode != PreconditionFailed");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if match success any test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfMatchSuccessAnyTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Match", EntityTagHeaderValue.Any.ToString());

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if match success test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfMatchSuccessTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Match", this.EntityTag.ToString());

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if modified since fail equal test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfModifiedSinceFailEqualTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Modified-Since", HeaderUtilities.FormatDate(this.LastModified));

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.NotModified, response.StatusCode, "StatusCode != PreconditionFailed");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if modified since fail equal test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfModifiedSinceFailLessTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Modified-Since", HeaderUtilities.FormatDate(this.LastModified.AddSeconds(1)));

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.NotModified, response.StatusCode, "StatusCode != NotModified");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if modified since if none match success test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfModifiedSinceIfNoneMatchSuccessTest()
        {
            // Arrange
            var tmpNewEntityTag = new EntityTagHeaderValue("\"xyzzy\"", true);
            this.Client.DefaultRequestHeaders.Add("If-None-Match", tmpNewEntityTag.ToString());
            this.Client.DefaultRequestHeaders.Add("If-Modified-Since", HeaderUtilities.FormatDate(this.LastModified.AddSeconds(1)));

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if modified since success less test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfModifiedSinceSuccessLessTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Modified-Since", HeaderUtilities.FormatDate(this.LastModified.AddSeconds(-1)));

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if modified since success less test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfModifiedSinceSuccessPutTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Modified-Since", HeaderUtilities.FormatDate(this.LastModified.AddSeconds(-1)));

            // Act
            HttpResponseMessage response = await this.Client.PutAsync("/test/file/file", new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if none match fail any get test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfNoneMatchFailAnyGetTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-None-Match", EntityTagHeaderValue.Any.ToString());

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.NotModified, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if none match fail any put test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfNoneMatchFailAnyPutTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-None-Match", EntityTagHeaderValue.Any.ToString());

            // Act
            HttpResponseMessage response = await this.Client.PutAsync("/test/file/file", new StringContent(string.Empty));

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PreconditionFailed, response.StatusCode, "StatusCode != PreconditionFailed");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if none match get fail test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfNoneMatchGetFailTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-None-Match", this.EntityTag.ToString());

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.NotModified, response.StatusCode, "StatusCode != NotModified");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if none match get fail weak test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfNoneMatchGetFailWeakTest()
        {
            // Arrange
            string entityTag = this.EntityTag.ToString();
            var tmpNewEntityTag = new EntityTagHeaderValue(entityTag, true);
            this.Client.DefaultRequestHeaders.Add("If-None-Match", tmpNewEntityTag.ToString());

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.NotModified, response.StatusCode, "StatusCode != NotModified");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if none match head fail test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfNoneMatchHeadFailTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-None-Match", this.EntityTag.ToString());

            // Act
            HttpResponseMessage response = await this.Client.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/test/file/file"));

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.NotModified, response.StatusCode, "StatusCode != NotModified");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if none match put fail test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfNoneMatchPutFailTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-None-Match", this.EntityTag.ToString());

            // Act
            HttpResponseMessage response = await this.Client.PutAsync("/test/file/file", new StringContent(string.Empty));

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PreconditionFailed, response.StatusCode, "StatusCode != PreconditionFailed");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if none match success test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfNoneMatchSuccessTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-None-Match", "\"xyzzy\"");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if none match success weak test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfNoneMatchSuccessWeakTest()
        {
            // Arrange
            var tmpNewEntityTag = new EntityTagHeaderValue("\"xyzzy\"", true);
            this.Client.DefaultRequestHeaders.Add("If-None-Match", tmpNewEntityTag.ToString());

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if range correct entity test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfRangeEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");
            this.Client.DefaultRequestHeaders.Add("If-Range", this.EntityTag.ToString());

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode, "StatusCode != PartialContent");
            Assert.AreEqual("1", responseString, "should be second byte");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 1-1/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 1-1/62");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if range correct entity empty test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfRangeIgnoreEtagEmptyTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");
            this.Client.DefaultRequestHeaders.Add("If-Range", this.EntityTag.ToString());

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/false");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.IsNull(response.Headers.ETag, "ETag != null");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if range ignore entity test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfRangeIgnoreEtagTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");
            this.Client.DefaultRequestHeaders.Add("If-Range", "\"xyzzy\"");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if range ignore test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfRangeIgnoreTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Range", "\"xyzzy\"");

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if range correct entity weak ignore test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfRangeIgnoreWeakEtagTest()
        {
            // Arrange
            var tmpNewEntityTag = new EntityTagHeaderValue(this.EntityTag.ToString(), true);
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");
            this.Client.DefaultRequestHeaders.Add("If-Range", tmpNewEntityTag.ToString());

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if range correct entity test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfRangeLastModifiedEqualTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");
            this.Client.DefaultRequestHeaders.Add("If-Range", HeaderUtilities.FormatDate(this.LastModified));

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode, "StatusCode != PartialContent");
            Assert.AreEqual("1", responseString, "should be second byte");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 1-1/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 1-1/62");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if range correct entity weak ignore test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfRangeLastModifiedIgnoreTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");
            this.Client.DefaultRequestHeaders.Add("If-Range", HeaderUtilities.FormatDate(this.LastModified.AddSeconds(-1)));

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if range correct entity less test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfRangeLastModifiedLessTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("Range", "bytes=1-1");
            this.Client.DefaultRequestHeaders.Add("If-Range", HeaderUtilities.FormatDate(this.LastModified.AddSeconds(1)));

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            // Assert
            Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode, "StatusCode != PartialContent");
            Assert.AreEqual("1", responseString, "should be second byte");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNotNull(response.Content.Headers.ContentRange, "Content-Range == null");
            Assert.AreEqual("bytes 1-1/62", response.Content.Headers.ContentRange.ToString(), "Content-Range != bytes 1-1/62");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if unmodified since fail test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfUnmodifiedSinceFailTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Unmodified-Since", HeaderUtilities.FormatDate(this.LastModified.AddSeconds(-1)));

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.PreconditionFailed, response.StatusCode, "StatusCode != PreconditionFailed");
            Assert.AreEqual(string.Empty, responseString, "response is not empty");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if unmodified since if match success test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfUnmodifiedSinceIfMatchSuccessTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Match", this.EntityTag.ToString());
            this.Client.DefaultRequestHeaders.Add("If-Unmodified-Since", HeaderUtilities.FormatDate(this.LastModified.AddSeconds(-1)));

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if unmodified since success equal test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfUnmodifiedSinceSuccessEqualTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Unmodified-Since", HeaderUtilities.FormatDate(this.LastModified));

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        /// <summary>
        /// The precondition if unmodified since success less test.
        /// </summary>
        [TestMethod]
        public async Task PreconditionIfUnmodifiedSinceSuccessLessTest()
        {
            // Arrange
            this.Client.DefaultRequestHeaders.Add("If-Unmodified-Since", HeaderUtilities.FormatDate(this.LastModified.AddSeconds(1)));

            // Act
            HttpResponseMessage response = await this.Client.GetAsync("/test/file/physical/true/true");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
            Assert.AreEqual("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ", responseString, "no full file");
            Assert.AreEqual("bytes", response.Headers.AcceptRanges.ToString(), "AcceptRanges != bytes");
            Assert.AreEqual(this.EntityTag, response.Headers.ETag, "ETag != EntityTag");
            Assert.IsNull(response.Content.Headers.ContentRange, "Content-Range != null");
            Assert.AreEqual("attachment", response.Content.Headers.ContentDisposition.DispositionType, "DispositionType != attachment");
        }

        #endregion
    }
}