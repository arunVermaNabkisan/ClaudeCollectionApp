// Get DOM elements
const pdf1Input = document.getElementById('pdf1');
const pdf2Input = document.getElementById('pdf2');
const filename1 = document.getElementById('filename1');
const filename2 = document.getElementById('filename2');
const mergeBtn = document.getElementById('mergeBtn');
const statusMessage = document.getElementById('statusMessage');
const progressContainer = document.getElementById('progressContainer');
const progressBar = document.getElementById('progressBar');

// Store the selected files
let file1 = null;
let file2 = null;

// Handle file selection for PDF 1
pdf1Input.addEventListener('change', (e) => {
    const file = e.target.files[0];
    if (file && file.type === 'application/pdf') {
        file1 = file;
        filename1.textContent = file.name;
        pdf1Input.parentElement.parentElement.classList.add('file-selected');
        updateMergeButton();
        clearStatus();
    } else {
        showStatus('Please select a valid PDF file', 'error');
        pdf1Input.value = '';
    }
});

// Handle file selection for PDF 2
pdf2Input.addEventListener('change', (e) => {
    const file = e.target.files[0];
    if (file && file.type === 'application/pdf') {
        file2 = file;
        filename2.textContent = file.name;
        pdf2Input.parentElement.parentElement.classList.add('file-selected');
        updateMergeButton();
        clearStatus();
    } else {
        showStatus('Please select a valid PDF file', 'error');
        pdf2Input.value = '';
    }
});

// Enable/disable merge button based on file selection
function updateMergeButton() {
    if (file1 && file2) {
        mergeBtn.disabled = false;
    } else {
        mergeBtn.disabled = true;
    }
}

// Show status message
function showStatus(message, type) {
    statusMessage.textContent = message;
    statusMessage.className = `status-message ${type}`;
}

// Clear status message
function clearStatus() {
    statusMessage.textContent = '';
    statusMessage.className = 'status-message';
}

// Update progress bar
function updateProgress(percent) {
    progressBar.style.width = percent + '%';
    progressBar.textContent = percent + '%';
}

// Show/hide progress bar
function showProgress(show) {
    progressContainer.style.display = show ? 'block' : 'none';
    if (!show) {
        updateProgress(0);
    }
}

// Convert file to ArrayBuffer
function fileToArrayBuffer(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = reject;
        reader.readAsArrayBuffer(file);
    });
}

// Merge PDFs using pdf-lib
async function mergePDFs() {
    try {
        // Disable button and show progress
        mergeBtn.disabled = true;
        showProgress(true);
        showStatus('Loading PDF files...', 'info');
        updateProgress(10);

        // Read both PDF files
        const pdf1Bytes = await fileToArrayBuffer(file1);
        updateProgress(25);

        const pdf2Bytes = await fileToArrayBuffer(file2);
        updateProgress(40);

        showStatus('Merging PDFs...', 'info');

        // Load the PDFs
        const pdf1Doc = await PDFLib.PDFDocument.load(pdf1Bytes);
        updateProgress(55);

        const pdf2Doc = await PDFLib.PDFDocument.load(pdf2Bytes);
        updateProgress(70);

        // Create a new PDF document
        const mergedPdf = await PDFLib.PDFDocument.create();
        updateProgress(75);

        // Copy pages from first PDF
        const pages1 = await mergedPdf.copyPages(pdf1Doc, pdf1Doc.getPageIndices());
        pages1.forEach((page) => mergedPdf.addPage(page));
        updateProgress(85);

        // Copy pages from second PDF
        const pages2 = await mergedPdf.copyPages(pdf2Doc, pdf2Doc.getPageIndices());
        pages2.forEach((page) => mergedPdf.addPage(page));
        updateProgress(95);

        // Save the merged PDF
        const mergedPdfBytes = await mergedPdf.save();
        updateProgress(100);

        showStatus('PDFs merged successfully!', 'success');

        // Download the merged PDF
        downloadPDF(mergedPdfBytes);

        // Reset after a delay
        setTimeout(() => {
            showProgress(false);
            showStatus('Ready to merge more PDFs', 'info');
        }, 2000);

    } catch (error) {
        console.error('Error merging PDFs:', error);
        showStatus('Error merging PDFs: ' + error.message, 'error');
        showProgress(false);
    } finally {
        mergeBtn.disabled = false;
        updateMergeButton();
    }
}

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

// Handle merge button click
mergeBtn.addEventListener('click', mergePDFs);

// Add drag and drop support
function setupDragAndDrop(inputElement, filenameElement) {
    const uploadBox = inputElement.parentElement.querySelector('.upload-box');

    uploadBox.addEventListener('dragover', (e) => {
        e.preventDefault();
        uploadBox.style.background = '#e8ebff';
    });

    uploadBox.addEventListener('dragleave', () => {
        uploadBox.style.background = '#f8f9ff';
    });

    uploadBox.addEventListener('drop', (e) => {
        e.preventDefault();
        uploadBox.style.background = '#f8f9ff';

        const files = e.dataTransfer.files;
        if (files.length > 0 && files[0].type === 'application/pdf') {
            inputElement.files = files;
            inputElement.dispatchEvent(new Event('change'));
        } else {
            showStatus('Please drop a valid PDF file', 'error');
        }
    });
}

// Enable drag and drop for both upload areas
setupDragAndDrop(pdf1Input, filename1);
setupDragAndDrop(pdf2Input, filename2);

// Initial status
showStatus('Select two PDF files to merge', 'info');
