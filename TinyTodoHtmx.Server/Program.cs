using Htmx;
using Htmx.TagHelpers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddAntiforgery();
    builder.Services.AddSingleton<Database>();
}
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAntiforgery();
app.MapHtmxAntiforgeryScript();

app.MapGet("/", (IServiceProvider sp) => Results.Extensions.RazorSlice("/Slices/Index.cshtml", sp));

app.MapPost("/add", ([FromForm]string title, Database db) => {
    db.Todos.Add(new Todo(title));
    return Results.Extensions.RazorSlice("/Slices/_Todos.cshtml", db.Todos);
});

app.MapPost("/addStart", ([FromForm]string title, HttpContext context, Database db) => {
    db.Todos.Add(new Todo(title));
    db.Todos.Reverse();
    context.Response.Htmx(h => { h.WithTrigger("cool"); });
    return Results.Extensions.RazorSlice("/Slices/_Todos.cshtml", db.Todos);
});

app.MapDelete("/delete", ([FromForm]int index, Database db) => {
    db.Todos.RemoveAt(index);
    return Results.Extensions.RazorSlice("/Slices/_Todos.cshtml", db.Todos);
});

app.Run();
