namespace PetStore.Models
{
    /// <summary>
    /// A class describing a Pet.
    /// </summary>
    public class Pet
    {
        /// <summary>
        /// Gets or sets the unique identifier of the pet.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the pet.
        /// </summary>
        public string Name { get; set; }
    }
}