import { useMemo } from 'react';
import { calculateStats } from '../utils/helpers';

export const StatsSection = ({ vouchers }) => {
  const stats = useMemo(() => calculateStats(vouchers), [vouchers]);

  return (
    <section className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
      {/* Card 1: Tổng số Voucher */}
      <div className="relative group overflow-hidden rounded-2xl bg-gradient-to-br from-slate-900 to-slate-950 p-6 border border-slate-800 hover:border-amber-500/40 transition-all duration-300">
        <div className="absolute top-0 right-0 w-24 h-24 bg-amber-500/5 rounded-full blur-2xl group-hover:bg-amber-500/10 transition-all"></div>
        <div className="flex justify-between items-start">
          <div>
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Tổng số chiến dịch</p>
            <h3 className="text-3xl font-black mt-2 text-white tracking-tight">{stats.total}</h3>
          </div>
          <div className="p-3 rounded-xl bg-amber-500/10 text-amber-400 border border-amber-500/20">
            <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 110 4v3a2 2 0 002 2h14a2 2 0 002-2v-3a2 2 0 110-4V7a2 2 0 00-2-2H5z" />
            </svg>
          </div>
        </div>
        <div className="mt-4 flex items-center gap-1.5 text-xs text-amber-300">
          <span className="font-bold">Đã lưu trữ ổn định</span>
          <span className="text-slate-500">•</span>
          <span className="text-slate-400">Thời gian thực</span>
        </div>
      </div>

      {/* Card 2: Đang Hoạt Động */}
      <div className="relative group overflow-hidden rounded-2xl bg-gradient-to-br from-slate-900 to-slate-950 p-6 border border-slate-800 hover:border-emerald-500/40 transition-all duration-300">
        <div className="absolute top-0 right-0 w-24 h-24 bg-emerald-500/5 rounded-full blur-2xl group-hover:bg-emerald-500/10 transition-all"></div>
        <div className="flex justify-between items-start">
          <div>
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Đang hoạt động</p>
            <h3 className="text-3xl font-black mt-2 text-emerald-400 tracking-tight">{stats.active}</h3>
          </div>
          <div className="p-3 rounded-xl bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 animate-pulse">
            <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
        </div>
        <div className="mt-4 flex items-center gap-1.5 text-xs text-slate-400">
          <span className="text-emerald-400 font-bold">Live</span>
          <span className="text-slate-500">•</span>
          <span>Khách đang áp dụng mua bánh</span>
        </div>
      </div>

      {/* Card 3: Tỷ lệ Sử Dụng */}
      <div className="relative group overflow-hidden rounded-2xl bg-gradient-to-br from-slate-900 to-slate-950 p-6 border border-slate-800 hover:border-rose-500/40 transition-all duration-300">
        <div className="absolute top-0 right-0 w-24 h-24 bg-rose-500/5 rounded-full blur-2xl group-hover:bg-rose-500/10 transition-all"></div>
        <div className="flex justify-between items-start">
          <div>
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Lượt sử dụng</p>
            <h3 className="text-3xl font-black mt-2 text-white tracking-tight">
              {stats.totalClaims} <span className="text-xs font-normal text-slate-400">lượt</span>
            </h3>
          </div>
          <div className="p-3 rounded-xl bg-rose-500/10 text-rose-400 border border-rose-500/20">
            <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
            </svg>
          </div>
        </div>
        {/* Thanh tiến độ sử dụng */}
        <div className="mt-4">
          <div className="flex justify-between text-xs mb-1.5">
            <span className="text-slate-400">Tỷ lệ phát hành</span>
            <span className="text-rose-400 font-bold">{stats.percentClaimed}%</span>
          </div>
          <div className="w-full bg-slate-800 h-2 rounded-full overflow-hidden">
            <div
              className="bg-gradient-to-r from-rose-500 to-amber-500 h-full rounded-full transition-all duration-500"
              style={{ width: `${Math.min(stats.percentClaimed, 100)}%` }}
            ></div>
          </div>
        </div>
      </div>

      {/* Card 4: Doanh Số Kích Cầu */}
      <div className="relative group overflow-hidden rounded-2xl bg-gradient-to-br from-slate-900 to-slate-950 p-6 border border-slate-800 hover:border-cyan-500/40 transition-all duration-300">
        <div className="absolute top-0 right-0 w-24 h-24 bg-cyan-500/5 rounded-full blur-2xl group-hover:bg-cyan-500/10 transition-all"></div>
        <div className="flex justify-between items-start">
          <div>
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Doanh số kích cầu (ước tính)</p>
            <h3 className="text-3xl font-black mt-2 text-cyan-400 tracking-tight">
              ₫{Math.round(stats.estimatedSales).toLocaleString('vi-VN')}
            </h3>
          </div>
          <div className="p-3 rounded-xl bg-cyan-500/10 text-cyan-400 border border-cyan-500/20">
            <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
        </div>
        <div className="mt-4 flex items-center gap-1.5 text-xs text-slate-400">
          <span className="text-cyan-400 font-bold">ROI ấn tượng</span>
          <span className="text-slate-500">•</span>
          <span>Từ lượng khách dùng voucher</span>
        </div>
      </div>
    </section>
  );
};

export default StatsSection;
