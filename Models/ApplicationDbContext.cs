using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace QuanLyDaoTaoWeb.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public required DbSet<Khoa> Khoa { get; set; }
        public required DbSet<GiangVien> GiangVien { get; set; }
        public required DbSet<SinhVien> SinhVien { get; set; }
        public required DbSet<MonHoc> MonHoc { get; set; }
        public required DbSet<ChuongTrinhDaoTao> ChuongTrinhDaoTao { get; set; }
        public required DbSet<LopHoc> LopHoc { get; set; }
        public required DbSet<DangKyLopHoc> DangKyLopHoc { get; set; }
        public required DbSet<DeCuong> DeCuong { get; set; }
        public required DbSet<BaiGiang> BaiGiang { get; set; }
        public required DbSet<PhanCongGiangDay> PhanCongGiangDay { get; set; }
        public required DbSet<TaiLieu> TaiLieu { get; set; }
        public required DbSet<DanhGia> DanhGia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thêm index cho Email trong ApplicationUser
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Cấu hình bảng Khoa
            modelBuilder.Entity<Khoa>(entity =>
            {
                entity.Property(e => e.MaKhoa).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.TenKhoa).HasMaxLength(100);
                entity.Property(e => e.DiaChi).HasMaxLength(200);
                entity.Property(e => e.DienThoai).HasMaxLength(15);
            });

            // Cấu hình bảng GiangVien
            modelBuilder.Entity<GiangVien>(entity =>
            {
                entity.Property(e => e.MaGV).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.HoTen).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(50);
                entity.Property(e => e.MaKhoa).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.HasOne(gv => gv.Khoa)
                      .WithMany(gv => gv.GiangViens)
                      .HasForeignKey(gv => gv.MaKhoa);
            });

            // Cấu hình bảng SinhVien
            modelBuilder.Entity<SinhVien>(entity =>
            {
                entity.Property(e => e.MaSV).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.HoTen).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(50);
                entity.Property(e => e.MaKhoa).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.HasOne(sv => sv.Khoa)
                      .WithMany(sv => sv.SinhViens)
                      .HasForeignKey(sv => sv.MaKhoa);
            });

            // Cấu hình bảng MonHoc
            modelBuilder.Entity<MonHoc>(entity =>
            {
                entity.Property(e => e.MaMH).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.TenMH).HasMaxLength(100);
                entity.Property(e => e.MaKhoa).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.HasOne(mh => mh.Khoa)
                      .WithMany(mh => mh.MonHocs)
                      .HasForeignKey(mh => mh.MaKhoa);
            });

            // Cấu hình bảng ChuongTrinhDaoTao
            modelBuilder.Entity<ChuongTrinhDaoTao>(entity =>
            {
                entity.Property(e => e.MaCTDT).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.TenCTDT).HasMaxLength(100);
                entity.Property(e => e.MaKhoa).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.HasOne(ctdt => ctdt.Khoa)
                      .WithMany()
                      .HasForeignKey(ctdt => ctdt.MaKhoa);
            });

            // Cấu hình bảng LopHoc
            modelBuilder.Entity<LopHoc>(entity =>
            {
                entity.Property(e => e.MaLop).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.TenLop).HasMaxLength(50);
                entity.Property(e => e.MaCTDT).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.HasOne(lh => lh.ChuongTrinhDaoTao)
                      .WithMany(lh => lh.LopHocs)
                      .HasForeignKey(lh => lh.MaCTDT);
            });

            // Cấu hình bảng DangKyLopHoc
            modelBuilder.Entity<DangKyLopHoc>(entity =>
            {
                entity.HasKey(dk => new { dk.MaSV, dk.MaLopHoc });
                entity.Property(e => e.MaSV).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.MaLopHoc).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.HasOne(dk => dk.SinhVien)
                      .WithMany(s => s.DangKyLopHocs)
                      .HasForeignKey(dk => dk.MaSV);
                entity.HasOne(dk => dk.LopHoc)
                      .WithMany(c => c.DangKyLopHocs)
                      .HasForeignKey(dk => dk.MaLopHoc);
            });

            // Cấu hình bảng DeCuong
            modelBuilder.Entity<DeCuong>(entity =>
            {
                entity.Property(e => e.MaDC).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.MaMH).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.MoTa).HasMaxLength(500);
                entity.Property(e => e.MucTieu).HasMaxLength(500);
                entity.HasOne(dc => dc.MonHoc)
                      .WithMany()
                      .HasForeignKey(dc => dc.MaMH);
            });

            // Cấu hình bảng BaiGiang
            modelBuilder.Entity<BaiGiang>(entity =>
            {
                entity.Property(e => e.MaBG).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.MaMH).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.TieuDe).HasMaxLength(200);
                entity.HasOne(bg => bg.MonHoc)
                      .WithMany()
                      .HasForeignKey(bg => bg.MaMH);
            });

            // Cấu hình bảng PhanCongGiangDay
            modelBuilder.Entity<PhanCongGiangDay>(entity =>
            {
                entity.Property(e => e.MaPCGD).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.MaGV).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.MaMH).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.MaLop).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.HasOne(pcgd => pcgd.GiangVien)
                      .WithMany()
                      .HasForeignKey(pcgd => pcgd.MaGV);
                entity.HasOne(pcgd => pcgd.MonHoc)
                      .WithMany()
                      .HasForeignKey(pcgd => pcgd.MaMH);
                entity.HasOne(pcgd => pcgd.LopHoc)
                      .WithMany()
                      .HasForeignKey(pcgd => pcgd.MaLop);
            });

            // Cấu hình bảng TaiLieu
            modelBuilder.Entity<TaiLieu>(entity =>
            {
                entity.Property(e => e.MaTL).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.TenTL).HasMaxLength(200);
                entity.Property(e => e.MaBG).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.DuongDan).HasMaxLength(500);
                entity.HasOne(tl => tl.BaiGiang)
                      .WithMany()
                      .HasForeignKey(tl => tl.MaBG);
            });

            // Cấu hình bảng DanhGia
            modelBuilder.Entity<DanhGia>(entity =>
            {
                entity.Property(e => e.MaDG).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.MaSV).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.MaMH).HasColumnType("nvarchar(10)").HasMaxLength(10);
                entity.Property(e => e.NhanXet).HasMaxLength(500);
                entity.HasOne(dg => dg.SinhVien)
                      .WithMany()
                      .HasForeignKey(dg => dg.MaSV);
                entity.HasOne(dg => dg.MonHoc)
                      .WithMany()
                      .HasForeignKey(dg => dg.MaMH);
            });

            // Cấu hình ApplicationUser
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("nvarchar(450)").HasMaxLength(450);
                entity.Property(e => e.UserName).HasMaxLength(256);
                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
                entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Cấu hình IdentityRole
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("nvarchar(450)").HasMaxLength(450);
                entity.Property(e => e.Name).HasMaxLength(256);
                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            // Cấu hình IdentityUserLogin
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(e => e.LoginProvider).HasColumnType("nvarchar(450)").HasMaxLength(450);
                entity.Property(e => e.ProviderKey).HasColumnType("nvarchar(450)").HasMaxLength(450);
            });

            // Cấu hình IdentityUserRole
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnType("nvarchar(450)").HasMaxLength(450);
                entity.Property(e => e.RoleId).HasColumnType("nvarchar(450)").HasMaxLength(450);
            });

            // Cấu hình IdentityUserToken
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnType("nvarchar(450)").HasMaxLength(450);
                entity.Property(e => e.LoginProvider).HasColumnType("nvarchar(450)").HasMaxLength(450);
                entity.Property(e => e.Name).HasColumnType("nvarchar(450)").HasMaxLength(450);
            });

            // Cấu hình IdentityUserClaim
            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnType("nvarchar(450)").HasMaxLength(450);
            });

            // Cấu hình IdentityRoleClaim
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnType("nvarchar(450)").HasMaxLength(450);
            });

            // Đặt hành vi xóa mặc định là Restrict
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
