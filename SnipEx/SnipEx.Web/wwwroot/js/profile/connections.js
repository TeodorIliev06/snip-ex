document.addEventListener('DOMContentLoaded', function () {
    initializePagination();

    const connectionButtons = document.querySelectorAll('.connect-button');
    connectionButtons.forEach(button => {
        button.addEventListener('click', async function () {
            const targetUserId = button.getAttribute('data-target-user-id');
            await toggleConnection(targetUserId);
        });
    });
});

async function toggleConnection(targetUserId) {
    const button = document.querySelector(`.connect-button[data-target-user-id="${targetUserId}"]`);
    const originalText = button.innerHTML;

    button.disabled = true;
    button.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i>';

    try {
        const data = await fetchWithToastr(`https://localhost:7000/UserActionApi/ToggleConnection/${targetUserId}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include'
        });

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

        // Check if we need to reload the page due to filtering
        const currentFilter = getCurrentFilter();
        if (shouldReloadPage(data.isConnected, currentFilter)) {
            window.location.reload();
        }

    } catch (error) {
        console.error('Error toggling connection:', error);
        button.innerHTML = originalText;
    } finally {
        button.disabled = false;
    }
}

function shouldReloadPage(isConnected, currentFilter) {
    if (currentFilter === 'connected' && !isConnected) {
        return true;
    }
    if (currentFilter === 'mutual' && isConnected) {
        return true;
    }
    return false;
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

    const allClasses = ['btn-connect', 'btn-disconnect', 'connected', 'mutual'];

    connectButton.classList.remove(...allClasses);
    connectButton.classList.add(...config.button.classes);
    connectButton.innerHTML = config.button.text;

    connectionBadge.classList.remove(...allClasses);
    connectionBadge.classList.add(...config.badge.classes);
    connectionBadge.innerHTML = config.badge.text;

    connectionItem.classList.remove(...allClasses);
    connectionItem.classList.add(...config.item.classes);
}