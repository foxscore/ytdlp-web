function setCookie(name, value, days) {
    const date = new Date();
    date.setDate(date.getDate() + days);
    document.cookie = name + "=" + encodeURIComponent(value) + ((days == null) ? "" : "; expires=" + date.toUTCString());
}

function getCookie(name) {
    let i, x, y, ARCookies = document.cookie.split(";");
    for (i = 0; i < ARCookies.length; i++) {
        x = ARCookies[i].substring(0, ARCookies[i].indexOf("="));
        y = ARCookies[i].substring(ARCookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");
        if (x === name) {
            return decodeURIComponent(y);
        }
    }
}

function getBaseURL() {
    const currentURL = window.location.href;
    const urlObject = new URL(currentURL);
    return urlObject.origin;
}

async function copyToClipboard(text) {
    try {
        await navigator.clipboard.writeText(text);
        console.log("Copied to clipboard!");
    } catch (err) {
        console.error("Failed to copy:", err);
        // ToDo: Show proper error notification
    }
}