document.addEventListener('DOMContentLoaded', function () {
    // Get all reply buttons
    const replyButtons = document.querySelectorAll('.comment-action.reply-button, .comment-action[title="Reply"]');
    const cancelButtons = document.querySelectorAll('.cancel-reply');
    const replyForms = document.querySelectorAll('.reply-form');

    // Attach event listeners to reply buttons
    replyButtons.forEach(button => {
        button.addEventListener('click', function () {
            const commentId = this.dataset.commentId || this.closest('.comment').querySelector('.like-button').dataset.commentId;

            // Hide all reply forms first
            document.querySelectorAll('.reply-form-container').forEach(form => {
                form.style.display = 'none';
            });

            // Show the specific reply form
            const replyForm = document.getElementById(`reply-form-${commentId}`);
            if (replyForm) {
                replyForm.style.display = 'block';
                // Focus on the textarea
                replyForm.querySelector('textarea').focus();
            }
        });
    });

    // Attach event listeners to cancel buttons
    cancelButtons.forEach(button => {
        button.addEventListener('click', function () {
            const commentId = this.dataset.commentId;
            const replyForm = document.getElementById(`reply-form-${commentId}`);
            if (replyForm) {
                replyForm.style.display = 'none';
            }
        });
    });

    // Handle form submissions
    replyForms.forEach(form => {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            const postId = this.dataset.postId;
            const parentCommentId = this.dataset.parentCommentId;
            const content = this.querySelector('textarea[name="Content"]').value;
            const userId = document.getElementById('currentUserId').value;

            // Prepare the data
            const replyData = {
                Content: content,
                PostId: postId,
                UserId: userId,
                ParentCommentId: parentCommentId
            };

            // Send the data to the server
            fetch('https://localhost:7000/CommentApi/AddReply', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(replyData)
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Failed to add reply');
                    }
                    return response.ok;
                })
                .then(success => {
                    if (success) {
                        // Refresh comments section to show the new reply
                        refreshComments(postId);

                        // Clear and hide the form
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

    // Function to refresh comments
    function refreshComments(postId) {
        fetch(`https://localhost:7000/CommentApi/GetComments/${postId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to get comments');
                }
                return response.json();
            })
            .then(comments => {
                // This would ideally rebuild the comments DOM
                // For simplicity, we'll just reload the page
                window.location.reload();
            })
            .catch(error => {
                console.error('Error refreshing comments:', error);
            });
    }
});