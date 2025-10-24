using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Timely.MarketingTest.Services;

namespace Timely.MarketingTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly PokemonService _pokemonService;
        private readonly PokemonBlockListService _blockListService;
        private readonly IContentService _contentService;

        public HomeController(
            PokemonService pokemonService, 
            IContentService contentService)
        {
            _pokemonService = pokemonService;
            _blockListService = new PokemonBlockListService();
            _contentService = contentService;
        }

        [HttpGet("pokemon")]
        public async Task<IActionResult> GetPokemon(int count = 5)
        {
            var pokemonList = await _pokemonService.GetPokemonAsync(count);
            
            var viewModels = _blockListService.CreatePokemonViewModels(pokemonList);
            
            return Ok(viewModels);
        }

        [HttpPost("content-count")]
        public IActionResult GetContentCount()
        {
            var publishedContent = _contentService.GetPagedDescendants(-1, 0, int.MaxValue, out var total);
            var publishedCount = publishedContent.Count(c => c.Published);

            return Ok(new { count = publishedCount });
        }
    }
}