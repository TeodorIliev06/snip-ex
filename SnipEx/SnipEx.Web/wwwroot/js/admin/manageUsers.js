document.addEventListener('DOMContentLoaded', () => {
    const userTable = document.querySelector('table');
    const searchInput = document.getElementById('userSearch');
    const clearFiltersBtn = document.querySelector('.clear-filters-btn');
    const bannedFilterBtn = document.querySelector('.banned-filter-btn');
    let showBannedOnly = false;

    const filterUsers = () => {
        const searchTerm = searchInput.value.toLowerCase().trim();
        userTable.querySelectorAll('tbody tr').forEach(row => {
            const username = row.cells[0].textContent.toLowerCase();
            const banStatus = row.cells[3].textContent.trim().toLowerCase();
            const isBanned = banStatus.includes('banned');
            
            const matchesSearch = username.includes(searchTerm);
            const matchesBanFilter = !showBannedOnly || isBanned;
            
            row.style.display = (matchesSearch && matchesBanFilter) ? '' : 'none';
        });
    };

    if (searchInput) {
        searchInput.addEventListener('input', filterUsers);
    }

    if (bannedFilterBtn) {
        bannedFilterBtn.addEventListener('click', () => {
            showBannedOnly = !showBannedOnly;
            bannedFilterBtn.textContent = showBannedOnly ? 'Show All Users' : 'Show Banned Only';
            bannedFilterBtn.className = showBannedOnly ? 'banned-filter-btn btn btn-info' : 'banned-filter-btn btn btn-warning';
            filterUsers();
        });
    }

    if (clearFiltersBtn) {
        clearFiltersBtn.addEventListener('click', () => {
            searchInput.value = '';
            showBannedOnly = false;
            bannedFilterBtn.textContent = 'Show Banned Only';
            bannedFilterBtn.className = 'banned-filter-btn btn btn-warning';
            filterUsers();
        });
    }

    // Add confirmation dialog for ban/unban actions
    const actionButtons = document.querySelectorAll('#actionBtn');
    actionButtons.forEach(button => {
        button.addEventListener('click', (e) => {
            e.preventDefault();
            const action = button.textContent.trim();
            const username = button.closest('tr').cells[0].textContent.trim();
            const confirmMessage = action === 'Ban User'
                ? `Are you sure you want to ban ${username}? This will prevent them from accessing the platform.`
                : `Are you sure you want to unban ${username}? This will restore their access to the platform.`;

            if (confirm(confirmMessage)) {
                const form = button.closest('form');
                const isBanAction = action === 'Ban User';

                form.submit();

                const successMessage = isBanAction
                    ? `${username} has been banned successfully!`
                    : `${username} has been unbanned successfully!`;

                toastr.success(successMessage, "", {
                    timeOut: 3000,
                    closeButton: true
                });
            }
            // If user cancels, do nothing
        });
    });

    filterUsers();
});