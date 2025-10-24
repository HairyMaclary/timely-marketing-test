using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Website.Controllers;
using Timely.MarketingTest.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace Timely.MarketingTest.Controllers
{
    public class PokemonSurfaceController : SurfaceController
    {
        private readonly PokemonService _pokemonService;
        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;

        public PokemonSurfaceController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            PokemonService pokemonService,
            IContentService contentService,
            IContentTypeService contentTypeService)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _pokemonService = pokemonService;
            _contentService = contentService;
            _contentTypeService = contentTypeService;
        }

        [HttpPost]
        public async Task<IActionResult> LoadPokemon()
        {
            try
            {
                var pokemonList = await _pokemonService.GetPokemonAsync(5);
                
                var currentPage = CurrentPage;
                if (currentPage == null)
                {
                    TempData["Error"] = "Could not find current page";
                    return CurrentUmbracoPage();
                }

                var pokemonContentType = _contentTypeService.Get("pokemon");
                if (pokemonContentType == null)
                {
                    TempData["Error"] = "Pokemon element type not found";
                    return CurrentUmbracoPage();
                }

                var contentData = new List<object>();
                var layoutData = new List<object>();
                string blockListJson = "";
                
                foreach (var pokemon in pokemonList)
                {
                    var elementKey = Guid.NewGuid();
                    
                    contentData.Add(new
                    {
                        key = elementKey,
                        contentTypeKey = pokemonContentType.Key,
                        pokemonName = pokemon.Name,
                        pokemonHeight = pokemon.Height,
                        pokemonWeight = pokemon.Weight,
                        pokemonSpecies = pokemon.Species
                    });
                    
                    layoutData.Add(new
                    {
                        contentUdi = $"umb://element/{elementKey:N}"
                    });
                }

                var homepageContent = _contentService.GetById(currentPage.Id);
                if (homepageContent != null)
                {
                    blockListJson = JsonConvert.SerializeObject(new
                    {
                        layout = new Dictionary<string, object>
                        {
                            ["Umbraco.BlockList"] = layoutData.ToArray()
                        },
                        contentData = contentData.ToArray(),
                        settingsData = new object[0]
                    });
                    
                    homepageContent.SetValue("pokemonList", blockListJson);
                    
                    _contentService.Save(homepageContent);
                    _contentService.Publish(homepageContent, new string[0]);
                }

                TempData["Success"] = $"Loaded {pokemonList.Count} Pokemon successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading Pokemon: {ex.Message}";
            }

            return CurrentUmbracoPage();
        }

        [HttpPost]
        public IActionResult GetContentCount()
        {
            try
            {
                var allContent = _contentService.GetPagedDescendants(-1, 0, int.MaxValue, out var total);
                var publishedCount = allContent.Count(c => c.Published);

                TempData["ContentCount"] = publishedCount;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error getting content count: {ex.Message}";
            }

            return CurrentUmbracoPage();
        }
    }
}