document.addEventListener('DOMContentLoaded', function () {
    formatDates();
    initializeFilterHandling();
    initializeCopyButtons();
    initializeCodePreviews();
    applyDynamicClasses();

    window.addEventListener('resize', debounce(handleWindowResize, 250));
});

/**
 * Format post dates to relative time (e.g. "2 days ago")
 */
function formatDates() {
    document.querySelectorAll('.post-date').forEach(dateSpan => {
        const timestampStr = dateSpan.getAttribute('data-timestamp');
        dateSpan.textContent = getRelativeTimeString(timestampStr);
    });
}

/**
 * Convert a date string in format 'dd/MM/yyyy' to a relative time string
 */
function getRelativeTimeString(dateStr) {
    const parts = dateStr.split('/');
    if (parts.length !== 3) {
        return dateStr;
    }

    const day = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10) - 1;
    const year = parseInt(parts[2], 10);

    const date = new Date(year, month, day);
    const now = new Date();

    if (isNaN(date.getTime())) {
        return dateStr;
    }

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

/**
 * Initialize all filter-related functionality
 */
function initializeFilterHandling() {
    document.querySelectorAll('.tag-filter').forEach(tagElement => {
        tagElement.addEventListener('click', function (e) {
            e.preventDefault();
            const tag = this.getAttribute('data-tag');
            document.getElementById('tagInput').value = tag;
            document.getElementById('filterForm').submit();
        });
    });

    document.querySelectorAll('.sort-btn').forEach(sortBtn => {
        sortBtn.addEventListener('click', function () {
            const sort = this.getAttribute('data-sort');
            document.getElementById('sortInput').value = sort;
            document.getElementById('filterForm').submit();
        });
    });

    document.getElementById('searchForm').addEventListener('submit', function (e) {
        e.preventDefault();
        const searchValue = document.getElementById('searchInput').value;
        document.getElementById('searchFormInput').value = searchValue;
        document.getElementById('filterForm').submit();
    });

    document.querySelectorAll('.remove-filter').forEach(removeBtn => {
        removeBtn.addEventListener('click', function () {
            const filterType = this.getAttribute('data-filter-type');
            if (filterType === 'search') {
                document.getElementById('searchFormInput').value = '';
            } else if (filterType === 'tag') {
                document.getElementById('tagInput').value = '';
            }
            document.getElementById('filterForm').submit();
        });
    });

    document.getElementById('clearFilters').addEventListener('click', function () {
        document.getElementById('tagInput').value = '';
        document.getElementById('searchFormInput').value = '';
        document.getElementById('filterForm').submit();
    });
}

/**
 * Initialize code snippet copy buttons
 */
function initializeCopyButtons() {
    document.querySelectorAll('.copy-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const content = this.getAttribute('data-content');
            navigator.clipboard.writeText(content)
                .then(() => {
                    const originalText = this.innerHTML;
                    this.innerHTML = '<i class="bi bi-check"></i> Copied!';
                    this.classList.remove('btn-outline-primary');
                    this.classList.add('btn-success');

                    setTimeout(() => {
                        this.innerHTML = originalText;
                        this.classList.remove('btn-success');
                        this.classList.add('btn-outline-primary');
                    }, 2000);
                })
                .catch(err => {
                    console.error('Could not copy text: ', err);
                });
        });
    });
}

/**
 * Improved code preview handling with better mobile support
 */
function initializeCodePreviews() {
    document.querySelectorAll('.code-preview').forEach(codeBlock => {
        const isMobile = window.innerWidth <= 768;
        const heightThreshold = isMobile ? 150 : 200;

        if (codeBlock.clientHeight > heightThreshold) {
            codeBlock.style.maxHeight = `${heightThreshold}px`;
            codeBlock.style.overflow = 'hidden';
            codeBlock.classList.add('collapsed');

            const card = codeBlock.closest('.snippet-card');
            const postId = card.getAttribute('data-post-id');
            const postUrl = `/Post/Details/${postId}`;

            const buttonContainer = card.querySelector('.d-flex.justify-content-between.align-items-center.mt-3');
            const copyButton = buttonContainer.querySelector('.copy-btn');

            if (!buttonContainer.querySelector('.see-more-btn')) {
                createSeeMoreButton(buttonContainer, copyButton, postUrl, isMobile);
            }
        }
    });
}

