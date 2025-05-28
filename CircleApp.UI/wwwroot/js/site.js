function updateCharCount(textarea) {
    const charCount = textarea.parentElement.querySelector('.char-count');
    const submitBtn = textarea.closest('form').querySelector('.add-comment-button');
    const length = textarea.value.length;

    charCount.textContent = length;

    // Update submit button state
    submitBtn.disabled = length === 0 || length > 500;

    // Update character counter color
    if (length > 450) {
        charCount.style.color = '#ef4444'; // red
    } else if (length > 400) {
        charCount.style.color = '#f59e0b'; // yellow
    } else {
        charCount.style.color = '#9ca3af'; // gray
    }
}

function expandCommentBox(textarea) {
    textarea.style.minHeight = '120px';
    textarea.parentElement.parentElement.style.transform = 'scale(1.02)';
}

function collapseCommentBox(textarea) {
    if (!textarea.value.trim()) {
        textarea.style.minHeight = '80px';
        textarea.parentElement.parentElement.style.transform = 'scale(1)';
    }
}

function clearComment(button) {
    const form = button.closest('form');
    const textarea = form.querySelector('textarea');
    const charCount = form.querySelector('.char-count');
    const submitBtn = form.querySelector('.add-comment-button');

    textarea.value = '';
    charCount.textContent = '0';
    charCount.style.color = '#9ca3af';
    submitBtn.disabled = true;

    textarea.style.minHeight = '80px';
    textarea.parentElement.parentElement.style.transform = 'scale(1)';
}


document.addEventListener('DOMContentLoaded', async function () {
    fetch("Notifications/GetNotificationsCount", {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => response.json())
        .then(data => {
            document.getElementById('notificationsCount').innerHTML = data;
        })

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notification-hub")
        .build();

    await connection.start()
        .then(() => console.log("SignalR connected"))
        .catch((err) => console.log("SignalR connection error: ", err));

    await connection.on("ReceiveNotification", (message) => {
        console.log("message ==> ", message);
        document.getElementById("notificationsCount").innerHTML = message;
    });

    document.body.addEventListener('click', async function (event) {
        const notificationBtn = event.target.closest('#notificationBtn');
        if (!notificationBtn) {
            return;
        }
        event.preventDefault()
        notificationBtn.disabled = true;
        try {
            const response = await fetch("/Notifications/GetNotifications", {
                method: 'get',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
            })
            const html = await response.text()
            document.getElementById('notification-dropdown').innerHTML = html;
        } finally {
            notificationBtn.disabled = false
        }
    })
})

async function setNotificationAsRead(notificationId) {
    const response = await fetch(`Notifications/SetNotificationAsRead?notificationId=${notificationId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({notificationId: notificationId})
    })
    const html = await response.text()
    document.getElementById('notification-dropdown').innerHTML = html;
}
