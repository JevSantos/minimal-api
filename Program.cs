using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOS;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.Db;
using minimal_api.Infraestrutura.Interfaces;

#region Builder

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorSevico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(connectionString: builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion  

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorSevico administradorSevico) => {
    if(administradorSevico.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();  
    }).WithTags("Administradores");
#endregion

#region Veiculos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {
        var veiculo = new Veiculo {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
        };
    veiculoServico.Incluir(veiculo); 

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
    }).WithTags("Veiculos");

app.MapGet("/veiculos", ( [FromQuery] int? pagina, IVeiculoServico veiculoServico) => {
        var veiculos = veiculoServico.Todos(pagina);         
    return Results.Ok(veiculos);
    }).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ( [FromRoute] int id, IVeiculoServico veiculoServico) =>
    {
        var veiculo = veiculoServico.BuscaId(id);
        if (veiculo != null) return Results.Ok(veiculo);
        return Results.NotFound();
    }).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ( [FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {
        var veiculo = veiculoServico.BuscaId(id);
        if (veiculo == null) return Results.NotFound();

        veiculo.Nome = veiculoDTO.Nome;
        veiculo.Marca = veiculoDTO.Marca;
        veiculo.Ano = veiculoDTO.Ano;

        veiculoServico.Atualizar(veiculo);

        return Results.Ok(veiculo);

    }).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ( [FromRoute] int id, IVeiculoServico veiculoServico) => {
        var veiculo = veiculoServico.BuscaId(id);
        if (veiculo == null) return Results.NotFound();

        veiculoServico.ApagarPorId(veiculo);

        return Results.NoContent();

    }).WithTags("Veiculos");


#endregion

#region App
IApplicationBuilder applicationBuilder1 = app.UseSwagger();     //inicialmente era app.UseSwagger;
IApplicationBuilder applicationBuilder = app.UseSwaggerUI();    //inicialmente era app.UseSwaggerUI;

app.Run();
#endregion
