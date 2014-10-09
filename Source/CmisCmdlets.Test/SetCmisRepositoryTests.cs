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

namespace CmisCmdlets.Test
{
    [TestFixture]
    public class SetCmisRepositoryTests : TestBase
    {
        [Test]
        public void SetCmisRepositoryWithNameWorks()
        {
            var cmd = String.Format("{0}; {1} {2}; ${3}", GetConnectToTestRepoCmd(),
                                    CmdletName(typeof(SetCmisRepositoryCommand)), TestRepositoryAlt,
                                    CmisCommandBase.SESSION_VAR_NAME);
            var res = Shell.Execute(cmd);
            ValidateSession(res, TestRepositoryAlt);
        }

        [Test]
        public void SetCmisRepositoryWithIdWorks()
        {
            var parameters = ConnectionFactory.CreateAtomPubParams(TestURL, TestUser, TestPassword);
            var repo = ConnectionFactory.GetRepositoryByName(parameters, TestRepositoryAlt);
            var cmd = String.Format("{0}; {1} -Id {2}; ${3}", GetConnectToTestRepoCmd(),
                                    CmdletName(typeof(SetCmisRepositoryCommand)), repo.Id,
                                    CmisCommandBase.SESSION_VAR_NAME);
            var res = Shell.Execute(cmd);
            ValidateSession(res, TestRepositoryAlt);
        }
    }
}

