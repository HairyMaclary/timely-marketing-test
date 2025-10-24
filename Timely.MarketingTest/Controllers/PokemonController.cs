using Microsoft.AspNetCore.Mvc;
using Timely.MarketingTest.Services;

namespace Timely.MarketingTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly PokemonService _pokemonService;

        public PokemonController(PokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPokemon(int count = 5)
        {
            var pokemon = await _pokemonService.GetPokemonAsync(count);
            return Ok(pokemon);
        }
    }
}