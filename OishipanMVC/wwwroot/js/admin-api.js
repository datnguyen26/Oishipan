window.AdminApi = (() => {
  const BASE = (window.adminApiBase ? window.adminApiBase.replace(/\/$/, '') + '/api' : '/api');

  const defaultHeaders = {
    'Content-Type': 'application/json'
  };

  async function request(path, options = {}) {
    const headers = {
      ...defaultHeaders,
      ...(options.headers || {})
    };

    // Add JWT token to Authorization header if available
    const token = window.adminApiToken;
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(`${BASE}/${path}`, {
      credentials: 'include',
      ...options,
      headers
    });

    const text = await response.text();
    let data = null;
    
    try {
      data = text ? JSON.parse(text) : null;
    } catch (parseError) {
      console.error(`Failed to parse JSON from ${path}:`, text);
      data = { 
        error: 'Invalid JSON response',
        rawResponse: text 
      };
    }

    if (!response.ok) {
      const errorInfo = {
        status: response.status,
        statusText: response.statusText,
        path: path,
        data: data
      };
      console.error(`API Error [${response.status}] ${path}:`, errorInfo);
      throw errorInfo;
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
    updateCategory: (id, payload) => request(`categories/${id}`, { method: 'PUT', body: JSON.stringify(payload) }),
    deleteCategory: (id) => request(`categories/${id}`, { method: 'DELETE' }),
    createBrand: (payload) => request('brands', { method: 'POST', body: JSON.stringify(payload) }),
    updateBrand: (id, payload) => request(`brands/${id}`, { method: 'PUT', body: JSON.stringify(payload) }),
    deleteBrand: (id) => request(`brands/${id}`, { method: 'DELETE' }),
    updateOrderStatus: (id, payload) => request(`orders/${id}/status`, { method: 'PUT', body: JSON.stringify(payload) }),
    cancelOrder: (id) => request(`orders/${id}/cancel`, { method: 'DELETE' }),
    getUsersList: (query = '') => request(`users${query ? '?' + query : ''}`),
    updateUser: (id, payload) => request(`users/${id}`, { method: 'PUT', body: JSON.stringify(payload) }),
    deleteUser: (id) => request(`users/${id}`, { method: 'DELETE' }),
    uploadProductImage: async (id, file) => {
      const formData = new FormData();
      formData.append('file', file);
      
      const headers = {};
      const token = window.adminApiToken;
      if (token) {
        headers['Authorization'] = `Bearer ${token}`;
      }
      
      const response = await fetch(`${BASE}/products/${id}/upload-image`, {
        method: 'POST',
        credentials: 'include',
        headers,
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
