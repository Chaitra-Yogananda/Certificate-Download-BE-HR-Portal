using HrCertificatePortal.Api.Data;
using HrCertificatePortal.Api.Repositories.Interfaces;
using HrCertificatePortal.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.IO;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ImageSharpImage = SixLabors.ImageSharp.Image;
using ImageSharpColor = SixLabors.ImageSharp.Color;

namespace HrCertificatePortal.Api.Services.Implementations
{
    public class CertificateService : ICertificateService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public CertificateService(ICourseRepository courseRepository, IEmployeeRepository employeeRepository)
        {
            _courseRepository = courseRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<byte[]?> GenerateCertificateAsync(string courseCode, string email)
        {
            if (string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(email))
                return null;

            var course = await _courseRepository.GetByCodeAsync(courseCode);
            if (course == null || course.Template == null || course.Template.Length == 0)
                return null;

            var employee = await _employeeRepository.GetByEmailAndCourseAsync(email, course.Id);
            if (employee == null)
                return null;

            using var image = ImageSharpImage.Load<Rgba32>(course.Template);

            // Keep original template resolution for maximum clarity

            FontFamily family;
            var freestyleFontPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "FreestyleScript.ttf");
            if (System.IO.File.Exists(freestyleFontPath))
            {
                var collection = new FontCollection();
                family = collection.Add(freestyleFontPath);
            }
            else if (!SystemFonts.TryGet("Freestyle Script", out family) && !SystemFonts.TryGet("Brush Script MT", out family))
            {
                family = SystemFonts.Families.First();
            }

            var name = employee.EmployeeName.Trim();
            // Start slightly smaller and fit to the yellow line width
            float targetFontSize = (float)System.Math.Max(12f, image.Height * 0.145f); // ~14.5% of height to start
            var font = family.CreateFont(targetFontSize, FontStyle.Regular);
            float maxTextWidth = image.Width * 0.68f; 
            try
            {
                var measure = SixLabors.Fonts.TextMeasurer.MeasureSize(name, new SixLabors.Fonts.TextOptions(font));
                while (measure.Width > maxTextWidth && targetFontSize > 10f)
                {
                    targetFontSize -= 1f;
                    font = family.CreateFont(targetFontSize, FontStyle.Regular);
                    measure = SixLabors.Fonts.TextMeasurer.MeasureSize(name, new SixLabors.Fonts.TextOptions(font));
                }
                // Reduce a bit more after fit and apply a right shift
                targetFontSize = (float)System.Math.Max(10f, targetFontSize * 0.90f);
                font = family.CreateFont(targetFontSize, FontStyle.Regular);
                measure = SixLabors.Fonts.TextMeasurer.MeasureSize(name, new SixLabors.Fonts.TextOptions(font));
                var xShift = image.Width * 0.015f; // ~1.5% shift to the right
                // Place the text just BELOW the heading yellow line (with a small gap)
                var headingLineY = image.Height * 0.44f;
                var gap = image.Height * 0.0125f; // ~1.25% of height as margin below the line
                var options = new RichTextOptions(font)
                {
                    HorizontalAlignment = SixLabors.Fonts.HorizontalAlignment.Center,
                    VerticalAlignment = SixLabors.Fonts.VerticalAlignment.Top,
                    Origin = new PointF(image.Width / 2f + xShift, headingLineY + gap)
                };
                var nameColor = new Rgba32(0x32, 0x3d, 0x55, 0xFF); // #323d55

                image.Mutate(ctx =>
                {
                    ctx.DrawText(options, name, nameColor);
                });

                goto AfterTextDraw;
            }
            catch
            {
                // Fallback: shrink a bit based on name length if measurement API differs
                int extra = System.Math.Max(0, name.Length - 12);
                targetFontSize = System.Math.Max(12f, targetFontSize - extra * 0.8f);
                font = family.CreateFont(targetFontSize, FontStyle.Regular);
            }
            {
                // Fallback placement: place just BELOW the heading line (no measured height available)
                var headingLineY = image.Height * 0.44f;
                var gap = image.Height * 0.0125f;
                // Reduce a bit more and apply right shift
                targetFontSize = (float)System.Math.Max(10f, targetFontSize * 0.90f);
                font = family.CreateFont(targetFontSize, FontStyle.Regular);
                var xShift = image.Width * 0.015f;
                var options = new RichTextOptions(font)
                {
                    HorizontalAlignment = SixLabors.Fonts.HorizontalAlignment.Center,
                    VerticalAlignment = SixLabors.Fonts.VerticalAlignment.Top,
                    Origin = new PointF(image.Width / 2f + xShift, headingLineY + gap)
                };
                var nameColor = new Rgba32(0x32, 0x3d, 0x55, 0xFF);
                image.Mutate(ctx => ctx.DrawText(options, name, nameColor));
            }

            AfterTextDraw:

            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 100 });
            ms.Position = 0;

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(image.Width, image.Height);
                    page.Margin(0);
                    page.Content().Image(ms.ToArray());
                });
            }).GeneratePdf();

            return pdf;
        }
    }
}

