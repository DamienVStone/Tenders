using System;
using System.Xml.Serialization;
using Tenders.Sberbank.Abstractions.Models;

#pragma warning disable IDE1006 // Стили именования
namespace Tenders.Sberbank.Models
{
    /// <summary>
    /// ВНИМАНИЕ! При дальнейших изменениях в моделях, сгенерированный код оставить в грязном виде!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncRefreshResponse<T> : IAsyncRefreshResponse<T>
        where T : IAsyncRefreshResponseData
    {
        public T d { get; set; }

        public IAsyncRefreshResponse<IAsyncRefreshResponseData> ToAbstraction()
        {
            return new AsyncRefreshResponse<IAsyncRefreshResponseData>
            {
                d = d
            };
        }
    }

    public class D : IAsyncRefreshResponseData
    {
        public string __type { get; set; }
        public string xmlData { get; set; }
        public string xmlError { get; set; }
        public string xmlResult { get; set; }
        public string curentTime { get; set; }
        public string shortTime { get; set; }
        public bool HideTradePlace { get; set; }
    }


    public class TradePlace : ITradePlace
    {
        public string ReqID {get; set;}
        public string ReqNo {get; set;}
        public string PriceSign {get; set;}
        public string PurchId {get; set;}
        public string PurchCode {get; set;}
        public string PurchName {get; set;}
        public string SuppName {get; set;}
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class purchaseoffer
    {
        public purchaseofferMyNewPriceContainer MyNewPriceContainer { get; set; }
        public string reqno { get; set; }
        public string suppname { get; set; }
        public string strauctionbegindate { get; set; }
        public DateTime auctionbegindate { get; set; }
        public string strauctionenddate { get; set; }
        public string strjavascriptauctionenddate { get; set; }
        public DateTime auctionenddate { get; set; }
        public string strpurchstate { get; set; }
        public string purchstate { get; set; }
        public decimal bestpricesign { get; set; }
        public decimal mypricesign { get; set; }
        public string purchname { get; set; }
        public string purchcode { get; set; }
        public string purchCurrencyName { get; set; }
        public decimal bestprice { get; set; }
        public decimal newprice { get; set; }
        public decimal myprice { get; set; }
        public string requestkind { get; set; }
        public decimal purchamount { get; set; }
        public bool isIncrease { get; set; }
        public decimal purchpricestepmin { get; set; }
        public decimal purchpricestepmax { get; set; }
        public string purchid { get; set; }
        public string isTradePanelVisible { get; set; }
        [XmlArrayItem("purchoffer", IsNullable = false)]
        public purchaseofferPurchoffer[] offers { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class purchaseofferMyNewPriceContainer : IPurchaseOfferMyNewPriceContainer
    {
        public string reqid { get; set; }
        public string pricesign { get; set; }
        public string purchaseID { get; set; }
        public string purchaseCode { get; set; }
        public string purchaseName { get; set; }
        public string requestNo { get; set; }
        public string supplierName { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class purchaseofferPurchoffer : IPurchaseofferPurchoffer
    {
        public byte offerPlace { get; set; }
        public object MyOfferMark { get; set; }
        public decimal offerPrice { get; set; }
        public string offerKind { get; set; }
        public byte reqno { get; set; }
        public string offerTime { get; set; }
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class Errors
    {
        public ErrorsError Error { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class ErrorsError
    {
        public string ErrorMessage { get; set; }
    }
}
#pragma warning restore IDE1006 // Стили именования