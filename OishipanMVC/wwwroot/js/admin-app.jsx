const { useState, useMemo, useEffect } = React;

const IMAGE_PLACEHOLDER = {
  banhMiThit: 'https://images.unsplash.com/photo-1509440159596-0249088772ff?auto=format&fit=crop&w=600&q=80',
  croissant: 'https://images.unsplash.com/photo-1555507036-ab1f4038808a?auto=format&fit=crop&w=600&q=80',
  banhNgot: 'https://images.unsplash.com/photo-1549931319-a545dcf3bc73?auto=format&fit=crop&w=600&q=80',
  banhKem: 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?auto=format&fit=crop&w=600&q=80',
  coffee: 'https://images.unsplash.com/photo-1509042239860-f550ce710b93?auto=format&fit=crop&w=600&q=80',
  defaultLogo: 'https://images.unsplash.com/photo-1528698827591-e19ccd7bc23d?auto=format&fit=crop&w=150&q=80'
};

const statusMap = {
  pending: 'Đang làm bánh',
  processing: 'Đang làm bánh',
  shipped: 'Đang giao hàng',
  completed: 'Đã giao hàng',
  cancelled: 'Đã hủy',
  canceled: 'Đã hủy'
};

const normalizeOrderStatus = (status) => {
  return statusMap[(status || '').toLowerCase()] || status || 'Chờ xử lý';
};

const safeParseJson = (value) => {
  if (!value) return [];
  try {
    return JSON.parse(value);
  } catch {
    return [];
  }
};

const normalizeProduct = (product) => {
  const variants = safeParseJson(product.VariantsJson || product.variants || '[]');
  return {
    id: product.ProductId || product.id,
    name: product.Name || product.name || 'Sản phẩm chưa tên',
    categoryId: product.CategoryId || product.categoryId || 0,
    category: product.Category?.CategoryName || product.category || '',
    brandId: product.BrandId || product.brandId || 0,
    brand: product.Brand?.BrandName || product.brand || '',
    price: Number(product.Price ?? product.price ?? 0),
    stock: Number(product.Quantity ?? product.stock ?? 0),
    image: product.Image || product.image || IMAGE_PLACEHOLDER.banhMiThit,
    status: Number(product.Quantity ?? product.stock ?? 0) > 0 ? 'Còn hàng' : 'Hết hàng',
    description: product.Description || product.description || '',
    variants
  };
};

const normalizeCategory = (category) => ({
  id: category.CategoryId || category.id,
  name: category.CategoryName || category.name || 'Danh mục',
  description: category.Description || category.description || '',
  slug: (category.CategoryName || category.name || '').toString().toLowerCase().replace(/\s+/g, '-'),
  count: 0
});

const normalizeBrand = (brand) => ({
  id: brand.BrandId || brand.id,
  name: brand.BrandName || brand.name || 'Thương hiệu',
  origin: brand.origin || 'Việt Nam',
  description: brand.Description || brand.description || '',
  logo: brand.Logo || brand.logo || IMAGE_PLACEHOLDER.defaultLogo
});

const normalizeUser = (user) => ({
  id: user.UserId || user.id,
  name: user.FullName || user.name || 'Người dùng',
  email: user.Email || user.email || 'unknown@oishipan.vn',
  role: (user.Role || user.role || 'user').toLowerCase(),
  status: user.Status === false ? 'Tạm khóa' : 'Hoạt động',
  avatar: user.Avatar || user.avatar || `https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=150&q=80`,
  phone: user.PhoneNumber || user.phone || '',
  address: user.Address || user.address || ''
});

const normalizeOrder = (order) => {
  const items = (order.OrderDetails || order.items || []).map((item) => ({
    name: (item.Product?.Name || item.name || `Sản phẩm #${item.ProductId ?? item.productId}`),
    qty: item.Quantity || item.qty || 0,
    price: Number(item.Price || item.price || 0)
  }));

  return {
    id: order.OrderId || order.id,
    customer: order.Customer || order.customer || `Khách hàng #${order.UserId || order.userId || '---'}`,
    phone: order.Phone || order.phone || 'Chưa có',
    date: order.OrderDate ? new Date(order.OrderDate).toLocaleDateString('vi-VN') : (order.date || ''),
    items,
    total: Number(order.TotalAmount || order.total || 0),
    status: normalizeOrderStatus(order.Status || order.status)
  };
};

