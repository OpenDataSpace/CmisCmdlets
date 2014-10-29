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

namespace CmisCmdlets.Test.Commands
{
    [TestFixture]
    public class GetCmisObjectTests : TestBaseWithAutoConnect
    {
        public static readonly string GetCmisObjectCmd = "Get-CmisObject ";

        [TestCase("/notexisting/path/")]
        [TestCase("/notexisting_file")]
        public void GetObjectOfNonExistingPathThrows(string path)
        {
            Assert.Throws<CmisObjectNotFoundException>(delegate {
                Shell.Execute(GetCmisObjectCmd + path);
            });
        }

        [Test]
        public void GetObjectFromFolderWithoutTrailingSlashReturnsDir()
        {
            var folder = CmisHelper.CreateTempFolder("/__tempFolder");
            CmisHelper.CreateTempDocument("/__tempFolder/foo");
            CmisHelper.CreateTempDocument("/__tempFolder/bar");

            var res = Shell.Execute(GetCmisObjectCmd + "/__tempFolder");
            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res.First(), CmisHelper.IsEqualObject(folder));
        }

        [TestCase("/__tempFolder/")] // by using a trailing slash in path
        [TestCase("/__tempFolder -RecursionDepth 1")] // by defining recursion depth
        public void GetObjectFromFolderCanReturnChildren(string parameters)
        {
            var folder = CmisHelper.CreateTempFolder("/__tempFolder/folder", true);
            var doc1 = CmisHelper.CreateTempDocument("/__tempFolder/foo");
            var doc2 = CmisHelper.CreateTempDocument("/__tempFolder/bar");
             CmisHelper.CreateTempDocument("/__tempFolder/folder/baz");

            var res = Shell.Execute(GetCmisObjectCmd + parameters);
            Assert.That(res.Count, Is.EqualTo(3));
            Assert.That(res, CmisHelper.ContainsObject(folder));
            Assert.That(res, CmisHelper.ContainsObject(doc1));
            Assert.That(res, CmisHelper.ContainsObject(doc2));
        }

        [Test]
        public void GetObjectFromFolderWithRecursionLevelReturnsDescendants()
        {
            var folder = CmisHelper.CreateTempFolder("/__tempFolder/folder", true);
            var doc1 = CmisHelper.CreateTempDocument("/__tempFolder/foo");
            var doc2 = CmisHelper.CreateTempDocument("/__tempFolder/bar");
            var grandchild = CmisHelper.CreateTempDocument("/__tempFolder/folder/baz");
            var grandchildf = CmisHelper.CreateTempFolder("/__tempFolder/folder/other");
            CmisHelper.CreateTempFolder("/__tempFolder/folder/other/file");

            var res = Shell.Execute(GetCmisObjectCmd + "/__tempFolder -RecursionDepth 2");
            Assert.That(res.Count, Is.EqualTo(5));
            Assert.That(res, CmisHelper.ContainsObject(folder));
            Assert.That(res, CmisHelper.ContainsObject(doc1));
            Assert.That(res, CmisHelper.ContainsObject(doc2));
            Assert.That(res, CmisHelper.ContainsObject(grandchild));
            Assert.That(res, CmisHelper.ContainsObject(grandchildf));
        }

        [Test]
        public void GetObjectWithNameFilterCanBeEmpty()
        {
            CmisHelper.CreateTempFolder("/__tempFolder/folder", true);
            CmisHelper.CreateTempDocument("/foo");
            CmisHelper.CreateTempDocument("/__tempFolder/bar");

            var res = Shell.Execute(GetCmisObjectCmd + "/__tempFolder -Name foo");
            Assert.That(res, Is.Empty);
        }

        [Test]
        public void GetObjectWithNameFiltersRecursion()
        {
            CmisHelper.CreateTempFolder("/__tempFolder/folder", true);
            CmisHelper.CreateTempDocument("/bar"); // won't be found, too high in hierarch
            var baDoc = CmisHelper.CreateTempDocument("/__tempFolder/ba"); // should be found
            var bazDoc = CmisHelper.CreateTempDocument("/__tempFolder/folder/baz"); // in 2nd level
            var bariumFolder = CmisHelper.CreateTempFolder("/__tempFolder/folder/barium");
            CmisHelper.CreateTempDocument("/__tempFolder/folder/barium/baz"); // 3rd level ignored

            var res = Shell.Execute(GetCmisObjectCmd + "/__tempFolder -Name ba -RecursionDepth 2");
            Assert.That(res.Count, Is.EqualTo(3));
            Assert.That(res, CmisHelper.ContainsObject(baDoc));
            Assert.That(res, CmisHelper.ContainsObject(bazDoc));
            Assert.That(res, CmisHelper.ContainsObject(bariumFolder));
        }

        [Test]
        public void GetObjectWithExactNames()
        {
            CmisHelper.CreateTempFolder("/__tempFolder/barium", true);
            CmisHelper.CreateTempDocument("/ba"); // won't be found, too high in hierarch
            var baDoc = CmisHelper.CreateTempDocument("/__tempFolder/ba"); // should be found
            CmisHelper.CreateTempDocument("/__tempFolder/bar"); // should be found
            CmisHelper.CreateTempDocument("/__tempFolder/barium/baz"); // in 2nd level
            var baFolder = CmisHelper.CreateTempFolder("/__tempFolder/barium/ba");

            var res = Shell.Execute(GetCmisObjectCmd + "/__tempFolder -Name ba -Exact -RecursionDepth 2");
            Assert.That(res.Count, Is.EqualTo(2));
            Assert.That(res, CmisHelper.ContainsObject(baDoc));
            Assert.That(res, CmisHelper.ContainsObject(baFolder));
        }
    }
}

