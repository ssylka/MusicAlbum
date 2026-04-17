using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;

namespace MusicAlbums.Services
{
    public class CoverGenerator
    {
        public byte[] Generate(string title, string artist, int seed)
        {
            title ??= "Unknown Title";
            artist ??= "Unknown Artist";

            var random = new Random(seed);

            int width = 300;
            int height = 300;

            using var image = new Image<Rgba32>(width, height);

            int style = Math.Abs(seed % 4);

            Color background = Color.White;

            image.Mutate(ctx =>
            {
                switch (style)
                {
                    case 0:
                        background = DrawGradientCircles(ctx, random, width, height);
                        break;
                    case 1:
                        background = DrawLinesStyle(ctx, random, width, height);
                        break;
                    case 2:
                        background = DrawNoiseStyle(ctx, random, width, height);
                        break;
                    case 3:
                        background = DrawPosterStyle(ctx, random, width, height, seed);
                        break;
                }

                ctx.Fill(new LinearGradientBrush(
                    new PointF(0, height * 0.65f),
                    new PointF(0, height),
                    GradientRepetitionMode.None,
                    new ColorStop(0, Color.Transparent),
                    new ColorStop(1, Color.Black.WithAlpha(80))
                ));
            });

            var fontBig = SystemFonts.CreateFont("Arial", 36, FontStyle.Bold);
            var fontSmall = SystemFonts.CreateFont("Arial", 18, FontStyle.Bold);

            var textColor = GetContrastColor(background);

            image.Mutate(ctx =>
            {
                var fontCollection = new FontCollection();
                var fontFamily = SystemFonts.Families.First();

                var textColor = GetContrastColor(background);
                var strokeColor = textColor == Color.White ? Color.Black : Color.White;

                DrawFittedText(
                    ctx,
                    title.ToUpper(),
                    new RectangleF(10, 10, width - 20, 80),
                    fontFamily,
                    40,
                    16,
                    textColor,
                    strokeColor
                );

                DrawFittedText(
                    ctx,
                    artist.ToUpper(),
                    new RectangleF(10, height - 70, width - 20, 60),
                    fontFamily,
                    28,
                    14,
                    textColor,
                    strokeColor
                ); ;
            });

            using var ms = new MemoryStream();
            image.SaveAsPng(ms);

            return ms.ToArray();
        }
        private void DrawFittedText(IImageProcessingContext ctx, string text, RectangleF area, FontFamily fontFamily, float maxFontSize, float minFontSize, Color fillColor, Color strokeColor)
        {
            Font font = null;

            for (float size = maxFontSize; size >= minFontSize; size -= 2)
            {
                var testFont = fontFamily.CreateFont(size, FontStyle.Bold);

                var options = new TextOptions(testFont)
                {
                    WrappingLength = area.Width,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var sizeF = TextMeasurer.MeasureSize(text, options);

                if (sizeF.Height <= area.Height)
                {
                    font = testFont;
                    break;
                }
            }

            font ??= fontFamily.CreateFont(minFontSize, FontStyle.Bold);

            var drawOptions = new RichTextOptions(font)
            {
                Origin = new PointF(area.X + area.Width / 2, area.Y),
                HorizontalAlignment = HorizontalAlignment.Center,
                WrappingLength = area.Width
            };

            var glyphs = TextBuilder.GenerateGlyphs(text, drawOptions);

            ctx.Draw(Pens.Solid(strokeColor, 3), glyphs);

            ctx.Fill(fillColor, glyphs);
        }

        private Color DrawGradientCircles(IImageProcessingContext ctx, Random random, int w, int h)
        {
            var c1 = RandomColor(random);
            var c2 = RandomColor(random);

            ctx.Fill(new LinearGradientBrush(
                new PointF(0, 0),
                new PointF(w, h),
                GradientRepetitionMode.None,
                new ColorStop(0, c1),
                new ColorStop(1, c2)
            ));

            for (int i = 0; i < 15; i++)
            {
                ctx.Fill(
                    RandomColor(random, 120),
                    new EllipsePolygon(random.Next(w), random.Next(h), random.Next(20, 80))
                );
            }

            return c1;
        }

        private Color DrawLinesStyle(IImageProcessingContext ctx, Random random, int w, int h)
        {
            var bg = RandomColor(random);
            ctx.Fill(bg);

            for (int i = 0; i < 20; i++)
            {
                var pen = Pens.Solid(RandomColor(random), random.Next(1, 3));

                ctx.DrawLine(
                    pen,
                    new PointF(random.Next(w), random.Next(h)),
                    new PointF(random.Next(w), random.Next(h))
                );
            }

            return bg;
        }

        private Color DrawNoiseStyle(IImageProcessingContext ctx, Random random, int w, int h)
        {
            var bg = RandomColor(random);
            ctx.Fill(bg);

            for (int i = 0; i < 400; i++)
            {
                ctx.Fill(
                    RandomColor(random, 80),
                    new EllipsePolygon(random.Next(w), random.Next(h), 2)
                );
            }

            ctx.GaussianBlur(8);

            return bg;
        }

        private Color DrawPosterStyle(IImageProcessingContext ctx, Random random, int w, int h, int seed)
        {
            var bg = RandomBrightColor(random);
            ctx.Fill(bg);

            var centerX = w / 2;
            var centerY = h / 2;

            var burstColor = RandomBrightColor(random);

            var points = new List<PointF>();

            int spikes = random.Next(8, 14);
            float outer = random.Next(70, 110);
            float inner = random.Next(30, 60);

            for (int i = 0; i < spikes * 2; i++)
            {
                float angle = i * MathF.PI / spikes;
                float radius = (i % 2 == 0) ? outer : inner;

                float x = centerX + radius * MathF.Cos(angle);
                float y = centerY + radius * MathF.Sin(angle);

                points.Add(new PointF(x, y));
            }

            ctx.Fill(burstColor, new Polygon(points.ToArray()));

            int objectType = Math.Abs(seed % 5);

            switch (objectType)
            {
                case 0: DrawMic(ctx, centerX, centerY); break;
                case 1: DrawNote(ctx, centerX, centerY); break;
                case 2: DrawSpeaker(ctx, centerX, centerY); break;
                case 3: DrawDisc(ctx, centerX, centerY); break;
                case 4: DrawGuitar(ctx, centerX, centerY); break;
            }

            return bg;
        }
        private void DrawMic(IImageProcessingContext ctx, float x, float y)
        {
            ctx.Fill(Color.Black, new EllipsePolygon(x, y - 30, 18));
            ctx.Fill(Color.Black, new RectangularPolygon(x - 4, y - 30, 8, 70));
        }

        private void DrawNote(IImageProcessingContext ctx, float x, float y)
        {
            ctx.Fill(Color.Black, new EllipsePolygon(x, y, 18));
            ctx.Fill(Color.Black, new RectangularPolygon(x + 10, y - 70, 6, 70));
        }

        private void DrawSpeaker(IImageProcessingContext ctx, float x, float y)
        {
            ctx.Fill(Color.Black, new RectangularPolygon(x - 35, y - 55, 70, 110));
            ctx.Fill(Color.Gray, new EllipsePolygon(x, y - 15, 18));
            ctx.Fill(Color.DarkGray, new EllipsePolygon(x, y + 25, 22));
        }

        private void DrawDisc(IImageProcessingContext ctx, float x, float y)
        {
            ctx.Fill(Color.Black, new EllipsePolygon(x, y, 55));
            ctx.Fill(Color.Gray, new EllipsePolygon(x, y, 12));
        }

        private void DrawGuitar(IImageProcessingContext ctx, float x, float y)
        {
            ctx.Fill(Color.Black, new EllipsePolygon(x, y, 25));
            ctx.Fill(Color.Black, new RectangularPolygon(x - 4, y - 70, 8, 70));
        }
        private Color RandomColor(Random random, byte alpha = 255)
        {
            return new Rgba32(
                (byte)random.Next(30, 230),
                (byte)random.Next(30, 230),
                (byte)random.Next(30, 230),
                alpha
            );
        }

        private Color RandomBrightColor(Random random)
        {
            return new Rgba32(
                (byte)random.Next(120, 255),
                (byte)random.Next(120, 255),
                (byte)random.Next(120, 255)
            );
        }

        private Color GetContrastColor(Color bg)
        {
            var rgba = bg.ToPixel<Rgba32>();

            var luminance = (0.299 * rgba.R + 0.587 * rgba.G + 0.114 * rgba.B) / 255;

            return luminance > 0.5 ? Color.Black : Color.White;
        }
    }
}