import { VOUCHER_CATEGORIES } from '../utils/constants';

export const FilterSection = ({
  searchTerm,
  onSearchChange,
  categoryFilter,
  onCategoryChange,
  statusFilter,
  onStatusChange,
  vouchers,
}) => {
  const getCountByStatus = (status) => {
    if (status === 'all') return vouchers.length;
    return vouchers.filter(v => v.status === status).length;
  };

  return (
    <div className="p-6 border-b border-slate-800 bg-slate-950/40 flex flex-col xl:flex-row gap-4 justify-between items-stretch xl:items-center">
      <div className="flex flex-col sm:flex-row gap-3 items-stretch sm:items-center flex-1">
        {/* Thanh tìm kiếm */}
        <div className="relative flex-1 max-w-md">
          <span className="absolute inset-y-0 left-0 flex items-center pl-3.5 pointer-events-none text-slate-500">
            <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          </span>
          <input
            type="text"
            placeholder="Tìm kiếm mã, tên voucher, chiến dịch..."
            value={searchTerm}
            onChange={(e) => onSearchChange(e.target.value)}
            className="w-full pl-11 pr-4 py-2.5 bg-slate-950 border border-slate-800 hover:border-slate-700 focus:border-amber-500 text-slate-200 placeholder-slate-500 rounded-xl text-sm transition-all outline-none focus:ring-1 focus:ring-amber-500/20"
          />
          {searchTerm && (
            <button
              onClick={() => onSearchChange('')}
              className="absolute inset-y-0 right-0 flex items-center pr-3 text-slate-400 hover:text-white"
            >
              <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          )}
        </div>

        {/* Lọc theo Danh mục */}
        <div className="relative min-w-[140px]">
          <select
            value={categoryFilter}
            onChange={(e) => onCategoryChange(e.target.value)}
            className="w-full px-3 py-2.5 bg-slate-950 border border-slate-800 text-slate-300 rounded-xl text-sm outline-none focus:border-amber-500 cursor-pointer transition-all"
          >
            <option value="all">Tất cả danh mục</option>
            {VOUCHER_CATEGORIES.map(cat => (
              <option key={cat} value={cat}>{cat}</option>
            ))}
          </select>
        </div>
      </div>

      {/* Các Tab Trạng Thái */}
      <div className="flex items-center gap-1.5 overflow-x-auto pb-2 xl:pb-0 scrollbar-none">
        {[
          { label: 'Tất cả', value: 'all' },
          { label: 'Đang hoạt động', value: 'active', color: 'text-emerald-400 bg-emerald-500/10' },
          { label: 'Chờ chạy', value: 'scheduled', color: 'text-amber-400 bg-amber-500/10' },
          { label: 'Hết hạn', value: 'expired', color: 'text-slate-400 bg-slate-800' }
        ].map((tab) => (
          <button
            key={tab.value}
            onClick={() => onStatusChange(tab.value)}
            className={`flex items-center gap-2 px-4 py-2 rounded-xl text-xs font-semibold transition-all shrink-0 border ${
              statusFilter === tab.value
                ? 'bg-gradient-to-r from-amber-500/25 to-orange-500/25 border-amber-500 text-amber-300 shadow-md shadow-amber-500/5'
                : 'bg-slate-950/80 border-slate-800 text-slate-400 hover:text-slate-200 hover:border-slate-700'
            }`}
          >
            {tab.label}
            <span className={`px-2 py-0.5 rounded-full text-[10px] font-bold ${
              statusFilter === tab.value ? 'bg-amber-400 text-slate-950' : 'bg-slate-800 text-slate-300'
            }`}>
              {getCountByStatus(tab.value)}
            </span>
          </button>
        ))}
      </div>
    </div>
  );
};

export default FilterSection;
