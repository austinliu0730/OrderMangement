using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderMangement.Models
{
    /// <summary>
    /// 用來接收 Total_Price Stored Procedure 回傳的結果
    /// </summary>
    public class TotalPriceResult
    {
        public string ProductName { get; set; }

        public int TotalQuantity { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
