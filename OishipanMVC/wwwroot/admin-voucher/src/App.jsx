import React, { useState, useMemo } from 'react';
import VoucherHeader from './components/VoucherHeader';
import StatsSection from './components/StatsSection';
import FilterSection from './components/FilterSection';
import VoucherTable from './components/VoucherTable';
import VoucherModal from './components/VoucherModal';
import Toast from './components/Toast';
import { useVouchers } from './hooks/useVouchers';
import { useToast } from './hooks/useToast';
import { filterVouchers, copyToClipboard } from './utils/helpers';

export default function App() {
  // State từ hooks
  const { vouchers, loading, error, fetchVouchers, addVoucher, updateVoucher, removeVoucher } = useVouchers();
  const { toast, showToast } = useToast();

  // State Local
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [categoryFilter, setCategoryFilter] = useState('all');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingVoucher, setEditingVoucher] = useState(null);

  // Form State
  const [formState, setFormState] = useState({
    code: '',
    name: '',
    description: '',
    discountType: 'percentage',
    discountValue: '',
    maxDiscount: '',
    minOrderValue: '',
    startDate: new Date().toISOString().split('T')[0],
    endDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
    usageLimit: '',
    status: 'active',
    category: 'Bánh Mì'
  });

  // Lọc dữ liệu
  const filteredVouchers = useMemo(() => {
    return filterVouchers(vouchers, searchTerm, statusFilter, categoryFilter);
  }, [vouchers, searchTerm, statusFilter, categoryFilter]);

  // Xử lý mở modal thêm mới
  const handleOpenAddModal = () => {
    setEditingVoucher(null);
    setFormState({
      code: '',
      name: '',
      description: '',
      discountType: 'percentage',
      discountValue: '',
      maxDiscount: '',
      minOrderValue: '',
      startDate: new Date().toISOString().split('T')[0],
      endDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
      usageLimit: '',
      status: 'active',
      category: 'Bánh Mì'
    });
    setIsModalOpen(true);
  };

  // Xử lý mở modal chỉnh sửa
  const handleOpenEditModal = (voucher) => {
    setEditingVoucher(voucher);
    setFormState({
      code: voucher.code,
      name: voucher.name,
      description: voucher.description,
      discountType: voucher.discountType,
      discountValue: voucher.discountValue.toString(),
      maxDiscount: voucher.maxDiscount.toString(),
      minOrderValue: voucher.minOrderValue.toString(),
      startDate: voucher.startDate instanceof Date ? voucher.startDate.toISOString().split('T')[0] : voucher.startDate,
      endDate: voucher.endDate instanceof Date ? voucher.endDate.toISOString().split('T')[0] : voucher.endDate,
      usageLimit: voucher.usageLimit.toString(),
      status: voucher.status,
      category: voucher.category || 'Toàn bộ'
    });
    setIsModalOpen(true);
  };

  // Xử lý thay đổi form
  const handleFormChange = (key, value) => {
    setFormState(prev => ({ ...prev, [key]: value }));
  };

  // Xử lý lưu voucher
  const handleSaveVoucher = async (e) => {
    e.preventDefault();
    if (!formState.code || !formState.name || !formState.discountValue) {
      showToast('Vui lòng điền đầy đủ thông tin bắt buộc!', 'error');
      return;
    }

    try {
      if (editingVoucher) {
        // Cập nhật
        await updateVoucher(editingVoucher.voucherId, formState);
        showToast(`Đã cập nhật thành công voucher ${formState.code}!`, 'success');
      } else {
        // Tạo mới
        await addVoucher(formState);
        showToast(`Đã tạo thành công voucher mới: ${formState.code}!`, 'success');
      }
      setIsModalOpen(false);
    } catch (err) {
      showToast(err.message || 'Có lỗi xảy ra!', 'error');
    }
  };

  // Xử lý xóa voucher
  const handleDeleteVoucher = (id, code) => {
    if (window.confirm(`Bạn có chắc chắn muốn xóa voucher ${code}? Hành động này không thể hoàn tác.`)) {
      removeVoucher(id);
      showToast(`Đã xóa thành công voucher ${code}!`, 'success');
    }
  };

  // Xử lý toggle trạng thái
  const handleToggleStatus = async (id, currentStatus) => {
    const nextStatus = currentStatus === 'active' ? 'expired' : 'active';
    try {
      await updateVoucher(id, { status: nextStatus });
      showToast(`Trạng thái của voucher đã được chuyển thành ${nextStatus === 'active' ? 'Đang hoạt động' : 'Tạm dừng'}`, 'info');
    } catch (err) {
      showToast(err.message || 'Có lỗi xảy ra!', 'error');
    }
  };

  // Xử lý sao chép mã
  const handleCopyCode = (code) => {
    copyToClipboard(code);
    showToast(`Đã sao chép mã ${code} vào bộ nhớ tạm!`, 'success');
  };

  // Loading State
  if (loading) {
    return (
      <div className="min-h-screen bg-slate-950 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-amber-500 mx-auto mb-4"></div>
          <p className="text-slate-300">Đang tải dữ liệu...</p>
        </div>
      </div>
    );
  }

  // Error State
  if (error) {
    return (
      <div className="min-h-screen bg-slate-950 flex items-center justify-center p-4">
        <div className="text-center max-w-md">
          <div className="w-16 h-16 bg-rose-500/10 rounded-2xl flex items-center justify-center mx-auto mb-4 border border-rose-500/20">
            <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8 text-rose-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4v.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <h2 className="text-xl font-bold text-white mb-2">Lỗi tải dữ liệu</h2>
          <p className="text-slate-300 mb-6">{error}</p>
          <button
            onClick={() => fetchVouchers()}
            className="px-6 py-2.5 bg-amber-500 hover:bg-amber-400 text-slate-950 font-bold rounded-lg transition-colors"
          >
            Thử lại
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100 font-sans antialiased selection:bg-amber-500 selection:text-slate-900 pb-16">

      {/* Header */}
      <VoucherHeader onAddClick={handleOpenAddModal} />

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 relative">

        {/* Stats */}
        <StatsSection vouchers={vouchers} />

        {/* Filter & Table */}
        <div className="bg-slate-900/60 border border-slate-800 rounded-3xl overflow-hidden backdrop-blur-md shadow-2xl">

          {/* Filter */}
          <FilterSection
            searchTerm={searchTerm}
            onSearchChange={setSearchTerm}
            categoryFilter={categoryFilter}
            onCategoryChange={setCategoryFilter}
            statusFilter={statusFilter}
            onStatusChange={setStatusFilter}
            vouchers={vouchers}
          />

          {/* Table */}
          <VoucherTable
            vouchers={filteredVouchers}
            onEdit={handleOpenEditModal}
            onDelete={handleDeleteVoucher}
            onToggleStatus={handleToggleStatus}
          />
        </div>

      </main>

      {/* Modal */}
      <VoucherModal
        isOpen={isModalOpen}
        editingVoucher={editingVoucher}
        formState={formState}
        onFormChange={handleFormChange}
        onGenerateCode={(code) => handleFormChange('code', code)}
        onSave={handleSaveVoucher}
        onClose={() => setIsModalOpen(false)}
      />

      {/* Toast */}
      <Toast toast={toast} />

    </div>
  );
}
