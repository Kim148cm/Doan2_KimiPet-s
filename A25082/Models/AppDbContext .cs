using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using WebCafe.Models;

namespace A25082.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<LoaiKemChongNang> LoaiKemChongNangs { get; set; }
        public DbSet<LoaiAnh> LoaiAnhs { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SanPhamKemChongNang> SanPhamKemChongNangs { get; set; }
        public DbSet<AnhKemChongNang> AnhKemChongNangs { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<ChiTietThanhToan> ChiTietThanhToans { get; set; }

        // ── Spa ──────────────────────────────────────────────────
        public DbSet<DichVuSpa> DichVuSpas { get; set; }
        public DbSet<LoaiThuCung> LoaiThuCungs { get; set; }
        public DbSet<DatLichSpa> DatLichSpas { get; set; }
        // ─────────────────────────────────────────────────────────

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoaiKemChongNang>().ToTable("LoaiKemChongNang");
            modelBuilder.Entity<Slider>().ToTable("Slider");
            modelBuilder.Entity<LoaiAnh>().ToTable("LoaiAnh");
            modelBuilder.Entity<SanPhamKemChongNang>().ToTable("SanPhamKemChongNang");
            modelBuilder.Entity<AnhKemChongNang>().ToTable("AnhKemChongNang");
            modelBuilder.Entity<GioHang>().ToTable("GioHang");
            modelBuilder.Entity<ThanhToan>().ToTable("ThanhToan");
            modelBuilder.Entity<VaiTro>().ToTable("VaiTro");
            modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");
            modelBuilder.Entity<DanhGia>().ToTable("DanhGia");
            modelBuilder.Entity<ChiTietThanhToan>().ToTable("ChiTietThanhToan");

            // ── Spa ──────────────────────────────────────────────
            modelBuilder.Entity<DichVuSpa>().ToTable("DichVuSpa");
            modelBuilder.Entity<LoaiThuCung>().ToTable("LoaiThuCung");
            modelBuilder.Entity<DatLichSpa>().ToTable("DatLichSpa");
            // ─────────────────────────────────────────────────────

            base.OnModelCreating(modelBuilder);
        }
    }
}