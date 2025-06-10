document.addEventListener('DOMContentLoaded', function () {
    // Filter buttons functionality
    const filterButtons = document.querySelectorAll('.filter-button');
    const connectionItems = document.querySelectorAll('.connection-item');

    filterButtons.forEach(button => {
        button.addEventListener('click', () => {
            // Update active filter button
            filterButtons.forEach(btn => btn.classList.remove('active'));
            button.classList.add('active');

            const filterValue = button.getAttribute('data-filter');

            // Show/hide connection items based on filter
            connectionItems.forEach(item => {
                if (filterValue === 'all') {
                    item.style.display = 'flex';
                } else {
                    if (item.classList.contains(filterValue)) {
                        item.style.display = 'flex';
                    } else {
                        item.style.display = 'none';
                    }
                }
            });
        });
    });

    const connectionButtons = document.querySelectorAll('.connect-button');

    connectionButtons.forEach(button => {
        button.addEventListener('click', async function () {
            const targetUserId = button.getAttribute('data-target-user-id');

            await toggleConnection(targetUserId);
        });
    });

    // Load more functionality
    const loadMoreBtn = document.getElementById('load-more-btn');
    if (loadMoreBtn) {
        loadMoreBtn.addEventListener('click', function () {
            // Here you would make an AJAX request to load more connections
            this.textContent = 'Loading...';

            // For demo purposes, create dummy connections after a short delay
            setTimeout(() => {
                const connectionsList = document.querySelector('.connections-list');

                // Example of dynamically adding new connections
                for (let i = 0; i < 3; i++) {
                    const template = `
                        <div class="connection-item ${['mutual', 'following', 'follower'][Math.floor(Math.random() * 3)]}" data-id="${Math.floor(Math.random() * 1000) + 100}">
                            <div class="connection-avatar">
                                <img src="/images/default-avatar.png" alt="dynamic-user">
                            </div>
                            <div class="connection-content">
                                <div class="connection-header">
                                    <div class="connection-name">
                                        <span>user${Math.floor(Math.random() * 1000)}</span>
                                        <span class="connection-badge ${['mutual', 'following', 'follower'][Math.floor(Math.random() * 3)]}">${['Mutual', 'Following', 'Follower'][Math.floor(Math.random() * 3)]}</span>
                                    </div>
                                    <div class="connection-actions">
                                        <button class="${Math.random() > 0.5 ? 'btn-unfollow' : 'btn-follow'}">${Math.random() > 0.5 ? 'Unfollow' : 'Follow'}</button>
                                    </div>
                                </div>
                                <div class="connection-bio">
                                    Developer with ${Math.floor(Math.random() * 10) + 1} years of experience in web development.
                                </div>
                                <div class="connection-meta">
                                    <span class="meta-item"><i class="fa-solid fa-code"></i> ${Math.floor(Math.random() * 100)} snippets</span>
                                    <span class="meta-item"><i class="fa-solid fa-star"></i> ${Math.floor(Math.random() * 200)} stars</span>
                                    <span class="meta-item"><i class="fa-solid fa-users"></i> ${Math.floor(Math.random() * 20)} mutual connections</span>
                                </div>
                            </div>
                        </div>
                    `;

                    // Add the new connection to the list
                    connectionsList.insertAdjacentHTML('beforeend', template);
                }

                // Restore the button text
                this.textContent = 'Load more';

                // Reattach event listeners to the new buttons
                document.querySelectorAll('.btn-follow:not([data-initialized])').forEach(btn => {
                    btn.addEventListener('click', function () {
                        console.log('Follow button clicked for dynamically added connection');
                        // Similar functionality as above...
                    });
                    btn.setAttribute('data-initialized', 'true');
                });

                document.querySelectorAll('.btn-unfollow:not([data-initialized])').forEach(btn => {
                    btn.addEventListener('click', function () {
                        console.log('Unfollow button clicked for dynamically added connection');
                        // Similar functionality as above...
                    });
                    btn.setAttribute('data-initialized', 'true');
                });

            }, 1000);
        });
    }
});

async function toggleConnection(targetUserId) {
    await fetchWithToastr(`https://localhost:7000/UserActionApi/ToggleConnection/${targetUserId}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include'
    })
    .then(data => {
        if (!data) return;

        const connectionItem = document
            .querySelector(`.connect-button[data-target-user-id="${targetUserId}"]`)
            ?.closest('.connection-item');
        if (!connectionItem) return;

        const connectionCount = document.querySelector('.stat-number');
        connectionCount.textContent = data.connectionsCount;

        if (data.isConnected) {
            updateConnectionUI(connectionItem, 'connected');
        } else {
            const originalType = connectionItem.getAttribute('data-original-type') || 'mutual';
            updateConnectionUI(connectionItem, originalType);
        }
    });
}

const connectionStates = {
    connected: {
        button: { classes: ['btn-disconnect'], text: 'Disconnect' },
        badge: { classes: ['connected'], text: 'CONNECTED' },
        item: { classes: ['connected'] }
    },
    mutual: {
        button: { classes: ['btn-connect'], text: 'Connect' },
        badge: { classes: ['mutual'], text: 'MUTUAL' },
        item: { classes: ['mutual'] }
    },
    disconnected: {
        button: { classes: ['btn-connect'], text: 'Connect' },
        badge: { classes: [], text: '' },
        item: { classes: [] }
    }
};

function updateConnectionUI(connectionItem, newState) {
    const connectButton = connectionItem.querySelector('.connect-button');
    const connectionBadge = connectionItem.querySelector('.connection-badge');
    const config = connectionStates[newState];

    // Clear all possible classes
    const allClasses = ['btn-connect', 'btn-disconnect', 'connected', 'mutual'];

    // Update button
    connectButton.classList.remove(...allClasses);
    connectButton.classList.add(...config.button.classes);
    connectButton.innerHTML = config.button.text;

    // Update badge
    connectionBadge.classList.remove(...allClasses);
    connectionBadge.classList.add(...config.badge.classes);
    connectionBadge.innerHTML = config.badge.text;

    // Update item
    connectionItem.classList.remove(...allClasses);
    connectionItem.classList.add(...config.item.classes);
}