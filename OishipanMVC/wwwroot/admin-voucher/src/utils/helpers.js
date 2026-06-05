import { VOUCHER_PREFIXES } from './constants';

/**
 * Tạo mã voucher ngẫu nhiên
 */
export const generateRandomVoucherCode = () => {
  const randomPrefix = VOUCHER_PREFIXES[Math.floor(Math.random() * VOUCHER_PREFIXES.length)];
  const randomNum = Math.floor(1000 + Math.random() * 9000);
  return `${randomPrefix}${randomNum}`;
};

/**
 * Tính toán thống kê voucher
 */
export const calculateStats = (vouchers) => {
  const total = vouchers.length;
  const active = vouchers.filter(v => v.status === 'active').length;
  const totalClaims = vouchers.reduce((sum, v) => sum + v.usageCount, 0);
  const totalLimit = vouchers.reduce((sum, v) => sum + v.usageLimit, 0);
  const percentClaimed = totalLimit > 0 ? Math.round((totalClaims / totalLimit) * 100) : 0;

  // Ước tính doanh số (Giả lập mỗi voucher tạo ra giá trị đơn hàng trung bình gấp 4.5 lần giá trị giảm)
  const estimatedSales = vouchers.reduce((sum, v) => {
    const discount = v.discountType === 'percentage'
      ? Math.min((v.minOrderValue * v.discountValue) / 100, v.maxDiscount)
      : v.discountValue;
    return sum + (v.usageCount * discount * 4.5);
  }, 0);

  return { total, active, totalClaims, percentClaimed, estimatedSales };
};

/**
 * Lọc vouchers theo tiêu chí
 */
export const filterVouchers = (vouchers, searchTerm, statusFilter, categoryFilter) => {
  return vouchers.filter(v => {
    const matchesSearch = v.code.toLowerCase().includes(searchTerm.toLowerCase()) ||
      v.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      v.description.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'all' || v.status === statusFilter;
    const matchesCategory = categoryFilter === 'all' || v.category === categoryFilter;
    return matchesSearch && matchesStatus && matchesCategory;
  });
};

/**
 * Copy text vào clipboard
 */
export const copyToClipboard = (text) => {
  const tempInput = document.createElement('input');
  tempInput.value = text;
  document.body.appendChild(tempInput);
  tempInput.select();
  document.execCommand('copy');
  document.body.removeChild(tempInput);
};

/**
 * Format currency
 */
export const formatCurrency = (value) => {
  return value.toLocaleString('vi-VN');
};

/**
 * Format date
 */
export const formatDate = (date) => {
  if (typeof date === 'string') {
    return date;
  }
  return new Date(date).toISOString().split('T')[0];
};
