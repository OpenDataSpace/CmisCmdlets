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
using NUnit.Framework;
using DotCMIS;
using System.Reflection;
using System.Collections;
using DotCMIS.Client;

namespace CmisCmdlets.Test.Commands
{
    [TestFixture]
    public class GetCmisPropertyTests : CmisTestBase
    {
        public static readonly string GetCmisPropertyCmd = "Get-CmisProperty ";

        [Test]
        public void GetAllCommonCmisProperties()
        {
            var res = Shell.Execute(GetCmisPropertyCmd);
            var knownProperties = typeof(PropertyIds).GetFields(BindingFlags.Static |
                                                                BindingFlags.Public);
            Assert.That(res.Count, Is.EqualTo(1));
            var table = res[0] as IDictionary;
            Assert.That(table, Is.Not.Null);
            Assert.That(table.Count, Is.EqualTo(knownProperties.Length));
            foreach (var prop in knownProperties)
            {
                Assert.That(table[prop.Name], Is.EqualTo(prop.GetValue(null).ToString()));
            }
        }

        [Test]
        public void GetSomeCommonCmisPropertiesByName()
        {
            var res = Shell.Execute(GetCmisPropertyCmd + " *name");
            Assert.That(res.Count, Is.EqualTo(2));
            Assert.That(res, Contains.Item("cmis:contentStreamFileName"));
            Assert.That(res, Contains.Item("cmis:name"));
        }

        [Test]
        public void GetAllCmisPropertiesFromObject()
        {
            CmisHelper.RegisterTempObject("__getPropsTests.txt");
            var res = Shell.Execute(
                GetConnectToTestRepoCmd(),
                "$doc = " + CmdletName(typeof(NewCmisDocumentCommand)) + " __getPropsTests.txt",
                "$doc | " + GetCmisPropertyCmd
            );

            var obj = CmisHelper.Get("__getPropsTests.txt");
            Assert.That(res.Count, Is.EqualTo(obj.Properties.Count));
            foreach (var prop in obj.Properties)
            {
                // select value from member with same name as property from result
                var member = (from p in res
                        where (p is IProperty && ((IProperty)p).LocalName.Equals(prop.LocalName))
                        select ((IProperty)p).Value).ToList();
                Assert.That(member.Count, Is.EqualTo(1));
                Assert.That(member.First(), Is.EqualTo(prop.Value));
            }
        }

        [Test]
        public void GetSpecificCmisPropertyFromObject()
        {
            CmisHelper.RegisterTempObject("__getPropsTests.txt");
            var res = Shell.Execute(
                GetConnectToTestRepoCmd(),
                "$doc = " + CmdletName(typeof(NewCmisDocumentCommand)) + " __getPropsTests.txt",
                "$doc | " + GetCmisPropertyCmd + " *name"
            );
            Assert.That(res.Count, Is.EqualTo(2));

        }
    }
}

