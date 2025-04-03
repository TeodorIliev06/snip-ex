// Code for animated particles in the background
document.addEventListener('DOMContentLoaded', function () {
    // Terminal animation (from your previous code)
    const lines = document.querySelectorAll('.terminal-line');
    lines.forEach((line, index) => {
        setTimeout(() => {
            line.classList.add('visible');
        }, 300 * index);
    });

    // Create animated code particles
    createCodeParticles();

    // Add subtle pulse animation to login button
    const loginButton = document.querySelector('.btn-login');
    if (loginButton) {
        loginButton.addEventListener('mouseenter', function () {
            this.style.transition = 'all 0.3s ease, transform 0.3s ease, box-shadow 0.3s ease';
        });
    }
});

function createCodeParticles() {
    const container = document.querySelector('.login-container');
    const codeSnippets = [
        'const user =', 'function auth()', 'if (valid) {', '} else {',
        'return true;', 'import {', 'export default', '=> {', '});',
        'async/await', '.then()', 'try {', 'catch (e) {', '</>',
        '[]', '{}', '()', '===', '=>'
    ];

    // Create 15-20 particles
    const particleCount = Math.floor(Math.random() * 6) + 15;

    for (let i = 0; i < particleCount; i++) {
        createParticle(container, codeSnippets);
    }
}

function createParticle(container, codeSnippets) {
    const particle = document.createElement('div');
    particle.classList.add('code-particle');

    // Random position
    const posX = Math.random() * 100;
    const posY = Math.random() * 100;

    // Random text content
    const snippetIndex = Math.floor(Math.random() * codeSnippets.length);
    particle.textContent = codeSnippets[snippetIndex];

    // Set initial position
    particle.style.left = `${posX}%`;
    particle.style.top = `${posY}%`;

    // Random opacity
    particle.style.opacity = (Math.random() * 0.4) + 0.2;

    // Add to container
    container.appendChild(particle);

    // Animate the particle
    animateParticle(particle);
}

function animateParticle(particle) {
    // Initial position
    const startX = parseFloat(particle.style.left);
    const startY = parseFloat(particle.style.top);

    // Generate target position (small movement)
    const targetX = startX + (Math.random() * 10) - 5;
    const targetY = startY + (Math.random() * 10) - 5;

    // Animation duration
    const duration = Math.random() * 8000 + 8000; // 8-16 seconds

    // Start time
    const startTime = Date.now();

    function update() {
        const elapsed = Date.now() - startTime;
        const progress = Math.min(elapsed / duration, 1);

        // Ease in-out function
        const easeProgress = progress < 0.5
            ? 2 * progress * progress
            : 1 - Math.pow(-2 * progress + 2, 2) / 2;

        // Calculate current position
        const currentX = startX + (targetX - startX) * easeProgress;
        const currentY = startY + (targetY - startY) * easeProgress;

        // Update position
        particle.style.left = `${currentX}%`;
        particle.style.top = `${currentY}%`;

        // Continue animation until duration is complete
        if (progress < 1) {
            requestAnimationFrame(update);
        } else {
            // Animation complete, start a new one
            animateParticle(particle);
        }
    }

    // Start the animation
    requestAnimationFrame(update);
}