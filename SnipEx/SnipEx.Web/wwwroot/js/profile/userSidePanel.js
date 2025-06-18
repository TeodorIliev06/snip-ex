document.addEventListener('DOMContentLoaded', function () {
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.user-navigation a, .user-navigation .logout-link');

    navLinks.forEach(link => {
        const href = link.getAttribute('href') || (link.closest('form') ? link.closest('form').getAttribute('action') : null);
        if (href && currentPath.includes(href)) {
            link.classList.add('active');
        }
    });

    updateNotificationCount();

    function getAccessToken() {
        const match = document.cookie.match(/(^| )JwtToken=([^;]+)/);
        const token = match ? match[2] : null;
        return token;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7000/notificationHub", {
            accessTokenFactory: () => {
                const token = getAccessToken();
                return token;
            }
        })
        .build();

    connection.on("ReceiveNotification", function (notification) {
        updateNotificationCount();
    });

    connection.start()
        .then(() => {
            console.log("SignalR Connected successfully");
        })
        .catch(err => {
            console.error("SignalR Connection Error:", err);
        });

    const profilePicture = document.getElementById('profileAvatar');
    const fileInput = document.getElementById('fileInput');

    if (profilePicture) {
        loadProfilePicture(profilePicture);
    }

    if (fileInput) {
        fileInput.addEventListener('change', function (e) {
            if (fileInput.files && fileInput.files[0]) {
                // Update preview image immediately
                const reader = new FileReader();
                reader.onload = function (e) {
                    profilePicture.src = e.target.result;
                };
                reader.readAsDataURL(fileInput.files[0]);

                uploadProfilePicture(fileInput.files[0], profilePicture);
            }
        });
    }

    // NEW: Mobile responsive functionality
    // Create mobile toggle button
    function createMobileToggle() {
        const existingToggle = document.querySelector('.mobile-toggle');
        if (existingToggle) return;

        const toggleButton = document.createElement('button');
        toggleButton.className = 'mobile-toggle';
        toggleButton.innerHTML = '<i class="bi bi-list"></i>';
        toggleButton.setAttribute('aria-label', 'Toggle sidebar');

        document.body.appendChild(toggleButton);
        return toggleButton;
    }

    // Create mobile overlay
    function createMobileOverlay() {
        const existingOverlay = document.querySelector('.mobile-overlay');
        if (existingOverlay) return;

        const overlay = document.createElement('div');
        overlay.className = 'mobile-overlay';
        document.body.appendChild(overlay);
        return overlay;
    }

    // Initialize mobile functionality
    function initializeMobileToggle() {
        const sidePanel = document.querySelector('.side-panel');
        if (!sidePanel) return;

        const toggleButton = createMobileToggle();
        const overlay = createMobileOverlay();

        // Toggle sidebar function
        function toggleSidebar() {
            const isOpen = sidePanel.classList.contains('mobile-open');

            if (isOpen) {
                closeSidebar();
            } else {
                openSidebar();
            }
        }

        // Open sidebar
        function openSidebar() {
            sidePanel.classList.add('mobile-open');
            overlay.classList.add('active');
            toggleButton.innerHTML = '<i class="bi bi-x"></i>';
            document.body.style.overflow = 'hidden'; // Prevent background scrolling
        }

        // Close sidebar
        function closeSidebar() {
            sidePanel.classList.remove('mobile-open');
            overlay.classList.remove('active');
            toggleButton.innerHTML = '<i class="bi bi-list"></i>';
            document.body.style.overflow = ''; // Restore scrolling
        }

        // Event listeners
        toggleButton.addEventListener('click', toggleSidebar);
        overlay.addEventListener('click', closeSidebar);

        // Close sidebar on escape key
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && sidePanel.classList.contains('mobile-open')) {
                closeSidebar();
            }
        });

        // Handle window resize
        window.addEventListener('resize', function () {
            // Close sidebar when resizing to desktop size
            if (window.innerWidth > 768 && sidePanel.classList.contains('mobile-open')) {
                closeSidebar();
            }
        });

        // Handle orientation change on mobile devices
        window.addEventListener('orientationchange', function () {
            setTimeout(function () {
                if (window.innerWidth > 768 && sidePanel.classList.contains('mobile-open')) {
                    closeSidebar();
                }
            }, 100);
        });
    }

    // Initialize mobile functionality
    initializeMobileToggle();

    // Handle touch events for better mobile experience
    let touchStartX = 0;
    let touchEndX = 0;

    function handleSwipeGesture() {
        const sidePanel = document.querySelector('.side-panel');
        if (!sidePanel) return;

        const swipeThreshold = 50;
        const swipeDistance = touchEndX - touchStartX;

        // Only handle swipe gestures on mobile
        if (window.innerWidth <= 768) {
            // Swipe right to open (from left edge)
            if (swipeDistance > swipeThreshold && touchStartX < 50) {
                if (!sidePanel.classList.contains('mobile-open')) {
                    sidePanel.classList.add('mobile-open');
                    document.querySelector('.mobile-overlay').classList.add('active');
                    document.querySelector('.mobile-toggle').innerHTML = '<i class="bi bi-x"></i>';
                    document.body.style.overflow = 'hidden';
                }
            }
            // Swipe left to close (when sidebar is open)
            else if (swipeDistance < -swipeThreshold && sidePanel.classList.contains('mobile-open')) {
                sidePanel.classList.remove('mobile-open');
                document.querySelector('.mobile-overlay').classList.remove('active');
                document.querySelector('.mobile-toggle').innerHTML = '<i class="bi bi-list"></i>';
                document.body.style.overflow = '';
            }
        }
    }

    // Add touch event listeners
    document.addEventListener('touchstart', function (e) {
        touchStartX = e.changedTouches[0].screenX;
    });

    document.addEventListener('touchend', function (e) {
        touchEndX = e.changedTouches[0].screenX;
        handleSwipeGesture();
    });

    // Improve accessibility
    function improveAccessibility() {
        const sidePanel = document.querySelector('.side-panel');
        const toggleButton = document.querySelector('.mobile-toggle');

        if (!sidePanel || !toggleButton) return;

        // Add ARIA attributes
        sidePanel.setAttribute('role', 'navigation');
        sidePanel.setAttribute('aria-label', 'User navigation');

        // Add focus management
        toggleButton.addEventListener('click', function () {
            setTimeout(function () {
                if (sidePanel.classList.contains('mobile-open')) {
                    // Focus on first focusable element in sidebar
                    const firstFocusable = sidePanel.querySelector('a, button, input, [tabindex]:not([tabindex="-1"])');
                    if (firstFocusable) {
                        firstFocusable.focus();
                    }
                }
            }, 300); // Wait for animation to complete
        });

        // Trap focus within sidebar when open on mobile
        sidePanel.addEventListener('keydown', function (e) {
            if (window.innerWidth <= 768 && sidePanel.classList.contains('mobile-open')) {
                const focusableElements = sidePanel.querySelectorAll('a, button, input, [tabindex]:not([tabindex="-1"])');
                const firstFocusable = focusableElements[0];
                const lastFocusable = focusableElements[focusableElements.length - 1];

                if (e.key === 'Tab') {
                    if (e.shiftKey) {
                        // Shift + Tab
                        if (document.activeElement === firstFocusable) {
                            e.preventDefault();
                            lastFocusable.focus();
                        }
                    } else {
                        // Tab
                        if (document.activeElement === lastFocusable) {
                            e.preventDefault();
                            firstFocusable.focus();
                        }
                    }
                }
            }
        });
    }

    improveAccessibility();

    function adjustNotificationBadge() {
        const badge = document.querySelector('.notification-badge');
        if (!badge) return;

        if (window.innerWidth <= 480) {
            badge.style.right = '-8px';
            badge.style.top = '-8px';
        } else if (window.innerWidth <= 768) {
            badge.style.right = '-10px';
            badge.style.top = '-6px';
        } else {
            badge.style.right = '-10px';
            badge.style.top = '-6px';
        }
    }

    adjustNotificationBadge();
    window.addEventListener('resize', adjustNotificationBadge);

    // Optimize performance by debouncing resize events
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    // Use debounced resize handler
    const debouncedResize = debounce(function () {
        adjustNotificationBadge();

        const sidePanel = document.querySelector('.side-panel');
        if (window.innerWidth > 768 && sidePanel && sidePanel.classList.contains('mobile-open')) {
            sidePanel.classList.remove('mobile-open');
            document.querySelector('.mobile-overlay').classList.remove('active');
            document.querySelector('.mobile-toggle').innerHTML = '<i class="bi bi-list"></i>';
            document.body.style.overflow = '';
        }
    }, 250);

    window.addEventListener('resize', debouncedResize);
});

