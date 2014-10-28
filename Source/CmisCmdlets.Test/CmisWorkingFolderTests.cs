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
    }
}

