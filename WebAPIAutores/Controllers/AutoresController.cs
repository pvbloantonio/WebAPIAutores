using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;
using WebAPIAutores.Filtros;


namespace WebAPIAutores.Controllers
{
    [Route("api/autores")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController( ApplicationDbContext context, IMapper mapper, IConfiguration configuration )
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        

        [HttpGet(Name = "ObtenerAutores")]
        [AllowAnonymous]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
           var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id:int}", Name = "obtenerAutor")]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorlibroDB => autorlibroDB.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(autor == null)
            {
                return NotFound();
            }
            return mapper.Map<AutorDTOConLibros>(autor);

            
        }

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombre")]
        public async Task<ActionResult<List<AutorDTO>>> Get(string nombre)
        {
            var autores = await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost(Name = "crearAutor")]

        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

            if(existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

           
            var autor = mapper.Map<Autor>(autorCreacionDTO);


            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutor", new {id = autor.Id}, autorDTO );
        }

        [HttpPut("{id:int}", Name ="actualizarAutor")]

        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if(autor.Id != id) 
            {
                return BadRequest("El id del autor no coincide con el id de la url");
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}", Name = "borrarAutor")]

        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if(!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id});
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
