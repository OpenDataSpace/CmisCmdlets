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
using DotCMIS.Client;
using DotCMIS;
using System.Reflection;
using System.Collections.Generic;

namespace CmisCmdlets
{
    [Cmdlet(VerbsCommon.Get, "CmisProperty")]
    public class GetCmisPropertyCommand : CmisCommandBase
    {
        [Parameter(Mandatory = false, Position = 1, ValueFromPipeline = true)]
        public ICmisObject Object { get; set; }

        [Parameter(Mandatory = false, Position = 0)]
        public string Property { get; set; }


        private static Dictionary<string, string> _existingProperties;

        static GetCmisPropertyCommand()
        {
            var flags = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public;
            var fields = from prop in typeof(PropertyIds).GetFields(flags) select prop;
            _existingProperties = new Dictionary<string, string>(
                StringComparer.InvariantCultureIgnoreCase);
            foreach (var curField in fields)
            {
                var value = curField.GetValue(null);
                if (value is string)
                {
                    _existingProperties.Add(curField.Name, (string) value);
                }
            }
        }

        protected override void ProcessRecord()
        {
            if (String.IsNullOrEmpty(Property))
            {
                if (Object == null)
                {
                    WriteObject(_existingProperties);
                }
                else
                {
                    WriteObject(Object.Properties);
                }
                return;
            }

            var wildcard = new WildcardPattern(Property + "*", WildcardOptions.IgnoreCase);
            if (Object == null)
            {
                WriteObject(from pair in _existingProperties where wildcard.IsMatch(pair.Key)
                            select pair.Value, true);
                return;
            }
            WriteObject(from prop in Object.Properties where wildcard.IsMatch(prop.LocalName)
                        select prop, true);
        }
    }
}

