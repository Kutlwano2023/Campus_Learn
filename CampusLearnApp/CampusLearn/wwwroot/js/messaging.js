// Messaging functionality - Global for all pages
document.addEventListener('DOMContentLoaded', function () {
    const messageWidgetBtn = document.getElementById('messageWidgetBtn');
    const messagingModal = document.getElementById('messagingModal');
    const closeMessagingBtn = document.getElementById('closeMessaging');
    const messageText = document.getElementById('messageText');
    const sendMessageBtn = document.getElementById('sendMessage');
    const searchConversations = document.getElementById('searchConversations');
    const conversations = document.querySelectorAll('.conversation');

    // Only initialize if user is authenticated and elements exist
    if (!messageWidgetBtn || !messagingModal) return;

    function openMessaging() {
        messagingModal.style.display = 'flex';
        document.body.style.overflow = 'hidden';
    }

    function closeMessaging() {
        messagingModal.style.display = 'none';
        document.body.style.overflow = 'auto';
    }

    // Open messaging modal
    messageWidgetBtn.addEventListener('click', openMessaging);

    // Close messaging modal
    closeMessagingBtn.addEventListener('click', closeMessaging);

    // Close modal when clicking outside
    messagingModal.addEventListener('click', function (e) {
        if (e.target === messagingModal) {
            closeMessaging();
        }
    });

    // Close modal with Escape key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && messagingModal.style.display === 'flex') {
            closeMessaging();
        }
    });

    // Send message functionality
    function sendMessage() {
        const text = messageText.value.trim();
        if (text) {
            const messagesContainer = document.querySelector('.messages-container');
            const messageDiv = document.createElement('div');
            messageDiv.className = 'message sent';

            const now = new Date();
            const timeString = now.toLocaleTimeString('en-US', {
                hour: 'numeric',
                minute: '2-digit',
                hour12: true
            });

            messageDiv.innerHTML = `
                <div class="message-content">
                    <p>${text}</p>
                    <span>${timeString}</span>
                </div>
            `;

            messagesContainer.appendChild(messageDiv);
            messageText.value = '';

            // Auto-scroll to bottom
            messagesContainer.scrollTop = messagesContainer.scrollHeight;

            // Simulate reply after 1-3 seconds
            setTimeout(simulateReply, Math.random() * 2000 + 1000);
        }
    }

    function simulateReply() {
        const messagesContainer = document.querySelector('.messages-container');
        const activeConversation = document.querySelector('.conversation.active');
        const userName = activeConversation ? activeConversation.querySelector('strong').textContent : 'User';
        const avatar = activeConversation ? activeConversation.querySelector('.avatar').textContent : 'U';

        const replies = [
            "Thanks for letting me know!",
            "That sounds great!",
            "I'll check it out right away.",
            "Can we discuss this in our next session?",
            "I have some questions about that.",
            "Perfect timing! I was just working on that."
        ];

        const randomReply = replies[Math.floor(Math.random() * replies.length)];
        const now = new Date();
        const timeString = now.toLocaleTimeString('en-US', {
            hour: 'numeric',
            minute: '2-digit',
            hour12: true
        });

        const messageDiv = document.createElement('div');
        messageDiv.className = 'message received';
        messageDiv.innerHTML = `
            <div class="message-avatar">${avatar}</div>
            <div class="message-content">
                <p>${randomReply}</p>
                <span>${timeString}</span>
            </div>
        `;

        messagesContainer.appendChild(messageDiv);

        // Auto-scroll to bottom
        messagesContainer.scrollTop = messagesContainer.scrollHeight;

        // Update conversation preview
        if (activeConversation) {
            const conversationInfo = activeConversation.querySelector('.conversation-info p');
            conversationInfo.textContent = randomReply;

            const conversationTime = activeConversation.querySelector('.conversation-info span');
            conversationTime.textContent = 'Just now';
        }
    }

    // Send message on button click
    if (sendMessageBtn) {
        sendMessageBtn.addEventListener('click', sendMessage);
    }

    // Send message on Enter key
    if (messageText) {
        messageText.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                sendMessage();
            }
        });
    }

    // Switch conversations
    conversations.forEach(conversation => {
        conversation.addEventListener('click', function () {
            // Remove active class from all conversations
            conversations.forEach(c => c.classList.remove('active'));
            // Add active class to clicked conversation
            this.classList.add('active');

            // Update chat header with selected user
            const userName = this.querySelector('strong').textContent;
            const userAvatar = this.querySelector('.avatar').textContent;
            const chatHeader = document.querySelector('.chat-user');
            chatHeader.querySelector('strong').textContent = userName;
            chatHeader.querySelector('.avatar').textContent = userAvatar;

            // Clear and load new conversation messages
            const messagesContainer = document.querySelector('.messages-container');
            messagesContainer.innerHTML = `
                <div class="message received">
                    <div class="message-avatar">${userAvatar}</div>
                    <div class="message-content">
                        <p>${this.querySelector('p').textContent}</p>
                        <span>${this.querySelector('span').textContent}</span>
                    </div>
                </div>
            `;

            // Auto-scroll to bottom
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
        });
    });

    // Search conversations
    if (searchConversations) {
        searchConversations.addEventListener('input', function () {
            const searchTerm = this.value.toLowerCase();
            conversations.forEach(conversation => {
                const userName = conversation.querySelector('strong').textContent.toLowerCase();
                const message = conversation.querySelector('p').textContent.toLowerCase();

                if (userName.includes(searchTerm) || message.includes(searchTerm)) {
                    conversation.style.display = 'flex';
                } else {
                    conversation.style.display = 'none';
                }
            });
        });
    }
});