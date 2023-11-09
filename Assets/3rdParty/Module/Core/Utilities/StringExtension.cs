namespace Module.Core {
    public static class StringExtension {
        public static string RemoveRichText(this string source) {
            return source
                .Replace("<b>", string.Empty)
                .Replace("</b>", string.Empty)
                .Replace("&lt;b&gt;", string.Empty)
                .Replace("&lt;/b&gt;", string.Empty);
        }
    }
}
