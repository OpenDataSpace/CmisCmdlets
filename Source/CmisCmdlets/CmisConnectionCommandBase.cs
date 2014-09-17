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
using System.Management.Automation;
using System.Collections;

namespace CmisCmdlets
{
    public class CmisConnectionCommandBase : CmisCommandBase
    {
        [Parameter(Mandatory=true,
                   ParameterSetName = "AtomPub",
                   ValueFromPipelineByPropertyName = true,
                   Position = 0,
                   HelpMessage = "Url to the AtomPub interface of the Cmis repository")]
        public string Url { get; set; }

        [Parameter(Mandatory=true,
                   ParameterSetName = "AtomPub",
                   ValueFromPipelineByPropertyName = true,
                   Position = 1,
                   HelpMessage = "Username to the AtomPub interface of the Cmis repository")]
        public string UserName { get; set; }

        [Parameter(Mandatory=true,
                   ParameterSetName = "AtomPub",
                   ValueFromPipelineByPropertyName = true,
                   Position = 2,
                   HelpMessage = "Password to the AtomPub interface of the Cmis repository")]
        public string Password { get; set; }

        [Parameter(Mandatory=true,
                   ParameterSetName = "CustomParameters",
                   ValueFromPipeline = true,
                   Position = 0,
                   HelpMessage = "OpenCMIS parameters for the Cmis repository")]
        public Hashtable Parameters { get; set; }

    }
}

