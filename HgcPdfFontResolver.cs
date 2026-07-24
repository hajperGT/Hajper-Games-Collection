using PdfSharp.Fonts;
using System.IO;

namespace HGC
{
    internal class HgcPdfFontResolver : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            string fontsFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.Fonts);

            string fontFile = faceName switch
            {
                "SegoeUI#Regular" => "segoeui.ttf",
                "SegoeUI#Bold" => "segoeuib.ttf",
                _ => "segoeui.ttf"
            };

            string fontPath = Path.Combine(fontsFolder, fontFile);

            return File.ReadAllBytes(fontPath);
        }

        public FontResolverInfo ResolveTypeface(
            string familyName,
            bool isBold,
            bool isItalic)
        {
            if (isBold)
                return new FontResolverInfo("SegoeUI#Bold");

            return new FontResolverInfo("SegoeUI#Regular");
        }
    }
}