/**
 * Dalabat API Integration Script for Geed Balaadh Bootstrap Template
 * Connects Frontend HTML Forms, Bookings, Cart, Coupons, Reviews, Authentication & Order Tracking
 */

const API_BASE = '/api';
const AUTH_KEY = 'geed_balaadh_user';

// Global Cart & Coupon State
let cart = [];
let appliedCoupon = null;
let pendingActionCallback = null;

$(document).ready(function () {
    fixNavbarButtons();
    initAuthSystem();
    initNavbarCart();
    loadRestaurantsAndMenu();
    setupBookingForms();
    setupNewsletterForms();
    setupOrderTracker();
    setupReviewSystem();
    loadReviews();
});

// Bind functions to window object for global HTML click access
window.getCurrentUser = getCurrentUser;
window.setCurrentUser = setCurrentUser;
window.logoutUser = logoutUser;
window.requireAuth = requireAuth;
window.openAuthModal = openAuthModal;
window.handleAuthLogin = handleAuthLogin;
window.handleAuthRegister = handleAuthRegister;
window.openCartModal = openCartModal;
window.openTrackerModal = openTrackerModal;
window.openReviewModal = openReviewModal;
window.addToCart = addToCart;
window.updateCartQuantity = updateCartQuantity;
window.applyCouponCode = applyCouponCode;
window.submitCartOrder = submitCartOrder;
window.fetchOrderStatus = fetchOrderStatus;

// User Session Management & Auth Functions
function getCurrentUser() {
    try {
        const data = localStorage.getItem(AUTH_KEY);
        return data ? JSON.parse(data) : null;
    } catch (e) {
        return null;
    }
}

function setCurrentUser(user) {
    localStorage.setItem(AUTH_KEY, JSON.stringify(user));
    updateAuthUI();
}

function logoutUser() {
    localStorage.removeItem(AUTH_KEY);
    showNotification("You have logged out successfully.");
    updateAuthUI();
}

function requireAuth(callback, promptMsg) {
    const user = getCurrentUser();
    if (user) {
        if (typeof callback === 'function') callback();
        return true;
    }
    pendingActionCallback = callback || null;
    openAuthModal(promptMsg || "Please login or register to continue.");
    return false;
}

function openAuthModal(promptMessage) {
    if (promptMessage) {
        $('#authMessageAlert').html(`<i class="fa fa-exclamation-circle me-1"></i> ${promptMessage}`).removeClass('d-none');
    } else {
        $('#authMessageAlert').addClass('d-none');
    }
    const modalEl = document.getElementById('authModal');
    if (modalEl) {
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();
    }
}

function initAuthSystem() {
    // Inject Auth Modal if not present
    if (!$('#authModal').length) {
        const authModalHtml = `
        <div class="modal fade" id="authModal" tabindex="-1" aria-labelledby="authModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content shadow-lg border-0">
                    <div class="modal-header bg-dark text-white border-secondary">
                        <h5 class="modal-title text-primary" id="authModalLabel"><i class="fa fa-user-lock me-2"></i> Account Access</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body p-4">
                        <div id="authMessageAlert" class="alert alert-warning d-none py-2 mb-3 small"></div>

                        <!-- Nav Tabs for Login & Register -->
                        <ul class="nav nav-pills nav-justified mb-4" id="authTabs" role="tablist">
                            <li class="nav-item" role="presentation">
                                <button class="nav-link active fw-bold" id="login-tab" data-bs-toggle="pill" data-bs-target="#loginTabPane" type="button" role="tab">
                                    <i class="fa fa-sign-in-alt me-1"></i> Login
                                </button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button class="nav-link fw-bold" id="register-tab" data-bs-toggle="pill" data-bs-target="#regTabPane" type="button" role="tab">
                                    <i class="fa fa-user-plus me-1"></i> Register
                                </button>
                            </li>
                        </ul>

                        <div class="tab-content" id="authTabsContent">
                            <!-- LOGIN TAB -->
                            <div class="tab-pane fade show active" id="loginTabPane" role="tabpanel">
                                <form id="loginForm" onsubmit="handleAuthLogin(event)">
                                    <div class="mb-3">
                                        <label class="form-label fw-bold text-dark">Email Address</label>
                                        <div class="input-group">
                                            <span class="input-group-text bg-light"><i class="fa fa-envelope text-primary"></i></span>
                                            <input type="email" id="loginEmailInput" class="form-control" placeholder="name@example.com" required value="ahmed.user@gmail.com">
                                        </div>
                                    </div>
                                    <div class="mb-3">
                                        <label class="form-label fw-bold text-dark">Password</label>
                                        <div class="input-group">
                                            <span class="input-group-text bg-light"><i class="fa fa-key text-primary"></i></span>
                                            <input type="password" id="loginPasswordInput" class="form-control" placeholder="••••••••" required value="Password123!">
                                        </div>
                                    </div>
                                    <div id="loginErrorMsg" class="text-danger small mb-3"></div>
                                    <button type="submit" id="loginSubmitBtn" class="btn btn-primary w-100 py-2 fw-bold shadow-sm">
                                        <i class="fa fa-sign-in-alt me-1"></i> Log In
                                    </button>
                                </form>
                            </div>

                            <!-- REGISTER TAB -->
                            <div class="tab-pane fade" id="regTabPane" role="tabpanel">
                                <form id="registerForm" onsubmit="handleAuthRegister(event)">
                                    <div class="mb-2">
                                        <label class="form-label fw-bold text-dark small mb-1">Full Name *</label>
                                        <input type="text" id="regNameInput" class="form-control form-control-sm" placeholder="e.g. Abdifatah Faisal" required>
                                    </div>
                                    <div class="mb-2">
                                        <label class="form-label fw-bold text-dark small mb-1">Email Address *</label>
                                        <input type="email" id="regEmailInput" class="form-control form-control-sm" placeholder="name@example.com" required>
                                    </div>
                                    <div class="mb-2">
                                        <label class="form-label fw-bold text-dark small mb-1">Phone Number</label>
                                        <input type="tel" id="regPhoneInput" class="form-control form-control-sm" placeholder="407596">
                                    </div>
                                    <div class="mb-2">
                                        <label class="form-label fw-bold text-dark small mb-1">Delivery Address</label>
                                        <input type="text" id="regAddressInput" class="form-control form-control-sm" placeholder="Hargeisa Xero-awr">
                                    </div>
                                    <div class="mb-3">
                                        <label class="form-label fw-bold text-dark small mb-1">Password *</label>
                                        <input type="password" id="regPasswordInput" class="form-control form-control-sm" placeholder="Create a password" required>
                                    </div>
                                    <div id="regErrorMsg" class="text-danger small mb-3"></div>
                                    <button type="submit" id="regSubmitBtn" class="btn btn-warning text-dark w-100 py-2 fw-bold shadow-sm">
                                        <i class="fa fa-user-plus me-1"></i> Register Account
                                    </button>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        `;
        $('body').append(authModalHtml);
    }

    updateAuthUI();
}

