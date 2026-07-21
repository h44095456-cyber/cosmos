using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SkiaSharp;

namespace Cosmos.Rendering;

public class PdfiumRenderer : IPdfRenderer, IDisposable
{
    private IntPtr _document = IntPtr.Zero;
    private bool _disposed = false;

    public int PageCount { get; private set; }

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FPDF_InitLibrary();

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FPDF_DestroyLibrary();

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr FPDF_LoadDocument(string file_path, string? password);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FPDF_CloseDocument(IntPtr document);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int FPDF_GetPageCount(IntPtr document);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr FPDF_LoadPage(IntPtr document, int page_index);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FPDF_ClosePage(IntPtr page);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern double FPDF_GetPageWidth(IntPtr page);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern double FPDF_GetPageHeight(IntPtr page);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr FPDFBitmap_CreateEx(int width, int height, int format, IntPtr first_scan, int stride);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FPDFBitmap_Destroy(IntPtr bitmap);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FPDFBitmap_FillRect(IntPtr bitmap, int left, int top, int width, int height, uint color);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr FPDFBitmap_GetBuffer(IntPtr bitmap);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int FPDFBitmap_GetStride(IntPtr bitmap);

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FPDF_RenderPageBitmap(IntPtr bitmap, IntPtr page, int start_x, int start_y, int size_x, int size_y, int rotate, int flags);

    private const int FPDF_BITMAP_BGRA = 4;
    private const int FPDF_ANNOT = 0x01;
    private const int FPDF_LCD_TEXT = 0x02;

    private static bool _initialized = false;
    private static readonly object _initLock = new object();

    public PdfiumRenderer()
    {
        InitializePdfium();
    }

    private void InitializePdfium()
    {
        lock (_initLock)
        {
            if (!_initialized)
            {
                FPDF_InitLibrary();
                _initialized = true;
            }
        }
    }

    public Task LoadDocumentAsync(string path)
    {
        return Task.Run(() =>
        {
            if (_document != IntPtr.Zero)
            {
                FPDF_CloseDocument(_document);
                _document = IntPtr.Zero;
            }

            _document = FPDF_LoadDocument(path, null);
            if (_document == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Failed to load PDF: {path}");
            }

            PageCount = FPDF_GetPageCount(_document);
        });
    }

    public Task<byte[]?> RenderPageAsync(int pageIndex, float scale)
    {
        return Task.Run(() =>
        {
            if (_document == IntPtr.Zero || pageIndex < 0 || pageIndex >= PageCount)
            {
                return null;
            }

            IntPtr page = FPDF_LoadPage(_document, pageIndex);
            if (page == IntPtr.Zero)
            {
                return null;
            }

            try
            {
                double width = FPDF_GetPageWidth(page);
                double height = FPDF_GetPageHeight(page);

                int bitmapWidth = (int)(width * scale);
                int bitmapHeight = (int)(height * scale);

                IntPtr bitmap = FPDFBitmap_CreateEx(bitmapWidth, bitmapHeight, FPDF_BITMAP_BGRA, IntPtr.Zero, 0);
                if (bitmap == IntPtr.Zero)
                {
                    return null;
                }

                try
                {
                    FPDFBitmap_FillRect(bitmap, 0, 0, bitmapWidth, bitmapHeight, 0xFFFFFFFF);
                    FPDF_RenderPageBitmap(bitmap, page, 0, 0, bitmapWidth, bitmapHeight, 0, FPDF_ANNOT | FPDF_LCD_TEXT);

                    IntPtr buffer = FPDFBitmap_GetBuffer(bitmap);
                    int stride = FPDFBitmap_GetStride(bitmap);

                    using var skBitmap = new SKBitmap(bitmapWidth, bitmapHeight, SKColorType.Bgra8888, SKAlphaType.Premul);

                    unsafe
                    {
                        byte* src = (byte*)buffer;
                        byte* dst = (byte*)skBitmap.GetPixels();

                        for (int y = 0; y < bitmapHeight; y++)
                        {
                            Buffer.MemoryCopy(src + y * stride, dst + y * skBitmap.RowBytes, skBitmap.RowBytes, bitmapWidth * 4);
                        }
                    }

                    using var image = SKImage.FromBitmap(skBitmap);
                    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                    return data.ToArray();
                }
                finally
                {
                    FPDFBitmap_Destroy(bitmap);
                }
            }
            finally
            {
                FPDF_ClosePage(page);
            }
        });
    }

    public Task<(int Width, int Height)> GetPageSizeAsync(int pageIndex)
    {
        return Task.Run(() =>
        {
            if (_document == IntPtr.Zero || pageIndex < 0 || pageIndex >= PageCount)
            {
                return (0, 0);
            }

            IntPtr page = FPDF_LoadPage(_document, pageIndex);
            if (page == IntPtr.Zero)
            {
                return (0, 0);
            }

            try
            {
                double width = FPDF_GetPageWidth(page);
                double height = FPDF_GetPageHeight(page);
                return ((int)width, (int)height);
            }
            finally
            {
                FPDF_ClosePage(page);
            }
        });
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_document != IntPtr.Zero)
            {
                FPDF_CloseDocument(_document);
                _document = IntPtr.Zero;
            }
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    ~PdfiumRenderer()
    {
        Dispose();
    }
}
