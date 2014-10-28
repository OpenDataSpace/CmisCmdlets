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
using System.Collections.Generic;

namespace CmisCmdlets
{
    [Cmdlet(VerbsCommon.Remove, "CmisObject")]
    public class RemoveCmisObjectCommand : CmisCommandBase
    {
        [Parameter(Position = 0, ValueFromPipeline = true, ParameterSetName = "ByPath")]
        public string[] Path { get; set; }

        [Parameter(Position = 0, ValueFromPipeline = true, ParameterSetName = "ByObject")]
        public ICmisObject[] Object { get; set; }
    
        [Parameter]
        public SwitchParameter Recursive { get; set; }

        protected override void ProcessRecord()
        {
            var navigation = new CmisNavigation(CmisSession, WorkingFolder);
            if (Path != null)
            {
                foreach (var path in Path)
                {
                    WriteFailErrors(navigation.Delete(path, Recursive.IsPresent));
                }
            }
            else
            {
                foreach (var obj in Object)
                {
                    WriteFailErrors(navigation.Delete(obj, Recursive.IsPresent));
                }
            }
        }

        private void WriteFailErrors(IList<string> fails)
        {
            if (fails == null)
            {
                return;
            }
            foreach (var fail in fails)
            {
                var ex = new Exception(String.Format("Failed to delete CMIS object '{0}'",
                                                     fail));
                WriteError(new ErrorRecord(ex, "DeleteFailed", ErrorCategory.NotSpecified,
                                           fail));
            }
        }
    }
}

