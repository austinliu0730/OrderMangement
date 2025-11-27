using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace OrderMangement.Models
{

    public class OrderSearchViewModel
    {
        private OMS_DBEntities db = new OMS_DBEntities();
        public string CustomerName {  get; set; }
        public string OrderDate { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();

        // 分頁相關屬性
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }

        // 排序相關屬性
        public string SortOrder { get; set; } = "desc";  // "desc" = 降冪（從新到舊），"asc" = 升冪（從舊到新）

        // 產品統計相關屬性
        public int? ProductID { get; set; }  // 選擇的產品 ID，null 或 0 表示全部產品
        public TotalPriceResult TotalPriceResult { get; set; }  // 存放 SP 回傳的統計結果

        public void SearchOrders()
        {
            //可以繼續串接查詢條件AsQueryable
            var orders = db.Orders.Include(o => o.Customer).Include(o => o.Product).AsQueryable();

            if (!string.IsNullOrEmpty(CustomerName))
            {
                orders = orders.Where(o => o.Customer.CustomerName.Contains(CustomerName));
            }

            if (!string.IsNullOrEmpty(OrderDate))
            {
                DateTime date = DateTime.Parse(OrderDate);
                orders = orders.Where(o => DbFunctions.TruncateTime(o.OrderDate) == date.Date);
            }

            // 計算總筆數
            TotalCount = orders.Count();

            // 確保頁碼有效
            if (PageNumber < 1)
            {
                PageNumber = 1;
            }

            // 根據排序參數套用排序
            if (SortOrder == "asc")
            {
                orders = orders.OrderBy(o => o.OrderDate);  // 升冪（從舊到新）
            }
            else
            {
                orders = orders.OrderByDescending(o => o.OrderDate);  // 降冪（從新到舊）
            }

            // 如果選擇了特定產品，執行 Total_Price Stored Procedure 取得統計資料
            if (ProductID.HasValue && ProductID.Value > 0)
            {
                var productIdParam = new SqlParameter("@ProductID", ProductID.Value);
                var result = db.Database.SqlQuery<TotalPriceResult>(
                    "EXEC Total_Price @ProductID",
                    productIdParam
                ).FirstOrDefault();

                TotalPriceResult = result;  // 儲存統計結果，供 View 顯示
            }
            else
            {
                TotalPriceResult = null;  // 沒有選擇產品時，清空統計結果
            }

            Orders = orders
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}