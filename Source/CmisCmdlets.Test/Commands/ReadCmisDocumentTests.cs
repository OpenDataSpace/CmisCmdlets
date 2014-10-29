// CmisCmdlets - Cmdlets to use CMIS from Powershell and Pash
// Copyright (C) GRAU DATA 2013-2014
//
// Author(s): Stefan Burnicki <stefan.burnicki@graudata.com>
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL was
// not distributed with this file, You can obtain one at
//  http://mozilla.org/MPL/2.0/.
using System;
using System.Linq;
using NUnit.Framework;
using DotCMIS.Exceptions;
using System.Text;

namespace CmisCmdlets.Test.Commands
{
    [TestFixture]
    public class ReadCmisDocumentTests : TestBaseWithAutoConnect
    {
        public static readonly string ReadCmisDocumentCmd = "Read-CmisDocument ";

        [Test]
        public void ReadingNonExistingDocumentThrows()
        {
            Assert.Throws<CmisObjectNotFoundException>(delegate {
                Shell.Execute(ReadCmisDocumentCmd + " /doesntExist.tmp");
            });
        }

        [Test]
        public void ReadingFolderDocumentThrows()
        {
            CmisHelper.CreateTempFolder("/__testFolder");
            Assert.Throws<CmisObjectNotFoundException>(delegate {
                Shell.Execute(ReadCmisDocumentCmd + " /__testFolder");
            });
        }

        public void ReadingByPath()
        {
            CmisHelper.CreateTempDocument("/__readByPath", "Hello World!", "text/plain");
            var res = Shell.Execute(ReadCmisDocumentCmd + " /__testFolder");
            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res.First(), Is.EqualTo("Hello World!"));
        }

