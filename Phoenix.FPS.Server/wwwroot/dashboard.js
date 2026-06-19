export function startPolling(dotNetRef, intervalMs) {
    window.dashboardPollTimer = setInterval(() => {
        dotNetRef.invokeMethodAsync('Poll');
    }, intervalMs);
}

export function stopPolling() {
    if (window.dashboardPollTimer) {
        clearInterval(window.dashboardPollTimer);
        window.dashboardPollTimer = null;
    }
}
