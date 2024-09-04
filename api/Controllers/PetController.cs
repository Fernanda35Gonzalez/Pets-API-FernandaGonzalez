using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Mappers;
using api.Dtos.Pet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
   
    [Route("api/pet")]
    [ApiController]
    public class PetController : ControllerBase
    {

        private readonly ApplicationDBContext _context;
        public PetController(ApplicationDBContext context)
        {
            _context = context;
        }
        
        [HttpGet] //obtener
        public async Task<IActionResult> GetAll(){
            var pets = await _context.Pets.ToListAsync();
            var petsDto = pets.Select(pets => pets.ToDto());
            return Ok(petsDto);
        }


        [HttpGet("{id}")] // Obtener por id la mascota
        public async Task<IActionResult> getById([FromRoute] int id) {
            var pet = await _context.Pets.FirstOrDefaultAsync(u => u.Id == id);

            if (pet == null) {
                return NotFound();
            }

            return Ok(pet.ToDto());
        }



        [HttpPost] //crear una nueva mascota
         public async Task<IActionResult> Create([FromBody] CreatePetsRequestDto PetDto){
            var petModel = PetDto.ToPetFromCreateDto();
            await _context.Pets.AddAsync(petModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(getById), new { id = petModel.Id}, petModel.ToDto());
        }

        [HttpPut]
        [Route("{id}")] //actualizar una mascota
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePetRequestDto PetDto){
            var petModel = await _context.Pets.FirstOrDefaultAsync(pet => pet.Id == id);
            if (petModel == null){
                return NotFound();
            }
            petModel.Name = PetDto.Name;
            petModel.Animal = PetDto.Animal;
           

             await _context.SaveChangesAsync(); //guardar la info 

            return Ok(petModel.ToDto());
        }


        [HttpDelete] //eliminar mascota
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id){
            var petModel = await _context.Pets.FirstOrDefaultAsync(pet => pet.Id == id);
            if (petModel == null){
                return NotFound();
            }
            _context.Pets.Remove(petModel);

            await _context.SaveChangesAsync();

            return NoContent(); 
        }

    }
}