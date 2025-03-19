document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('click', function (e) {
        // Handle toggle replies button clicks
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

        // Handle reply button clicks
        if (e.target.closest('.comment-action[title="Reply"]') || e.target.closest('.comment-action.reply-button')) {
            const button = e.target.closest('.comment-action[title="Reply"]') || e.target.closest('.comment-action.reply-button');
            const commentId = button.dataset.commentId ||
                button.closest('.comment').querySelector('.like-button').dataset.commentId;

            document.querySelectorAll('.reply-form-container').forEach(form => {
                form.style.display = 'none';
                form.classList.remove('visible');
            });

            const replyForm = document.getElementById(`reply-form-${commentId}`);
            if (replyForm) {
                replyForm.style.display = 'block';
                setTimeout(() => {
                    replyForm.classList.add('visible');
                    replyForm.querySelector('textarea').focus();
                }, 10);
            }
        }

        // Handle cancel button clicks
        if (e.target.closest('.cancel-reply')) {
            const button = e.target.closest('.cancel-reply');
            const commentId = button.dataset.commentId;
            const replyForm = document.getElementById(`reply-form-${commentId}`);
            if (replyForm) {
                replyForm.classList.remove('visible');
                setTimeout(() => {
                    replyForm.style.display = 'none';
                }, 300);
            }
        }
    });

    // Handle form submissions
    document.querySelectorAll('.reply-form').forEach(form => {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            const postId = this.dataset.postId;
            const parentCommentId = this.dataset.parentCommentId;
            const content = this.querySelector('textarea[name="Content"]').value;
            const userId = document.getElementById('currentUserId').value;

            const replyData = { Content: content, PostId: postId, UserId: userId, ParentCommentId: parentCommentId };

            fetch('https://localhost:7000/CommentApi/AddReply', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(replyData)
            })
                .then(response => response.ok ? response.ok : Promise.reject('Failed to add reply'))
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

    function refreshComments(postId) {
        fetch(`https://localhost:7000/CommentApi/GetComments/${postId}`)
            .then(response => response.ok ? response.json() : Promise.reject('Failed to get comments'))
            .then(() => window.location.reload())
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
        `;
        document.head.appendChild(style);
    }
});

//document.addEventListener('DOMContentLoaded', function () {
//    // Get all reply buttons
//    const replyButtons = document.querySelectorAll('.comment-action.reply-button, .comment-action[title="Reply"]');
//    const cancelButtons = document.querySelectorAll('.cancel-reply');
//    const replyForms = document.querySelectorAll('.reply-form');

//    // Attach event listeners to reply buttons with event delegation
//    document.addEventListener('click', function (e) {
//        // Handle reply button clicks
//        if (e.target.closest('.comment-action[title="Reply"]') || e.target.closest('.comment-action.reply-button')) {
//            const button = e.target.closest('.comment-action[title="Reply"]') || e.target.closest('.comment-action.reply-button');
//            const commentId = button.dataset.commentId ||
//                button.closest('.comment').querySelector('.like-button').dataset.commentId;

//            // Hide all reply forms first
//            document.querySelectorAll('.reply-form-container').forEach(form => {
//                form.style.display = 'none';
//                form.classList.remove('visible');
//            });

//            // Show the specific reply form with animation
//            const replyForm = document.getElementById(`reply-form-${commentId}`);
//            if (replyForm) {
//                replyForm.style.display = 'block';
//                // Small delay for the animation to work properly
//                setTimeout(() => {
//                    replyForm.classList.add('visible');
//                    // Focus on the textarea
//                    replyForm.querySelector('textarea').focus();
//                }, 10);
//            }
//        }

//        // Handle cancel button clicks
//        if (e.target.closest('.cancel-reply')) {
//            const button = e.target.closest('.cancel-reply');
//            const commentId = button.dataset.commentId;
//            const replyForm = document.getElementById(`reply-form-${commentId}`);
//            if (replyForm) {
//                replyForm.classList.remove('visible');
//                // Wait for animation to complete before hiding
//                setTimeout(() => {
//                    replyForm.style.display = 'none';
//                }, 300);
//            }
//        }
//    });

//    // Handle form submissions
//    replyForms.forEach(form => {
//        form.addEventListener('submit', function (e) {
//            e.preventDefault();

//            const postId = this.dataset.postId;
//            const parentCommentId = this.dataset.parentCommentId;
//            const content = this.querySelector('textarea[name="Content"]').value;
//            const userId = document.getElementById('currentUserId').value;

//            // Prepare the data
//            const replyData = {
//                Content: content,
//                PostId: postId,
//                UserId: userId,
//                ParentCommentId: parentCommentId
//            };

//            // Send the data to the server
//            fetch('https://localhost:7000/CommentApi/AddReply', {
//                method: 'POST',
//                headers: {
//                    'Content-Type': 'application/json'
//                },
//                body: JSON.stringify(replyData)
//            })
//                .then(response => {
//                    if (!response.ok) {
//                        throw new Error('Failed to add reply');
//                    }
//                    return response.ok;
//                })
//                .then(success => {
//                    if (success) {
//                        // Refresh comments section to show the new reply
//                        refreshComments(postId);

//                        // Clear and hide the form
//                        this.querySelector('textarea').value = '';
//                        this.closest('.reply-form-container').style.display = 'none';
//                    }
//                })
//                .catch(error => {
//                    console.error('Error:', error);
//                    alert('Failed to add reply. Please try again.');
//                });
//        });
//    });

//    // Function to refresh comments
//    function refreshComments(postId) {
//        fetch(`https://localhost:7000/CommentApi/GetComments/${postId}`)
//            .then(response => {
//                if (!response.ok) {
//                    throw new Error('Failed to get comments');
//                }
//                return response.json();
//            })
//            .then(comments => {
//                // This would ideally rebuild the comments DOM
//                // For simplicity, we'll just reload the page
//                window.location.reload();
//            })
//            .catch(error => {
//                console.error('Error refreshing comments:', error);
//            });
//    }
//});