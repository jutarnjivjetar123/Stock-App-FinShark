using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Stock.UpdateStockRequestDto
{
    public class UpdateStockRequestDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Symbol must be at least 3 characters long")]
        [MaxLength(4, ErrorMessage = "Symbol must not be larger than 4 characters")]
        public string Symbol { get; set; } = string.Empty;
        [Required]
        [MinLength(1, ErrorMessage = "Company name must be at least one character long")]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000000000)]
        public decimal Purchase { get;  set; }
        [Required]
        [Range(0.00, 1000)]
        public decimal LastDiv { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Industry must be at least 5 characters long")]
        [MaxLength(30, ErrorMessage = "Industry must not be longer than 30 characters long")]
        public string Industry { get; set; } = string.Empty;
        [Required]
        [Range(1, 5000000000)]
        public long MarketCap { get; set; }
    }
}