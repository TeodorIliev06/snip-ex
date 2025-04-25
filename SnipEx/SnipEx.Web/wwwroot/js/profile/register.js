document.addEventListener('DOMContentLoaded', function () {
    const lines = document.querySelectorAll('.register-terminal-content .terminal-line');
    lines.forEach((line, index) => {
        setTimeout(() => {
            line.classList.add('visible');
        }, 300 * index);
    });

    createLightweightParticles();
    const registerBtn = document.querySelector('.btn-register');
    if (registerBtn) {
        registerBtn.addEventListener('mouseenter', function () {
            this.style.transition = 'all 0.3s ease, transform 0.3s ease, box-shadow 0.3s ease';
        });
    }
});

function createLightweightParticles() {
    const particleCount = 6;
    const codeSnippets = ['*', '*', '*'];
    const container = document.querySelector('.register-container');

    createParticleBatch();

    setInterval(() => {
        createParticleBatch();
    }, 2700);

    function createParticleBatch() {
        for (let i = 0; i < particleCount; i++) {
            const particle = document.createElement('div');
            particle.classList.add('code-particle');
            
            const posX = Math.random() * 100;
            const posY = Math.random() * 100;
            
            const snippetIndex = Math.floor(Math.random() * codeSnippets.length);

            particle.textContent = codeSnippets[snippetIndex];
            particle.style.left = `${posX}%`;
            particle.style.top = `${posY}%`;

            const particleOpacity = (Math.random() * 0.4) + 0.2;
            particle.style.setProperty('--particle-opacity', particleOpacity);
            particle.style.setProperty('--move-x', `${(Math.random() * 30) - 15}%`);
            particle.style.setProperty('--move-y', `${(Math.random() * 30) - 15}%`);

            const duration = Math.random() * 3 + 5;
            particle.style.setProperty('--duration', `${duration}s`);
            particle.style.setProperty('--fade-out-delay', `${6}s`);
            
            container.appendChild(particle);
            
            setTimeout(() => {
                if (particle.parentNode === container) {
                    container.removeChild(particle);
                }
            }, 8000);
        }
    }
}