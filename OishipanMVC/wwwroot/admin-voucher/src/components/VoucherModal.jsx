import { generateRandomVoucherCode } from '../utils/helpers';
import { VOUCHER_CATEGORIES, DISCOUNT_TYPES } from '../utils/constants';

export const VoucherModal = ({
  isOpen,
  editingVoucher,
  formState,
  onFormChange,
  onGenerateCode,
  onSave,
  onClose,
}) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-950/80 backdrop-blur-md overflow-y-auto">
      <div className="relative w-full max-w-2xl bg-gradient-to-b from-slate-900 to-slate-950 rounded-3xl border border-slate-800 shadow-2xl shadow-amber-500/5 max-h-[90vh] overflow-y-auto">

        {/* Header Modal */}
        <div className="p-6 border-b border-slate-800/80 flex justify-between items-center bg-slate-950/60 sticky top-0 z-10 backdrop-blur">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-xl bg-gradient-to-tr from-amber-500 to-orange-500 p-0.5 flex items-center justify-center">
              <div className="w-full h-full bg-slate-950 rounded-[10px] flex items-center justify-center text-amber-400">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 110 4v3a2 2 0 002 2h14a2 2 0 002-2v-3a2 2 0 110-4V7a2 2 0 00-2-2H5z" />
                </svg>
              </div>
            </div>
            <div>
              <h3 className="text-lg font-extrabold text-white">
                {editingVoucher ? `Chỉnh Sửa Voucher ${editingVoucher.code}` : 'Tạo Chiến Dịch Voucher Mới'}
              </h3>
              <p className="text-xs text-slate-400 mt-0.5">Vui lòng điền đủ thông tin thiết lập cấu hình giảm giá</p>
            </div>
          </div>
          <button
            onClick={onClose}
            className="p-1.5 rounded-lg bg-slate-900 hover:bg-slate-800 text-slate-400 hover:text-white transition-colors"
          >
            <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Form Content */}
        <form onSubmit={onSave} className="p-6 space-y-6">

          {/* Mã Voucher & Danh Mục */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-bold uppercase text-slate-400 tracking-wider mb-2">
                Mã Code Voucher <span className="text-rose-500">*</span>
              </label>
              <div className="flex gap-2">
                <input
                  type="text"
                  placeholder="VD: OISHI100K"
                  value={formState.code}
                  onChange={(e) => onFormChange('code', e.target.value.toUpperCase())}
                  disabled={!!editingVoucher}
                  className="flex-1 px-4 py-2.5 bg-slate-950 border border-slate-800 focus:border-amber-500 text-white placeholder-slate-600 rounded-xl text-sm transition-all outline-none disabled:opacity-50 font-mono font-bold"
                  required
                />
                {!editingVoucher && (
                  <button
                    type="button"
                    onClick={() => onGenerateCode(generateRandomVoucherCode())}
                    className="px-3.5 py-2.5 bg-slate-800 hover:bg-slate-700 text-slate-200 rounded-xl text-xs font-bold transition-colors shrink-0 flex items-center gap-1.5"
                  >
                    ⚡ Ngẫu nhiên
                  </button>
                )}
              </div>
            </div>

            <div>
              <label className="block text-xs font-bold uppercase text-slate-400 tracking-wider mb-2">
                Danh mục áp dụng <span className="text-rose-500">*</span>
              </label>
              <select
                value={formState.category}
                onChange={(e) => onFormChange('category', e.target.value)}
                className="w-full px-4 py-2.5 bg-slate-950 border border-slate-800 focus:border-amber-500 text-white rounded-xl text-sm transition-all outline-none cursor-pointer"
              >
                {VOUCHER_CATEGORIES.map(cat => (
                  <option key={cat} value={cat}>{cat}</option>
                ))}
              </select>
            </div>
          </div>

          {/* Tên & Mô tả */}
          <div className="space-y-4">
            <div>
              <label className="block text-xs font-bold uppercase text-slate-400 tracking-wider mb-2">
                Tên chương trình voucher <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                placeholder="VD: Bánh Mì Sáng Năng Lượng"
                value={formState.name}
                onChange={(e) => onFormChange('name', e.target.value)}
                className="w-full px-4 py-2.5 bg-slate-950 border border-slate-800 focus:border-amber-500 text-white placeholder-slate-600 rounded-xl text-sm transition-all outline-none"
                required
              />
            </div>

            <div>
              <label className="block text-xs font-bold uppercase text-slate-400 tracking-wider mb-2">
                Mô tả chương trình hiển thị cho khách hàng
              </label>
              <textarea
                rows={2}
                placeholder="VD: Nhập mã này để được giảm ngay 15% tối đa 30.000đ khi mua bánh mì ngũ cốc..."
                value={formState.description}
                onChange={(e) => onFormChange('description', e.target.value)}
                className="w-full px-4 py-2.5 bg-slate-950 border border-slate-800 focus:border-amber-500 text-white placeholder-slate-600 rounded-xl text-sm transition-all outline-none resize-none"
              ></textarea>
            </div>
          </div>

          {/* Loại giảm giá & Giá trị */}
          <div className="bg-slate-950/60 p-4 rounded-2xl border border-slate-800/80 space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label className="block text-xs font-bold text-slate-400 tracking-wider mb-2">
                  Hình thức giảm giá
                </label>
                <div className="flex bg-slate-900 p-1 rounded-xl border border-slate-800">
                  {Object.values(DISCOUNT_TYPES).map(type => (
                    <button
                      key={type}
                      type="button"
                      onClick={() => onFormChange('discountType', type)}
                      className={`flex-1 py-1.5 rounded-lg text-xs font-bold transition-all ${
                        formState.discountType === type
                          ? 'bg-amber-500 text-slate-950 shadow-md'
                          : 'text-slate-400 hover:text-white'
                      }`}
                    >
                      {type === 'percentage' ? '% Phần trăm' : 'Tiền mặt (đ)'}
                    </button>
                  ))}
                </div>
              </div>

              <div>
                <label className="block text-xs font-bold text-slate-400 tracking-wider mb-2">
                  Mức ưu đãi giảm <span className="text-rose-500">*</span>
                </label>
                <div className="relative">
                  <input
                    type="number"
                    placeholder={formState.discountType === 'percentage' ? 'VD: 15' : 'VD: 30000'}
                    value={formState.discountValue}
                    onChange={(e) => onFormChange('discountValue', e.target.value)}
                    className="w-full pl-4 pr-10 py-2.5 bg-slate-950 border border-slate-800 focus:border-amber-500 text-white placeholder-slate-600 rounded-xl text-sm transition-all outline-none font-bold"
                    required
                    min="1"
                    max={formState.discountType === 'percentage' ? '100' : '500000'}
                  />
                  <span className="absolute inset-y-0 right-0 flex items-center pr-3.5 text-slate-400 font-bold text-xs pointer-events-none">
                    {formState.discountType === 'percentage' ? '%' : 'đ'}
                  </span>
                </div>
              </div>

              <div>
                <label className="block text-xs font-bold text-slate-400 tracking-wider mb-2">
                  Giảm giá tối đa
                </label>
                <div className="relative">
                  <input
                    type="number"
                    placeholder="VD: 50000"
                    value={formState.maxDiscount}
                    onChange={(e) => onFormChange('maxDiscount', e.target.value)}
                    disabled={formState.discountType === 'fixed'}
                    className="w-full pl-4 pr-10 py-2.5 bg-slate-950 border border-slate-800 focus:border-amber-500 text-white placeholder-slate-600 rounded-xl text-sm transition-all outline-none font-bold disabled:opacity-40"
                  />
                  <span className="absolute inset-y-0 right-0 flex items-center pr-3.5 text-slate-400 font-bold text-xs pointer-events-none">
                    đ
                  </span>
                </div>
              </div>
            </div>
          </div>

          {/* Điều kiện & Hạn mức */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-bold uppercase text-slate-400 tracking-wider mb-2">
                Giá trị hóa đơn tối thiểu (đ)
              </label>
              <div className="relative">
                <input
                  type="number"
                  placeholder="VD: 150000 (0 nếu không yêu cầu)"
                  value={formState.minOrderValue}
                  onChange={(e) => onFormChange('minOrderValue', e.target.value)}
                  className="w-full pl-4 pr-10 py-2.5 bg-slate-950 border border-slate-800 focus:border-amber-500 text-white placeholder-slate-600 rounded-xl text-sm transition-all outline-none font-semibold"
                />
                <span className="absolute inset-y-0 right-0 flex items-center pr-3.5 text-slate-400 font-bold text-xs pointer-events-none">
                  đ
                </span>
              </div>
            </div>

            <div>
              <label className="block text-xs font-bold uppercase text-slate-400 tracking-wider mb-2">
                Tổng số lượng phát hành tối đa
              </label>
              <input
                type="number"
                placeholder="VD: 500 lượt"
                value={formState.usageLimit}
                onChange={(e) => onFormChange('usageLimit', e.target.value)}
                className="w-full px-4 py-2.5 bg-slate-950 border border-slate-800 focus:border-amber-500 text-white placeholder-slate-600 rounded-xl text-sm transition-all outline-none font-semibold"
                required
                min="1"
              />
            </div>
          </div>

          {/* Hạn sử dụng */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-bold uppercase text-slate-400 tracking-wider mb-2">
                Ngày có hiệu lực
              </label>
              <input
                type="date"
                value={formState.startDate}
                onChange={(e) => onFormChange('startDate', e.target.value)}
                className="w-full px-4 py-2.5 bg-slate-950 border border-slate-800 focus:border-amber-500 text-white rounded-xl text-sm transition-all outline-none"
                required
              />
            </div>

            <div>
              <label className="block text-xs font-bold uppercase text-slate-400 tracking-wider mb-2">
                Ngày hết hiệu lực
              </label>
              <input
                type="date"
                value={formState.endDate}
                onChange={(e) => onFormChange('endDate', e.target.value)}
                className="w-full px-4 py-2.5 bg-slate-950 border border-slate-800 focus:border-amber-500 text-white rounded-xl text-sm transition-all outline-none"
                required
              />
            </div>
          </div>

          {/* Trạng thái hoạt động */}
          <div>
            <label className="block text-xs font-bold uppercase text-slate-400 tracking-wider mb-2">
              Trạng thái bắt đầu
            </label>
            <div className="grid grid-cols-3 gap-3">
              {[
                { label: 'Hoạt động luôn', value: 'active', desc: 'Cho phép khách dùng ngay' },
                { label: 'Hẹn giờ chạy', value: 'scheduled', desc: 'Chạy khi đến ngày' },
                { label: 'Vô hiệu hóa', value: 'expired', desc: 'Khóa voucher tạm thời' },
              ].map((opt) => (
                <button
                  key={opt.value}
                  type="button"
                  onClick={() => onFormChange('status', opt.value)}
                  className={`p-3 rounded-xl border text-left transition-all ${
                    formState.status === opt.value
                      ? 'bg-amber-500/10 border-amber-500 text-amber-300 ring-1 ring-amber-500/20'
                      : 'bg-slate-950 border-slate-800 text-slate-400 hover:border-slate-700'
                  }`}
                >
                  <div className="font-bold text-xs text-white">{opt.label}</div>
                  <div className="text-[10px] text-slate-500 mt-0.5 leading-tight">{opt.desc}</div>
                </button>
              ))}
            </div>
          </div>

          {/* Buttons */}
          <div className="pt-4 border-t border-slate-800/60 flex items-center justify-end gap-3">
            <button
              type="button"
              onClick={onClose}
              className="px-5 py-2.5 bg-slate-950 hover:bg-slate-900 border border-slate-800 text-slate-400 hover:text-white rounded-xl text-xs font-bold transition-all"
            >
              Hủy bỏ
            </button>
            <button
              type="submit"
              className="px-6 py-2.5 bg-gradient-to-r from-amber-400 to-orange-400 hover:from-amber-300 hover:to-orange-300 text-slate-950 font-bold rounded-xl text-xs tracking-wider shadow-lg shadow-amber-500/10 hover:scale-[1.01] active:scale-[0.98] transition-all"
            >
              {editingVoucher ? 'CẬP NHẬT' : 'PHÁT HÀNH VOUCHER'}
            </button>
          </div>

        </form>
      </div>
    </div>
  );
};

export default VoucherModal;
