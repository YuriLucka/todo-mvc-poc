using System.Net.Http.Json;
using TodoApp.Shared;

namespace TodoApp.Client.Services;

public class TodoApiClient(HttpClient http)
{
    private const string Base = "api/todos";

    public async Task<List<TodoDto>> ListAsync() =>
        await http.GetFromJsonAsync<List<TodoDto>>(Base) ?? [];

    public async Task<TodoDto> CreateAsync(TodoInput input) =>
        await ReadAsync(await http.PostAsJsonAsync(Base, input));

    public async Task<TodoDto> UpdateAsync(int id, TodoInput input) =>
        await ReadAsync(await http.PutAsJsonAsync($"{Base}/{id}", input));

    public async Task<TodoDto> ToggleAsync(int id) =>
        await ReadAsync(await http.PatchAsync($"{Base}/{id}/toggle", null));

    public async Task DeleteAsync(int id) =>
        (await http.DeleteAsync($"{Base}/{id}")).EnsureSuccessStatusCode();

    private static async Task<TodoDto> ReadAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TodoDto>())!;
    }
}
