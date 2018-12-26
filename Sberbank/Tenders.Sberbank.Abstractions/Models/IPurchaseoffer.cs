using System;
using Tenders.Core.Abstractions.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
#pragma warning disable IDE1006 // Стили именования

    public interface IPurchaseoffer<TMyPrice, TOffer>
        where TMyPrice : IPurchaseOfferMyNewPriceContainer
        where TOffer : IPurchaseofferPurchoffer
    {
        
    }
}
#pragma warning restore IDE1006 // Стили именования