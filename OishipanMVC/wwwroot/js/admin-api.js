window.AdminApi = (() => {
  const BASE = (window.adminApiBase ? window.adminApiBase.replace(/\/$/, '') + '/api' : '/api');

  const defaultHeaders = {
    'Content-Type': 'application/json'
  };

  async function request(path, options = {}) {
    const response = await fetch(`${BASE}/${path}`, {
      credentials: 'include',
      ...options,
      headers: {
        ...defaultHeaders,
        ...(options.headers || {})
      }
    });

    const text = await response.text();
    const data = text ? JSON.parse(text) : null;

    if (!response.ok) {
      throw { status: response.status, data };
    }

    return data;
  }

  return {
    getProducts: () => request('products'),
    getCategories: () => request('categories'),
    getBrands: () => request('brands'),
    getOrders: () => request('orders'),
    getUsers: () => request('users'),
    createProduct: (payload) => request('products', { method: 'POST', body: JSON.stringify(payload) }),
    updateProduct: (id, payload) => request(`products/${id}`, { method: 'PUT', body: JSON.stringify(payload) }),
    deleteProduct: (id) => request(`products/${id}`, { method: 'DELETE' }),
    createCategory: (payload) => request('categories', { method: 'POST', body: JSON.stringify(payload) }),
    createBrand: (payload) => request('brands', { method: 'POST', body: JSON.stringify(payload) }),
    updateOrderStatus: (id, payload) => request(`orders/${id}/status`, { method: 'PUT', body: JSON.stringify(payload) }),
    cancelOrder: (id) => request(`orders/${id}/cancel`, { method: 'DELETE' }),
    getUsersList: (query = '') => request(`users${query ? '?' + query : ''}`),
    updateUser: (id, payload) => request(`users/${id}`, { method: 'PUT', body: JSON.stringify(payload) }),
    deleteUser: (id) => request(`users/${id}`, { method: 'DELETE' }),
    uploadProductImage: async (id, file) => {
      const formData = new FormData();
      formData.append('file', file);
      const response = await fetch(`${BASE}/products/${id}/upload-image`, {
        method: 'POST',
        credentials: 'include',
        body: formData
      });

      const data = await response.json().catch(() => null);
      if (!response.ok) {
        throw { status: response.status, data };
      }

      return data;
    }
  };
})();