async function handleAuthLogin(e) {
    e.preventDefault();
    const email = $('#loginEmailInput').val().trim();
    const password = $('#loginPasswordInput').val();
    const errDiv = $('#loginErrorMsg').empty();
    const btn = $('#loginSubmitBtn').prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span> Logging in...');

    try {
        const res = await fetch(`${API_BASE}/users/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        const data = await res.json();
        if (!res.ok) {
            throw new Error(data.message || 'Login failed.');
        }

        setCurrentUser(data);
        showNotification(`Welcome back, ${data.name}! 👋`);

        const modalEl = document.getElementById('authModal');
        const modal = bootstrap.Modal.getInstance(modalEl);
        if (modal) modal.hide();

        if (pendingActionCallback) {
            const cb = pendingActionCallback;
            pendingActionCallback = null;
            cb();
        }
    } catch (err) {
        errDiv.text(err.message);
    } finally {
        btn.prop('disabled', false).html('<i class="fa fa-sign-in-alt me-1"></i> Log In');
    }
}

async function handleAuthRegister(e) {
    e.preventDefault();
    const name = $('#regNameInput').val().trim();
    const email = $('#regEmailInput').val().trim();
    const phone = $('#regPhoneInput').val().trim();
    const address = $('#regAddressInput').val().trim();
    const password = $('#regPasswordInput').val();

    const errDiv = $('#regErrorMsg').empty();
    const btn = $('#regSubmitBtn').prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span> Creating Account...');

    try {
        const res = await fetch(`${API_BASE}/users/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, email, phone, address, password })
        });

        const data = await res.json();
        if (!res.ok) {
            throw new Error(data.message || 'Registration failed.');
        }

        setCurrentUser(data);
        showNotification(`Account created! Welcome, ${data.name}! 🎉`);

        const modalEl = document.getElementById('authModal');
        const modal = bootstrap.Modal.getInstance(modalEl);
        if (modal) modal.hide();

        if (pendingActionCallback) {
            const cb = pendingActionCallback;
            pendingActionCallback = null;
            cb();
        }
    } catch (err) {
        errDiv.text(err.message);
    } finally {
        btn.prop('disabled', false).html('<i class="fa fa-user-plus me-1"></i> Register Account');
    }
}

