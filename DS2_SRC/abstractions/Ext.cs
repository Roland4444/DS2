namespace ExtensionMethods
{
    public static class Extensions
    {
        public static string sbstr(this String str, int begin, int end)
        {
            return str.Substring(begin, end-begin);
        }

        public static string sbstr(this String str, int begin)
        {
            return str.Substring(begin);
        }
    }
}