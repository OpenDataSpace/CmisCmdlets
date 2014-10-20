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

namespace CmisCmdlets
{
    [Cmdlet(VerbsCommon.New, "CmisObject")]
    public class NewCmisObject : CmisCommandBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true, Position = 0)]
        public string[] Path { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 1)]
        public CmisObjectType Type { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true)]
        public SwitchParameter Recursive { get; set; }

        protected override void ProcessRecord()
        {
            var navigation = new CmisNavigation(GetCmisSession(), GetCmisDirectory());
            foreach (string p in Path)
            {
                if (Type.Equals(CmisObjectType.Folder))
                {
                    WriteObject(navigation.CreateFolder(p, Recursive.IsPresent));
                }
                else
                {
                    //WriteObject(navigation.CreateDocument(p));
                }
            }
        }

        protected void CreateFolder(string path)
        {

        }
    }
}

