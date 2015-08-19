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
using DotCMIS.Client;

namespace CmisProvider.Test
{
    [TestFixture]
    public class GetChildItemsTests : ProviderTestBaseWithAutoDrive
    {
        [Test]
        public void GetChildItemDoesntThrow()
        {
            Shell.Execute("Get-ChildItem .");
        }

        [Test]
        public void GetChildItemContainsDocument()
        {
            CmisHelper.CreateTempDocument("/__gciDoc");
            var res = Shell.Execute("Get-ChildItem . | ? { $_.Name -eq '__gciDoc' }");
            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res[0], Is.Not.Null);
            Assert.That(res[0], Is.InstanceOf<IDocument>());
        }
    }
}

