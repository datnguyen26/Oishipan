import { copyToClipboard, formatCurrency } from '../utils/helpers';

export const VoucherTable = ({
  vouchers,
  onEdit,
  onDelete,
  onToggleStatus,
}) => {
  if (vouchers.length === 0) {
    return (
      <div className="py-16 text-center">
        <div className="w-16 h-16 bg-slate-950 rounded-2xl flex items-center justify-center mx-auto text-slate-600 mb-4 border border-slate-800">
          <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
        </div>
        <h3 className="text-base font-bold text-white">Không tìm thấy voucher phù hợp</h3>
        <p className="text-slate-400 text-xs mt-1 max-w-xs mx-auto">Vui lòng điều chỉnh lại điều kiện tìm kiếm hoặc trạng thái lọc để tìm lại.</p>
      </div>
    );
  }

  return (
    <div className="overflow-x-auto">
      <table className="w-full text-left border-collapse">
        <thead>
          <tr className="bg-slate-950/60 border-b border-slate-800/80 text-[11px] font-bold uppercase text-slate-400 tracking-wider">
            <th className="py-4 px-6 text-center">Mã Code / Danh mục</th>
            <th className="py-4 px-6">Thông tin chương trình</th>
            <th className="py-4 px-6">Ưu đãi giảm giá</th>
            <th className="py-4 px-6">Điều kiện áp dụng</th>
            <th className="py-4 px-6">Lượt dùng / Hạn mức</th>
            <th className="py-4 px-6 text-center">Hạn sử dụng</th>
            <th className="py-4 px-6 text-center">Trạng thái</th>
            <th className="py-4 px-6 text-right">Tác vụ</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-800/60">
          {vouchers.map((v) => {
            const isExp = v.status === 'expired';
            const isSch = v.status === 'scheduled';
            const isActive = v.status === 'active';
            const usagePercent = Math.min((v.usageCount / v.usageLimit) * 100, 100);
            const startDateStr = v.startDate instanceof Date ? v.startDate.toISOString().split('T')[0] : v.startDate;
            const endDateStr = v.endDate instanceof Date ? v.endDate.toISOString().split('T')[0] : v.endDate;

            return (
              <tr key={v.voucherId} className="hover:bg-slate-800/25 transition-all group">
                {/* Mã Code / Danh mục */}
                <td className="py-5 px-6 whitespace-nowrap text-center">
                  <div className="flex flex-col items-center gap-1.5">
                    <div className="flex items-center gap-1 bg-slate-950 border border-slate-800 group-hover:border-amber-500/40 p-1 pr-2 rounded-lg transition-all">
                      <span className="font-mono text-sm font-bold bg-gradient-to-r from-amber-400 to-orange-400 bg-clip-text text-transparent px-2 py-0.5">
                        {v.code}
                      </span>
                      <button
                        onClick={() => {
                          copyToClipboard(v.code);
                        }}
                        title="Sao chép nhanh"
                        className="p-1 hover:bg-slate-800 rounded text-slate-400 hover:text-amber-400 transition-colors"
                      >
                        <svg xmlns="http://www.w3.org/2000/svg" className="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M8 5H6a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2v-1M8 5a2 2 0 002 2h2a2 2 0 002-2M8 5a2 2 0 012-2h2a2 2 0 012 2m0 0h2a2 2 0 012 2v3m2 4H10m0 0l3-3m-3 3l3 3" />
                        </svg>
                      </button>
                    </div>
                    <span className="inline-block px-2.5 py-0.5 rounded-full text-[10px] font-semibold bg-slate-800 text-slate-300 border border-slate-700">
                      🏷️ {v.category || 'Bánh Mì'}
                    </span>
                  </div>
                </td>

                {/* Thông tin chương trình */}
                <td className="py-5 px-6">
                  <div className="max-w-xs">
                    <h4 className="font-bold text-sm text-white group-hover:text-amber-300 transition-colors">{v.name}</h4>
                    <p className="text-xs text-slate-400 line-clamp-2 mt-1">{v.description}</p>
                  </div>
                </td>

                {/* Ưu đãi giảm giá */}
                <td className="py-5 px-6 whitespace-nowrap">
                  <div className="flex flex-col">
                    {v.discountType === 'percentage' ? (
                      <>
                        <span className="text-base font-extrabold text-amber-400">
                          Giảm {v.discountValue}%
                        </span>
                        <span className="text-[11px] text-slate-400">
                          Tối đa: ₫{formatCurrency(v.maxDiscount)}
                        </span>
                      </>
                    ) : (
                      <>
                        <span className="text-base font-extrabold text-rose-400">
                          Giảm ₫{formatCurrency(v.discountValue)}
                        </span>
                        <span className="text-[11px] text-slate-400">Khấu trừ trực tiếp</span>
                      </>
                    )}
                  </div>
                </td>

                {/* Điều kiện áp dụng */}
                <td className="py-5 px-6 whitespace-nowrap">
                  <div className="text-xs">
                    <div className="flex items-center gap-1">
                      <span className="text-slate-400">Hóa đơn từ:</span>
                      <span className="font-bold text-slate-200">₫{formatCurrency(v.minOrderValue)}</span>
                    </div>
                    <span className="text-[11px] text-slate-500 block mt-0.5">Áp dụng khách mua lẻ</span>
                  </div>
                </td>

                {/* Lượt dùng / Hạn mức */}
                <td className="py-5 px-6 whitespace-nowrap">
                  <div className="w-36">
                    <div className="flex justify-between items-center text-xs mb-1">
                      <span className="font-medium text-slate-300">
                        <strong className="text-white">{v.usageCount}</strong> / {v.usageLimit}
                      </span>
                      <span className="text-[10px] text-slate-500">
                        ({Math.round(usagePercent)}%)
                      </span>
                    </div>
                    <div className="w-full bg-slate-950 h-1.5 rounded-full overflow-hidden border border-slate-800">
                      <div
                        className={`h-full rounded-full ${
                          isExp ? 'bg-slate-600' : 'bg-gradient-to-r from-amber-500 to-rose-500'
                        }`}
                        style={{ width: `${usagePercent}%` }}
                      ></div>
                    </div>
                  </div>
                </td>

                {/* Hạn sử dụng */}
                <td className="py-5 px-6 whitespace-nowrap text-center">
                  <div className="text-xs">
                    <div className="text-slate-300">{endDateStr}</div>
                    <div className="text-[10px] text-slate-500 mt-0.5">Bắt đầu: {startDateStr}</div>
                  </div>
                </td>

                {/* Trạng thái */}
                <td className="py-5 px-6 whitespace-nowrap text-center">
                  <span className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-bold leading-none ${
                    isActive ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20' :
                    isSch ? 'bg-amber-500/10 text-amber-400 border border-amber-500/20' :
                    'bg-slate-800 text-slate-400 border border-slate-700'
                  }`}>
                    <span className={`w-1.5 h-1.5 rounded-full ${
                      isActive ? 'bg-emerald-400 animate-pulse' :
                      isSch ? 'bg-amber-400' :
                      'bg-slate-400'
                    }`}></span>
                    {isActive ? 'Đang chạy' : isSch ? 'Lên lịch' : 'Hết hạn'}
                  </span>
                </td>

                {/* Tác vụ */}
                <td className="py-5 px-6 whitespace-nowrap text-right">
                  <div className="flex items-center justify-end gap-2">
                    {/* Toggle Status */}
                    <button
                      onClick={() => onToggleStatus(v.voucherId, v.status)}
                      title={v.status === 'active' ? "Tạm dừng Voucher" : "Kích hoạt Voucher"}
                      className={`p-1.5 rounded-lg border transition-all ${
                        v.status === 'active'
                          ? 'bg-emerald-500/5 hover:bg-emerald-500/20 border-emerald-500/20 text-emerald-400'
                          : 'bg-slate-950 hover:bg-slate-800 border-slate-800 text-slate-400'
                      }`}
                    >
                      <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5.636 18.364a9 9 0 010-12.728m12.728 0a9 9 0 010 12.728m-9.9-2.829a5 5 0 010-7.071m7.071 0a5 5 0 010 7.071M13 12a1 1 0 11-2 0 1 1 0 012 0z" />
                      </svg>
                    </button>

                    {/* Edit Button */}
                    <button
                      onClick={() => onEdit(v)}
                      title="Chỉnh sửa thông tin"
                      className="p-1.5 rounded-lg bg-slate-950 hover:bg-slate-800 border border-slate-800 text-slate-300 hover:text-amber-400 transition-all"
                    >
                      <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                      </svg>
                    </button>

                    {/* Delete Button */}
                    <button
                      onClick={() => onDelete(v.voucherId, v.code)}
                      title="Xóa voucher"
                      className="p-1.5 rounded-lg bg-slate-950 hover:bg-rose-950/40 border border-slate-800 hover:border-rose-800/40 text-slate-400 hover:text-rose-400 transition-all"
                    >
                      <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                      </svg>
                    </button>
                  </div>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
};

export default VoucherTable;
