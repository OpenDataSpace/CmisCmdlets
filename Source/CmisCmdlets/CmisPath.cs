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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using DotCMIS.Enums;

namespace CmisCmdlets
{
    public class CmisPath
    {
        private static readonly Regex WrongSlashSearch = new Regex(@"(?<!\\)\\(?!\\)");
        private static readonly Regex CorrectSlashSearch = new Regex(@"/");

        public static readonly string CorrectSlash = "/";
        public static readonly string WrongSlash = @"\";

        private string _path;

        public CmisPath(string path)
        {
            _path = NormalizeSlashes(path);
            Normalize();
        }

        public static implicit operator CmisPath(string path)
        {
            return new CmisPath(path);
        }

        public CmisPath Combine(CmisPath other)
        {
            if (other.IsAbsolutePath())
            {
                return other.Clone();
            }
            return new CmisPath(_path + CorrectSlash + other.ToString());
        }

        public bool IsAbsolutePath()
        {
            return _path.StartsWith(CorrectSlash);
        }

        public bool HasTrailingSlash()
        {
            return _path.EndsWith(CorrectSlash);
        }

        public CmisPath WithoutTrailingSlash()
        {
            if (!HasTrailingSlash())
            {
                return this;
            }
            return new CmisPath(_path.Remove(_path.Length - 1, 1));
        }

        public string[] Split()
        {
            return CorrectSlashSearch.Split(_path).Where( x => x.Length > 0).ToArray();
        }

        public string[] GetComponents()
        {
            if (HasTrailingSlash())
            {
                return new string[] {_path, ""};
            }
            var parts = Split().ToList();
            var basename = parts.Last();
            parts.RemoveAt(parts.Count - 1);
            if (IsAbsolutePath())
            {
                parts.Insert(0, "");
            }
            var basepath = String.Join(CorrectSlash, parts);
            if (basepath.Length > 0)
            {
                basepath += CorrectSlash;
            }
            return new string[] { basepath, basename };
        }

        public CmisPath Clone()
        {
            return new CmisPath(_path);
        }

        public override string ToString()
        {
            return _path;
        }

        public override bool Equals(object obj)
        {
            if (obj is CmisPath)
            {
                var path = obj as CmisPath;
                if (Object.ReferenceEquals(this, path))
                {
                    return true;
                }
                return _path.Equals(path.ToString());
            }
            throw new ArgumentException("Cannot compare against non-CmisPath object.");
        }

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }

        public static string NormalizeSlashes(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return "";
            }
            return WrongSlashSearch.Replace(path, CorrectSlash);
        }
        
        private void Normalize()
        {
            var parts = Split();
            var newParts = new List<string>();
            foreach (var part in parts)
            {
                // handle (and remove) relative directory directives
                if (part.Length == 0)
                {
                    // empty parts might be double slashes and can be skipped. start and end are
                    // handled separately below
                    continue;
                }
                if (part.Equals("..") && newParts.Count > 0)
                {
                    newParts.RemoveAt(newParts.Count - 1);
                }
                else if (part.Equals("."))
                {
                    // "." can just be skipped if not at the beginning
                    continue;
                }
                else
                {
                    newParts.Add(part);
                }
            }
            // absolute path means we shouldn't have a "." or ".." at the beginning
            if (IsAbsolutePath())
            {
                if (newParts.Count == 0)
                {
                    _path = CorrectSlash;
                    return;
                }
                if (newParts[0].Equals(".."))
                {
                    // "/../foo" is invalid
                    var msg = String.Format("Invalid path '{0}' goes beyond root directory", 
                                            String.Join(CorrectSlash, newParts));
                    throw new CmisPathException(msg);
                }
                // makes sure we get a CorrectSlash infront when joining
                newParts.Insert(0, "");
            }
            var newPath = String.Join(CorrectSlash, newParts);
            // check for a trailing slash to preserve
            if (HasTrailingSlash())
            {
                newPath += CorrectSlash;
            }
            _path = newPath;
        }
    }
}

