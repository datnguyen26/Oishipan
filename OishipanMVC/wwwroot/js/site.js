// Site-wide JavaScript functionality

// API Base URL
const API_BASE_URL = window.apiBaseUrl || 'http://localhost:5000/api';

// Get JWT Token from session storage
function getAuthToken() {
    return sessionStorage.getItem('jwtToken') || localStorage.getItem('jwtToken');
}

// Set JWT Token
function setAuthToken(token) {
    sessionStorage.setItem('jwtToken', token);
}

// Clear JWT Token
function clearAuthToken() {
    sessionStorage.removeItem('jwtToken');
    localStorage.removeItem('jwtToken');
}

// Make API call with Authorization header
async function apiCall(endpoint, method = 'GET', data = null) {
    const headers = {
        'Content-Type': 'application/json',
    };

    const token = getAuthToken();
    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const options = {
        method: method,
        headers: headers,
    };

    if (data && (method === 'POST' || method === 'PUT')) {
        options.body = JSON.stringify(data);
    }

    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, options);

        if (response.status === 401) {
            clearAuthToken();
            window.location.href = '/Account/Login';
            return null;
        }

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error('API call failed:', error);
        throw error;
    }
}

// SPA-like navigation for links that should not reload the page
function initializeSpaNavigation() {
    document.body.addEventListener('click', function (event) {
        const anchor = event.target.closest('a.spa-link');
        if (!anchor) return;

        const href = anchor.getAttribute('href');
        if (!href || href.startsWith('#') || href.startsWith('mailto:') || href.startsWith('tel:') || href.startsWith('javascript:')) {
            return;
        }

        const targetUrl = new URL(href, window.location.origin);
        if (targetUrl.origin !== window.location.origin) {
            return;
        }

        event.preventDefault();
        navigateToUrl(targetUrl.pathname + targetUrl.search);
    });

    window.addEventListener('popstate', function () {
        loadPage(window.location.pathname + window.location.search, false);
    });
}

async function navigateToUrl(url) {
    await loadPage(url, true);
}

async function loadPage(url, pushHistory = true) {
    try {
        const response = await fetch(url, {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (!response.ok) {
            window.location.href = url;
            return;
        }

        const html = await response.text();
        const parser = new DOMParser();
        const doc = parser.parseFromString(html, 'text/html');
        const newMain = doc.querySelector('main');
        const newTitle = doc.querySelector('title')?.textContent;

        if (newMain) {
            document.querySelector('main').innerHTML = newMain.innerHTML;
        }

        if (newTitle) {
            document.title = newTitle;
        }

        if (pushHistory) {
            history.pushState({}, '', url);
        }

        runPageScripts(doc);
    } catch (error) {
        console.error('Navigation failed:', error);
        window.location.href = url;
    }
}

function runPageScripts(doc) {
    const scripts = doc.querySelectorAll('script');
    scripts.forEach(script => {
        if (script.src) {
            const src = script.src;
            if (src.includes('bootstrap') || src.includes('site.js')) {
                return;
            }
            const newScript = document.createElement('script');
            newScript.src = src;
            document.body.appendChild(newScript);
        } else {
            const inlineScript = document.createElement('script');
            inlineScript.text = script.textContent;
            document.body.appendChild(inlineScript);
        }
    });
}

document.addEventListener('DOMContentLoaded', initializeSpaNavigation);
