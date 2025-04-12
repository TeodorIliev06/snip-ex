﻿function getRelativeTimeString(dateStr) {
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

function getSenderUsernameFromNotification(notificationTextElement) {
    if (!notificationTextElement) return;

    const originalText = notificationTextElement.textContent.trim();
    const messageParts = originalText.split(' ');
    const firstWord = messageParts.shift();
    const restOfMessage = messageParts.join(' ');

    notificationTextElement.innerHTML = `<strong>${firstWord}</strong> ${restOfMessage}`;
}

document.addEventListener('DOMContentLoaded', function () {
    const filterButtons = document.querySelectorAll('.filter-button');
    const notificationItems = document.querySelectorAll('.notification-item');

    notificationItems.forEach(item => {
        item.addEventListener('click', function () {
            const notificationId = this.getAttribute('data-id');
            const unreadBadge = this.querySelector('.notification-badge');

            if (unreadBadge) {
                fetch(`https://localhost:7000/NotificationApi/MarkAsRead/${notificationId}`, {
                        method: 'PATCH',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        credentials: 'include'
                    })
                    .then(() => {
                        unreadBadge.style.opacity = '0';
                        setTimeout(() => {
                            unreadBadge.remove();
                        }, 300);
                    })
                    .catch(error => console.error('Error marking notification as read:', error));
            }

            const relatedLink = this.querySelector('.notification-link');
            if (relatedLink) {
                window.location.href = relatedLink.getAttribute('href');
            }
        });
    });

    const markAllReadBtn = document.getElementById('mark-all-read');
    if (markAllReadBtn) {
        markAllReadBtn.addEventListener('click', function () {
            fetch('https://localhost:7000/NotificationApi/MarkAllAsRead', {
                    method: 'PATCH',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include'
                })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        document.querySelectorAll('.notification-badge').forEach(badge => {
                            badge.style.opacity = '0';
                            setTimeout(() => {
                                badge.remove();
                            }, 300);
                        });

                        // Hide the mark all as read button
                        markAllReadBtn.style.display = 'none';
                    }
                })
                .catch(error => console.error('Error marking all notifications as read:', error));
        });
    }

    const notificationMessages = document.querySelectorAll('.notification-text');
    notificationMessages.forEach(message => {
        getSenderUsernameFromNotification(message);
    });

    const timestamps = document.querySelectorAll('.notification-time');
    timestamps.forEach(dateSpan => {
        const timestampStr = dateSpan.getAttribute('data-timestamp');
        dateSpan.textContent = getRelativeTimeString(timestampStr);
    });

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
    //const loadMoreBtn = document.getElementById('load-more-btn');
    //if (loadMoreBtn) {
    //    let page = 1;

    //    loadMoreBtn.addEventListener('click', function () {
    //        page++;

    //        // Show loading state
    //        this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Loading...';
    //        this.disabled = true;

    //        // Simulate loading more notifications with AJAX
    //        fetch(`/User/GetMoreNotifications?page=${page}`, {
    //            headers: {
    //                'X-Requested-With': 'XMLHttpRequest'
    //            }
    //        })
    //            .then(response => response.json())
    //            .then(data => {
    //                if (data.notifications && data.notifications.length > 0) {
    //                    // Append new notifications
    //                    const notificationsList = document.querySelector('.notifications-list');

    //                    data.notifications.forEach(notification => {
    //                        const notificationElement = createNotificationElement(notification);
    //                        notificationsList.appendChild(notificationElement);
    //                    });

    //                    // Reset button state
    //                    loadMoreBtn.innerHTML = 'Load more';
    //                    loadMoreBtn.disabled = false;

    //                    // If no more notifications, hide the button
    //                    if (data.hasMore === false) {
    //                        loadMoreBtn.style.display = 'none';
    //                    }
    //                } else {
    //                    // No more notifications
    //                    loadMoreBtn.innerHTML = 'No more notifications';
    //                    loadMoreBtn.disabled = true;
    //                    setTimeout(() => {
    //                        loadMoreBtn.style.display = 'none';
    //                    }, 2000);
    //                }
    //            })
    //            .catch(error => {
    //                console.error('Error loading more notifications:', error);
    //                loadMoreBtn.innerHTML = 'Error loading more. Try again';
    //                loadMoreBtn.disabled = false;
    //            });
    //    });
    //}

    // Function to create a notification element
    function createNotificationElement(notification) {
        const div = document.createElement('div');
        div.className = `notification-item ${notification.cssType}`;

        const avatarDiv = document.createElement('div');
        avatarDiv.className = 'notification-avatar';
        const img = document.createElement('img');
        img.src = notification.senderAvatar || '/images/profile_pics/default_user1.png';
        img.alt = notification.senderUsername;
        img.onerror = function () { this.src = '/images/profile_pics/default_user1.png'; };
        avatarDiv.appendChild(img);

        const contentDiv = document.createElement('div');
        contentDiv.className = 'notification-content';

        const textDiv = document.createElement('div');
        textDiv.className = 'notification-text';

        textDiv.textContent = notification.message;
        getSenderUsernameFromNotification(textDiv);

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
});