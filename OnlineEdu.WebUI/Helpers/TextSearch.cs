using System.Globalization;

namespace OnlineEdu.WebUI.Helpers
{
    public static class TextSearch
    {
        private static readonly CompareInfo Turkish = new CultureInfo("tr-TR").CompareInfo;

        // Türkçe karakterleri doğru eşleyen, büyük/küçük harf duyarsız arama (İ/i, I/ı)
        public static bool ContainsTr(this string? source, string? term)
        {
            if (string.IsNullOrWhiteSpace(term)) return true;
            if (string.IsNullOrEmpty(source)) return false;
            return Turkish.IndexOf(source, term.Trim(), CompareOptions.IgnoreCase) >= 0;
        }
    }
}
