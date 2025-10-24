using Timely.MarketingTest.Models;

namespace Timely.MarketingTest.Services
{
    public class PokemonBlockListService
    {
        public List<PokemonViewModel> CreatePokemonViewModels(List<Pokemon> pokemonList)
        {
            // Convert Pokemon API data to simple view models that match the element type aliases
            return pokemonList.Select(pokemon => new PokemonViewModel
            {
                PokemonName = pokemon.Name,
                PokemonHeight = pokemon.Height,
                PokemonWeight = pokemon.Weight,
                PokemonSpecies = pokemon.Species
            }).ToList();
        }
    }

    public class PokemonViewModel
    {
        public string PokemonName { get; set; } = string.Empty;
        public decimal PokemonHeight { get; set; }
        public decimal PokemonWeight { get; set; }
        public string PokemonSpecies { get; set; } = string.Empty;
    }
}