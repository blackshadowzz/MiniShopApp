function DownloadReceipt(fileName, contentType, base64Data) {
    const link = document.createElement('a');
    const fileUrl = "data:" + contentType + ";base64," + base64Data;
    link.href = fileUrl;

    link.download = fileName;
    //document.body.appendChild(link);
    //link.click();
    //document.body.removeChild(link);

    // Step 1: Decode base64 into raw binary
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);

    // Step 2: Create a Blob URL
    const blob = new Blob([byteArray], { type: 'application/pdf' });
    const blobUrl = URL.createObjectURL(blob);


    // Step 3: Open new tab and embed the PDF inside iframe
    const html = `
      <html>
        <body style="margin:0">
          <iframe src="${blobUrl}" style="width:100%;height:100vh;border:none;" id="pdfFrame"></iframe>
          <script>
            const iframe = document.getElementById("pdfFrame");
            iframe.onload = () => {
              setTimeout(() => {
                iframe.contentWindow.focus();
                iframe.contentWindow.print();
              }, 500);
            };
          </script>
        </body>
      </html>
    `;


    const win = window.open("", "_blank");
    if (win) {
        win.document.open();
        win.document.write(html);
        
        win.document.close();
       
    } else {
        alert("Popup blocked. Please allow popups for this site.");
    }



};