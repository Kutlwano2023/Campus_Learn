// Client-side fetch + UI logic for notifications partial.
// Assumes jQuery is available. If not, replace with fetch() + DOM APIs.

(function () {
    const btn = document.getElementById('notification-btn');
    const countEl = document.getElementById('notification-count');
    const dropdown = document.getElementById('notification-dropdown');
    const listEl = document.getElementById('notification-list');
    const lastUpdated = document.getElementById('notification-lastupdated');

    let dropdownVisible = false;

    async function fetchCountAndRender() {
        try {
            const res = await fetch('/Notifications/GetCount', { credentials: 'same-origin' });
            if (!res.ok) return;
            const json = await res.json();
            const count = json.unread || 0;
            if (count > 0) {
                countEl.style.display = 'inline-block';
                countEl.textContent = count > 99 ? '99+' : count;
            } else {
                countEl.style.display = 'none';
            }
            if (lastUpdated) lastUpdated.textContent = new Date().toLocaleTimeString();
        } catch (err) {
            console.error('Notification count fetch error', err);
        }
    }

    async function fetchListAndRender() {
        try {
            const res = await fetch('/Notifications/GetRecent?limit=20', { credentials: 'same-origin' });
            if (!res.ok) {
                listEl.innerHTML = '<div class="list-group-item text-danger">Error loading notifications</div>';
                return;
            }
            const items = await res.json();

            if (!items || items.length === 0) {
                listEl.innerHTML = '<div class="list-group-item text-muted">No notifications</div>';
                return;
            }

            listEl.innerHTML = '';
            items.forEach(it => {
                const readClass = it.isRead ? 'text-muted' : '';
                const created = new Date(it.createdAtUtc || it.createdAt || it.createdAtUtc);
                const a = document.createElement('a');
                a.href = it.link || '#';
                a.className = 'list-group-item list-group-item-action d-flex justify-content-between align-items-start';
                a.innerHTML = `<div class="${readClass} me-2" style="flex:1;">
                                    <div><strong>${escapeHtml(it.title)}</strong></div>
                                    <div class="small">${escapeHtml(it.message)}</div>
                                </div>
                                <div class="small text-muted text-end" style="width:50px;">
                                    ${timeAgo(created)}
                                </div>`;
                a.addEventListener('click', function (ev) {
                    // optimistic mark-as-read (don't block navigation)
                    try { markAsRead(it.id); } catch (e) { /* ignore */ }
                });
                listEl.appendChild(a);
            });
        } catch (err) {
            console.error('Notification list fetch error', err);
            listEl.innerHTML = '<div class="list-group-item text-danger">Error loading notifications</div>';
        }
    }

    function escapeHtml(unsafe) {
        if (!unsafe) return '';
        return String(unsafe).replace(/[&<"'>]/g, function (m) {
            return ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' })[m];
        });
    }

    function timeAgo(date) {
        if (!date || isNaN(date)) return '';
        const seconds = Math.floor((new Date() - date) / 1000);
        if (seconds < 60) return 'now';
        const minutes = Math.floor(seconds / 60);
        if (minutes < 60) return `${minutes}m`;
        const hours = Math.floor(minutes / 60);
        if (hours < 24) return `${hours}h`;
        const days = Math.floor(hours / 24);
        return `${days}d`;
    }

    async function markAsRead(id) {
        try {
            await fetch('/Notifications/MarkAsRead', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(id),
                credentials: 'same-origin'
            });
            await fetchCountAndRender();
            await fetchListAndRender();
        } catch (err) {
            console.error('Error marking as read', err);
        }
    }

    if (btn) {
        btn.addEventListener('click', async function (e) {
            e.preventDefault();
            dropdownVisible = !dropdownVisible;
            dropdown.style.display = dropdownVisible ? 'block' : 'none';
            if (dropdownVisible) {
                listEl.innerHTML = '<div class="list-group-item">Loading...</div>';
                await fetchListAndRender();
            }
        });

        document.addEventListener('click', function (e) {
            if (!btn.contains(e.target) && (!dropdown || !dropdown.contains(e.target))) {
                dropdownVisible = false;
                dropdown.style.display = 'none';
            }
        });

        // initial load + poll
        fetchCountAndRender();
        setInterval(fetchCountAndRender, 30000);
    }
})();