function loadProfilePicture(profilePicture) {
    const cachedImageUrl = sessionStorage.getItem('profilePictureUrl');
    const cachedTimestamp = sessionStorage.getItem('profilePictureTimestamp');

    // Use cached image if it's less than 1 hour old
    if (cachedImageUrl && cachedTimestamp && (Date.now() - cachedTimestamp < 3600000)) {
        profilePicture.src = cachedImageUrl;
    }

    fetch('https://localhost:7000/ProfilePictureApi/GetProfilePicture', {
        method: 'GET',
        headers: {
            'Accept': 'image/*',
        },
        cache: 'no-store',
        credentials: "include"
    })
        .then(response => {
            if (response.ok) {
                return response.blob();
            }
            throw new Error('Failed to load profile picture');
        })
        .then(blob => {
            const imageUrl = URL.createObjectURL(blob);
            profilePicture.src = imageUrl;
            profilePicture.width = 120;
            profilePicture.height = 120;

            sessionStorage.setItem('profilePictureUrl', imageUrl);
            sessionStorage.setItem('profilePictureTimestamp', Date.now());
        })
        .catch(error => {
            console.error('Error loading profile picture:', error);
            profilePicture.src = '/images/default-avatar.png';
        });
}

function uploadProfilePicture(file, profilePicture) {
    const formData = new FormData();
    formData.append('file', file);

    fetch('https://localhost:7000/ProfilePictureApi/UploadProfilePicture', {
        method: 'POST',
        body: formData,
        headers: {
            'Accept': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
        credentials: "include"
    })
        .then(response => {
            profilePicture.classList.remove('uploading');

            if (response.ok) {
                return response.json();
            }
            return response.json().then(data => {
                throw new Error(data.message || 'Failed to upload profile picture');
            });
        })
        .then(data => {
            console.log('Success:', data.message);
            loadProfilePicture(profilePicture);
        })
        .catch(error => {
            console.error('Error:', error);
            loadProfilePicture(profilePicture);
        });
}

function updateNotificationCount() {
    const userId = document.getElementById('currentUserId')?.value;
    if (!userId) return;

    fetch('https://localhost:7000/NotificationApi/ShowNewNotification', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
        credentials: 'include'
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const notificationBadge = document.getElementById('notification-count');
            if (notificationBadge) {
                if (data.likesCount > 0) {
                    // Display "9+" if there are more than 9 notifications
                    notificationBadge.textContent = data.likesCount > 9 ? "9+" : data.likesCount;
                    notificationBadge.classList.remove('d-none');
                } else {
                    notificationBadge.classList.add('d-none');
                }
            }
        })
        .catch(error => {
            console.error('Error updating notification count:', error);
        });
}