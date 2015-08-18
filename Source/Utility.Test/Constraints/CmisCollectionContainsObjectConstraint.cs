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
using System.Collections.Generic;

namespace CmisUtility.Test.Constraints
{
    public class CmisCollectionContainsObjectConstraint : CmisBaseConstraint
    {
        private readonly ICmisObject _object;

        public CmisCollectionContainsObjectConstraint(ICmisObject obj)
        {
            _object = obj;
        }

        public override bool Matches(object actual)
        {
            var enumerable = actual as IEnumerable<object>;
            if (enumerable == null)
            {
                return Problem("An IEnumerable<object>", "Something else");
            }
            foreach (var obj in enumerable)
            {
                if (MatchObject(_object, obj as ICmisObject))
                {
                    return true;
                }
            }
            return Problem("An IEnumerable with " + _object.ToString(), "An IEnumerable without it");
        }
    }
}

