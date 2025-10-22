class MessagingSystem {
    constructor() {
        this.currentChatUser = null;
        this.currentConversationId = null;
        this.isWidgetOpen = false;
        this.unreadCount = 0;
        this.hubConnection = null;
        this.userId = document.body.dataset.userId;

        this.initializeSignalR();
        this.initializeEventListeners();
    }

    async initializeSignalR() {
        // Import SignalR
        await this.loadScript('https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/6.0.1/signalr.min.js');

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/messagingHub")
            .withAutomaticReconnect()
            .build();

        this.setupHubEvents();
        await this.startConnection();
    }

    loadScript(src) {
        return new Promise((resolve, reject) => {
            if (document.querySelector(`script[src="${src}"]`)) {
                resolve();
                return;
            }
            const script = document.createElement('script');
            script.src = src;
            script.onload = resolve;
            script.onerror = reject;
            document.head.appendChild(script);
        });
    }

    setupHubEvents() {
        this.hubConnection.on("ReceiveMessage", (senderId, message) => {
            this.handleIncomingMessage(senderId, message);
        });

        this.hubConnection.on("MessageSent", (receiverId, message) => {
            this.handleMessageSent(receiverId, message);
        });

        this.hubConnection.on("UserOnline", (userId) => {
            this.updateUserStatus(userId, true);
        });

        this.hubConnection.on("UserOffline", (userId) => {
            this.updateUserStatus(userId, false);
        });
    }

    async startConnection() {
        try {
            await this.hubConnection.start();
            console.log("SignalR Connected");
        } catch (err) {
            console.error("SignalR Connection Error: ", err);
            setTimeout(() => this.startConnection(), 5000);
        }
    }

    initializeEventListeners() {
        // Toggle widget
        const toggleBtn = document.getElementById('messageWidgetBtn');
        const widget = document.getElementById('messagingWidget');
        const minimizeBtn = document.getElementById('minimizeWidget');
        const closeBtn = document.getElementById('closeWidget');
        const backBtn = document.getElementById('backToConversations');

        toggleBtn?.addEventListener('click', () => this.toggleWidget());
        minimizeBtn?.addEventListener('click', () => this.minimizeWidget());
        closeBtn?.addEventListener('click', () => this.closeWidget());
        backBtn?.addEventListener('click', () => this.showConversations());

        // Message sending
        const sendBtn = document.getElementById('sendMessage');
        const messageInput = document.getElementById('messageInput');

        sendBtn?.addEventListener('click', () => this.sendMessage());
        messageInput?.addEventListener('keypress', (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                this.sendMessage();
            }
        });

        // User search with debounce
        const searchInput = document.getElementById('searchUsers');
        let searchTimeout;
        searchInput?.addEventListener('input', (e) => {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                this.searchUsers(e.target.value);
            }, 300);
        });

        // Close widget when clicking outside
        document.addEventListener('click', (e) => {
            if (this.isWidgetOpen &&
                !widget.contains(e.target) &&
                !toggleBtn.contains(e.target)) {
                this.closeWidget();
            }
        });
    }

    toggleWidget() {
        const widget = document.getElementById('messagingWidget');
        widget.classList.toggle('active');
        this.isWidgetOpen = widget.classList.contains('active');

        if (this.isWidgetOpen) {
            this.resetUnreadCount();
            this.loadConversations();
        }
    }

    minimizeWidget() {
        const widget = document.getElementById('messagingWidget');
        widget.classList.remove('active');
        this.isWidgetOpen = false;
    }

    closeWidget() {
        this.minimizeWidget();
        this.showConversations();
    }

    async loadConversations() {
        const container = document.getElementById('conversationsContainer');
        if (!container) return;

        try {
            const conversations = await this.fetchConversations();
            this.renderConversations(conversations);
        } catch (error) {
            console.error('Error loading conversations:', error);
            container.innerHTML = this.getEmptyState('Error loading conversations');
        }
    }

    async fetchConversations() {
        // Simulated conversations data - replace with actual API call
        return [
            {
                id: 'conv1',
                userId: 'user2',
                userName: 'Alice Johnson',
                userInitials: 'AJ',
                lastMessage: 'Hey, are we still meeting today?',
                timestamp: '2 min ago',
                unread: true,
                isOnline: true
            },
            {
                id: 'conv2',
                userId: 'user3',
                userName: 'Bob Smith',
                userInitials: 'BS',
                lastMessage: 'Thanks for the help with the project!',
                timestamp: '1 hour ago',
                unread: false,
                isOnline: false
            },
            {
                id: 'conv3',
                userId: 'user4',
                userName: 'Carol Davis',
                userInitials: 'CD',
                lastMessage: 'Can you explain this concept again?',
                timestamp: '3 hours ago',
                unread: true,
                isOnline: true
            }
        ];
    }

    renderConversations(conversations) {
        const container = document.getElementById('conversationsContainer');
        if (!container) return;

        if (conversations.length === 0) {
            container.innerHTML = this.getEmptyState('No conversations yet');
            return;
        }

        container.innerHTML = conversations.map(conv => `
            <div class="conversation-item" data-conversation-id="${conv.id}" data-user-id="${conv.userId}">
                <div class="conversation-avatar ${conv.isOnline ? 'online' : ''}">${conv.userInitials}</div>
                <div class="conversation-info">
                    <div class="conversation-name">
                        ${conv.userName}
                        ${conv.unread ? '<span class="badge bg-danger ms-1">●</span>' : ''}
                    </div>
                    <div class="conversation-preview">${conv.lastMessage}</div>
                </div>
                <div class="conversation-time">${conv.timestamp}</div>
            </div>
        `).join('');

        // Add click event listeners to conversation items
        container.querySelectorAll('.conversation-item').forEach(item => {
            item.addEventListener('click', () => {
                const conversationId = item.dataset.conversationId;
                const userId = item.dataset.userId;
                const userName = item.querySelector('.conversation-name').textContent.replace('●', '').trim();
                const userInitials = item.querySelector('.conversation-avatar').textContent;

                this.openChat(conversationId, userId, userName, userInitials);
            });
        });

        this.updateUnreadCount(conversations);
    }

    async searchUsers(query) {
        const container = document.getElementById('conversationsContainer');
        if (!container) return;

        if (!query.trim()) {
            this.loadConversations();
            return;
        }

        try {
            if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
                const results = await this.hubConnection.invoke("SearchUsers", query);
                this.renderSearchResults(results);
            } else {
                console.error('SignalR not connected');
                this.renderSearchResults([]);
            }
        } catch (error) {
            console.error('Error searching users:', error);
            this.renderSearchResults([]);
        }
    }

    renderSearchResults(users) {
        const container = document.getElementById('conversationsContainer');
        if (!container) return;

        if (users.length === 0) {
            container.innerHTML = this.getEmptyState('No users found');
            return;
        }

        container.innerHTML = users.map(userData => {
            const [userId, userName, userInitials] = userData.split('|');
            return `
                <div class="conversation-item" data-user-id="${userId}">
                    <div class="conversation-avatar">${userInitials}</div>
                    <div class="conversation-info">
                        <div class="conversation-name">${userName}</div>
                        <div class="conversation-preview">Click to start conversation</div>
                    </div>
                    <div class="conversation-time">
                        <button class="btn btn-primary btn-sm start-chat-btn">Chat</button>
                    </div>
                </div>
            `;
        }).join('');

        // Add event listeners for starting new chats
        container.querySelectorAll('.start-chat-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.stopPropagation();
                const conversationItem = btn.closest('.conversation-item');
                const userId = conversationItem.dataset.userId;
                const userName = conversationItem.querySelector('.conversation-name').textContent;
                const userInitials = conversationItem.querySelector('.conversation-avatar').textContent;

                this.startNewChat(userId, userName, userInitials);
            });
        });
    }

    async openChat(conversationId, userId, userName, userInitials) {
        this.currentConversationId = conversationId;
        this.currentChatUser = { id: userId, name: userName, initials: userInitials };

        // Show chat container
        document.querySelector('.conversations-list').classList.add('d-none');
        document.querySelector('.chat-container').classList.remove('d-none');

        // Update chat header
        document.getElementById('currentChatUser').textContent = userName;

        // Load messages
        await this.loadMessages(conversationId);

        // Mark as read
        this.markAsRead(conversationId);
    }

    startNewChat(userId, userName, userInitials) {
        // Create a new conversation ID
        const conversationId = `conv_${Date.now()}`;
        this.openChat(conversationId, userId, userName, userInitials);
    }

    showConversations() {
        document.querySelector('.conversations-list').classList.remove('d-none');
        document.querySelector('.chat-container').classList.add('d-none');
        this.currentConversationId = null;
        this.currentChatUser = null;
    }

    async loadMessages(conversationId) {
        const container = document.getElementById('messagesContainer');
        if (!container) return;

        try {
            const messages = await this.fetchMessages(conversationId);
            this.renderMessages(messages);
        } catch (error) {
            console.error('Error loading messages:', error);
            container.innerHTML = '<div class="empty-state">Error loading messages</div>';
        }
    }

    async fetchMessages(conversationId) {
        // Simulated messages - in real app, fetch from API
        return [
            {
                id: 1,
                text: 'Hey there! How can I help you today?',
                timestamp: '2:30 PM',
                sent: false
            },
            {
                id: 2,
                text: 'I have a question about the course material.',
                timestamp: '2:31 PM',
                sent: true
            }
        ];
    }

    renderMessages(messages) {
        const container = document.getElementById('messagesContainer');
        if (!container) return;

        if (messages.length === 0) {
            container.innerHTML = '<div class="empty-state">No messages yet. Start the conversation!</div>';
            return;
        }

        container.innerHTML = messages.map(msg => `
            <div class="message ${msg.sent ? 'sent' : 'received'}">
                <div class="message-text">${this.escapeHtml(msg.text)}</div>
                <div class="message-time">${msg.timestamp}</div>
            </div>
        `).join('');

        container.scrollTop = container.scrollHeight;
    }

    async sendMessage() {
        const input = document.getElementById('messageInput');
        const text = input?.value.trim();

        if (!text || !this.currentChatUser?.id) return;

        if (this.hubConnection?.state !== signalR.HubConnectionState.Connected) {
            alert('Connection lost. Please try again.');
            return;
        }

        try {
            await this.hubConnection.invoke("SendMessage", this.currentChatUser.id, text);
            input.value = '';
        } catch (error) {
            console.error('Error sending message:', error);
            alert('Failed to send message. Please try again.');
        }
    }

    handleIncomingMessage(senderId, message) {
        // If we're currently chatting with this user, add the message to UI
        if (this.currentChatUser?.id === senderId) {
            this.addMessageToUI({
                text: message,
                timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
                sent: false
            });
        } else {
            // Update unread count and conversation list
            this.incrementUnreadCount();
            this.updateConversationPreview(senderId, message);
        }
    }

    handleMessageSent(receiverId, message) {
        // Add sent message to UI
        this.addMessageToUI({
            text: message,
            timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
            sent: true
        });
    }

    addMessageToUI(message) {
        const container = document.getElementById('messagesContainer');
        if (!container) return;

        // Remove empty state if present
        if (container.querySelector('.empty-state')) {
            container.innerHTML = '';
        }

        const messageElement = document.createElement('div');
        messageElement.className = `message ${message.sent ? 'sent' : 'received'}`;
        messageElement.innerHTML = `
            <div class="message-text">${this.escapeHtml(message.text)}</div>
            <div class="message-time">${message.timestamp}</div>
        `;

        container.appendChild(messageElement);
        container.scrollTop = container.scrollHeight;
    }

    updateConversationPreview(userId, lastMessage) {
        // Update the conversation list with the new message
        const conversationItem = document.querySelector(`[data-user-id="${userId}"]`);
        if (conversationItem) {
            const preview = conversationItem.querySelector('.conversation-preview');
            const time = conversationItem.querySelector('.conversation-time');
            if (preview) preview.textContent = lastMessage;
            if (time) time.textContent = 'Just now';

            // Add unread indicator
            conversationItem.querySelector('.conversation-name').innerHTML +=
                '<span class="badge bg-danger ms-1">●</span>';
        }
    }

    updateUserStatus(userId, isOnline) {
        const conversationItem = document.querySelector(`[data-user-id="${userId}"]`);
        if (conversationItem) {
            const avatar = conversationItem.querySelector('.conversation-avatar');
            if (isOnline) {
                avatar.classList.add('online');
            } else {
                avatar.classList.remove('online');
            }
        }
    }

    incrementUnreadCount() {
        this.unreadCount++;
        this.updateBadge();
    }

    updateUnreadCount(conversations) {
        this.unreadCount = conversations.filter(conv => conv.unread).length;
        this.updateBadge();
    }

    resetUnreadCount() {
        this.unreadCount = 0;
        this.updateBadge();
    }

    updateBadge() {
        const badge = document.querySelector('.message-badge');
        if (badge) {
            if (this.unreadCount > 0) {
                badge.textContent = this.unreadCount > 99 ? '99+' : this.unreadCount;
                badge.classList.remove('d-none');
            } else {
                badge.classList.add('d-none');
            }
        }
    }

    markAsRead(conversationId) {
        const conversationItem = document.querySelector(`[data-conversation-id="${conversationId}"]`);
        if (conversationItem) {
            const badge = conversationItem.querySelector('.badge');
            if (badge) badge.remove();
        }
    }

    escapeHtml(unsafe) {
        return unsafe
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    getEmptyState(message) {
        return `
            <div class="empty-state">
                <i class="fas fa-comments"></i>
                <p>${message}</p>
            </div>
        `;
    }
}

// Initialize messaging system when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    new MessagingSystem();
});