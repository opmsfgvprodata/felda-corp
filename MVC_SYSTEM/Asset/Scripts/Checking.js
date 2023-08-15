
    // Set timeout variables.
    var logoutUrl = ''; // URL to logout page.

    var warningTimer;
    var timeoutTimer;

    // Start timers.
    function StartTimers(url, timeoutwarningtime, timeoutime) {
        var timoutWarning = timeoutwarningtime; 
        var timoutNow = timeoutime; 
        //alert(timoutWarning);
        warningTimer = setTimeout("IdleWarning()", timoutWarning);
        timeoutTimer = setTimeout("IdleTimeout()", timoutNow);
        logoutUrl = url;
        //alert(logoutUrl);

    }

    // Reset timers.
    function ResetTimers(url,timeoutwarningtime,timeoutime) {
        clearTimeout(warningTimer);
        clearTimeout(timeoutTimer);
        StartTimers(url,timeoutwarningtime,timeoutime);
        //$.simplyToast('Sistem akan auto logout', 'warning');
    }

    // Show idle timeout warning dialog.
    function IdleWarning() {
        window.focus();
        self.focus();
        $.simplyToast('Sistem akan automatik keluar sebentar lagi', 'warning');
    }

    // Logout the user.
    function IdleTimeout() {
        window.focus();
        self.focus();
        window.location = logoutUrl;
    }