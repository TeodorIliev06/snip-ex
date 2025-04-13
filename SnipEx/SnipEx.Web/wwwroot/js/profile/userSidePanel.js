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
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7024/notificationHub")
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveNotification", function (notification) {
        // Update notification count when a new notification is received
        updateNotificationCount();
        console.log("New notification received:", notification);
    });

    connection.start()
        .then(() => console.log("SignalR Connected"))
        .catch(err => console.error(err));

    const profilePicture = document.getElementById("profilePicture");
    const fileInput = document.getElementById("profilePictureUpload");

    // Only add event listeners if elements exist
    if (profilePicture && fileInput) {
        loadProfilePicture(profilePicture);

        profilePicture.addEventListener('click', function () {
            console.log("Profile picture clicked");
            if (!fileInput) {
                console.error("File input element is null");
                return;
            }
            fileInput.click();
        });

        fileInput.addEventListener('change', function () {
            console.log("File input changed:", this.files);
            if (this.files && this.files[0]) {
                uploadProfilePicture(this.files[0]);
            } else {
                console.error("No files selected or files object is undefined");
            }
        });
    } else {
        console.error("Could not find required elements");
    }
});

function loadProfilePicture(profilePicture) {
    fetch("https://localhost:7000/ProfilePictureApi/GetProfilePicture",
            {
                method: "GET",
                credentials: "include"
            })
        .then(response => {
            if (!response.ok) throw new Error("Profile picture not found");
            return response.blob();
        })
        .then(imageBlob => {
            const imageUrl = URL.createObjectURL(imageBlob);
            profilePicture.src = imageUrl;
            profilePicture.width = 120;
            profilePicture.height = 120;
        })
        .catch(error => {
            console.error("Error loading profile picture:", error);
        });
}

function uploadProfilePicture(file) {
    const formData = new FormData();
    formData.append('file', file);

    fetch('https://localhost:7000/ProfilePictureApi/UploadProfilePicture', {
            method: 'POST',
            credentials: "include",
            body: formData
        })
        .then(response => response.json())
        .then(data => {
            if (data.message) {
                // TODO: Display success message
                const profilePicture = document.getElementById("profilePicture");

                loadProfilePicture(profilePicture);
            } else {
                // TODO: Display error message
            }
        })
        .catch(error => {
            console.error('Error uploading profile picture:', error);
            // TODO: Display error message
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