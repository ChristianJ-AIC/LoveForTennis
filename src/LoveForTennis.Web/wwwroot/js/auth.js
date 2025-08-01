// Authentication JavaScript functionality
let currentUser = null;
const baseUrl = ''; // Use web application base URL

// Initialize authentication state on page load
$(function () {
    checkAuthenticationStatus();
    setupEventHandlers();
    setupNavigationHandlers();
});

// Setup navigation event handlers for browser back/forward buttons
function setupNavigationHandlers() {
    // Handle browser back/forward navigation
    window.addEventListener('popstate', function(event) {
        // Re-check authentication status when navigating with browser buttons
        checkAuthenticationStatus();
    });
    
    // Handle page visibility changes (when returning to tab)
    document.addEventListener('visibilitychange', function() {
        if (!document.hidden) {
            // Re-check authentication status when tab becomes visible
            checkAuthenticationStatus();
        }
    });
    
    // Handle page show event (for browser back/forward cache)
    window.addEventListener('pageshow', function(event) {
        // Re-check authentication status when page is shown from cache
        checkAuthenticationStatus();
    });
}

// Setup event handlers for forms and modals
function setupEventHandlers() {
    // Login form submission
    $('#loginForm').on('submit', function(e) {
        e.preventDefault();
        handleLogin();
    });

    // Register form submission
    $('#registerForm').on('submit', function(e) {
        e.preventDefault();
        handleRegister();
    });

    // Forgot password form submission
    $('#forgotPasswordForm').on('submit', function(e) {
        e.preventDefault();
        handleForgotPassword();
    });

    // Reset password form submission
    $('#resetPasswordForm').on('submit', function(e) {
        e.preventDefault();
        handleResetPassword();
    });

    // Logout link
    $('#logout-link').on('click', function(e) {
        e.preventDefault();
        handleLogout();
    });

    // Password confirmation validation
    $('#registerConfirmPassword, #confirmNewPassword').on('input', function() {
        validatePasswordConfirmation(this);
    });
}

// Check current authentication status
async function checkAuthenticationStatus() {
    try {
        const response = await fetch(`${baseUrl}/Account/ProfileStatus`, {
            method: 'GET',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const result = await response.json();
            if (result.authenticated && result.user) {
                currentUser = result.user;
                updateUIForAuthenticatedUser();
            } else {
                currentUser = null;
                updateUIForUnauthenticatedUser();
            }
        } else {
            // Error checking status - assume unauthenticated
            currentUser = null;
            updateUIForUnauthenticatedUser();
        }
    } catch (error) {
        console.error('Error checking authentication status:', error);
        // Network error - assume unauthenticated for safety
        currentUser = null;
        updateUIForUnauthenticatedUser();
    }
}

// Update UI for authenticated user
function updateUIForAuthenticatedUser() {
    // Hide login/register menu items
    $('#login-menu, #register-menu').addClass('d-none');
    
    // Show authenticated menu items
    $('#profile-menu, #payment-menu, #signups-menu, #logout-menu, #logout-menu-item').removeClass('d-none');
}

// Update UI for unauthenticated user
function updateUIForUnauthenticatedUser() {
    // Show login/register menu items
    $('#login-menu, #register-menu').removeClass('d-none');
    
    // Hide authenticated menu items
    $('#profile-menu, #payment-menu, #signups-menu, #logout-menu, #logout-menu-item').addClass('d-none');
}

// Handle login form submission
async function handleLogin() {
    const email = $('#loginEmail').val();
    const password = $('#loginPassword').val();
    const rememberMe = $('#rememberMe').is(':checked');

    // Clear previous errors
    clearErrors('login');

    // Client-side validation
    if (!validateEmail(email)) {
        showFieldError('loginEmail', 'Please enter a valid email address.');
        return;
    }
    if (!password) {
        showFieldError('loginPassword', 'Password is required.');
        return;
    }

    try {
        const response = await fetch(`${baseUrl}/Account/Login`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                email: email,
                password: password,
                rememberMe: rememberMe
            })
        });

        const result = await response.json();

        if (result.success) {
            currentUser = result.user;
            updateUIForAuthenticatedUser();
            hideModal('loginModal');
            showSuccessMessage('login', 'Login successful!');
            $('#loginForm')[0].reset();
            // No need to reload - UI should update properly
        } else {
            showErrorMessage('login', result.message);
        }
    } catch (error) {
        console.error('Login error:', error);
        showErrorMessage('login', 'An error occurred during login. Please try again.');
    }
}

