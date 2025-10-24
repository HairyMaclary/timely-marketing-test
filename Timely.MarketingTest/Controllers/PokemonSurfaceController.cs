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
    /// <summary>
    /// Surface Controller for handling Pokemon-related operations and content counting.
    /// This controller demonstrates Umbraco's native patterns using SurfaceController,
    /// ContentService, and Block List management for the technical assessment.
    /// 
    /// Key features:
    /// - Loads Pokemon from external API and stores as Block List content in database
    /// - Clears Pokemon data from Block List and resets counters
    /// - Counts Pokemon items in Block List and stores result in contentCount property
    /// - Uses proper Umbraco form submission patterns with automatic page refresh
    /// </summary>
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
        public IActionResult ClearPokemon()
        {
            try
            {
                var currentPage = CurrentPage;
                if (currentPage == null)
                {
                    TempData["Error"] = "Could not find current page";
                    return CurrentUmbracoPage();
                }

                var homepageContent = _contentService.GetById(currentPage.Id);
                if (homepageContent != null)
                {
                    // Clear the Pokemon Block List by setting it to empty
                    var emptyBlockListJson = JsonConvert.SerializeObject(new
                    {
                        layout = new Dictionary<string, object>
                        {
                            ["Umbraco.BlockList"] = new object[0]
                        },
                        contentData = new object[0],
                        settingsData = new object[0]
                    });
                    
                    homepageContent.SetValue("pokemonList", emptyBlockListJson);
                    
                    homepageContent.SetValue("contentCount", 0);
                    
                    _contentService.Save(homepageContent);
                    _contentService.Publish(homepageContent, new string[0]);
                }

                TempData["Success"] = "Pokemon cleared successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error clearing Pokemon: {ex.Message}";
            }

            return CurrentUmbracoPage();
        }

        [HttpPost]
        public IActionResult GetContentCount()
        {
            try
            {
                var currentPage = CurrentPage;
                if (currentPage == null)
                {
                    TempData["Error"] = "Could not find current page";
                    return CurrentUmbracoPage();
                }

                // Count Pokemon in the Block List
                var homepageContent = _contentService.GetById(currentPage.Id);
                int pokemonCount = 0;
                
                if (homepageContent != null)
                {
                    // Get the Pokemon Block List and count the items
                    var pokemonBlockListJson = homepageContent.GetValue<string>("pokemonList");
                    
                    if (!string.IsNullOrEmpty(pokemonBlockListJson))
                    {
                        try
                        {
                            var blockListData = JsonConvert.DeserializeObject<dynamic>(pokemonBlockListJson);
                            if (blockListData?.contentData != null)
                            {
                                pokemonCount = ((Newtonsoft.Json.Linq.JArray)blockListData.contentData).Count;
                            }
                        }
                        catch
                        {
                            // If JSON parsing fails, pokemonCount stays 0
                        }
                    }

                    homepageContent.SetValue("contentCount", pokemonCount);
                    _contentService.Save(homepageContent);
                    _contentService.Publish(homepageContent, new string[0]);
                }

                TempData["ContentCount"] = pokemonCount;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error getting Pokemon count: {ex.Message}";
            }

            return CurrentUmbracoPage();
        }
    }
}