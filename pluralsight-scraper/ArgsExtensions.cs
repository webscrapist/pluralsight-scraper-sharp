using System;
using System.Collections.Generic;

// santi: [next] generate initial migration again
// santi: [next] move migrations folder under Data namespace
// santi: [next] run app against postgre

namespace VH.PluralsightScraper
{
    internal static class ArgsExtensions
    {
        public static bool Headless(this string[] args)
        {
            string argValue = GetArgValue(args, searchArgName: "-headless");

            return !bool.TryParse(argValue, out bool headless) || headless;
        }

        public static string Password(this string[] args)
        {
            return GetArgValue(args, searchArgName: "-password");
        }

        public static string Username(this string[] args)
        {
            return GetArgValue(args, searchArgName: "-username");
        }

        private static string GetArgValue(IReadOnlyList<string> args, string searchArgName)
        {
            for (var i = 0; i < args.Count; i++)
            {
                string thisArgName = args[i].ToLower();

                if (thisArgName != searchArgName.ToLower())
                {
                    continue;
                }

                string argValue = args[i + 1];

                return argValue.StartsWith("-") ? null : argValue;
            }

            throw new Exception($"argument [{searchArgName}] is missing");
        }
    }
}
