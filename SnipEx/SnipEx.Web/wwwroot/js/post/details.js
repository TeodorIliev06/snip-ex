let currentCommentSort = 'newest';

document.addEventListener('DOMContentLoaded', async function () {
    // Initialize Prism syntax highlighting
    if (typeof Prism !== 'undefined') {
        Prism.highlightAll();

        setTimeout(() => {
            addCustomLineNumbers();
            handleLongCode();
        }, 100);
    }

    // Initialize all functionality
    initializePostActions();
    initializeCodeCopy();
    initializeCommentSorting();

    formatCommentDates('.comment-date');

    // Increment post views
    const postEl = document.querySelector('.post-actions');
    const postId = postEl.getAttribute('data-post-id');
    await incrementPostViewsCount(postId);
});

/**
 * Initialize post action buttons (like, save)
 */
function initializePostActions() {
    const postEl = document.querySelector('.post-actions');
    const postId = postEl?.getAttribute('data-post-id');

    const postLikeButton = document.querySelector('.like-button:not([data-comment-id])');
    if (postLikeButton && postId) {
        postLikeButton.addEventListener('click', async function () {
            await togglePostLike(postId);
        });
    }

    const postSaveButton = document.querySelector('.save-button');
    if (postSaveButton && postId) {
        postSaveButton.addEventListener('click', async function () {
            await togglePostSave(postId);
        });
    }

    // Initialize comment like buttons
    const commentLikeButtons = document.querySelectorAll('.like-button[data-comment-id]');
    commentLikeButtons.forEach(button => {
        const commentId = button.getAttribute('data-comment-id');
        button.addEventListener('click', async function () {
            await toggleCommentLike(commentId);
        });
    });
}

/**
 * Initialize code copy functionality
 */
function initializeCodeCopy() {
    const copyButton = document.querySelector('.copy-btn');
    if (copyButton) {
        copyButton.addEventListener('click', function () {
            copyToClipboard();
        });
    }
}

/**
 * Initialize comment sorting functionality
 */
function initializeCommentSorting() {
    const sortButton = document.querySelector('.sort-comments');
    if (sortButton) {
        sortButton.addEventListener('click', function () {
            // Toggle between newest and oldest
            currentCommentSort = currentCommentSort === 'newest' ? 'oldest' : 'newest';

            // Update button text
            const icon = sortButton.querySelector('i');
            const text = currentCommentSort === 'newest' ? 'Sort by newest' : 'Sort by oldest';
            sortButton.innerHTML = `${icon.outerHTML} ${text}`;

            // Sort comments
            sortComments(currentCommentSort);
        });
    }
}

/**
 * Sort comments in the DOM
 * @param {string} sortOrder - 'newest' or 'oldest'
 */
function sortComments(sortOrder) {
    const commentsList = document.querySelector('.comments-list');
    if (!commentsList) return;

    // Get all top-level comment items (not replies)
    const comments = Array.from(commentsList.children).filter(child =>
        child.classList.contains('comment-item') && !child.closest('.comment-replies')
    );

    // Sort comments by timestamp
    comments.sort((a, b) => {
        const timestampA = a.getAttribute('data-timestamp') || a.querySelector('[data-timestamp]')?.getAttribute('data-timestamp');
        const timestampB = b.getAttribute('data-timestamp') || b.querySelector('[data-timestamp]')?.getAttribute('data-timestamp');

        if (!timestampA || !timestampB) return 0;

        const dateA = new Date(timestampA);
        const dateB = new Date(timestampB);

        if (sortOrder === 'oldest') {
            return dateA - dateB;
        }
        return dateB - dateA; // newest first
    });

    // Add sorting animation
    commentsList.style.opacity = '0.7';

    // Re-append sorted comments to the container
    comments.forEach(comment => commentsList.appendChild(comment));

    setTimeout(() => {
        commentsList.style.opacity = '1';
    }, 200);
}

/**
 * Copy code to clipboard
 */
function copyToClipboard() {
    const codeElement = document.querySelector('pre code');

    if (!codeElement) {
        return;
    }

    const textToCopy = codeElement.textContent;

    navigator.clipboard.writeText(textToCopy).then(() => {
        const copyButton = document.querySelector('.copy-btn');
        const originalText = copyButton.innerHTML;

        copyButton.innerHTML = '<i class="fa fa-check"></i> Copied!';
        copyButton.classList.add('copied');

        setTimeout(() => {
            copyButton.innerHTML = originalText;
            copyButton.classList.remove('copied');
        }, 2000);
    }).catch(err => {
        console.error('Failed to copy: ', err);
        // Fallback for older browsers
        try {
            const textArea = document.createElement('textarea');
            textArea.value = textToCopy;
            document.body.appendChild(textArea);
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);

            const copyButton = document.querySelector('.copy-btn');
            const originalText = copyButton.innerHTML;
            copyButton.innerHTML = '<i class="fa fa-check"></i> Copied!';
            copyButton.classList.add('copied');

            setTimeout(() => {
                copyButton.innerHTML = originalText;
                copyButton.classList.remove('copied');
            }, 2000);
        } catch (fallbackErr) {
            console.error('Fallback copy failed: ', fallbackErr);
            const copyButton = document.querySelector('.copy-btn');
            copyButton.innerHTML = '<i class="fa fa-times"></i> Failed to copy';
            setTimeout(() => {
                copyButton.innerHTML = '<i class="fa fa-copy"></i> Copy';
            }, 2000);
        }
    });
}

