﻿using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 20, ErrorMessage = "El campo {0} no debe de tener más de {1} carácteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

    }
}
