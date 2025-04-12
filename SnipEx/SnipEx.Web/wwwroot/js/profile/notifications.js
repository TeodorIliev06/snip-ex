document.addEventListener('DOMContentLoaded', function () {
    // Filter buttons functionality
    const filterButtons = document.querySelectorAll('.filter-button');
    const notificationItems = document.querySelectorAll('.notification-item');

    filterButtons.forEach(button => {
        button.addEventListener('click', function () {
            // Remove active class from all buttons
            filterButtons.forEach(btn => btn.classList.remove('active'));

            // Add active class to clicked button
            this.classList.add('active');

            const filter = this.getAttribute('data-filter');

            // Show/hide notifications based on filter
            notificationItems.forEach(item => {
                if (filter === 'all') {
                    item.style.display = 'flex';
                } else {
                    item.classList.contains(filter) ?
                        item.style.display = 'flex' :
                        item.style.display = 'none';
                }
            });
        });
    });

    // Load more functionality
    const loadMoreBtn = document.getElementById('load-more-btn');
    if (loadMoreBtn) {
        let page = 1;

        loadMoreBtn.addEventListener('click', function () {
            page++;

            // Show loading state
            this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Loading...';
            this.disabled = true;

            // Simulate loading more notifications with AJAX
            fetch(`/User/GetMoreNotifications?page=${page}`, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.notifications && data.notifications.length > 0) {
                        // Append new notifications
                        const notificationsList = document.querySelector('.notifications-list');

                        data.notifications.forEach(notification => {
                            const notificationElement = createNotificationElement(notification);
                            notificationsList.appendChild(notificationElement);
                        });

                        // Reset button state
                        loadMoreBtn.innerHTML = 'Load more';
                        loadMoreBtn.disabled = false;

                        // If no more notifications, hide the button
                        if (data.hasMore === false) {
                            loadMoreBtn.style.display = 'none';
                        }
                    } else {
                        // No more notifications
                        loadMoreBtn.innerHTML = 'No more notifications';
                        loadMoreBtn.disabled = true;
                        setTimeout(() => {
                            loadMoreBtn.style.display = 'none';
                        }, 2000);
                    }
                })
                .catch(error => {
                    console.error('Error loading more notifications:', error);
                    loadMoreBtn.innerHTML = 'Error loading more. Try again';
                    loadMoreBtn.disabled = false;
                });
        });
    }

    function getRelativeTimeString(dateStr) {
        // Parse the date string (assuming format is dd/MM/yyyy)
        const parts = dateStr.split('/');
        if (parts.length !== 3) {
            return dateStr; // Return original if format doesn't match
        }

        // Create date object (month is 0-indexed in JavaScript Date)
        const day = parseInt(parts[0], 10);
        const month = parseInt(parts[1], 10) - 1;
        const year = parseInt(parts[2], 10);

        const date = new Date(year, month, day);
        const now = new Date();

        // Check if date is valid
        if (isNaN(date.getTime())) {
            return dateStr; // Return original if date is invalid
        }

        // Calculate difference in milliseconds
        const diffInMs = now - date;
        const diffInSeconds = Math.floor(diffInMs / 1000);

        if (diffInSeconds < 60) {
            return 'just now';
        }

        const diffInMinutes = Math.floor(diffInSeconds / 60);
        if (diffInMinutes < 60) {
            return `${diffInMinutes} minute${diffInMinutes !== 1 ? 's' : ''} ago`;
        }

        const diffInHours = Math.floor(diffInMinutes / 60);
        if (diffInHours < 24) {
            return `${diffInHours} hour${diffInHours !== 1 ? 's' : ''} ago`;
        }

        const diffInDays = Math.floor(diffInHours / 24);
        if (diffInDays < 30) {
            return `${diffInDays} day${diffInDays !== 1 ? 's' : ''} ago`;
        }

        const diffInMonths = Math.floor(diffInDays / 30);
        if (diffInMonths < 12) {
            return `${diffInMonths} month${diffInMonths !== 1 ? 's' : ''} ago`;
        }

        const diffInYears = Math.floor(diffInMonths / 12);
        return `${diffInYears} year${diffInYears !== 1 ? 's' : ''} ago`;
    }

    // Function to create a notification element
    function createNotificationElement(notification) {
        const div = document.createElement('div');
        div.className = `notification-item ${notification.cssType}`;

        // Create avatar
        const avatarDiv = document.createElement('div');
        avatarDiv.className = 'notification-avatar';
        const img = document.createElement('img');
        img.src = notification.senderAvatar || '/images/profile_pics/default_user1.png';
        img.alt = notification.senderUsername;
        img.onerror = function () { this.src = '/images/profile_pics/default_user1.png'; };
        avatarDiv.appendChild(img);

        // Create content
        const contentDiv = document.createElement('div');
        contentDiv.className = 'notification-content';

        const textDiv = document.createElement('div');
        textDiv.className = 'notification-text';
        textDiv.innerHTML = `<strong>${notification.senderUsername}</strong> ${notification.message}`;

        const metaDiv = document.createElement('div');
        metaDiv.className = 'notification-meta';
        metaDiv.innerHTML = `<span class="notification-time">${getRelativeTimeString(notification.createdAt)}</span>`;

        if (notification.relatedLink) {
            const link = document.createElement('a');
            link.href = notification.relatedLink;
            link.className = 'notification-link';
            link.textContent = 'View';
            metaDiv.appendChild(link);
        }

        contentDiv.appendChild(textDiv);
        contentDiv.appendChild(metaDiv);

        // Create unread badge if needed
        if (!notification.isRead) {
            const badgeDiv = document.createElement('div');
            badgeDiv.className = 'notification-badge';
            const unreadSpan = document.createElement('span');
            unreadSpan.className = 'unread-indicator';
            badgeDiv.appendChild(unreadSpan);
            div.appendChild(badgeDiv);
        }

        // Assemble the notification item
        div.appendChild(avatarDiv);
        div.appendChild(contentDiv);

        return div;
    }

    // Mark notifications as read when they're visible
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const notificationItem = entry.target;
                const unreadBadge = notificationItem.querySelector('.notification-badge');

                if (unreadBadge) {
                    // Get notification ID
                    const notificationId = notificationItem.getAttribute('data-id');

                    // Call API to mark as read
                    fetch(`/User/MarkNotificationAsRead/${notificationId}`, {
                        method: 'POST',
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        }
                    })
                        .then(() => {
                            // Remove the unread badge with animation
                            unreadBadge.style.opacity = '0';
                            setTimeout(() => {
                                unreadBadge.remove();
                            }, 300);
                        })
                        .catch(error => console.error('Error marking notification as read:', error));
                }

                // Stop observing this notification
                observer.unobserve(notificationItem);
            }
        });
    }, { threshold: 0.5 });

    // Start observing all unread notifications
    document.querySelectorAll('.notification-item:has(.notification-badge)').forEach(item => {
        observer.observe(item);
    });
});