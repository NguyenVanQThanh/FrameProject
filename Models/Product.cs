using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Supplier { get; set; }
        [Required]
        public string VDeDa { get; set; }
        [Required]
        public string LoaiDa { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity not lower than 0")]
        public int Quantity { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage ="Price not lower than 0")]
        public double Price { get; set; }
        [Required]
        public Boolean Enable {  get; set; }
        [Required]
        public string Sex { get; set; }
        public string? ImageUrl { get; set; }

        public static List<string> listCategory = new List<string> {
            "Sữa rửa mặt",
            "Kem chống nắng",
            "Serum",
            "Kem dưỡng ẩm",
            "Nước tẩy trang"
            };

        public static List<string> listSex = new List<string>
        {
            "Nam",
            "Nữ",
            "Nam & nữ"
        };
        public static List<string> listLoaiDa = new List<string>
        {
            "Mọi loại da",
            "Da nhạy cảm",
            "Da thường",
            "Da dầu",
            "Da khô"
        };
        public static List<string> listVanDeDa = new List<string>
        {
            "Da nhạy cảm, kích ứng, mẫn đỏ",
            "Da xạm, xỉn, không đều màu",
            "Da dầu, lỗ chân lông to",
            "Da khô, mất nước",
            "Da mụn",
            "Da bị tổn thương"
        };
        
        
    }
}
