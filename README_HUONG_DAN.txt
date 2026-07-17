BINH SHELF CALCULATOR v1.0.1
Add-in Revit 2020 tính sức chứa giá/kệ cho quán ăn

========================================
1. MỤC TIÊU
========================================
Add-in có 2 nút chính trên ribbon:

1) Shelf Library
   - Quản lý thư viện loại giá/kệ.
   - Quản lý thư viện vật dụng dạng box chữ nhật.
   - Có sẵn vật dụng mặc định: Bếp ga mini, Bình ga mini, Cốc uống nước, Đĩa tròn, Bát, Đĩa bánh mì, Vỉ nướng, Khay chấm 3 ngăn, Bát chấm nhỏ, Âu để rau.
   - Cho phép tạo vật dụng mới với Dài / Rộng / Cao / Số lượng quy đổi.

2) Calculate Shelf
   - Chọn một đối tượng giá/kệ trong Revit.
   - Add-in chỉ đọc đúng 3 tham số của đối tượng được chọn: DÀI, RỘNG, CAO.
   - Không dùng BoundingBox để lấy kích thước kệ.
   - Không dùng Level Revit để tính.
   - "Tầng" trong add-in nghĩa là tầng/lớp/khoang của chính giá kệ.
   - Tính số lượng vật dụng có thể đặt vào từng tầng kệ và toàn bộ kệ.
   - Có thể tạo TextNote trên view hiện tại hoặc copy kết quả.

========================================
2. YÊU CẦU FAMILY GIÁ/KỆ
========================================
Family/Instance giá kệ cần có đúng 3 tham số instance:

DÀI
RỘNG
CAO

Ví dụ:
DÀI  = 3925 mm
RỘNG = 400 mm
CAO  = 800 mm

Add-in sẽ báo lỗi nếu đối tượng được chọn thiếu một trong ba tham số này.

========================================
3. CÁCH CÀI
========================================
Bước 1:
Giải nén file zip vào thư mục dễ quản lý, ví dụ:
D:\RevitAddin\BinhShelfCalculator_v1_0_0

Bước 2:
Mở file:
BinhShelfCalculator.sln
bằng Visual Studio 2019.

Bước 3:
Chọn cấu hình:
Debug | x64

Bước 4:
Build Solution.
Nhớ đóng Revit trước khi Build để tránh khóa DLL.

Bước 5:
Chạy file:
install_revit2020.bat

Nếu Windows hỏi quyền, chọn Run as administrator.

Bước 6:
Mở Revit 2020.
Trên ribbon sẽ có tab:
Binh Shelf

========================================
4. CÁCH DÙNG
========================================
A. Tạo/sửa thư viện
- Bấm Shelf Library.
- Tab Loại giá/kệ: nhập chiều dày tấm ngăn, khoảng cách giữa 2 tấm, khoảng thao tác thêm, hệ số sử dụng.
- Tab Vật dụng dạng box: tạo hoặc sửa vật dụng với Dài / Rộng / Cao.

B. Tính sức chứa
- Chọn một giá/kệ trong Revit hoặc bấm Calculate Shelf rồi chọn đối tượng.
- Chọn loại kệ.
- Chọn vật dụng.
- Chọn dùng khoảng cách đề xuất hoặc nhập tay khoảng cách giữa 2 tấm ngăn.
- Bấm Tính sức chứa.

========================================
5. NƠI LƯU THƯ VIỆN
========================================
Add-in tự tạo file thư viện tại:
Documents\BinhShelfCalculator\shelf_library.json

Nếu muốn reset thư viện, có thể đóng Revit và xóa file này. Add-in sẽ tự tạo lại dữ liệu mặc định khi mở lần sau.


GHI CHÚ v1.0.1:
- Sửa lỗi ambiguous Grid giữa WPF Grid và Revit DB Grid.
- Bổ sung using Models cho CmdCalculateShelf.
- Bổ sung reference System.Xml cho DataContractJsonSerializer.
