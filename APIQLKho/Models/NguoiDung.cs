﻿using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public string? TenDangNhap { get; set; }

    public string? MatKhau { get; set; }

    public string? TenNguoiDung { get; set; }

    public string? Email { get; set; }

    public string? Sdt { get; set; }

    public string? Anh { get; set; }

    public DateTime? NgayDk { get; set; }

    public int? Quyen { get; set; }

    public bool? Hide { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();

    public virtual ICollection<PhieuNhapHang> PhieuNhapHangs { get; set; } = new List<PhieuNhapHang>();

    public virtual ICollection<PhieuXuatHang> PhieuXuatHangs { get; set; } = new List<PhieuXuatHang>();
}