function updateAuthUI() {
    const user = getCurrentUser();
    let authNavContainer = $('#authNavContainer');

    if (!authNavContainer.length) {
        const target = $('#navbarCollapse');
        if (target.length) {
            target.append('<div id="authNavContainer" class="d-flex align-items-center ms-auto"></div>');
            authNavContainer = $('#authNavContainer');
        }
    }

    if (user) {
        authNavContainer.html(`
            <div class="nav-item dropdown ms-2">
                <a href="#" class="nav-link dropdown-toggle text-warning fw-bold d-flex align-items-center py-2 px-3 rounded bg-dark border border-warning" data-bs-toggle="dropdown">
                    <i class="fa fa-user-circle fa-lg me-2 text-primary"></i>
                    <span class="text-white me-1">${escapeHtml(user.name)}</span>
                </a>
                <div class="dropdown-menu dropdown-menu-end m-0 bg-dark border-secondary shadow-lg">
                    <div class="px-3 py-2 border-bottom border-secondary">
                        <small class="text-muted d-block">Signed in as</small>
                        <strong class="text-warning small">${escapeHtml(user.email)}</strong>
                    </div>
                    <a class="dropdown-item text-light py-2" href="javascript:void(0)" onclick="openCartModal()"><i class="fa fa-shopping-basket me-2 text-warning"></i> My Basket</a>
                    <a class="dropdown-item text-light py-2" href="javascript:void(0)" onclick="openTrackerModal()"><i class="fa fa-motorcycle me-2 text-info"></i> Track My Order</a>
                    <div class="dropdown-divider border-secondary"></div>
                    <a class="dropdown-item text-danger fw-bold py-2" href="javascript:void(0)" onclick="logoutUser()"><i class="fa fa-sign-out-alt me-2"></i> Logout</a>
                </div>
            </div>
        `);
    } else {
        authNavContainer.html(`
            <a href="login.html" class="btn btn-outline-warning py-2 px-3 ms-2 fw-bold shadow-sm">Login</a>
            <a href="register.html" class="btn btn-warning text-dark py-2 px-3 ms-2 fw-bold shadow-sm">Register</a>
        `);
    }

    // Auto-fill form fields if user logged in
    if (user) {
        $('#cartUserDisplay').html(`<div class="alert alert-success py-2 mb-0 small"><i class="fa fa-check-circle me-1"></i> Ordering as <strong>${escapeHtml(user.name)}</strong> (${escapeHtml(user.email)})</div>`);
        $('#name, input[placeholder*="Name"]').val(user.name);
        $('#email, input[placeholder*="Email"]').val(user.email);
    } else {
        $('#cartUserDisplay').html(`<div class="alert alert-warning py-2 mb-0 small"><i class="fa fa-exclamation-triangle me-1"></i> You are not logged in. <a href="javascript:void(0)" onclick="openAuthModal('Please login to complete checkout.')" class="fw-bold text-dark text-decoration-underline">Click here to Login</a></div>`);
    }
}

// 1. Fix Top Navbar Navigation & Replace Generic "Buy Pro Version" Button
function fixNavbarButtons() {
    $('a[href*="htmlcodex.com"]').each(function () {
        $(this).attr('href', 'booking.html')
            .removeClass('btn-primary')
            .addClass('btn-primary py-2 px-4 shadow-sm')
            .html('<i class="fa fa-utensils me-2"></i>Book A Table');
    });

    $('.hero-header a.btn-primary').each(function () {
        if ($(this).text().trim().toLowerCase().includes('book') || !$(this).attr('href')) {
            $(this).attr('href', 'booking.html');
        }
    });

    $('a.btn-primary').each(function () {
        const txt = $(this).text().trim().toLowerCase();
        if (txt.includes('read more')) {
            $(this).attr('href', 'about.html');
        }
    });
}

