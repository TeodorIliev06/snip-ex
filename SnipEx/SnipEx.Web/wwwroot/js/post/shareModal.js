function openShareModal() {
    const modal = document.getElementById('shareModal');
    const urlInput = document.getElementById('shareUrlInput');

    urlInput.value = window.location.href;

    modal.classList.add('show');
    document.body.style.overflow = 'hidden';
}

function closeShareModal() {
    const modal = document.getElementById('shareModal');
    modal.classList.remove('show');
    document.body.style.overflow = 'auto';

    const copyBtn = document.getElementById('copyBtn');
    copyBtn.classList.remove('copied');
    copyBtn.innerHTML = '<i class="fa fa-copy"></i><span>Copy</span>';
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

function copyLinkToClipboard() {
    const url = getCurrentPostUrl();
    copyToClipboard(url);
}

function copyFromInput() {
    const urlInput = document.getElementById('shareUrlInput');
    const copyBtn = document.getElementById('copyBtn');

    copyToClipboard(urlInput.value);

    // Visual feedback
    copyBtn.classList.add('copied');
    copyBtn.innerHTML = '<i class="fa fa-check"></i><span>Copied!</span>';

    setTimeout(() => {
        copyBtn.classList.remove('copied');
        copyBtn.innerHTML = '<i class="fa fa-copy"></i><span>Copy</span>';
    }, 2000);
}

async function copyToClipboard(text) {
    try {
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(text);
            showToast('Link copied to clipboard!', 'success');
        } else {
            fallbackCopyTextToClipboard(text);
        }
    } catch (err) {
        fallbackCopyTextToClipboard(text);
    }
}

function fallbackCopyTextToClipboard(text) {
    const textArea = document.createElement("textarea");
    textArea.value = text;
    textArea.style.position = "fixed";
    textArea.style.left = "-999999px";
    textArea.style.top = "-999999px";
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();

    try {
        document.execCommand('copy');
        showToast('Link copied to clipboard!', 'success');
    } catch (err) {
        showToast('Failed to copy link', 'error');
    }

    document.body.removeChild(textArea);
}

// Utility Functions
function getCurrentPostUrl() {
    return window.location.href;
}

function getPostTitle() {
    return document.querySelector('.post-title').textContent;
}

function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.innerHTML = `
                <i class="fa fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>
                <span>${message}</span>
            `;

    document.body.appendChild(toast);

    // Trigger animation
    setTimeout(() => toast.classList.add('show'), 100);

    // Remove after 3 seconds
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

document.getElementById('shareModal').addEventListener('click', function (e) {
    if (e.target === this) {
        closeShareModal();
    }
});

document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        closeShareModal();
    }
});