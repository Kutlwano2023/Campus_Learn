// wwwroot/js/forum.js
// Tiny accessible tab switcher for the forum

(function () {
    const tabs = Array.from(document.querySelectorAll('.tabs .tab'));
    const panels = Array.from(document.querySelectorAll('.tab-panel'));

    if (!tabs.length || !panels.length) return;

    function show(id) {
        panels.forEach((p) => {
            const active = p.id === id;
            p.hidden = !active;
            p.classList.toggle('active', active);
        });

        tabs.forEach((t) => {
            const selected = t.getAttribute('aria-controls') === id;
            t.classList.toggle('active', selected);
            t.setAttribute('aria-selected', String(selected));
            t.tabIndex = selected ? 0 : -1;
        });
    }

    // Click activation
    tabs.forEach((btn) => {
        btn.addEventListener('click', () => show(btn.getAttribute('aria-controls')));
    });

    // Keyboard navigation (Left/Right + Enter/Space)
    tabs.forEach((btn, i) => {
        btn.addEventListener('keydown', (e) => {
            const last = tabs.length - 1;
            if (e.key === 'ArrowRight') {
                e.preventDefault();
                tabs[i === last ? 0 : i + 1].focus();
            } else if (e.key === 'ArrowLeft') {
                e.preventDefault();
                tabs[i === 0 ? last : i - 1].focus();
            } else if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                show(btn.getAttribute('aria-controls'));
            }
        });
    });

    // Optional deep-link support: forum.html#panel-popular
    const hash = window.location.hash.slice(1);
    if (hash && document.getElementById(hash)) {
        show(hash);
    } else {
        // Ensure default shown (in case HTML state was altered)
        const defaultId = tabs[0]?.getAttribute('aria-controls');
        if (defaultId) show(defaultId);
    }

    // --- Extra interactive behaviors for forum buttons ---
    // Toast helper
    let toastEl = document.getElementById('site-toast');
    if (!toastEl) {
        toastEl = document.createElement('div');
        toastEl.id = 'site-toast';
        toastEl.setAttribute('aria-live', 'polite');
        toastEl.style.cssText = 'position:fixed;right:16px;bottom:16px;background:#111827;color:#fff;padding:8px 12px;border-radius:8px;opacity:0;transition:opacity .2s;z-index:9999;font-size:13px';
        document.body.appendChild(toastEl);
    }
    function showToast(msg, ms = 2200) {
        toastEl.textContent = msg;
        toastEl.style.opacity = '1';
        clearTimeout(toastEl._t);
        toastEl._t = setTimeout(() => { toastEl.style.opacity = '0'; }, ms);
    }

    // Event delegation for forum actions
    const forumMain = document.querySelector('main.container') || document;
    forumMain.addEventListener('click', (e) => {
        const btn = e.target.closest('button, a');
        if (!btn) return;

        // New Topic button -> open modal form
        if (btn.matches('.btn.new-topic')) {
            openNewTopicModal();
            return;
        }

        // Reply button
        if (btn.matches('.btn.reply')) {
            const topic = btn.closest('.topic');
            const t = topic?.querySelector('.topic-title')?.textContent?.trim() || 'Discussion';
            const reply = prompt(`Reply to \"${t}\" (demo):`);
            if (reply) {
                // Update replies counter shown in topic.stats if present
                try {
                    const statNodes = Array.from(topic.querySelectorAll('.stats .stat'));
                    // Try to find the replies stat by looking for the word 'reply' or the replies emoji
                    let repliesNode = statNodes.find(s => /reply/i.test(s.textContent));
                    // Fallback: assume first .stat is replies
                    if (!repliesNode) repliesNode = statNodes[0];
                    if (repliesNode) {
                        const strong = repliesNode.querySelector('strong');
                        if (strong) {
                            const n = parseInt(strong.textContent.trim(), 10) || 0;
                            strong.textContent = String(n + 1);
                        } else {
                            // If <strong> isn't present, append one
                            const num = 1;
                            repliesNode.insertAdjacentHTML('afterbegin', `\u00a0<strong>${num}</strong> `);
                        }
                    }
                } catch (err) {
                    // ignore errors in demo update
                    console.error('Failed updating replies count', err);
                }

                showToast('Reply posted (demo)');
            }
            return;
        }

        // Topic title links - open thread (demo behavior)
        if (btn.matches('a.topic-title')) {
            e.preventDefault();
            const t = btn.textContent.trim();
            showToast(`Opening thread: ${t}`);
            // Could navigate to a thread page in a real app
            return;
        }

        // Ghost/share/icon buttons
        if (btn.matches('.btn.ghost.icon') || btn.matches('.btn.ghost.icon *')) {
            // For demo, copy a pseudo-link to clipboard (if supported)
            const link = location.href.split('#')[0] + '#';
            navigator.clipboard?.writeText(link).then(() => {
                showToast('Link copied to clipboard');
            }).catch(() => {
                showToast('Copy not supported in this browser');
            });
            return;
        }
    });

    // Search box filtering
    const search = document.querySelector('.forum-header .search input[type="search"]');
    if (search) {
        let t;
        search.addEventListener('input', () => {
            clearTimeout(t);
            t = setTimeout(() => {
                const q = search.value.trim().toLowerCase();
                const topics = Array.from(document.querySelectorAll('.topic'));
                topics.forEach(topic => {
                    const text = (topic.textContent || '').toLowerCase();
                    topic.style.display = q ? (text.includes(q) ? '' : 'none') : '';
                });
            }, 180);
        });
    }

    // Category clicks in sidebar filter topics
    const cats = Array.from(document.querySelectorAll('.categories .pill'));
    if (cats.length) {
        cats.forEach(c => c.addEventListener('click', () => {
            // Toggle active look (not styled) and filter
            c.classList.toggle('active');
            const active = cats.filter(x => x.classList.contains('active')).map(x => x.textContent.trim().toLowerCase());
            const topics = Array.from(document.querySelectorAll('.topic'));
            if (!active.length) {
                topics.forEach(t => t.style.display = '');
                return;
            }
            topics.forEach(t => {
                const text = (t.textContent || '').toLowerCase();
                t.style.display = active.every(a => text.includes(a)) ? '' : 'none';
            });
        }));
    }

    // --- New Topic modal implementation ---
    function openNewTopicModal() {
        // If modal exists, focus first input
        let modal = document.getElementById('new-topic-modal');
        if (!modal) {
            modal = document.createElement('div');
            modal.id = 'new-topic-modal';
            modal.className = 'modal';
            modal.innerHTML = `
                <div class="modal-backdrop" role="dialog" aria-modal="true" aria-labelledby="nt-title">
                    <div class="modal-panel" style="max-width:680px;margin:40px auto;background:#fff;padding:16px;border-radius:8px;box-shadow:0 6px 20px rgba(2,6,23,.2);">
                        <h2 id="nt-title">Create New Topic</h2>
                        <form id="new-topic-form">
                            <div style="margin-bottom:8px;">
                                <label>Module / Category<br><select name="module" required>
                                    <option value="React">React</option>
                                    <option value="JavaScript">JavaScript</option>
                                    <option value="CSS">CSS</option>
                                    <option value="Database">Database</option>
                                    <option value="Career">Career</option>
                                </select></label>
                            </div>
                            <div style="margin-bottom:8px;">
                                <label>Title<br><input name="title" type="text" required style="width:100%"/></label>
                            </div>
                            <div style="margin-bottom:8px;">
                                <label>Content<br><textarea name="content" rows="6" style="width:100%"></textarea></label>
                            </div>
                            <div style="display:flex;gap:8px;justify-content:flex-end">
                                <button type="button" class="btn ghost" id="nt-cancel">Cancel</button>
                                <button type="submit" class="btn">Create Topic</button>
                            </div>
                        </form>
                    </div>
                </div>
            `;
            // Add minimal styles to ensure modal covers viewport
            modal.querySelector('.modal-backdrop').style.cssText = 'position:fixed;inset:0;background:rgba(2,6,23,.5);display:flex;align-items:flex-start;justify-content:center;padding:24px;overflow:auto;';
            document.body.appendChild(modal);

            // Form handling
            const form = modal.querySelector('#new-topic-form');
            form.addEventListener('submit', (ev) => {
                ev.preventDefault();
                const data = new FormData(form);
                const module = data.get('module')?.toString() || 'General';
                const title = (data.get('title') || '').toString().trim();
                const content = (data.get('content') || '').toString().trim();
                if (!title) { alert('Please enter a title'); return; }
                createTopic({ module, title, content });
                closeModal();
            });

            modal.querySelector('#nt-cancel').addEventListener('click', () => closeModal());
        }
        // show and focus
        modal.style.display = '';
        const firstInput = modal.querySelector('select[name="module"]');
        firstInput?.focus();

        function closeModal() {
            modal.style.display = 'none';
            // return focus to New Topic button
            const nt = document.querySelector('.btn.new-topic');
            nt?.focus();
        }
    }

    function submitNewTopic(formData) {
        fetch('/Forum/CreateTopic', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: new URLSearchParams(formData)
        })
            .then(response => {
                if (response.ok) {
                    return response.text();
                }
                throw new Error('Network response was not ok.');
            })
            .then(data => {
                showToast('Topic created successfully!');
                // Optionally reload the page or update the UI
                setTimeout(() => {
                    window.location.reload();
                }, 1500);
            })
            .catch(error => {
                console.error('Error:', error);
                showToast('Error creating topic. Please try again.');
            });
    }
    const form = modal.querySelector('#new-topic-form');
    form.addEventListener('submit', (ev) => {
        ev.preventDefault();
        const data = new FormData(form);
        const module = data.get('module')?.toString() || 'General';
        const title = (data.get('title') || '').toString().trim();
        const content = (data.get('content') || '').toString().trim();

        if (!title) {
            alert('Please enter a title');
            return;
        }

        // Use AJAX to submit to server
        submitNewTopic({
            title: title,
            content: content,
            category: module
        });

        closeModal();
    });


    // Fallback: ensure header New Topic button directly opens modal
    const headerNew = document.querySelector('.btn.new-topic');
    if (headerNew) headerNew.addEventListener('click', (e) => { e.preventDefault(); openNewTopicModal(); });

    function createTopic({ module, title, content }) {
        const recent = document.getElementById('panel-recent');
        if (!recent) {
            showToast('Could not find Recent Discussions panel');
            return;
        }
        // Create topic article DOM similar to existing markup
        const article = document.createElement('article');
        article.className = 'topic card';
        const avatar = module.slice(0, 2).toUpperCase();
        const html = `
            <div class="topic-body">
                <div class="avatar">${escapeHtml(avatar)}</div>
                <div class="topic-main">
                    <div class="topic-title-row">
                        <a href="#" class="topic-title">${escapeHtml(title)}</a>
                        <span class="pill cat">${escapeHtml(module)}</span>
                    </div>
                    <div class="meta">
                        <span>by <a href="#" class="author">You</a></span>
                        <span class="dot">•</span>
                        <span>just now</span>
                    </div>
                    <div class="stats">
                        <span class="stat">💬 <strong>0</strong> replies</span>
                        <span class="stat">👍 <strong>0</strong> likes</span>
                    </div>
                    <div class="tags">${escapeHtml(content.slice(0, 140))}</div>
                </div>
            </div>
            <div class="topic-actions">
                <button class="btn ghost icon">↩</button>
                <button class="btn reply">Reply</button>
            </div>
        `;
        article.innerHTML = html;
        // Prepend to the panel
        const panelInner = recent.querySelector('.tab-panel') || recent;
        // Find the active container inside recent panel (it already contains topics)
        const container = recent.querySelector('.topic.card, .tab-panel') ? recent : recent;
        // Insert at top of panel
        const firstTopic = document.querySelector('#panel-recent .topic');
        if (firstTopic) firstTopic.parentNode.insertBefore(article, firstTopic);
        else recent.appendChild(article);
        showToast(`Topic created: ${title}`);
    }

    // small HTML-escape helper
    function escapeHtml(s) {
        return String(s).replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": "&#39;" })[c]);
    }
})();