// 2. Navbar Cart, Order Tracking & Review Buttons Initialization
function initNavbarCart() {
    const navCollapse = $('#navbarCollapse .navbar-nav');
    if (navCollapse.length && !$('#cartNavBtn').length) {
        const cartNavHtml = `
            <a href="javascript:void(0)" id="cartNavBtn" class="nav-item nav-link position-relative text-warning fw-bold me-2" onclick="openCartModal()">
                <i class="fa fa-shopping-cart me-1"></i> Cart
                <span id="cartBadge" class="badge bg-primary text-white rounded-pill ms-1">0</span>
            </a>
            <a href="javascript:void(0)" id="trackNavBtn" class="nav-item nav-link text-info fw-bold me-2" onclick="openTrackerModal()">
                <i class="fa fa-motorcycle me-1"></i> Track Order
            </a>
            <a href="javascript:void(0)" id="reviewNavBtn" class="nav-item nav-link text-light fw-bold" onclick="openReviewModal()">
                <i class="fa fa-star text-warning me-1"></i> Reviews
            </a>
        `;
        $('#navbarCollapse').children('.btn-primary, a[href="booking.html"]').before(cartNavHtml);
    }

    // Inject Floating Quick Cart Button
    if (!$('#floatingCartBtn').length) {
        $('body').append(`
            <div id="floatingCartBtn" class="floating-cart-btn" onclick="openCartModal()" title="View Cart">
                <i class="fa fa-shopping-basket"></i>
                <span id="floatingCartBadge" class="badge bg-danger rounded-pill">0</span>
            </div>
        `);
    }

    // Inject Cart Modal HTML into body if not present
    if (!$('#cartModal').length) {
        const cartModalHtml = `
        <div class="modal fade" id="cartModal" tabindex="-1" aria-labelledby="cartModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-dark text-white">
                        <h5 class="modal-title text-primary" id="cartModalLabel"><i class="fa fa-shopping-bag me-2"></i> Your Dalabat Basket</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div id="cartItemsContainer">
                            <p class="text-muted text-center py-4">Your cart is currently empty.</p>
                        </div>
                        <hr class="my-4">
                        <!-- Coupon Section -->
                        <div class="card p-3 bg-light mb-3 border-0">
                            <label class="form-label text-dark fw-bold mb-1"><i class="fa fa-ticket-alt text-primary me-1"></i> Discount Coupon Code</label>
                            <div class="input-group">
                                <input type="text" id="couponCodeInput" class="form-control text-uppercase" placeholder="e.g. DALABAT20, WELCOME10, SAVE15">
                                <button type="button" class="btn btn-outline-primary fw-bold" onclick="applyCouponCode()">Apply Coupon</button>
                            </div>
                            <div id="couponMessage" class="mt-2 small"></div>
                        </div>

                        <h5 class="mb-3 text-dark">Customer & Delivery Details</h5>
                        <form id="checkoutForm">
                            <div class="row g-3 mb-2">
                                <div class="col-12" id="cartUserDisplay">
                                    <!-- User Status Injected Here -->
                                </div>
                                <div class="col-12">
                                    <label class="form-label text-dark fw-bold">Payment Method</label>
                                    <select class="form-select" id="cartPaymentStatus">
                                        <option value="Paid">Credit / Debit Card (Instant)</option>
                                        <option value="Cash On Delivery">Cash On Delivery</option>
                                        <option value="Apple Pay">Apple Pay / Digital Wallet</option>
                                    </select>
                                </div>
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer bg-light d-flex justify-content-between">
                        <div class="h5 mb-0 text-dark">
                            Total: <span id="cartTotalPrice" class="text-primary fw-bold">$0.00</span>
                            <span id="discountBadge" class="badge bg-success ms-2 d-none"></span>
                        </div>
                        <div>
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                            <button type="button" class="btn btn-primary px-4" id="submitOrderBtn" onclick="submitCartOrder()">Place Order Now</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        `;
        $('body').append(cartModalHtml);
    }

    // Inject Order Tracking Modal
    if (!$('#trackerModal').length) {
        const trackerModalHtml = `
        <div class="modal fade" id="trackerModal" tabindex="-1" aria-labelledby="trackerModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header bg-dark text-white">
                        <h5 class="modal-title text-primary" id="trackerModalLabel"><i class="fa fa-motorcycle me-2"></i> Track Your Dalabat Order</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <label class="form-label fw-bold">Enter Order ID</label>
                        <div class="input-group mb-3">
                            <input type="number" class="form-control" id="trackOrderIdInput" placeholder="Order ID (e.g. 1 or 3)" value="1">
                            <button class="btn btn-primary" type="button" onclick="fetchOrderStatus()">Track Now</button>
                        </div>
                        <div id="orderStatusResult" class="mt-3"></div>
                    </div>
                </div>
            </div>
        </div>
        `;
        $('body').append(trackerModalHtml);
    }

    // Inject Reservation Confirmation Modal
    if (!$('#reservationModal').length) {
        const reservationModalHtml = `
        <div class="modal fade" id="reservationModal" tabindex="-1" aria-labelledby="reservationModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content border-success">
                    <div class="modal-header bg-success text-white">
                        <h5 class="modal-title" id="reservationModalLabel"><i class="fa fa-check-circle me-2"></i> Table Reservation Confirmed!</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body text-center py-4" id="reservationModalBody">
                        <!-- Dynamic details injected here -->
                    </div>
                    <div class="modal-footer bg-light">
                        <button type="button" class="btn btn-secondary w-100" data-bs-dismiss="modal">Close & Done</button>
                    </div>
                </div>
            </div>
        </div>
        `;
        $('body').append(reservationModalHtml);
    }
}

// 3. Load Restaurants & Food Items dynamically from Backend SQL Server
async function loadRestaurantsAndMenu() {
    try {
        const response = await fetch(`${API_BASE}/restaurants`);
        if (!response.ok) throw new Error("Failed to load backend menu");
        const restaurants = await response.json();

        // Populate Restaurant Select dropdowns across all forms
        const restSelects = $('#select1, #selectRestaurant, #reviewRestaurantSelect, #bookingRestaurantSelect');
        restSelects.each(function () {
            const select = $(this);
            select.empty();
            restaurants.forEach(r => {
                select.append(`<option value="${r.restaurantID}">${r.name} (${r.address})</option>`);
            });
        });

        // Render dynamic menu tabs and items
        const tabContent = $('.tab-content');
        const tabPills = $('.nav-pills');

        if (tabContent.length && restaurants.length > 0) {
            tabPills.empty();
            tabContent.empty();

            const icons = ['fa-hamburger', 'fa-pizza-slice', 'fa-utensils', 'fa-coffee'];
            const images = ['img/menu-1.jpg', 'img/menu-2.jpg', 'img/menu-3.jpg', 'img/menu-4.jpg', 'img/menu-5.jpg', 'img/menu-6.jpg', 'img/menu-7.jpg', 'img/menu-8.jpg'];

            restaurants.forEach((restaurant, index) => {
                const isActive = index === 0 ? 'active' : '';
                const tabId = `tab-rest-${restaurant.restaurantID}`;
                const icon = icons[index % icons.length];

                // Render Tab Header
                tabPills.append(`
                    <li class="nav-item">
                        <a class="d-flex align-items-center text-start mx-2 pb-3 ${isActive}" data-bs-toggle="pill" href="#${tabId}">
                            <i class="fa ${icon} fa-2x text-primary"></i>
                            <div class="ps-3">
                                <small class="text-body">Restaurant</small>
                                <h6 class="mt-n1 mb-0">${restaurant.name}</h6>
                            </div>
                        </a>
                    </li>
                `);

                // Render Tab Body Items
                let itemsHtml = '';
                if (restaurant.menuItems && restaurant.menuItems.length > 0) {
                    restaurant.menuItems.forEach((item, imgIndex) => {
                        const img = images[imgIndex % images.length];
                        itemsHtml += `
                            <div class="col-lg-6">
                                <div class="d-flex align-items-center p-3 rounded border bg-light shadow-sm h-100">
                                    <img class="flex-shrink-0 img-fluid rounded" src="${img}" alt="${item.name}" style="width: 85px; height: 85px; object-fit: cover;">
                                    <div class="w-100 d-flex flex-column text-start ps-3">
                                        <h5 class="d-flex justify-content-between border-bottom pb-2 mb-1">
                                            <span>${item.name}</span>
                                            <span class="text-primary fw-bold">$${item.price.toFixed(2)}</span>
                                        </h5>
                                        <small class="fst-italic text-muted mb-2">${item.description || 'Freshly prepared specialty'}</small>
                                        <div class="text-end mt-auto">
                                            <button class="btn btn-sm btn-primary py-1 px-3 rounded-pill shadow-sm" onclick="addToCart(${item.foodID}, '${escapeHtml(item.name)}', ${item.price}, ${restaurant.restaurantID}, '${escapeHtml(restaurant.name)}')">
                                                <i class="fa fa-cart-plus me-1"></i> Add to Cart
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        `;
                    });
                } else {
                    itemsHtml = `<div class="col-12 text-center text-muted py-4">No menu items currently available.</div>`;
                }

                tabContent.append(`
                    <div id="${tabId}" class="tab-pane fade show p-0 ${isActive}">
                        <div class="row g-4">
                            ${itemsHtml}
                        </div>
                    </div>
                `);
            });
        }
    } catch (err) {
        console.error("Error loading Dalabat restaurant menu:", err);
    }
}

