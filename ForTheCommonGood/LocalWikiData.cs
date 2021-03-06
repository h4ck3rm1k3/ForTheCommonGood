﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace ForTheCommonGood
{
    static class LocalWikiData
    {
        public static string LocalDomain { get; private set; }
        public static string DisplayName { get; private set; }

        public static string Category1 { get; private set; }
        public static string Category2 { get; private set; }
        public static string Category3 { get; private set; }
        public static string DefaultCategory { get; private set; }

        public static string Information { get; private set; }
        public static string Description { get; private set; }
        public static string Date { get; private set; }
        public static string Source { get; private set; }
        public static string Author { get; private set; }
        public static string Permission { get; private set; }
        public static string Other_versions { get; private set; }

        public static string NowCommonsTag { get; private set; }
        public static string CopyToCommonsRegex { get; private set; }

        public static string Summary { get; private set; }
        public static string Licensing { get; private set; }
        public static string CategoryNamespace { get; private set; }

        public class PotentialProblem
        {
            public string Test { get; set; }
            public bool IsRegex { get; set; }
            public string Message { get; set; }
        }
        public static PotentialProblem[] PotentialProblems { get; private set; }

        public static StringDictionary Replacements { get; private set; }
        public static StringDictionary SelfLicenseReplacements { get; private set; }

        public static void LoadWikiData(string[] lines)
        {
            LocalDomain = Category1 = Category2 = Category3 = Information = Description =
                Date = Source = Author = Permission = Other_versions = Summary = Licensing = "";
            List<PotentialProblem> problems = new List<PotentialProblem>();
            Replacements = new StringDictionary();
            SelfLicenseReplacements = new StringDictionary();
            DefaultCategory = "1";

            for (int i = 0; i < lines.Length; i++)
            {
                string l = lines[i];
                if (l == "" || l.StartsWith("#"))
                    continue;
                if (l == "[PotentialProblem]")
                {
                    string test = lines[i + 1];
                    string message = lines[i + 2];
                    PotentialProblem prob = new PotentialProblem();
                    prob.Test = test.Substring(test.IndexOf('=') + 1);
                    prob.IsRegex = test.StartsWith("IfRegex=");
                    prob.Message = message.Substring(message.IndexOf('=') + 1);
                    problems.Add(prob);
                    i += 2;
                }
                else if (l == "[Replacement]")
                {
                    string lookfor = lines[i + 1];
                    string replacewith = lines[i + 2];
                    Replacements.Add(lookfor.Substring(lookfor.IndexOf('=') + 1),
                        replacewith.Substring(replacewith.IndexOf('=') + 1));
                    i += 2;
                }
                else if (l == "[SelfLicenseReplacement]")
                {
                    string lookfor = lines[i + 1];
                    string replacewith = lines[i + 2];
                    SelfLicenseReplacements.Add(lookfor.Substring(lookfor.IndexOf('=') + 1),
                        replacewith.Substring(replacewith.IndexOf('=') + 1));
                    i += 2;
                }
                else
                {
                    try
                    {
                        // a very naughty little bit of covert reflection here
                        typeof(LocalWikiData).GetProperty(l.Substring(0, l.IndexOf('=')), BindingFlags.Static | BindingFlags.Public)
                            .SetValue(null, l.Substring(l.IndexOf('=') + 1), null);
                    }
                    catch (Exception)
                    {
                        if (Debugger.IsAttached)
                            Debugger.Break();
                        //MessageBox.Show(Localization.GetString("LocalWikiDataLoadFailed") + "\n\n" + e.Message);
                        // silently fail for the moment
                    }
                }
            }

            PotentialProblems = problems.ToArray();
        }
    }
}
