using ShareMarket.Core.Entities.Equities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareMarket.Core.Models.Dtos.Equities
{
    public class EquityStockCalculationDto : AuditableDto
    {
        public decimal PerChange_1W { get; set; }
        public decimal PerChange_15D { get; set; }
        public decimal PerChange_1M { get; set; }
        public decimal PerChange_3M { get; set; }
        public decimal PerChange_6M { get; set; }
        public decimal PerChange_1Y { get; set; }
        public decimal PerChange_2Y { get; set; }
        public decimal PerChange_3Y { get; set; }
        public decimal PerChange_5Y { get; set; }

        public decimal RSI { get; set; }
        public decimal RSI14EMA { get; set; }
        public decimal RSI14EMADiff { get; set; }
        public decimal DMA5 { get; set; }
        public decimal DMA10 { get; set; }
        public decimal DMA20 { get; set; }
        public decimal DMA50 { get; set; }
        public decimal DMA100 { get; set; }
        public decimal DMA200 { get; set; }

        public decimal YearHigh { get; set; }
        public DateOnly? YearHighOn { get; set; }
        public decimal YearLow { get; set; }
        public DateOnly? YearLowOn { get; set; }

        public bool IsRaising { get; set; }

        public long EquityStockId { get; set; }
        public EquityStock? EquityStock { get; set; }
    }
}
