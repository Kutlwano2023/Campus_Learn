// Tests Dashboard Functionality
document.addEventListener('DOMContentLoaded', function () {
    // DOM Elements
    const tabs = document.querySelectorAll('.tab');
    const searchInput = document.getElementById('searchInput');
    const filterButton = document.getElementById('filterButton');
    const chips = document.querySelectorAll('.chip');

    // Sections
    const sections = {
        'Available Tests': document.getElementById('availableTestsSection'),
        'Tests in Progress': document.getElementById('progressTestsSection'),
        'Completed Tests': document.getElementById('completedTestsSection')
    };

    const sectionTitles = {
        'Tests in Progress': document.getElementById('progressSectionTitle'),
        'Completed Tests': document.getElementById('completedSectionTitle')
    };

    // Search and filter state
    let activeCategory = 'All Categories';
    let searchQuery = '';

    // Show only selected section
    function showSection(sectionName) {
        // Update active tab
        tabs.forEach(tab => {
            tab.classList.toggle('active', tab.textContent.trim() === sectionName);
        });

        // Show/hide sections
        Object.keys(sections).forEach(key => {
            const section = sections[key];
            const title = sectionTitles[key];

            if (section) {
                section.style.display = key === sectionName ? '' : 'none';
            }
            if (title) {
                title.style.display = key === sectionName ? '' : 'none';
            }
        });

        // Show/hide search and chips for Available Tests
        const searchBar = document.querySelector('.bar');
        const chipsContainer = document.querySelector('.chips');

        if (searchBar && chipsContainer) {
            if (sectionName === 'Available Tests') {
                searchBar.style.display = '';
                chipsContainer.style.display = '';
            } else {
                searchBar.style.display = 'none';
                chipsContainer.style.display = 'none';
            }
        }

        // Animate progress bars when showing progress section
        if (sectionName === 'Tests in Progress') {
            setTimeout(animateProgressBars, 100);
        }
    }

    // Tab click handlers
    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            const sectionName = tab.textContent.trim();
            showSection(sectionName);
        });
    });

    // Chip click handlers
    chips.forEach(chip => {
        chip.addEventListener('click', () => {
            chips.forEach(c => c.classList.remove('active'));
            chip.classList.add('active');
            activeCategory = chip.textContent.trim();
            filterTests();
        });
    });

    // Search input handler
    searchInput.addEventListener('input', (e) => {
        searchQuery = e.target.value.toLowerCase().trim();
        filterTests();
    });

    // Filter button handler
    filterButton.addEventListener('click', () => {
        // Implement advanced filtering modal/popover
        alert('Advanced filter functionality would open here');
    });

    // Filter tests based on search and category
    function filterTests() {
        const cards = document.querySelectorAll('#availableTestsSection .card');

        cards.forEach(card => {
            const title = card.querySelector('.title')?.textContent.toLowerCase() || '';
            const description = card.querySelector('.subtitle.sub')?.textContent.toLowerCase() || '';
            const badge = card.querySelector('.badge')?.textContent.toLowerCase() || '';

            const matchesSearch = !searchQuery ||
                title.includes(searchQuery) ||
                description.includes(searchQuery);

            const matchesCategory = activeCategory === 'All Categories' ||
                matchesTestCategory(card, activeCategory);

            card.style.display = matchesSearch && matchesCategory ? '' : 'none';
        });
    }

    // Check if test matches category
    function matchesTestCategory(card, category) {
        const title = card.querySelector('.title')?.textContent.toLowerCase() || '';
        const description = card.querySelector('.subtitle.sub')?.textContent.toLowerCase() || '';
        const badge = card.querySelector('.badge')?.textContent.toLowerCase() || '';

        const content = title + ' ' + description + ' ' + badge;
        const categoryLower = category.toLowerCase();

        const categoryMap = {
            'react': ['react'],
            'javascript': ['javascript', 'js', 'node'],
            'css': ['css', 'layout', 'flexbox', 'grid'],
            'backend': ['node', 'backend', 'api', 'server'],
            'beginner': ['beginner'],
            'intermediate': ['intermediate'],
            'advanced': ['advanced']
        };

        const keywords = categoryMap[categoryLower] || [categoryLower];
        return keywords.some(keyword => content.includes(keyword));
    }

    // Animate progress bars
    function animateProgressBars() {
        const progressBars = document.querySelectorAll('.bar-fill');

        progressBars.forEach(bar => {
            const targetWidth = bar.style.width;
            bar.style.width = '0%';

            setTimeout(() => {
                bar.style.width = targetWidth;
            }, 100);
        });
    }

    // Handle test actions (Start, Continue, Review)
    function handleTestAction(button, testId, action) {
        button.disabled = true;
        const originalText = button.textContent;

        // Show loading state
        button.textContent = 'Loading...';

        // Simulate API call
        setTimeout(() => {
            // In real implementation, this would be an API call
            console.log(`${action} test ${testId}`);

            // Show success state
            button.textContent = action === 'review' ? 'Opening...' :
                action === 'continue' ? 'Resuming...' : 'Starting...';

            // Redirect or open test
            setTimeout(() => {
                // Simulate navigation to test
                if (action === 'review') {
                    window.location.href = `/Tests/Review/${testId}`;
                } else {
                    window.location.href = `/Tests/Take/${testId}`;
                }
            }, 800);

        }, 1000);
    }

    // Wire up test action buttons
    function initializeTestButtons() {
        const startButtons = document.querySelectorAll('.btn-primary[data-test-id]');
        const reviewButtons = document.querySelectorAll('.btn-small[data-test-id]');

        startButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                const testId = button.getAttribute('data-test-id');
                const action = button.textContent.toLowerCase().includes('continue') ? 'continue' : 'start';
                handleTestAction(button, testId, action);
            });
        });

        reviewButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                const testId = button.getAttribute('data-test-id');
                handleTestAction(button, testId, 'review');
            });
        });
    }

    // Keyboard shortcuts
    document.addEventListener('keydown', (e) => {
        // Only trigger if not typing in input
        if (e.target.tagName === 'INPUT') return;

        switch (e.key) {
            case '1':
                e.preventDefault();
                showSection('Available Tests');
                break;
            case '2':
                e.preventDefault();
                showSection('Tests in Progress');
                break;
            case '3':
                e.preventDefault();
                showSection('Completed Tests');
                break;
            case '/':
                e.preventDefault();
                showSection('Available Tests');
                searchInput.focus();
                break;
        }
    });

    // Initialize
    showSection('Available Tests');
    animateProgressBars();
    initializeTestButtons();

    console.log('Tests dashboard initialized');
});