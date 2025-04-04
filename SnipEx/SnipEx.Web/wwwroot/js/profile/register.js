document.addEventListener('DOMContentLoaded', function () {
    const lines = document.querySelectorAll('.register-terminal-content .terminal-line');
    lines.forEach((line, index) => {
        setTimeout(() => {
            line.classList.add('visible');
        }, 300 * index);
    });

    createCodeParticles();

    const registerBtn = document.querySelector('.btn-register');
    if (registerBtn) {
        registerBtn.addEventListener('mouseenter', function () {
            this.style.transition = 'all 0.3s ease, transform 0.3s ease, box-shadow 0.3s ease';
        });
    }
});

function createCodeParticles() {
    const container = document.querySelector('.register-container');
    const codeSnippets = ['*', '*', '*'];

    const particleCount = Math.floor(Math.random() * 6) + 30;

    for (let i = 0; i < particleCount; i++) {
        createParticle(container, codeSnippets);
    }
}

function createParticle(container, codeSnippets) {
    const particle = document.createElement('div');
    particle.classList.add('code-particle');

    const posX = Math.random() * 100;
    const posY = Math.random() * 100;

    const snippetIndex = Math.floor(Math.random() * codeSnippets.length);
    particle.textContent = codeSnippets[snippetIndex];

    particle.style.left = `${posX}%`;
    particle.style.top = `${posY}%`;

    particle.style.opacity = (Math.random() * 0.4) + 0.2;

    container.appendChild(particle);

    animateParticle(particle);
}

function animateParticle(particle) {
    const startX = parseFloat(particle.style.left);
    const startY = parseFloat(particle.style.top);

    const targetX = startX + (Math.random() * 20) - 5;
    const targetY = startY + (Math.random() * 20) - 5;

    const duration = Math.random() * 8000 + 8000;
    const startTime = Date.now();

    function update() {
        const elapsed = Date.now() - startTime;
        const progress = Math.min(elapsed / duration, 1);

        const easeProgress = progress < 0.5
            ? 2 * progress * progress
            : 1 - Math.pow(-2 * progress + 2, 2) / 2;

        const currentX = startX + (targetX - startX) * easeProgress;
        const currentY = startY + (targetY - startY) * easeProgress;

        particle.style.left = `${currentX}%`;
        particle.style.top = `${currentY}%`;

        if (progress < 1) {
            requestAnimationFrame(update);
        } else {
            animateParticle(particle);
        }
    }

    requestAnimationFrame(update);
}