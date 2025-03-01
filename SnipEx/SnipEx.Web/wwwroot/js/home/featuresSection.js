// Wait for DOM to load
document.addEventListener('DOMContentLoaded', function () {
    // Initialize trending tags
    initTrendingTags();

    // Initialize snippet carousel
    initSnippetCarousel();

    // Initialize stats counter
    initStatsCounter();
});
// Trending Tags Animation
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

    // Create tag elements
    tags.forEach(tag => {
        const tagElement = document.createElement('div');
        tagElement.className = 'tag';
        tagElement.textContent = tag.text;
        tagElement.style.fontSize = `${tag.size}rem`;
        tagElement.style.opacity = '0';
        tagCloud.appendChild(tagElement);

        // Position tag randomly
        positionTagRandomly(tagElement);

        // Add click event for filtering
        tagElement.addEventListener('click', () => {
            alert(`Filtering by ${tag.text}`);
            // Implementation would go here to filter snippets by tag
        });
    });

    // Animate tags
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

    // Sample snippets (Replace with API call in the future)
    const snippets = [
        { title: 'React useState Hook', language: 'JavaScript', code: 'import React, { useState } from "react";\n\nfunction Counter() {\n  const [count, setCount] = useState(0);\n  return (\n    <button onClick={() => setCount(count + 1)}>\n      Count: {count}\n    </button>\n  );\n}', author: '@reactdev', date: '2 hours ago' },
        { title: 'Python List Comprehension', language: 'Python', code: 'numbers = [1, 2, 3, 4, 5]\n\n# Get squares of all numbers\nsquares = [x**2 for x in numbers]\n\n# Get even numbers only\nevens = [x for x in numbers if x % 2 == 0]', author: '@pythonista', date: '4 hours ago' },
        { title: 'C# LINQ Query', language: 'C#', code: 'var numbers = new List<int> { 1, 2, 3, 4, 5 };\n\nvar evenNumbers = numbers\n    .Where(n => n % 2 == 0)\n    .Select(n => n * n)\n    .ToList();', author: '@csharpdev', date: '6 hours ago' },
        { title: 'CSS Grid Layout', language: 'CSS', code: '.container {\n  display: grid;\n  grid-template-columns: repeat(3, 1fr);\n  grid-gap: 20px;\n}\n\n.item {\n  padding: 20px;\n  background-color: #f0f0f0;\n}', author: '@cssmaster', date: '8 hours ago' },
        { title: 'SQL JOIN Query', language: 'SQL', code: 'SELECT u.username, p.title, p.created_at\nFROM users u\nINNER JOIN posts p\n  ON u.id = p.user_id\nWHERE p.created_at > DATE_SUB(NOW(), INTERVAL 1 DAY)\nORDER BY p.created_at DESC;', author: '@datadev', date: '10 hours ago' }
    ];

    // Create snippet cards dynamically
    snippets.forEach(snippet => {
        const snippetCard = createSnippetCard(snippet);
        carousel.appendChild(snippetCard);
    });

    // **Carousel state**
    let position = 0;
    const visibleCards = 3; // Show 3 cards at a time
    const cardWidth = 370; // Width of a single card (including margin)
    const totalCards = snippets.length;

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
            position += visibleCards; // Move forward by 3 cards
            updateCarouselPosition();
        }
    }

    function movePrev() {
        if (position > 0) {
            position -= visibleCards; // Move backward by 3 cards
            updateCarouselPosition();
        }
    }

    // **Attach event listeners**
    nextButton.addEventListener('click', moveNext);
    prevButton.addEventListener('click', movePrev);

    // Initialize button states
    updateCarouselPosition();
}

// Function to create snippet card
function createSnippetCard(snippet) {
    const card = document.createElement('div');
    card.className = 'snippet-card';

    const header = document.createElement('div');
    header.className = 'snippet-header';

    const title = document.createElement('div');
    title.className = 'snippet-title';
    title.textContent = snippet.title;

    const language = document.createElement('div');
    language.className = 'snippet-language';
    language.textContent = snippet.language;

    header.appendChild(title);
    header.appendChild(language);

    const content = document.createElement('div');
    content.className = 'snippet-content';
    content.textContent = snippet.code;

    const footer = document.createElement('div');
    footer.className = 'snippet-footer';

    const author = document.createElement('div');
    author.textContent = snippet.author;

    const date = document.createElement('div');
    date.textContent = snippet.date;

    footer.appendChild(author);
    footer.appendChild(date);

    card.appendChild(header);
    card.appendChild(content);
    card.appendChild(footer);

    return card;
}


// Stats Counter
function initStatsCounter() {
    const developerCount = document.getElementById('developer-count');
    const snippetCount = document.getElementById('snippet-count');
    const commentCount = document.getElementById('comment-count');

    // Target values
    const targetDevs = 2500;
    const targetSnippets = 8900;
    const targetComments = 120;

    // Animate counters
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