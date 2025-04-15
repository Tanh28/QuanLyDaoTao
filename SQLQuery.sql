use QuanLyDaoTao

INSERT INTO Khoa (MaKhoa, TenKhoa, DiaChi, DienThoai) VALUES
('KHOA01', N'Khoa Công nghệ Thông tin', N'123 Đường ABC, Quận 1', '0123456789'),
('KHOA02', N'Khoa Kinh tế', N'456 Đường XYZ, Quận 2', '0987654321');

INSERT INTO ChuongTrinhDaoTao (MaCTDT, TenCTDT, NamBatDau, MaKhoa, TrangThai) VALUES
('CTDT01', N'Cử nhân Công nghệ Thông tin', 2020, 'KHOA01', 1),
('CTDT02', N'Cử nhân Kinh tế', 2021, 'KHOA02', 1);

INSERT INTO GiangVien (MaGV, HoTen, Email, MaKhoa, NgayNhanViec) VALUES
('GV001', N'Nguyễn Văn A', 'nguyenvana@email.com', 'KHOA01', '2020-01-15'),
('GV002', N'Trần Thị B', 'tranthib@email.com', 'KHOA02', '2021-03-10');

INSERT INTO SinhVien (MaSV, HoTen, NgaySinh, Email, MaKhoa) VALUES
('SV001', N'Lê Văn C', '2000-05-20', 'levanc@email.com', 'KHOA01'),
('SV002', N'Phạm Thị D', '2001-07-15', 'phamthid@email.com', 'KHOA02');

INSERT INTO MonHoc (MaMH, TenMH, SoTinChi, MaKhoa) VALUES
('MH001', N'Lập trình C#', 3, 'KHOA01'),
('MH002', N'Kinh tế vi mô', 2, 'KHOA02');

INSERT INTO LopHoc (MaLop, TenLop, MaCTDT) VALUES
('LH001', N'CNTT K45A', 'CTDT01'),
('LH002', N'Kinh tế K46B', 'CTDT02');

INSERT INTO DangKyLopHoc (MaSV, MaLopHoc, NgayDangKy) VALUES
('SV001', 'LH001', '2025-03-01 08:00:00'),
('SV002', 'LH002', '2025-03-02 09:00:00');

INSERT INTO DeCuong (MaDC, MaMH, MoTa, MucTieu, NgayCapNhat) VALUES
('DC001', 'MH001', N'Giới thiệu lập trình C#', N'Hiểu cú pháp cơ bản', '2025-01-10'),
('DC002', 'MH002', N'Cơ bản về kinh tế vi mô', N'Nắm nguyên lý cung cầu', '2025-02-15');

INSERT INTO BaiGiang (MaBG, MaMH, TieuDe, NoiDung) VALUES
('BG001', 'MH001', N'Bài 1: Biến và kiểu dữ liệu', N'Nội dung về biến trong C#...'),
('BG002', 'MH002', N'Bài 1: Cung và cầu', N'Nội dung về cung cầu...');

INSERT INTO PhanCongGiangDay (MaPCGD, MaGV, MaMH, MaLop, HocKy, NamHoc) VALUES
('PCGD001', 'GV001', 'MH001', 'LH001', 1, 2025),
('PCGD002', 'GV002', 'MH002', 'LH002', 1, 2025);

INSERT INTO TaiLieu (MaTL, TenTL, MaBG, DuongDan) VALUES
('TL001', N'Tài liệu biến C#', 'BG001', '/files/csharp_variables.pdf'),
('TL002', N'Tài liệu cung cầu', 'BG002', '/files/supply_demand.pdf');

INSERT INTO DanhGia (MaDG, MaSV, MaMH, DiemDanhGia, NhanXet, NgayDanhGia) VALUES
('DG001', 'SV001', 'MH001', 8, N'Bài giảng dễ hiểu', '2025-03-20'),
('DG002', 'SV002', 'MH002', 7, N'Cần thêm ví dụ', '2025-03-21');