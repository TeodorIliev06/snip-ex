document.addEventListener('DOMContentLoaded', function () {
    const languageFilter = document.getElementById('languageFilter');
    const sortByFilter = document.getElementById('sortBy');
    const searchInput = document.getElementById('searchSnippets');
    const searchButton = document.querySelector('.search-button');

    // Function to apply filters
    function applyFilters() {
        const language = languageFilter.value;
        const sortBy = sortByFilter.value;
        const searchTerm = searchInput.value.toLowerCase();

        const snippetCards = document.querySelectorAll('.snippet-card');

        snippetCards.forEach(card => {
            // Get card data
            const cardLanguage = card.querySelector('.snippet-language').textContent;
            const cardTitle = card.querySelector('.snippet-title').textContent.toLowerCase();
            const cardContent = card.querySelector('.snippet-content').textContent.toLowerCase();

            // Check language filter
            const languageMatch = !language || cardLanguage === language;

            // Check search term
            const searchMatch = !searchTerm ||
                cardTitle.includes(searchTerm) ||
                cardContent.includes(searchTerm);

            // Show or hide based on filters
            if (languageMatch && searchMatch) {
                card.style.display = 'flex';
            } else {
                card.style.display = 'none';
            }
        });

        // Apply sorting
        const snippetList = document.querySelector('.snippet-list');
        const snippets = Array.from(snippetCards);

        snippets.sort((a, b) => {
            const titleA = a.querySelector('.snippet-title').textContent.toLowerCase();
            const titleB = b.querySelector('.snippet-title').textContent.toLowerCase();
            const dateA = new Date(a.dataset.createDate);
            const dateB = new Date(b.dataset.createDate);

            switch (sortBy) {
                case 'newest':
                    return dateB - dateA;
                case 'oldest':
                    return dateA - dateB;
                case 'az':
                    return titleA.localeCompare(titleB);
                case 'za':
                    return titleB.localeCompare(titleA);
                default:
                    return 0;
            }
        });

        // Re-append sorted items
        snippets.forEach(snippet => {
            snippetList.appendChild(snippet);
        });
    }

    // Event listeners
    languageFilter.addEventListener('change', applyFilters);
    sortByFilter.addEventListener('change', applyFilters);
    searchButton.addEventListener('click', applyFilters);
    searchInput.addEventListener('keyup', function (event) {
        if (event.key === 'Enter') {
            applyFilters();
        }
    });

    // Initial sort
    applyFilters();
});