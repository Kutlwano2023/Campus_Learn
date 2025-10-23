// Modern Student Portal Functionality
document.addEventListener('DOMContentLoaded', function () {
    // Initialize portal
    initStudentPortal();
});

function initStudentPortal() {
    initializeTabs();
    initializeCourseInteractions();
    initializeProgressAnimations();
    initializeKeyboardShortcuts();
}

// Tab Management
function initializeTabs() {
    const tabButtons = document.querySelectorAll('.tab-btn');
    const tabContents = document.querySelectorAll('.tab-content');

    tabButtons.forEach(button => {
        button.addEventListener('click', () => {
            const targetTab = button.getAttribute('data-tab');

            // Update active tab button
            tabButtons.forEach(btn => btn.classList.remove('active'));
            button.classList.add('active');

            // Show target tab content
            tabContents.forEach(content => {
                content.classList.remove('active');
                if (content.id === targetTab) {
                    content.classList.add('active');
                }
            });

            // Save active tab
            localStorage.setItem('activeStudentTab', targetTab);

            // Trigger tab change event
            dispatchTabChangeEvent(targetTab);
        });
    });

    // Restore saved tab
    const savedTab = localStorage.getItem('activeStudentTab') || 'learning';
    const savedButton = document.querySelector(`[data-tab="${savedTab}"]`);
    if (savedButton) {
        savedButton.click();
    }
}

function dispatchTabChangeEvent(tabName) {
    const event = new CustomEvent('studentTabChange', {
        detail: { tab: tabName }
    });
    document.dispatchEvent(event);
}

// Course Interactions
function initializeCourseInteractions() {
    const courseButtons = document.querySelectorAll('.course-actions .btn');

    courseButtons.forEach(button => {
        button.addEventListener('click', (e) => {
            e.preventDefault();
            const courseId = button.getAttribute('data-course-id');
            const action = button.textContent.toLowerCase();

            handleCourseAction(button, courseId, action);
        });
    });
}

async function handleCourseAction(button, courseId, action) {
    const originalText = button.innerHTML;

    // Show loading state
    button.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>Loading...';
    button.disabled = true;

    try {
        // Simulate API call
        await simulateAPICall(1000);

        // Show success state
        button.innerHTML = '<i class="fas fa-check me-1"></i>Success!';
        button.classList.add('btn-success');

        // Show notification
        showNotification(`${action.charAt(0).toUpperCase() + action.slice(1)} course ${courseId}`, 'success');

        // Redirect after success
        setTimeout(() => {
            window.location.href = `/portal/course/${courseId}`;
        }, 1500);

    } catch (error) {
        // Show error state
        button.innerHTML = '<i class="fas fa-exclamation-triangle me-1"></i>Error';
        button.classList.add('btn-error');

        showNotification('Failed to process request', 'error');

        // Reset button after error
        setTimeout(() => {
            button.innerHTML = originalText;
            button.disabled = false;
            button.classList.remove('btn-error');
        }, 2000);
    }
}

// Progress Animations
function initializeProgressAnimations() {
    const progressBars = document.querySelectorAll('.progress-fill');

    progressBars.forEach(bar => {
        const width = bar.style.width;
        bar.style.width = '0%';

        setTimeout(() => {
            bar.style.transition = 'width 1s ease-in-out';
            bar.style.width = width;
        }, 100);
    });
}

// Keyboard Shortcuts
function initializeKeyboardShortcuts() {
    document.addEventListener('keydown', (e) => {
        if (e.ctrlKey || e.metaKey) {
            switch (e.key) {
                case '1':
                    e.preventDefault();
                    switchToTab('learning');
                    break;
                case '2':
                    e.preventDefault();
                    switchToTab('assessments');
                    break;
                case '3':
                    e.preventDefault();
                    switchToTab('groups');
                    break;
                case '4':
                    e.preventDefault();
                    switchToTab('peer');
                    break;
            }
        }
    });
}

function switchToTab(tabName) {
    const tabButton = document.querySelector(`[data-tab="${tabName}"]`);
    if (tabButton) {
        tabButton.click();
        showNotification(`Switched to ${getTabDisplayName(tabName)}`, 'info');
    }
}

function getTabDisplayName(tabName) {
    const names = {
        learning: 'Learning Materials',
        assessments: 'Assessment Progress',
        groups: 'Study Groups',
        peer: 'Peer Learning'
    };
    return names[tabName] || tabName;
}

// Notification System
function showNotification(message, type = 'info') {
    // Remove existing notifications
    removeExistingNotifications();

    const notification = createNotificationElement(message, type);
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

function createNotificationElement(message, type) {
    const notification = document.createElement('div');
    notification.className = `portal-notification notification-${type}`;

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

    // Add styles
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${getNotificationColor(type)};
        color: white;
        padding: 12px 16px;
        border-radius: 8px;
        box-shadow: var(--shadow-lg);
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

    return notification;
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

function removeExistingNotifications() {
    const existing = document.querySelectorAll('.portal-notification');
    existing.forEach(notification => notification.remove());
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

// Utility Functions
function simulateAPICall(duration) {
    return new Promise((resolve) => {
        setTimeout(resolve, duration);
    });
}

// Export functions for global access
window.switchToTab = switchToTab;
window.showNotification = showNotification;

console.log('Student Portal initialized successfully');