document.addEventListener('DOMContentLoaded', function () {
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.user-navigation a, .user-navigation .logout-link');

    navLinks.forEach(link => {
        const href = link.getAttribute('href') || (link.closest('form') ? link.closest('form').getAttribute('action') : null);
        if (href && currentPath.includes(href)) {
            link.classList.add('active');
        }
    });

    updateNotificationCount();

    // Set up SignalR connection

    function getAccessToken() {
        const match = document.cookie.match(/(^| )JwtToken=([^;]+)/);
        const token = match ? match[2] : null;

        return token;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7000/notificationHub", {
            accessTokenFactory: () => {
                const token = getAccessToken();

                return token;
            }
        })
        .build();

    connection.on("ReceiveNotification", function (notification) {
        updateNotificationCount();
    });

    connection.start()
        .then(() => {
            console.log("SignalR Connected successfully");
        })
        .catch(err => {
            console.error("SignalR Connection Error:", err);
        });

    const profilePicture = document.getElementById('profileAvatar');
    const fileInput = document.getElementById('fileInput');

    loadProfilePicture(profilePicture);

    fileInput.addEventListener('change', function (e) {
        if (fileInput.files && fileInput.files[0]) {
            // Update preview image immediately
            const reader = new FileReader();
            reader.onload = function (e) {
                profilePicture.src = e.target.result;
            };
            reader.readAsDataURL(fileInput.files[0]);

            uploadProfilePicture(fileInput.files[0], profilePicture);
        }
    });
});

function loadProfilePicture(profilePicture) {
    const cachedImageUrl = localStorage.getItem('profilePictureUrl');
    const cachedTimestamp = localStorage.getItem('profilePictureTimestamp');

    // Use cached image if it's less than 1 hour old
    if (cachedImageUrl && cachedTimestamp && (Date.now() - cachedTimestamp < 3600000)) {
        profilePicture.src = cachedImageUrl;
    }

    fetch('https://localhost:7000/ProfilePictureApi/GetProfilePicture', {
        method: 'GET',
        headers: {
            'Accept': 'image/*',
        },
        cache: 'no-store',
        credentials: "include"
    })
        .then(response => {
            if (response.ok) {
                return response.blob();
            }
            throw new Error('Failed to load profile picture');
        })
        .then(blob => {
            const imageUrl = URL.createObjectURL(blob);
            profilePicture.src = imageUrl;
            profilePicture.width = 120;
            profilePicture.height = 120;

            // Cache the image URL and timestamp
            localStorage.setItem('profilePictureUrl', imageUrl);
            localStorage.setItem('profilePictureTimestamp', Date.now());
        })
        .catch(error => {
            console.error('Error loading profile picture:', error);
            profilePicture.src = '/images/default-avatar.png';
        });
}

function uploadProfilePicture(file, profilePicture) {
    const formData = new FormData();
    formData.append('file', file);

    fetch('https://localhost:7000/ProfilePictureApi/UploadProfilePicture', {
        method: 'POST',
        body: formData,
        headers: {
            // Let the browser set Content-Type with the boundary parameter
            'Accept': 'application/json'
        },
        credentials: "include"
    })
        .then(response => {
            profilePicture.classList.remove('uploading');

            if (response.ok) {
                return response.json();
            }
            return response.json().then(data => {
                throw new Error(data.message || 'Failed to upload profile picture');
            });
        })
        .then(data => {
            console.log('Success:', data.message);
            loadProfilePicture();
        })
        .catch(error => {
            console.error('Error:', error);
            loadProfilePicture();
        });
}

function updateNotificationCount() {
    const userId = document.getElementById('currentUserId')?.value;
    if (!userId) return;

    fetch('https://localhost:7000/NotificationApi/ShowNewNotification', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const notificationBadge = document.getElementById('notification-count');
            if (notificationBadge) {
                if (data.likesCount > 0) {
                    // Display "9+" if there are more than 9 notifications
                    notificationBadge.textContent = data.likesCount > 9 ? "9+" : data.likesCount;
                    notificationBadge.classList.remove('d-none');
                } else {
                    notificationBadge.classList.add('d-none');
                }
            }
        })
        .catch(error => {
            console.error('Error updating notification count:', error);
        });
}