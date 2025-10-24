using System.Text.Json;
using Timely.MarketingTest.Models;

namespace Timely.MarketingTest.Services
{
    public class PokemonService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public PokemonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Pokemon>> GetPokemonAsync(int count = 5)
        {
            try
            {
                // Get list of Pokemon
                var listResponse = await _httpClient.GetStringAsync($"https://pokeapi.co/api/v2/pokemon?limit={count}");
                var pokemonList = JsonSerializer.Deserialize<PokemonListResponse>(listResponse, _jsonOptions);

                var pokemonDetails = new List<Pokemon>();

                // Get details for each Pokemon
                foreach (var item in pokemonList?.Results ?? new List<PokemonListItem>())
                {
                    var detailResponse = await _httpClient.GetStringAsync(item.Url);
                    var detail = JsonSerializer.Deserialize<PokemonDetailResponse>(detailResponse, _jsonOptions);

                    if (detail != null)
                    {
                        pokemonDetails.Add(new Pokemon
                        {
                            Name = detail.Name,
                            Height = detail.Height / 10m, // Convert to meters
                            Weight = detail.Weight / 10m, // Convert to kg
                            Species = detail.Species.Name
                        });
                    }
                }

                return pokemonDetails;
            }
            catch (Exception ex)
            {
                // Simple error handling - in production you'd want proper logging
                Console.WriteLine($"Error fetching Pokemon: {ex.Message}");
                return new List<Pokemon>();
            }
        }
    }
}