// 4. Cart Logic & Coupon Application
function addToCart(foodId, name, price, restaurantId, restaurantName) {
    if (!requireAuth(() => addToCart(foodId, name, price, restaurantId, restaurantName), "Please login or create an account to add items to your basket.")) {
        return;
    }

    if (cart.length > 0 && cart[0].restaurantId !== restaurantId) {
        if (!confirm(`Your cart has items from ${cart[0].restaurantName}. Clear cart and add from ${restaurantName}?`)) {
            return;
        }
        cart = [];
        appliedCoupon = null;
    }

    const existingIndex = cart.findIndex(item => item.foodId === foodId);
    if (existingIndex > -1) {
        cart[existingIndex].quantity += 1;
    } else {
        cart.push({
            foodId: foodId,
            name: name,
            price: price,
            quantity: 1,
            restaurantId: restaurantId,
            restaurantName: restaurantName
        });
    }

    updateCartUI();
    showNotification(`Added "${name}" to cart!`);
}

function updateCartQuantity(foodId, delta) {
    const item = cart.find(i => i.foodId === foodId);
    if (item) {
        item.quantity += delta;
        if (item.quantity <= 0) {
            cart = cart.filter(i => i.foodId !== foodId);
        }
    }
    if (cart.length === 0) appliedCoupon = null;
    updateCartUI();
    renderCartModal();
}

function updateCartUI() {
    const totalCount = cart.reduce((sum, item) => sum + item.quantity, 0);
    $('#cartBadge, #floatingCartBadge').text(totalCount);
}

function openCartModal() {
    updateAuthUI();
    renderCartModal();
    const modal = new bootstrap.Modal(document.getElementById('cartModal'));
    modal.show();
}

