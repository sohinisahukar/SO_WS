using Xunit;
using System.Net.Http.Json;
using RawCrudApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RawCrudApi.Tests.Integration;

public class BookApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BookApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Can_Create_Book()
    {
        var newBook = new Book { Title = "New Book", Author = "Author A", Year = 2024 };
        var response = await _client.PostAsJsonAsync("/api/books", newBook);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<Book>();
        Assert.NotNull(created?.Id);
    }

    [Fact]
    public async Task Can_Get_Book_By_Id()
    {
        var newBook = new Book { Title = "Read Test", Author = "Reader", Year = 2023 };
        var create = await _client.PostAsJsonAsync("/api/books", newBook);
        var created = await create.Content.ReadFromJsonAsync<Book>();

        var get = await _client.GetAsync($"/api/books/{created!.Id}");
        get.EnsureSuccessStatusCode();

        var result = await get.Content.ReadFromJsonAsync<Book>();
        Assert.Equal("Read Test", result!.Title);
    }

    [Fact]
    public async Task Can_Get_All_Books()
    {
        var response = await _client.GetAsync("/api/books");
        response.EnsureSuccessStatusCode();

        var books = await response.Content.ReadFromJsonAsync<List<Book>>();
        Assert.NotNull(books);
        Assert.True(books!.Count >= 0);
    }

    [Fact]
    public async Task Can_Update_Book()
    {
        var book = new Book { Title = "ToUpdate", Author = "Updater", Year = 2022 };
        var post = await _client.PostAsJsonAsync("/api/books", book);
        var created = await post.Content.ReadFromJsonAsync<Book>();

        created!.Title = "Updated Title";
        var put = await _client.PutAsJsonAsync($"/api/books/{created.Id}", created);
        put.EnsureSuccessStatusCode();

        var get = await _client.GetAsync($"/api/books/{created.Id}");
        var updated = await get.Content.ReadFromJsonAsync<Book>();
        Assert.Equal("Updated Title", updated!.Title);
    }

    [Fact]
    public async Task Can_Delete_Book()
    {
        var book = new Book { Title = "ToDelete", Author = "Remover", Year = 2020 };
        var post = await _client.PostAsJsonAsync("/api/books", book);
        var created = await post.Content.ReadFromJsonAsync<Book>();

        var delete = await _client.DeleteAsync($"/api/books/{created!.Id}");
        delete.EnsureSuccessStatusCode();

        var get = await _client.GetAsync($"/api/books/{created.Id}");
        Assert.False(get.IsSuccessStatusCode); // expecting 404
    }

}