function App() {
  const [activeTab, setActiveTab] = useState('dashboard');
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [brands, setBrands] = useState([]);
  const [users, setUsers] = useState([]);
  const [orders, setOrders] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  const [adminProfile, setAdminProfile] = useState(window.adminInitialProfile || {
    id: 'ND001',
    name: 'Admin Oishipan',
    nickname: 'Quản trị viên',
    email: 'admin@oishipan.vn',
    role: 'admin',
    status: 'Hoạt động',
    avatar: 'https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=150&q=80',
    joinDate: '12-04-2024',
    actionsCount: 142
  });

  const [searchQuery, setSearchQuery] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('Tất cả');
  const [orderStatusFilter, setOrderStatusFilter] = useState('Tất cả');

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [modalType, setModalType] = useState('');
  const [editingItem, setEditingItem] = useState(null);

  const [uploadedProductImageUrl, setUploadedProductImageUrl] = useState('');
  const [uploadedProductFile, setUploadedProductFile] = useState(null);
  const [uploadedBrandLogoUrl, setUploadedBrandLogoUrl] = useState('');
  const [uploadedBrandLogoFile, setUploadedBrandLogoFile] = useState(null);

  const [isVariantModalOpen, setIsVariantModalOpen] = useState(false);
  const [selectedProductForVariants, setSelectedProductForVariants] = useState(null);
  const [variantFormMode, setVariantFormMode] = useState('add');
  const [editingVariant, setEditingVariant] = useState(null);
  const [variantName, setVariantName] = useState('');
  const [variantPriceAdj, setVariantPriceAdj] = useState(0);
  const [variantStock, setVariantStock] = useState(10);

  const [toasts, setToasts] = useState([]);

  const showToast = (message, type = 'success') => {
    const id = Date.now();
    setToasts((prev) => [...prev, { id, message, type }]);
    setTimeout(() => setToasts((prev) => prev.filter((toast) => toast.id !== id)), 4000);
  };

  const loadData = async () => {
    try {
      setIsLoading(true);
      setErrorMessage('');

      const [rawProducts, rawCategories, rawBrands, rawOrders] = await Promise.all([
        window.AdminApi.getProducts(),
        window.AdminApi.getCategories(),
        window.AdminApi.getBrands(),
        window.AdminApi.getOrders()
      ]);

      let rawUsers = [];
      try {
        rawUsers = await window.AdminApi.getUsersList();
      } catch (uErr) {
        rawUsers = [];
        if (uErr?.status === 401) showToast('Không có quyền truy cập danh sách người dùng (401).', 'warning');
      }

      const categoryList = rawCategories.map(normalizeCategory);
      const brandList = rawBrands.map(normalizeBrand);
      const userList = rawUsers.Users ? rawUsers.Users.map(normalizeUser) : rawUsers.map(normalizeUser);
      const productList = rawProducts.map((product) => {
        const item = normalizeProduct(product);
        item.category = categoryList.find((c) => c.id === item.categoryId)?.name || item.category;
        item.brand = brandList.find((b) => b.id === item.brandId)?.name || item.brand;
        return item;
      });

      const orderList = rawOrders.map(normalizeOrder);
      const categoriesWithCount = categoryList.map((category) => ({
        ...category,
        count: productList.filter((product) => product.categoryId === category.id).length
      }));

      setCategories(categoriesWithCount);
      setBrands(brandList);
      setUsers(userList);
      setProducts(productList);
      setOrders(orderList);
    } catch (error) {
      const message = error?.data?.message || error?.data?.errors || error?.message || 'Không thể tải dữ liệu admin từ API.';
      setErrorMessage(typeof message === 'string' ? message : JSON.stringify(message));
      showToast('Lỗi khi tải dữ liệu từ API.', 'danger');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const stats = useMemo(() => {
    const totalRev = orders.filter((o) => ['Đã giao hàng', 'Đang giao hàng', 'Đang làm bánh'].includes(o.status)).reduce((sum, o) => sum + o.total, 0);
    const activeProducts = products.filter((p) => p.status === 'Còn hàng').length;
    const totalOrders = orders.length;
    const activeCust = users.filter((u) => u.role === 'user').length;
    return { totalRev, activeProducts, totalOrders, activeCust };
  }, [products, orders, users]);

  const syncAdminWithUsersList = (updatedAdmin) => {
    setUsers((prev) => prev.map((u) => {
      if (u.id === updatedAdmin.id) {
        return {
          ...u,
          name: updatedAdmin.nickname || updatedAdmin.name,
          email: updatedAdmin.email,
          avatar: updatedAdmin.avatar
        };
      }
      return u;
    }));
  };

  const handleDelete = async (id, type) => {
    try {
      if (type === 'product') {
        await window.AdminApi.deleteProduct(id);
        setProducts((prev) => prev.filter((p) => p.id !== id));
        showToast('Đã xóa sản phẩm thành công!', 'success');
      } else if (type === 'category') {
        showToast('Xóa danh mục hiện tại chưa được hỗ trợ bởi API admin.', 'warning');
      } else if (type === 'brand') {
        showToast('Xóa thương hiệu hiện tại chưa được hỗ trợ bởi API admin.', 'warning');
      } else if (type === 'user') {
        const userToDelete = users.find((u) => u.id === id);
        if (userToDelete && userToDelete.role === 'admin') {
          showToast('Không thể xóa tài khoản Quản trị viên tối cao!', 'danger');
          return;
        }
        await window.AdminApi.deleteUser(id);
        setUsers((prev) => prev.filter((u) => u.id !== id));
        showToast('Đã xóa tài khoản người dùng!', 'warning');
      } else if (type === 'order') {
        await window.AdminApi.cancelOrder(id);
        setOrders((prev) => prev.map((o) => (o.id === id ? { ...o, status: 'Đã hủy' } : o)));
        showToast('Đã hủy đơn hàng thành công!', 'warning');
      }
    } catch (error) {
      showToast('Lỗi khi xóa dữ liệu. Kiểm tra quyền truy cập API.', 'danger');
    }
  };

  const openModal = (type, item = null) => {
    setModalType(type);
    setEditingItem(item);
    if (type === 'product') {
      setUploadedProductImageUrl(item ? item.image : '');
      setUploadedProductFile(null);
    } else if (type === 'brand') {
      setUploadedBrandLogoUrl(item ? item.logo : '');
      setUploadedBrandLogoFile(null);
    }
    setIsModalOpen(true);
  };

  const handleProductImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setUploadedProductFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setUploadedProductImageUrl(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleBrandLogoChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setUploadedBrandLogoFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setUploadedBrandLogoUrl(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleAdminAvatarChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        const avatarBase64 = reader.result;
        setAdminProfile((prev) => {
          const updated = { ...prev, avatar: avatarBase64 };
          syncAdminWithUsersList(updated);
          return updated;
        });
        showToast('Đã tải ảnh đại diện Admin mới lên!', 'success');
      };
      reader.readAsDataURL(file);
    }
  };

  const saveProductVariantMeta = async (product) => {
    const payload = {
      Name: product.name,
      Price: product.price,
      Quantity: product.stock,
      CategoryId: product.categoryId,
      BrandId: product.brandId,
      Description: product.description || '',
      VariantsJson: JSON.stringify(product.variants || [])
    };

    await window.AdminApi.updateProduct(product.id, payload);
  };

  const handleSaveItem = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);

    try {
      if (modalType === 'product') {
        const payload = {
          Name: formData.get('name'),
          Price: parseFloat(formData.get('price')) || 0,
          Quantity: parseInt(formData.get('stock'), 10) || 0,
          CategoryId: parseInt(formData.get('category'), 10) || 0,
          BrandId: parseInt(formData.get('brand'), 10) || 0,
          Description: formData.get('description') || '',
          VariantsJson: JSON.stringify(editingItem?.variants || [])
        };

        if (editingItem) {
          await window.AdminApi.updateProduct(editingItem.id, payload);
          const updated = { ...editingItem, ...payload, status: payload.Quantity > 0 ? 'Còn hàng' : 'Hết hàng', variants: editingItem.variants };
          if (uploadedProductFile) {
            const uploadResult = await window.AdminApi.uploadProductImage(editingItem.id, uploadedProductFile);
            if (uploadResult?.url) updated.image = uploadResult.url;
          }
          setProducts((prev) => prev.map((p) => (p.id === editingItem.id ? updated : p)));
          showToast('Đã cập nhật thông tin sản phẩm!', 'success');
        } else {
          const created = await window.AdminApi.createProduct(payload);
          let newProduct = normalizeProduct(created);
          newProduct.category = categories.find((c) => c.id === newProduct.categoryId)?.name || newProduct.category;
          newProduct.brand = brands.find((b) => b.id === newProduct.brandId)?.name || newProduct.brand;
          if (uploadedProductFile && newProduct.id) {
            const uploadResult = await window.AdminApi.uploadProductImage(newProduct.id, uploadedProductFile);
            if (uploadResult?.url) {
              newProduct.image = uploadResult.url;
            }
          }
          setProducts((prev) => [newProduct, ...prev]);
          showToast('Đã tạo sản phẩm mới thành công!', 'success');
        }
      } else if (modalType === 'category') {
        const payload = {
          CategoryName: formData.get('name'),
          Description: formData.get('description') || ''
        };
        const created = await window.AdminApi.createCategory(payload);
        const normalized = normalizeCategory(created);
        setCategories((prev) => [...prev, normalized]);
        showToast('Đã thêm danh mục mới!', 'success');
      } else if (modalType === 'brand') {
        const payload = {
          BrandName: formData.get('name'),
          Description: formData.get('description') || ''
        };
        const created = await window.AdminApi.createBrand(payload);
        const normalized = normalizeBrand(created);
        setBrands((prev) => [...prev, normalized]);
        showToast('Đã liên kết thương hiệu mới!', 'success');
      } else if (modalType === 'user') {
        const roleValue = editingItem?.role === 'admin' ? 'admin' : formData.get('role');
        const payload = {
          FullName: formData.get('name'),
          Email: formData.get('email'),
          PhoneNumber: formData.get('phone') || '',
          Address: formData.get('address') || '',
          Role: roleValue === 'staff' ? 'Staff' : roleValue === 'admin' ? 'Admin' : 'User',
          Status: formData.get('status') === 'Hoạt động'
        };

        if (editingItem) {
          const response = await window.AdminApi.updateUser(editingItem.id, payload);
          const updatedUser = normalizeUser(response.user || response);
          setUsers((prev) => prev.map((u) => (u.id === editingItem.id ? updatedUser : u)));
          showToast('Cập nhật thành viên thành công!', 'success');
          if (editingItem.id === adminProfile.id) {
            setAdminProfile((prev) => ({ ...prev, nickname: updatedUser.name, email: updatedUser.email }));
          }
        } else {
          const response = await window.AdminApi.createUser(payload);
          const createdUser = normalizeUser(response.user || response);
          setUsers((prev) => [createdUser, ...prev]);
          showToast('Đã tạo mới tài khoản thành công!', 'success');
        }
      }

      setIsModalOpen(false);
      setEditingItem(null);
      setUploadedProductImageUrl('');
      setUploadedProductFile(null);
      setUploadedBrandLogoUrl('');
      setUploadedBrandLogoFile(null);
    } catch (error) {
      showToast('Lỗi khi lưu thông tin. Kiểm tra API hoặc quyền truy cập.', 'danger');
    }
  };

  const handleUpdateAdminProfile = (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const updatedNickname = formData.get('nickname');
    const updatedEmail = formData.get('email');

    setAdminProfile((prev) => {
      const updated = { ...prev, nickname: updatedNickname, email: updatedEmail };
      syncAdminWithUsersList(updated);
      return updated;
    });

    showToast('Đã lưu cập nhật Hồ Sơ Admin thành công!', 'success');
  };

  const handleNextOrderStatusStep = async (orderId, currentStatus) => {
    let nextStatus = '';
    if (currentStatus === 'Đang làm bánh') nextStatus = 'Đang giao hàng';
    else if (currentStatus === 'Đang giao hàng') nextStatus = 'Đã giao hàng';

    if (!nextStatus) return;

    try {
      await window.AdminApi.updateOrderStatus(orderId, { Status: nextStatus });
      setOrders((prev) => prev.map((o) => (o.id === orderId ? { ...o, status: nextStatus } : o)));
      showToast(`Đơn hàng ${orderId} chuyển tiếp thành công!`, 'success');
    } catch (error) {
      showToast('Lỗi khi cập nhật trạng thái đơn hàng.', 'danger');
    }
  };

  const handleCancelOrderStep = async (orderId) => {
    try {
      await window.AdminApi.cancelOrder(orderId);
      setOrders((prev) => prev.map((o) => (o.id === orderId ? { ...o, status: 'Đã hủy' } : o)));
      showToast(`Đơn hàng ${orderId} đã được hủy bỏ thành công.`, 'warning');
    } catch (error) {
      showToast('Lỗi khi hủy đơn hàng.', 'danger');
    }
  };

  const openVariantManager = (product) => {
    setSelectedProductForVariants(product);
    setIsVariantModalOpen(true);
    resetVariantForm();
  };

  const resetVariantForm = () => {
    setVariantFormMode('add');
    setEditingVariant(null);
    setVariantName('');
    setVariantPriceAdj(0);
    setVariantStock(10);
  };

  const handleEditVariantClick = (variant) => {
    setVariantFormMode('edit');
    setEditingVariant(variant);
    setVariantName(variant.name);
    setVariantPriceAdj(variant.priceAdjustment);
    setVariantStock(variant.stock || 0);
  };

  const handleSaveVariant = async (e) => {
    e.preventDefault();
    if (!variantName.trim()) {
      showToast('Vui lòng điền tên biến thể', 'warning');
      return;
    }

    const previousVariants = selectedProductForVariants.variants || [];
    let updatedVariants = [...previousVariants];

    if (variantFormMode === 'add') {
      updatedVariants.push({
        id: `V_${selectedProductForVariants.id}_${Date.now()}`,
        name: variantName,
        priceAdjustment: parseInt(variantPriceAdj, 10) || 0,
        stock: parseInt(variantStock, 10) || 0
      });
      showToast(`Đã thêm biến thể "${variantName}"`, 'success');
    } else {
      updatedVariants = updatedVariants.map((v) => (
        v.id === editingVariant.id ? { ...v, name: variantName, priceAdjustment: parseInt(variantPriceAdj, 10) || 0, stock: parseInt(variantStock, 10) || 0 } : v
      ));
      showToast(`Đã sửa biến thể thành "${variantName}"`, 'success');
    }

    const updatedProduct = { ...selectedProductForVariants, variants: updatedVariants };
    setProducts((prev) => prev.map((p) => (p.id === updatedProduct.id ? updatedProduct : p)));
    setSelectedProductForVariants(updatedProduct);

    try {
      await saveProductVariantMeta(updatedProduct);
    } catch (error) {
      showToast('Lỗi khi lưu biến thể vào API.', 'danger');
    }

    resetVariantForm();
  };

  const handleDeleteVariant = async (vId) => {
    const updatedVariants = (selectedProductForVariants.variants || []).filter((v) => v.id !== vId);
    const updatedProduct = { ...selectedProductForVariants, variants: updatedVariants };
    setProducts((prev) => prev.map((p) => (p.id === updatedProduct.id ? updatedProduct : p)));
    setSelectedProductForVariants(updatedProduct);

    try {
      await saveProductVariantMeta(updatedProduct);
      showToast('Đã xóa biến thể!', 'warning');
    } catch (error) {
      showToast('Lỗi khi xóa biến thể từ API.', 'danger');
    }

    resetVariantForm();
  };

  const filteredProducts = products.filter((p) => {
    const query = searchQuery.toLowerCase();
    const matchesSearch = p.name.toLowerCase().includes(query) || String(p.id).toLowerCase().includes(query);
    const matchesCategory = categoryFilter === 'Tất cả' || p.category === categoryFilter;
    return matchesSearch && matchesCategory;
  });

  const filteredOrders = orders.filter((o) => {
    const query = searchQuery.toLowerCase();
    const matchesSearch = o.customer.toLowerCase().includes(query) || String(o.id).toLowerCase().includes(query) || o.phone.toLowerCase().includes(query);
    const matchesStatus = orderStatusFilter === 'Tất cả' || o.status === orderStatusFilter;
    return matchesSearch && matchesStatus;
  });

  const filteredCategories = categories.filter((c) => c.name.toLowerCase().includes(searchQuery.toLowerCase()));
  const filteredBrands = brands.filter((b) => b.name.toLowerCase().includes(searchQuery.toLowerCase()) || b.origin.toLowerCase().includes(searchQuery.toLowerCase()));
  const filteredUsers = users.filter((u) => u.name.toLowerCase().includes(searchQuery.toLowerCase()) || u.email.toLowerCase().includes(searchQuery.toLowerCase()));

  const renderRolePill = (role) => {
    if (role === 'admin') return (<span className="inline-flex items-center gap-1 bg-red-100 text-red-800 border border-red-200 px-3 py-1 rounded-full text-xs font-bold"><i className="bi bi-shield-lock-fill text-xs"></i> Admin</span>);
    if (role === 'staff') return (<span className="inline-flex items-center gap-1 bg-amber-100 text-amber-900 border border-amber-200 px-3 py-1 rounded-full text-xs font-bold"><i className="bi bi-person-workspace text-xs"></i> Staff</span>);
    return (<span className="inline-flex items-center gap-1 bg-stone-100 text-stone-700 border border-stone-200 px-3 py-1 rounded-full text-xs font-bold"><i className="bi bi-person-fill text-xs"></i> User</span>);
  };

  const getBrandOptionValue = (brandId) => brandId || brands[0]?.id || 0;
  const getCategoryOptionValue = (categoryId) => categoryId || categories[0]?.id || 0;

  return (
    <div className="min-h-screen bg-stone-50 text-stone-800 font-sans flex flex-col md:flex-row antialiased">
      <div className="fixed inset-0 z-40 pointer-events-none">
        <div className="fixed top-5 right-5 z-50 flex flex-col gap-2 pointer-events-none">
          {toasts.map((toast) => (
            <div key={toast.id} className={`px-5 py-3 rounded-xl shadow-lg border text-white font-medium flex items-center gap-3 animate-bounce transition-all duration-300 ${toast.type === 'success' ? 'bg-amber-600 border-amber-500' : 'bg-red-600 border-red-500'}`}>
              <span>{toast.type === 'success' ? <i className="bi bi-check-circle-fill text-lg"></i> : <i className="bi bi-exclamation-triangle-fill text-lg"></i>}</span>
              <span>{toast.message}</span>
            </div>
          ))}
        </div>
      </div>

      <aside className="w-full md:w-64 bg-amber-950 text-amber-50 flex-shrink-0 flex flex-col justify-between shadow-xl border-r border-amber-900">
        <div>
          <div className="p-6 border-b border-amber-900/60 flex items-center gap-3">
            <div className="w-10 h-10 bg-amber-500 text-amber-950 rounded-full flex items-center justify-center text-xl shadow-md shadow-amber-500/20 transform hover:rotate-45 transition-transform duration-300">
              <i className="bi bi-cookie"></i>
            </div>
            <div>
              <h1 className="font-extrabold text-xl tracking-wider text-amber-400">OISHIPAN</h1>
              <span className="text-xs text-amber-300/80 uppercase font-semibold">Hệ thống quản trị</span>
            </div>
          </div>

          <nav className="p-4 space-y-1.5">
            {[
              { key: 'dashboard', icon: 'bi-grid', label: 'Tổng quan' },
              { key: 'products', icon: 'bi-book', label: 'Sản phẩm' },
              { key: 'categories', icon: 'bi-archive', label: 'Danh mục' },
              { key: 'brands', icon: 'bi-briefcase', label: 'Thương Hiệu' },
              { key: 'orders', icon: 'bi-bag', label: 'Đơn hàng' },
              { key: 'users', icon: 'bi-people', label: 'Người dùng' },
              { key: 'profile', icon: 'bi-person-circle', label: 'Hồ sơ Admin' }
            ].map((item) => (
              <button key={item.key} onClick={() => { setActiveTab(item.key); setSearchQuery(''); }} className={`w-full flex items-center gap-3.5 px-4 py-3 rounded-xl transition-all duration-200 text-left ${activeTab === item.key ? 'bg-amber-500 text-stone-900 font-bold shadow-lg shadow-amber-500/10' : 'hover:bg-amber-900/50 text-amber-200'}`}>
                <i className={`${item.icon} text-lg`}></i>
                <span>{item.label}</span>
              </button>
            ))}
          </nav>
        </div>

        <div onClick={() => { setActiveTab('profile'); setSearchQuery(''); }} className={`p-4 border-t border-amber-900/60 bg-amber-950/80 cursor-pointer hover:bg-amber-900/60 transition-colors group ${activeTab === 'profile' ? 'bg-amber-900 border-amber-500 border-t-2' : ''}`} title="Chỉnh sửa hồ sơ của bạn">
          <div className="flex items-center gap-3">
            <img src={adminProfile.avatar} alt="Admin Avatar" className="w-10 h-10 rounded-xl object-cover border-2 border-amber-500 group-hover:scale-105 transition-transform" />
            <div className="flex-1 min-w-0">
              <p className="font-bold text-sm text-amber-100 truncate">{adminProfile.nickname || adminProfile.name}</p>
              <p className="text-xs text-amber-400 font-medium flex items-center gap-1.5">
                <span>Chủ tiệm Oishipan</span>
                <i className="bi bi-gear-fill opacity-0 group-hover:opacity-100 transition-opacity text-[10px]"></i>
              </p>
            </div>
          </div>
        </div>
      </aside>

      <main className="flex-1 p-6 md:p-8 flex flex-col gap-6 overflow-y-auto max-w-7xl mx-auto w-full">
        <header className="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-stone-200 pb-5">
          <div>
            <h2 className="text-2xl md:text-3xl font-extrabold text-stone-800 tracking-tight">
              {activeTab === 'dashboard' && 'Bảng Thống Kê Tổng Quan'}
              {activeTab === 'products' && 'Quản Lý Danh Sách Sản Phẩm'}
              {activeTab === 'categories' && 'Quản Lý Danh Mục'}
              {activeTab === 'brands' && 'Quản Lý Thương Hiệu'}
              {activeTab === 'orders' && 'Lịch Sử Đơn Hàng'}
              {activeTab === 'users' && 'Hệ Thống Thành Viên & Nhân Sự'}
              {activeTab === 'profile' && 'Thiết Lập Hồ Sơ Cá Nhân'}
            </h2>
            <p className="text-sm text-stone-500 mt-1">Hôm nay: {new Date().toLocaleDateString('vi-VN', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}</p>
          </div>

          {activeTab !== 'dashboard' && activeTab !== 'profile' && (
            <div className="flex items-center gap-2">
              <div className="relative w-full md:w-64">
                <input type="text" placeholder="Tìm kiếm nhanh..." value={searchQuery} onChange={(e) => setSearchQuery(e.target.value)} className="w-full bg-white border border-stone-300 rounded-xl py-2.5 pl-10 pr-4 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent shadow-sm" />
                <i className="bi bi-search text-stone-400 absolute left-3.5 top-3 text-sm"></i>
              </div>
              {activeTab === 'products' && (<button onClick={() => openModal('product')} className="bg-amber-600 hover:bg-amber-700 text-white font-semibold text-sm px-4.5 py-2.5 rounded-xl flex items-center gap-2 shadow-lg shadow-amber-600/10 transition-all duration-200 whitespace-nowrap"><i className="bi bi-plus-lg"></i> Thêm Sản Phẩm</button>)}
              {activeTab === 'categories' && (<button onClick={() => openModal('category')} className="bg-amber-600 hover:bg-amber-700 text-white font-semibold text-sm px-4.5 py-2.5 rounded-xl flex items-center gap-2 shadow-lg shadow-amber-600/10 transition-all duration-200 whitespace-nowrap"><i className="bi bi-plus-lg"></i> Thêm Danh Mục</button>)}
              {activeTab === 'brands' && (<button onClick={() => openModal('brand')} className="bg-amber-600 hover:bg-amber-700 text-white font-semibold text-sm px-4.5 py-2.5 rounded-xl flex items-center gap-2 shadow-lg shadow-amber-600/10 transition-all duration-200 whitespace-nowrap"><i className="bi bi-plus-lg"></i> Thêm Thương Hiệu</button>)}
              {activeTab === 'users' && (<button onClick={() => openModal('user')} className="bg-amber-600 hover:bg-amber-700 text-white font-semibold text-sm px-4.5 py-2.5 rounded-xl flex items-center gap-2 shadow-lg shadow-amber-600/10 transition-all duration-200 whitespace-nowrap"><i className="bi bi-plus-lg"></i> Thêm Người Dùng</button>)}
            </div>
          )}
        </header>

        {isLoading ? (
          <div className="rounded-3xl border border-stone-200 bg-white p-12 text-center text-stone-500 shadow-sm">
            <i className="bi bi-hourglass-split text-4xl animate-pulse"></i>
            <p className="mt-4 text-sm">Đang tải dữ liệu quản trị từ API...</p>
          </div>
        ) : errorMessage ? (
          <div className="rounded-3xl border border-red-200 bg-red-50 p-6 text-red-700">
            <p className="font-bold">Lỗi tải dữ liệu</p>
            <p className="text-sm mt-2">{errorMessage}</p>
          </div>
        ) : (
          <>
            {/* Dashboard and other tabs... */}
            {activeTab === 'dashboard' && (
              <div className="space-y-6">
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
                  <div className="bg-white p-6 rounded-2xl border border-stone-200 shadow-sm flex items-center justify-between relative overflow-hidden group">
                    <div className="absolute top-0 right-0 w-24 h-24 bg-amber-500/5 rounded-full -mr-8 -mt-8 group-hover:scale-110 transition-transform duration-300"></div>
                    <div>
                      <span className="text-stone-400 text-xs font-bold uppercase tracking-wider">Doanh thu lò bánh</span>
                      <h3 className="text-2xl font-black text-stone-800 mt-2">{stats.totalRev.toLocaleString('vi-VN')} đ</h3>
                      <p className="text-xs text-emerald-600 font-semibold mt-1 flex items-center gap-1"><span>↑ 12.5%</span> <span className="text-stone-400">so với hôm qua</span></p>
                    </div>
                    <div className="w-12 h-12 bg-amber-50 rounded-xl flex items-center justify-center text-xl shadow-inner text-amber-600"><i className="bi bi-cash-coin"></i></div>
                  </div>
                  <div className="bg-white p-6 rounded-2xl border border-stone-200 shadow-sm flex items-center justify-between relative overflow-hidden group">
                    <div className="absolute top-0 right-0 w-24 h-24 bg-amber-500/5 rounded-full -mr-8 -mt-8 group-hover:scale-110 transition-transform duration-300"></div>
                    <div>
                      <span className="text-stone-400 text-xs font-bold uppercase tracking-wider">Bánh đang mở bán</span>
                      <h3 className="text-2xl font-black text-stone-800 mt-2">{stats.activeProducts} món</h3>
                      <p className="text-xs text-stone-500 mt-1">Đủ cung ứng mọi khung giờ</p>
                    </div>
                    <div className="w-12 h-12 bg-amber-50 rounded-xl flex items-center justify-center text-xl shadow-inner text-amber-600"><i className="bi bi-egg-fried"></i></div>
                  </div>
                  <div className="bg-white p-6 rounded-2xl border border-stone-200 shadow-sm flex items-center justify-between relative overflow-hidden group">
                    <div className="absolute top-0 right-0 w-24 h-24 bg-amber-500/5 rounded-full -mr-8 -mt-8 group-hover:scale-110 transition-transform duration-300"></div>
                    <div>
                      <span className="text-stone-400 text-xs font-bold uppercase tracking-wider">Lượng đơn hàng</span>
                      <h3 className="text-2xl font-black text-stone-800 mt-2">{stats.totalOrders} đơn</h3>
                      <p className="text-xs text-amber-600 font-semibold mt-1">{orders.filter((o) => o.status === 'Đang làm bánh').length} đơn đang nhào bột nướng</p>
                    </div>
                    <div className="w-12 h-12 bg-amber-50 rounded-xl flex items-center justify-center text-xl shadow-inner text-amber-600"><i className="bi bi-box-seam-fill"></i></div>
                  </div>
                  <div className="bg-white p-6 rounded-2xl border border-stone-200 shadow-sm flex items-center justify-between relative overflow-hidden group">
                    <div className="absolute top-0 right-0 w-24 h-24 bg-amber-500/5 rounded-full -mr-8 -mt-8 group-hover:scale-110 transition-transform duration-300"></div>
                    <div>
                      <span className="text-stone-400 text-xs font-bold uppercase tracking-wider">Khách hàng VIP</span>
                      <h3 className="text-2xl font-black text-stone-800 mt-2">{stats.activeCust} tài khoản</h3>
                      <p className="text-xs text-emerald-600 font-semibold mt-1">↑ 4 khách mới tuần này</p>
                    </div>
                    <div className="w-12 h-12 bg-amber-50 rounded-xl flex items-center justify-center text-xl shadow-inner text-amber-600"><i className="bi bi-person-vcard-fill"></i></div>
                  </div>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                  <div className="bg-white p-6 rounded-2xl border border-stone-200 shadow-sm lg:col-span-2 flex flex-col justify-between">
                    <div>
                      <div className="flex items-center justify-between mb-4">
                        <h4 className="font-bold text-stone-800 flex items-center gap-2 text-lg"><i className="bi bi-bar-chart-line-fill text-amber-600"></i> Hiệu Suất Tiêu Thụ Bánh Theo Giờ</h4>
                        <span className="text-xs bg-amber-100 text-amber-800 px-3 py-1 rounded-full font-bold">Thời gian thực</span>
                      </div>
                      <div className="h-48 flex items-end justify-between gap-2 pt-6 border-b border-stone-100">
                        {['35%', '90%', '55%', '80%', '25%'].map((value, index) => (
                          <div key={index} className="w-full flex flex-col items-center gap-2">
                            <div className={`bg-amber-${index % 2 === 0 ? '300' : '500'} w-full rounded-t-lg transition-all duration-500 hover:bg-amber-600`} style={{ height: value }}></div>
                            <span className={`text-xs ${index % 2 === 0 ? 'text-stone-400' : 'text-stone-500 font-bold'}`}>{['06:00 - 09:00', '09:00 - 12:00', '12:00 - 15:00', '15:00 - 18:00', '18:00 - 21:00'][index]}</span>
                          </div>
                        ))}
                      </div>
                    </div>
                    <div className="grid grid-cols-3 gap-4 pt-4 text-center">
                      <div className="border-r border-stone-100"><p className="text-xs text-stone-400">Khung giờ vàng</p><p className="font-bold text-amber-600">09h - 11h Sáng</p></div>
                      <div className="border-r border-stone-100"><p className="text-xs text-stone-400">Sản phẩm Best-Seller</p><p className="font-bold text-amber-600">Bánh mì Ôi-Shi</p></div>
                      <div><p className="text-xs text-stone-400">Trung bình đơn</p><p className="font-bold text-amber-600">110.000 đ</p></div>
                    </div>
                  </div>
                  <div className="bg-white p-6 rounded-2xl border border-stone-200 shadow-sm flex flex-col justify-between">
                    <div>
                      <h4 className="font-bold text-stone-800 text-lg mb-4 flex items-center gap-2"><i className="bi bi-fire text-orange-500"></i> Trạng Thái Hoạt Động Bếp</h4>
                      <div className="space-y-4">
                        {[
                          { label: 'Đã nướng xong & lên kệ', value: '80%', color: 'bg-amber-500' },
                          { label: 'Ủ bột & lên men bánh mì Âu', value: '55%', color: 'bg-orange-500' },
                          { label: 'Đóng gói & Chờ shipper', value: '95%', color: 'bg-emerald-500' }
                        ].map((item) => (
                          <div key={item.label}>
                            <div className="flex justify-between text-xs font-semibold text-stone-600 mb-1.5"><span>{item.label}</span><span>{item.value}</span></div>
                            <div className="w-full bg-stone-100 h-2 rounded-full overflow-hidden"><div className={`${item.color} h-full rounded-full`} style={{ width: item.value }}></div></div>
                          </div>
                        ))}
                      </div>
                    </div>
                    <div className="bg-amber-50/50 border border-amber-100 rounded-xl p-4 mt-4">
                      <p className="text-xs text-amber-900 leading-relaxed font-medium">🔥 <strong className="font-bold">Lưu ý Bếp Trưởng:</strong> Hôm nay nhu cầu đặt "Bánh mì Ôi-Shi" tăng đột biến, vui lòng kiểm tra lượng nhân xá xíu trong kho lạnh để kịp thời bổ sung lúc 16h.</p>
                    </div>
                  </div>
                </div>
                <div className="bg-white rounded-2xl border border-stone-200 shadow-sm overflow-hidden">
                  <div className="p-6 border-b border-stone-100 flex items-center justify-between">
                    <h4 className="font-bold text-stone-800 flex items-center gap-2 text-lg"><i className="bi bi-bell-fill text-amber-500"></i> Đơn Hàng Mới Nhận Hôm Nay</h4>
                    <button onClick={() => { setActiveTab('orders'); setSearchQuery(''); }} className="text-amber-600 hover:text-amber-700 font-bold text-sm flex items-center gap-1">Xem toàn bộ danh sách <i className="bi bi-arrow-right-short"></i></button>
                  </div>
                  <div className="overflow-x-auto">
                    <table className="w-full text-left border-collapse">
                      <thead><tr className="bg-stone-50 border-b border-stone-100 text-xs text-stone-400 font-bold uppercase"><th className="p-4">Mã đơn</th><th className="p-4">Khách hàng</th><th className="p-4">Nội dung bánh đặt</th><th className="p-4 text-right">Tổng tiền</th><th className="p-4 text-center">Trạng thái</th></tr></thead>
                      <tbody className="divide-y divide-stone-100 text-sm">
                        {orders.slice(0, 3).map((order) => (
                          <tr key={order.id} className="hover:bg-amber-50/20 transition-colors">
                            <td className="p-4 font-black text-stone-700">{order.id}</td>
                            <td className="p-4"><p className="font-semibold text-stone-800">{order.customer}</p><p className="text-xs text-stone-400">{order.phone}</p></td>
                            <td className="p-4 max-w-xs truncate text-stone-600">{order.items.map((item) => `${item.name} (x${item.qty})`).join(', ')}</td>
                            <td className="p-4 text-right font-bold text-stone-900">{order.total.toLocaleString('vi-VN')} đ</td>
                            <td className="p-4 text-center"><span className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${order.status === 'Đã giao hàng' ? 'bg-emerald-50 text-emerald-700 border border-emerald-200' : order.status === 'Đang giao hàng' ? 'bg-blue-50 text-blue-700 border border-blue-200' : order.status === 'Đang làm bánh' ? 'bg-amber-50 text-amber-700 border border-amber-200' : 'bg-red-50 text-red-700 border border-red-200'}`}>{order.status}</span></td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            )}

            {activeTab === 'products' && (
              <div className="space-y-6">
                <div className="flex flex-wrap items-center gap-2 border-b border-stone-200 pb-4"><span className="text-xs text-stone-400 font-bold uppercase mr-2">Phân loại bánh:</span>{['Tất cả', ...categories.map((c) => c.name)].map((cat) => (<button key={cat} onClick={() => setCategoryFilter(cat)} className={`px-4 py-1.5 rounded-full text-xs font-semibold transition-all duration-150 ${categoryFilter === cat ? 'bg-amber-600 text-white shadow' : 'bg-white hover:bg-stone-100 text-stone-600 border border-stone-200'}`}>{cat}</button>))}</div>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                  {filteredProducts.map((p) => (<div key={p.id} className="bg-white rounded-2xl border border-stone-200 shadow-sm overflow-hidden flex flex-col justify-between group hover:shadow-md transition-all duration-200">
                    <div className="relative h-44 overflow-hidden bg-stone-100"><img src={p.image} alt={p.name} className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" onError={(e) => { e.target.src = IMAGE_PLACEHOLDER.banhMiThit; }} /><div className="absolute top-3 left-3 bg-white/90 backdrop-blur-sm px-2.5 py-1 rounded-lg text-xs font-black text-amber-900 border border-amber-100">{p.id}</div><div className="absolute top-3 right-3"><span className={`inline-block px-2.5 py-1 rounded-lg text-xs font-extrabold ${p.status === 'Còn hàng' ? 'bg-emerald-500 text-white shadow-sm' : 'bg-stone-500 text-white'}`}>{p.status}</span></div></div>
                    <div className="p-5 flex-1 flex flex-col justify-between gap-4"><div><div className="flex items-center gap-2 mb-1"><span className="text-xs bg-amber-50 text-amber-800 font-bold px-2 py-0.5 rounded">{p.category}</span><span className="text-xs text-stone-400">•</span><span className="text-xs text-stone-500 font-medium">{p.brand}</span></div><h3 className="font-bold text-stone-800 text-lg leading-snug hover:text-amber-600 transition-colors">{p.name}</h3><div className="flex items-baseline gap-2 mt-3"><span className="text-xl font-black text-amber-600">{p.price.toLocaleString('vi-VN')} đ</span><span className="text-xs text-stone-400">/ giá gốc</span></div><p className="text-xs text-stone-400 mt-1">Còn lại trong tủ kính: <span className="font-bold text-stone-700">{p.stock} bánh</span></p></div><div className="border-t border-stone-100 pt-3"><div className="flex justify-between items-center mb-1.5"><span className="text-xs text-stone-400 font-bold uppercase tracking-wider">Các biến thể ({p.variants?.length || 0}):</span><button onClick={() => openVariantManager(p)} className="text-xs text-amber-600 hover:text-amber-800 font-extrabold flex items-center gap-1 transition-colors"><i className="bi bi-gear-fill"></i> Cấu hình biến thể</button></div>{p.variants && p.variants.length > 0 ? (<div className="flex flex-wrap gap-1.5 max-h-24 overflow-y-auto">{p.variants.map((v) => (<span key={v.id} className="text-xs bg-stone-50 text-stone-600 border border-stone-200/60 rounded px-2 py-1 flex items-center gap-1"><span className="font-semibold">{v.name}</span><span className="text-amber-700 font-medium">({v.priceAdjustment >= 0 ? `+${v.priceAdjustment.toLocaleString('vi-VN')}đ` : `${v.priceAdjustment.toLocaleString('vi-VN')}đ`})</span></span>))}</div>) : (<p className="text-xs text-stone-400 italic">Chưa cấu hình biến thể nào cho sản phẩm này.</p>)}</div><div className="border-t border-stone-100 pt-3 flex items-center justify-between gap-2 mt-auto"><button onClick={() => openModal('product', p)} className="flex-1 bg-stone-100 hover:bg-amber-500 hover:text-stone-900 text-stone-700 font-semibold text-xs py-2 rounded-xl transition-all duration-150 flex items-center justify-center gap-1.5"><i className="bi bi-pencil-square"></i> Chỉnh Sửa</button><button onClick={() => handleDelete(p.id, 'product')} className="bg-red-50 hover:bg-red-100 text-red-600 p-2 rounded-xl transition-all duration-150 border border-red-100" title="Xóa sản phẩm"><i className="bi bi-trash3-fill"></i></button></div></div></div>))}
                </div>
                {filteredProducts.length === 0 && (<div className="text-center py-12 bg-white rounded-2xl border border-stone-200"><span className="text-4xl"><i className="bi bi-exclamation-circle text-stone-400"></i></span><p className="text-stone-500 font-bold mt-2">Không tìm thấy chiếc bánh nào khớp với yêu cầu!</p></div>)}
              </div>
            )}

            {activeTab === 'categories' && (
              <div className="bg-white rounded-2xl border border-stone-200 shadow-sm overflow-hidden">
                <div className="overflow-x-auto">
                  <table className="w-full text-left border-collapse"><thead><tr className="bg-stone-50 border-b border-stone-100 text-xs text-stone-400 font-bold uppercase"><th className="p-4 pl-6">Mã DM</th><th className="p-4">Tên danh mục</th><th className="p-4">Đường dẫn tĩnh (Slug)</th><th className="p-4">Mô tả chi tiết</th><th className="p-4 text-center">Số loại bánh</th><th className="p-4 text-right pr-6">Hành động</th></tr></thead><tbody className="divide-y divide-stone-100 text-sm">{filteredCategories.map((c) => (<tr key={c.id} className="hover:bg-amber-50/10 transition-colors"><td className="p-4 pl-6 font-black text-stone-700">{c.id}</td><td className="p-4 font-bold text-stone-800">{c.name}</td><td className="p-4 text-stone-400 font-mono text-xs">/{c.slug}</td><td className="p-4 max-w-sm text-stone-500">{c.description}</td><td className="p-4 text-center"><span className="bg-amber-50 text-amber-800 font-extrabold px-3 py-1 rounded-full text-xs border border-amber-100">{c.count} món bánh</span></td><td className="p-4 text-right pr-6 space-x-1"><button onClick={() => openModal('category', c)} className="bg-stone-100 hover:bg-amber-500 hover:text-stone-900 text-stone-700 font-semibold text-xs px-3 py-1.5 rounded-lg transition-colors"><i className="bi bi-pencil-fill"></i> Sửa</button><button onClick={() => handleDelete(c.id, 'category')} className="bg-red-50 hover:bg-red-100 text-red-600 text-xs px-3 py-1.5 rounded-lg transition-colors"><i className="bi bi-trash3-fill"></i> Xóa</button></td></tr>))}</tbody></table>
                </div>
              </div>
            )}

            {activeTab === 'brands' && (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">{filteredBrands.map((b) => (<div key={b.id} className="bg-white rounded-2xl border border-stone-200 p-6 shadow-sm flex flex-col justify-between relative group hover:shadow-md transition-all duration-200"><div><div className="flex items-center justify-between mb-4"><div className="w-16 h-16 bg-stone-100 rounded-xl overflow-hidden flex items-center justify-center border border-stone-200 shadow-inner"><img src={b.logo} alt={b.name} className="w-full h-full object-cover" onError={(e) => { e.target.src = IMAGE_PLACEHOLDER.defaultLogo; }} /></div><span className="text-xs bg-amber-50 text-amber-800 border border-amber-100 font-black px-2.5 py-1 rounded-lg">{b.id}</span></div><h3 className="font-extrabold text-stone-800 text-lg group-hover:text-amber-600 transition-colors">{b.name}</h3><p className="text-xs text-amber-700 font-bold mt-1">📍 Xuất xứ: {b.origin}</p><p className="text-sm text-stone-500 mt-3 leading-relaxed">{b.description}</p></div><div className="border-t border-stone-100 pt-4 mt-6 flex items-center justify-end gap-2"><button onClick={() => openModal('brand', b)} className="bg-stone-100 hover:bg-amber-500 hover:text-stone-900 text-stone-700 font-semibold text-xs px-3 py-1.5 rounded-lg transition-colors"><i className="bi bi-gear-fill"></i> Cấu hình</button><button onClick={() => handleDelete(b.id, 'brand')} className="bg-red-50 hover:bg-red-100 text-red-600 text-xs px-3 py-1.5 rounded-lg transition-colors"><i className="bi bi-trash3-fill"></i> Gỡ</button></div></div>))}</div>)}

            {activeTab === 'orders' && (
              <div className="space-y-6"><div className="flex flex-wrap items-center gap-2 border-b border-stone-200 pb-4"><span className="text-xs text-stone-400 font-bold uppercase mr-2">Trạng thái bếp & ship:</span>{['Tất cả', 'Đang làm bánh', 'Đang giao hàng', 'Đã giao hàng', 'Đã hủy'].map((status) => (<button key={status} onClick={() => setOrderStatusFilter(status)} className={`px-4 py-1.5 rounded-full text-xs font-semibold transition-all duration-150 ${orderStatusFilter === status ? 'bg-amber-600 text-white shadow' : 'bg-white hover:bg-stone-100 text-stone-600 border border-stone-200'}`}>{status}</button>))}</div><div className="bg-white rounded-2xl border border-stone-200 shadow-sm overflow-hidden"><div className="overflow-x-auto"><table className="w-full text-left border-collapse"><thead><tr className="bg-stone-50 border-b border-stone-100 text-xs text-stone-400 font-bold uppercase"><th className="p-4 pl-6">Mã đơn</th><th className="p-4">Ngày mua</th><th className="p-4">Khách hàng</th><th className="p-4">Giỏ hàng</th><th className="p-4 text-right">Tổng tiền</th><th className="p-4 text-center">Trạng thái hiện tại</th><th className="p-4 text-center pr-6">Xử lý nhanh một chiều (Không quay lại)</th></tr></thead><tbody className="divide-y divide-stone-100 text-sm">{filteredOrders.map((o) => (<tr key={o.id} className="hover:bg-amber-50/10 transition-colors"><td className="p-4 pl-6 font-black text-stone-700">{o.id}</td><td className="p-4 text-stone-500">{o.date}</td><td className="p-4"><p className="font-bold text-stone-800">{o.customer}</p><p className="text-xs text-stone-400 font-mono">{o.phone}</p></td><td className="p-4 max-w-xs font-medium">{o.items.map((item, idx) => (<div key={idx} className="text-stone-600 text-xs">🥖 {item.name} <span className="text-stone-400 font-bold">x{item.qty}</span></div>))}</td><td className="p-4 text-right font-black text-stone-800">{o.total.toLocaleString('vi-VN')} đ</td><td className="p-4 text-center"><span className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${o.status === 'Đã giao hàng' ? 'bg-emerald-50 text-emerald-700 border border-emerald-200' : o.status === 'Đang giao hàng' ? 'bg-blue-50 text-blue-700 border border-blue-200' : o.status === 'Đang làm bánh' ? 'bg-amber-50 text-amber-700 border border-amber-200' : 'bg-red-50 text-red-700 border border-red-200'}`}>{o.status}</span></td><td className="p-4 text-center pr-6"><div className="flex items-center justify-center gap-2">{o.status === 'Đang làm bánh' && (<><button onClick={() => handleNextOrderStatusStep(o.id, o.status)} className="bg-amber-500 hover:bg-amber-600 text-stone-950 text-xs font-bold px-3 py-1.5 rounded-lg shadow-sm transition-all flex items-center gap-1.5"><i className="bi bi-bicycle"></i> Giao hàng</button><button onClick={() => handleCancelOrderStep(o.id)} className="bg-stone-100 hover:bg-red-50 text-red-600 hover:text-red-700 text-xs font-bold px-2.5 py-1.5 rounded-lg border border-stone-200 transition-all flex items-center gap-1"><i className="bi bi-x-circle"></i> Hủy đơn</button></>)}{o.status === 'Đang giao hàng' && (<button onClick={() => handleNextOrderStatusStep(o.id, o.status)} className="bg-emerald-600 hover:bg-emerald-700 text-white text-xs font-bold px-3 py-1.5 rounded-lg shadow-sm transition-all flex items-center gap-1.5"><i className="bi bi-check-circle"></i> Hoàn thành</button>)}{o.status === 'Đã giao hàng' && (<span className="text-xs text-emerald-600 font-bold flex items-center gap-1"><i className="bi bi-check-all text-sm"></i> Thành công (Đóng lưu trữ)</span>)}{o.status === 'Đã hủy' && (<span className="text-xs text-stone-400 italic"><i className="bi bi-trash"></i> Đơn hàng đã hủy</span>)}</div></td></tr>))}</tbody></table></div></div></div>)}

            {activeTab === 'users' && (
              <div className="bg-white rounded-2xl border border-stone-200 shadow-sm overflow-hidden"><div className="overflow-x-auto"><table className="w-full text-left border-collapse"><thead><tr className="bg-stone-50 border-b border-stone-100 text-xs text-stone-400 font-bold uppercase"><th className="p-4 pl-6">Thành viên</th><th className="p-4">Email liên hệ</th><th className="p-4">Vai trò hệ thống</th><th className="p-4 text-center">Trạng thái khóa</th><th className="p-4 text-right pr-6">Thao tác</th></tr></thead><tbody className="divide-y divide-stone-100 text-sm">{filteredUsers.map((u) => (<tr key={u.id} className="hover:bg-amber-50/10 transition-colors"><td className="p-4 pl-6 flex items-center gap-3"><img src={u.id === adminProfile.id ? adminProfile.avatar : u.avatar} alt={u.name} className="w-10 h-10 rounded-full object-cover border border-stone-200" /><div><p className="font-bold text-stone-800">{u.id === adminProfile.id ? (adminProfile.nickname || adminProfile.name) : u.name}</p><p className="text-xs text-stone-400">Mã ID: {u.id}</p></div></td><td className="p-4 text-stone-600 font-mono text-xs">{u.id === adminProfile.id ? adminProfile.email : u.email}</td><td className="p-4">{renderRolePill(u.role)}</td><td className="p-4 text-center"><span className={`inline-block w-2.5 h-2.5 rounded-full ${u.status === 'Hoạt động' ? 'bg-emerald-500' : 'bg-red-500'}`} title={u.status}></span><span className="text-xs text-stone-500 ml-1.5">{u.status}</span></td><td className="p-4 text-right pr-6 space-x-1"><button onClick={() => openModal('user', u)} className="bg-stone-100 hover:bg-amber-500 hover:text-stone-900 text-stone-700 font-semibold text-xs px-3 py-1.5 rounded-lg transition-colors"><i className="bi bi-pencil-square"></i> Sửa tài khoản</button>{u.role !== 'admin' && (<button onClick={() => handleDelete(u.id, 'user')} className="bg-red-50 hover:bg-red-100 text-red-600 text-xs px-3 py-1.5 rounded-lg transition-colors"><i className="bi bi-trash3-fill"></i> Xóa</button>)}</td></tr>))}</tbody></table></div></div>)}

            {activeTab === 'profile' && (
              <div className="grid grid-cols-1 lg:grid-cols-3 gap-8"><div className="bg-white rounded-3xl border border-stone-200 shadow-sm p-6 text-center flex flex-col items-center justify-between relative overflow-hidden"><div className="absolute top-0 left-0 w-full h-24 bg-gradient-to-r from-amber-800 to-amber-950"></div><div className="relative mt-10 z-10"><div className="w-32 h-32 rounded-full border-4 border-white overflow-hidden shadow-md bg-stone-100"><img src={adminProfile.avatar} alt="Current Admin" className="w-full h-full object-cover" /></div><label className="absolute bottom-1 right-1 bg-amber-500 hover:bg-amber-600 text-stone-950 w-9 h-9 rounded-full flex items-center justify-center cursor-pointer shadow-lg border-2 border-white transition-all transform hover:scale-110"><i className="bi bi-camera-fill text-sm"></i><input type="file" accept="image/*" onChange={handleAdminAvatarChange} className="hidden" /></label></div><div className="mt-4"><h3 className="text-xl font-extrabold text-stone-800">{adminProfile.nickname || adminProfile.name}</h3><span className="inline-flex items-center gap-1 mt-1 bg-red-100 text-red-800 px-3 py-0.5 rounded-full text-xs font-black uppercase tracking-wider"><i className="bi bi-shield-lock-fill"></i> {adminProfile.role}</span><p className="text-xs text-stone-400 mt-2 font-mono">{adminProfile.email}</p></div><div className="w-full grid grid-cols-2 gap-3 border-t border-stone-100 pt-5 mt-6"><div className="bg-amber-50/50 p-3 rounded-xl border border-amber-100/50 text-left"><span className="text-[10px] font-bold text-stone-400 uppercase">Ngày gia nhập</span><p className="text-sm font-extrabold text-amber-900 mt-0.5">{adminProfile.joinDate}</p></div><div className="bg-amber-50/50 p-3 rounded-xl border border-amber-100/50 text-left"><span className="text-[10px] font-bold text-stone-400 uppercase">Hành động quản trị</span><p className="text-sm font-extrabold text-amber-900 mt-0.5">{adminProfile.actionsCount} lần</p></div></div><div className="w-full bg-stone-50 border border-stone-100 rounded-2xl p-4 mt-5 text-left text-xs text-stone-500 leading-relaxed"><i className="bi bi-info-circle-fill text-amber-700"></i> <strong className="font-bold text-stone-700">Lưu ý bảo mật:</strong> Quyền quản trị tối cao (Admin) của tiệm bánh Oishipan không thể tự chuyển nhượng hay hạ cấp để bảo vệ an ninh tuyệt đối cho chuỗi cửa hàng.</div></div><div className="lg:col-span-2 bg-white rounded-3xl border border-stone-200 shadow-sm p-6 md:p-8"><h3 className="text-lg font-extrabold text-stone-800 mb-6 pb-2 border-b border-stone-100 flex items-center gap-2"><i className="bi bi-person-fill-gear text-amber-600"></i> Thông tin cá nhân & Thiết lập biệt danh</h3><form onSubmit={handleUpdateAdminProfile} className="space-y-5"><div className="grid grid-cols-1 md:grid-cols-2 gap-4"><div><label className="block text-xs font-bold text-stone-400 uppercase mb-1.5">Mã số Quản trị viên</label><input disabled type="text" value={adminProfile.id} className="w-full bg-stone-100 border border-stone-200 text-stone-400 rounded-xl px-4 py-2.5 text-sm cursor-not-allowed" /></div><div><label className="block text-xs font-bold text-stone-400 uppercase mb-1.5">Tên gốc trên căn cước</label><input disabled type="text" value={adminProfile.name} className="w-full bg-stone-100 border border-stone-200 text-stone-400 rounded-xl px-4 py-2.5 text-sm cursor-not-allowed" /></div></div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1.5">Biệt danh hiển thị trên lò bánh *</label><input required type="text" name="nickname" defaultValue={adminProfile.nickname} placeholder="Ví dụ: Tâm Bánh Mì, Tâm Oishipan..." className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-3 text-sm font-semibold text-stone-800 focus:outline-none focus:ring-2 focus:ring-amber-500" /><p className="text-[10px] text-stone-400 mt-1">Biệt danh này sẽ xuất hiện trên bảng xếp hạng người dùng, góc bên trái thanh sidebar và các lịch sử thao tác nướng bánh.</p></div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1.5">Địa chỉ Email liên hệ *</label><input required type="email" name="email" defaultValue={adminProfile.email} placeholder="minhtam.admin@oishipan.vn" className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div><div className="border-t border-stone-100 pt-5 flex items-center justify-end gap-3"><button type="button" onClick={() => { setActiveTab('dashboard'); }} className="bg-stone-100 hover:bg-stone-200 text-stone-700 font-bold text-sm px-6 py-2.5 rounded-xl transition-all">Quay lại Dashboard</button><button type="submit" className="bg-amber-600 hover:bg-amber-700 text-white font-bold text-sm px-6 py-2.5 rounded-xl shadow-lg shadow-amber-600/10 transition-all"><i className="bi bi-floppy2-fill mr-1"></i> Lưu cập nhật hồ sơ</button></div></form></div></div>)}
          </>
        )}
      </main>

      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-stone-900/60 backdrop-blur-xs">
          <div className="bg-white rounded-3xl shadow-2xl border border-stone-200 w-full max-w-lg overflow-hidden transform transition-all duration-300">
            <div className="bg-amber-950 text-amber-100 p-6 flex justify-between items-center">
              <div><h3 className="font-extrabold text-lg">{editingItem ? `✍️ Cập nhật ${modalType === 'product' ? 'sản phẩm' : modalType === 'category' ? 'danh mục' : modalType === 'brand' ? 'thương hiệu' : 'người dùng'}` : modalType === 'product' ? '🥖 Tạo sản phẩm mới' : modalType === 'brand' ? '🍞 Tạo thương hiệu mới' : `🥐 Tạo ${modalType === 'category' ? 'danh mục mới' : 'người dùng mới'}`}</h3><span className="text-xs text-amber-400 font-medium">Bảng quản trị Oishipan</span></div><button onClick={() => { setIsModalOpen(false); setEditingItem(null); setUploadedProductImageUrl(''); setUploadedProductFile(null); setUploadedBrandLogoUrl(''); setUploadedBrandLogoFile(null); }} className="w-8 h-8 rounded-full bg-amber-900 text-amber-200 hover:bg-amber-800 flex items-center justify-center text-lg font-bold">✕</button></div>
            <form onSubmit={handleSaveItem} className="p-6 space-y-4 max-h-[80vh] overflow-y-auto">
              {modalType === 'product' && (<><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Tên Bánh / Thức Uống *</label><input required type="text" name="name" defaultValue={editingItem?.name || ''} placeholder="Bánh Mì Ôi-Shi Giòn Rụm" className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div><div className="grid grid-cols-2 gap-4"><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Giá Gốc (VNĐ) *</label><input required type="number" name="price" defaultValue={editingItem?.price || ''} placeholder="35000" className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Số Lượng Tổng *</label><input required type="number" name="stock" defaultValue={editingItem?.stock ?? 10} placeholder="50" className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div></div><div className="grid grid-cols-2 gap-4"><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Phân hệ danh mục</label><select name="category" defaultValue={getCategoryOptionValue(editingItem?.categoryId)} className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500">{categories.map((c) => (<option key={c.id} value={c.id}>{c.name}</option>))}</select></div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Thương hiệu bơ/bột</label><select name="brand" defaultValue={getBrandOptionValue(editingItem?.brandId)} className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500">{brands.map((b) => (<option key={b.id} value={b.id}>{b.name}</option>))}</select></div></div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Hình ảnh sản phẩm *</label><div className="flex items-center gap-4 p-3 bg-stone-50 border border-dashed border-stone-300 rounded-xl"><div className="w-16 h-16 rounded-lg bg-stone-200 overflow-hidden flex-shrink-0 flex items-center justify-center">{uploadedProductImageUrl ? (<img src={uploadedProductImageUrl} alt="Xem trước" className="w-full h-full object-cover" />) : (<span className="text-2xl text-stone-400"><i className="bi bi-image"></i></span>)}</div><div className="flex-1"><input type="file" accept="image/*" onChange={handleProductImageChange} className="text-xs text-stone-500 file:mr-3 file:py-1.5 file:px-3 file:rounded-lg file:border-0 file:text-xs file:font-bold file:bg-amber-100 file:text-amber-800 hover:file:bg-amber-200 cursor-pointer" /><p className="text-[10px] text-stone-400 mt-1">Hỗ trợ JPG, PNG, WEBP...</p></div></div></div></>) }
              {modalType === 'category' && (<><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Tên Danh Mục Bánh *</label><input required type="text" name="name" defaultValue={editingItem?.name || ''} placeholder="Bánh Mì Thơm Ngậy" className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Mô Tả Tổng Quan</label><textarea name="description" defaultValue={editingItem?.description || ''} placeholder="Nhóm sản phẩm bánh thích hợp làm quà tặng hoặc ăn nhẹ tiện lợi..." className="w-full h-24 bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div></>) }
              {modalType === 'brand' && (<><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Tên Thương Hiệu *</label><input required type="text" name="name" defaultValue={editingItem?.name || ''} placeholder="Anchor Butter" className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Quốc gia xuất xứ *</label><input required type="text" name="origin" defaultValue={editingItem?.origin || 'Việt Nam'} placeholder="New Zealand" className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" disabled /></div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Hình ảnh Logo Thương hiệu *</label><div className="flex items-center gap-4 p-3 bg-stone-50 border border-dashed border-stone-300 rounded-xl"><div className="w-16 h-16 rounded-lg bg-stone-200 overflow-hidden flex-shrink-0 flex items-center justify-center">{uploadedBrandLogoUrl ? (<img src={uploadedBrandLogoUrl} alt="Logo xem trước" className="w-full h-full object-cover" />) : (<span className="text-2xl text-stone-400"><i className="bi bi-award-fill"></i></span>)}</div><div className="flex-1"><input type="file" accept="image/*" onChange={handleBrandLogoChange} className="text-xs text-stone-500 file:mr-3 file:py-1.5 file:px-3 file:rounded-lg file:border-0 file:text-xs file:font-bold file:bg-amber-100 file:text-amber-800 hover:file:bg-amber-200 cursor-pointer" /><p className="text-[10px] text-stone-400 mt-1">Tải lên tệp ảnh logo thương hiệu từ máy tính.</p></div></div></div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Mô tả cam kết</label><textarea name="description" defaultValue={editingItem?.description || ''} placeholder="Cung cấp các sản phẩm sữa, bơ nhạt chuẩn quốc tế..." className="w-full h-24 bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div></>) }
              {modalType === 'user' && (<><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Họ Và Tên Thành Viên *</label><input required type="text" name="name" defaultValue={editingItem?.name || ''} placeholder="Trần Anh Quốc" className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Email Đăng Ký *</label><input required type="email" name="email" defaultValue={editingItem?.email || ''} placeholder="anhquoc@oishipan.vn" className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div><div className="grid grid-cols-2 gap-4"><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Số điện thoại</label><input required type="text" name="phone" defaultValue={editingItem?.phone || ''} placeholder="0901234567" className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Địa chỉ</label><input type="text" name="address" defaultValue={editingItem?.address || ''} placeholder="Hà Nội, Việt Nam" className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" /></div></div><div className="grid grid-cols-2 gap-4"><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Vai Trò Hệ Thống</label>{editingItem?.role === 'admin' ? (<div className="bg-red-50 border border-red-200 p-2 rounded-xl text-xs text-red-700 font-extrabold flex items-center gap-1.5 mt-0.5"><i className="bi bi-shield-lock-fill"></i> Admin tối cao (Bị Khóa)</div>) : (<select name="role" defaultValue={editingItem?.role || 'user'} className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500"><option value="user">User (Khách hàng)</option><option value="staff">Staff (Nhân viên tiệm)</option></select>)}{editingItem?.role === 'admin' && (<p className="text-[10px] text-red-500 mt-1 font-semibold">Để bảo mật hệ thống, không thể cho hoặc thay đổi quyền Admin.</p>)}</div><div><label className="block text-xs font-bold text-stone-500 uppercase mb-1">Trạng Thái Khóa</label><select name="status" defaultValue={editingItem?.status || 'Hoạt động'} className="w-full bg-stone-50 border border-stone-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500" disabled={editingItem?.role === 'admin'}><option value="Hoạt động">Hoạt động</option><option value="Tạm khóa">Tạm khóa</option></select></div></div></>) }
              <div className="border-t border-stone-100 pt-5 flex items-center justify-end gap-3"><button type="button" onClick={() => { setIsModalOpen(false); setEditingItem(null); setUploadedProductImageUrl(''); setUploadedProductFile(null); setUploadedBrandLogoUrl(''); setUploadedBrandLogoFile(null); }} className="bg-stone-100 hover:bg-stone-200 text-stone-700 font-bold text-sm px-5 py-2.5 rounded-xl transition-colors">Hủy bỏ</button><button type="submit" className="bg-amber-600 hover:bg-amber-700 text-white font-bold text-sm px-5 py-2.5 rounded-xl shadow-lg shadow-amber-600/10 transition-colors">{editingItem ? 'Lưu thay đổi' : 'Thêm mới ngay'}</button></div>
            </form>
          </div>
        </div>
      )}

      {isVariantModalOpen && selectedProductForVariants && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-stone-900/60 backdrop-blur-xs">
          <div className="bg-white rounded-3xl shadow-2xl border border-stone-200 w-full max-w-2xl overflow-hidden transform transition-all duration-300 flex flex-col">
            <div className="bg-amber-900 text-amber-50 p-5 flex justify-between items-center border-b border-amber-800"><div className="flex items-center gap-3"><img src={selectedProductForVariants.image} alt={selectedProductForVariants.name} className="w-12 h-12 rounded-lg object-cover border border-amber-700" /><div><h3 className="font-extrabold text-base md:text-lg">⚙️ Quản Lý Biến Thể Riêng</h3><p className="text-xs text-amber-300">Sản phẩm: <span className="underline font-bold">{selectedProductForVariants.name}</span></p></div></div><button onClick={() => { setIsVariantModalOpen(false); setSelectedProductForVariants(null); }} className="w-8 h-8 rounded-full bg-amber-950 text-amber-200 hover:bg-amber-800 flex items-center justify-center text-lg font-bold">✕</button></div>
            <div className="p-6 grid grid-cols-1 md:grid-cols-5 gap-6 max-h-[75vh] overflow-y-auto"><form onSubmit={handleSaveVariant} className="md:col-span-2 space-y-3.5 bg-amber-50/40 p-4 rounded-2xl border border-amber-100"><h4 className="font-bold text-amber-900 text-xs uppercase tracking-wider">{variantFormMode === 'add' ? '➕ Thêm Biến Thể' : '✏️ Sửa Biến Thể'}</h4><div><label className="block text-[10px] font-bold text-stone-500 uppercase mb-1">Tên biến thể *</label><input required type="text" value={variantName} onChange={(e) => setVariantName(e.target.value)} placeholder="Ví dụ: Thêm phô mai, Size Lớn..." className="w-full bg-white border border-stone-300 rounded-lg px-3 py-1.5 text-xs focus:outline-none focus:ring-1 focus:ring-amber-500" /></div><div><label className="block text-[10px] font-bold text-stone-500 uppercase mb-1">Giá chênh lệch (VNĐ) *</label><input required type="number" value={variantPriceAdj} onChange={(e) => setVariantPriceAdj(e.target.value)} placeholder="Ví dụ: +15000 hoặc -5000" className="w-full bg-white border border-stone-300 rounded-lg px-3 py-1.5 text-xs focus:outline-none focus:ring-1 focus:ring-amber-500" /><p className="text-[10px] text-stone-400 mt-1">Hệ thống tự động cộng/trừ vào giá gốc sản phẩm.</p></div><div><label className="block text-[10px] font-bold text-stone-500 uppercase mb-1">Số lượng tồn kho biến thể *</label><input required type="number" value={variantStock} onChange={(e) => setVariantStock(e.target.value)} placeholder="10" className="w-full bg-white border border-stone-300 rounded-lg px-3 py-1.5 text-xs focus:outline-none focus:ring-1 focus:ring-amber-500" /></div><div className="pt-2 flex gap-2"><button type="submit" className="flex-1 bg-amber-700 hover:bg-amber-800 text-white font-bold text-xs py-2 rounded-xl shadow transition-colors">{variantFormMode === 'add' ? 'Thêm biến thể' : 'Lưu biến thể'}</button>{variantFormMode === 'edit' && (<button type="button" onClick={resetVariantForm} className="bg-stone-200 hover:bg-stone-300 text-stone-700 font-bold text-xs px-3 py-2 rounded-xl transition-colors">Hủy</button>)}</div></form><div className="md:col-span-3 flex flex-col gap-3"><h4 className="font-bold text-stone-700 text-xs uppercase tracking-wider">📋 Danh sách biến thể hiện tại ({selectedProductForVariants.variants?.length || 0})</h4>{selectedProductForVariants.variants && selectedProductForVariants.variants.length > 0 ? (<div className="border border-stone-200 rounded-2xl overflow-hidden bg-white shadow-xs max-h-72 overflow-y-auto"><table className="w-full text-left text-xs"><thead><tr className="bg-stone-100 border-b border-stone-200 text-stone-500 font-bold"><th className="p-3">Tên biến thể</th><th className="p-3 text-right">Chênh lệch giá</th><th className="p-3 text-center">Tồn kho</th><th className="p-3 text-center">Hành động</th></tr></thead><tbody className="divide-y divide-stone-100">{selectedProductForVariants.variants.map((variant) => (<tr key={variant.id} className="hover:bg-amber-50/20 transition-colors"><td className="p-3 font-semibold text-stone-800">{variant.name}</td><td className="p-3 text-right font-bold text-amber-700">{variant.priceAdjustment >= 0 ? `+${variant.priceAdjustment.toLocaleString('vi-VN')} đ` : `${variant.priceAdjustment.toLocaleString('vi-VN')} đ`}</td><td className="p-3 text-center text-stone-600 font-mono">{variant.stock ?? 0}</td><td className="p-3 text-center space-x-1 whitespace-nowrap"><button onClick={() => handleEditVariantClick(variant)} className="bg-blue-50 text-blue-600 px-2 py-1 rounded hover:bg-blue-100 transition-colors">Sửa</button><button onClick={() => handleDeleteVariant(variant.id)} className="bg-red-50 text-red-600 px-2 py-1 rounded hover:bg-red-100 transition-colors">Xóa</button></td></tr>))}</tbody></table></div>) : (<div className="flex flex-col items-center justify-center p-8 bg-stone-50 border border-stone-200 rounded-2xl border-dashed"><span className="text-3xl mb-1"><i className="bi bi-slash-circle text-stone-400"></i></span><p className="text-xs text-stone-400 italic">Chưa có biến thể nào được tạo. Hãy tạo mới bên cạnh!</p></div>)}</div></div><div className="border-t border-stone-200 p-4 bg-stone-50 flex justify-end"><button type="button" onClick={() => { setIsVariantModalOpen(false); setSelectedProductForVariants(null); }} className="bg-amber-950 text-white font-bold text-xs px-5 py-2.5 rounded-xl hover:bg-amber-900 transition-colors">Hoàn tất quản lý</button></div></div></div>)}
    </div>
  );
}

const rootElement = document.getElementById('admin-root');
if (rootElement) {
  const root = ReactDOM.createRoot(rootElement);
  root.render(<App />);
}