async function applyCouponCode() {
    const code = $('#couponCodeInput').val().trim();
    const msgDiv = $('#couponMessage');

    if (!code) {
        msgDiv.html('<span class="text-danger">Please enter a coupon code.</span>');
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/coupons/validate/${encodeURIComponent(code)}`);
        if (!response.ok) {
            msgDiv.html('<span class="text-danger"><i class="fa fa-times-circle"></i> Invalid or expired coupon code.</span>');
            appliedCoupon = null;
            renderCartModal();
            return;
        }

        const coupon = await response.json();
        appliedCoupon = coupon;
        msgDiv.html(`<span class="text-success fw-bold"><i class="fa fa-check-circle"></i> Coupon "${coupon.code}" applied! (${coupon.discountPercentage}% OFF)</span>`);
        renderCartModal();
    } catch (err) {
        msgDiv.html(`<span class="text-danger">Error: ${err.message}</span>`);
    }
}

function renderCartModal() {
    const container = $('#cartItemsContainer');
    const totalElement = $('#cartTotalPrice');
    const discountBadge = $('#discountBadge');
    const submitBtn = $('#submitOrderBtn');

    if (cart.length === 0) {
        container.html('<p class="text-muted text-center py-4">Your cart is currently empty.</p>');
        totalElement.text('$0.00');
        discountBadge.addClass('d-none');
        submitBtn.prop('disabled', true);
        return;
    }

    submitBtn.prop('disabled', false);
    let subtotalTotal = 0;

    let html = `
        <div class="text-primary font-weight-bold mb-2"><i class="fa fa-store me-1"></i> Restaurant: <strong>${cart[0].restaurantName}</strong></div>
        <table class="table table-hover align-middle">
            <thead class="table-light">
                <tr>
                    <th>Item</th>
                    <th>Price</th>
                    <th class="text-center">Quantity</th>
                    <th class="text-end">Subtotal</th>
                </tr>
            </thead>
            <tbody>
    `;

    cart.forEach(item => {
        const subtotal = item.price * item.quantity;
        subtotalTotal += subtotal;
        html += `
            <tr>
                <td class="fw-bold">${item.name}</td>
                <td>$${item.price.toFixed(2)}</td>
                <td class="text-center">
                    <div class="btn-group btn-group-sm" role="group">
                        <button type="button" class="btn btn-outline-secondary py-0 px-2" onclick="updateCartQuantity(${item.foodId}, -1)">-</button>
                        <span class="btn btn-light py-0 px-3 disabled text-dark fw-bold">${item.quantity}</span>
                        <button type="button" class="btn btn-outline-secondary py-0 px-2" onclick="updateCartQuantity(${item.foodId}, 1)">+</button>
                    </div>
                </td>
                <td class="text-end fw-bold">$${subtotal.toFixed(2)}</td>
            </tr>
        `;
    });

    html += `</tbody></table>`;

    let finalTotal = subtotalTotal;
    if (appliedCoupon) {
        let discount = (subtotalTotal * appliedCoupon.discountPercentage) / 100;
        if (discount > appliedCoupon.maxDiscountAmount) {
            discount = appliedCoupon.maxDiscountAmount;
        }
        finalTotal = subtotalTotal - discount;
        discountBadge.removeClass('d-none').text(`-${appliedCoupon.discountPercentage}% OFF (-$${discount.toFixed(2)})`);
    } else {
        discountBadge.addClass('d-none');
    }

    container.html(html);
    totalElement.text(`$${finalTotal.toFixed(2)}`);
}

// 5. Submit Order to Backend API
async function submitCartOrder() {
    const user = getCurrentUser();
    if (!user) {
        const modalElement = document.getElementById('cartModal');
        const modal = bootstrap.Modal.getInstance(modalElement);
        if (modal) modal.hide();
        openAuthModal("Please login or create an account to complete your order.");
        return;
    }

    if (cart.length === 0) return;

    const paymentStatus = $('#cartPaymentStatus').val();
    const restaurantId = cart[0].restaurantId;

    const payload = {
        userID: user.userID,
        restaurantID: restaurantId,
        paymentStatus: paymentStatus,
        items: cart.map(item => ({
            foodID: item.foodId,
            quantity: item.quantity
        }))
    };

    const submitBtn = $('#submitOrderBtn');
    submitBtn.prop('disabled', true).text('Processing Order...');

    try {
        const response = await fetch(`${API_BASE}/orders`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            const errData = await response.json();
            throw new Error(errData.message || 'Failed to place order.');
        }

        const result = await response.json();

        // Clear cart
        cart = [];
        appliedCoupon = null;
        updateCartUI();

        // Hide cart modal
        const modalElement = document.getElementById('cartModal');
        const modal = bootstrap.Modal.getInstance(modalElement);
        if (modal) modal.hide();

        showNotification(`🎉 Order #${result.orderID} placed successfully!`);

        // Open tracker modal automatically
        $('#trackOrderIdInput').val(result.orderID);
        openTrackerModal();
        fetchOrderStatus();
    } catch (err) {
        alert(`Error placing order: ${err.message}`);
    } finally {
        submitBtn.prop('disabled', false).text('Place Order Now');
    }
}

// 6. Connect Table Booking / Reservation Forms to SQL Server Backend
function setupBookingForms() {
    $('form').each(function () {
        const form = $(this);
        const submitBtn = form.find('button[type="submit"]');
        const btnText = submitBtn.text().trim().toLowerCase();

        if (btnText.includes('book') || btnText.includes('reserve')) {
            form.off('submit').on('submit', async function (e) {
                e.preventDefault();

                const user = getCurrentUser();
                if (!user) {
                    openAuthModal("Please login or register to complete table reservation.");
                    return;
                }

                const nameInput = form.find('#name, input[placeholder*="Name"]').val() || user.name;
                const emailInput = form.find('#email, input[placeholder*="Email"]').val() || user.email;
                const dateTimeInput = form.find('#datetime, input[placeholder*="Date"]').val();
                const peopleInput = parseInt(form.find('#select1, select[id*="People"]').val()) || 2;
                const restSelect = form.find('#selectRestaurant, #select1').val();
                const restaurantId = parseInt(restSelect) || 1;
                const specialReq = form.find('#message, textarea').val();

                submitBtn.prop('disabled', true).text('Submitting Reservation...');

                const reservationPayload = {
                    userID: user.userID,
                    restaurantID: restaurantId,
                    customerName: nameInput,
                    customerEmail: emailInput,
                    reservationDate: dateTimeInput ? new Date(dateTimeInput).toISOString() : new Date().toISOString(),
                    guestCount: peopleInput,
                    specialRequest: specialReq || "Online Table Reservation",
                    status: "Confirmed"
                };

                try {
                    const response = await fetch(`${API_BASE}/reservations`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify(reservationPayload)
                    });

                    if (!response.ok) {
                        const err = await response.json();
                        throw new Error(err.message || "Failed to confirm table booking.");
                    }

                    const resData = await response.json();

                    // Render Reservation Details in Modal
                    $('#reservationModalBody').html(`
                        <div class="display-6 text-success mb-3"><i class="fa fa-calendar-check"></i></div>
                        <h4 class="text-dark">Reservation #${resData.reservationID}</h4>
                        <p class="text-muted mb-2">Thank you, <strong>${escapeHtml(resData.customerName)}</strong>!</p>
                        <div class="card bg-light p-3 border-0 my-3 text-start">
                            <p class="mb-1"><strong>Email:</strong> ${escapeHtml(resData.customerEmail)}</p>
                            <p class="mb-1"><strong>Guests:</strong> ${resData.guestCount} Person(s)</p>
                            <p class="mb-1"><strong>Status:</strong> <span class="badge bg-success">${resData.status}</span></p>
                            <p class="mb-0"><strong>Special Request:</strong> ${escapeHtml(resData.specialRequest || 'None')}</p>
                        </div>
                        <p class="small text-muted mb-0">Your table has been reserved in our SQL Server database system.</p>
                    `);

                    form[0].reset();

                    const modal = new bootstrap.Modal(document.getElementById('reservationModal'));
                    modal.show();

                } catch (err) {
                    alert(`Booking Error: ${err.message}`);
                } finally {
                    submitBtn.prop('disabled', false).text('Book Now');
                }
            });
        }
    });
}

