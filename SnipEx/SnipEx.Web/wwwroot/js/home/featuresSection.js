document.addEventListener('DOMContentLoaded', function () {
    initTrendingTags();

    initSnippetCarousel();

    initStatsCounter();
});

function initTrendingTags() {
    const tagCloud = document.getElementById('tag-cloud');
    const tags = [
        { text: '#JavaScript', size: 1.2 },
        { text: '#Python', size: 1.4 },
        { text: '#React', size: 1.3 },
        { text: '#CSharp', size: 1.2 },
        { text: '#TypeScript', size: 1.1 },
        { text: '#Java', size: 1.0 },
        { text: '#HTML', size: 0.9 },
        { text: '#CSS', size: 0.9 },
        { text: '#Angular', size: 1.0 },
        { text: '#Vue', size: 1.1 },
        { text: '#Node', size: 1.2 },
        { text: '#PHP', size: 0.8 },
        { text: '#Ruby', size: 0.8 },
        { text: '#Go', size: 1.0 },
        { text: '#SQL', size: 1.1 }
    ];

    tags.forEach(tag => {
        const tagElement = document.createElement('div');
        tagElement.className = 'tag';
        tagElement.textContent = tag.text;
        tagElement.style.fontSize = `${tag.size}rem`;
        tagElement.style.opacity = '0';
        tagCloud.appendChild(tagElement);

        positionTagRandomly(tagElement);

        tagElement.addEventListener('click', () => {
            alert(`Filtering by ${tag.text}`);
            // Implementation would go here to filter snippets by tag
        });
    });

    animateTagCloud();
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