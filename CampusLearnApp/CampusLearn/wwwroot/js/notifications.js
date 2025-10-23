
(function () {
    const btn = document.getElementById('notificationBtn');
    const dropdown = document.getElementById('notificationDropdown');
    const badge = document.getElementById('notificationBadge');

    if (!btn || !dropdown || !badge) return;

    // Toggle dropdown visibility
    btn.addEventListener('click', (e) => {
        e.stopPropagation();
        dropdown.classList.toggle('hidden');
    });

    // Close dropdown when clicking outside
    document.addEventListener('click', () => {
        dropdown.classList.add('hidden');
    });

    // Stop click inside dropdown from closing it
    dropdown.addEventListener('click', (e) => {
        e.stopPropagation();
    });

    // Function to add a new notification
    window.addNotification = function (message) {
        const list = document.getElementById('notificationList');
        if (!list) return;

        const li = document.createElement('li');
        li.className = 'p-2 text-sm hover:bg-gray-100 cursor-pointer';
        li.textContent = message;

        list.prepend(li); // newest on top
        badge.textContent = parseInt(badge.textContent || '0') + 1;
        badge.classList.remove('hidden');
    };

})();

