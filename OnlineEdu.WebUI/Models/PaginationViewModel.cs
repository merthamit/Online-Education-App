namespace OnlineEdu.WebUI.Models
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string BaseUrl { get; set; } = "";

        // Sayfa linklerinde korunacak diğer parametreler (arama, filtre, sıralama)
        public Dictionary<string, string?> Query { get; set; } = new();

        public string? Search
        {
            get => Query.TryGetValue("search", out var value) ? value : null;
            set => Query["search"] = value;
        }

        public string PageUrl(int page)
        {
            var parts = Query.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                             .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}")
                             .Append($"page={page}");
            return $"{BaseUrl}?{string.Join("&", parts)}";
        }

        // Listeyi sayfalar ve sayfa numarasını geçerli aralığa çeker
        public static List<T> Paginate<T>(List<T> items, int page, int pageSize, string baseUrl, out PaginationViewModel pagination, string? search = null)
        {
            var totalPages = Math.Max(1, (int)Math.Ceiling(items.Count / (double)pageSize));
            page = Math.Clamp(page, 1, totalPages);
            pagination = new PaginationViewModel { CurrentPage = page, TotalPages = totalPages, BaseUrl = baseUrl, Search = search };
            return items.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
