# Cosmos PDF Editor — Architecture Design Document

## 1. Overview

A full-featured PDF editor for Windows, targeting feature parity with Adobe Acrobat Pro. Built as a native Windows desktop application.

**Target Platform**: Windows 10/11 (x64, ARM64)

---

## 2. Recommended Tech Stack

| Layer | Choice | Rationale |
|-------|--------|-----------|
| **UI Framework** | WinUI 3 (Windows App SDK) | Native Windows 11 look, Fluent Design, best performance |
| **Language** | C# (.NET 8+) | Strong PDF library ecosystem, Windows-native |
| **PDF Core** | iText 7 (C#) | Most complete PDF manipulation, editing, signing |
| **PDF Rendering** | Pdfium + SkiaSharp | Fast, high-fidelity rendering (Chrome's engine) |
| **OCR Engine** | Tesseract 5 (Tesseract.NET) | Open-source, 100+ languages, offline |
| **Document Conversion** | LibreOffice SDK + custom converters | PDF ↔ Word/Excel/PPT |
| **Digital Signatures** | iText 7 + Windows Certificate Store | PKCS#7, CAdES, LTV, timestamps |
| **Cloud Storage** | Azure SDK / REST APIs | OneDrive, SharePoint, Dropbox, Google Drive |
| **Installer** | MSIX | Modern Windows packaging, auto-update, sandbox |

### Alternative (Cost-Effective) Stack
If commercial licenses are a concern:
- Replace iText 7 → **PdfSharp + custom engine** (AGPL, no cost for open-source projects)
- Replace Syncfusion → **CommunityToolkit.WinUI** (MIT)
- OCR → **Windows.Media.Ocr** (built-in, free)

---

## 3. Feature Map (Adobe Acrobat Parity)

### 3.1 View & Navigation
- [ ] Multi-tab document viewer
- [ ] Zoom (fit page, fit width, custom %)
- [ ] Page layouts: single, facing, continuous scroll
- [ ] Thumbnail panel (page navigator)
- [ ] Bookmarks panel (hierarchical tree)
- [ ] Attachments panel
- [ ] Full-screen reading mode
- [ ] Pan & Scroll tool
- [ ] Magnifier / Loupe
- [ ] Dark mode

### 3.2 Text Editing
- [ ] Edit existing text (inline WYSIWYG)
- [ ] Add new text boxes
- [ ] Change font, size, color, alignment
- [ ] Spell check (Hunspell)
- [ ] Find & Replace (text across all pages)
- [ ] Paragraph formatting
- [ ] Copy/paste text
- [ ] Auto font matching for edited text

### 3.3 Image & Object Editing
- [ ] Add images (PNG, JPEG, TIFF, BMP, SVG)
- [ ] Resize, rotate, crop images
- [ ] Replace images
- [ ] Move/resize any PDF object
- [ ] Object alignment tools (snap, distribute)
- [ ] Layer management (z-order)

### 3.4 Annotations & Markup
- [ ] Highlight, underline, strikethrough
- [ ] Sticky notes (with rich text)
- [ ] Freehand drawing (pen tool)
- [ ] Shapes (rectangle, circle, arrow, line, polygon)
- [ ] Text callouts
- [ ] Stamps (Approved, Draft, Confidential, custom)
- [ ] Sound/video attachments
- [ ] Annotation summary & filter
- [ ] Reply threads on annotations
- [ ] Measurement tools (distance, perimeter, area)

### 3.5 Forms
- [ ] Create form fields: text, checkbox, radio, dropdown, listbox, button, signature
- [ ] Form field properties (validation, calculation, formatting)
- [ ] JavaScript actions on form events
- [ ] Import/export form data (FDF, XFDF, XML)
- [ ] Flatten forms
- [ ] Auto-detect form fields from document layout
- [ ] Distribute & collect form responses

### 3.6 Page Organization
- [ ] Insert pages (blank, from file, from scanner)
- [ ] Delete pages
- [ ] Reorder pages (drag & drop)
- [ ] Rotate pages
- [ ] Extract pages to new PDF
- [ ] Split PDF (by pages, bookmarks, file size)
- [ ] Merge multiple PDFs
- [ ] Replace pages
- [ ] Crop pages
- [ ] Page labels & numbering
- [ ] Bates numbering

### 3.7 OCR & Scanned Documents
- [ ] OCR single page / entire document
- [ ] Make scanned PDF searchable (text layer)
- [ ] Make scanned PDF editable (convert to editable text)
- [ ] Deskew scanned pages
- [ ] Auto-detect language
- [ ] Batch OCR (multiple files)

### 3.8 Conversion & Export
- [ ] PDF → Word (.docx)
- [ ] PDF → Excel (.xlsx)
- [ ] PDF → PowerPoint (.pptx)
- [ ] PDF → HTML
- [ ] PDF → Images (PNG, JPEG, TIFF, BMP)
- [ ] PDF → PDF/A
- [ ] Word/Excel/PPT/HTML → PDF
- [ ] Image → PDF
- [ ] Batch conversion
- [ ] Create PDF portfolio

### 3.9 Security & Protection
- [ ] Password protect (open + permissions)
- [ ] Encrypt (AES-128, AES-256)
- [ ] Remove passwords/encryption
- [ ] Certificate-based security
- [ ] Redaction (permanent, with sanitization)
- [ ] Remove hidden information (metadata, comments, form data, hidden layers)
- [ ] Compare documents (visual diff)
- [ ] Sanitize document

### 3.10 Digital Signatures
- [ ] Sign with digital certificate
- [ ] Verify signatures
- [ ] Timestamp signatures
- [ ] Long-term validation (LTV)
- [ ] Multiple signatures
- [ ] Signature appearance customization
- [ ] Request e-signatures (workflow)

### 3.11 Headers, Footers & Watermarks
- [ ] Add headers and footers
- [ ] Add watermarks (text or image)
- [ ] Page numbers
- [ ] Date/time stamps
- [ ] Background colors/images
- [ ] Apply to specific page ranges

### 3.12 Accessibility
- [ ] Tagged PDF creation/editing
- [ ] Reading order panel
- [ ] Alt text for images
- [ ] Accessibility checker (PDF/UA)
- [ ] Auto-tag document
- [ ] Screen reader support

### 3.13 PDF Standards
- [ ] PDF/A-1, PDF/A-2, PDF/A-3 validation & conversion
- [ ] PDF/X validation
- [ ] PDF/UA validation
- [ ] Preflight checks
- [ ] Preflight profiles (fixups)

### 3.14 Collaboration & Review
- [ ] Shared review (upload, invite, track)
- [ ] Comment management
- [ ] Review status tracking
- [ ] Email-based review workflow

### 3.15 Automation & Productivity
- [ ] Action Wizard (multi-step automation)
- [ ] JavaScript API for PDF actions
- [ ] Batch processing
- [ ] Custom stamps
- [ ] Quick tools / favorites
- [ ] Recent files with thumbnails
- [ ] Cloud sync (OneDrive, Dropbox, Google Drive)

---

## 4. Module Architecture

```
┌─────────────────────────────────────────────────────┐
│                   Cosmos PDF Editor                  │
├─────────────────────────────────────────────────────┤
│                    Presentation                      │
│  ┌───────────┐ ┌───────────┐ ┌───────────────────┐ │
│  │  WinUI 3  │ │  Fluent   │ │   Custom Controls  │ │
│  │  Shell    │ │  Design   │ │   (Canvas, Tools)  │ │
│  └─────┬─────┘ └─────┬─────┘ └────────┬──────────┘ │
├────────┼─────────────┼────────────────┼─────────────┤
│        └──────┬──────┘                │             │
│              ViewModel Layer (MVVM)    │             │
│  ┌────────────────────────────────────┴──────────┐  │
│  │  DocumentVM │ ToolsVM │ AnnotationsVM │ ...    │  │
│  └─────────────────────┬─────────────────────────┘  │
├────────────────────────┼────────────────────────────┤
│                   Services Layer                    │
│  ┌──────────┐ ┌───────┐ ┌────────┐ ┌───────────┐  │
│  │ Document │ │  OCR  │ │ Convert│ │ Signature  │  │
│  │ Service  │ │Service│ │Service │ │  Service   │  │
│  └────┬─────┘ └───┬───┘ └───┬────┘ └─────┬─────┘  │
│  ┌────┴───┐ ┌─────┴──┐ ┌────┴───┐ ┌─────┴─────┐  │
│  │ Edit   │ │Annotate│ │ Form   │ │ Security   │  │
│  │Service │ │Service │ │Service │ │  Service   │  │
│  └────┬───┘ └───┬────┘ └───┬────┘ └─────┬─────┘  │
├────────┼─────────┼──────────┼────────────┼──────────┤
│                    Core Engine                      │
│  ┌──────────┐ ┌───────────┐ ┌──────────────────┐   │
│  │ Pdfium   │ │  iText 7  │ │  Tesseract.NET   │   │
│  │ Renderer │ │  PDF Core │ │  OCR Engine       │   │
│  └──────────┘ └───────────┘ └──────────────────┘   │
│  ┌──────────┐ ┌───────────┐ ┌──────────────────┐   │
│  │ Skia     │ │  Undo/    │ │  Plugin          │   │
│  │ Graphics │ │  Redo Mgr │ │  System          │   │
│  └──────────┘ └───────────┘ └──────────────────┘   │
└─────────────────────────────────────────────────────┘
```

---

## 5. Key Design Patterns

| Pattern | Where Used |
|---------|-----------|
| **MVVM** | All UI ↔ data binding |
| **Command** | All user actions (undoable) |
| **Observer** | Document change notifications |
| **Strategy** | Tool switching (pen, select, text, etc.) |
| **Memento** | Undo/Redo system |
| **Factory** | Document creation, export formats |
| **Plugin** | Extensibility via MEF/dependency injection |

---

## 6. Undo/Redo System

All document modifications go through a **Command Pattern** with an undo stack:

```csharp
public interface ICommand
{
    void Execute();
    void Undo();
    string Description { get; }
}

public class CommandManager
{
    private readonly Stack<ICommand> _undoStack;
    private readonly Stack<ICommand> _redoStack;
    // Grouped commands for multi-step operations
    // Auto-save checkpoints every N operations
}
```

---

## 7. Rendering Pipeline

```
PDF Page → Pdfium parse → SkiaSharp bitmap
    → Transform (zoom, rotation, crop)
    → Overlay layer (annotations, selections, guides)
    → Composite → GPU-accelerated display (Win2D/Direct2D)
```

- **Lazy rendering**: Only visible pages rendered at current zoom
- **Tile-based**: Large pages split into tiles for smooth zooming
- **Background threading**: Rendering off UI thread
- **Cache**: LRU cache of rendered page tiles

---

## 8. Data Flow for Text Editing

```
User clicks text → Hit-test on PDF objects
    → Identify text object (font, position, content)
    → Load into editable TextBlock
    → User modifies → Diff changes
    → iText 7 writes modified content stream
    → Re-render affected page region
    → Push to undo stack
```

---

## 9. Project Structure

```
Cosmos.sln
├── src/
│   ├── Cosmos.App/                    # WinUI 3 application entry point
│   ├── Cosmos.Core/                   # Core PDF engine abstractions
│   │   ├── Interfaces/
│   │   ├── Models/
│   │   │   ├── PdfDocument.cs
│   │   │   ├── PdfPage.cs
│   │   │   ├── PdfAnnotation.cs
│   │   │   ├── PdfFormField.cs
│   │   │   └── PdfBookmark.cs
│   │   ├── Commands/                  # Undo/Redo commands
│   │   └── Extensions/
│   ├── Cosmos.Rendering/              # Pdfium + SkiaSharp rendering
│   ├── Cosmos.Editing/                # Text & object editing engine
│   ├── Cosmos.Annotations/            # Annotation creation & rendering
│   ├── Cosmos.Forms/                  # PDF form handling
│   ├── Cosmos.OCR/                    # Tesseract integration
│   ├── Cosmos.Conversion/             # Import/export formats
│   ├── Cosmos.Security/              # Encryption, redaction, signing
│   ├── Cloud/                  # Cloud storage integrations
│   ├── Cosmos.Plugins/                # Plugin infrastructure
│   └── Cosmos.Tests/                  # Unit & integration tests
├── assets/                            # Icons, stamps, templates
├── docs/                              # Documentation
└── tools/                             # Build scripts, codegen
```

---

## 10. Development Phases

### Phase 1 — Foundation (MVP Viewer + Basic Editor)
- WinUI 3 shell with tabbed document viewer
- Pdfium rendering pipeline with zoom/pan/scroll
- Basic text selection and copy
- Page navigation (thumbnails, bookmarks)
- Open/save PDF files
- Undo/redo infrastructure

### Phase 2 — Core Editing
- Text editing (modify existing, add new)
- Image insertion and manipulation
- Basic annotations (highlight, note, shapes)
- Page operations (insert, delete, reorder, rotate)
- Find & Replace

### Phase 3 — Advanced Features
- OCR engine integration
- Form field creation and editing
- Merge/split PDFs
- Headers, footers, watermarks, page numbers
- Stamps and custom annotations

### Phase 4 — Security & Signatures
- Password protection and encryption
- Redaction tool
- Digital signature creation and verification
- Certificate management
- Document sanitization

### Phase 5 — Conversion & Export
- PDF → Word/Excel/PPT converters
- Image export
- PDF/A validation and conversion
- Batch processing
- Import from multiple formats

### Phase 6 — Collaboration & Cloud
- Shared review workflows
- Cloud storage integration (OneDrive, Dropbox)
- Comment threading
- Document comparison (visual diff)

### Phase 7 — Polish & Ship
- Accessibility (PDF/UA, screen reader)
- Plugin system for extensibility
- Performance optimization
- MSIX packaging and auto-update
- Localization (multi-language UI)

---

## 11. Licensing & Cost Considerations

| Component | License | Cost |
|-----------|---------|------|
| WinUI 3 / .NET 8 | MIT | Free |
| Pdfium | BSD | Free |
| SkiaSharp | MIT | Free |
| iText 7 | **AGPL** (commercial license needed for closed-source) | ~$2,500+/year or open-source |
| Tesseract | Apache 2.0 | Free |
| Hunspell | MPL/LGPL | Free |

**iText 7 alternative for commercial use**: Use **PdfSharp/MigraDoc** (MIT) + custom content stream editing — more work, no license cost.

---

## 12. Next Steps

1. Set up solution structure and WinUI 3 project
2. Integrate Pdfium + SkiaSharp for basic PDF rendering
3. Build tabbed document viewer with navigation
4. Implement first editing feature (text selection → copy)
5. Wire up undo/redo command infrastructure
