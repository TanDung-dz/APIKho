﻿using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class Menu
{
    public int MenuId { get; set; }

    public string? Name { get; set; }

    public int? Order { get; set; }

    public string? Link { get; set; }

    public bool? Hide { get; set; }

    public int MaNguoiDung { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
