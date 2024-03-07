using Microsoft.AspNetCore.Mvc;
using Rinha.Backend.Data;
using Rinha.Backend.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddNpgsqlDataSource(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? ":x - connection string is missing!!!"
);

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

var app = builder.Build();

var clientes = new HashSet<int>{ 1, 2, 3, 4, 5 };

app.MapGet("/clientes/{id}/extrato", async (int id, IClienteRepository repository) =>
{
    if (!clientes.Contains(id))
        return Results.NotFound();
        
    return Results.Ok(await repository.GetExtratoAsync(id));
});

app.MapPost("/clientes/{id}/transacoes", async (int id, TransacaoRequest transacao, IClienteRepository repository) =>
{
    if (!clientes.Contains(id))
        return Results.NotFound();

    if (!transacao.Valida())
        return Results.UnprocessableEntity();

    var response = await repository.InserirTransacaoAsync(id, transacao);
    if (response is null)
        return Results.UnprocessableEntity();

    return Results.Ok(response);
});

app.Run();

[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(ExtratoResponse))]
[JsonSerializable(typeof(TransacaoRequest))]
[JsonSerializable(typeof(TransacaoResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
