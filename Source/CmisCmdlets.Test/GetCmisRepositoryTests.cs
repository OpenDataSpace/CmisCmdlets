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
using CmisCmdlets;

namespace CmisCmdlets.Test
{
    [TestFixture]
    public class GetCmisRepositoryTests : TestBase
    {
        public void GetRepositoriesAtomPubWorks()
        {
            var cmd = String.Format("{0} -url {1} -user {1} -password {2}",
                                    CmdletName(typeof(GetCmisRepositoryCommand)),
                                    TestURL, TestUser, TestPassword);
            var results = Shell.Execute(cmd);
            Assert.NotNull(results);
            Assert.Greater(results.Count, 0);
        }
    }
}

