document.addEventListener('DOMContentLoaded', function () {
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.user-navigation a, .user-navigation .logout-link');

    navLinks.forEach(link => {
        const href = link.getAttribute('href') || (link.closest('form') ? link.closest('form').getAttribute('action') : null);
        if (href && currentPath.includes(href)) {
            link.classList.add('active');
        }
    });

    const profilePicture = document.getElementById("profilePicture");

    fetch("https://localhost:7000/ProfilePictureApi/GetProfilePicture", {
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
});