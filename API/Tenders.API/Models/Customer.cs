using System.ComponentModel.DataAnnotations;

namespace TenderPlanAPI.Models
{
    public class Customer : ModelBase
    {
        /// <summary>
        /// Наименование поставщика
        /// </summary>
        [Display(Name = "Наименование заказчика")]
        public string Name { get; set; }
        /// <summary>
        /// Номер поставщика
        /// </summary>
        [Display(Name = "Номер поставщика")]
        public string RegNum { get; set; }

        /// <summary>
        /// ИНН заказчика
        /// </summary>
        [Display(Name = "ИНН заказчика")]
        public string INN { get; set; }

        // <summary>
        // Адрес
        // </summary>
        [Display(Name = "Адрес")]
        public string Address { get; set; }
    }
}