// 7. Footer Newsletter Subscription Button
function setupNewsletterForms() {
    $('.footer button').each(function () {
        const btn = $(this);
        if (btn.text().toLowerCase().includes('signup') || btn.text().toLowerCase().includes('subscribe')) {
            btn.on('click', function (e) {
                e.preventDefault();
                const emailInput = btn.siblings('input');
                const email = emailInput.val() ? emailInput.val().trim() : '';

                if (!email || !email.includes('@')) {
                    alert('Please enter a valid email address.');
                    return;
                }

                showNotification(`📧 Thank you for subscribing to Dalabat Newsletter (${email})!`);
                emailInput.val('');
            });
        }
    });
}

// 8. Order Status Tracker
function openTrackerModal() {
    const modal = new bootstrap.Modal(document.getElementById('trackerModal'));
    modal.show();
}

async function fetchOrderStatus() {
    const orderId = $('#trackOrderIdInput').val();
    const resultDiv = $('#orderStatusResult');

    if (!orderId) {
        resultDiv.html('<div class="alert alert-warning">Please enter a valid Order ID.</div>');
        return;
    }

    resultDiv.html('<div class="text-center text-primary"><div class="spinner-border spinner-border-sm me-2"></div> Fetching order status...</div>');

    try {
        const response = await fetch(`${API_BASE}/orders/${orderId}`);
        if (!response.ok) {
            resultDiv.html(`<div class="alert alert-danger">Order #${orderId} not found in database.</div>`);
            return;
        }

        const order = await response.json();

        let itemsHtml = order.items.map(i => `<li>${i.quantity}x ${i.foodName} ($${i.price.toFixed(2)})</li>`).join('');
        let progressHtml = order.progressHistory.map(p => `
            <li class="list-group-item d-flex justify-content-between align-items-center">
                <div>
                    <strong class="text-primary">${p.status}</strong>: ${p.description || ''}
                </div>
                <small class="text-muted">${new Date(p.updatedAt).toLocaleTimeString()}</small>
            </li>
        `).join('');

        resultDiv.html(`
            <div class="card border-primary">
                <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                    <span>Order #${order.orderID}</span>
                    <span class="badge bg-light text-dark fw-bold">${order.orderStatus}</span>
                </div>
                <div class="card-body">
                    <p class="mb-1"><strong>Customer:</strong> ${order.userName}</p>
                    <p class="mb-1"><strong>Restaurant:</strong> ${order.restaurantName}</p>
                    <p class="mb-1"><strong>Total Amount:</strong> $${order.totalAmount.toFixed(2)} (${order.paymentStatus})</p>
                    <h6 class="mt-3">Items Ordered:</h6>
                    <ul class="mb-3">${itemsHtml}</ul>
                    <h6>Status Timeline:</h6>
                    <ul class="list-group list-group-flush">${progressHtml}</ul>
                </div>
            </div>
        `);
    } catch (err) {
        resultDiv.html(`<div class="alert alert-danger">Error: ${err.message}</div>`);
    }
}

