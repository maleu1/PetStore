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
        // this should be replaced by a persistence layer with e.g. a database connection. 
        private List<Pet> AllPets = new List<Pet> {
            new Pet { Id = 1, Name = "Bird" }, 
            new Pet { Id = 2, Name = "Dog" }, 
            new Pet { Id = 3, Name = "Cat" }, 
            new Pet { Id = 4, Name = "Fish" }, 
            new Pet { Id = 5, Name = "Spider" }, 
        };

        private readonly ILogger<PetStoreController> _logger;

        /// <summary>
        /// Constructs a new instance of <see cref="PetStoreController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public PetStoreController(ILogger<PetStoreController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of all available pets. 
        /// </summary>
        /// <returns>A list of pets.</returns>
        [HttpGet]
        public async Task<IEnumerable<Pet>> GetAllAsync()
        {
            return AllPets;
        }
        
        /// <summary>
        /// Returns a specific pet by a unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the pet.</param>
        /// <returns>A pet.</returns>
        [HttpGet("{id}")]
        public async Task<Pet> GetPetAsync([FromRoute] int id)
        {
            return AllPets.FirstOrDefault(pet => pet.Id == id);
        }
        
        /// <summary>
        /// Creates a new pet.
        /// </summary>
        /// <param name="pet">The pet to be created.</param>
        /// <returns>OK</returns>
        [HttpPost]
        public async Task<IActionResult> CreateNewPetAsync([FromBody] Pet pet)
        {
            AllPets.Add(pet);

            return Ok();
        }
        
        /// <summary>
        /// Updates a specific pet instance.
        /// </summary>
        /// <param name="id">Identifies the pet to be updated.</param>
        /// <param name="name">The new name of the pet.</param>
        /// <returns>The updated pet.</returns>
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