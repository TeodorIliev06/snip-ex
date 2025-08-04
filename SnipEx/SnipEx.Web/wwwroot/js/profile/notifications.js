function getSenderUsernameFromNotification(notificationTextElement) {
    if (!notificationTextElement) return;

    const originalText = notificationTextElement.textContent.trim();
    const messageParts = originalText.split(' ');
    const firstWord = messageParts.shift();
    const restOfMessage = messageParts.join(' ');

    notificationTextElement.innerHTML = `<strong>${firstWord}</strong> ${restOfMessage}`;
}





// Utility functions for pagination (similar to connections.js)
function setActiveFilterButton(filterButtonSelector = '.filter-button') {
    const urlParams = new URLSearchParams(window.location.search);
    const currentFilter = urlParams.get('filter') || 'all';

    document.querySelectorAll(filterButtonSelector).forEach(btn => {
        btn.classList.remove('active');
    });

    const activeButton = document.querySelector(`${filterButtonSelector}[data-filter="${currentFilter}"]`);
    if (activeButton) {
        activeButton.classList.add('active');
    }
}

function attachFilterListeners(filterButtonSelector = '.filter-button', onFilterChange = null) {
    const filterButtons = document.querySelectorAll(filterButtonSelector);
    filterButtons.forEach(button => {
        button.addEventListener('click', () => {
            const filterValue = button.getAttribute('data-filter');
            navigateToFilter(filterValue);
            if (onFilterChange) {
                onFilterChange(filterValue);
            }
        });
    });
}

function navigateToFilter(filterValue) {
    const url = new URL(window.location);
    url.searchParams.set('filter', filterValue);
    url.searchParams.set('page', '1'); // Reset to first page when filtering
    window.location.href = url.toString();
}

function getCurrentFilter() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('filter') || 'all';
}

function getCurrentPage() {
    const urlParams = new URLSearchParams(window.location.search);
    return parseInt(urlParams.get('page')) || 1;
}

function initializePagination(options = {}) {
    const filterButtonSelector = options.filterButtonSelector || '.filter-button';
    const onFilterChange = options.onFilterChange || null;

    setActiveFilterButton(filterButtonSelector);
    attachFilterListeners(filterButtonSelector, onFilterChange);
}

document.addEventListener('DOMContentLoaded', function () {
    // Initialize pagination and filtering
    initializePagination();

    // Process existing notification messages to format usernames
    const notificationMessages = document.querySelectorAll('.notification-text');
    notificationMessages.forEach(message => {
        getSenderUsernameFromNotification(message);
    });

    // Update timestamps to relative time
    const timestamps = document.querySelectorAll('.notification-time');
    timestamps.forEach(dateSpan => {
        const timestampStr = dateSpan.getAttribute('data-timestamp');
        dateSpan.textContent = getRelativeTimeString(timestampStr);
    });
});