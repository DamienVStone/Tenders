using Newtonsoft.Json;
namespace Tenders.Sberbank.Models.Requesting
{

    /// <summary>
    /// Ответ поиска в гостевой зоне
    /// </summary>
    public class Rootobject
    {
        public string result { get; set; }
        public _data Data { get { return JsonConvert.DeserializeObject<_data>(data); } }
        public string data { get; set; }
    }

    public class _data
    {
        public string tableXml { get; set; }
        public string statisticXml { get; set; }
        public string pagerTotal { get; set; }
        public dataRow Data { get { return JsonConvert.DeserializeObject<dataRow>(data); } }
        public string data { get; set; }
    }

    public class dataRow
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public _Shards _shards { get; set; }
        public Hits hits { get; set; }
        public Aggregations aggregations { get; set; }
    }

    public class _Shards
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int failed { get; set; }
    }

    public class Hits
    {
        public int total { get; set; }
        public object max_score { get; set; }
        public Hit[] hits { get; set; }
    }

    public class Hit
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public object _score { get; set; }
        public Fields fields { get; set; }
        public Highlight highlight { get; set; }
        public object sort { get; set; }
    }

    public class Fields
    {
        public string[] PurchaseTypeName { get; set; }
        public string[] purchStateName { get; set; }
        public int[] CreateRequestAlowed { get; set; }
        public string[] BidName { get; set; }
        public int[] TradeSectionId { get; set; }
        public string[] purchCodeTerm { get; set; }
        public string[] objectHrefTerm { get; set; }
        public string[] OrgName { get; set; }
        public string[] CreateRequestHrefTerm { get; set; }
        public string[] RequestStartDate { get; set; }
        public string[] EndDate { get; set; }
        public string[] RequestAcceptDate { get; set; }
        public string[] RequestDate { get; set; }
        public string[] SourceHrefTerm { get; set; }
        public string[] purchName { get; set; }
        public string[] SourceTerm { get; set; }
        public string[] purchCurrency { get; set; }
        public float[] purchAmount { get; set; }
        public string[] PublicDate { get; set; }
    }

    public class Highlight
    {
        public string[] purchName { get; set; }
        public string[] BidName { get; set; }
    }

    public class Aggregations
    {
        public Distinctorgs DistinctOrgs { get; set; }
        public Totalsum TotalSum { get; set; }
        public Branch Branch { get; set; }
        public Times Times { get; set; }
        public Stage Stage { get; set; }
        public Region Region { get; set; }
        public Sources Sources { get; set; }
    }

    public class Distinctorgs
    {
        public int value { get; set; }
    }

    public class Totalsum
    {
        public float value { get; set; }
    }

    public class Branch
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public Bucket[] buckets { get; set; }
    }

    public class Bucket
    {
        public string key { get; set; }
        public int doc_count { get; set; }
        public Price_Sums price_sums { get; set; }
    }

    public class Price_Sums
    {
        public float value { get; set; }
    }

    public class Times
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public Bucket1[] buckets { get; set; }
    }

    public class Bucket1
    {
        public string key { get; set; }
        public int doc_count { get; set; }
        public Price_Sums1 price_sums { get; set; }
    }

    public class Price_Sums1
    {
        public float value { get; set; }
    }

    public class Stage
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public Bucket2[] buckets { get; set; }
    }

    public class Bucket2
    {
        public string key { get; set; }
        public int doc_count { get; set; }
        public Price_Sums2 price_sums { get; set; }
    }

    public class Price_Sums2
    {
        public float value { get; set; }
    }

    public class Region
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public Bucket3[] buckets { get; set; }
    }

    public class Bucket3
    {
        public string key { get; set; }
        public int doc_count { get; set; }
        public Price_Sums3 price_sums { get; set; }
    }

    public class Price_Sums3
    {
        public float value { get; set; }
    }

    public class Sources
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public Bucket4[] buckets { get; set; }
    }

    public class Bucket4
    {
        public string key { get; set; }
        public int doc_count { get; set; }
        public Price_Sums4 price_sums { get; set; }
    }

    public class Price_Sums4
    {
        public float value { get; set; }
    }
}