// Handle register form submission
async function handleRegister() {
    const firstName = $('#registerFirstName').val();
    const lastName = $('#registerLastName').val();
    const email = $('#registerEmail').val();
    const password = $('#registerPassword').val();
    const confirmPassword = $('#registerConfirmPassword').val();

    // Clear previous errors
    clearErrors('register');

    // Client-side validation
    if (!firstName.trim()) {
        showFieldError('registerFirstName', 'First name is required.');
        return;
    }
    if (!lastName.trim()) {
        showFieldError('registerLastName', 'Last name is required.');
        return;
    }
    if (!validateEmail(email)) {
        showFieldError('registerEmail', 'Please enter a valid email address.');
        return;
    }
    if (password.length < 8) {
        showFieldError('registerPassword', 'Password must be at least 8 characters long.');
        return;
    }
    if (password !== confirmPassword) {
        showFieldError('registerConfirmPassword', 'Passwords do not match.');
        return;
    }

    try {
        const response = await fetch(`${baseUrl}/Account/Register`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                firstName: firstName,
                lastName: lastName,
                email: email,
                password: password,
                confirmPassword: confirmPassword
            })
        });

        const result = await response.json();

        if (result.success) {
            showSuccessMessage('register', 'Registration successful! You can now login.');
            $('#registerForm')[0].reset();
            setTimeout(() => showLoginModal(), 2000);
        } else {
            showErrorMessage('register', result.message);
        }
    } catch (error) {
        console.error('Registration error:', error);
        showErrorMessage('register', 'An error occurred during registration. Please try again.');
    }
}

// Handle forgot password form submission
async function handleForgotPassword() {
    const email = $('#forgotPasswordEmail').val();

    // Clear previous errors
    clearErrors('forgotPassword');

    // Client-side validation
    if (!validateEmail(email)) {
        showFieldError('forgotPasswordEmail', 'Please enter a valid email address.');
        return;
    }

    try {
        const response = await fetch(`${baseUrl}/Account/ForgotPassword`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                email: email
            })
        });

        const result = await response.json();

        if (result.success) {
            showSuccessMessage('forgotPassword', result.message);
            $('#forgotPasswordForm')[0].reset();
            
            // For demo purposes, if a token is returned, show reset password modal
            if (result.token) {
                setTimeout(() => {
                    $('#resetEmail').val(email);
                    $('#resetToken').val(result.token);
                    showResetPasswordModal();
                }, 2000);
            }
        } else {
            showErrorMessage('forgotPassword', result.message);
        }
    } catch (error) {
        console.error('Forgot password error:', error);
        showErrorMessage('forgotPassword', 'An error occurred while processing your request. Please try again.');
    }
}

// Handle reset password form submission
async function handleResetPassword() {
    const email = $('#resetEmail').val();
    const token = $('#resetToken').val();
    const newPassword = $('#newPassword').val();
    const confirmPassword = $('#confirmNewPassword').val();

    // Clear previous errors
    clearErrors('resetPassword');

    // Client-side validation
    if (newPassword.length < 8) {
        showFieldError('newPassword', 'Password must be at least 8 characters long.');
        return;
    }
    if (newPassword !== confirmPassword) {
        showFieldError('confirmNewPassword', 'Passwords do not match.');
        return;
    }

    try {
        const response = await fetch(`${baseUrl}/Account/ResetPassword`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                email: email,
                token: token,
                newPassword: newPassword,
                confirmPassword: confirmPassword
            })
        });

        const result = await response.json();

        if (result.success) {
            showSuccessMessage('resetPassword', 'Password reset successful! You can now login with your new password.');
            $('#resetPasswordForm')[0].reset();
            setTimeout(() => showLoginModal(), 2000);
        } else {
            showErrorMessage('resetPassword', result.message);
        }
    } catch (error) {
        console.error('Reset password error:', error);
        showErrorMessage('resetPassword', 'An error occurred while resetting your password. Please try again.');
    }
}

