using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
namespace DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { 

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(
                    new Product {
                        Id = 1,
                        Name = "Cetaphil",
                        Category = "Sữa rửa mặt",
                        Description = "Có khả năng làm sạch sâu, loại bỏ hoàn toàn bụi bẩn, và tạp chất trên da một cách dịu nhẹ nhưng vẫn duy trì độ ẩm tự nhiên để bảo vệ da khỏi tình trạng khô ráp, sữa rửa mặt Cetaphil mới với công thức lành tính không gây kích ứng sẽ an toàn cho mọi loại da, kể cả da nhạy cảm.",
                        Supplier = "VFU Shop",
                        Quantity = 15,
                        Price = 329000,
                        Enable = true,
                        LoaiDa = "Mọi loại da",
                        VDeDa = "Da nhạy cảm, kích ứng, mẫn đỏ",
                        Sex = "Nam & Nữ",
                        ImageUrl = ""
                    }, 
                    new Product
                    {
                        Id = 2,
                        Name = "Simple",
                        Category = "Sữa rửa mặt",
                        Description = "sản phẩm sữa rửa mặt dạng gel dành cho mọi loại da nổi tiếng. Với công thức dịu nhẹ không chứa xà phòng cùng thành phần Pro-Vitamin B5 và Vitamin E, sản phẩm giúp làm sạch da hiệu quả, cuốn đi chất nhờn, bụi bẩn và các tạp chất khác mà không gây kích ứng, cho da mềm mịn, đồng thời mang lại cảm giác tươi mát và sạch thoáng cho da.",
                        Supplier = "VFU Shop",
                        Quantity = 20,
                        Price = 100000,
                        Enable = true,
                        LoaiDa = "Da nhạy cảm",
                        VDeDa = "Da nhạy cảm, kích ứng, mẫn đỏ",
                        Sex = "Nam & Nữ",
                        ImageUrl = ""
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "Anessa",
                        Category = "Kem chống nắng",
                        Description = "Sản phẩm chống nắng đến từ thương hiệu chống nắng dưỡng da ANESSA hàng đầu Nhật Bản suốt 21 năm liên tiếp, giúp chống lại tác hại của tia UV & bụi mịn tối ưu dưới mọi điều kiện sinh hoạt, kể cả thời tiết khắc nghiệt nhất. Sản phẩm ứng dụng công nghệ Auto Booster và Very Water Resistant độc quyền từ thương hiệu ANESSA, giúp cho lớp màng chống UV trở nên bền vững hơn khi gặp NHIỆT ĐỘ CAO - ĐỘ ẨM - MỒ HÔI - NƯỚC - MA SÁT, đồng thời chống trôi trong nước lên đến 80 phút, chống bụi mịn PM.25 và chống dính cát. Ngoài ra, sự bổ sung của phức hợp 50% thành phần dưỡng da giúp ngăn ngừa lão hoá do tia UV hiệu quả và nuôi dưỡng da ẩm mịn.",
                        Supplier = "Olay",
                        Quantity = 45,
                        Price = 347000,
                        Enable = true,
                        LoaiDa = "Da dầu",
                        VDeDa = "Da dầu, lỗ chân lông to",
                        Sex = "Nam & Nữ",
                        ImageUrl = ""
                    },
                    new Product
                    {
                        Id = 4,
                        Name = "Gel Chống Nắng Bioré",
                        Category = "Kem chống nắng",
                        Description = "sản phẩm chống nắng da mặt đến từ thương hiệu mỹ phẩm Bioré của tập đoàn Kao Nhật Bản, với chiết xuất hương hoa mẫu đơn, sữa ong chúa giúp làm giảm kích ứng và dưỡng ẩm làn da trước các tác nhân gây hại từ UV .",
                        Supplier = "Revlon",
                        Quantity = 12,
                        Price = 172000,
                        Enable = true,
                        LoaiDa = "Mọi loại da",
                        VDeDa = "Da khô, mất nước",
                        Sex = "Nam & Nữ",
                        ImageUrl = ""
                    },
                    new Product
                    {
                        Id = 5,
                        Name = "Serum Garnier",
                        Category = "Serum",
                        Description = "sản phẩm serum đến từ thương hiệu mỹ phẩm Garnier của Pháp, với công thức vượt trội 4% phức hợp Vitamin C, BHA, Niacinamide, AHA có công dụng giảm mụn, mờ vết thâm và vết thâm do mụn đồng thời làn da sẽ được làm dịu, sáng hơn rõ rệt. Sản phẩm hoạt động theo cơ chế 3 tác động làm giảm bã nhờn - tiêu sừng - mờ thâm mang lại làn da sáng mịn, rạng ngời.",
                        Supplier = "Revlon",
                        Quantity = 40,
                        Price = 259000,
                        Enable = true,
                        LoaiDa = "Mọi loại da",
                        VDeDa = "Da mụn",
                        Sex = "Nam & Nữ",
                        ImageUrl = ""
                    },
                    new Product
                    {
                        Id = 6,
                        Name = "Sữa Dưỡng Ẩm Embryolisse",
                        Category = "Kem dưỡng ẩm",
                        Description = "sản phẩm kem dưỡng đa năng đến từ thương hiệu Embryolisse của Pháp, với các thành phần tự nhiên lành tính, không gây dị ứng, hỗ trợ cấp nước, giữ ẩm cho làn da luôn căng bóng, mềm mịn và rạng rỡ. Kết cấu sản phẩm dạng kem sữa, phù hợp làm lớp lót trước trang điểm, kem dưỡng ẩm hoặc mặt nạ dưỡng da, mang lại sự thoải mái cho cả những làn da khô & nhạy cảm nhất.",
                        Supplier = "Bici Cosmetic",
                        Quantity = 200,
                        Price = 375000,
                        Enable = true,
                        LoaiDa = "Mọi loại da",
                        VDeDa = "Da khô, mất nước",
                        Sex = "Nam",
                        ImageUrl = "\\images\\product\\fc9da19e-9b23-46ff-97fc-b39097f8fc02.jpeg"
                    },
                    new Product
                    {
                        Id = 7,
                        Name = "Nước Tẩy Trang Senka",
                        Category = "Tẩy trang",
                        Description = "dòng sản phẩm tẩy trang dạng nước từ thương hiệu mỹ phẩm SENKA Nhật Bản, với công thức Micellar giúp giúp làm sạch bụi bẩn, bã nhờn, lớp trang điểm lâu trôi tận sâu lỗ chân lông một cách hiệu quả mà vẫn dịu nhẹ cho làn da. Đặc biệt, mỗi phân loại được bổ sung các chiết xuất thiên nhiên giúp nuôi dưỡng và hỗ trợ cải thiện từng vấn đề về da riêng biệt.",
                        Supplier = "KissA Skincare",
                        Quantity = 120,
                        Price = 80000,
                        Enable = true,
                        LoaiDa = "Da thường",
                        VDeDa = "Da dầu, lỗ chân lông to",
                        Sex = "Nam & Nữ",
                        ImageUrl = ""
                    }
                );
        }

    }
}