/**
 * Add custom line numbers to code blocks
 */
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

/**
 * Check if code is too long and add a "Show more" button
 */
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

        codeContainer.parentNode.insertBefore(expandButton, codeContainer.nextSibling);
    }
}

/**
 * Toggle post like status
 */
async function togglePostLike(postId) {
    await fetchWithToastr(`https://localhost:7000/UserActionApi/TogglePostLike/${postId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
        credentials: 'include'
    })
        .then(data => {
            if (!data) return;

            const likeButton = document.querySelector(`.like-button:not([data-comment-id])`);
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

/**
 * Toggle post save status
 */
async function togglePostSave(postId) {
    await fetchWithToastr(`https://localhost:7000/UserActionApi/TogglePostSave/${postId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
        credentials: 'include'
    })
        .then(data => {
            if (!data) return;

            const saveButton = document.querySelector(`.save-button`);
            saveButton.classList.toggle('saved', data.isSaved);

            const spanElement = saveButton.querySelector('span');
            if (spanElement) {
                spanElement.textContent = data.isSaved ? 'Saved' : 'Save';
            }
        });
}

/**
 * Toggle comment like status
 */
async function toggleCommentLike(commentId) {
    await fetchWithToastr(`https://localhost:7000/UserActionApi/ToggleCommentLike/${commentId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
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

/**
 * Increment post views count
 */
async function incrementPostViewsCount(postId) {
    await fetchWithToastr(`https://localhost:7000/UserActionApi/IncrementPostViewsCount/${postId}`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
        credentials: 'include'
    })
        .then(data => {
            if (!data) return;
        });
}

/**
 * Format comment dates to relative time for elements with data-timestamp attribute
 * @param {string} selector - CSS selector for comment date elements (default: '.comment-date')
 */
function formatCommentDates(selector = '.comment-date') {
    formatDates(selector);
}

/**
 * Sort comments by date (newest first or oldest first)
 * @param {Array} comments - Array of comment objects with CreatedAt property
 * @param {string} sortOrder - 'newest' or 'oldest'
 * @returns {Array} Sorted comments array
 */
function sortCommentsByDate(comments, sortOrder = 'newest') {
    return [...comments].sort((a, b) => {
        const dateA = new Date(a.CreatedAt);
        const dateB = new Date(b.CreatedAt);

        if (sortOrder === 'oldest') {
            return dateA - dateB;
        }
        return dateB - dateA; // newest first
    });
}

/**
 * Sort DOM elements containing comments by their data-timestamp attribute
 * @param {string} containerSelector - Selector for the container holding comment elements
 * @param {string} commentSelector - Selector for individual comment elements
 * @param {string} sortOrder - 'newest' or 'oldest'
 */
function sortCommentElementsByDate(containerSelector = '.comments-list', commentSelector = '.comment-item', sortOrder = 'newest') {
    const container = document.querySelector(containerSelector);
    if (!container) return;

    const comments = Array.from(container.querySelectorAll(commentSelector));

    comments.sort((a, b) => {
        const timestampA = a.getAttribute('data-timestamp') || a.querySelector('[data-timestamp]')?.getAttribute('data-timestamp');
        const timestampB = b.getAttribute('data-timestamp') || b.querySelector('[data-timestamp]')?.getAttribute('data-timestamp');

        if (!timestampA || !timestampB) return 0;

        const dateA = new Date(timestampA);
        const dateB = new Date(timestampB);

        if (sortOrder === 'oldest') {
            return dateA - dateB;
        }
        return dateB - dateA; // newest first
    });

    // Re-append sorted comments to container
    comments.forEach(comment => container.appendChild(comment));
}

/**
 * Initialize date formatting for the current page
 * Call this when the page loads or after dynamic content is added
 */
function initializeDateFormatting() {
    // Format post dates
    formatDates('.post-date');

    // Format comment dates
    formatCommentDates('.comment-date');

    // Re-format dates when new content is dynamically added
    const observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                // Check if any added nodes contain date elements
                mutation.addedNodes.forEach(function (node) {
                    if (node.nodeType === 1) { // Element node
                        const postDates = node.querySelectorAll ? node.querySelectorAll('.post-date') : [];
                        const commentDates = node.querySelectorAll ? node.querySelectorAll('.comment-date') : [];

                        postDates.forEach(dateElement => {
                            const timestampStr = dateElement.getAttribute('data-timestamp');
                            if (timestampStr) {
                                dateElement.textContent = getRelativeTimeString(timestampStr);
                            }
                        });

                        commentDates.forEach(dateElement => {
                            const timestampStr = dateElement.getAttribute('data-timestamp');
                            if (timestampStr) {
                                dateElement.textContent = getRelativeTimeString(timestampStr);
                            }
                        });
                    }
                });
            }
        });
    });

    // Start observing
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
}

// Auto-initialize when DOM is loaded
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeDateFormatting);
} else {
    initializeDateFormatting();
}