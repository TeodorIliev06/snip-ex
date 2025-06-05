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

    setTimeout(() => editor.focus(), 100);

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
    document.getElementById('language-selector').addEventListener('change', function () {
        const selectedMode = this.value;
        editor.setOption('mode', selectedMode);

        // Update hidden language field
        document.getElementById('selected-language').value = languageMappings[selectedMode] || selectedMode;
    });

    document.getElementById('copy-code-btn').addEventListener('click', function () {
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

    document.getElementById('code-view-btn').addEventListener('click', function () {
        this.classList.add('active');
        document.getElementById('terminal-view-btn').classList.remove('active');
        document.getElementById('terminal-preview').style.display = 'none';
        document.querySelector('.editor-container').style.display = 'block';
    });

    document.getElementById('terminal-view-btn').addEventListener('click', function () {
        this.classList.add('active');
        document.getElementById('code-view-btn').classList.remove('active');
        document.getElementById('terminal-preview').style.display = 'block';
        document.querySelector('.editor-container').style.display = 'none';

        const code = editor.getValue();
        const terminalContent = document.querySelector('.terminal-content');
        terminalContent.innerHTML = '';

        const preElement = document.createElement('pre');
        const codeElement = document.createElement('code');
        codeElement.className = 'language-' + (languageMappings[editor.getOption('mode')] || editor.getOption('mode'));
        codeElement.textContent = editor.getValue() || 'No code to preview yet...';
        preElement.appendChild(codeElement);
        terminalContent.appendChild(preElement);

        // If using highlight.js or Prism for terminal highlighting
        if (typeof hljs !== 'undefined') {
            hljs.highlightElement(codeElement);
        }
    });

    // Handle tag management
    const tagsContainer = document.getElementById('tags-container');
    const tagInput = document.getElementById('tag-input');
    const addTagButton = document.getElementById('add-tag-btn');
    const tagIdsContainer = document.getElementById('tag-ids-container');

    let tags = [];

    function addTag() {
        const tagValue = tagInput.value.trim();

        // Basic client-side validation (server-side will also validate)
        if (!tagValue) {
            flashInputError();
            return;
        }

        if (tags.some(tag => tag.toLowerCase() === tagValue.toLowerCase())) {
            flashInputError();
            return;
        }

        tags.push(tagValue);

        // Create visual tag element
        const tagElement = document.createElement('div');
        tagElement.className = 'tag';
        tagElement.innerHTML = `${tagValue} <span class="tag-remove" data-tag="${tagValue}">×</span>`;

        // Add event listener for tag removal
        tagElement.querySelector('.tag-remove').addEventListener('click', function () {
            const tagToRemove = this.getAttribute('data-tag');
            tags = tags.filter(tag => tag !== tagToRemove);
            tagElement.remove();
            updateHiddenTagFields();
        });

        tagsContainer.appendChild(tagElement);

        updateHiddenTagFields();

        tagInput.value = '';
    }

    // Flash input border red for validation errors
    function flashInputError() {
        tagInput.style.borderColor = '#f85149';
        setTimeout(() => {
            tagInput.style.borderColor = '';
        }, 1000);
    }

    // Update hidden fields for form submission (proper model binding structure)
    function updateHiddenTagFields() {
        tagIdsContainer.innerHTML = '';

        tags.forEach((tag, index) => {
            const hiddenField = document.createElement('input');
            hiddenField.type = 'hidden';
            hiddenField.name = `Tags[${index}].Name`;
            hiddenField.value = tag;
            tagIdsContainer.appendChild(hiddenField);
        });

        // Reparse validation for dynamically added elements
        if (typeof $.validator !== 'undefined') {
            const form = document.querySelector('form');
            $(form).removeData('validator').removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);
        }
    }

    addTagButton.addEventListener('click', addTag);

    tagInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            addTag();
        }
    });

    // Save draft functionality (if save-draft-btn exists)
    //const saveDraftBtn = document.getElementById('save-draft-btn');
    //if (saveDraftBtn) {
    //    saveDraftBtn.addEventListener('click', function () {
    //        // Add a draft flag to the form
    //        const draftInput = document.createElement('input');
    //        draftInput.type = 'hidden';
    //        draftInput.name = 'IsDraft';
    //        draftInput.value = 'true';
    //        document.querySelector('form').appendChild(draftInput);

    //        // Submit the form
    //        document.querySelector('form').submit();
    //    });
    //}

    // Ensure content field is updated with editor content on form submit
    document.querySelector('form').addEventListener('submit', function () {
        document.getElementById('code-editor').value = editor.getValue();
    });

    editor.setOption('extraKeys', {
        'Ctrl-S': function (cm) {
            if (saveDraftBtn) {
                saveDraftBtn.click();
            }
            return false;
        },
        'Ctrl-Enter': function (cm) {
            document.querySelector('form button[type="submit"]').click();
            return false;
        },
        'Tab': function (cm) {
            const spaces = Array(cm.getOption('indentUnit') + 1).join(' ');
            cm.replaceSelection(spaces);
            return false;
        },
        'Shift-Tab': function (cm) {
            const cursor = cm.getCursor();
            const line = cm.getLine(cursor.line);

            // Check if the line starts with 4 spaces, then remove them
            if (line.startsWith('    ')) {
                cm.replaceRange('', { line: cursor.line, ch: 0 }, { line: cursor.line, ch: 4 });
            }
            return false;
        }
    });

    if (window.innerWidth < 768) {
        editor.setOption('lineNumbers', false);
        editor.setOption('lineWrapping', true);
    }
});