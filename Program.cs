using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOS;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enuns;
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
    
app.MapGet("/administradores/", ([FromQuery] int? pagina, IAdministradorSevico administradorSevico) => {
    var adms = new List<AdministradorModelView>();
    var administradores = administradorSevico.Todos(pagina);
    foreach(var adm in administradores)
    {
        adms.Add(new AdministradorModelView{
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(adms);  
    }).WithTags("Administradores");

app.MapGet("/administradores/{id}", ( [FromRoute] int id, IAdministradorSevico administradorSevico) =>
    {
        var administrador = administradorSevico.BuscaPorId(id);
        if (administrador != null) return Results.Ok(new AdministradorModelView{
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
        return Results.NotFound();
    }).WithTags("Administradores");    

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorSevico administradorSevico) => {
    var validacao = new ErrosDeValidacao{
        Mensagem = new List<string>()
    };
    
    if(string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagem.Add("Email não pode ser vazio");

    if(string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagem.Add("Senha não pode ser vazia");

    if(administradorDTO.Perfil == null)
        validacao.Mensagem.Add("Email não pode ser vazio");    
        
            if(validacao.Mensagem.Count > 0)
            return Results.BadRequest(validacao);

        var administrador = new Administrador {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil?.ToString() ?? Perfil.Editor.ToString()
        };

    administradorSevico.Incluir(administrador); 

    return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView{
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
}).WithTags("Administradores");



#endregion

#region Veiculos
ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao{
        Mensagem = new List<string>()
    };

    if(string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagem.Add("O nome não pode ser nulo");
    if(string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagem.Add("A marca não pode ficar em branco");
    if(veiculoDTO.Ano < 1950)
        validacao.Mensagem.Add("O veículo é muito antigo, somente anos superiores a 1950 são aceitos");
    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
    {   
        var validacao = validaDTO(veiculoDTO);
            if(validacao.Mensagem.Count > 0)
            return Results.BadRequest(validacao);

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
    
    var validacao = validaDTO(veiculoDTO);
            if(validacao.Mensagem.Count > 0)
            return Results.BadRequest(validacao);
 
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
