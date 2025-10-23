(function () {
    const tabs = Array.from(document.querySelectorAll('.tabs .tab'));
    const panels = Array.from(document.querySelectorAll('.tab-panel'));

    function activate(id) {
        panels.forEach(p => {
            const on = p.id === id;
            p.hidden = !on;
            p.classList.toggle('active', on);
        });
        tabs.forEach(t => {
            const sel = t.getAttribute('aria-controls') === id;
            t.classList.toggle('active', sel);
            t.setAttribute('aria-selected', String(sel));
            t.tabIndex = sel ? 0 : -1;
        });
    }

    tabs.forEach(btn => {
        btn.addEventListener('click', () => activate(btn.getAttribute('aria-controls')));
        btn.addEventListener('keydown', (e) => {
            const i = tabs.indexOf(btn);
            const last = tabs.length - 1;
            if (e.key === 'ArrowRight') {
                e.preventDefault();
                tabs[i === last ? 0 : i + 1].focus();
            } else if (e.key === 'ArrowLeft') {
                e.preventDefault();
                tabs[i === 0 ? last : i - 1].focus();
            } else if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                activate(btn.getAttribute('aria-controls'));
            }
        });
    });

    // Deep link support
    const hash = location.hash.slice(1);
    const initial = (hash && document.getElementById(hash)) ? hash : tabs[0]?.getAttribute('aria-controls');
    if (initial) activate(initial);

    // Filter button
    const filterBtn = document.getElementById('filterButton');
    if (filterBtn) {
        filterBtn.addEventListener('click', () => {
            showNotification('Filter options coming soon!', 'info');
        });
    }

    // Chip toggles
    const chips = Array.from(document.querySelectorAll('.chip'));
    if (chips.length) {
        chips.forEach(c => {
            c.addEventListener('click', () => {
                // Remove active from all chips in this group
                chips.forEach(chip => chip.classList.remove('active'));
                // Add active to clicked chip
                c.classList.add('active');
                applyFilters();
            });
        });
    }

    // Search input filtering
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        let debounce;
        searchInput.addEventListener('input', (e) => {
            clearTimeout(debounce);
            debounce = setTimeout(() => applyFilters(), 180);
        });
    }

    function applyFilters() {
        const activeChip = document.querySelector('.chip.active');
        const category = activeChip ? activeChip.textContent.trim().toLowerCase() : 'all types';
        const query = (searchInput?.value || '').trim().toLowerCase();

        const cards = Array.from(document.querySelectorAll('.card.assignment'));
        cards.forEach(card => {
            const text = (card.textContent || '').toLowerCase();
            let visible = true;

            // Category filter
            if (category !== 'all types') {
                visible = text.includes(category);
            }

            // Search filter
            if (query) {
                visible = visible && text.includes(query);
            }

            card.style.display = visible ? '' : 'none';
        });
    }

    // Assignment actions
    document.addEventListener('click', (e) => {
        const btn = e.target.closest('button');
        if (!btn) return;

        // Start Assignment
        if (btn.classList.contains('btn-primary') && btn.textContent.includes('Start')) {
            const card = btn.closest('.card');
            const title = card.querySelector('.title')?.textContent?.trim() || 'Assignment';
            showNotification(`Starting "${title}" - moved to In Progress`, 'success');
            return;
        }

        // Continue Work
        if (btn.classList.contains('btn-primary') && btn.textContent.includes('Continue')) {
            const card = btn.closest('.card');
            const title = card.querySelector('.title')?.textContent?.trim() || 'Assignment';
            showNotification(`Continuing work on "${title}"`, 'info');
            return;
        }

        // View Details
        if (btn.classList.contains('btn-small')) {
            const card = btn.closest('.card');
            const title = card.querySelector('.title')?.textContent?.trim() || 'Item';
            showNotification(`Opening details for "${title}"`, 'info');
            return;
        }
    });

    // Notification function matching dashboard style
    function showNotification(message, type = 'info') {
        const existing = document.querySelectorAll('.dashboard-notification');
        existing.forEach(notification => notification.remove());

        const notification = document.createElement('div');
        notification.className = `dashboard-notification notification-${type}`;

        const icons = {
            success: 'fa-check-circle',
            error: 'fa-exclamation-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle'
        };

        notification.innerHTML = `
            <div class="notification-content">
                <i class="fas ${icons[type]} me-2"></i>
                <span>${message}</span>
            </div>
            <button class="notification-close" onclick="this.parentElement.remove()">
                <i class="fas fa-times"></i>
            </button>
        `;

        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: ${getNotificationColor(type)};
            color: white;
            padding: 12px 16px;
            border-radius: 8px;
            box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
            z-index: 1000;
            font-size: 14px;
            max-width: 300px;
            transform: translateX(100%);
            opacity: 0;
            transition: all 0.3s ease;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 1rem;
        `;

        document.body.appendChild(notification);

        // Animate in
        setTimeout(() => {
            notification.style.transform = 'translateX(0)';
            notification.style.opacity = '1';
        }, 10);

        // Auto remove
        setTimeout(() => {
            hideNotification(notification);
        }, 4000);
    }

    function getNotificationColor(type) {
        const colors = {
            success: '#10b981',
            error: '#ef4444',
            warning: '#f59e0b',
            info: '#3b82f6'
        };
        return colors[type] || colors.info;
    }

    function hideNotification(notification) {
        notification.style.transform = 'translateX(100%)';
        notification.style.opacity = '0';
        setTimeout(() => {
            if (notification.parentElement) {
                notification.remove();
            }
        }, 300);
    }
})();