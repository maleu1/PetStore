using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetStore.Models;

namespace PetStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetStoreController : ControllerBase
    {
        private List<Pet> AllPets = new List<Pet> {
            new Pet { Id = 1, Name = "Bird" }, 
            new Pet { Id = 2, Name = "Dog" }, 
            new Pet { Id = 3, Name = "Cat" }, 
            new Pet { Id = 4, Name = "Fish" }, 
            new Pet { Id = 5, Name = "Spider" }, 
        };

        private readonly ILogger<PetStoreController> _logger;

        public PetStoreController(ILogger<PetStoreController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Pet>> GetAllAsync()
        {
            return AllPets;
        }
        
        [HttpGet("{id}")]
        public async Task<Pet> GetPetAsync([FromRoute] int id)
        {
            return AllPets.FirstOrDefault(pet => pet.Id == id);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateNewPetAsync([FromBody] Pet pet)
        {
            AllPets.Add(pet);

            return Ok();
        }
        
        [HttpPut("{id}")]
        public async Task<Pet> UpdatePetAsync([FromRoute] int id, [FromBody] string name)
        {
            Pet petToUpdate = AllPets.FirstOrDefault(pet => pet.Id == id);

            if (petToUpdate != null)
            {
                petToUpdate.Name = name;
            }

            return petToUpdate;
        }
    }
}