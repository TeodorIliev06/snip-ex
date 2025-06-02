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

    // Follow/Unfollow button functionality
    const followButtons = document.querySelectorAll('.btn-follow');
    const disconnectButtons = document.querySelectorAll('.btn-disconnect');

    followButtons.forEach(button => {
        button.addEventListener('click', function () {
            const connectionItem = this.closest('.connection-item');
            const connectionId = connectionItem.getAttribute('data-id');

            // Here you would make an AJAX request to follow the user
            console.log(`Following user with ID: ${connectionId}`);

            // UI update (for demo purposes)
            if (this.textContent === 'Follow') {
                this.textContent = 'Following...';
                setTimeout(() => {
                    this.textContent = 'Unfollow';
                    this.classList.remove('btn-follow');
                    this.classList.add('btn-unfollow');

                    // Update the badge if it was a suggested connection
                    const badge = connectionItem.querySelector('.connection-badge');
                    if (badge && badge.classList.contains('suggested')) {
                        badge.classList.remove('suggested');
                        badge.classList.add('following');
                        badge.textContent = 'Following';

                        // Update the item's class for filtering
                        connectionItem.classList.remove('suggested');
                        connectionItem.classList.add('following');
                    }

                    // Update the connection-item border
                    connectionItem.classList.remove('suggested');
                    connectionItem.classList.add('following');

                }, 500);
            } else if (this.textContent === 'Follow Back') {
                this.textContent = 'Following...';
                setTimeout(() => {
                    this.textContent = 'Unfollow';
                    this.classList.remove('btn-follow');
                    this.classList.add('btn-unfollow');

                    // Update the badge
                    const badge = connectionItem.querySelector('.connection-badge');
                    if (badge) {
                        badge.classList.remove('follower');
                        badge.classList.add('mutual');
                        badge.textContent = 'Mutual';
                    }

                    // Update the connection-item border
                    connectionItem.classList.remove('follower');
                    connectionItem.classList.add('mutual');

                }, 500);
            }
        });
    });

    disconnectButtons.forEach(button => {
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

        const connectButton = document.querySelector('.connect-button');
        const connectionCount = document.querySelector('.stat-number');
        const connectionBadge = document.querySelector('.connection-badge');

        connectionCount.textContent = data.connectionsCount;

        if (data.isConnected) {
            connectButton.classList.remove('btn-connect');
            connectButton.classList.add('btn-disconnect');
            connectionBadge.classList.add('connected');

            connectButton.innerHTML = 'Disconnect';
            connectionBadge.innerHTML = 'Connected';

        } else {
            connectButton.classList.remove('btn-disconnect');
            connectButton.classList.add('btn-connect');
            connectionBadge.classList.remove('connected');

            connectButton.innerHTML = 'Connect';
            connectionBadge.innerHTML = '';
        }
    });
}