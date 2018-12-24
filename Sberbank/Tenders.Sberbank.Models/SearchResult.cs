using System.Xml.Serialization;
using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Sberbank.Models
{
    public class SearchResult : ISearchResult
    {
        public ISearchResultEntry[] Entries { get; set; }
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "data", Namespace = "", IsNullable = false)]
    public class data
    {
        [XmlElement("row")]
        public SearchResultEntry[] Entries { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class SearchResultEntry : ISearchResultEntry
    {
        public string reqID { get; set; }
        public string reqState { get; set; }
        public string reqDocID { get; set; }
        public string reqStateStr { get; set; }
        public string purchID { get; set; }
        public string purchCode { get; set; }
        public string purchType { get; set; }
        public string purchName { get; set; }
        public string purchStateStr { get; set; }
        public string purchState { get; set; }
        public string PublicDate { get; set; }
        public string RequestDate { get; set; }
        public string AuctionBeginDate { get; set; }
        public string ToAS { get; set; }
        public string ASID { get; set; }
        public string RN { get; set; }
        public string mybgguarantee { get; set; }
    }
}
