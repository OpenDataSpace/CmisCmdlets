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
using DotCMIS.Client;
using DotCMIS.Exceptions;

namespace CmisCmdlets
{
    [Cmdlet(VerbsCommon.Set, "CmisDirectory")]
    public class SetCmisDirectoryCommand : CmisCommandBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Path { get; set; }

        protected override void EndProcessing()
        {
            //make sure it exists
            var newPath = GetCmisDirectory().Combine(Path);
            try
            {
                new CmisNavigation(GetCmisSession()).GetFolder(Path);
            }
            catch (CmisBaseException ex)
            {
                ThrowTerminatingError(new ErrorRecord(ex, "InvalidCmisDirectory",
                                                      ErrorCategory.ObjectNotFound, newPath));
            }

            SetCmisDirectory(newPath.ToString());
            WriteObject(newPath.ToString());
        }
    }
}

