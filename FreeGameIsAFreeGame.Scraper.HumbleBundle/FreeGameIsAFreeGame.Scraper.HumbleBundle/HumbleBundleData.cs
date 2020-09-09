using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FreeGameIsAFreeGame.Scraper.HumbleBundle
{
    using J = JsonPropertyAttribute;
    using N = NullValueHandling;

    public partial class HumbleBundleData
    {
        [J("num_results")] public long NumResults { get; set; }
        [J("page_index")] public long PageIndex { get; set; }
        [J("request")] public long Request { get; set; }
        [J("num_pages")] public long NumPages { get; set; }
        [J("results")] public List<Result> Results { get; set; }
    }

    public partial class Result
    {
        [J("standard_carousel_image")] public Uri StandardCarouselImage { get; set; }
        [J("delivery_methods")] public List<string> DeliveryMethods { get; set; }
        [J("machine_name")] public string MachineName { get; set; }
        [J("xray_traits_thumbnail")] public Uri XrayTraitsThumbnail { get; set; }
        [J("content_types")] public List<string> ContentTypes { get; set; }
        [J("human_url")] public string HumanUrl { get; set; }
        [J("platforms")] public List<string> Platforms { get; set; }
        [J("icon_dict")] public IconDict IconDict { get; set; }
        [J("featured_image_recommendation")] public string FeaturedImageRecommendation { get; set; }
        [J("large_capsule")] public Uri LargeCapsule { get; set; }
        [J("human_name")] public string HumanName { get; set; }
        [J("type")] public string Type { get; set; }
        [J("icon")] public Uri Icon { get; set; }
        [J("current_price")] public Price CurrentPrice { get; set; }
        [J("sale_end")] public long SaleEnd { get; set; }
        [J("non_rewards_charity_split")] public long NonRewardsCharitySplit { get; set; }
        [J("sale_type")] public string SaleType { get; set; }
        [J("rewards_split")] public double RewardsSplit { get; set; }
        [J("empty_tpkds")] public EmptyTpkds EmptyTpkds { get; set; }
        [J("cta_badge")] public object CtaBadge { get; set; }
        [J("rating_for_current_region")] public string RatingForCurrentRegion { get; set; }
        [J("full_price")] public Price FullPrice { get; set; }

        [J("auth_required_for_purchase_if_free", NullValueHandling = N.Ignore)]
        public bool? AuthRequiredForPurchaseIfFree { get; set; }

        [J("required_account_links", NullValueHandling = N.Ignore)]
        public List<string> RequiredAccountLinks { get; set; }
    }

    public partial class Price
    {
        [J("currency")] public string Currency { get; set; }
        [J("amount")] public double Amount { get; set; }
    }

    public partial class EmptyTpkds
    {
    }

    public partial class IconDict
    {
        [J("download", NullValueHandling = N.Ignore)]
        public Download Download { get; set; }

        [J("steam", NullValueHandling = N.Ignore)]
        public Download Steam { get; set; }
    }

    public partial class Download
    {
        [J("available")] public List<string> Available { get; set; }
        [J("unavailable")] public List<string> Unavailable { get; set; }
    }

    public partial class HumbleBundleData
    {
        public static HumbleBundleData FromJson(string json) =>
            JsonConvert.DeserializeObject<HumbleBundleData>(json,
                Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this HumbleBundleData self) =>
            JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
    }
}
