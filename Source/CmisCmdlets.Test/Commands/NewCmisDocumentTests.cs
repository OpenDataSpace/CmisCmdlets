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
using DotCMIS.Client;
using DotCMIS.Exceptions;
using System.Text;

namespace CmisCmdlets.Test.Commands
{
    [TestFixture]
    public class NewCmisDocumentTests : TestBaseWithAutoConnect
    {
        public static readonly string NewCmisDocumentCmd = "New-CmisDocument ";

        [Test]
        public void CreatingEmptyDocument()
        {
            CmisHelper.RegisterTempObject("/__emptyDoc");
            var res = Shell.Execute(NewCmisDocumentCmd + " /__emptyDoc");
            var doc = res.First() as IDocument;
            Assert.That(doc, Is.Not.Null);
            Assert.That(doc.Paths, Contains.Item("/__emptyDoc"));
            Assert.That("/__emptyDoc", CmisHelper.Exists);
        }

        [TestCase("/notexisting/__emptyDoc")]
        [TestCase("/notexisting/")] // empty filename
        [TestCase("/notexisting/.")] // will be interpreted as special . folder
        [TestCase("/notexisting/bar/..")] // will be interpreted as special .. folder
        public void CreatingDocumentWithInvalidPathThrows(string path)
        {
            CmisHelper.RegisterTempObject(path);
            Assert.Throws<CmisConstraintException>(delegate {
                Shell.Execute(NewCmisDocumentCmd + path);
            });
            Assert.That(path, CmisHelper.DoesNotExist);
        }

        [Test]
        public void CreatingDocumentAtExistingPathThrows()
        {
            CmisHelper.CreateTempDocument("__existingFile", null);
            Assert.Throws<CmisConstraintException>(delegate {
                Shell.Execute(NewCmisDocumentCmd + "__existingFile");
            });
        }

        [Test]
        public void CreatingDocumentWithContent()
        {
            CmisHelper.RegisterTempObject("/__contentDoc");
            var cmd = NewCmisDocumentCmd + " /__contentDoc -Content 'baz' -MimeType 'text/html'";
            var res = Shell.Execute(cmd);
            var doc = res.First() as IDocument;
            Assert.That(doc, Is.Not.Null);
            Assert.That(doc.Paths, Contains.Item("/__contentDoc"));
            Assert.That("/__contentDoc", CmisHelper.Exists);
            Assert.That(doc, CmisHelper.HasContent("baz", "text/html"));
        }

        [Test]
        public void CreatingDocumentWithContentByPipeline()
        {
            CmisHelper.RegisterTempObject("/__contentPipeDoc");
            var cmd = "'foo','bar' | " +
                NewCmisDocumentCmd + " /__contentPipeDoc -MimeType 'text/plain'";
            var res = Shell.Execute(cmd);
            var doc = res.First() as IDocument;
            Assert.That(doc, Is.Not.Null);
            Assert.That(doc.Paths, Contains.Item("/__contentPipeDoc"));
            Assert.That("/__contentPipeDoc", CmisHelper.Exists);
            Assert.That(doc, CmisHelper.HasContent("foo" + Environment.NewLine + "bar", "text/html"));
        }

        [Test]
        public void CreatingDocumentWithLocalFile()
        {
            FileSystemHelper.CreateTempFile("__testContentFile.html", "abcde", Encoding.UTF8);
            CmisHelper.RegisterTempObject("/__testContentFile.html");
            var cmd = NewCmisDocumentCmd + " / -LocalFile __testContentFile.html";
            var res = Shell.Execute(cmd);
            var doc = res.First() as IDocument;
            Assert.That(doc, Is.Not.Null);
            Assert.That(doc.Paths, Contains.Item("/__testContentFile.html"));
            Assert.That("/__testContentFile.html", CmisHelper.Exists);
            Assert.That(doc, CmisHelper.HasContent(Encoding.UTF8.GetBytes("abcde"), "text/html"));
        }


        [Test]
        public void CreatingDocumentWithLocalFileAndOtherFilename()
        {
            FileSystemHelper.CreateTempFile("__testContentFile.html", "foobar", Encoding.UTF8);
            CmisHelper.RegisterTempObject("/__otherFilename");
            var cmd = NewCmisDocumentCmd + " /__otherFilename -LocalFile __testContentFile.html";
            var res = Shell.Execute(cmd);
            var doc = res.First() as IDocument;
            Assert.That(doc, Is.Not.Null);
            Assert.That(doc.Paths, Contains.Item("/__otherFilename"));
            Assert.That("/__otherFilename", CmisHelper.Exists);
            Assert.That(doc, CmisHelper.HasContent(Encoding.UTF8.GetBytes("foobar"), "text/html"));
        }

        // TODO: tests with properties
    }
}

