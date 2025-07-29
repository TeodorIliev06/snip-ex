async function fetchWithToastr(url, options = {}) {
    try {
        const response = await fetch(url, options);

        if (response.status === 401) {
            window.location.href = '/Identity/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
            return null;
        }

        const data = await response.json();

        if (!response.ok) {
            let errorMessage = "An error occurred.";

            if (data?.errors) {
                // Take the first error we find
                const firstField = Object.keys(data.errors)[0];
                errorMessage = data.errors[firstField][0];
            }
            else if (data?.message) {
                errorMessage = data.message;
            }

            toastr.error(errorMessage, "", { timeOut: 5000, closeButton: true });
            return null;
        }


        return data;
    } catch (error) {
        console.error('Fetch Error:', error);
        toastr.error("Something went wrong. Please try again.", "", { timeOut: 5000, closeButton: true });
        return null;
    }
}

function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]').value;
}

/**
 * Convert a UTC date string to a relative time string in user's local timezone
 */
function getRelativeTimeString(dateStr) {
    if (!dateStr) {
        return 'Unknown';
    }

    let date;

    // Handle ISO 8601 format
    try {
        // If the string doesn't end with 'Z' but is in ISO format, assume it is UTC
        if (dateStr.includes('T') && !dateStr.endsWith('Z') &&
            !dateStr.includes('+') && !dateStr.includes('-', 10)) {
            // Add 'Z' to indicate UTC
            date = new Date(dateStr + 'Z');
        } else {
            // Parse as-is (should handle Z suffix and other timezone indicators)
            date = new Date(dateStr);
        }
    } catch (error) {
        console.warn('Error parsing date:', dateStr, error);
        return dateStr;
    }

    if (isNaN(date.getTime())) {
        console.warn('Invalid date format:', dateStr);
        return dateStr; // Return original string if parsing fails
    }

    const now = new Date();
    const diffInMs = now - date;

    if (diffInMs < 0) {
        return 'just now';
    }

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

/**
 * Format post dates to relative time for elements with data-timestamp attribute
 */
function formatDates(selector = '.post-date') {
    document.querySelectorAll(selector).forEach(dateElement => {
        const timestampStr = dateElement.getAttribute('data-timestamp');
        if (timestampStr) {
            dateElement.textContent = getRelativeTimeString(timestampStr);
        }
    });
}