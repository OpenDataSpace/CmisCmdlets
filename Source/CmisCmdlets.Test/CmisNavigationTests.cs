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
using System.Collections.Generic;
using DotCMIS.Exceptions;
using DotCMIS.Enums;
using DotCMIS.Data.Impl;
using System.IO;
using System.Text;
using DotCMIS.Binding.Services;

namespace CmisCmdlets.Test
{
    [TestFixture]
    public class CmisNavigationTests : TestBase
    {
        private CmisNavigation _cmisNav;

        [SetUp]
        public void SetUp()
        {
            _cmisNav = new CmisNavigation(CmisSession);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateSingleFolder(bool recursive)
        {
            var cmisF = _cmisNav.CreateFolder("__cfsFolder/", recursive);
            CmisHelper.RegisterTempObject(cmisF);
            Assert.That(cmisF.Path, Is.EqualTo("/__cfsFolder"));
            Assert.That(cmisF.Name, Is.EqualTo("__cfsFolder"));
        }

        [Test]
        public void CreateExistingFolderWithoutRecursiveThrows()
        {
            CmisHelper.CreateTempFolder("__cefwrtFolder", false);
            Assert.Throws<CmisConstraintException>(delegate {
                _cmisNav.CreateFolder("__cefwrtFolder", false);
            });
        }

        [Test]
        public void CreateExistingFolderWithRecursiveWorks()
        {
            var cmisF = CmisHelper.CreateTempFolder("__cefwrwFolder", false);
            var otherF = _cmisNav.CreateFolder("__cefwrwFolder", true);
            Assert.That(otherF, Is.EqualTo(cmisF));
        }
        [Test]
        public void CreateRecursiveFolder()
        {
            var cmisF = _cmisNav.CreateFolder("/__crfFolder/recursive/", true);
            CmisHelper.RegisterTempObject(CmisHelper.Get("/__crfFolder"), cmisF);
            Assert.That(cmisF.Path, Is.EqualTo("/__crfFolder/recursive"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteDocument(bool recursive)
        {
            CmisHelper.CreateTempDocument("__ddDoc", null);

            var fails = _cmisNav.Delete("__ddDoc", recursive);
            Assert.That(fails, Is.Null);
            Assert.That("__ddDoc", CmisHelper.DoesNotExist);
            CmisHelper.ForgetTempObjects();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteDocumentByObject(bool recursive)
        {
            var doc = CmisHelper.CreateTempDocument("__ddDoc", null);

            var fails = _cmisNav.Delete(doc, recursive);
            Assert.That(fails, Is.Null);
            Assert.That("__ddDoc", CmisHelper.DoesNotExist);
            CmisHelper.ForgetTempObjects();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteEmptyFolder(bool recursive)
        {
            CmisHelper.CreateTempFolder("__defFolder", false);

            var fails = _cmisNav.Delete("__defFolder", recursive);
            Assert.That(fails, Is.Null);
            Assert.That("__defFolder", CmisHelper.DoesNotExist);
            CmisHelper.ForgetTempObjects();
        }

        [Test]
        public void DeleteNonEmptyFolderWithRecursion()
        {
            CmisHelper.CreateTempFolder("__dnefwrFolder/subdir", true);
            CmisHelper.CreateTempDocument("__dnefwrFolder/testfile", null);

            var fails = _cmisNav.Delete("__dnefwrFolder", true);
            Assert.That(fails, Is.Null);
            Assert.That("__dnefwrFolder/subdir", CmisHelper.DoesNotExist);
            Assert.That("__dnefwrFolder/testfile", CmisHelper.DoesNotExist);
            Assert.That("__dnefwrFolder", CmisHelper.DoesNotExist);
            CmisHelper.ForgetTempObjects();
        }

        [Test]
        public void DeleteNonEmptyFolderWithoutRecursionThrows()
        {
            CmisHelper.CreateTempFolder("__dnefwrtFolder/subdir", true);

            var fails = _cmisNav.Delete("__dnefwrtFolder", false);
            Assert.That(fails, Is.EquivalentTo(new string[] { "/__dnefwrtFolder/" }));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteNotExistingObjectReturnsProblems(bool recursive)
        {
            var fails = _cmisNav.Delete("__dneotObj", recursive);
            Assert.That(fails, Is.EquivalentTo(new string[] { "/__dneotObj" }));
        }

        [Test]
        public void GetObjectFromFolder()
        {
            CmisHelper.CreateTempFolder("__goffFolder", false);
            var folder = _cmisNav.Get("__goffFolder");
            Assert.That(folder.Name, Is.EqualTo("__goffFolder"));
        }

        [Test]
        public void GetRootObject()
        {
            var obj = _cmisNav.Get("/");
            var folder = obj as IFolder;
            Assert.NotNull(folder);
            Assert.That(folder.Path, Is.EqualTo("/"));
        }

        [Test]
        public void GetNotExistingObjectThrows()
        {
            Assert.Throws<CmisObjectNotFoundException>(delegate {
                _cmisNav.Get("__gneotObj");
            });
        }

        [Test]
        public void GetFolder()
        {
            CmisHelper.CreateTempFolder("__gfFolder", false);
            var folder = _cmisNav.GetFolder("__gfFolder");
            Assert.That(folder.Path, Is.EqualTo("/__gfFolder"));
        }

        [Test]
        public void GetFolderFromDocThrows()
        {
            CmisHelper.CreateTempDocument("__gffdtDoc", null);
            Assert.Throws<CmisObjectNotFoundException>(delegate {
                _cmisNav.GetFolder("__gffdtDoc");
            });
        }

        [Test]
        public void GetDocument()
        {
            CmisHelper.CreateTempDocument("__gdDoc", null);
            var doc = _cmisNav.GetDocument("/__gdDoc");
            Assert.That(doc.Paths, Contains.Item("/__gdDoc"));
            Assert.That(doc.Name, Is.EqualTo("__gdDoc"));
        }

        [Test]
        public void GetDocumentFromFolderThrows()
        {
            CmisHelper.CreateTempFolder("__gdffFolder", false);

            Assert.Throws<CmisObjectNotFoundException>(delegate {
                _cmisNav.GetDocument("__gdffFolder");
            });
        }

        [Test]
        public void CreateDocument()
        {
            var content = Encoding.UTF8.GetBytes("Test Content!");
            var stream = new ContentStream();
            stream.FileName = "mycontent.txt";
            stream.MimeType = "text/plain";
            stream.Stream = new MemoryStream(content);
            stream.Length = content.Length;

            var obj = _cmisNav.CreateDocument("__cdDoc", stream);
            CmisHelper.RegisterTempObject(obj);

            Assert.That(obj.Paths, Contains.Item("/__cdDoc"));
            Assert.That(obj.Name, Is.EqualTo("__cdDoc"));
            Assert.That(obj, CmisHelper.HasContent(content, stream.MimeType));
        }

        [Test]
        public void CreateExistingDocumentThrows()
        {
            CmisHelper.CreateTempDocument("__cedtDoc", null);
            Assert.Throws<CmisConstraintException>(delegate {
                _cmisNav.CreateDocument("__cedtDoc", null);
            });
        }

        [Test]
        public void CreateDocumentWithoutContent()
        {
            var obj = _cmisNav.CreateDocument("__cdwcDoc", null);
            CmisHelper.RegisterTempObject(obj);

            var conbtentStream = obj.GetContentStream();
            Assert.That(obj.Paths, Contains.Item("/__cdwcDoc"));
            Assert.That(obj.Name, Is.EqualTo("__cdwcDoc"));
            Assert.That(conbtentStream, Is.Not.Null);
            Assert.That(conbtentStream.Length, Is.EqualTo(0));
        }
    }
}

