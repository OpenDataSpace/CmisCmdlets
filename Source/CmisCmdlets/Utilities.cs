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
using System.Collections;

namespace CmisCmdlets
{
    public class Utilities
    {

        public static Dictionary<string, string> HashtableToStringDict(Hashtable hashtable)
        {
            if (hashtable == null)
            {
                return null;
            }
            return hashtable.Cast<DictionaryEntry>().ToDictionary(
                kvp => (string) kvp.Key.ToString(), kvp => (string) kvp.Value.ToString()
            );
        }

        public static Dictionary<string, object> HashtableToDict(Hashtable hashtable)
        {
            if (hashtable == null)
            {
                return null;
            }
            return hashtable.Cast<DictionaryEntry>().ToDictionary(
                kvp => (string) kvp.Key.ToString(), kvp => (object) kvp.Value
            );
        }

        
        public static void UpdateDictionary(IDictionary<string, object> original,
                                             IDictionary<string, object> updates)
        {
            if (updates == null)
            {
                return;
            }
            foreach (var pair in updates)
            {
                original[pair.Key] = pair.Value;
            }
        }
    }
}

