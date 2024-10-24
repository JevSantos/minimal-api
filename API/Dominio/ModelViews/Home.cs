namespace minimal_api.Dominio.ModelViews;

public struct Home
{
    public readonly string Mensagem { get => "Bem vindo a API de veículos - Minimal API"; }
    public readonly string Doc { get => "/swagger";}
}