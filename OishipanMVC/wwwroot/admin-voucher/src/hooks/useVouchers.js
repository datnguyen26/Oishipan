import { useState, useEffect } from 'react';
import voucherService from '../services/voucherService';

/**
 * Hook quản lý vouchers - Fetch, create, update, delete
 */
export const useVouchers = () => {
  const [vouchers, setVouchers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Lấy danh sách vouchers
  const fetchVouchers = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await voucherService.getAllVouchers();
      setVouchers(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // Fetch vouchers khi component mount
  useEffect(() => {
    fetchVouchers();
  }, []);

  // Thêm voucher mới
  const addVoucher = async (voucherData) => {
    try {
      const newVoucher = await voucherService.createVoucher(voucherData);
      setVouchers(prev => [newVoucher, ...prev]);
      return newVoucher;
    } catch (err) {
      setError(err.message);
      throw err;
    }
  };

  // Cập nhật voucher
  const updateVoucher = async (id, voucherData) => {
    try {
      const updatedVoucher = await voucherService.updateVoucher(id, voucherData);
      setVouchers(prev => prev.map(v => v.voucherId === id ? updatedVoucher : v));
      return updatedVoucher;
    } catch (err) {
      setError(err.message);
      throw err;
    }
  };

  // Xóa voucher
  const removeVoucher = async (id) => {
    try {
      await voucherService.deleteVoucher(id);
      setVouchers(prev => prev.filter(v => v.voucherId !== id));
    } catch (err) {
      setError(err.message);
      throw err;
    }
  };

  return {
    vouchers,
    loading,
    error,
    fetchVouchers,
    addVoucher,
    updateVoucher,
    removeVoucher,
  };
};
