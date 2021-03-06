﻿using Dev2.Activities.DropBox2016.DropboxFileActivity;
using Dev2.Activities.DropBox2016.Result;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Dev2.Tests.Activities.ActivityTests.DropBox2016.DropboxFiles
{
    [TestClass]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class DropboxFileReadTests
    {
        private Mock<IDropboxFileRead> CreateDropboxReadMock()
        {
            var mock = new Mock<IDropboxFileRead>();
            var successResult = new DropboxListFolderSuccesResult(It.IsAny<ListFolderResult>());
            mock.Setup(upload => upload.ExecuteTask(It.IsAny<DropboxClient>()))
                 .Returns(successResult);
            return mock;
        }

        [TestMethod]
        [Owner("Nkosinathi Sangweni")]
        public void CreateDropBoxActivity_GivenIsNew_ShouldNotBeNull()
        {
            //---------------Set up test pack-------------------
            var dropboxFileRead = CreateDropboxReadMock().Object;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            Assert.IsNotNull(dropboxFileRead);
        }

        [TestMethod]
        [Owner("Nkosinathi Sangweni")]
        public void ExecuteTask_GivendropboxFileRead_ShouldReturnFileMetadata()
        {
            //---------------Set up test pack-------------------
            var downloadMock = CreateDropboxReadMock();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(downloadMock);
            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            downloadMock.Object.ExecuteTask(It.IsAny<DropboxClient>());
            downloadMock.Verify(upload => upload.ExecuteTask(It.IsAny<DropboxClient>()));
        }

        [TestMethod]
        [Owner("Nkosinathi Sangweni")]
        public void CreateNewDropboxUpload_GivenEmptyPath_ShouldBeValid()
        {
            //---------------Set up test pack-------------------
            var dropboxFileRead = new DropboxFileRead(true, "", false, false);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(dropboxFileRead);
            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
        }

        [TestMethod]
        [Owner("Nkosinathi Sangweni")]
        public void CreateNewdropboxFileRead_GivenPath_ShouldBeValid()
        {
            //---------------Set up test pack-------------------
            var dropboxFileRead = new DropboxFileRead(true, "a.file", false, false);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            Assert.IsNotNull(dropboxFileRead);
        }
    }
}