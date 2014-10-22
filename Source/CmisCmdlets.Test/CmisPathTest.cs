// CmisCmdlets - Cmdlets to use CMIS from Powershell and Pash
// Copyright (C) GRAU DATA 2013-2014
//
// Author(s): Stefan Burnicki <stefan.burnicki@graudata.com>
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL was
// not distributed with this file, You can obtain one at
//  http://mozilla.org/MPL/2.0/.
using NUnit.Framework;
using System;

namespace CmisCmdlets.Test
{
    [TestFixture]
    public class CmisPathTest
    {
        [TestCase(@"/", @"/")]
        [TestCase(@"\", @"/")]
        [TestCase(@"foo/bar", @"foo/bar")]
        [TestCase(@"foo\bar", @"foo/bar")]
        [TestCase(@"foo/bar", @"foo/bar")]
        [TestCase(@"foo\\bar", @"foo\\bar")]
        [TestCase(@"/\\foo\bar/baz\\bla\", @"/\\foo/bar/baz\\bla/")]
        [TestCase("", "")]
        public void PathCorrectsSlashes(string input, string expected)
        {
            Assert.That(new CmisPath(input).ToString(), Is.EqualTo(expected));
        }

        [TestCase("/foo/bar//", "baz/bam", "/foo/bar/baz/bam")]
        [TestCase(@"\foo", @"bla\blub/baz\\bam", @"/foo/bla/blub/baz\\bam")]
        [TestCase(@"/path/to/somehwhere\", @"\other/path\to/here\", @"/other/path/to/here/")]
        public void PathCombiningWorks(string a, string b, string expected)
        {
            Assert.That(new CmisPath(a).Combine(b).ToString(), Is.EqualTo(expected));
        }

        [TestCase("//path///to/somewhere/", "/path/to/somewhere/")]
        [TestCase("anydir", "anydir")]
        [TestCase(@"\dir/too/../to/./.some/../../../root\directory", "/root/directory")]
        [TestCase(@"/complicated/./root/..\../path/..", "/")]
        [TestCase(@"../\./no/prob", "../no/prob")]
        [TestCase(@"./././foo/../okay", "okay")]
        [TestCase(@"..\test", "../test")]
        public void NormalizationWorks(string ugly, string expected)
        {
            Assert.That(new CmisPath(ugly).ToString(), Is.EqualTo(expected));
        }

        [TestCase("/./..")]
        [TestCase("/..")]
        [TestCase("/foo//bar/baz/../.././../tmp/../..")]
        public void NormalizationThrowsOnInvalidPath(string invalid)
        {
            Assert.Throws<CmisPathException>(delegate {
                new CmisPath(invalid);
            });
        }

        [TestCase("/foo/bar", "/foo/", "bar")]
        [TestCase("/foo/bar/", "/foo/bar/", "")]
        [TestCase("foo", "", "foo")]
        [TestCase("/", "/", "")]
        public void GetComponentsWorks(string path, string comp1, string comp2)
        {
            var components = new string[] { comp1, comp2 };
            Assert.That(new CmisPath(path).GetComponents(), Is.EquivalentTo(components));
        }
    }
}

