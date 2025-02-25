document.addEventListener('DOMContentLoaded', function () {
    // Initialize post timestamps
    formatDates();

    // Initialize filter handling
    initializeFilterHandling();

    // Initialize copy buttons
    initializeCopyButtons();

    // Limit code preview height
    initializeCodePreviews();

    // Apply dynamic class assignments
    applyDynamicClasses();
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

/**
 * Initialize all filter-related functionality
 */
function initializeFilterHandling() {
    // Handle tag filters
    document.querySelectorAll('.tag-filter').forEach(tagElement => {
        tagElement.addEventListener('click', function (e) {
            e.preventDefault();
            const tag = this.getAttribute('data-tag');
            document.getElementById('tagInput').value = tag;
            document.getElementById('filterForm').submit();
        });
    });

    // Handle sort buttons
    document.querySelectorAll('.sort-btn').forEach(sortBtn => {
        sortBtn.addEventListener('click', function () {
            const sort = this.getAttribute('data-sort');
            document.getElementById('sortInput').value = sort;
            document.getElementById('filterForm').submit();
        });
    });

    // Handle search form
    document.getElementById('searchForm').addEventListener('submit', function (e) {
        e.preventDefault();
        const searchValue = document.getElementById('searchInput').value;
        document.getElementById('searchFormInput').value = searchValue;
        document.getElementById('filterForm').submit();
    });

    // Handle removing individual filters
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

    // Handle clear all filters button
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
 * Limit code preview height and add expand/collapse functionality
 */
function initializeCodePreviews() {
    document.querySelectorAll('.code-preview').forEach(codeBlock => {
        if (codeBlock.clientHeight > 200) {
            codeBlock.style.maxHeight = '200px';
            codeBlock.style.overflow = 'hidden';
            codeBlock.classList.add('collapsed');

            const expandBtn = document.createElement('button');
            expandBtn.className = 'btn btn-sm btn-link expand-code-btn';
            expandBtn.innerHTML = '<i class="bi bi-arrows-expand"></i> Show more';
            expandBtn.style.display = 'block';
            expandBtn.style.width = '100%';
            expandBtn.style.marginTop = '8px';

            expandBtn.addEventListener('click', function () {
                if (codeBlock.classList.contains('collapsed')) {
                    codeBlock.style.maxHeight = 'none';
                    codeBlock.classList.remove('collapsed');
                    this.innerHTML = '<i class="bi bi-arrows-collapse"></i> Show less';
                } else {
                    codeBlock.style.maxHeight = '200px';
                    codeBlock.classList.add('collapsed');
                    this.innerHTML = '<i class="bi bi-arrows-expand"></i> Show more';
                }
            });

            codeBlock.parentNode.insertBefore(expandBtn, codeBlock.nextSibling);
        }
    });
}

/**
* Apply dynamic classes to elements like selected tags and sort buttons
*/
function applyDynamicClasses() {
    // Apply active class to the selected tag
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

    // Apply active class to the selected sort button
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

    // Show/hide active filters container
    const searchQuery = document.getElementById('searchFormInput').value.trim();
    const activeFiltersContainer = document.getElementById('activeFilters');
    if (searchQuery || selectedTag) {
        activeFiltersContainer.classList.remove('d-none');
    } else {
        activeFiltersContainer.classList.add('d-none');
    }
}