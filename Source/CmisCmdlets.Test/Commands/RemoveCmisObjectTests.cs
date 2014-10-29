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
using DotCMIS.Exceptions;
using TestShell;
using System.Management.Automation;

namespace CmisCmdlets.Test.Commands
{
    [TestFixture]
    public class RemoveCmisObjectTests : TestBaseWithAutoConnect
    {
        public static readonly string RemoveCmisObjectCmd = "Remove-CmisObject ";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Shell.SetPreExecutionCommands(GetConnectToTestRepoCmd());
        }

        [Test]
        public void RemoveNonExistingObjectIsError()
        {
            var ex = Assert.Throws<ShellExecutionHasErrorsException>(delegate {
                Shell.Execute(RemoveCmisObjectCmd + "/__notExisting.txt");
            });
            var errors = ex.Errors;
            Assert.That(errors.Count, Is.EqualTo(1));
            var errorRecord = errors[0] as ErrorRecord;
            Assert.That(errorRecord.Exception, Is.InstanceOf<CmisObjectNotFoundException>());
        }

        [Test]
        public void RemoveDocument()
        {
            var f = CmisHelper.CreateTempFolder("__dummyDir");
            CmisHelper.CreateTempDocument("__dummyDir/removableFile.txt", "my content", "text/plain");

            var res = Shell.Execute(RemoveCmisObjectCmd + "__dummyDir/removableFile.txt");
            Assert.That(res, Is.Empty);
            Assert.That("__dummyDir/removableFile.txt", CmisHelper.DoesNotExist);

            CmisHelper.ForgetTempObjects();
            CmisHelper.RegisterTempObject(f);
        }

        [Test]
        public void RemoveEmptyDirectory()
        {
            CmisHelper.CreateTempFolder("__emptyDir");

            var res = Shell.Execute(RemoveCmisObjectCmd + "__emptyDir");
            Assert.That(res, Is.Empty);
            Assert.That("__emptyDir", CmisHelper.DoesNotExist);

            CmisHelper.ForgetTempObjects();
        }

        [Test]
        public void RemoveNonEmptyDirectoryWithoutRecursionIsError()
        {

        }

        [Test]
        public void RemoveNonEmptyDirectoryWithRecursion()
        {

        }
    }
}

