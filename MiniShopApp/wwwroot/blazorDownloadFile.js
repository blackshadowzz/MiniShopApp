window.DownloadReceiptFile = (fileName, contentType, base64Data) => {
    const link = document.createElement('a');
    const fileUrl = "data:" + contentType + ";base64," + base64Data;
    link.href = fileUrl;

    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    //// Open in new tab and auto-print
    //const printWindow = window.open(fileUrl, '_blank');
    //if (printWindow) {
    //    printWindow.onload = () => {
    //        printWindow.focus();
    //        printWindow.print();
    //    };
    //} else {
    //    alert("Popup blocked! Please allow popups for this site.");
    //}

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
