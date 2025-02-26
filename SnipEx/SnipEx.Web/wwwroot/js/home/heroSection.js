


document.addEventListener('DOMContentLoaded', function () {
    // Terminal animation
    const terminalContent = document.getElementById('terminal-content');
    const terminalLines = [
        { text: 'Welcome to SnipEx', delay: 0 },
        { text: 'Loading developer environment...', delay: 1000 },
        { text: 'npm install @snipex/core', delay: 2000 },
        { text: 'Successfully installed packages', delay: 3000 },
        { text: 'Initializing code environment...', delay: 4000 },
        { text: 'Ready to share and explore code!', delay: 5000 }
    ];

    terminalLines.forEach(line => {
        setTimeout(() => {
            const lineElement = document.createElement('div');
            lineElement.classList.add('terminal-line');

            // Simulate typing effect
            let i = 0;
            const typing = setInterval(() => {
                lineElement.textContent = line.text.substring(0, i);
                i++;

                if (i > line.text.length) {
                    clearInterval(typing);
                }
            }, 30);

            terminalContent.appendChild(lineElement);
            terminalContent.scrollTop = terminalContent.scrollHeight;
        }, line.delay);
    });
});