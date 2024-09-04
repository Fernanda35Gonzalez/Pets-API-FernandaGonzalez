using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.User;
using api.Mappers;
using api.Dtos.Pet;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        
        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<IActionResult> GetAll(){
            var users = await _context.Users.Include(user => user.pets).ToListAsync();
            var usersDto = users.Select(users => users.ToDto());
            return Ok(usersDto);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> getById([FromRoute] int id){
            var user =  await _context.Users.Include(user => user.pets).FirstOrDefaultAsync(u => u.Id == id);

            if(user== null){
                return NotFound();
            }
            return Ok(user.ToDto());
        }

        [HttpPost]
         public async Task<IActionResult> Create([FromBody] CreateUserRequestDto userDto){
            var userModel = userDto.ToUserFromCreateDto();
            await _context.Users.AddAsync(userModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(getById), new { id = userModel.Id}, userModel.ToDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequestDto userDto){
            var userModel = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (userModel == null){
                return NotFound();
            }
            userModel.Age = userDto.Age;
            userModel.FirstName = userDto.FirstName;
            userModel.LastName = userDto.LastName;

             await _context.SaveChangesAsync(); //guardar la info 

            return Ok(userModel.ToDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id){
            var userModel = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (userModel == null){
                return NotFound();
            }
            _context.Users.Remove(userModel);

            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        [HttpPost("{userId}/assign-pet/{petId}")] //método en UserController para poder asignar un Pet a un respectivo User
        public async Task<IActionResult> AssignPetToUser([FromRoute] int userId, [FromRoute] int petId)
        {
            // Verificar si el usuario existe
            var user = await _context.Users.Include(u => u.pets).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            // Verificar si la mascota existe
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == petId);
            if (pet == null)
            {
                return NotFound();
            }

            // Asignar la mascota al usuario
            pet.UserId = userId;
            user.pets.Add(pet);

            // Guardar cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = $"La mascota con Id {petId} ha sido asignada al usuario con Id {userId}." });

        }


        [HttpPost("crearUsuariosYPets")]// crear un usuario completamente nuevo con sus respectivos datos e incluir n cantidad de Pets en el mismo request body
        public async Task<IActionResult> CrearUsuariosPets([FromBody] CreateUserRequestDto createUserRequestDto)
        {
          
            var userModel = createUserRequestDto.ToUserFromCreateDto();

          
            foreach (var petDto in createUserRequestDto.Pets)
            {
                var petModel = petDto.ToPetFromCreateDto();
                petModel.UserId = userModel.Id;  
                userModel.pets.Add(petModel);
            }

            await _context.Users.AddAsync(userModel);
            await _context.Pets.AddRangeAsync(userModel.pets); 
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(getById), new { id = userModel.Id }, userModel.ToDto());
        }

        

    }
}