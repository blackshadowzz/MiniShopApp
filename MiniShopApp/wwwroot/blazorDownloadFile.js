window.DownloadReceiptFile = (fileName, contentType, base64Data) => {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = "data:" + contentType + ";base64," + base64Data;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
window.DownloadReportFile = (fileName, contentType, content) => {
    // Convert base64 to raw binary data held in a string
    var byteCharacters = atob(btoa(String.fromCharCode.apply(null, content)));
    var byteNumbers = new Array(byteCharacters.length);
    for (var i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    var byteArray = new Uint8Array(byteNumbers);
    var blob = new Blob([byteArray], { type: contentType });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
};
