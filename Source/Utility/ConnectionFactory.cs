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
using System.Collections.Generic;
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using DotCMIS;

namespace Cmis.Utility
{
    public class ConnectionFactory
    {

        #region GetRepositories
        public static IList<IRepository> GetAtomPubRepositories(string featuredUrl)
        {
            string user, pw, rawUrl;
            ParseURL(featuredUrl, out user, out pw, out rawUrl);
            return GetAtomPubRepositories(rawUrl, user, pw);
        }

        public static IList<IRepository> GetAtomPubRepositories(string url, string user,
                                                                string password)
        {
            return GetRepositories(CreateAtomPubParams(url, user, password));
        }

        public static IList<IRepository> GetRepositories(IDictionary<string, string> parameters)
        {
            var fact = SessionFactory.NewInstance();
            return fact.GetRepositories(parameters);
        }

        #endregion

        #region Connection

        public static ISession ConnectAtomPub(string featuredUrl, string repoName)
        {
            string user, pw, rawUrl;
            ParseURL(featuredUrl, out user, out pw, out rawUrl);
            return ConnectAtomPub(rawUrl, user, pw, repoName);
        }

        public static ISession ConnectAtomPub(string url, string user, string password, 
                                              string repoName)
        {
            var parameters = CreateAtomPubParams(url, user, password);
            return Connect(parameters, repoName);
        }

        public static ISession ConnectAtomPubById(string url, string user, string password,
                                                  string repoId)
        {
            return Connect(CreateAtomPubParams(url, user, password, repoId));
        }

        public static ISession Connect(IDictionary<string, string> parameters, string repoName)
        { 
            var repo = GetRepositoryByName(parameters, repoName);
            parameters[SessionParameter.RepositoryId] = repo.Id;
            return Connect(parameters);
        }

        public static ISession Connect(IDictionary<string, string> parameters)
        {
            var fact = SessionFactory.NewInstance();
            return fact.CreateSession(parameters);
        }

        public static Dictionary<string, string> CreateAtomPubParams(string url, string user, string pw,
            string repoId = null)
        {
            var parameters = new Dictionary<string, string> {
                {SessionParameter.User, user},
                {SessionParameter.Password, pw},
                {SessionParameter.AtomPubUrl, url},
                {SessionParameter.BindingType, BindingType.AtomPub}
            };
            if (!String.IsNullOrEmpty(repoId))
            {
                parameters[SessionParameter.RepositoryId] = repoId;
            }
            return parameters;
        }

        #endregion

        #region internal stuff

        internal static IRepository GetRepositoryByName(IDictionary<string, string> parameters,
                                                        string repoName)
        {
            var repos = GetRepositories(parameters);
            var correctRepo = from rep in repos where rep.Name.Equals(repoName) select rep;
            var numFound = correctRepo.Count();
            if (numFound == 0)
            {
                throw new ArgumentException(String.Format("Repository '{0}' does not exist",
                                                          repoName));
            }
            else if (numFound > 1)
            {
                throw new ArgumentException(String.Format("Multiple repositories with name '{0}'",
                                                          repoName));
            }
            return correctRepo.First();
        }

        internal static void ParseURL(string url, out string username, out string password,
                                      out string rawURL)
        {
            username = "";
            password = "";
            rawURL = "";
            if (String.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL is null or empty");
            }
            var protSplit = url.IndexOf("://");
            if (protSplit < 1)
            {
                throw new ArgumentException(
                    String.Format("URL '{0}' contains invalid protocol", url));
            }
            string protocol = url.Substring(0, protSplit + 3);
            string urlRest = url.Substring(protSplit + 3);

            int usersplit = urlRest.IndexOf("@");
            if (usersplit < 0) //no user name and password
            {
                rawURL = url;
                return;
            }
            // else we have user (and probably pw) set
            rawURL = protocol + urlRest.Substring(usersplit + 1);
            var userAndPw = urlRest.Substring(0, usersplit);

            var pwsplit = userAndPw.IndexOf(":");
            if (pwsplit < 0) //only user provided
            {
                username = userAndPw;
                return;
            }
            username = userAndPw.Substring(0, pwsplit);
            password = userAndPw.Substring(pwsplit + 1);
        }

        #endregion
    }
}

