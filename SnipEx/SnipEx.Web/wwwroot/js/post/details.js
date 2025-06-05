document.addEventListener('DOMContentLoaded', async function () {
    if (typeof Prism !== 'undefined') {
        Prism.highlightAll();

        setTimeout(() => {
            addCustomLineNumbers();
            handleLongCode();
        }, 100);
    }

    const copyButton = document.querySelector('.copy-button');
    if (copyButton) {
        copyButton.addEventListener('click', copyToClipboard);
    }

    const postEl = document.querySelector('.post-actions');
    const postId = postEl.getAttribute('data-post-id');
    await incrementPostViewsCount(postId);

    const postLikeButton = document.querySelector('.like-button');
    if (postLikeButton) {
        postLikeButton.addEventListener('click', async function () {
            await togglePostLike(postId);
        });
    }

    const postSaveButton = document.querySelector('.save-button');
    if (postSaveButton) {
        postSaveButton.addEventListener('click', async function () {
            await togglePostSave(postId);
        });
    }

    const commentLikeButtons = document.querySelectorAll('.like-button[data-comment-id]');
    commentLikeButtons.forEach(button => {
        const commentId = button.getAttribute('data-comment-id');
        button.addEventListener('click', async function () {
            await toggleCommentLike(commentId);
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

function addCustomLineNumbers() {
    const codeSnippets = document.querySelectorAll('.code-snippet');

    codeSnippets.forEach(snippet => {
        const pre = snippet.querySelector('pre');
        const code = snippet.querySelector('code');

        if (!pre || !code) return;

        const existingLineNumbers = snippet.querySelector('.line-numbers');
        if (existingLineNumbers) {
            existingLineNumbers.remove();
        }

        const lines = code.textContent.split('\n');
        const lineCount = lines.length;

        if (lineCount >= 100) {
            snippet.classList.add('three-digit-lines');
        } else {
            snippet.classList.remove('three-digit-lines');
        }

        const lineNumbersContainer = document.createElement('div');
        lineNumbersContainer.className = 'line-numbers';

        for (let i = 1; i <= lineCount; i++) {
            const lineNumber = document.createElement('span');
            lineNumber.className = 'line-number';
            lineNumber.textContent = i;
            lineNumbersContainer.appendChild(lineNumber);
        }

        snippet.appendChild(lineNumbersContainer);
    });
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

async function togglePostLike(postId) {
    await fetchWithToastr(`https://localhost:7000/UserActionApi/TogglePostLike/${postId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include'
    })
    .then(data => {
        if (!data) return;

        const likeButton = document.querySelector(`.like-button`);
        let likeCountSpan = likeButton.querySelector('.count');

        let currentCount = parseInt(likeCountSpan?.textContent || "0", 10);
        let newCount = data.isLiked ? currentCount + 1 : currentCount - 1;

        if (newCount >= 1) {
            if (!likeCountSpan) {
                likeCountSpan = document.createElement("span");
                likeCountSpan.classList.add("count");
                likeButton.appendChild(likeCountSpan);
            }
            likeCountSpan.textContent = newCount;
        } else if (likeCountSpan) {
            likeCountSpan.remove();
        }

        likeButton.classList.toggle('liked', data.isLiked);
    });
}

async function togglePostSave(postId) {
    await fetchWithToastr(`https://localhost:7000/UserActionApi/TogglePostSave/${postId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include'
    })
    .then(data => {
        if (!data) return;

        const saveButton = document.querySelector(`.save-button`);
        saveButton.classList.toggle('saved', data.isLiked);

        const spanElement = saveButton.querySelector('span');
        if (spanElement) {
            spanElement.textContent = data.isSaved ? 'Saved' : 'Save';
        }
    });
}

async function toggleCommentLike(commentId) {
    await fetchWithToastr(`https://localhost:7000/UserActionApi/ToggleCommentLike/${commentId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include'
    })
    .then(data => {
        if (!data) return;

        const likeButton = document.querySelector(`.like-button[data-comment-id="${commentId}"]`);
        let likeCountSpan = likeButton.querySelector('.count');

        let currentCount = parseInt(likeCountSpan?.textContent || "0", 10);
        let newCount = data.isLiked ? currentCount + 1 : currentCount - 1;

        if (newCount >= 1) {
            if (!likeCountSpan) {
                likeCountSpan = document.createElement("span");
                likeCountSpan.classList.add("count");
                likeButton.appendChild(likeCountSpan);
            }
            likeCountSpan.textContent = newCount;
        } else if (likeCountSpan) {
            likeCountSpan.remove();
        }

        likeButton.classList.toggle('liked', data.isLiked);
    });
}

async function incrementPostViewsCount(postId) {
    await fetchWithToastr(`https://localhost:7000/UserActionApi/IncrementPostViewsCount/${postId}`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include'
        })
        .then(data => {
            if (!data) return;
        });
}