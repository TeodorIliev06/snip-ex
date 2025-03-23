document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('click', function (e) {
        const toggleButton = e.target.closest('.toggle-replies-btn') ||
            e.target.closest('[data-target^="replies-"]');

        if (toggleButton) {
            const commentId = toggleButton.dataset.commentId ||
                toggleButton.dataset.target?.replace('replies-', '');

            if (commentId) {
                const repliesContainer = document.getElementById(`replies-${commentId}`);
                const toggleTextElement = toggleButton.querySelector('.toggle-text') || toggleButton;
                const iconElement = toggleButton.querySelector('i.fa-chevron-down') ||
                    toggleButton.querySelector('i.fa-chevron-up');

                if (repliesContainer) {
                    if (repliesContainer.style.display === 'none' || !repliesContainer.style.display) {
                        repliesContainer.style.display = 'block';
                        setTimeout(() => {
                            repliesContainer.classList.add('visible');
                        }, 10);

                        if (toggleTextElement) {
                            toggleTextElement.textContent = toggleTextElement.textContent.replace('Show', 'Hide');
                        }
                        if (iconElement) {
                            iconElement.classList.replace('fa-chevron-down', 'fa-chevron-up');
                        }
                    } else {
                        repliesContainer.classList.remove('visible');
                        setTimeout(() => {
                            repliesContainer.style.display = 'none';
                        }, 300);

                        if (toggleTextElement) {
                            toggleTextElement.textContent = toggleTextElement.textContent.replace('Hide', 'Show');
                        }
                        if (iconElement) {
                            iconElement.classList.replace('fa-chevron-up', 'fa-chevron-down');
                        }
                    }
                }
            }
        }

        if (e.target.closest('.comment-action[title="Reply"]') || e.target.closest('.comment-action.reply-button')) {
            const button = e.target.closest('.comment-action[title="Reply"]') || e.target.closest('.comment-action.reply-button');
            const commentElement = button.closest('.comment');
            const commentId = button.dataset.commentId || commentElement.querySelector('.like-button').dataset.commentId;

            const usernameElement = commentElement.querySelector('.comment-author');
            const username = usernameElement ? usernameElement.textContent.trim() : '';

            document.querySelectorAll('.reply-form-container').forEach(form => {
                form.style.display = 'none';
                form.classList.remove('visible');
            });

            const replyForm = document.getElementById(`reply-form-${commentId}`);
            if (replyForm) {
                const textarea = replyForm.querySelector('textarea');

                const referenceCommentId = button.dataset.referenceCommentId;
                const isReplyToReply = referenceCommentId !== undefined && referenceCommentId !== null;

                if (isReplyToReply && username) {
                    textarea.value = `@${username} `;
                }

                replyForm.style.display = 'block';
                setTimeout(() => {
                    replyForm.classList.add('visible');
                    textarea.focus();

                    if (textarea.value) {
                        textarea.selectionStart = textarea.selectionEnd = textarea.value.length;
                    }
                }, 10);
            }
        }

        if (e.target.closest('.cancel-reply')) {
            const button = e.target.closest('.cancel-reply');
            const commentId = button.dataset.commentId;
            const replyForm = document.getElementById(`reply-form-${commentId}`);
            if (replyForm) {
                replyForm.classList.remove('visible');
                setTimeout(() => {
                    replyForm.style.display = 'none';
                    replyForm.querySelector('textarea').value = '';
                }, 300);
            }
        }
    });

    document.querySelectorAll('.reply-form').forEach(form => {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            const postId = this.dataset.postId;
            const parentCommentId = this.dataset.parentCommentId;
            const content = this.querySelector('textarea[name="Content"]').value;
            const userId = document.getElementById('currentUserId').value;

            const replyData = {
                Content: content,
                PostId: postId,
                UserId: userId,
                ParentCommentId: parentCommentId
            };

            fetch('https://localhost:7000/CommentApi/AddReply', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify(replyData)
            })
            .then(response => {
                if (response.status === 401) {
                    // User is not authenticated
                    window.location.href = '/Identity/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
                    return null;
                }
                return response.ok ? response.ok : Promise.reject('Failed to add reply');
            })
            .then(success => {
                if (success) {
                    refreshComments(postId);
                    this.querySelector('textarea').value = '';
                    this.closest('.reply-form-container').style.display = 'none';
                }
            })
            .catch(error => {
                console.error('Error:', error);
                alert('Failed to add reply. Please try again.');
            });
        });
    });

    function parseAndHighlightMentions() {
        document.querySelectorAll('.comment-content, .reply-content').forEach(contentElement => {
            const content = contentElement.innerHTML;
            const mentionPattern = /@([\w.]+(?:@[\w.]+\.\w+)?)/g;

            // Replace @mentions with highlighted versions
            const highlightedContent = content.replace(mentionPattern, '<span class="user-mention">@$1</span>');
            contentElement.innerHTML = highlightedContent;

            // Add click event listeners to mentions
            contentElement.querySelectorAll('.user-mention').forEach(mention => {
                mention.addEventListener('click', function () {
                    const username = this.textContent.substring(1); // Remove @ symbol
                    console.log(`Clicked on mention: ${username}`);
                });
            });
        });
    }

    function refreshComments(postId) {
        fetch(`https://localhost:7000/CommentApi/GetComments/${postId}`, {
            credentials: 'include'
        })
            .then(response => response.ok ? response.json() : Promise.reject('Failed to get comments'))
            .then(() => {
                window.location.reload();

                window.addEventListener('load', function () {
                    parseAndHighlightMentions();
                });
            })
            .catch(error => console.error('Error refreshing comments:', error));
    }

    document.querySelectorAll('.replies-container').forEach(container => {
        container.style.display = 'none';
    });

    if (!document.getElementById('comments-animation-styles')) {
        const style = document.createElement('style');
        style.id = 'comments-animation-styles';
        style.textContent = `
            .replies-container, .reply-form-container {
                opacity: 0;
                transition: opacity 0.3s ease-in-out;
            }
            .replies-container.visible, .reply-form-container.visible {
                opacity: 1;
            }
            .user-mention {
                color: #1DA1F2;
                font-weight: 500;
                cursor: pointer;
            }
            .user-mention:hover {
                text-decoration: underline;
            }
        `;
        document.head.appendChild(style);
    }

    parseAndHighlightMentions();
});