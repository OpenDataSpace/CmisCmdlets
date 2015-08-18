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
    public class CmisObjectEqualityConstraint : CmisBaseConstraint
    {
        private readonly ICmisObject _object;

        public CmisObjectEqualityConstraint(ICmisObject obj)
        {
            _object = obj;
        }

        public override bool Matches(object actual)
        {
            return MatchObject(_object, actual as ICmisObject);
        }
    }
}

