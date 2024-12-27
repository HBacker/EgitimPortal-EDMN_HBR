let connection = new signalR.HubConnectionBuilder()
    .withUrl("/generalhub")
    .withAutomaticReconnect()
    .build();

async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(startConnection, 5000);
    }
};

connection.onclose(async () => {
    await startConnection();
});

connection.on("ReceiveCourseNotification", function (notification) {
    Swal.fire({
        title: 'Yeni Kurs Eklendi!',
        text: `${notification.courseTitle} - ${notification.message}`,
        icon: 'success',
        timer: 4000,
        timerProgressBar: true,
        showConfirmButton: false
    });
});

startConnection();
