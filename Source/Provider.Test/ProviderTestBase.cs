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
using CmisProvider;
using System.IO;

namespace CmisProvider.Test
{
    public class ProviderTestBase : CmisTestBase
    {
        protected const string TestDrive = "CmisTest";
        protected readonly string SystemSlash = Path.DirectorySeparatorChar.ToString();

        public ProviderTestBase() : base(typeof(CmisProvider))
        {
        }

        protected string GetNewDriveCommand(bool passThru = false)
        {
            var ptPrefix = passThru ? "" : "[void] ";
            var credential = CreateCredentialCommand(TestUser, TestPassword);
            return "(" + ptPrefix + "(New-PSDrive -PSProvider 'Cmis' -Name '" + TestDrive + "'" +
                   " -Credential " + credential + " -CmisURL '" + TestURL + "'" +
                   " -Repository '" + TestRepository + "' -Root '/'))";
        }

        protected string CreateCredentialCommand(string username, string password)
        {
            return "(New-Object System.Management.Automation.PSCredential ('" + username + "', (" +
                    "ConvertTo-SecureString '" + password + "' -AsPlainText -Force )))";
        }
    }
}

