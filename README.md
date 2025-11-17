# PDF Merge Blazor Application

A modern .NET Blazor WebAssembly application for merging two PDF files into one. Built with C# and running entirely in your browser - no files are uploaded to any server.

## Features

- 📄 Merge two PDF files into a single document
- 🎨 Modern, responsive Blazor UI
- 🔒 100% client-side processing - your files stay private
- 📱 Mobile-friendly design
- 🎯 Drag and drop support
- ⚡ Fast and efficient using pdf-lib library with C# interop
- 🔷 Built with .NET 8 and Blazor WebAssembly

## Technologies Used

- **.NET 8** - Latest .NET framework
- **Blazor WebAssembly** - Client-side C# web framework
- **C#** - Primary programming language
- **Razor Components** - Component-based UI
- **JavaScript Interop** - For PDF manipulation
- **[pdf-lib](https://pdf-lib.js.org/)** - JavaScript library for PDF manipulation

## File Structure

```
ClaudeCollectionApp/
├── PdfMergeApp.csproj          # Project file
├── Program.cs                   # Application entry point
├── App.razor                    # Root component
├── _Imports.razor               # Global using directives
├── Pages/
│   └── Index.razor              # Main PDF merge page
├── Shared/
│   └── MainLayout.razor         # Layout component
├── wwwroot/
│   ├── index.html               # Host HTML page
│   ├── css/
│   │   └── app.css              # Application styles
│   └── js/
│       └── pdfMerge.js          # PDF merging JavaScript
└── README.md                    # This file
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

## How to Run

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ClaudeCollectionApp
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Open your browser**
   - Navigate to `https://localhost:5001` (or the URL shown in the console)

5. **Use the application**
   - Click on the first upload box or drag and drop your first PDF file
   - Click on the second upload box or drag and drop your second PDF file
   - Click the "Merge PDFs" button
   - The merged PDF will be automatically downloaded to your device

## Build for Production

To build the application for production deployment:

```bash
dotnet publish -c Release
```

The output will be in `bin/Release/net8.0/publish/wwwroot/`

You can host these static files on any web server or static hosting service like:
- Azure Static Web Apps
- GitHub Pages
- Netlify
- Vercel

## Browser Compatibility

Works with all modern browsers that support WebAssembly:
- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)

## Privacy & Security

- All PDF processing happens entirely in your browser via WebAssembly
- No files are uploaded to any server
- No data is stored or transmitted
- Your documents remain completely private
- Runs completely client-side with no backend required

## Future Enhancements

Potential features for future versions:
- Support for merging more than 2 PDFs
- Page reordering
- PDF page deletion
- Page rotation
- PDF compression options

## License

This project is open source and available for personal and commercial use.