using System;
using System.Collections.Generic;
using System.Text;

namespace Sberbank.Bidding.Models
{
    public class FutureAuction
    {
        public int Id { get; set; }

        /// <summary>
        /// Код аукциона
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Время начала аукциона
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Минимальная цена, достигаемая в ходе аукциона
        /// </summary>
        public string MinCost { get; set; }
    }
}
