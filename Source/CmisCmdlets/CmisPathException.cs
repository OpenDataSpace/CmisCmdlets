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
using DotCMIS.Exceptions;

namespace CmisCmdlets
{
    public class CmisPathException : CmisBaseException
    {
        public CmisPathException()
            : base()
        {
        }

        public CmisPathException(string message)
            : base(message)
        {
        }

        public CmisPathException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public CmisPathException(System.Runtime.Serialization.SerializationInfo info,
                                 System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}

