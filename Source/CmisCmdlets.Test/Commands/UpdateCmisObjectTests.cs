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
using NUnit.Framework;
using CmisCmdlets.Test;
using DotCMIS.Exceptions;
using DotCMIS.Client;
using System.Runtime.ConstrainedExecution;
using CmisCmdlets;
using System.IO;
using System.Management.Automation;

namespace CmisCmdlets.Test.Commands
{
    [TestFixture]
    public class UpdateCmisObjectTests : TestBaseWithAutoConnect
    {
        public static readonly string UpdateCmisObjectCmd = "Update-CmisObject ";

        [Test]
        public void UpdateNonExistingObjectThrows()
        {
            Assert.Throws<CmdletInvocationException>(delegate {
                Shell.Execute(UpdateCmisObjectCmd + " __not/existing -Name 'bar'");
            });
        }

        [Test]
        public void RenameFolderByPath()
        {
            CmisHelper.CreateTempFolder("__oldFolder");
            CmisHelper.CreateTempDocument("__oldFolder/test");

            var res = Shell.Execute(UpdateCmisObjectCmd + "__oldFolder -Name __newFolder");
            Assert.That("/__oldFolder", CmisHelper.DoesNotExist);
            Assert.That("/__oldFolder/test", CmisHelper.DoesNotExist);
            Assert.That("/__newFolder", CmisHelper.Exists);
            Assert.That("/__newFolder/test", CmisHelper.Exists);
            Assert.That(res.Count, Is.EqualTo(1));
            var folder = res[0] as IFolder;
            Assert.That(folder, Is.Not.Null);
            Assert.That(folder.Name, Is.EqualTo("__newFolder"));

            CmisHelper.ForgetTempObjects();
            CmisHelper.RegisterTempObject("/__newFolder");
            CmisHelper.RegisterTempObject("/__newFolder/test");
        }

        [Test]
        public void RenameFolderByObject()
        {
            CmisHelper.CreateTempFolder("__oldFolder");
            CmisHelper.CreateTempDocument("__oldFolder/test");

            var res = Shell.Execute(
                "$f = " + CmdletName(typeof(GetCmisObjectCommand)) + " __oldFolder",
                UpdateCmisObjectCmd + "-Object $f -Name __newFolder"
            );
            Assert.That("/__oldFolder", CmisHelper.DoesNotExist);
            Assert.That("/__oldFolder/test", CmisHelper.DoesNotExist);
            Assert.That("/__newFolder", CmisHelper.Exists);
            Assert.That("/__newFolder/test", CmisHelper.Exists);
            Assert.That(res.Count, Is.EqualTo(1));
            var folder = res[0] as IFolder;
            Assert.That(folder, Is.Not.Null);
            Assert.That(folder.Name, Is.EqualTo("__newFolder"));

            CmisHelper.ForgetTempObjects();
            CmisHelper.RegisterTempObject("/__newFolder");
            CmisHelper.RegisterTempObject("/__newFolder/test");
        }

        [Test]
        public void RenameDocument()
        {
            CmisHelper.CreateTempDocument("__oldDoc");

            var res = Shell.Execute(UpdateCmisObjectCmd + "__oldDoc -Name __newDoc");
            Assert.That("/__oldDoc", CmisHelper.DoesNotExist);
            Assert.That("/__newDoc", CmisHelper.Exists);
            Assert.That(res.Count, Is.EqualTo(1));
            var doc = res[0] as IDocument;
            Assert.That(doc, Is.Not.Null);
            Assert.That(doc.Name, Is.EqualTo("__newDoc"));
            Assert.That(doc.Paths[0], Is.EqualTo("/__newDoc"));

            CmisHelper.ForgetTempObjects();
            CmisHelper.RegisterTempObject("__newDoc");
        }

        [Test]
        [Ignore("Doesn't work")]
        public void UpdateDocumentProperties()
        {
            CmisHelper.CreateTempDocument("__upProps");

            var res = Shell.Execute(UpdateCmisObjectCmd + " __upProps -Properties @{foo='bar'}");
            Assert.That(res.Count, Is.EqualTo(1));
            var doc = res[0] as IDocument;
            Assert.That(doc, Is.Not.Null);
            Assert.That(doc, CmisHelper.HasProperty("foo", "bar"));
        }

        [Test]
        [Ignore("Doesn't work")]
        public void UpdateFolderProperties()
        {
            CmisHelper.CreateTempFolder("__upPropsFolder");

            var res = Shell.Execute(UpdateCmisObjectCmd + " __upPropsFolder -Properties @{bar='baz'}");
            Assert.That(res.Count, Is.EqualTo(1));
            var doc = res[0] as IDocument;
            Assert.That(doc, Is.Not.Null);
            Assert.That(doc, CmisHelper.HasProperty("bar", "baz"));
        }

        [Test]
        public void UpdateFolderContentThrows()
        {
            CmisHelper.CreateTempFolder("__throwOnUpdate");

            Assert.Throws<CmdletInvocationException>(delegate {
                Shell.Execute(
                    UpdateCmisObjectCmd + "__throwOnUpdate -Content 'foo' -MimeType 'text/plain'"
                );
            });
        }

        [Test]
        public void UpdateDocumentContentByLocalFile()
        {
            FileSystemHelper.CreateTempFile("_newContent.html", "Ciao!");
            CmisHelper.CreateTempDocument("__upContent", "Hello World!", "text/plain");

            var res = Shell.Execute(UpdateCmisObjectCmd + " __upContent _newContent.html");
            Assert.That(res.Count, Is.EqualTo(1));
            var doc = res[0] as IDocument;
            Assert.That(doc, Is.Not.Null);
            var content = File.ReadAllBytes("_newContent.html");
            Assert.That(doc, CmisHelper.HasContent(content, "text/html"));
        }

        [Test]
        public void UpdateDocumentContentByPipeline()
        {
            CmisHelper.CreateTempDocument("__upContent", "Hello World!", "text/plain");

            var res = Shell.Execute(
                "'Bye','World' | " + UpdateCmisObjectCmd + " __upContent -MimeType 'text/html'"
                );
            Assert.That(res.Count, Is.EqualTo(1));
            var doc = res[0] as IDocument;
            Assert.That(doc, Is.Not.Null);
            Assert.That(doc, CmisHelper.HasContent(NewlineJoin("Bye", "World"), "text/html"));
        }

    }
}