        [TestCase("$doc | ", "")]
        [TestCase("", " $doc")]
        public void ReadingByObject(string preCmd, string postCmd)
        {
            CmisHelper.RegisterTempObject("__readByObj.tmp");
            var res = Shell.Execute(
                "$doc = " + CmdletName(typeof(NewCmisDocumentCommand)) + " /__readByObj.tmp"
                          + " -Content 'Hello World!' -MimeType 'text/html",
                preCmd + ReadCmisDocumentCmd + postCmd);
            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res.First(), Is.EqualTo("Hello World!"));
        }
        
        [Test]
        public void ReadingByPathToFile()
        {
            CmisHelper.CreateTempDocument("/__readByPath", "Hello World!", "text/plain");
            FileSystemHelper.RegisterTempFile("fromPathToFile.html");

            var res = Shell.Execute(ReadCmisDocumentCmd + " /__testFolder fromPathToFile.html");
            Assert.That(res, Is.Empty);
            Assert.That("Hello World!", FileSystemHelper.IsContentOf("fromPathToFile.html"));
        }

        [Test, Ignore("Try on windows to check if Pash works as PS with param binding. It should fail")]
        public void ____ReadingByObjectToFile2()
        {
            CmisHelper.RegisterTempObject("__readByObj.tmp");
            FileSystemHelper.RegisterTempFile("fromObjToFile.html");

            var res = Shell.Execute(
                "$doc = " + CmdletName(typeof(NewCmisDocumentCommand)) + " /__readByObj.tmp"
                + " -Content 'Hello World!' -MimeType 'text/html",
                "$doc | " + ReadCmisDocumentCmd + " fromObjToFile.html");
            Assert.That(res, Is.Empty);
            Assert.That("Hello World!", FileSystemHelper.IsContentOf("fromObjToFile.html"));
        }

        [Test]
        public void ReadingByObjectToFile()
        {
            CmisHelper.RegisterTempObject("__readByObj.tmp");
            FileSystemHelper.RegisterTempFile("fromObjToFile.html");

            var res = Shell.Execute(
                "$doc = " + CmdletName(typeof(NewCmisDocumentCommand)) + " /__readByObj.tmp"
                + " -Content 'Hello World!' -MimeType 'text/html",
                "$doc | " + ReadCmisDocumentCmd + " -Dest fromObjToFile.html");
            Assert.That(res, Is.Empty);
            Assert.That("Hello World!", FileSystemHelper.IsContentOf("fromObjToFile.html"));
        }

        [Test]
        public void ReadingToExistingFileThrows()
        {
            FileSystemHelper.CreateTempFile("__existingFile", "foo");
            CmisHelper.CreateTempDocument("/__readToExistingFile", "bar!", "text/plain");

            Assert.Throws<Exception>(delegate {
                Shell.Execute(ReadCmisDocumentCmd + " /__readToExistingFile __existingFile");
            });
        }

        public void ReadingToExistingFileWithForce()
        {
            FileSystemHelper.CreateTempFile("__toBeOverwritten.txt", "foo");
            CmisHelper.CreateTempDocument("/__overwriteExistingFile", "bar!", "text/plain");

            Shell.Execute(ReadCmisDocumentCmd +
                          " /__overwriteExistingFile __toBeOverwritten.txt -Force");
            Assert.That("bar!", FileSystemHelper.IsContentOf("__toBeOverwritten.txt"));
        }

        [Test]
        public void ReadingNonPlainTextToFile()
        {
            CmisHelper.CreateTempDocument("/__binaryMimeType.bin", "Binary Stuff!",
                                          "application/octet-stream");
            FileSystemHelper.RegisterTempFile("__destFile.bin");

            Shell.Execute(ReadCmisDocumentCmd + " /__binaryMimeType.bin __destFile.bin");
            Assert.That("Binary Stuff!", FileSystemHelper.IsContentOf("__destFile.bin"));
        }

        [Test]
        public void ReadingNonPlainTextToPipelineThrows()
        {
            CmisHelper.CreateTempDocument("/__binaryMimeType.bin", "binaryStuff!",
                                          "application/octet-stream");

            Assert.Throws<Exception>(delegate {
                Shell.Execute(ReadCmisDocumentCmd + " /__binaryMimeType.bin");
            });
        }

        [Test]
        public void ReadingNonPlainTextToPipelineWithForce()
        {
            CmisHelper.CreateTempDocument("/__binaryMimeType2.bin", "More Binary Stuff!",
                                          "application/octet-stream");

            var res = Shell.Execute(ReadCmisDocumentCmd + " /__binaryMimeType2.bin -Force");
            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res.First(), Is.EqualTo("More Binary Stuff!"));
        }

        [Test]
        public void ReadingBigFileToFile()
        {
            var content = GetStringOver100Kb();
            CmisHelper.CreateTempDocument("/__bigfile.txt", content, "text/plain");
            FileSystemHelper.RegisterTempFile("__bigout.txt");

            Shell.Execute(ReadCmisDocumentCmd + " /__bigfile.txt __bigout.txt");
            Assert.That(content, FileSystemHelper.IsContentOf("__bigout.txt"));
        }

        [Test]
        public void ReadingBigFileToPipelineThrows()
        {
            var content = GetStringOver100Kb();
            CmisHelper.CreateTempDocument("/__bigfileToPipe.txt", content, "text/plain");

            Assert.Throws<Exception>(delegate {
                Shell.Execute(ReadCmisDocumentCmd + " /__bigfileToPipe.txt");
            });
        }

        [Test]
        public void ReadingBigFileToPipelineWithForce()
        {
            var content = GetStringOver100Kb();
            CmisHelper.CreateTempDocument("/__bigfileWithForce.txt", content, "text/plain");

            var res = Shell.Execute(ReadCmisDocumentCmd + " /__bigfileWithForce.txt -Force");
            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res.First(), Is.EqualTo(content));
        }

        private string GetStringOver100Kb()
        {
            var size = 102500;
            var bytes = new byte[size];
            for (int i = 0; i < size; i++)
            {
                bytes[i] = (byte)'f';
            }
            return Encoding.UTF8.GetString(bytes);
        }

    }
}

