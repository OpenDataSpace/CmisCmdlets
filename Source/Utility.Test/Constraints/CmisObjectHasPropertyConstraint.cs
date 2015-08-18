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
using DotCMIS.Client;

namespace CmisUtility.Test.Constraints
{
    public class CmisObjectHasPropertyConstraint : CmisBaseConstraint
    {
        private readonly string _propertyName;
        private readonly object _propertyValue;

        public CmisObjectHasPropertyConstraint(string propertyName, object propertyValue)
        {
            _propertyName = propertyName;
            _propertyValue = propertyValue;
        }

        public override bool Matches(object obj)
        {
            var cmisObj = obj as ICmisObject;
            if (cmisObj == null)
            {
                return Problem("A cmis object", "Something else");
            }
            foreach (var prop in cmisObj.Properties)
            {
                if (!prop.LocalName.Equals(_propertyName))
                {
                    continue;
                }
                if (!prop.Value.Equals(_propertyValue))
                {
                    return Problem("Object with property value {0}", _propertyValue, prop.Value);
                }
                return true;
            }
            return Problem("Object with the specified property", "Object without that property");
        }
    }
}