// Handle logout
async function handleLogout() {
    try {
        // Get CSRF token from the page
        const csrfToken = $('input[name="__RequestVerificationToken"]').val();
        
        if (!csrfToken) {
            console.warn('CSRF token not found, falling back to page reload');
            currentUser = null;
            updateUIForUnauthenticatedUser();
            location.reload();
            return;
        }

        // Make the logout request with proper CSRF token
        const response = await fetch('/Account/Logout', {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: new URLSearchParams({
                '__RequestVerificationToken': csrfToken
            })
        });

        if (response.ok) {
            // Successful logout - update UI without page reload
            currentUser = null;
            
            // Hide any open modals
            hideAllModals();
            
            // Check authentication status to update UI properly
            await checkAuthenticationStatus();
            
            // Show confirmation message
            if ($('#logoutSuccess').length === 0) {
                $('body').prepend('<div id="logoutSuccess" class="alert alert-success alert-dismissible fade show position-fixed" style="top: 20px; right: 20px; z-index: 9999;" role="alert"><i class="fas fa-check-circle me-2"></i>Successfully logged out!<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                // Auto-hide after 3 seconds
                setTimeout(() => $('#logoutSuccess').alert('close'), 3000);
            }
        } else {
            // Logout failed on server - fallback to page reload
            console.warn('Logout response not OK, falling back to page reload');
            currentUser = null;
            updateUIForUnauthenticatedUser();
            location.reload();
        }
    } catch (error) {
        console.error('Logout error:', error);
        // Network error - fallback to page reload for safety
        currentUser = null;
        updateUIForUnauthenticatedUser();
        location.reload();
    }
}

// Modal navigation functions
function showLoginModal() {
    hideAllModals();
    new bootstrap.Modal(document.getElementById('loginModal')).show();
}

function showRegisterModal() {
    hideAllModals();
    new bootstrap.Modal(document.getElementById('registerModal')).show();
}

function showForgotPasswordModal() {
    hideAllModals();
    new bootstrap.Modal(document.getElementById('forgotPasswordModal')).show();
}

function showResetPasswordModal() {
    hideAllModals();
    new bootstrap.Modal(document.getElementById('resetPasswordModal')).show();
}

function hideAllModals() {
    const modals = ['loginModal', 'registerModal', 'forgotPasswordModal', 'resetPasswordModal'];
    modals.forEach(modalId => hideModal(modalId));
}

function hideModal(modalId) {
    const modalElement = document.getElementById(modalId);
    const modal = bootstrap.Modal.getInstance(modalElement);
    if (modal) {
        modal.hide();
    }
}

// Utility functions
function validateEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function validatePasswordConfirmation(element) {
    const isRegisterForm = element.id === 'registerConfirmPassword';
    const passwordId = isRegisterForm ? 'registerPassword' : 'newPassword';
    const password = $(`#${passwordId}`).val();
    const confirmPassword = $(element).val();

    if (confirmPassword && password !== confirmPassword) {
        showFieldError(element.id, 'Passwords do not match.');
    } else {
        clearFieldError(element.id);
    }
}

function showFieldError(fieldId, message) {
    const field = $(`#${fieldId}`);
    field.addClass('is-invalid');
    field.siblings('.invalid-feedback').text(message);
}

function clearFieldError(fieldId) {
    const field = $(`#${fieldId}`);
    field.removeClass('is-invalid');
    field.siblings('.invalid-feedback').text('');
}

function clearErrors(formType) {
    $(`#${formType}Modal .is-invalid`).removeClass('is-invalid');
    $(`#${formType}Modal .invalid-feedback`).text('');
    $(`#${formType}Error, #${formType}Success`).addClass('d-none');
}

function showErrorMessage(formType, message) {
    $(`#${formType}Error`).text(message).removeClass('d-none');
    $(`#${formType}Success`).addClass('d-none');
}

function showSuccessMessage(formType, message) {
    $(`#${formType}Success`).text(message).removeClass('d-none');
    $(`#${formType}Error`).addClass('d-none');
}