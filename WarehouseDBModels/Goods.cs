﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseDBModels
{
    /// <summary>
    /// 物品
    /// </summary>
    public class Goods
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string QuickCode { get; set; }

        public int GoodsUnitId { get; set; }//物品单位
        public int GoodsTypeId { get; set; }//物品类型
        [MaxLength(50)]
        public string Specification { get; set; }//规格
        public decimal SalePrice { get; set; }//零售价
        [MaxLength(200)]
        public string Remark { get; set; }//备注

        public bool IsStock { get; set; }//是否用于库存

        public bool IsDel { get; set; }
        public int DelUser { get; set; }
        public DateTime DelTime { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
