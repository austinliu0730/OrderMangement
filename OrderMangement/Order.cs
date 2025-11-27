
namespace OrderMangement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Order
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int CustomerID { get; set; }
        [Required(ErrorMessage = "數量為必填")]
        [Range(1, int.MaxValue, ErrorMessage = "數量必須為正整數")]
        public int Quantity { get; set; }
        public System.DateTime OrderDate { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }

    }
}
