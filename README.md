# Cosmos PDF Editor

A full-featured PDF editor for Windows, targeting feature parity with Adobe Acrobat Pro.

## Status

**Phase 1 - Foundation** (In Progress)

## Features (Phase 1)

- Multi-tab document viewer with WinUI 3
- High-performance PDF rendering using Pdfium + SkiaSharp
- Page navigation with thumbnail panel
- Zoom and pan controls
- Basic document open/save

## Tech Stack

- **UI**: WinUI 3 (Windows App SDK)
- **Language**: C# / .NET 8+
- **PDF Rendering**: Pdfium + SkiaSharp
- **PDF Core**: iText 7
- **Architecture**: MVVM with CommunityToolkit.Mvvm

## Building

### Prerequisites

- Windows 10 version 1809 or higher
- Visual Studio 2022 with Windows App SDK workload
- .NET 8 SDK

### Build Commands

```bash
dotnet restore
dotnet build
```

### Run

```bash
dotnet run --project src/Cosmos.App
```

## Project Structure

```
Cosmos/
├── src/
│   ├── Cosmos.App/              # WinUI 3 application
│   ├── Cosmos.Core/             # Core models and abstractions
│   └── Cosmos.Rendering/        # Pdfium rendering engine
├── docs/
│   └── DESIGN.md                # Architecture design document
└── README.md
```

## Roadmap

See [docs/DESIGN.md](docs/DESIGN.md) for the complete architecture and feature roadmap.

### Phase 1 - Foundation ✓
- [x] Solution structure
- [x] WinUI 3 shell
- [x] Pdfium rendering integration
- [x] Basic document viewer
- [ ] Page navigation
- [ ] Open/save functionality

### Phase 2 - Core Editing
- [ ] Text editing
- [ ] Image insertion
- [ ] Annotations
- [ ] Page operations

### Phase 3+ 
- [ ] OCR
- [ ] Forms
- [ ] Security & Signatures
- [ ] Conversion
- [ ] Collaboration

## License

TBD - See [docs/DESIGN.md](docs/DESIGN.md#11-licensing--cost-considerations) for library licensing information.
