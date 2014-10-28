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
using DotCMIS.Exceptions;
using System.Collections;

namespace CmisCmdlets
{
    [Cmdlet(VerbsCommon.New, "CmisFolder")]
    public class NewCmisFolder : CmisCommandBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true, Position = 0)]
        public string[] Path { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true)]
        public SwitchParameter Recursive { get; set; }

        [Parameter(Position = 1, Mandatory = false)]
        public Hashtable Properties { get; set; }

        protected override void ProcessRecord()
        {
            var navigation = new CmisNavigation(GetCmisSession(), GetWorkingFolder());
            foreach (string p in Path)
            {
                try
                {
                    var props = Utilities.HashtableToDict(Properties);
                    WriteObject(navigation.CreateFolder(p, Recursive.IsPresent, props));
                }
                catch (CmisBaseException e)
                {
                    ThrowTerminatingError(new ErrorRecord(e, "FolderCreationFailed",
                                                          ErrorCategory.WriteError, p));
                }
            }
        }
    }
}

