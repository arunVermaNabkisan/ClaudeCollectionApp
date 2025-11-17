// Initialize PDF merge functionality
window.initializePdfMerge = function () {
    console.log('PDF Merge initialized');
};

// Convert base64 to Uint8Array
function base64ToUint8Array(base64) {
    const binaryString = window.atob(base64);
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes;
}

// Merge PDFs using pdf-lib
window.mergePDFsInBrowser = async function (pdf1Base64, pdf2Base64) {
    try {
        console.log('Starting PDF merge...');

        // Convert base64 strings to Uint8Array
        const pdf1Bytes = base64ToUint8Array(pdf1Base64);
        const pdf2Bytes = base64ToUint8Array(pdf2Base64);

        // Load the PDFs
        const pdf1Doc = await PDFLib.PDFDocument.load(pdf1Bytes);
        const pdf2Doc = await PDFLib.PDFDocument.load(pdf2Bytes);

        // Create a new PDF document
        const mergedPdf = await PDFLib.PDFDocument.create();

        // Copy pages from first PDF
        const pages1 = await mergedPdf.copyPages(pdf1Doc, pdf1Doc.getPageIndices());
        pages1.forEach((page) => mergedPdf.addPage(page));

        // Copy pages from second PDF
        const pages2 = await mergedPdf.copyPages(pdf2Doc, pdf2Doc.getPageIndices());
        pages2.forEach((page) => mergedPdf.addPage(page));

        // Save the merged PDF
        const mergedPdfBytes = await mergedPdf.save();

        // Download the merged PDF
        downloadPDF(mergedPdfBytes);

        console.log('PDF merge completed successfully');
        return true;
    } catch (error) {
        console.error('Error merging PDFs:', error);
        alert('Error merging PDFs: ' + error.message);
        return false;
    }
};

// Download the merged PDF
function downloadPDF(pdfBytes) {
    const blob = new Blob([pdfBytes], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;

    // Generate filename with timestamp
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, -5);
    link.download = `merged-pdf-${timestamp}.pdf`;

    // Trigger download
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    // Clean up the URL object
    URL.revokeObjectURL(url);
}
