using System;
using System.Collections.Generic;

namespace VH.PluralsightScraper
{
    internal static class ArgsExtensions
    {
        public static bool RunHeadless(this string[] args)
        {
            string argValue = GetArgValue(args, searchArgName: "-headless", isOptional: true);

            return bool.TryParse(argValue, out bool runHeadless) && runHeadless;
        }

        public static bool IsFastRun(this string[] args)
        {
            string argValue = GetArgValue(args, searchArgName: "-isFastRun", isOptional: true);

            return bool.TryParse(argValue, out bool isFastRun) && isFastRun;
        }

        public static string Password(this string[] args)
        {
            return GetArgValue(args, searchArgName: "-password", isOptional: false);
        }

        public static string Username(this string[] args)
        {
            return GetArgValue(args, searchArgName: "-username", isOptional: false);
        }

        private static string GetArgValue(IReadOnlyList<string> args, string searchArgName, bool isOptional)
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

            if (isOptional)
            {
                return null;
            }

            throw new Exception($"argument [{searchArgName}] is missing");
        }
    }
}
