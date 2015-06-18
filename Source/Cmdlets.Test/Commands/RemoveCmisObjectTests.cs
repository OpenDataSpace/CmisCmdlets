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
using PSTesting;

namespace CmisCmdlets.Test.Commands
{
    [TestFixture]
    public class RemoveCmisObjectTests : TestBaseWithAutoConnect
    {
        public static readonly string RemoveCmisObjectCmd = "Remove-CmisObject ";

        [Test]
        public void RemoveNonExistingObjectIsError()
        {
            var ex = Assert.Throws<ShellExecutionHasErrorsException>(delegate {
                Shell.Execute(RemoveCmisObjectCmd + "/__notExisting.txt");
            });
            var errors = ex.Errors;
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].Exception, Is.InstanceOf<CmisRuntimeException>());
        }

        [Test]
        public void RemoveDocument()
        {
            var f = CmisHelper.CreateTempFolder("__dummyDir");
            CmisHelper.CreateTempDocument("__dummyDir/removableFile.txt");

            var res = Shell.Execute(RemoveCmisObjectCmd + "__dummyDir/removableFile.txt");
            Assert.That(res, Is.Empty);
            Assert.That("__dummyDir/removableFile.txt", CmisHelper.DoesNotExist);

            CmisHelper.ForgetTempObjects();
            CmisHelper.RegisterTempObject(f);
        }

        [Test]
        public void RemoveDocumentByObject()
        {
            CmisHelper.RegisterTempObject("/__removeByObject.txt");

            var res = Shell.Execute(
                "$doc = " + CmdletName(typeof(NewCmisDocumentCommand)) + " /__removeByObject.txt",
                RemoveCmisObjectCmd + " -Object $doc"
            );
            Assert.That(res, Is.Empty);
            Assert.That("/__removeByObject.txt", CmisHelper.DoesNotExist);
            CmisHelper.ForgetTempObjects();
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
            CmisHelper.CreateTempFolder("__dummyDir/subdir", true);
            var ex = Assert.Throws<ShellExecutionHasErrorsException>(delegate {
                Shell.Execute(RemoveCmisObjectCmd + "/__dummyDir");
            });
            var errors = ex.Errors;
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].Exception, Is.InstanceOf<CmisRuntimeException>());
        }

        [Test]
        public void RemoveNonEmptyDirectoryWithRecursion()
        {
            CmisHelper.CreateTempFolder("/__dummyDir2/subdir", true);

            var res = Shell.Execute(RemoveCmisObjectCmd + "__dummyDir2 -Recursive");
            Assert.That(res, Is.Empty);
            Assert.That("/__dummyDir2", CmisHelper.DoesNotExist);
            Assert.That("__dummyDir2/subdir", CmisHelper.DoesNotExist);

            CmisHelper.ForgetTempObjects();
        }
    }
}

