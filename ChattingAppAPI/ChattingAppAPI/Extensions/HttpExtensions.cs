using ChattingAppAPI.Helpers;
using System.Text.Json;

namespace ChattingAppAPI.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
    {
        var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize,
            data.TotalCount, data.TotalPages);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
        //for CORS to make angular access the Pagination
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
}