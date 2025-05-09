﻿using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KhachHangController : ControllerBase
    {
        private readonly ILogger<KhachHangController> _logger;
        private readonly QlkhohangContext _context;

        public KhachHangController(ILogger<KhachHangController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các khách hàng, bao gồm loại khách hàng và phiếu xuất hàng.
        /// </summary>
        /// <returns>Danh sách các khách hàng.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachHangDto>>> Get()
        {
            var customers = await _context.KhachHangs
                                          .Where(kh => kh.Hide == false || kh.Hide == null)  // Chỉ lấy khách hàng không bị ẩn
                                          .Include(kh => kh.MaLoaiNavigation)
                                          .Select(kh => new KhachHangDto
                                          {
                                              MaKhachHang = kh.MaKhachHang,
                                              TenKhachHang = kh.TenKhachHang,
                                              SoDt = kh.SoDt,
                                              Diachi = kh.Diachi,
                                              Email = kh.Email,
                                              MaLoai = kh.MaLoai,
                                              TenLoai = kh.MaLoaiNavigation.TenLoai
                                          })
                                          .ToListAsync();

            return Ok(customers);
        }


        /// <summary>
        /// Lấy thông tin chi tiết của một khách hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của khách hàng cần lấy.</param>
        /// <returns>Thông tin của khách hàng nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<KhachHangDto>> GetById(int id)
        {
            var customer = await _context.KhachHangs
                                         .Where(kh => kh.MaKhachHang == id && (kh.Hide == false || kh.Hide == null))  // Chỉ lấy khách hàng không bị ẩn
                                         .Include(kh => kh.MaLoaiNavigation)
                                         .Select(kh => new KhachHangDto
                                         {
                                             MaKhachHang = kh.MaKhachHang,
                                             TenKhachHang = kh.TenKhachHang,
                                             SoDt = kh.SoDt,
                                             Diachi = kh.Diachi,
                                             Email = kh.Email,
                                             MaLoai = kh.MaLoai,
                                             TenLoai = kh.MaLoaiNavigation.TenLoai
                                         })
                                         .FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            return Ok(customer);
        }


        /// <summary>
        /// Tạo mới một khách hàng.
        /// </summary>
        /// <param name="newCustomerDto">Thông tin của khách hàng mới cần tạo.</param>
        /// <returns>Khách hàng vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPost]
        public async Task<ActionResult<KhachHang>> CreateCustomer(KhachHangDto newCustomerDto)
        {
            if (newCustomerDto == null)
            {
                return BadRequest("Customer data is null.");
            }

            var newCustomer = new KhachHang
            {
                TenKhachHang = newCustomerDto.TenKhachHang,
                SoDt = newCustomerDto.SoDt,
                Diachi = newCustomerDto.Diachi,
                Email = newCustomerDto.Email,
                MaLoai = newCustomerDto.MaLoai,
                Hide = false
            };

            _context.KhachHangs.Add(newCustomer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newCustomer.MaKhachHang }, newCustomer);
        }

        /// <summary>
        /// Cập nhật thông tin của một khách hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của khách hàng cần cập nhật.</param>
        /// <param name="updatedCustomerDto">Thông tin mới của khách hàng.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, KhachHangDto updatedCustomerDto)
        {
            if (updatedCustomerDto == null)
            {
                return BadRequest("Customer data is null.");
            }

            var existingCustomer = await _context.KhachHangs.FindAsync(id);
            if (existingCustomer == null)
            {
                return NotFound("Customer not found.");
            }

            // Cập nhật các thuộc tính
            existingCustomer.TenKhachHang = updatedCustomerDto.TenKhachHang;
            existingCustomer.MaLoai = updatedCustomerDto.MaLoai;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.KhachHangs.AnyAsync(kh => kh.MaKhachHang == id))
                {
                    return NotFound("Customer not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa một khách hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của khách hàng cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.KhachHangs.FindAsync(id);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Cập nhật trường Hide thành true thay vì xóa
            customer.Hide = true;

            try
            {
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.KhachHangs.AnyAsync(kh => kh.MaKhachHang == id))
                {
                    return NotFound("Customer not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo tên.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (tên khách hàng).</param>
        /// <returns>Danh sách các khách hàng có tên phù hợp với từ khóa tìm kiếm.</returns>
        // GET: api/khachhang/search
        [HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<KhachHangDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.KhachHangs
											  .Include(kh => kh.MaLoaiNavigation)
											  .Where(kh => kh.TenKhachHang.Contains(keyword))
                                              .Select(kh => new KhachHangDto
                                              {
                                                  MaKhachHang = kh.MaKhachHang,
                                                  TenKhachHang = kh.TenKhachHang,
                                                  SoDt = kh.SoDt,
                                                  Diachi = kh.Diachi,
                                                  Email = kh.Email,
                                                  MaLoai = kh.MaLoai,
                                                  TenLoai = kh.MaLoaiNavigation.TenLoai
                                              })
                                          .ToListAsync();

            return Ok(searchResults);
		}

	}
}
