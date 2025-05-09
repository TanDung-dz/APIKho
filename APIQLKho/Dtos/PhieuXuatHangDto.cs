﻿namespace APIQLKho.Dtos
{
    public class PhieuXuatHangDto
    {
        public int MaPhieuXuatHang { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string? HinhThucThanhToan { get; set; }
        public decimal? PhiVanChuyen { get; set; }
        public int? TrangThai { get; set; }
        public bool? Hide { get; set; }
        // Thông tin cơ bản về người dùng và khách hàng
        public int MaNguoiDung { get; set; }
        public string? TenNguoiDung { get; set; } // Tên người dùng từ NguoiDung

        public int MaKhachHang { get; set; }
        public string? TenKhachHang { get; set; } // Tên khách hàng từ KhachHang
    }

}
