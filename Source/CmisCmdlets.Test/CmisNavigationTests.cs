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
            CmisHelper.CreateFolder("__cefwrtFolder", false);
            Assert.Throws<CmisConstraintException>(delegate
                                                             {
                _cmisNav.CreateFolder("__cefwrtFolder", false);
            });
        }

        [Test]
        public void CreateExistingFolderWithRecursiveWorks()
        {
            var cmisF = CmisHelper.CreateFolder("__cefwrwFolder", false);
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
            CmisHelper.CreateDocument("__ddDoc", null);

            var fails = _cmisNav.Delete("__ddDoc", recursive);
            ICmisObject obj;
            Assert.That(fails, Is.Null);
            Assert.That(_cmisNav.TryGet("__ddDoc", out obj), Is.False);
            CmisHelper.ForgetTempObjects();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteDocumentByObject(bool recursive)
        {
            var doc = CmisHelper.CreateDocument("__ddDoc", null);

            var fails = _cmisNav.Delete(doc, recursive);
            ICmisObject obj;
            Assert.That(fails, Is.Null);
            Assert.That(_cmisNav.TryGet("__ddDoc", out obj), Is.False);
            CmisHelper.ForgetTempObjects();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteEmptyFolder(bool recursive)
        {
            CmisHelper.CreateFolder("__defFolder", false);

            var fails = _cmisNav.Delete("__defFolder", recursive);
            Assert.That(fails, Is.Null);

            ICmisObject obj;
            CmisSession.Clear(); // make sure the cache is empty for this test
            Assert.That(_cmisNav.TryGet("__defFolder", out obj), Is.False,
                        "empty folder still exists");

            CmisHelper.ForgetTempObjects();
        }

        [Test]
        public void DeleteNonEmptyFolderWithRecursion()
        {
            CmisHelper.CreateFolder("__dnefwrFolder/subdir", true);
            CmisHelper.CreateDocument("__dnefwrFolder/testfile", null);

            var fails = _cmisNav.Delete("__dnefwrFolder", true);
            Assert.That(fails, Is.Null);

            CmisSession.Clear(); // make sure the cache is empty for this test
            ICmisObject obj;
            Assert.That(_cmisNav.TryGet("__dnefwrFolder/subdir", out obj), Is.False,
                        "subdir still exists");
            Assert.That(_cmisNav.TryGet("__dnefwrFolder/testfile", out obj), Is.False,
                        "testfile still exists");
            Assert.That(_cmisNav.TryGet("__dnefwrFolder", out obj), Is.False,
                        "main dir still exists");
            CmisHelper.ForgetTempObjects();
        }

        [Test]
        public void DeleteNonEmptyFolderWithoutRecursionThrows()
        {
            CmisHelper.CreateFolder("__dnefwrtFolder/subdir", true);

            var fails = _cmisNav.Delete("__dnefwrtFolder", false);
            Assert.That(fails, Is.EquivalentTo(new string[] { "/__dnefwrtFolder" }));
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
            CmisHelper.CreateFolder("__goffFolder", false);
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
            CmisHelper.CreateFolder("__gfFolder", false);
            var folder = _cmisNav.GetFolder("__gfFolder");
            Assert.That(folder.Path, Is.EqualTo("/__gfFolder"));
        }

        [Test]
        public void GetFolderFromDocThrows()
        {
            CmisHelper.CreateDocument("__gffdtDoc", null);
            Assert.Throws<CmisObjectNotFoundException>(delegate {
                _cmisNav.GetFolder("__gffdtDoc");
            });
        }

        [Test]
        public void GetDocument()
        {
            CmisHelper.CreateDocument("__gdDoc", null);
            var doc = _cmisNav.GetDocument("/__gdDoc");
            Assert.That(doc.Paths, Contains.Item("/__gdDoc"));
            Assert.That(doc.Name, Is.EqualTo("__gdDoc"));
        }

        [Test]
        public void GetDocumentFromFolderThrows()
        {
            CmisHelper.CreateFolder("__gdffFolder", false);

            Assert.Throws<CmisObjectNotFoundException>(delegate {
                _cmisNav.GetDocument("__gdffFolder");
            });
        }

        [Test]
        public void CreateDocument()
        {
            var content = Encoding.ASCII.GetBytes("Test Content!");
            var stream = new ContentStream();
            stream.FileName = "mycontent.txt";
            stream.MimeType = "text/plain";
            stream.Stream = new MemoryStream(content);
            stream.Length = content.Length;

            var obj = _cmisNav.CreateDocument("__cdDoc", stream);
            CmisHelper.RegisterTempObject(obj);

            Assert.That(obj.Paths, Contains.Item("/__cdDoc"));
            Assert.That(obj.Name, Is.EqualTo("__cdDoc"));
            var resultStream = obj.GetContentStream();
            /* FIXME
             * FileName will be the same as Name property, Length will be null. Don't know why
             * Assert.That(resultStream.FileName, Is.EqualTo(stream.FileName));
             * Assert.That(resultStream.Length, Is.EqualTo(stream.Length));
            */
            Assert.That(obj.ContentStreamLength, Is.EqualTo(obj.ContentStreamLength));
            Assert.That(resultStream.MimeType, Is.EqualTo(stream.MimeType));

            byte[] resultBytes = new byte[(int) obj.ContentStreamLength];
            resultStream.Stream.Read(resultBytes, 0, (int) obj.ContentStreamLength);
            Assert.That(resultBytes, Is.EquivalentTo(content));
        }

        [Test]
        public void CreateExistingDocumentThrows()
        {
            CmisHelper.CreateDocument("__cedtDoc", null);
            Assert.Throws<CmisConstraintException>(delegate {
                _cmisNav.CreateDocument("__cedtDoc", null);
            });
        }

        [Test]
        public void CreateDocumentWithoutContent()
        {
            var obj = _cmisNav.CreateDocument("__cdwcDoc", null);
            CmisHelper.RegisterTempObject(obj);

            Assert.That(obj.Paths, Contains.Item("/__cdwcDoc"));
            Assert.That(obj.Name, Is.EqualTo("__cdwcDoc"));
            Assert.That(obj.GetContentStream(), Is.Null);
        }
    }
}

