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
using NUnit.Framework.Constraints;
using DotCMIS.Client;
using System.Linq;

namespace CmisUtility.Test.Constraints
{
    public abstract class CmisBaseConstraint : Constraint
    {
        protected string ActualValue { get; set; }
        protected string ExpectedValue { get; set; }


        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WriteLine(ExpectedValue);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteLine(ActualValue);
        }

        protected bool Problem(string format, object expected, object actual)
        {
            return Problem(String.Format(format, expected), String.Format(format, actual));
        }

        protected bool Problem(string expected, string actual)
        {
            ExpectedValue = expected.ToString();
            ActualValue = actual.ToString();
            return false;
        }

        protected bool MatchObject(ICmisObject expected, ICmisObject actual)
        {
            if (actual == null)
            {
                return Problem("An ICmisObject", "Something else");
            }
            if (expected.BaseTypeId != actual.BaseTypeId)
            {
                return Problem("Object with BaseTypeId {0}", expected.BaseTypeId,
                    actual.BaseTypeId);
            }
            if (expected.Id != actual.Id)
            {
                return Problem("Object with Id \"{0}\"", expected.BaseTypeId,
                    actual.BaseTypeId);
            }
            /*
             * This would have to be done by hand. Skipping for now
            if (!expected.Properties.SequenceEqual(actual.Properties))
            {
                return Problem("Object with Properties \"{0}\"", expected.Properties,
                               actual.Properties);
            }
            */
            if (expected is IFolder)
            {
                return MatchFolder(expected as IFolder, actual as IFolder);
            }
            else if (expected is IDocument)
            {
                return MatchDocument(expected as IDocument, actual as IDocument);
            }
            return true;
        }

        protected bool MatchFolder(IFolder expected, IFolder actual)
        {
            if (actual == null)
            {
                return Problem("An IFolder object", "Something else");
            }
            if (!expected.Path.Equals(actual.Path))
            {
                return Problem("IFolder with Path {0}", expected.Path, actual.Path);
            }
            return true;
        }

        protected bool MatchDocument(IDocument expected, IDocument actual)
        {
            if (actual == null)
            {
                return Problem("An IDocument object", "Something else");
            }
            if (!expected.Paths.SequenceEqual(actual.Paths))
            {
                return Problem("IDocument with Paths {0}", expected.Paths, actual.Paths);
            }
            return true;
        }
    }
}

