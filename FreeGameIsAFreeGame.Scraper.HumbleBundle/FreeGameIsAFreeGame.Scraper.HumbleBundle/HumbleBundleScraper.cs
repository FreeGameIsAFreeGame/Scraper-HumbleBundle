using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using FreeGameIsAFreeGame.Core;
using FreeGameIsAFreeGame.Core.Models;
using NLog;

namespace FreeGameIsAFreeGame.Scraper.HumbleBundle
{
    public class HumbleBundleScraper : IScraper
    {
        private const string URL =
            "https://www.humblebundle.com/store/api/search?sort=discount&filter=onsale&hmb_source=store_navbar&request=__REQUEST__&page=__PAGE__";

        private readonly IBrowsingContext context;
        private readonly ILogger logger;

        private int requestCount;

        string IScraper.Identifier => "HumbleBundleFree";
        string IScraper.DisplayName => "Humble Bundle";

        public HumbleBundleScraper()
        {
            context = BrowsingContext.New(Configuration.Default
                .WithDefaultLoader()
                .WithDefaultCookies());

            logger = LogManager.GetLogger(GetType().FullName);
        }

        async Task<IEnumerable<IDeal>> IScraper.Scrape(CancellationToken token)
        {
            List<IDeal> deals = new List<IDeal>();
            int pageCount = await GetPageCount(token);
            if (token.IsCancellationRequested)
                return null;

            for (int i = 0; i < pageCount; i++)
            {
                await Task.Delay(1500, token);
                if (token.IsCancellationRequested)
                    return null;

                string content = await GetPageContent(i, token);
                if (token.IsCancellationRequested)
                    return null;

                HumbleBundleData data = HumbleBundleData.FromJson(content);

                foreach (Result result in data.Results)
                {
                    int discount = GetDiscount(result);
                    if (discount != 100)
                        continue;

                    Deal deal = new Deal
                    {
                        Discount = discount,
                        Image = result.FeaturedImageRecommendation,
                        Link = $"https://humblebundle.com/store/{result.HumanUrl}",
                        Title = result.HumanName,
                        Start = null,
                        End = DateTimeOffset.FromUnixTimeSeconds(result.SaleEnd).UtcDateTime
                    };

                    deals.Add(deal);
                }
            }

            return deals;
        }

        private async Task<int> GetPageCount(CancellationToken token)
        {
            string content = await GetPageContent(0, token);
            if (token.IsCancellationRequested)
                return 0;
            HumbleBundleData data = HumbleBundleData.FromJson(content);
            return (int) data.NumPages;
        }

        private async Task<string> GetPageContent(int page, CancellationToken token)
        {
            Url url = Url.Create(GetUrl(page));
            DocumentRequest request = DocumentRequest.Get(url);
            IDocument document = await context.OpenAsync(request, token);
            if (token.IsCancellationRequested)
                return null;
            string content = document.Body.Text();
            return content;
        }

        private string GetUrl(int page)
        {
            return URL
                .Replace("__REQUEST__", GetRequestCount().ToString())
                .Replace("__PAGE__", page.ToString());
        }

        private int GetRequestCount()
        {
            return ++requestCount;
        }

        private int GetDiscount(Result result)
        {
            Price current = result.CurrentPrice;
            if (current == null)
            {
                return 0;
            }

            if (current.Amount == 0)
            {
                return 100;
            }

            Price full = result.FullPrice;

            return 100 - (int) Math.Round(current.Amount / full.Amount * 100);
        }
    }
}
