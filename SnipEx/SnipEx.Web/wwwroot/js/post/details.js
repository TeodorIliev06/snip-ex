document.addEventListener('DOMContentLoaded', function () {
    copyToClipboard();
});

function copyToClipboard() {
    const codeElement = document.querySelector('pre code');
    const textToCopy = codeElement.textContent;

    navigator.clipboard.writeText(textToCopy).then(() => {
        const copyButton = document.querySelector('.copy-button');
        copyButton.textContent = 'Copied!';
        setTimeout(() => {
            copyButton.textContent = 'Copy';
        }, 2000);
    }).catch(err => {
        console.error('Failed to copy: ', err);
    });
}