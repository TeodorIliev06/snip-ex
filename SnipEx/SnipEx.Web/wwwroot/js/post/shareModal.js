function openShareModal() {
    const modal = document.getElementById('shareModal');
    const urlInput = document.getElementById('shareUrlInput');

    if (!modal || !urlInput) {
        return;
    }

    urlInput.value = window.location.href;
    modal.classList.add('show');
    document.body.style.overflow = 'hidden';
}

function closeShareModal() {
    const modal = document.getElementById('shareModal');
    if (!modal) {
        return;
    }

    modal.classList.remove('show');
    document.body.style.overflow = 'auto';

    const copyBtn = document.getElementById('copyBtn');
    if (copyBtn) {
        copyBtn.classList.remove('copied');
        copyBtn.innerHTML = '<i class="fa fa-copy"></i><span>Copy</span>';
    }
}

function shareViaTwitter() {
    const url = getCurrentPostUrl();
    const title = getPostTitle();
    const text = encodeURIComponent(`Check out this code snippet: ${title}`);
    const shareUrl = `https://twitter.com/intent/tweet?text=${text}&url=${encodeURIComponent(url)}`;
    window.open(shareUrl, '_blank', 'width=600,height=400');
}

function shareViaLinkedIn() {
    const url = getCurrentPostUrl();
    const shareUrl = `https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(url)}`;
    window.open(shareUrl, '_blank', 'width=600,height=500');
}

function shareViaFacebook() {
    const url = getCurrentPostUrl();
    const shareUrl = `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(url)}`;
    window.open(shareUrl, '_blank', 'width=600,height=400');
}

function shareViaTelegram() {
    const url = getCurrentPostUrl();
    const title = getPostTitle();
    const text = encodeURIComponent(`${title}\n${url}`);
    const shareUrl = `https://t.me/share/url?url=${encodeURIComponent(url)}&text=${text}`;
    window.open(shareUrl, '_blank', 'width=600,height=400');
}

function shareViaEmail() {
    const url = getCurrentPostUrl();
    const title = getPostTitle();
    const subject = encodeURIComponent(`Check out: ${title}`);
    const body = encodeURIComponent(`I thought you might find this code snippet interesting:\n\n${title}\n${url}`);
    const shareUrl = `mailto:?subject=${subject}&body=${body}`;
    window.location.href = shareUrl;
}

async function copyLinkToClipboard() {
    const url = getCurrentPostUrl();
    try {
        await navigator.clipboard.writeText(url);
        showToast('Link copied to clipboard!', 'success');
    } catch (err) {
        console.error('Failed to copy link:', err);
        showToast('Failed to copy link', 'error');
    }
}

async function copyFromInput() {
    const urlInput = document.getElementById('shareUrlInput');
    const copyBtn = document.getElementById('copyBtn');

    if (!urlInput || !copyBtn) {
        return;
    }

    try {
        await navigator.clipboard.writeText(urlInput.value);

        copyBtn.classList.add('copied');
        copyBtn.innerHTML = '<i class="fa fa-check"></i><span>Copied!</span>';

        setTimeout(() => {
            copyBtn.classList.remove('copied');
            copyBtn.innerHTML = '<i class="fa fa-copy"></i><span>Copy</span>';
        }, 2000);
    } catch (err) {
        console.error('Failed to copy from input:', err);
        showToast('Failed to copy link', 'error');
    }
}

// Utility Functions
function getCurrentPostUrl() {
    return window.location.href;
}

function getPostTitle() {
    const titleElement = document.querySelector('.post-title');
    return titleElement ? titleElement.textContent : 'Code Snippet';
}

function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.innerHTML = `
        <i class="fa fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>
        <span>${message}</span>
    `;

    document.body.appendChild(toast);

    setTimeout(() => toast.classList.add('show'), 100);

    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

document.addEventListener('DOMContentLoaded', function () {
    const shareButton = document.querySelector('.share-button');
    if (shareButton) {
        shareButton.removeAttribute('onclick');
        shareButton.addEventListener('click', openShareModal);
        console.log('ShareModal: Share button listener attached');
    }

    const closeBtn = document.querySelector('.share-close-btn');
    if (closeBtn) {
        closeBtn.removeAttribute('onclick');
        closeBtn.addEventListener('click', closeShareModal);
        console.log('ShareModal: Close button listener attached');
    }

    const twitterOption = document.querySelector('.share-option.twitter');
    if (twitterOption) {
        twitterOption.removeAttribute('onclick');
        twitterOption.addEventListener('click', shareViaTwitter);
    }

    const linkedinOption = document.querySelector('.share-option.linkedin');
    if (linkedinOption) {
        linkedinOption.removeAttribute('onclick');
        linkedinOption.addEventListener('click', shareViaLinkedIn);
    }

    const facebookOption = document.querySelector('.share-option.facebook');
    if (facebookOption) {
        facebookOption.removeAttribute('onclick');
        facebookOption.addEventListener('click', shareViaFacebook);
    }

    const telegramOption = document.querySelector('.share-option.telegram');
    if (telegramOption) {
        telegramOption.removeAttribute('onclick');
        telegramOption.addEventListener('click', shareViaTelegram);
    }

    const emailOption = document.querySelector('.share-option.email');
    if (emailOption) {
        emailOption.removeAttribute('onclick');
        emailOption.addEventListener('click', shareViaEmail);
    }

    const copyLinkOption = document.querySelector('.share-option.copy');
    if (copyLinkOption) {
        copyLinkOption.removeAttribute('onclick');
        copyLinkOption.addEventListener('click', copyLinkToClipboard);
    }

    const copyBtn = document.getElementById('copyBtn');
    if (copyBtn) {
        copyBtn.removeAttribute('onclick');
        copyBtn.addEventListener('click', copyFromInput);
    }

    const shareModal = document.getElementById('shareModal');
    if (shareModal) {
        shareModal.addEventListener('click', function (e) {
            if (e.target === this) {
                closeShareModal();
            }
        });
    }

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            const modal = document.getElementById('shareModal');
            if (modal && modal.classList.contains('show')) {
                closeShareModal();
            }
        }
    });
});