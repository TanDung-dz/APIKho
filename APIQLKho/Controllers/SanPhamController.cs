﻿using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using ZXing;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SanPhamController : ControllerBase
    {
        private readonly ILogger<SanPhamController> _logger;
        private readonly QlkhohangContext _context;
        private readonly Cloudinary _cloudinary;
        public SanPhamController(ILogger<SanPhamController> logger, QlkhohangContext context, Cloudinary cloudinary)
        {
            _logger = logger;
            _context = context;
            _cloudinary = cloudinary;
        }

		/// <summary>
		/// Lấy danh sách tất cả các sản phẩm.
		/// </summary>
		/// <returns>Một danh sách các sản phẩm, bao gồm thông tin loại sản phẩm và hãng sản xuất.</returns>
		// GET: api/sanpham
		[HttpGet]
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> Get()
        {
            var products = await _context.SanPhams
                                         .Where(sp => sp.Hide == false || sp.Hide == null) // Chỉ lấy sản phẩm không bị ẩn
                                         .Include(sp => sp.MaLoaiSanPhamNavigation)
                                         .Include(sp => sp.MaHangSanXuatNavigation)
                                         .Include(sp => sp.MaNhaCungCapNavigation) // Bao gồm thông tin nhà cung cấp
                                         .Select(sp => new SanPhamDto
                                         {
                                             MaSanPham = sp.MaSanPham,
                                             TenSanPham = sp.TenSanPham,
                                             Mota = sp.Mota,
                                             SoLuong = sp.SoLuong,
                                             DonGia = sp.DonGia,
                                             KhoiLuong = sp.KhoiLuong,
                                             KichThuoc = sp.KichThuoc,
                                             XuatXu = sp.XuatXu,
                                             Image = sp.Image,
                                             Image2 = sp.Image2,
                                             Image3 = sp.Image3,
                                             Image4 = sp.Image4,
                                             Image5 = sp.Image5,
                                             MaVach = sp.MaVach,
                                             TrangThai = sp.TrangThai,
                                             NgayTao = sp.NgayTao,
                                             NgayCapNhat = sp.NgayCapNhat,
                                             MaLoaiSanPham = sp.MaLoaiSanPham,
                                             TenLoaiSanPham = sp.MaLoaiSanPhamNavigation.TenLoaiSanPham,
                                             MaHangSanXuat = sp.MaHangSanXuat,
                                             TenHangSanXuat = sp.MaHangSanXuatNavigation.TenHangSanXuat,
                                             MaNhaCungCap = sp.MaNhaCungCap,
                                             TenNhaCungCap = sp.MaNhaCungCapNavigation.TenNhaCungCap
                                         })
                                         .ToListAsync();

            return Ok(products);
        }




        /// <summary>
        /// Lấy thông tin chi tiết của một sản phẩm dựa vào ID.
        /// </summary>
        /// <param name="id">ID của sản phẩm cần lấy thông tin.</param>
        /// <returns>Thông tin chi tiết của sản phẩm nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        // GET: api/sanpham/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPhamDto>> GetById(int id)
        {
            var product = await _context.SanPhams
                                        .Where(sp => sp.MaSanPham == id && (sp.Hide == false || sp.Hide == null)) // Chỉ lấy sản phẩm không bị ẩn
                                        .Include(sp => sp.MaLoaiSanPhamNavigation)
                                        .Include(sp => sp.MaHangSanXuatNavigation)
                                        .Include(sp => sp.MaNhaCungCapNavigation) // Bao gồm thông tin nhà cung cấp
                                        .Select(sp => new SanPhamDto
                                        {
                                            MaSanPham = sp.MaSanPham,
                                            TenSanPham = sp.TenSanPham,
                                            Mota = sp.Mota,
                                            SoLuong = sp.SoLuong,
                                            DonGia = sp.DonGia,
                                            KhoiLuong = sp.KhoiLuong,
                                            KichThuoc = sp.KichThuoc,
                                            XuatXu = sp.XuatXu,
                                            Image = sp.Image,
                                            Image2 = sp.Image2,
                                            Image3 = sp.Image3,
                                            Image4 = sp.Image4,
                                            Image5 = sp.Image5,
                                            MaVach = sp.MaVach,
                                            TrangThai = sp.TrangThai,
                                            NgayTao = sp.NgayTao,
                                            NgayCapNhat = sp.NgayCapNhat,
                                            MaLoaiSanPham = sp.MaLoaiSanPham,
                                            TenLoaiSanPham = sp.MaLoaiSanPhamNavigation.TenLoaiSanPham,
                                            MaHangSanXuat = sp.MaHangSanXuat,
                                            TenHangSanXuat = sp.MaHangSanXuatNavigation.TenHangSanXuat,
                                            MaNhaCungCap = sp.MaNhaCungCap,
                                            TenNhaCungCap = sp.MaNhaCungCapNavigation.TenNhaCungCap
                                        })
                                        .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(product);
        }



        [HttpPost]
        [Route("uploadfile")]
        public async Task<ActionResult<SanPham>> CreateProduct([FromForm] SanPhamDto newProductDto)
        {
            if (newProductDto == null)
            {
                return BadRequest("Product data is null.");
            }

            // Tạo sản phẩm mới
            var newProduct = new SanPham
            {
                TenSanPham = newProductDto.TenSanPham,
                Mota = newProductDto.Mota,
                SoLuong = newProductDto.SoLuong,
                DonGia = newProductDto.DonGia,
                KhoiLuong = newProductDto.KhoiLuong,
                KichThuoc = newProductDto.KichThuoc,
                XuatXu = newProductDto.XuatXu,
                MaLoaiSanPham = newProductDto.MaLoaiSanPham,
                MaHangSanXuat = newProductDto.MaHangSanXuat,
                MaNhaCungCap = newProductDto.MaNhaCungCap,
                NgayTao = DateTime.Now,
                TrangThai = true,
                Hide = false,
            };

            // Xử lý danh sách ảnh tải lên
            if (newProductDto.Images != null && newProductDto.Images.Any())
            {
                var imageUrls = new List<string>();
                foreach (var img in newProductDto.Images)
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(img.FileName, img.OpenReadStream()),
                        Folder = "products", // Thư mục trên Cloudinary
                        Transformation = new Transformation().Crop("limit").Width(800).Height(800)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return BadRequest("Failed to upload image to Cloudinary.");
                    }

                    imageUrls.Add(uploadResult.SecureUrl.ToString());
                }
                // Gán đường dẫn ảnh từ Cloudinary vào sản phẩm
                newProduct.Image = imageUrls.ElementAtOrDefault(0); // Ảnh 1
                newProduct.Image2 = imageUrls.ElementAtOrDefault(1); // Ảnh 2
                newProduct.Image3 = imageUrls.ElementAtOrDefault(2); // Ảnh 3
                newProduct.Image4 = imageUrls.ElementAtOrDefault(3); // Ảnh 4
                newProduct.Image5 = imageUrls.ElementAtOrDefault(4); // Ảnh 5
            }
            else
            {
                // Nếu không có ảnh nào được tải lên
                newProduct.Image = "";
                newProduct.Image2 = "";
                newProduct.Image3 = "";
                newProduct.Image4 = "";
                newProduct.Image5 = "";
            }

            // Thêm sản phẩm mới vào cơ sở dữ liệu
            _context.SanPhams.Add(newProduct);
            await _context.SaveChangesAsync();
            // Tạo mã vạch cho sản phẩm
            try
            {
                var barcodeWriter = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = 150,
                        Width = 300,
                        Margin = 10
                    }
                };

                var pixelData = barcodeWriter.Write(newProduct.MaSanPham.ToString());

                // Chuyển đổi mã vạch thành hình ảnh
                using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
                {
                    var bitmapData = bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppRgb);

                    try
                    {
                        Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    // Lưu mã vạch tạm thời vào MemoryStream
                    using (var memoryStream = new MemoryStream())
                    {
                        bitmap.Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        // Tải mã vạch lên Cloudinary
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription("barcode.png", memoryStream),
                            Folder = "barcodes", // Thư mục mã vạch trên Cloudinary
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            return BadRequest("Failed to upload barcode to Cloudinary.");
                        }

                        // Gán URL mã vạch vào sản phẩm
                        newProduct.MaVach = uploadResult.SecureUrl.ToString();
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating or uploading barcode.");
                return StatusCode(500, "Error generating or uploading barcode.");
            }
            return CreatedAtAction(nameof(GetById), new { id = newProduct.MaSanPham }, newProduct);
        }




        /// <summary>
        /// Cập nhật thông tin của một sản phẩm dựa vào ID.
        /// </summary>
        /// <param name="id">ID của sản phẩm cần cập nhật.</param>
        /// <param name="updatedProductDto">Thông tin sản phẩm cần cập nhật (dữ liệu từ DTO).</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        // PUT: api/sanpham/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] SanPhamDto updatedProductDto)
        {
            if (updatedProductDto == null)
            {
                return BadRequest("Product data is null.");
            }

            var existingProduct = await _context.SanPhams.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            // Cập nhật các thuộc tính của sản phẩm (không bao gồm ảnh)
            existingProduct.TenSanPham = updatedProductDto.TenSanPham;
            existingProduct.Mota = updatedProductDto.Mota;
            existingProduct.SoLuong = updatedProductDto.SoLuong;
            existingProduct.DonGia = updatedProductDto.DonGia;
            existingProduct.KhoiLuong = updatedProductDto.KhoiLuong;
            existingProduct.KichThuoc = updatedProductDto.KichThuoc;
            existingProduct.XuatXu = updatedProductDto.XuatXu;
            existingProduct.MaLoaiSanPham = updatedProductDto.MaLoaiSanPham;
            existingProduct.MaHangSanXuat = updatedProductDto.MaHangSanXuat;
            existingProduct.MaNhaCungCap = updatedProductDto.MaNhaCungCap;
            existingProduct.NgayCapNhat = DateTime.Now;

            // Xử lý danh sách ảnh tải lên (nếu có)
            if (updatedProductDto.Images != null && updatedProductDto.Images.Any())
            {
                //// Xóa ảnh cũ nếu tồn tại
                //var oldImages = new List<string>
                //{
                //    existingProduct.Image,
                //    existingProduct.Image2,
                //    existingProduct.Image3,
                //    existingProduct.Image4,
                //    existingProduct.Image5
                //};

                //foreach (var oldImagePath in oldImages)
                //{
                //    if (!string.IsNullOrEmpty(oldImagePath))
                //    {
                //        var fullOldImagePath = Path.Combine(Directory.GetCurrentDirectory(), oldImagePath.TrimStart('/'));
                //        if (System.IO.File.Exists(fullOldImagePath))
                //        {
                //            System.IO.File.Delete(fullOldImagePath);
                //        }
                //    }
                //}

                var imageUrls = new List<string>();
                foreach (var img in updatedProductDto.Images)
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(img.FileName, img.OpenReadStream()),
                        Folder = "products",
                        Transformation = new Transformation().Crop("limit").Width(800).Height(800)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return BadRequest("Failed to upload image to Cloudinary.");
                    }

                    imageUrls.Add(uploadResult.SecureUrl.ToString());
                }

                // Cập nhật các URL ảnh mới
                existingProduct.Image = imageUrls.ElementAtOrDefault(0);
                existingProduct.Image2 = imageUrls.ElementAtOrDefault(1);
                existingProduct.Image3 = imageUrls.ElementAtOrDefault(2);
                existingProduct.Image4 = imageUrls.ElementAtOrDefault(3);
                existingProduct.Image5 = imageUrls.ElementAtOrDefault(4);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.SanPhams.AnyAsync(sp => sp.MaSanPham == id))
                {
                    return NotFound("Product not found.");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        /// <summary>
        /// Xóa một sản phẩm dựa vào ID.
        /// </summary>
        /// <param name="id">ID của sản phẩm cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        // DELETE: api/sanpham/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.SanPhams.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // Cập nhật trường Hide thành true thay vì xóa sản phẩm
            product.Hide = true;

            try
            {
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.SanPhams.AnyAsync(sp => sp.MaSanPham == id))
                {
                    return NotFound("Product not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo tên hoặc mô tả.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm.</param>
        /// <returns>Danh sách các sản phẩm phù hợp với từ khóa.</returns>
        // GET: api/sanpham/search
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.SanPhams
                                              .Where(sp => (sp.Hide == false || sp.Hide == null) &&
                                                           (sp.TenSanPham.Contains(keyword) || sp.Mota.Contains(keyword)))
                                              .Include(sp => sp.MaLoaiSanPhamNavigation)
                                              .Include(sp => sp.MaHangSanXuatNavigation)
                                              .Include(sp => sp.MaNhaCungCapNavigation) // Bao gồm thông tin nhà cung cấp
                                              .Select(sp => new SanPhamDto
                                              {
                                                  MaSanPham = sp.MaSanPham,
                                                  TenSanPham = sp.TenSanPham,
                                                  Mota = sp.Mota,
                                                  SoLuong = sp.SoLuong,
                                                  DonGia = sp.DonGia,
                                                  KhoiLuong = sp.KhoiLuong,
                                                  KichThuoc = sp.KichThuoc,
                                                  XuatXu = sp.XuatXu,
                                                  Image = sp.Image,
                                                  Image2 = sp.Image2,
                                                  Image3 = sp.Image3,
                                                  Image4 = sp.Image4,
                                                  Image5 = sp.Image5,
                                                  MaVach = sp.MaVach,
                                                  TrangThai = sp.TrangThai,
                                                  NgayTao = sp.NgayTao,
                                                  NgayCapNhat = sp.NgayCapNhat,
                                                  MaLoaiSanPham = sp.MaLoaiSanPham,
                                                  TenLoaiSanPham = sp.MaLoaiSanPhamNavigation.TenLoaiSanPham,
                                                  MaHangSanXuat = sp.MaHangSanXuat,
                                                  TenHangSanXuat = sp.MaHangSanXuatNavigation.TenHangSanXuat,
                                                  MaNhaCungCap = sp.MaNhaCungCap,
                                                  TenNhaCungCap = sp.MaNhaCungCapNavigation.TenNhaCungCap
                                              })
                                              .ToListAsync();

            return Ok(searchResults);
        }
    }
}
