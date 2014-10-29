// CmisCmdlets - Cmdlets to use CMIS from Powershell and Pash
// Copyright (C) GRAU DATA 2013-2014
//
// Author(s): Stefan Burnicki <stefan.burnicki@graudata.com>
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL was
// not distributed with this file, You can obtain one at
//  http://mozilla.org/MPL/2.0/.
using NUnit.Framework;
using System;
using System.Linq;
using System.Management.Automation;

namespace CmisCmdlets.Test
{
    [TestFixture]
    public class CmisWorkingFolderTests : TestBase
    {
        public static readonly string GetCmisWorkingFolderCmd = "Get-CmisWorkingFolder";
        public static readonly string SetCmisWorkingFolderCmd = "Set-CmisWorkingFolder";

        [Test]
        public void GetWFThrowsIfNotConnected()
        {
            Assert.Throws<RuntimeException>(delegate {
                Shell.Execute(GetCmisWorkingFolderCmd);
            });
        }

        [Test]
        public void GetWFIsRootWhenConnected()
        {
            var res = Shell.Execute(
                GetConnectToTestRepoCmd(),
                GetCmisWorkingFolderCmd
                );
            Assert.That(res.First(), Is.EqualTo("/"));
        }

        [Test]
        public void SetWFThrowsWhenNotExisting()
        {
            Assert.Throws<RuntimeException>(delegate {
                Shell.Execute(
                    GetConnectToTestRepoCmd(),
                    SetCmisWorkingFolderCmd + "__nonExisting"
                );
            });
        }

        [Test]
        public void SetWFThrowsWhenNotConnected()
        {
            Assert.Throws<RuntimeException>(delegate {
                Shell.Execute(
                    SetCmisWorkingFolderCmd + "/"
                );
            });
        }

        [Test]
        public void SetWFWorks()
        {
            CmisHelper.CreateTempFolder("/__subdir1/foo", true);
            CmisHelper.CreateTempFolder("/__subdir2", false);
            var res = Shell.Execute(
                GetConnectToTestRepoCmd(),
                SetCmisWorkingFolderCmd + "/__subdir1",
                GetCmisWorkingFolderCmd,
                SetCmisWorkingFolderCmd + "foo",
                GetCmisWorkingFolderCmd,
                SetCmisWorkingFolderCmd + "../../__subdir2",
                GetCmisWorkingFolderCmd,
                SetCmisWorkingFolderCmd + "/",
                GetCmisWorkingFolderCmd
            );
            Assert.That(res, Is.EquivalentTo(new [] {
                "/__subdir1", "/__subdir1",
                "/__subdir1/foo", "/__subdir1/foo",
                "/__subdir2", "/__subdir2",
                "/", "/"
            }));
        }
    }
}

