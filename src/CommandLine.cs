namespace VpnGuardForGames
{
    static class CommandLine
    {
        public static string SplitFirstToken(string commandLine, out string rest)
        {
            string trimmed = commandLine.TrimStart();
            string first;
            int end;

            if (trimmed.StartsWith("\""))
            {
                int closingQuote = trimmed.IndexOf('"', 1);
                if (closingQuote < 0)
                {
                    closingQuote = trimmed.Length;
                }

                first = trimmed.Substring(1, closingQuote - 1);
                end = System.Math.Min(closingQuote + 1, trimmed.Length);
            }
            else
            {
                end = trimmed.IndexOf(' ');
                if (end < 0)
                {
                    end = trimmed.Length;
                }

                first = trimmed.Substring(0, end);
            }

            rest = trimmed.Substring(end).TrimStart();
            return first;
        }
    }
}
