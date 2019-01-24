using System.Collections.Generic;
using System.Xml.Serialization;
using Tenders.Sberbank.Abstractions.Models;
using Tenders.Sberbank.Abstractions.Models.Requesting;

namespace Tenders.Sberbank.Models
{
    [XmlRoot(ElementName = "mainSearchBar")]
    public class MainSearchBar
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "minimum_should_match")]
        public string Minimum_should_match { get; set; }
    }

    [XmlRoot(ElementName = "purchAmount")]
    public class PurchAmount
    {
        [XmlElement(ElementName = "minvalue")]
        public string Minvalue { get; set; }
        [XmlElement(ElementName = "maxvalue")]
        public string Maxvalue { get; set; }
    }

    [XmlRoot(ElementName = "PublicDate")]
    public class PublicDate
    {
        [XmlElement(ElementName = "minvalue")]
        public string Minvalue { get; set; }
        [XmlElement(ElementName = "maxvalue")]
        public string Maxvalue { get; set; }
    }

    [XmlRoot(ElementName = "PurchaseStageTerm")]
    public class PurchaseStageTerm
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "visiblepart")]
        public string Visiblepart { get; set; }
    }

    [XmlRoot(ElementName = "SourceTerm")]
    public class SourceTerm
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "visiblepart")]
        public string Visiblepart { get; set; }
    }

    [XmlRoot(ElementName = "RegionNameTerm")]
    public class RegionNameTerm
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "visiblepart")]
        public string Visiblepart { get; set; }
    }

    [XmlRoot(ElementName = "RequestStartDate")]
    public class RequestStartDate
    {
        [XmlElement(ElementName = "minvalue")]
        public string Minvalue { get; set; }
        [XmlElement(ElementName = "maxvalue")]
        public string Maxvalue { get; set; }
    }

    [XmlRoot(ElementName = "RequestDate")]
    public class RequestDate
    {
        [XmlElement(ElementName = "minvalue")]
        public string Minvalue { get; set; }
        [XmlElement(ElementName = "maxvalue")]
        public string Maxvalue { get; set; }
    }

    [XmlRoot(ElementName = "AuctionBeginDate")]
    public class AuctionBeginDate
    {
        [XmlElement(ElementName = "minvalue")]
        public string Minvalue { get; set; }
        [XmlElement(ElementName = "maxvalue")]
        public string Maxvalue { get; set; }
    }

    [XmlRoot(ElementName = "okdp2MultiMatch")]
    public class Okdp2MultiMatch
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "okdp2tree")]
    public class Okdp2tree
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "productField")]
        public string ProductField { get; set; }
        [XmlElement(ElementName = "branchField")]
        public string BranchField { get; set; }
    }

    [XmlRoot(ElementName = "classifier")]
    public class Classifier
    {
        [XmlElement(ElementName = "visiblepart")]
        public string Visiblepart { get; set; }
    }

    [XmlRoot(ElementName = "orgCondition")]
    public class OrgCondition
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "orgDictionary")]
    public class OrgDictionary
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "organizator")]
    public class Organizator
    {
        [XmlElement(ElementName = "visiblepart")]
        public string Visiblepart { get; set; }
    }

    [XmlRoot(ElementName = "CustomerCondition")]
    public class CustomerCondition
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "CustomerDictionary")]
    public class CustomerDictionary
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "customer")]
    public class Customer
    {
        [XmlElement(ElementName = "visiblepart")]
        public string Visiblepart { get; set; }
    }

    [XmlRoot(ElementName = "PurchaseWayTerm")]
    public class PurchaseWayTerm
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "visiblepart")]
        public string Visiblepart { get; set; }
    }

    [XmlRoot(ElementName = "PurchaseTypeNameTerm")]
    public class PurchaseTypeNameTerm
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "visiblepart")]
        public string Visiblepart { get; set; }
    }

    [XmlRoot(ElementName = "BranchNameTerm")]
    public class BranchNameTerm
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "visiblepart")]
        public string Visiblepart { get; set; }
    }

    [XmlRoot(ElementName = "IsSMPTerm")]
    public class IsSMPTerm
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "visiblepart")]
        public string Visiblepart { get; set; }
    }

    [XmlRoot(ElementName = "statistic")]
    public class Statistic
    {
        [XmlElement(ElementName = "totalProc")]
        public string TotalProc { get; set; }
        [XmlElement(ElementName = "TotalSum")]
        public string TotalSum { get; set; }
        [XmlElement(ElementName = "DistinctOrgs")]
        public string DistinctOrgs { get; set; }
    }

    [XmlRoot(ElementName = "filters")]
    public class Filters
    {
        [XmlElement(ElementName = "mainSearchBar")]
        public MainSearchBar MainSearchBar { get; set; }
        [XmlElement(ElementName = "purchAmount")]
        public PurchAmount PurchAmount { get; set; }
        [XmlElement(ElementName = "PublicDate")]
        public PublicDate PublicDate { get; set; }
        [XmlElement(ElementName = "PurchaseStageTerm")]
        public PurchaseStageTerm PurchaseStageTerm { get; set; }
        [XmlElement(ElementName = "SourceTerm")]
        public SourceTerm SourceTerm { get; set; }
        [XmlElement(ElementName = "RegionNameTerm")]
        public RegionNameTerm RegionNameTerm { get; set; }
        [XmlElement(ElementName = "RequestStartDate")]
        public RequestStartDate RequestStartDate { get; set; }
        [XmlElement(ElementName = "RequestDate")]
        public RequestDate RequestDate { get; set; }
        [XmlElement(ElementName = "AuctionBeginDate")]
        public AuctionBeginDate AuctionBeginDate { get; set; }
        [XmlElement(ElementName = "okdp2MultiMatch")]
        public Okdp2MultiMatch Okdp2MultiMatch { get; set; }
        [XmlElement(ElementName = "okdp2tree")]
        public Okdp2tree Okdp2tree { get; set; }
        [XmlElement(ElementName = "classifier")]
        public Classifier Classifier { get; set; }
        [XmlElement(ElementName = "orgCondition")]
        public OrgCondition OrgCondition { get; set; }
        [XmlElement(ElementName = "orgDictionary")]
        public OrgDictionary OrgDictionary { get; set; }
        [XmlElement(ElementName = "organizator")]
        public Organizator Organizator { get; set; }
        [XmlElement(ElementName = "CustomerCondition")]
        public CustomerCondition CustomerCondition { get; set; }
        [XmlElement(ElementName = "CustomerDictionary")]
        public CustomerDictionary CustomerDictionary { get; set; }
        [XmlElement(ElementName = "customer")]
        public Customer Customer { get; set; }
        [XmlElement(ElementName = "PurchaseWayTerm")]
        public PurchaseWayTerm PurchaseWayTerm { get; set; }
        [XmlElement(ElementName = "PurchaseTypeNameTerm")]
        public PurchaseTypeNameTerm PurchaseTypeNameTerm { get; set; }
        [XmlElement(ElementName = "BranchNameTerm")]
        public BranchNameTerm BranchNameTerm { get; set; }
        [XmlElement(ElementName = "IsSMPTerm")]
        public IsSMPTerm IsSMPTerm { get; set; }
        [XmlElement(ElementName = "statistic")]
        public Statistic Statistic { get; set; }
    }

    [XmlRoot(ElementName = "fields")]
    public class Fields
    {
        [XmlElement(ElementName = "field")]
        public List<string> Field { get; set; }
    }

    [XmlRoot(ElementName = "sort")]
    public class Sort
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "direction")]
        public string Direction { get; set; }
    }

    [XmlRoot(ElementName = "empty")]
    public class Empty
    {
        [XmlElement(ElementName = "filterType")]
        public string FilterType { get; set; }
        [XmlElement(ElementName = "field")]
        public string Field { get; set; }
    }

    [XmlRoot(ElementName = "aggregations")]
    public class Aggregations
    {
        [XmlElement(ElementName = "empty")]
        public Empty Empty { get; set; }
    }

    [XmlRoot(ElementName = "elasticrequest")]
    public class ElasticRequest : IElasticRequest
    {
        [XmlElement(ElementName = "filters")]
        public Filters Filters { get; set; }
        [XmlElement(ElementName = "fields")]
        public Fields Fields { get; set; }
        [XmlElement(ElementName = "sort")]
        public Sort Sort { get; set; }
        [XmlElement(ElementName = "aggregations")]
        public Aggregations Aggregations { get; set; }
        [XmlElement(ElementName = "size")]
        public string Size { get; set; }
        [XmlElement(ElementName = "from")]
        public string From { get; set; }

        public void Init(ISearchParameters request)
        {
            Filters = new Filters
            {
                MainSearchBar = new MainSearchBar
                {
                    Value = request.Text,
                    Type = "best_fields",
                    Minimum_should_match = "1%"
                },
                PublicDate = new PublicDate
                {
                    Minvalue = request.PublicDateFrom.ToString("dd.MM.yyyy HH:mm"),
                    Maxvalue = request.PublicDateTo.ToString("dd.MM.yyyy HH:mm")
                },
                PurchaseStageTerm = new PurchaseStageTerm
                {
                    Value = "Подача заявок",
                    Visiblepart = "Подача заявок"
                },
                SourceTerm = new SourceTerm
                {
                    Value = "Госзакупки по 44-ФЗ",
                    Visiblepart = "Госзакупки по 44-ФЗ"
                }
            };

            Fields = new Fields
            {
                Field = new List<string>
                {
                    "TradeSectionId",
                    "purchAmount",
                    "purchCurrency",
                    "purchCodeTerm",
                    "PurchaseTypeName",
                    "purchStateName",
                    "BidStatusName",
                    "OrgName",
                    "SourceTerm",
                    "PublicDate",
                    "RequestDate",
                    "RequestStartDate",
                    "RequestAcceptDate",
                    "EndDate",
                    "CreateRequestHrefTerm",
                    "CreateRequestAlowed",
                    "purchName",
                    "BidName",
                    "SourceHrefTerm",
                    "objectHrefTerm",
                    "needPayment",
                    "IsSMP",
                    "isIncrease"
                }
            };

            Sort = new Sort
            {
                Value = "default"
            };

            Aggregations = new Aggregations
            {
                Empty = new Empty
                {
                    FilterType = "filter_aggregation"
                }
            };

            Size = "100";
            From = "0";
        }
    }
}
