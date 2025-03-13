﻿document.addEventListener('DOMContentLoaded', function () {
    // Initialize Prism for syntax highlighting
    if (typeof Prism !== 'undefined') {
        Prism.highlightAll();
    }

    const copyButton = document.querySelector('.copy-button');
    if (copyButton) {
        copyButton.addEventListener('click', copyToClipboard);
    }

    const likeBtn = document.querySelectorAll('.like-button');

    likeBtn.forEach(button => {
        const postId = button.getAttribute('data-post-id');
        button.addEventListener('click', function () {
            togglePostLike(postId);
        });
    });
});

function copyToClipboard() {
    const codeElement = document.querySelector('pre code');
    if (!codeElement) return;

    const textToCopy = codeElement.textContent;

    navigator.clipboard.writeText(textToCopy).then(() => {
        const copyButton = document.querySelector('.copy-button');
        const originalText = copyButton.innerHTML;

        // Use an icon to indicate success
        copyButton.innerHTML = '<i class="fa fa-check"></i> Copied!';
        copyButton.classList.add('copied');

        setTimeout(() => {
            copyButton.innerHTML = originalText;
            copyButton.classList.remove('copied');
        }, 2000);
    }).catch(err => {
        console.error('Failed to copy: ', err);
        // Show error feedback to user
        const copyButton = document.querySelector('.copy-button');
        copyButton.innerHTML = 'Failed to copy';
        setTimeout(() => {
            copyButton.innerHTML = 'Copy';
        }, 2000);
    });
}

// Add line numbers to code
function addLineNumbers() {
    const codeElement = document.querySelector('pre code');
    if (!codeElement) return;

    const codeLines = codeElement.innerHTML.split('\n');
    let numberedCode = '';

    codeLines.forEach((line, index) => {
        // Skip empty lines at the end
        if (index === codeLines.length - 1 && line.trim() === '') return;
        numberedCode += `<span class="line-number">${index + 1}</span>${line}\n`;
    });

    codeElement.innerHTML = numberedCode;
}

// Check if code is too long and add a "Show more" button
function handleLongCode() {
    const codeContainer = document.querySelector('.code-snippet');
    if (!codeContainer) return;

    const maxHeight = 600; // Max height in pixels

    if (codeContainer.scrollHeight > maxHeight) {
        codeContainer.classList.add('collapsed');

        const expandButton = document.createElement('button');
        expandButton.className = 'expand-code-button';
        expandButton.innerHTML = 'Show more <i class="fa fa-chevron-down"></i>';

        expandButton.addEventListener('click', function () {
            if (codeContainer.classList.contains('collapsed')) {
                codeContainer.classList.remove('collapsed');
                expandButton.innerHTML = 'Show less <i class="fa fa-chevron-up"></i>';
            } else {
                codeContainer.classList.add('collapsed');
                expandButton.innerHTML = 'Show more <i class="fa fa-chevron-down"></i>';
                // Scroll back to the top of the code when collapsing
                codeContainer.scrollTop = 0;
            }
        });

        // Add the button after the code container
        codeContainer.parentNode.insertBefore(expandButton, codeContainer.nextSibling);
    }
}

function togglePostLike(postId) {
    const userId = document.getElementById('currentUserId').value;

    fetch(`https://localhost:7000/LikeApi/TogglePostLike/${postId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include',
            body: JSON.stringify({ userId })
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const likeButton = document.querySelector(`.like-button[data-post-id="${postId}"]`);
            const likeCount = likeButton.querySelector('.count');

            let currentCount = parseInt(likeCount.textContent, 10);
            likeCount.textContent = data.isLiked ? currentCount + 1 : currentCount - 1;

            likeButton.classList.toggle('liked', data.isLiked);
        })
        .catch(error => {
            console.error('Error:', error);
        });
}

// Run all code enhancements after the document is ready
document.addEventListener('DOMContentLoaded', function () {
    if (typeof Prism !== 'undefined') {
        Prism.highlightAll();

        // Run these after syntax highlighting
        setTimeout(() => {
            addLineNumbers();
            handleLongCode();
        }, 100);
    }
});