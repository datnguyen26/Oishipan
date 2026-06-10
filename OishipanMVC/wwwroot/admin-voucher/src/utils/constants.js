// API Configuration
export const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';
export const VOUCHERS_ENDPOINT = `${API_BASE_URL}/vouchers`;

// Discount Types
export const DISCOUNT_TYPES = {
  PERCENTAGE: 'percentage',
  FIXED: 'fixed',
};

// Statuses
export const VOUCHER_STATUSES = {
  ACTIVE: 'active',
  SCHEDULED: 'scheduled',
  EXPIRED: 'expired',
};

// Categories
export const VOUCHER_CATEGORIES = [
  'Bánh Mì',
  'Bánh Ngọt',
  'Nước uống',
  'Toàn bộ',
];

// Toast Types
export const TOAST_TYPES = {
  SUCCESS: 'success',
  ERROR: 'error',
  INFO: 'info',
};

// Voucher Code Prefixes
export const VOUCHER_PREFIXES = ['OISHI', 'BREAD', 'CAKE', 'SWEET', 'YUMMY', 'BAKERY'];
