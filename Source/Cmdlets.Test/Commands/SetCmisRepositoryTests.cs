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

namespace CmisCmdlets.Test.Commands
{
    [TestFixture]
    public class SetCmisRepositoryTests : TestBaseWithAutoConnect
    {
        public static readonly string SetCmisRepositoryCmd = "Set-CmisRepository ";

        [Test]
        public void SetCmisRepositoryWithNameWorks()
        {
            Shell.Execute(SetCmisRepositoryCmd + TestRepositoryAlt);
            ValidateSession(Shell.GetVariableValue(CmisCommandBase.SESSION_VAR_NAME),
                            TestRepositoryAlt);
        }

        [Test]
        public void SetCmisRepositoryWithIdWorks()
        {
            var parameters = ConnectionFactory.CreateAtomPubParams(TestURL, TestUser, TestPassword);
            var repo = ConnectionFactory.GetRepositoryByName(parameters, TestRepositoryAlt);
            Shell.Execute(SetCmisRepositoryCmd + " -Id '" + repo.Id + "'");
            ValidateSession(Shell.GetVariableValue(CmisCommandBase.SESSION_VAR_NAME),
                            TestRepositoryAlt);
        }
    }
}

