document.addEventListener('DOMContentLoaded', function () {
    const connectButton = document.querySelector('.connect-button[data-target-user-id]');
    if (connectButton) {
        const targetUserId = connectButton.getAttribute('data-target-user-id');
        connectButton.addEventListener('click', function () {
            toggleConnection(targetUserId);
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
        const connectionCountElement = document.querySelector('.stat-item:nth-child(3) strong');

        connectionCountElement.textContent = data.connectionsCount;

        if (data.isConnected) {
            connectButton.classList.remove('btn-connect');
            connectButton.classList.add('btn-disconnect');
            connectButton.innerHTML = '<i class="fa-solid fa-user-minus"></i> Disconnect';
        } else {
            connectButton.classList.remove('btn-disconnect');
            connectButton.classList.add('btn-connect');
            connectButton.innerHTML = '<i class="fa-solid fa-user-plus"></i> Connect';
        }
    });
}