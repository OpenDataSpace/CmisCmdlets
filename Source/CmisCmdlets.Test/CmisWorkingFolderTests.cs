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
        [Test]
        public void GetWFThrowsIfNotConnected()
        {
            Assert.Throws<RuntimeException>(delegate {
                Shell.Execute(CmdletName(typeof(GetCmisWorkingFolderCommand)));
            });
        }

        [Test]
        public void GetWFIsRootWhenConnected()
        {
            var res = Shell.Execute(
                GetConnectToTestRepoCmd(),
                CmdletName(typeof(GetCmisWorkingFolderCommand))
                );
            Assert.That(res.First(), Is.EqualTo("/"));
        }

        [Test]
        public void SetWFThrowsWhenNotExisting()
        {
            Assert.Throws<RuntimeException>(delegate {
                Shell.Execute(
                    CmdletName(typeof(GetCmisWorkingFolderCommand)),
                    CmdletName(typeof(SetCmisWorkingFolderCommand)) + " __nonExisting"
                );
            });
        }

        [Test]
        public void SetWFThrowsWhenNotConnected()
        {
            Assert.Throws<RuntimeException>(delegate {
                Shell.Execute(
                    CmdletName(typeof(SetCmisWorkingFolderCommand)) + " /"
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
                CmdletName(typeof(SetCmisWorkingFolderCommand)) + " /__subdir1",
                CmdletName(typeof(GetCmisWorkingFolderCommand)),
                CmdletName(typeof(SetCmisWorkingFolderCommand)) + " foo",
                CmdletName(typeof(GetCmisWorkingFolderCommand)),
                CmdletName(typeof(SetCmisWorkingFolderCommand)) + " ../../__subdir2",
                CmdletName(typeof(GetCmisWorkingFolderCommand)),
                CmdletName(typeof(SetCmisWorkingFolderCommand)) + " /",
                CmdletName(typeof(GetCmisWorkingFolderCommand))
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

