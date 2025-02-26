document.addEventListener('DOMContentLoaded', function () {
    const editor = CodeMirror.fromTextArea(document.getElementById("code-editor"), {
        lineNumbers: true,
    mode: "clike", // Default to C# highlighting
    theme: "dracula",
    indentUnit: 4,
    tabSize: 4,
    lineWrapping: true,
    autoCloseBrackets: true,
        matchBrackets: true,
        readOnly: false,
        });

    // Language mappings (CodeMirror mode -> language name for backend)
    const languageMappings = {
        "clike": "csharp",
    "javascript": "javascript",
    "htmlmixed": "html",
    "css": "css",
    "python": "python",
    "java": "java",
    "php": "php",
    "ruby": "ruby",
    "go": "go",
    "rust": "rust",
    "sql": "sql"
        };

    // Handle language selection change
    document.getElementById('language-selector').addEventListener('change', function() {
            const selectedMode = this.value;
    editor.setOption('mode', selectedMode);

    // Update hidden language field
    document.getElementById('selected-language').value = languageMappings[selectedMode] || selectedMode;
        });

    // Copy code button
    document.getElementById('copy-code-btn').addEventListener('click', function() {
            const code = editor.getValue();
    navigator.clipboard.writeText(code)
                .then(() => {
        this.innerHTML = '<i class="fas fa-check"></i> Copied!';
                    setTimeout(() => {
        this.innerHTML = '<i class="fas fa-copy"></i> Copy';
                    }, 2000);
                })
                .catch(() => {
        this.innerHTML = '<i class="fas fa-times"></i> Failed';
                    setTimeout(() => {
        this.innerHTML = '<i class="fas fa-copy"></i> Copy';
                    }, 2000);
                });
        });

    // Handle preview toggle
    document.getElementById('code-view-btn').addEventListener('click', function() {
        this.classList.add('active');
    document.getElementById('terminal-view-btn').classList.remove('active');
    document.getElementById('terminal-preview').style.display = 'none';
    document.querySelector('.editor-container').style.display = 'block';
        });

    document.getElementById('terminal-view-btn').addEventListener('click', function() {
        this.classList.add('active');
    document.getElementById('code-view-btn').classList.remove('active');
    document.getElementById('terminal-preview').style.display = 'block';
    document.querySelector('.editor-container').style.display = 'none';

    // Update terminal preview with current code
    const code = editor.getValue();
    const terminalContent = document.querySelector('.terminal-content');
    terminalContent.innerHTML = '';

    if (code.trim()) {
                const lines = code.split('\n');
                lines.forEach(line => {
                    const terminalLine = document.createElement('div');
    terminalLine.className = 'terminal-line';
    terminalLine.textContent = line;
    terminalContent.appendChild(terminalLine);
                });
            } else {
                const terminalLine = document.createElement('div');
    terminalLine.className = 'terminal-line';
    terminalLine.textContent = 'No code to preview yet...';
    terminalContent.appendChild(terminalLine);
            }
        });

    // Handle tag management
    const tagsContainer = document.getElementById('tags-container');
    const tagInput = document.getElementById('tag-input');
    const addTagButton = document.getElementById('add-tag-btn');
    const tagIdsContainer = document.getElementById('tag-ids-container');

    // Tags storage
    let tags = [];

    // Add tag function
    function addTag() {
            const tagValue = tagInput.value.trim().toLowerCase();

    // Validate tag (non-empty and not a duplicate)
    if (tagValue && !tags.includes(tagValue)) {
        tags.push(tagValue);

    // Create visual tag element
    const tagElement = document.createElement('div');
    tagElement.className = 'tag';
    tagElement.innerHTML = `${tagValue} <span class="tag-remove" data-tag="${tagValue}">×</span>`;

    // Add event listener for tag removal
    tagElement.querySelector('.tag-remove').addEventListener('click', function() {
                    const tagToRemove = this.getAttribute('data-tag');
                    tags = tags.filter(tag => tag !== tagToRemove);
    tagElement.remove();
    updateHiddenTagFields();
                });

    // Add tag to container
    tagsContainer.appendChild(tagElement);

    // Update hidden fields
    updateHiddenTagFields();

    // Clear input
    tagInput.value = '';
            } else if (tags.includes(tagValue)) {
        // Flash input to indicate duplicate
        tagInput.style.borderColor = '#f85149';
                setTimeout(() => {
        tagInput.style.borderColor = '';
                }, 1000);
            }
        }

    // Update hidden fields for form submission
    function updateHiddenTagFields() {
        // Clear previous hidden fields
        tagIdsContainer.innerHTML = '';

            // Add hidden fields for each tag
            tags.forEach((tag, index) => {
                const hiddenField = document.createElement('input');
    hiddenField.type = 'hidden';
    hiddenField.name = `Tags[${index}]`;
    hiddenField.value = tag;
    tagIdsContainer.appendChild(hiddenField);
            });
        }

    // Add tag on button click
    addTagButton.addEventListener('click', addTag);

    // Add tag on Enter key
    tagInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
        e.preventDefault();
    addTag();
            }
        });

    // Save draft functionality
    document.getElementById('save-draft-btn').addEventListener('click', function() {
            // Add a draft flag to the form
            const draftInput = document.createElement('input');
    draftInput.type = 'hidden';
    draftInput.name = 'IsDraft';
    draftInput.value = 'true';
    document.querySelector('form').appendChild(draftInput);

    // Submit the form
    document.querySelector('form').submit();
        });

    // Ensure content field is updated with editor content on form submit
    document.querySelector('form').addEventListener('submit', function() {
        // Update the content field with current editor value
        document.getElementById('code-editor').value = editor.getValue();
    });
});