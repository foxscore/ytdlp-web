const linkInput = document.getElementById('sourceUrlInput');
const mp3Button = document.getElementById('submitMp3');
const mp4Button = document.getElementById('submitMp4');

function submitDownloadRequest(type) {
    function setButtonsEnabled(state) {
        linkInput.enabled = state;
        mp3Button.enabled = state;
        mp4Button.enabled = state;
    }
    function fail(message) {
        setButtonsEnabled(true);
        // ToDo: Show error notification
    }
    
    if (type !== 'audio' || type !== 'video')
        fail('Invalid type: ' + type);
    
    setButtonsEnabled(false);
    
    fetch('/api/initiateDownload', {
        method: 'POST',
        body: JSON.stringify({
            url: linkInput.value,
            type: type,
        }),
        headers: {
            'Content-Type': 'application/json',
        }
    })
        .then(async res => {
            if (!res.ok){
                if (res.status === 422) {
                    fail(await res.text());
                } else {
                    fail('There was an error while trying to start the download.')
                }
                return;
            }
            const id = await res.text();
            document.location.href = '/' + id;
        })
        .catch(e => {
            console.error(e);
            fail('Failed to ')
        })
}

mp3Button.onclick = () => submitDownloadRequest('audio');
mp4Button.onclick = () => submitDownloadRequest('video');