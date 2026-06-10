export const VoucherHeader = ({ onAddClick }) => (
  <header className="relative overflow-hidden border-b border-slate-800/80 bg-slate-900/40 backdrop-blur-md">
    {/* Vệt sáng Decor phía sau */}
    <div className="absolute top-0 left-1/4 -translate-y-1/2 w-96 h-96 bg-amber-500/10 rounded-full blur-3xl"></div>
    <div className="absolute -top-12 right-1/4 w-96 h-96 bg-rose-500/10 rounded-full blur-3xl"></div>

    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 flex flex-col md:flex-row justify-between items-center gap-6 relative z-10">
      <div className="flex items-center gap-4">
        {/* Logo Tiệm Bánh Oishipan */}
        <div className="w-14 h-14 rounded-2xl bg-gradient-to-tr from-amber-500 via-orange-500 to-rose-500 p-0.5 shadow-lg shadow-orange-500/20 flex items-center justify-center">
          <div className="w-full h-full bg-slate-950 rounded-[14px] flex flex-col items-center justify-center">
            <span className="text-xl font-black bg-gradient-to-r from-amber-400 to-orange-500 bg-clip-text text-transparent">おい</span>
            <span className="text-[10px] font-bold text-amber-200 tracking-wider -mt-1">PAN</span>
          </div>
        </div>
        <div>
          <div className="flex items-center gap-2">
            <h1 className="text-2xl font-extrabold tracking-tight bg-gradient-to-r from-white via-amber-100 to-amber-300 bg-clip-text text-transparent">
              OISHIPAN ADMIN
            </h1>
            <span className="bg-gradient-to-r from-amber-500/20 to-orange-500/20 border border-amber-500/30 text-amber-300 text-[10px] uppercase tracking-widest px-2.5 py-0.5 rounded-full font-bold">
              PRO PANEL
            </span>
          </div>
          <p className="text-xs text-slate-400 mt-1">Hệ thống quản lý chiến dịch phát hành Voucher kích cầu mua sắm</p>
        </div>
      </div>

      <div className="flex items-center gap-3">
        {/* Nút Tạo Mới Tỏa Sáng */}
        <button
          onClick={onAddClick}
          className="group relative px-6 py-3 rounded-xl font-bold text-sm tracking-wide text-slate-950 bg-gradient-to-r from-amber-400 via-orange-400 to-rose-400 hover:from-amber-300 hover:via-orange-300 hover:to-rose-300 shadow-xl shadow-orange-500/20 transition-all duration-300 hover:scale-[1.03] active:scale-[0.98] flex items-center gap-2 overflow-hidden"
        >
          {/* Hiệu ứng hào quang chạy qua nút */}
          <span className="absolute inset-0 w-full h-full bg-gradient-to-r from-transparent via-white/30 to-transparent -translate-x-full group-hover:animate-[shimmer_1.5s_infinite]"></span>
          <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 4v16m8-8H4" />
          </svg>
          TẠO VOUCHER MỚI
        </button>
      </div>
    </div>
  </header>
);

export default VoucherHeader;
