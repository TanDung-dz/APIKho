﻿namespace APIQLKho.Dtos
{
    public class BlogDto
    {
        public int BlogId { get; set; }
        public string? Anh { get; set; }
        public string? Mota { get; set; }
        public string? Link { get; set; }
        public bool? Hide { get; set; }
        public int MaNguoiDung { get; set; }
        public string? TenNguoiDung { get; set; }
        public IFormFile? Image { get; set; } // Để nhận file ảnh tải lên
    }
}
