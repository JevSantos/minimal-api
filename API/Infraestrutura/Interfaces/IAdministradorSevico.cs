using minimal_api.Dominio.DTOS;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestrutura.Interfaces
{
    public interface IAdministradorSevico
    {
        Administrador? Login(LoginDTO loginDTO);

        Administrador? Incluir(Administrador administrador);

        Administrador? BuscaPorId(int pagina);
        
        List<Administrador> Todos(int? pagina);
    }
}