// 9. Reviews & Rating System
function setupReviewSystem() {
    if (!$('#reviewModal').length) {
        const reviewModalHtml = `
        <div class="modal fade" id="reviewModal" tabindex="-1" aria-labelledby="reviewModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header bg-dark text-white">
                        <h5 class="modal-title text-warning" id="reviewModalLabel"><i class="fa fa-star me-2"></i> Write a Customer Review</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <form id="reviewForm">
                            <div class="mb-3">
                                <label class="form-label fw-bold">Select Restaurant</label>
                                <select class="form-select" id="reviewRestaurantSelect" required></select>
                            </div>
                            <div class="mb-3">
                                <label class="form-label fw-bold">Rating (1 to 5 Stars)</label>
                                <select class="form-select" id="reviewRatingSelect">
                                    <option value="5">⭐⭐⭐⭐⭐ 5 Stars (Excellent)</option>
                                    <option value="4">⭐⭐⭐⭐ 4 Stars (Very Good)</option>
                                    <option value="3">⭐⭐⭐ 3 Stars (Average)</option>
                                    <option value="2">⭐⭐ 2 Stars (Poor)</option>
                                    <option value="1">⭐ 1 Star (Terrible)</option>
                                </select>
                            </div>
                            <div class="mb-3">
                                <label class="form-label fw-bold">Your Review Comment</label>
                                <textarea class="form-control" id="reviewCommentInput" rows="3" placeholder="Share your dining experience..."></textarea>
                            </div>
                            <button type="submit" class="btn btn-warning w-100 fw-bold">Submit Review</button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
        `;
        $('body').append(reviewModalHtml);
    }

    $(document).on('submit', '#reviewForm', async function (e) {
        e.preventDefault();

        const user = getCurrentUser();
        if (!user) {
            const modalElement = document.getElementById('reviewModal');
            const modal = bootstrap.Modal.getInstance(modalElement);
            if (modal) modal.hide();
            openAuthModal("Please login or create an account to write a review.");
            return;
        }

        const restId = parseInt($('#reviewRestaurantSelect').val()) || 1;
        const rating = parseInt($('#reviewRatingSelect').val()) || 5;
        const comment = $('#reviewCommentInput').val();

        const payload = {
            userID: user.userID,
            restaurantID: restId,
            rating: rating,
            comment: comment
        };

        try {
            const response = await fetch(`${API_BASE}/reviews`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            if (!response.ok) throw new Error("Failed to submit review.");

            const modal = bootstrap.Modal.getInstance(document.getElementById('reviewModal'));
            if (modal) modal.hide();

            showNotification("⭐ Thank you! Your review has been saved.");
            $('#reviewForm')[0].reset();
            loadReviews();
        } catch (err) {
            alert(`Review Error: ${err.message}`);
        }
    });
}

function openReviewModal() {
    const modal = new bootstrap.Modal(document.getElementById('reviewModal'));
    modal.show();
}

// 10. Load and Render Dynamic Reviews from Database
async function loadReviews() {
    const container = $('#reviewsListContainer');
    if (!container.length) return;

    try {
        const response = await fetch(`${API_BASE}/reviews`);
        if (!response.ok) throw new Error("Could not load reviews.");
        const reviews = await response.json();

        if (!reviews || reviews.length === 0) {
            container.html(`
                <div class="col-12 text-center text-muted py-4">
                    <p>No customer reviews yet. Be the first to leave a review!</p>
                </div>
            `);
            return;
        }

        let html = '';
        reviews.forEach(r => {
            const starCount = Math.min(Math.max(parseInt(r.rating) || 5, 1), 5);
            const stars = '⭐'.repeat(starCount);
            const dateStr = r.createdAt ? new Date(r.createdAt).toLocaleDateString() : 'Recently';
            const userInitial = (r.userName || 'Diner').charAt(0).toUpperCase();

            html += `
                <div class="col-lg-4 col-md-6 mb-4">
                    <div class="card bg-dark text-white border-start border-4 border-warning rounded shadow-sm h-100 p-4">
                        <div class="d-flex align-items-center mb-3">
                            <div class="rounded-circle bg-warning text-dark fw-bold d-flex align-items-center justify-content-center me-3" style="width: 45px; height: 45px; font-size: 1.2rem;">
                                ${userInitial}
                            </div>
                            <div>
                                <h6 class="text-white mb-0">${escapeHtml(r.userName || 'Valued Diner')}</h6>
                                <small class="text-warning">${stars} (${starCount}/5)</small>
                            </div>
                        </div>
                        <p class="text-light fst-italic mb-3">"${escapeHtml(r.comment || 'Great experience!')}"</p>
                        <div class="mt-auto d-flex justify-content-between align-items-center pt-2 border-top border-secondary">
                            <small class="text-warning fw-bold"><i class="fa fa-utensils me-1"></i>${escapeHtml(r.restaurantName || 'Dalabat Restaurant')}</small>
                            <small class="text-muted">${dateStr}</small>
                        </div>
                    </div>
                </div>
            `;
        });

        container.html(html);
    } catch (err) {
        container.html(`<div class="col-12 text-center text-muted">Unable to load customer reviews at this time.</div>`);
    }
}

// Utility Toast & Escaping
function showNotification(msg) {
    const toastHtml = `
        <div class="position-fixed bottom-0 end-0 p-3" style="z-index: 9999">
            <div class="toast align-items-center text-white bg-primary border-0 show" role="alert">
                <div class="d-flex">
                    <div class="toast-body">
                        <i class="fa fa-info-circle me-2"></i> ${msg}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        </div>
    `;
    const $toast = $(toastHtml);
    $('body').append($toast);
    setTimeout(() => { $toast.fadeOut(500, () => $toast.remove()); }, 3000);
}

function escapeHtml(text) {
    if (!text) return '';
    return text.replace(/'/g, "\\'").replace(/"/g, "&quot;");
}
