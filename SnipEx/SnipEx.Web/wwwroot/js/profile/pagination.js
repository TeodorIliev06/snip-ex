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
    url.searchParams.set('page', '1');
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