/**
 * Create "See more" button with mobile-responsive layout
 */
function createSeeMoreButton(buttonContainer, copyButton, postUrl, isMobile) {
    const rightButtonsWrapper = document.createElement('div');
    rightButtonsWrapper.className = isMobile ? 'd-flex flex-column gap-2 w-100' : 'd-flex gap-2 align-items-center';

    if (copyButton) {
        rightButtonsWrapper.appendChild(copyButton);
    }

    // Create see more button
    const seeMoreLink = document.createElement('a');
    seeMoreLink.href = postUrl;
    seeMoreLink.className = 'btn btn-sm see-more-btn';
    seeMoreLink.innerHTML = '<i class="bi bi-arrow-right-circle"></i> See more';

    rightButtonsWrapper.appendChild(seeMoreLink);
    buttonContainer.appendChild(rightButtonsWrapper);

    if (isMobile) {
        buttonContainer.classList.remove('justify-content-between');
        buttonContainer.classList.add('flex-column', 'align-items-stretch');
    }
}

/**
 * Apply dynamic classes to elements (selected tags and sort buttons)
 */
function applyDynamicClasses() {
    const selectedTag = document.getElementById('tagInput').value;
    document.querySelectorAll('.tag-filter').forEach(tagElement => {
        if (tagElement.getAttribute('data-tag') === selectedTag) {
            tagElement.classList.add('bg-primary');
            tagElement.classList.remove('bg-light');
        } else {
            tagElement.classList.remove('bg-primary');
            tagElement.classList.add('bg-light');
        }
    });

    const selectedSort = document.getElementById('sortInput').value;
    document.querySelectorAll('.sort-btn').forEach(sortBtn => {
        if (sortBtn.getAttribute('data-sort') === selectedSort) {
            sortBtn.classList.add('btn-primary');
            sortBtn.classList.remove('btn-outline-primary');
        } else {
            sortBtn.classList.remove('btn-primary');
            sortBtn.classList.add('btn-outline-primary');
        }
    });

    const searchQuery = document.getElementById('searchFormInput').value.trim();
    const activeFiltersContainer = document.getElementById('activeFilters');
    if (searchQuery || selectedTag) {
        activeFiltersContainer.classList.remove('d-none');
    } else {
        activeFiltersContainer.classList.add('d-none');
    }
}

/**
 * Handle window resize for responsive adjustments
 */
function handleWindowResize() {
    initializeCodePreviews();

    const isMobile = window.innerWidth <= 768;
    document.querySelectorAll('.snippet-card').forEach(card => {
        const buttonContainer = card.querySelector('.d-flex.justify-content-between.align-items-center.mt-3, .d-flex.flex-column.align-items-stretch.mt-3');
        const rightButtonsWrapper = buttonContainer?.querySelector('.d-flex.gap-2.align-items-center, .d-flex.flex-column.gap-2.w-100');

        if (rightButtonsWrapper) {
            if (isMobile) {
                rightButtonsWrapper.className = 'd-flex flex-column gap-2 w-100';
                buttonContainer.classList.remove('justify-content-between');
                buttonContainer.classList.add('flex-column', 'align-items-stretch');
            } else {
                rightButtonsWrapper.className = 'd-flex gap-2 align-items-center';
                buttonContainer.classList.add('justify-content-between');
                buttonContainer.classList.remove('flex-column', 'align-items-stretch');
            }
        }
    });
}

/**
 * Debounce function to limit how often resize handler is called
 */
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