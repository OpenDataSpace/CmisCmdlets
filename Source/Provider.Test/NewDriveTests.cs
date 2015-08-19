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
using CmisProvider;

namespace CmisProvider.Test
{
    [TestFixture]
    public class NewDriveTests : ProviderTestBase
    {
        [Test]
        public void CreateNewDrive()
        {
            var cmd = GetNewDriveCommand(true);
            var res = Shell.Execute(cmd);
            Assert.That(res.Count, Is.EqualTo(1));
            var drive = res.First() as CmisDrive;
            Assert.That(drive, Is.Not.Null);
            Assert.That(drive.Connection, Is.Not.Null);
            Assert.That(drive.Connection.RepositoryInfo.Name, Is.EqualTo(TestRepository));
        }

        [Test]
        public void ChangeIntoNewDrive()
        {
            var res = Shell.Execute(
                GetNewDriveCommand(),
                "Set-Location CmisTest:",
                "Get-Location"
            );
            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res[0].ToString(), Is.EqualTo("CmisTest:\\"));
        }
    }
}

