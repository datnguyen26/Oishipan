import apiClient from './apiClient';
import { VOUCHERS_ENDPOINT } from '../utils/constants';

/**
 * Voucher Service - Gọi API
 */
const voucherService = {
  /**
   * Lấy tất cả vouchers (Admin only)
   */
  async getAllVouchers() {
    return apiClient.get(VOUCHERS_ENDPOINT);
  },

  /**
   * Lấy chi tiết voucher theo ID (Admin only)
   */
  async getVoucherById(id) {
    return apiClient.get(`${VOUCHERS_ENDPOINT}/id/${id}`);
  },

  /**
   * Lấy voucher theo mã (Public)
   */
  async getVoucherByCode(code) {
    return apiClient.get(`${VOUCHERS_ENDPOINT}/${code}`);
  },

  /**
   * Kiểm tra tính hợp lệ của voucher (Public)
   */
  async validateVoucher(code) {
    return apiClient.get(`${VOUCHERS_ENDPOINT}/${code}/validate`);
  },

  /**
   * Tạo voucher mới (Admin only)
   */
  async createVoucher(data) {
    return apiClient.post(VOUCHERS_ENDPOINT, {
      code: data.code,
      name: data.name,
      description: data.description,
      discountType: data.discountType,
      discountValue: parseFloat(data.discountValue),
      maxDiscount: parseFloat(data.maxDiscount || data.discountValue),
      minOrderValue: parseFloat(data.minOrderValue || 0),
      startDate: data.startDate,
      endDate: data.endDate,
      usageLimit: parseInt(data.usageLimit, 10),
      status: data.status,
      category: data.category,
    });
  },

  /**
   * Cập nhật voucher (Admin only)
   */
  async updateVoucher(id, data) {
    return apiClient.put(`${VOUCHERS_ENDPOINT}/${id}`, {
      name: data.name || undefined,
      description: data.description || undefined,
      discountType: data.discountType || undefined,
      discountValue: data.discountValue ? parseFloat(data.discountValue) : undefined,
      maxDiscount: data.maxDiscount ? parseFloat(data.maxDiscount) : undefined,
      minOrderValue: data.minOrderValue ? parseFloat(data.minOrderValue) : undefined,
      startDate: data.startDate || undefined,
      endDate: data.endDate || undefined,
      usageLimit: data.usageLimit ? parseInt(data.usageLimit, 10) : undefined,
      status: data.status || undefined,
      category: data.category || undefined,
    });
  },

  /**
   * Xóa voucher (Admin only)
   */
  async deleteVoucher(id) {
    return apiClient.delete(`${VOUCHERS_ENDPOINT}/${id}`);
  },
};

export default voucherService;
