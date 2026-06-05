# 🎟️ Oishipan Admin - Hệ Thống Quản Lý Voucher

Ứng dụng React hiện đại để quản lý các chiến dịch voucher/khuyến mại cho Tiệm Bánh Oishipan.

## ✨ Tính Năng

- ✅ **CRUD Voucher** - Tạo, xem, chỉnh sửa, xóa voucher
- ✅ **Quản Lý Trạng Thái** - Hoạt động, chờ chạy, hết hạn
- ✅ **Thống Kê Thực Thời** - Tổng voucher, đang hoạt động, lượt sử dụng, doanh số
- ✅ **Tìm Kiếm & Lọc** - Theo mã code, tên, danh mục, trạng thái
- ✅ **Sao Chép Mã** - Copy voucher code nhanh chóng
- ✅ **Toast Notification** - Thông báo trạng thái hành động
- ✅ **UI/UX Chuyên Nghiệp** - Dark theme với Tailwind CSS
- ✅ **Tự Động Tạo Mã** - Sinh voucher code ngẫu nhiên

## 📁 Cấu Trúc Project

```
admin-voucher/
├── src/
│   ├── components/          # React components
│   │   ├── VoucherHeader.jsx
│   │   ├── StatsSection.jsx
│   │   ├── FilterSection.jsx
│   │   ├── VoucherTable.jsx
│   │   ├── VoucherModal.jsx
│   │   └── Toast.jsx
│   ├── hooks/               # Custom React hooks
│   │   ├── useVouchers.js
│   │   └── useToast.js
│   ├── services/            # API services
│   │   ├── apiClient.js
│   │   └── voucherService.js
│   ├── utils/               # Utility functions
│   │   ├── constants.js
│   │   └── helpers.js
│   ├── App.jsx              # Main app component
│   ├── index.jsx            # Entry point
│   ├── index.css            # Global styles
│   └── index.html           # HTML template
├── package.json
├── vite.config.js           # Vite configuration
├── tailwind.config.js       # Tailwind CSS configuration
├── postcss.config.js
├── .env.example
└── README.md
```

## 🚀 Cài Đặt & Chạy

### 1. Install Dependencies

```bash
cd OishipanMVC/wwwroot/admin-voucher
npm install
```

### 2. Cấu Hình Environment

```bash
cp .env.example .env.local
```

Cập nhật `.env.local`:

```env
VITE_APP_API_URL=http://localhost:5000/api
```

### 3. Chạy Development Server

```bash
npm run dev
```

Server sẽ chạy tại `http://localhost:5000` (hoặc port tự động được cấp)

### 4. Build cho Production

```bash
npm run build
```

Output sẽ được lưu trong folder `dist/`

## 🔌 API Integration

### Backend Requirements

Ứng dụng này yêu cầu các API endpoints từ OishipanAPI:

#### Authentication
- Token JWT được lưu trong `sessionStorage` hoặc `localStorage`
- Authorization header: `Bearer {token}`

#### Endpoints

**Admin Only (Yêu cầu role Admin)**

```
GET    /api/vouchers              - Lấy danh sách tất cả vouchers
GET    /api/vouchers/id/{id}      - Lấy chi tiết voucher theo ID
POST   /api/vouchers              - Tạo voucher mới
PUT    /api/vouchers/{id}         - Cập nhật voucher
DELETE /api/vouchers/{id}         - Xóa voucher
```

**Public**

```
GET    /api/vouchers/{code}       - Lấy voucher theo mã
GET    /api/vouchers/{code}/validate - Kiểm tra tính hợp lệ
```

### Voucher Model

```javascript
{
  voucherId: number,
  code: string,
  name: string,
  description: string,
  discountType: 'percentage' | 'fixed',
  discountValue: decimal,
  maxDiscount: decimal,
  minOrderValue: decimal,
  startDate: datetime,
  endDate: datetime,
  usageLimit: number,
  usageCount: number,
  status: 'active' | 'scheduled' | 'expired',
  category: string,
  createdAt: datetime,
  updatedAt: datetime | null
}
```

## 📝 Hướng Dẫn Sử Dụng

### Tạo Voucher Mới

1. Nhấn nút **"TẠO VOUCHER MỚI"** ở góc phải header
2. Điền các thông tin bắt buộc:
   - Mã code voucher
   - Tên chương trình
   - Loại giảm giá (% hoặc tiền mặt)
   - Mức giảm
3. Cấu hình các tuỳ chọn:
   - Danh mục áp dụng
   - Giá trị hóa đơn tối thiểu
   - Hạn mức sử dụng
   - Ngày bắt đầu/kết thúc
   - Trạng thái ban đầu
4. Nhấn **"PHÁT HÀNH VOUCHER"** để lưu

### Chỉnh Sửa Voucher

1. Tìm voucher cần sửa trong bảng
2. Nhấn nút **Edit** (biểu tượng bút chì)
3. Thay đổi thông tin cần thiết
4. Nhấn **"CẬP NHẬT"** để lưu

> **Lưu ý**: Mã code không thể chỉnh sửa, chỉ các thông tin khác có thể sửa đổi

