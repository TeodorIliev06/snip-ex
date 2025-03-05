document.addEventListener('DOMContentLoaded', function () {
    initTrendingTags();

    initSnippetCarousel();

    initStatsCounter();
});

function initTrendingTags() {
    const tagCloud = document.getElementById('tag-cloud');
    const tags = Array.from(tagCloud.querySelectorAll('.tag'));

    tags.forEach(tagElement => {
        const size = calculateTagSize(tagElement);
        tagElement.style.fontSize = `${size}rem`;
        tagElement.style.opacity = '0';
        positionTagRandomly(tagElement);
    });

    animateTagCloud();
}

function calculateTagSize(tagElement) {
    const baseSize = 0.8;
    const variation = 0.4;
    return baseSize + (Math.random() * variation);
}

function positionTagRandomly(tagElement) {
    const cloudWidth = document.getElementById('tag-cloud').offsetWidth;
    const cloudHeight = document.getElementById('tag-cloud').offsetHeight;

    const left = Math.random() * (cloudWidth - 150);
    const top = Math.random() * (cloudHeight - 50);

    tagElement.style.left = `${left}px`;
    tagElement.style.top = `${top}px`;
}

function animateTagCloud() {
    const tags = document.querySelectorAll('.tag');

    tags.forEach((tag, index) => {
        // Random delay for each tag
        setTimeout(() => {
            // Fade in
            fadeInTag(tag);

            // Move around slowly
            setInterval(() => {
                positionTagRandomly(tag);

                // Smooth transition
                tag.style.transition = 'left 3s ease, top 3s ease';
            }, 5000 + (index * 500));

            // Occasional fade out and in
            setInterval(() => {
                fadeOutTag(tag);
                setTimeout(() => fadeInTag(tag), 1000);
            }, 15000 + (index * 1000));

        }, index * 300);
    });
}

function fadeInTag(tag) {
    tag.style.opacity = '0';
    let opacity = 0;
    const interval = setInterval(() => {
        if (opacity < 0.9) {
            opacity += 0.1;
            tag.style.opacity = opacity;
        } else {
            clearInterval(interval);
        }
    }, 100);
}

function fadeOutTag(tag) {
    let opacity = 0.9;
    const interval = setInterval(() => {
        if (opacity > 0) {
            opacity -= 0.1;
            tag.style.opacity = opacity;
        } else {
            clearInterval(interval);
        }
    }, 100);
}

function initSnippetCarousel() {
    const carousel = document.getElementById('snippet-carousel');
    const prevButton = document.getElementById('prev-snippet');
    const nextButton = document.getElementById('next-snippet');

    let position = 0;
    const visibleCards = 3;
    const cardWidth = 390;
    const totalCards = carousel.children.length;

    function updateCarouselPosition() {
        carousel.style.transition = 'transform 0.5s ease';
        carousel.style.transform = `translateX(-${position * cardWidth}px)`;

        // **Disable buttons when at limits**
        prevButton.disabled = position === 0;
        nextButton.disabled = position >= totalCards - visibleCards;
        prevButton.style.opacity = prevButton.disabled ? "0.5" : "1";
        nextButton.style.opacity = nextButton.disabled ? "0.5" : "1";
    }

    function moveNext() {
        if (position + visibleCards < totalCards) {
            position += visibleCards;
            updateCarouselPosition();
        }
    }

    function movePrev() {
        if (position > 0) {
            position -= visibleCards;
            updateCarouselPosition();
        }
    }

    nextButton.addEventListener('click', moveNext);
    prevButton.addEventListener('click', movePrev);

    updateCarouselPosition();
}

// Stats Counter
function initStatsCounter() {
    const developerCount = document.getElementById('developer-count');
    const snippetCount = document.getElementById('snippet-count');
    const commentCount = document.getElementById('comment-count');

    const targetDevs = 2500;
    const targetSnippets = 8900;
    const targetComments = 120;

    animateCounter(developerCount, targetDevs, 2000);
    animateCounter(snippetCount, targetSnippets, 2000);
    animateCounter(commentCount, targetComments, 2000);
}

function animateCounter(element, target, duration) {
    let startTime = null;
    const startValue = 0;

    function step(timestamp) {
        if (!startTime) startTime = timestamp;
        const progress = Math.min((timestamp - startTime) / duration, 1);
        const value = Math.floor(progress * (target - startValue) + startValue);
        element.textContent = value.toLocaleString();

        if (progress < 1) {
            window.requestAnimationFrame(step);
        }
    }

    window.requestAnimationFrame(step);
}