function getSenderUsernameFromNotification(notificationTextElement) {
    if (!notificationTextElement) return;

    const originalText = notificationTextElement.textContent.trim();
    const messageParts = originalText.split(' ');
    const firstWord = messageParts.shift();
    const restOfMessage = messageParts.join(' ');

    notificationTextElement.innerHTML = `<strong>${firstWord}</strong> ${restOfMessage}`;
}

function createNotificationElement(notification) {
    const div = document.createElement('div');
    div.className = `notification-item ${notification.cssType}`;
    div.setAttribute('data-id', notification.id);

    const avatarDiv = document.createElement('div');
    avatarDiv.className = 'notification-avatar';
    const img = document.createElement('img');
    img.src = notification.actorAvatar || '/images/profile_pics/default_user1.png';
    img.alt = 'User avatar';
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

    const timeSpan = document.createElement('span');
    timeSpan.className = 'notification-time';
    timeSpan.setAttribute('data-timestamp', notification.createdAt);
    timeSpan.textContent = getRelativeTimeString(notification.createdAt);
    metaDiv.appendChild(timeSpan);

    if (notification.relatedLink) {
        const link = document.createElement('a');
        link.href = notification.relatedLink;
        link.className = 'notification-link';
        link.textContent = 'View';
        metaDiv.appendChild(link);
    }

    contentDiv.appendChild(textDiv);
    contentDiv.appendChild(metaDiv);

    if (!notification.isRead) {
        const badgeDiv = document.createElement('div');
        badgeDiv.className = 'notification-badge';
        const unreadSpan = document.createElement('span');
        unreadSpan.className = 'unread-indicator';
        badgeDiv.appendChild(unreadSpan);
        div.appendChild(badgeDiv);
    }

    div.appendChild(avatarDiv);
    div.appendChild(contentDiv);

    return div;
}

document.addEventListener('DOMContentLoaded', function () {
    const filterButtons = document.querySelectorAll('.filter-button');
    const notificationItems = document.querySelectorAll('.notification-item');

    const notificationMessages = document.querySelectorAll('.notification-text');
    notificationMessages.forEach(message => {
        getSenderUsernameFromNotification(message);
    });

    const timestamps = document.querySelectorAll('.notification-time');
    timestamps.forEach(dateSpan => {
        const timestampStr = dateSpan.getAttribute('data-timestamp');
        dateSpan.textContent = getRelativeTimeString(timestampStr);
    });
});