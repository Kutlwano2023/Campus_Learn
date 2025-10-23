// Schedule session functionality
document.addEventListener('DOMContentLoaded', function () {
    // Handle tutor booking buttons
    const bookButtons = document.querySelectorAll('.tutor .btn.dark');
    bookButtons.forEach(button => {
        button.addEventListener('click', function () {
            const tutorId = this.getAttribute('data-tutor-id');
            const tutorCard = this.closest('.tutor');
            const tutorName = tutorCard.querySelector('.t-name').textContent;
            const tutorSubjects = tutorCard.querySelector('.t-sub').textContent;

            // Pre-fill the form with tutor information
            prefillTutorForm(tutorName, tutorSubjects, tutorId);

            // Scroll to form
            document.querySelector('.form').scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        });
    });

    // Handle session form submission
    const sessionForm = document.querySelector('.form');
    if (sessionForm) {
        sessionForm.addEventListener('submit', async function (e) {
            e.preventDefault();

            const submitButton = this.querySelector('button[type="submit"]');
            const originalText = submitButton.innerHTML;

            // Show loading state
            submitButton.innerHTML = '<span class="i">⏳</span> Scheduling...';
            submitButton.disabled = true;

            try {
                const formData = new FormData(this);
                const sessionData = {
                    subject: formData.get('Subject'),
                    sessionType: formData.get('SessionType'),
                    date: formData.get('SessionDate'),
                    time: formData.get('SessionTime'),
                    details: formData.get('SessionDetails'),
                    tutorId: formData.get('TutorId') || null
                };

                // Validate required fields
                if (!sessionData.subject || !sessionData.sessionType ||
                    !sessionData.date || !sessionData.time) {
                    throw new Error('Please fill in all required fields');
                }

                // Validate date is not in the past
                const selectedDateTime = new Date(sessionData.date + 'T' + sessionData.time);
                if (selectedDateTime < new Date()) {
                    throw new Error('Please select a future date and time');
                }

                // Send to server
                const response = await fetch('/Portal/ScheduleSession', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(sessionData)
                });

                const data = await response.json();

                if (data.success) {
                    showMessage('Session scheduled successfully!', 'success');
                    this.reset();
                    // Refresh upcoming sessions
                    loadUpcomingSessions();
                } else {
                    throw new Error(data.message || 'Error scheduling session');
                }
            } catch (error) {
                console.error('Error:', error);
                showMessage(error.message, 'error');
            } finally {
                // Restore button state
                submitButton.innerHTML = originalText;
                submitButton.disabled = false;
            }
        });
    }

    // Handle join session buttons
    const joinButtons = document.querySelectorAll('.up-actions .btn.dark');
    joinButtons.forEach(button => {
        button.addEventListener('click', function () {
            const sessionCard = this.closest('.up');
            const sessionTitle = sessionCard.querySelector('.up-title').textContent;
            const tutorName = sessionCard.querySelector('.up-name').textContent;

            if (confirm(`Join session "${sessionTitle}" with ${tutorName}?`)) {
                // Implement actual join functionality
                window.open('/meeting/join', '_blank');
            }
        });
    });

    // Handle reschedule buttons
    const rescheduleButtons = document.querySelectorAll('.up-actions .btn:not(.dark)');
    rescheduleButtons.forEach(button => {
        button.addEventListener('click', function () {
            const sessionCard = this.closest('.up');
            const sessionTitle = sessionCard.querySelector('.up-title').textContent;
            alert(`Reschedule functionality for "${sessionTitle}" would open here.`);
        });
    });

    // Utility Functions
    function prefillTutorForm(tutorName, subjects, tutorId) {
        // You can pre-fill specific fields or show the tutor selection
        showMessage(`Selected tutor: ${tutorName} (${subjects})`, 'success');

        // Add hidden input for tutor ID if needed
        let tutorInput = sessionForm.querySelector('input[name="TutorId"]');
        if (!tutorInput) {
            tutorInput = document.createElement('input');
            tutorInput.type = 'hidden';
            tutorInput.name = 'TutorId';
            sessionForm.appendChild(tutorInput);
        }
        tutorInput.value = tutorId;
    }

    function showMessage(message, type) {
        // Remove existing messages
        const existingMessage = document.querySelector('.form-message');
        if (existingMessage) {
            existingMessage.remove();
        }

        // Create new message
        const messageDiv = document.createElement('div');
        messageDiv.className = `form-message ${type}`;
        messageDiv.textContent = message;

        // Insert after the form description
        const formDesc = sessionForm.querySelector('.muted.small');
        formDesc.parentNode.insertBefore(messageDiv, formDesc.nextSibling);

        // Auto-remove after 5 seconds
        setTimeout(() => {
            messageDiv.remove();
        }, 5000);
    }

    function loadUpcomingSessions() {
        // This would typically fetch updated sessions from the server
        console.log('Loading updated sessions...');
        // Implement AJAX call to refresh upcoming sessions
    }

    // Initialize date input with min date as today
    const dateInput = document.querySelector('input[type="date"]');
    if (dateInput) {
        const today = new Date().toISOString().split('T')[0];
        dateInput.min = today;
    }

    // Add keyboard navigation for tutors
    document.querySelectorAll('.tutor').forEach(tutor => {
        tutor.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                tutor.querySelector('.btn.dark').click();
            }
        });

        // Make tutor cards focusable
        tutor.setAttribute('tabindex', '0');
    });
});