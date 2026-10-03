using System.Net.Http.Json;
using TodoApp.Shared;

namespace TodoApp.Client.Services;

public class UsageApiClient(HttpClient http)
{
    public async Task<UsageDto> GetAsync() =>
        (await http.GetFromJsonAsync<UsageDto>("api/usage"))!;
}
