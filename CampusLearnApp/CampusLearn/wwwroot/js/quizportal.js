// Quiz Portal interactions: tabs, search/filter, toasts, button actions
(function () {
    const $ = (s, r = document) => r.querySelector(s);
    const $$ = (s, r = document) => Array.from(r.querySelectorAll(s));

    // Defer initialization until DOM is ready to ensure elements exist
    function init() {
        // Toast
        let toastRoot = $('.quiz-toast-root');
        if (!toastRoot) {
            toastRoot = document.createElement('div');
            toastRoot.className = 'quiz-toast-root';
            toastRoot.setAttribute('aria-live', 'polite');
            document.body.appendChild(toastRoot);
        }

        function toast(msg, t = 2000) {
            const el = document.createElement('div');
            el.className = 'quiz-toast';
            el.textContent = msg;
            toastRoot.appendChild(el);
            setTimeout(() => {
                el.style.opacity = '0';
                el.style.transform = 'translateY(6px)';
                setTimeout(() => el.remove(), 260);
            }, t);
        }

        // Tabs
        const tabs = $$('.quiz-tabs .quiz-tab');
        const panels = $$('.quiz-tab-panel');

        function showPanel(id) {
            panels.forEach(p => {
                const on = p.id === id;
                p.hidden = !on;
                p.classList.toggle('active', on);
            });
            tabs.forEach(t => {
                const on = t.getAttribute('aria-controls') === id;
                t.classList.toggle('active', on);
                t.setAttribute('aria-selected', String(on));
            });
        }

        tabs.forEach(t => {
            t.addEventListener('click', (ev) => {
                ev.preventDefault();
                const id = t.getAttribute('aria-controls');
                if (id) {
                    showPanel(id);
                    try {
                        history.replaceState(null, '', `#${id}`);
                    } catch (e) { }
                }
            });

            // keyboard nav: ArrowLeft / ArrowRight / Home / End
            t.addEventListener('keydown', (ev) => {
                const idx = tabs.indexOf(t);
                if (ev.key === 'ArrowRight') {
                    ev.preventDefault();
                    const next = tabs[(idx + 1) % tabs.length];
                    next.focus();
                    next.click();
                }
                if (ev.key === 'ArrowLeft') {
                    ev.preventDefault();
                    const prev = tabs[(idx - 1 + tabs.length) % tabs.length];
                    prev.focus();
                    prev.click();
                }
                if (ev.key === 'Home') {
                    ev.preventDefault();
                    tabs[0].focus();
                    tabs[0].click();
                }
                if (ev.key === 'End') {
                    ev.preventDefault();
                    tabs[tabs.length - 1].focus();
                    tabs[tabs.length - 1].click();
                }
            });
        });

        const hash = location.hash.slice(1);
        if (hash && document.getElementById(hash)) showPanel(hash);

        // Create quiz button
        $('.quiz-create-btn')?.addEventListener('click', () => toast('Create Quiz: Coming soon'));

        // Take Quiz — search and chips filter
        const search = $('.quiz-search-input');
        const chips = $$('.quiz-chip');
        const cards = $$('.quiz-card-item');

        function applyFilters() {
            const q = (search?.value || '').toLowerCase().trim();
            const activeChips = chips.filter(c => c.classList.contains('active') && c.dataset.chip !== 'all')
                .map(c => c.dataset.chip);
            cards.forEach(card => {
                const title = (card.querySelector('.quiz-q-title')?.textContent || '').toLowerCase();
                const tags = (card.dataset.tags || '');
                const matchesText = !q || title.includes(q);
                const matchesChips = activeChips.length === 0 || activeChips.every(tag => tags.includes(tag));
                card.style.display = matchesText && matchesChips ? '' : 'none';
            });
        }

        search?.addEventListener('input', applyFilters);
        chips.forEach(c => c.addEventListener('click', () => {
            if (c.dataset.chip === 'all') {
                chips.forEach(x => x.classList.remove('active'));
                c.classList.add('active');
            } else {
                document.querySelector('.quiz-chip[data-chip="all"]')?.classList.remove('active');
                c.classList.toggle('active');
                // if none selected, revert to All
                const any = chips.some(x => x.dataset.chip !== 'all' && x.classList.contains('active'));
                if (!any) document.querySelector('.quiz-chip[data-chip="all"]')?.classList.add('active');
            }
            applyFilters();
        }));

        // Start/Review/Retake buttons
        $$('.quiz-start-btn, .quiz-result .quiz-btn, .quiz-cat .quiz-btn').forEach(btn => {
            btn.addEventListener('click', (ev) => {
                const text = (btn.textContent || '').trim();
                const quizId = btn.closest('.quiz-card-item')?.dataset.quizId;

                if (btn.classList.contains('quiz-start-btn') && quizId) {
                    // Submit form to start quiz
                    const form = document.createElement('form');
                    form.method = 'POST';
                    form.action = '/Quiz/StartQuiz';

                    const input = document.createElement('input');
                    input.type = 'hidden';
                    input.name = 'quizId';
                    input.value = quizId;

                    form.appendChild(input);
                    document.body.appendChild(form);

                    // Add anti-forgery token
                    const token = document.querySelector('input[name="__RequestVerificationToken"]');
                    if (token) {
                        const tokenClone = token.cloneNode(true);
                        form.appendChild(tokenClone);
                    }

                    form.submit();
                } else {
                    toast(`${text} clicked`);
                }
            });
        });

        // Quiz Analytics: ensure percentage text is explicit
        (function exposeAnalyticsPercentages() {
            const analytics = document.getElementById('panel-analytics');
            if (!analytics) return;
            const bars = Array.from(analytics.querySelectorAll('.quiz-bars .quiz-bar'));
            bars.forEach(bar => {
                const fill = bar.querySelector('i');
                if (!fill) return;

                let pct = null;
                const inline = fill.getAttribute('style') || '';
                const m = inline.match(/width\s*:\s*(\d+)%/i);
                if (m) pct = Number(m[1]);

                if (pct === null || Number.isNaN(pct)) return;

                // Remove any <em> left inside the bar to avoid duplicates
                const oldInnerEm = bar.querySelector('em');
                if (oldInnerEm && !oldInnerEm.classList.contains('pct')) oldInnerEm.remove();

                // Place an <em class="pct"> element immediately after the .bar
                const li = bar.closest('li');
                if (li) {
                    let pctEl = null;
                    const next = bar.nextElementSibling;
                    if (next && next.matches && next.matches('em.pct')) pctEl = next;
                    if (!pctEl) {
                        pctEl = document.createElement('em');
                        pctEl.className = 'pct';
                        if (bar.parentNode) bar.parentNode.insertBefore(pctEl, bar.nextSibling);
                    }
                    pctEl.textContent = `${pct}%`;
                }

                // Accessibility
                bar.setAttribute('role', 'progressbar');
                bar.setAttribute('aria-label', `${bar.previousElementSibling ? bar.previousElementSibling.textContent.trim() : 'Progress'}`);
                bar.setAttribute('aria-valuemin', '0');
                bar.setAttribute('aria-valuemax', '100');
                bar.setAttribute('aria-valuenow', String(pct));
            });
        })();
    }

    if (document.readyState === 'loading')
        document.addEventListener('DOMContentLoaded', init);
    else
        init();
})();

