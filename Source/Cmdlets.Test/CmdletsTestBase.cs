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
using CmisUtility.Test;

namespace CmisCmdlets.Test
{
    public class CmdletsTestBase : CmisTestBase
    {
        public CmdletsTestBase() : base(typeof(ConnectCmisCommand))
        {
        }

        protected string GetConnectToTestRepoCmd(bool insecure = false)
        {
            return String.Format("{0} -url '{1}' -user '{2}' -password '{3}' -repo '{4}' {5};",
                CmdletName(typeof(ConnectCmisCommand)), TestURL, TestUser,
                TestPassword, TestRepository, insecure ? "-Insecure" : "");
        }
    }
}

