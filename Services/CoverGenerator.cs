using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
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

            int style = seed % 4;

            image.Mutate(ctx =>
            {
                switch (style)
                {
                    case 0:
                        DrawGradientCircles(ctx, random, width, height);
                        break;
                    case 1:
                        DrawLinesStyle(ctx, random, width, height);
                        break;
                    case 2:
                        DrawNoiseStyle(ctx, random, width, height);
                        break;
                    case 3:
                        DrawPosterStyle(ctx, random, width, height, seed); 
                        break;
                }

                // shadow
                ctx.Fill(new LinearGradientBrush(
                    new PointF(0, height * 0.6f),
                    new PointF(0, height),
                    GradientRepetitionMode.None,
                    new ColorStop(0, Color.Transparent),
                    new ColorStop(1, Color.Black.WithAlpha(70))
                ));
            });

            var fontBig = SystemFonts.CreateFont("Arial", 40, FontStyle.Bold);
            var fontSmall = SystemFonts.CreateFont("Arial", 20, FontStyle.Bold);

            image.Mutate(ctx =>
            {
                // up
                ctx.DrawText(title.ToUpper(), fontBig, Color.Black, new PointF(10, 10));

                // down
                ctx.DrawText(artist.ToUpper(), fontSmall, Color.Black, new PointF(10, height - 60));
            });

            using var ms = new MemoryStream();
            image.SaveAsPng(ms);

            return ms.ToArray();
        }
        private void DrawGradientCircles(IImageProcessingContext ctx, Random random, int w, int h)
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
        }
        private void DrawLinesStyle(IImageProcessingContext ctx, Random random, int w, int h)
        {
            ctx.Fill(RandomColor(random));

            for (int i = 0; i < 20; i++)
            {
                var pen = Pens.Solid(RandomColor(random), random.Next(1, 4));

                ctx.DrawLine(
                    pen,
                    new PointF(random.Next(w), random.Next(h)),
                    new PointF(random.Next(w), random.Next(h))
                );
            }
        }
        private void DrawNoiseStyle(IImageProcessingContext ctx, Random random, int w, int h)
        {
            ctx.Fill(RandomColor(random));

            for (int i = 0; i < 500; i++)
            {
                ctx.Fill(
                    RandomColor(random, 80),
                    new EllipsePolygon(random.Next(w), random.Next(h), 2)
                );
            }

            ctx.GaussianBlur(10); // 🔥 blur = AI feel
        }
        private Color RandomColor(Random random, byte alpha = 255)
        {
            return new Rgba32(
                (byte)random.Next(256),
                (byte)random.Next(256),
                (byte)random.Next(256),
                alpha
            );
        }
        private void DrawPosterStyle(IImageProcessingContext ctx, Random random, int w, int h, int seed)
        {
            var bg = RandomBrightColor(random);
            ctx.Fill(bg);

            var centerX = w / 2;
            var centerY = h / 2;

            var burstColor = RandomBrightColor(random);

            var points = new List<PointF>();

            int spikes = random.Next(8, 16);
            float outer = random.Next(80, 120);
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

            int objectType = seed % 5;

            switch (objectType)
            {
                case 0: DrawMic(ctx, centerX, centerY); break;
                case 1: DrawNote(ctx, centerX, centerY); break;
                case 2: DrawSpeaker(ctx, centerX, centerY); break;
                case 3: DrawDisc(ctx, centerX, centerY); break;
                case 4: DrawGuitar(ctx, centerX, centerY); break;
            }
        }
        private void DrawMic(IImageProcessingContext ctx, float x, float y)
        {
            ctx.Fill(Color.Black, new EllipsePolygon(x, y - 30, 20));
            ctx.Fill(Color.Black, new RectangularPolygon(x - 5, y - 30, 10, 80));
        }
        private void DrawNote(IImageProcessingContext ctx, float x, float y)
        {
            ctx.Fill(Color.Black, new EllipsePolygon(x, y, 20));
            ctx.Fill(Color.Black, new RectangularPolygon(x + 10, y - 80, 8, 80));
        }
        private void DrawSpeaker(IImageProcessingContext ctx, float x, float y)
        {
            ctx.Fill(Color.Black, new RectangularPolygon(x - 40, y - 60, 80, 120));

            ctx.Fill(Color.Gray, new EllipsePolygon(x, y - 20, 20));
            ctx.Fill(Color.DarkGray, new EllipsePolygon(x, y + 30, 25));
        }
        private void DrawDisc(IImageProcessingContext ctx, float x, float y)
        {
            ctx.Fill(Color.Black, new EllipsePolygon(x, y, 60));
            ctx.Fill(Color.Gray, new EllipsePolygon(x, y, 15));
        }
        private void DrawGuitar(IImageProcessingContext ctx, float x, float y)
        {
            ctx.Fill(Color.Black, new EllipsePolygon(x, y, 30));
            ctx.Fill(Color.Black, new RectangularPolygon(x - 5, y - 80, 10, 80));
        }

        private Color RandomBrightColor(Random random)
        {
            return new Rgba32(
                (byte)random.Next(100, 255),
                (byte)random.Next(100, 255),
                (byte)random.Next(100, 255)
            );
        }
    }
}
