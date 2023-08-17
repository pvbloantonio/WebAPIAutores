using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "ObtenerRoot")]
        public ActionResult<IEnumerable<DatoHATEOAS>> Get()
        {
            var datosHateoas = new List<DatoHATEOAS>();

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerRoot", new {}), descripcion: "self", metodo: "GET"));

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "autores", metodo:"GET"));
            
            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }), descripcion: "autor-crear", metodo:"POST"));

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearLibro", new { }), descripcion: "libro-crear", metodo:"POST"));

            return datosHateoas;
        }
    }
}
