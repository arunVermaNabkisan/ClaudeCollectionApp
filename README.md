# PDF Merge Web Application - Blazor .NET 9

A modern, server-side Blazor web application for merging two PDF files into one. Built with .NET 9 and C#, this application provides a responsive and user-friendly interface for PDF manipulation.

## Features

- 📄 Merge two PDF files into a single document
- 🎨 Modern, responsive user interface
- 🔒 Server-side processing with iText7 library
- 📱 Mobile-friendly design
- 🎯 Drag and drop support
- ⚡ Fast and efficient PDF merging
- 🚀 Built with Blazor Server for interactive components

## Technologies Used

- **ASP.NET Core 9.0** - Modern web framework
- **Blazor Server** - Interactive server-side rendering
- **C# 12** - Latest C# features
- **iText7** - Professional PDF manipulation library
- **CSS3** - Modern styling with gradients and animations
- **JavaScript Interop** - For file downloads

## Prerequisites

To run this application, you need:

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- A modern web browser (Chrome, Edge, Firefox, Safari)

## Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd ClaudeCollectionApp
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Build the application:**
   ```bash
   dotnet build
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

5. **Open your browser:**
   - Navigate to `https://localhost:5001` or `http://localhost:5000`
   - The application will open automatically if configured in `launchSettings.json`

## How to Use

1. Open the application in your web browser
2. Click on the first upload box and select your first PDF file (or drag and drop)
3. Click on the second upload box and select your second PDF file (or drag and drop)
4. Click the "Merge PDFs" button
5. Wait for the merge process to complete (progress bar will show status)
6. The merged PDF will be automatically downloaded to your device

## Project Structure

```
ClaudeCollectionApp/
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor      # Main layout component
│   ├── Pages/
│   │   └── Home.razor             # Home page with PDF merge functionality
│   ├── App.razor                  # Root component
│   ├── Routes.razor               # Routing configuration
│   └── _Imports.razor             # Global using statements
├── Properties/
│   └── launchSettings.json        # Development server configuration
├── wwwroot/
│   ├── css/
│   │   └── app.css                # Application styles
│   └── js/
│       └── download.js            # JavaScript for file downloads
├── ClaudeCollectionApp.csproj     # Project file
├── Program.cs                     # Application entry point
├── appsettings.json               # Configuration
└── README.md                      # This file
```

## Configuration

### Launch Settings

The application is configured to run on:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`

You can modify these settings in `Properties/launchSettings.json`.

### File Size Limits

The application supports PDF files up to 50MB per file. You can adjust this limit in `Components/Pages/Home.razor`:

```csharp
await file.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024).CopyToAsync(memoryStream);
```

## Dependencies

This project uses the following NuGet packages:

- **itext7** (8.0.5) - Core PDF library
- **itext7.bouncy-castle-adapter** (8.0.5) - Security features for iText7

All dependencies are managed through NuGet and will be restored automatically when you build the project.

## Development

### Building for Production

```bash
dotnet publish -c Release
```

The published files will be in the `bin/Release/net9.0/publish/` directory.

### Running in Development Mode

```bash
dotnet watch run
```

This will start the application with hot reload enabled, automatically restarting when you make changes to the code.

## Deployment

### Deploy to Azure

```bash
az webapp up --name <app-name> --resource-group <resource-group>
```

### Deploy to IIS

1. Publish the application:
   ```bash
   dotnet publish -c Release
   ```

2. Copy the contents of `bin/Release/net9.0/publish/` to your IIS web directory

3. Configure IIS with the ASP.NET Core Module

### Deploy to Docker

Create a `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ClaudeCollectionApp.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClaudeCollectionApp.dll"]
```

Then build and run:
```bash
docker build -t pdf-merger .
docker run -p 8080:80 pdf-merger
```

## Privacy & Security

- PDF files are processed on the server during the session
- Files are kept in memory and never stored permanently on disk
- All processing happens during the user session
- Files are disposed of immediately after merging
- Secure HTTPS connection recommended for production

## Browser Compatibility

Works with all modern browsers:
- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Opera (latest)

## Future Enhancements

Potential features for future versions:
- Support for merging more than 2 PDFs
- Page reordering within PDFs
- PDF page deletion
- Page rotation
- PDF compression options
- Batch processing
- User authentication and saved merge history
- Cloud storage integration

## Troubleshooting

### Port Already in Use

If ports 5000 or 5001 are already in use, you can specify different ports:

```bash
dotnet run --urls="http://localhost:5050;https://localhost:5051"
```

### PDF Merge Fails

- Ensure both PDF files are valid and not corrupted
- Check that files are not password-protected
- Verify file sizes are within the 50MB limit

### Application Won't Start

- Verify .NET 9 SDK is installed: `dotnet --version`
- Ensure all NuGet packages are restored: `dotnet restore`
- Check for port conflicts

## License

This project is open source and available for personal and commercial use.

## Support

For issues, questions, or contributions, please open an issue in the GitHub repository.
