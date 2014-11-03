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
using System.Management.Automation;

namespace CmisCmdlets.Test.Commands
{
    [TestFixture]
    public class NewCmisFolderTests : TestBaseWithAutoConnect
    {
        public string NewCmisFolderCmd = "New-CmisFolder ";

        [Test]
        public void CreateSimpleFolder()
        {
            CmisHelper.RegisterTempObject("__newFolder");

            var res = Shell.Execute(NewCmisFolderCmd + "__newFolder");
            var folder = res.First() as IFolder;
            Assert.That(folder, Is.Not.Null);
            Assert.That(folder.Path, Is.EqualTo("/__newFolder"));
            Assert.That("/__newFolder", CmisHelper.Exists);
        }

        [Test]
        public void CreateExistingFolderWithoutRecursionFails()
        {
            CmisHelper.CreateTempFolder("/__existingFolder");

            Assert.Throws<CmdletInvocationException>(delegate {
                Shell.Execute(NewCmisFolderCmd + "/__existingFolder");
            });
            Assert.That("/__existingFolder", CmisHelper.Exists);
        }

        [Test]
        public void CreateExistingFolderWithRecursionReturns()
        {
            CmisHelper.CreateTempFolder("/__existingFolder");
            var res = Shell.Execute(NewCmisFolderCmd + "/__existingFolder -Recursive");
            var folder = res.First() as IFolder;
            Assert.That(folder, Is.Not.Null);
            Assert.That(folder.Path, Is.EqualTo("/__existingFolder"));
        }

        [Test]
        public void CreateSubFolder()
        {
            CmisHelper.CreateTempFolder("/__parentFolder");
            CmisHelper.RegisterTempObject("/__parentFolder/subfolder");

            var res = Shell.Execute(NewCmisFolderCmd + "/__parentFolder/subfolder");
            var folder = res.First() as IFolder;
            Assert.That(folder, Is.Not.Null);
            Assert.That(folder.Path, Is.EqualTo("/__parentFolder/subfolder"));
            Assert.That("/__parentFolder/subfolder", CmisHelper.Exists);
        }

        [Test]
        public void CreateRecursiveFoldersWithRecursion()
        {
            CmisHelper.CreateTempFolder("/__parentFolder");
            CmisHelper.RegisterTempObject("/__parentFolder/1", "/__parentFolder/1/2",
                                          "/__parentFolder/1/2/3");

            var res = Shell.Execute(NewCmisFolderCmd + "/__parentFolder/1/2/3 -Recursive");
            var folder = res.First() as IFolder;
            Assert.That(folder, Is.Not.Null);
            Assert.That(folder.Path, Is.EqualTo("/__parentFolder/1/2/3"));
            Assert.That("/__parentFolder/1", CmisHelper.Exists);
            Assert.That("/__parentFolder/1/2", CmisHelper.Exists);
            Assert.That("/__parentFolder/1/2/3", CmisHelper.Exists);
        }

        [Test]
        public void CreateRecursiveFoldersWithoutRecursionFails()
        {
            CmisHelper.RegisterTempObject("/_foo/", "/_foo/bar"); //makes sure we clean up
            Assert.Throws<CmdletInvocationException>(delegate {
                Shell.Execute(NewCmisFolderCmd + "/__foo/bar");
            });
            Assert.That("/__foo", CmisHelper.DoesNotExist);
            Assert.That("/__foo/bar", CmisHelper.DoesNotExist);
            CmisHelper.ForgetTempObjects();
        }

        // TODO: tests with properties
    }
}

