using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Untappd.KolosBot.Infrastucture.Models;

namespace Untappd.KolosBot.Infrastucture
{
    public class UntappdParser
    {
        private readonly IBrowsingContext _browsingContext;
        private readonly string _address;

        public UntappdParser()
        {
            var config = Configuration.Default.WithDefaultLoader();
            _address = "https://untappd.com/v/zolotij-kolos/7078045";
            _browsingContext = BrowsingContext.New(config);
        }

        public async Task<List<Beer>> GetBeers()
        {
           
            var document = await _browsingContext.OpenAsync(_address);

            var cellSelector = "ul.menu-section-list";
            var element = document.QuerySelector(cellSelector);
            var beersResponse = new List<Beer>();
            beersResponse.AddRange(GrabBeerFromHtml(element));

            beersResponse.AddRange(await GrabLastBeersFromShowMore(document.ToHtml()));

            return beersResponse;
        }

        private async Task<List<Beer>> GrabLastBeersFromShowMore(string webPageHtml)
        {
            HtmlDocument lastBeersView = new HtmlDocument();
            lastBeersView.LoadHtml(webPageHtml);
            var sectionId = lastBeersView.DocumentNode.SelectSingleNode("//a[@class='yellow button more show-more-section track-click']").Attributes["data-section-id"].Value;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
            var response = await httpClient.GetStringAsync($"https://untappd.com/venue/more_menu/7078045/15?section_id={sectionId}");
            var lastBeers = JsonConvert.DeserializeObject<GetLastBeersResponse>(response);

            var beersDoc = await _browsingContext.OpenAsync(req => req.Content(lastBeers.view));
            return GrabBeerFromHtml(beersDoc.DocumentElement);
        }

        private List<Beer> GrabBeerFromHtml(IElement element)
        {
            var beersResponse = new List<Beer>();
            var beers = element.QuerySelectorAll("li");
            foreach (var beer in beers)
            {
                var beerResponse = new Beer();
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(beer.InnerHtml);
                var image = pageDocument.DocumentNode.SelectSingleNode("//div[@class='beer-label']//img")
                    .Attributes["src"].Value;

                var beerDetails = pageDocument.DocumentNode.SelectSingleNode("//div[@class='beer-details']");

                var beerName = beerDetails.SelectSingleNode("//a").InnerText;
                var beerType = beerDetails.SelectSingleNode("//em").InnerText;
                var abvIbuAndBrewery = beerDetails.SelectSingleNode("//h6").InnerText.Replace("\n", string.Empty);
                var rating = beerDetails.SelectSingleNode("//div[@class='caps small']").Attributes["data-rating"]
                    .Value;

                var beerPrices = pageDocument.DocumentNode.SelectSingleNode("//div[@class='beer-prices']");

                var priceNodes = beerPrices.SelectNodes("//p");

                var prices = new StringBuilder();
                foreach (var priceNode in priceNodes)
                {
                    var priceAndSize = priceNode.Elements("span").ToArray();
                    var size = priceAndSize[0].InnerText;
                    var actualPrice = priceAndSize[1].InnerText;
                    prices.AppendLine($"{size} -- {actualPrice}");
                }

                beerResponse.ImageUrl = image;
                beerResponse.Name = beerName;
                beerResponse.Type = beerType;
                beerResponse.AbvAndIbu = abvIbuAndBrewery;
                beerResponse.Rating = rating;
                beerResponse.Prices = prices.ToString();
                beersResponse.Add(beerResponse);
            }

            return beersResponse;
        }
    }

    public class Beer
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string AbvAndIbu { get; set; }
        public string Rating { get; set; }
        public string Prices { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"*{Name}*");
            stringBuilder.AppendLine($"{Type}");
            stringBuilder.AppendLine($"{AbvAndIbu}");
            stringBuilder.AppendLine($"Rating:{Rating}");
            stringBuilder.AppendLine($"{Prices}");

            return stringBuilder.ToString();
        }
    }
}
