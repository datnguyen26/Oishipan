/* ============================================
   MODERN ADMIN THEME - JavaScript
   ============================================ */

document.addEventListener('DOMContentLoaded', function () {
    initializeAdmin();
});

function initializeAdmin() {
    setupSidebarToggle();
    setupThemeToggle();
    setupActiveNavigation();
    setupToastNotifications();
}

/**
 * Setup Sidebar Toggle for Mobile
 */
function setupSidebarToggle() {
    const toggleBtn = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('adminSidebar');

    if (toggleBtn && sidebar) {
        toggleBtn.addEventListener('click', function () {
            sidebar.classList.toggle('open');
        });

        // Close sidebar when a nav item is clicked
        const navItems = sidebar.querySelectorAll('.nav-item');
        navItems.forEach(item => {
            item.addEventListener('click', function () {
                if (window.innerWidth <= 768) {
                    sidebar.classList.remove('open');
                }
            });
        });

        // Close sidebar when clicking outside
        document.addEventListener('click', function (e) {
            if (!sidebar.contains(e.target) && !toggleBtn.contains(e.target)) {
                sidebar.classList.remove('open');
            }
        });
    }
}

/**
 * Setup Theme Toggle (Dark/Light Mode)
 */
function setupThemeToggle() {
    const themeToggle = document.getElementById('themeToggle');
    const html = document.documentElement;

    if (themeToggle) {
        // Load saved theme or default to dark
        const savedTheme = localStorage.getItem('admin-theme') || 'dark';
        setTheme(savedTheme);

        themeToggle.addEventListener('click', function () {
            const currentTheme = html.getAttribute('data-theme') || 'dark';
            const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
            setTheme(newTheme);
            localStorage.setItem('admin-theme', newTheme);
        });
    }
}

function setTheme(theme) {
    const html = document.documentElement;
    const themeToggle = document.getElementById('themeToggle');

    if (theme === 'light') {
        html.setAttribute('data-theme', 'light');
        document.body.style.background = '#f8fafc';
        if (themeToggle) {
            themeToggle.innerHTML = '<i class="bi bi-moon-stars-fill"></i>';
        }
    } else {
        html.removeAttribute('data-theme');
        document.body.style.background = 'linear-gradient(135deg, #0f172a 0%, #08101f 50%, #02040a 100%)';
        if (themeToggle) {
            themeToggle.innerHTML = '<i class="bi bi-sun-fill"></i>';
        }
    }
}

/**
 * Setup Active Navigation Link
 */
function setupActiveNavigation() {
    const currentPath = window.location.pathname;
    const navItems = document.querySelectorAll('.sidebar-nav .nav-item');

    navItems.forEach(item => {
        const href = item.getAttribute('href');
        if (href && currentPath.includes(href)) {
            item.classList.add('active');
        } else {
            item.classList.remove('active');
        }
    });
}

/**
 * Toast Notification System
 */
function setupToastNotifications() {
    // Check if there are any toast messages to display
    const toastMessages = document.querySelectorAll('.toast-message');
    toastMessages.forEach(msg => {
        showToast(msg.textContent, msg.dataset.type || 'success');
    });
}

function showToast(message, type = 'success', duration = 4000) {
    const container = document.querySelector('.toast-container');
    if (!container) return;

    const toastId = 'toast-' + Date.now();
    const bgColor = type === 'success' ? '#10b981' : type === 'warning' ? '#f59e0b' : '#ef4444';
    const icon = type === 'success' ? 'bi-check-circle-fill' : type === 'warning' ? 'bi-exclamation-triangle-fill' : 'bi-x-circle-fill';

    const toast = document.createElement('div');
    toast.id = toastId;
    toast.className = 'toast-notification animate-slide-in';
    toast.innerHTML = `
        <div class="toast-content">
            <i class="bi ${icon}"></i>
            <span>${message}</span>
        </div>
    `;
    toast.style.cssText = `
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 16px 20px;
        background: rgba(${parseInt(bgColor.slice(1, 3), 16)}, ${parseInt(bgColor.slice(3, 5), 16)}, ${parseInt(bgColor.slice(5, 7), 16)}, 0.15);
        border: 1px solid ${bgColor}40;
        border-radius: 12px;
        color: ${bgColor};
        font-weight: 500;
        animation: slideIn 0.3s ease-out;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
    `;

    container.appendChild(toast);

    setTimeout(() => {
        toast.style.animation = 'fadeOut 0.3s ease-out';
        setTimeout(() => toast.remove(), 300);
    }, duration);
}

/**
 * Format Currency (Vietnamese Đồng)
 */
function formatCurrency(value) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(value);
}

/**
 * Table Actions - Edit/Delete
 */
function setupTableActions() {
    const deleteButtons = document.querySelectorAll('[data-action="delete"]');
    deleteButtons.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const itemId = this.dataset.id;
            const itemName = this.dataset.name || 'bản ghi này';

            if (confirm(`Bạn có chắc chắn muốn xóa ${itemName}?`)) {
                // Call delete API or submit form
                this.closest('form')?.submit();
            }
        });
    });
}

/**
 * Form Validation Helper
 */
function validateForm(formElement) {
    let isValid = true;
    const requiredFields = formElement.querySelectorAll('[required]');

    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            field.classList.add('is-invalid');
            isValid = false;
        } else {
            field.classList.remove('is-invalid');
        }
    });

    return isValid;
}

/**
 * Search/Filter Helper
 */
function setupSearchFilter() {
    const searchInput = document.getElementById('searchInput');
    const filterSelect = document.getElementById('filterSelect');
    const tableRows = document.querySelectorAll('table tbody tr');

    function filterTable() {
        const searchTerm = (searchInput?.value || '').toLowerCase();
        const filterValue = filterSelect?.value || '';

        tableRows.forEach(row => {
            let matches = true;

            if (searchTerm) {
                matches = row.textContent.toLowerCase().includes(searchTerm);
            }

            if (filterValue && filterValue !== 'All') {
                const statusCell = row.querySelector('[data-status]');
                if (statusCell) {
                    matches = matches && statusCell.dataset.status === filterValue;
                }
            }

            row.style.display = matches ? '' : 'none';
        });
    }

    if (searchInput) {
        searchInput.addEventListener('input', filterTable);
    }
    if (filterSelect) {
        filterSelect.addEventListener('change', filterTable);
    }
}

/**
 * Pagination Helper
 */
function setupPagination() {
    const paginationLinks = document.querySelectorAll('.pagination a');
    paginationLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            // Let default link behavior or add AJAX loading
        });
    });
}

/**
 * Export Data Helper
 */
function exportTableToCSV(filename = 'export.csv') {
    const table = document.querySelector('table');
    if (!table) return;

    let csv = [];
    const rows = table.querySelectorAll('tr');

    rows.forEach(row => {
        let csvRow = [];
        const cells = row.querySelectorAll('td, th');
        cells.forEach(cell => {
            csvRow.push('"' + cell.textContent.trim().replace(/"/g, '""') + '"');
        });
        csv.push(csvRow.join(','));
    });

    const csvContent = 'data:text/csv;charset=utf-8,' + encodeURIComponent(csv.join('\n'));
    const link = document.createElement('a');
    link.setAttribute('href', csvContent);
    link.setAttribute('download', filename);
    link.click();
}

/**
 * Print Helper
 */
function printPage() {
    window.print();
}

/**
 * Initialize after page load
 */
window.addEventListener('load', function () {
    setupTableActions();
    setupSearchFilter();
    setupPagination();
});
