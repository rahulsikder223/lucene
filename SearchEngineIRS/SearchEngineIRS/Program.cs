using LuceneWorker;
using System.Web.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

WriteIndex writer = new WriteIndex();
//writer.Write();
ReadIndex reader = new ReadIndex();

app.MapGet("/search", ([FromUri] string query) =>
{
    var result = reader.Read(query);
    return result;
})
.WithName("GetSearchResults");

app.Run();