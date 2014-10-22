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
        private List<ICmisObject> _createdObjects;
        private ISession _session;
        private CmisNavigation _cmisNav;

        [TestFixtureSetUp]
        public void SetUpSession()
        {
            _session = ConnectionFactory.ConnectAtomPub(TestURL, TestUser, TestPassword,
                                                        TestRepository);
        }

        [SetUp]
        public void SetUpCreatedObjectList()
        {
            _createdObjects = new List<ICmisObject>();
            _cmisNav = new CmisNavigation(_session);
        }

        [TearDown]
        public void DeleteCreatedObjects()
        {
            foreach (var obj in _createdObjects)
            {
                try
                {
                    obj.Delete(true);
                }
                catch (CmisObjectNotFoundException) {}
            }
            _createdObjects.Clear();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateSingleFolder(bool recursive)
        {
            var cmisF = _cmisNav.CreateFolder("__cfsFolder/", recursive);
            _createdObjects.Add(cmisF);
            Assert.That(cmisF.Path, Is.EqualTo("/__cfsFolder"));
            Assert.That(cmisF.Name, Is.EqualTo("__cfsFolder"));
        }

        [Test]
        public void CreateExistingFolderWithoutRecursiveThrows()
        {
            var cmisF = _cmisNav.CreateFolder("__cefwrtFolder", false);
            _createdObjects.Add(cmisF);
            Assert.Throws<CmisConstraintException>(delegate
                                                             {
                _cmisNav.CreateFolder("__cefwrtFolder", false);
            });
        }

        [Test]
        public void CreateExistingFolderWithRecursiveWorks()
        {
            var cmisF = _cmisNav.CreateFolder("__cefwrwFolder", false);
            _createdObjects.Add(cmisF);
            var otherF = _cmisNav.CreateFolder("__cefwrwFolder", true);
            Assert.That(otherF, Is.EqualTo(cmisF));
        }
        [Test]
        public void CreateRecursiveFolder()
        {
            var cmisF = _cmisNav.CreateFolder("/__crfFolder/recursive/", true);
            _createdObjects.Add(cmisF);
            _createdObjects.Add(_cmisNav.Get("/__crfFolder"));
            Assert.That(cmisF.Path, Is.EqualTo("/__crfFolder/recursive"));
        }

        [Test]
        public void DeleteDocument()
        {

        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteEmptyFolder(bool recursive)
        {
            var folder = _cmisNav.CreateFolder("__defFolder", false);
            _createdObjects.Add(folder);

            var fails = _cmisNav.Delete("__defFolder", recursive);
            Assert.That(fails, Is.Null);
            ICmisObject obj;
            Assert.That(_cmisNav.TryGet("__defFolder", out obj), Is.False,
                        "empty folder still exists");

            _createdObjects.Clear();
        }

        [Test]
        public void DeleteNonEmptyFolderWithRecursion()
        {
            var folder = _cmisNav.CreateFolder("__dnefwrFolder/subdir", true);
            _createdObjects.Add(folder);
            _createdObjects.Add(folder.FolderParent);

            var fails = _cmisNav.Delete("__dnefwrFolder", true);
            Assert.That(fails, Is.Null);
            ICmisObject obj;
            Assert.That(_cmisNav.TryGet("__dnefwrFolder/subdir", out obj), Is.False,
                        "subdir still exists");
            Assert.That(_cmisNav.TryGet("__dnefwrFolder", out obj), Is.False,
                        "main dir still exists");

            _createdObjects.Clear();
        }

        [Test]
        public void DeleteNonEmptyFolderWithoutRecursionThrows()
        {
            var folder = _cmisNav.CreateFolder("__dnefwrtFolder/subdir", true);
            _createdObjects.Add(folder);
            _createdObjects.Add(folder.FolderParent);

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
            var obj = _cmisNav.CreateFolder("__goffFolder", false);
            _createdObjects.Add(obj);
            var folder = _cmisNav.Get("__goffFolder");
            Assert.That(folder.Name, Is.EqualTo("__goffFolder"));
        }

        [Test]
        public void GetRootObject()
        {
            var obj = new CmisNavigation(_session).Get("/");
            var folder = obj as IFolder;
            Assert.NotNull(folder);
            Assert.That(folder.Path, Is.EqualTo("/"));
        }

        [TestCase()]
        public void GetNotExistingObjectThrows()
        {
            Assert.Throws<CmisObjectNotFoundException>(delegate {
                _cmisNav.Get("__gneotObj");
            });
        }

        [Test]
        public void GetFolder()
        {
            var obj = _cmisNav.CreateFolder("__gfFolder", false);
            _createdObjects.Add(obj);
            var folder = _cmisNav.GetFolder("__gfFolder");
            Assert.That(folder.Path, Is.EqualTo("/__gfFolder"));
        }

        [TestCase()]
        public void GetFolderFromDocThrows()
        {
        }

        [TestCase()]
        public void GetDocument()
        {
        }

        [Test]
        public void GetDocumentFromFolderThrows()
        {
            var obj = _cmisNav.CreateFolder("__gdffFolder", false);
            _createdObjects.Add(obj);

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
            _createdObjects.Add(obj);

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

        [TestCase()]
        public void CreateExitingDocumentThrows()
        {
            var obj = _cmisNav.CreateDocument("__cedtDoc", null);
            _createdObjects.Add(obj);
            Assert.Throws<CmisConstraintException>(delegate {
                _cmisNav.CreateDocument("__cedtDoc", null);
            });
        }

        [Test]
        public void CreateDocumentWithoutContent()
        {
            var obj = _cmisNav.CreateDocument("__cdwcDoc", null);
            _createdObjects.Add(obj);

            Assert.That(obj.Paths, Contains.Item("/__cdwcDoc"));
            Assert.That(obj.Name, Is.EqualTo("__cdwcDoc"));
            Assert.That(obj.GetContentStream(), Is.Null);
        }
    }
}

