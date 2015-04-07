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
using NUnit.Framework;
using System.Runtime.ConstrainedExecution;
using Cmis.Utility;

namespace CmisCmdlets.Test
{
    [TestFixture]
    public class UtilityTests
    {

        [TestCase("https://user:pw@my.host.is.great/foo/bar", "user", "pw", "https://my.host.is.great/foo/bar")]
        [TestCase("https://user1@my.host.is.great/foo/bar", "user1", "", "https://my.host.is.great/foo/bar")]
        [TestCase("https://my.host.is.great/foo/bar", "", "", "https://my.host.is.great/foo/bar")]
        public void parsingURLWorks(string input, string user, string pw, string url)
        {
            string resUrl, resUser, resPw;
            ConnectionFactory.ParseURL(input, out resUser, out resPw, out resUrl);
            Assert.AreEqual(user, resUser);
            Assert.AreEqual(pw, resPw);
            Assert.AreEqual(url, resUrl);
        }

    }
}

