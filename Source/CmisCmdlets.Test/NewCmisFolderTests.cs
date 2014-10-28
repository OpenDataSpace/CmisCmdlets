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

namespace CmisCmdlets.Test
{
    [TestFixture]
    public class NewCmisFolderTests : TestBase
    {
        public string NewCmisFolderCmd = CmdletName(typeof(NewCmisFolderCommand)) + " ";

        [Test]
        public void CreateSimpleFolder()
        {
            var res = Shell.Execute(
                NewCmisFolderCmd + "__newFolder"
            );
            var folder = res.First() as IFolder;
            Assert.That(folder, Is.Not.Null);
            Assert.That(folder.Path, Is.EqualTo("/__newFolder"));
        }

    }
}