### Xóa Voucher

1. Tìm voucher cần xóa
2. Nhấn nút **Delete** (biểu tượng thùng rác)
3. Xác nhận xóa trong hộp thoại
4. Voucher sẽ bị xóa vĩnh viễn

### Tạm Dừng / Kích Hoạt

1. Tìm voucher cần thay đổi trạng thái
2. Nhấn nút **Toggle** (biểu tượng wifi/toggle)
3. Trạng thái sẽ được đảo ngược giữa "active" và "expired"

### Sao Chép Mã Voucher

1. Tìm voucher cần sao chép
2. Nhấn nút **Copy** (biểu tượng thẻ) bên cạnh mã code
3. Mã sẽ được sao chép vào clipboard

### Tìm Kiếm & Lọc

- **Tìm kiếm**: Nhập mã, tên, hoặc mô tả voucher
- **Lọc danh mục**: Chọn danh mục sản phẩm
- **Lọc trạng thái**: Chọn các tab (Tất cả, Đang hoạt động, Chờ chạy, Hết hạn)

## 🎨 Tùy Chỉnh Giao Diện

Toàn bộ giao diện được xây dựng với **Tailwind CSS**, dễ dàng tùy chỉnh:

- Cấu hình màu sắc: `tailwind.config.js`
- CSS toàn cục: `src/index.css`
- Component styles: Inline className trong các component

### Tham Khảo Các Hằng Số

- `src/utils/constants.js` - Các hằng số như API URL, status, categories
- `src/utils/helpers.js` - Các hàm tiện ích

## 🐛 Troubleshooting

### "Cannot GET /api/vouchers"
- Đảm bảo OishipanAPI đang chạy (port 5000 theo mặc định)
- Kiểm tra VITE_APP_API_URL trong `.env.local`
- Kiểm tra CORS configuration trong Program.cs

### "401 Unauthorized"
- Đảm bảo token JWT được lưu chính xác
- Kiểm tra token chưa hết hạn
- Đăng nhập lại bằng tài khoản admin

### "Failed to fetch"
- Kiểm tra kết nối internet
- Kiểm tra API server có online không
- Mở DevTools (F12) → Network tab để xem chi tiết

## 📊 Component Mô Tả

### App.jsx
Component chính quản lý state toàn bộ ứng dụng, xử lý CRUD operations, tìm kiếm, lọc.

### VoucherHeader.jsx
Header với logo Oishipan, tiêu đề, và nút "Tạo Voucher Mới".

### StatsSection.jsx
4 card thống kê: Tổng voucher, đang hoạt động, lượt sử dụng, doanh số ước tính.

### FilterSection.jsx
Thanh tìm kiếm, bộ lọc danh mục, và tab trạng thái.

### VoucherTable.jsx
Bảng hiển thị danh sách vouchers với các cột thông tin và tác vụ.

### VoucherModal.jsx
Modal form để tạo/chỉnh sửa voucher.

### Toast.jsx
Thông báo toast hiển thị ở góc phải dưới.

## 🔑 API Service

### voucherService.js

Các method chính:
- `getAllVouchers()` - Lấy danh sách
- `getVoucherById(id)` - Lấy chi tiết
- `createVoucher(data)` - Tạo mới
- `updateVoucher(id, data)` - Cập nhật
- `deleteVoucher(id)` - Xóa
- `validateVoucher(code)` - Kiểm tra tính hợp lệ

## 🎯 Custom Hooks

### useVouchers()
Quản lý state vouchers, loading, error, và CRUD operations.

```javascript
const { 
  vouchers,      // Danh sách vouchers
  loading,       // Đang tải?
  error,         // Lỗi nếu có
  fetchVouchers, // Fetch lại dữ liệu
  addVoucher,    // Thêm voucher
  updateVoucher, // Cập nhật voucher
  removeVoucher  // Xóa voucher
} = useVouchers();
```

### useToast()
Hiển thị thông báo toast.

```javascript
const { toast, showToast } = useToast();
showToast('Thành công!', 'success');
```

## 📦 Dependencies

- **React 18** - UI library
- **Vite 5** - Build tool
- **Tailwind CSS 3** - CSS framework
- **Fetch API** - HTTP client (built-in)

## 🚢 Deployment

### Build

```bash
npm run build
```

### Deploy to Static Hosting

1. Upload nội dung folder `dist/` lên hosting
2. Cấu hình reverse proxy để `/api` trỏ tới backend

### Docker (Optional)

```dockerfile
FROM node:18-alpine AS builder
WORKDIR /app
COPY package.json .
RUN npm install
COPY src .
RUN npm run build

FROM node:18-alpine
WORKDIR /app
RUN npm install -g serve
COPY --from=builder /app/dist dist
EXPOSE 3000
CMD ["serve", "-s", "dist", "-l", "3000"]
```

## 📞 Support

Liên hệ team development hoặc tạo issue tại repository.

## 📄 License

Private project - Oishipan Bakery

---

**Phiên Bản**: 0.0.1  
**Cập Nhật Lần Cuối**: 03/06/2026
