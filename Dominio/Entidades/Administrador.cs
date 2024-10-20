using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Dominio.Entidades
{
    public class Administrador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set;} = default!;

        [StringLength(255)]
         public string Email { get; set;} = default!;

         [StringLength(50)]
        public string Senha { get; set;} = default!;

        [StringLength(10)]
         public string Perfil { get; set;} = default!;
    }
}