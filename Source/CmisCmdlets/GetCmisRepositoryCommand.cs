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
using System.Management.Automation;

namespace CmisCmdlets
{
    [Cmdlet(VerbsCommon.Get, "CmisRepository")]
    public class GetCmisRepositoryCommand : CmisCommandBase
    {
        [Parameter(Mandatory = false, HelpMessage = "Name of the Cmis repository")]
        public string[] Name { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Whether or not you look for an exact name")]
        public SwitchParameter Exact { get; set; }

        protected override void ProcessRecord()
        {
            var repos = ConnectionFactory.GetRepositories(ConnectionParameters);
            if (Name == null || Name.Length == 0)
            {
                Name = new [] { "" };
            }
            foreach (var curName in Name)
            {
                var pattern = "*";
                if (!String.IsNullOrEmpty(curName))
                {
                    pattern = Exact.IsPresent ? curName : curName + "*";
                }
                var options = Exact.IsPresent ? WildcardOptions.None : WildcardOptions.IgnoreCase;
                WildcardPattern wildcard = new WildcardPattern(pattern, options);

                WriteObject(from rep in repos where wildcard.IsMatch(rep.Name) select rep, true);
            }
        }
    }
}

