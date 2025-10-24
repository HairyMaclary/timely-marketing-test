namespace Timely.MarketingTest.Models
{
    public class Pokemon
    {
        public string Name { get; set; } = string.Empty;
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string Species { get; set; } = string.Empty;
    }

    public class PokemonListResponse
    {
        public List<PokemonListItem> Results { get; set; } = new();
    }

    public class PokemonListItem
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class PokemonDetailResponse
    {
        public string Name { get; set; } = string.Empty;
        public int Height { get; set; }
        public int Weight { get; set; }
        public PokemonSpecies Species { get; set; } = new();
    }

    public class PokemonSpecies
    {
        public string Name { get; set; } = string.Empty;
    }
}