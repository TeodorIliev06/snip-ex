document.addEventListener('DOMContentLoaded', function () {
    // Animate stats numbers on page load
    animateStatsNumbers();

    // Add click effects to action cards
    addActionCardEffects();

    // Refresh activity feed periodically (optional)
    // setInterval(refreshActivityFeed, 30000); // Refresh every 30 seconds
});

function animateStatsNumbers() {
    const statNumbers = document.querySelectorAll('.stat-number');

    statNumbers.forEach(stat => {
        const finalNumber = parseInt(stat.textContent.replace(/,/g, ''));
        const duration = 2000; // 2 seconds
        const startTime = Date.now();

        const updateNumber = () => {
            const elapsed = Date.now() - startTime;
            const progress = Math.min(elapsed / duration, 1);

            // Easing function for smooth animation
            const easeOut = 1 - Math.pow(1 - progress, 3);
            const currentNumber = Math.floor(finalNumber * easeOut);

            stat.textContent = currentNumber.toLocaleString();

            if (progress < 1) {
                requestAnimationFrame(updateNumber);
            }
        };

        // Start with 0
        stat.textContent = '0';
        requestAnimationFrame(updateNumber);
    });
}

function addActionCardEffects() {
    const actionCards = document.querySelectorAll('.action-card');

    actionCards.forEach(card => {
        card.addEventListener('click', function (e) {
            // Add ripple effect
            const ripple = document.createElement('div');
            ripple.style.position = 'absolute';
            ripple.style.borderRadius = '50%';
            ripple.style.background = 'rgba(88, 166, 255, 0.3)';
            ripple.style.transform = 'scale(0)';
            ripple.style.animation = 'ripple 0.6s linear';
            ripple.style.pointerEvents = 'none';

            const rect = card.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';

            card.appendChild(ripple);

            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });
}

function refreshActivityFeed() {
    // This would typically make an AJAX call to get fresh activity data
    // For now, we'll just add a subtle animation to show it's refreshing
    const activityCard = document.querySelector('.activity-card');

    if (activityCard) {
        activityCard.style.opacity = '0.7';
        activityCard.style.transform = 'translateY(-2px)';

        setTimeout(() => {
            activityCard.style.opacity = '1';
            activityCard.style.transform = 'translateY(0)';
        }, 300);
    }
}

// Add CSS for ripple animation
const style = document.createElement('style');
style.textContent = `
    @keyframes ripple {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
    
    .action-card {
        position: relative;
        overflow: hidden;
    }
`;
document.head.appendChild(style);