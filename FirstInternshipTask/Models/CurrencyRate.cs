using System.ComponentModel.DataAnnotations.Schema;

namespace FirstInternshipTask.Models
{
    // New Model CurrencyRate for Store data in SQL
    public class CurrencyRate
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PKR { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal INR { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EUR { get; set; }

        public DateTime LastUpdated { get; set; }

    }
}
