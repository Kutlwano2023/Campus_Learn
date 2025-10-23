// wwwroot/js/resources.js
// Simple interactions for Resources page

(function () {
    // Helpers
    const $ = (sel, root = document) => root.querySelector(sel);
    const $$ = (sel, root = document) => Array.from(root.querySelectorAll(sel));

    // Toasts
    const toastRoot = $('.resources-toast-root');
    function toast(message, timeout = 2200) {
        if (!toastRoot) return;
        const el = document.createElement('div');
        el.className = 'resources-toast';
        el.textContent = message;
        toastRoot.appendChild(el);
        setTimeout(() => {
            el.style.opacity = '0';
            el.style.transform = 'translateY(6px)';
            setTimeout(() => el.remove(), 260);
        }, timeout);
    }

    // Tabs
    const tabs = $$('.resources-tabs .resources-tab');
    const panels = $$('.resources-tab-panel');

    function showPanel(id) {
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
        btn.addEventListener('click', () => {
            const id = btn.getAttribute('aria-controls');
            showPanel(id);
            // Optional: update hash for deep links
            history.replaceState(null, '', `#${id}`);
        });
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
                btn.click();
            }
        });
    });

    // Deep-link support (e.g., resources.html#panel-papers)
    const hash = location.hash.slice(1);
    if (hash && document.getElementById(hash)) {
        showPanel(hash);
    } else {
        // ensure default is visible
        const defaultId = tabs[0]?.getAttribute('aria-controls');
        if (defaultId) showPanel(defaultId);
    }

    // Download buttons - handle form submission
    $$('.resources-btn.dark').forEach(btn => {
        if (btn.textContent.trim().toLowerCase().includes('download')) {
            btn.addEventListener('click', (e) => {
                // If it's already a form button, let the form handle it
                if (btn.type === 'submit') return;

                e.preventDefault();
                const resourceCard = btn.closest('.resources-res, .resources-guide, .resources-paper, .resources-pack');
                const resourceId = resourceCard?.dataset.resourceId;
                const label = resourceCard?.querySelector('.resources-title-link, .resources-g-title, .resources-p-title, .resources-pack-title')?.textContent?.trim() || 'Resource';

                if (resourceId) {
                    // Submit form for server-side handling
                    const form = document.createElement('form');
                    form.method = 'POST';
                    form.action = '/Resources/DownloadResource';

                    const input = document.createElement('input');
                    input.type = 'hidden';
                    input.name = 'resourceId';
                    input.value = resourceId;

                    form.appendChild(input);

                    // Add anti-forgery token
                    const token = document.querySelector('input[name="__RequestVerificationToken"]');
                    if (token) {
                        const tokenClone = token.cloneNode(true);
                        form.appendChild(tokenClone);
                    }

                    document.body.appendChild(form);
                    form.submit();
                } else {
                    // Fallback for demo
                    btn.disabled = true;
                    btn.style.opacity = '0.8';
                    toast(`Downloading "${label}"...`);
                    setTimeout(() => {
                        btn.disabled = false;
                        btn.style.opacity = '';
                        toast('Download complete');
                    }, 1500);
                }
            });
        }
    });

    // Share buttons
    $$('.resources-btn').forEach(btn => {
        const isShare = btn.textContent.trim().toLowerCase().startsWith('share');
        if (!isShare) return;
        btn.addEventListener('click', async () => {
            const wrap = btn.closest('.resources-res, .resources-guide, .resources-paper, .resources-pack');
            const titleEl = wrap?.querySelector('.resources-title-link, .resources-g-title, .resources-p-title, .resources-pack-title');
            const title = titleEl?.textContent?.trim() || 'Resource';
            const url = `${location.origin}${location.pathname}?share=${encodeURIComponent(title)}`;
            try {
                await navigator.clipboard.writeText(url);
                toast('Share link copied to clipboard');
            } catch {
                toast('Share link: ' + url, 3000);
            }
        });
    });

    // Preview modal
    const modal = $('.resources-modal');
    const modalBody = $('.resources-modal-body');
    const modalClose = $('.resources-modal-close');
    const modalCancel = $('.resources-modal-cancel');
    const modalBackdrop = $('.resources-backdrop');
    const modalDownload = $('.resources-modal-download');

    function openPreview(title, meta = '') {
        if (!modal) return;
        $('.resources-modal-title').textContent = title || 'Preview';
        modalBody.innerHTML = `
            <p class="resources-small resources-muted" style="margin-top:0">${meta}</p>
            <div style="border:1px dashed var(--line);border-radius:10px;padding:16px;background:#fafbff">
                <p style="margin:0;color:#334155">
                    This is a preview placeholder for "${title}". Replace this with
                    an embedded PDF/image/markdown viewer as needed.
                </p>
            </div>
        `;
        modal.hidden = false;
        document.body.style.overflow = 'hidden';
    }

    function closePreview() {
        if (!modal) return;
        modal.hidden = true;
        document.body.style.overflow = '';
    }

    [modalClose, modalCancel, modalBackdrop].forEach(el => {
        if (el) el.addEventListener('click', closePreview);
    });

    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape' && modal && !modal.hidden) closePreview();
    });

    // Hook preview buttons
    $$('.resources-btn').forEach(btn => {
        if (btn.textContent.trim().toLowerCase() !== 'preview') return;
        btn.addEventListener('click', () => {
            const wrap = btn.closest('.resources-res, .resources-guide, .resources-paper');
            const title = wrap?.querySelector('.resources-title-link, .resources-g-title, .resources-p-title')?.textContent?.trim() || 'Resource';
            const meta = wrap?.querySelector('.resources-sub')?.textContent?.trim() || '';
            openPreview(title, meta);
            if (modalDownload) {
                modalDownload.onclick = () => {
                    closePreview();
                    toast(`Downloading "${title}"...`);
                };
            }
        });
    });

    // Upload Resource button handler
    $('.resources-upload-btn')?.addEventListener('click', () => {
        toast('Upload Resource: Coming soon');
    });

    // Search functionality
    const searchInput = $('.resources-search-input');
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', () => {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                const query = searchInput.value.toLowerCase().trim();
                const resources = $$('.resources-res, .resources-guide, .resources-paper, .resources-pack');

                resources.forEach(resource => {
                    const text = resource.textContent.toLowerCase();
                    resource.style.display = query === '' || text.includes(query) ? '' : 'none';
                });
            }, 300);
        });
    }

    // Type filter
    const typeFilter = $('.resources-type-filter');
    if (typeFilter) {
        typeFilter.addEventListener('change', (e) => {
            const selectedType = e.target.value.toLowerCase();
            const resources = $$('.resources-res');

            resources.forEach(resource => {
                if (selectedType === 'all types') {
                    resource.style.display = '';
                    return;
                }

                const fileType = resource.querySelector('.resources-sub')?.textContent.toLowerCase() || '';
                resource.style.display = fileType.includes(selectedType) ? '' : 'none';
            });
        });
    }
})();