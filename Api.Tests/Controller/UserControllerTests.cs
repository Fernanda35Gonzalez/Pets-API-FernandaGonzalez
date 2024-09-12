using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using api.Controllers;
using api.Data;
using api.Dtos.User;
using api.Models;

namespace Api.Tests.Controller
{
     public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly ApplicationDBContext _context;

        public UserControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDBContext(options);

            _controller = new UserController(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.Users.Any())
            {
                _context.Users.AddRange(new List<User>
                {
                    new User { Id = 1, FirstName = "John", LastName = "Doe", Age = 30 },
                    new User { Id = 2, FirstName = "Jane", LastName = "Doe", Age = 25 }
                });
                _context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfUsers()
        {
            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnUsers = okResult.Value as IEnumerable<UserDto>;

            Assert.NotNull(returnUsers); // Ensure it's not null
            var userList = returnUsers.ToList(); // Convert IEnumerable to List for easier assertions

            Assert.Equal(2, userList.Count);
            Assert.Contains(userList, u => u.FirstName == "John" && u.LastName == "Doe");
            Assert.Contains(userList, u => u.FirstName == "Jane" && u.LastName == "Doe");
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithUser()
        {
            // Act
            var result = await _controller.getById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(1, returnUser.Id);
            Assert.Equal("John", returnUser.FirstName);
        }

        [Fact]
        public async Task Create_ReturnsCreatedResult_WithNewUser()
        {
            // Arrange
            var newUser = new CreateUserRequestDto
            {
                FirstName = "Mark",
                LastName = "Smith",
                Age = 28
            };

            // Act
            var result = await _controller.Create(newUser);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnUser = Assert.IsType<UserDto>(createdResult.Value);
            Assert.Equal("Mark", returnUser.FirstName);
            Assert.Equal("Smith", returnUser.LastName);

            // Verify that the user was actually added to the database
            var userInDb = await _context.Users.FindAsync(returnUser.Id);
            Assert.NotNull(userInDb);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WithUpdatedUser()
        {
            // Arrange
            var updatedUserDto = new UpdateUserRequestDto
            {
                FirstName = "John Updated",
                LastName = "Doe Updated",
                Age = 31
            };

            // Act
            var result = await _controller.Update(1, updatedUserDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("John Updated", returnUser.FirstName);
            Assert.Equal("Doe Updated", returnUser.LastName);

            // Verify that the user was actually updated in the database
            var userInDb = await _context.Users.FindAsync(1);
            Assert.NotNull(userInDb);
            Assert.Equal("John Updated", userInDb.FirstName);
            Assert.Equal("Doe Updated", userInDb.LastName);
        }

        [Fact]
        public async Task Delete_ReturnsNoContentResult()
        {
            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify that the user was actually removed from the database
            var userInDb = await _context.Users.FindAsync(1);
            Assert.Null(userInDb);
        }
    }
}