    using Bogus;
    using MusicAlbums.Models;

    namespace MusicAlbums.Services
    {
        public class SongGenerator
        {
            private Faker faker;
            private static readonly Dictionary<string, string> genreMap = new()
            {
                { "Rock", "Рок" },
                { "Jazz", "Джаз" },
                { "Hip Hop", "Хип-хоп" },
                { "Electronic", "Электронная" },
                { "Classical", "Классическая" },
                { "Pop", "Поп" },
                { "Blues", "Блюз" },
                { "Reggae", "Регги" },
                { "Metal", "Метал" },
                { "Folk", "Фолк" },
                { "Country", "Кантри" },
                { "Soul", "Соул" },
                { "Funk", "Фанк" },
                { "Latin", "Латинская" },
                { "World", "Мировая" },
                { "Rap", "Рэп" },
                { "Stage And Screen", "Саундтрек" },
                { "Non Music", "Другое" }
            };
            public List<Song> Generate(int page, int seed, double avgLikes, string location = "en_US")
            {
                faker = new Faker(location);
                Randomizer.Seed = new Random(seed * 3 + page);
                
                var songs = new List<Song>();

                for (int i = 0; i < 20; i++)
                {
                    int index = (page - 1) * 20 + i + 1;
                    string title = GenerateSongTitle(location);
                    string artist = faker.Name.FullName();
                    string album = GenerateAlbumTitle(location);
                    songs.Add(new Song
                    {
                        Index = index,
                        Title = title,
                        Artist = artist,
                        Album = album,
                        Genre = GenerateGenre(location), 
                        Likes = GenerateLikes(avgLikes),
                        CoverUrl = $"cover?title={Uri.EscapeDataString(album)}&artist={Uri.EscapeDataString(artist)}&seed={seed * 3 + index}",
                        MusicUrl = $"music?seed={seed * 3 + index}"
                    });
                }

                return songs;
            }
        private string GenerateGenre(string location)
        {
            var genre = faker.Music.Genre();

            if (location == "ru" && genreMap.ContainsKey(genre))
                return genreMap[genre];

            return genre;
        }

        private string GenerateDefaultSongTitle(string location)
            {
                int pattern = new Randomizer().Int(1, 5);

                return pattern switch
                {
                    1 => Cap(faker.Random.Word()),
                    2 => $"{Cap(faker.Random.Word())} {faker.Random.Word().ToLower()}",
                    3 => $"{Cap(faker.Random.Word())} of {faker.Random.Word().ToLower()}",
                    4 => $"{Cap(faker.Random.Word())} in the {faker.Random.Word().ToLower(  )}",
                    5 => $"{Cap(faker.Random.Word())} & {faker.Random.Word().ToLower(   )}",
                    _ => Cap(faker.Random.Word())
                };
            }
            private string GenerateDefaultAlbumTitle(string location)
            {
                int pattern = new Randomizer().Int(1, 5);

                return pattern switch
                {
                    1 => $"{Cap(faker.Random.Word())} {faker.Random.Word().ToLower()}",
                    2 => $"The {Cap(faker.Random.Word())}",
                    3 => "Single",
                    4 => $"{Cap(faker.Random.Word())} & {faker.Random.Word().ToLower()}",
                    _ => Cap(faker.Random.Word())
                };
            }
            private string GenerateAlbumTitle(string location)
            {
                if(location == "ru")
                { 
                    int pattern = new Randomizer().Int(1, 5);

                    return pattern switch
                    {
                        1 => $"{Cap(faker.Lorem.Word())} {faker.Lorem.Word().ToLower()}",
                        2 => $"{Cap(faker.Lorem.Word())}",
                        3 => "Сингл",
                        4 => $"{Cap(faker.Lorem.Word())} и {faker.Lorem.Word().ToLower()}",
                        _ => Cap(faker.Lorem.Word())
                    };
                }
                else
                {
                    return GenerateDefaultAlbumTitle(location);
                }
            }
            private string GenerateSongTitle(string location)
            {
                if (location == "ru")
                {
                    int pattern = new Randomizer().Int(1, 5);

                    return pattern switch
                    {
                        1 => Cap(faker.Lorem.Word()),
                        2 => $"{Cap(faker.Lorem.Word())} {faker.Lorem.Word().ToLower()}",
                        3 => $"{Cap(faker.Lorem.Word())} для {faker.Lorem.Word().ToLower()}",
                        4 => $"{Cap(faker.Lorem.Word())} в {faker.Lorem.Word().ToLower()}",
                        5 => $"{Cap(faker.Lorem.Word())} и {faker.Lorem.Word().ToLower()}",
                        _ => Cap(faker.Lorem.Word())
                    };
                }
                else
                {
                    return GenerateDefaultSongTitle(location);
                }
            }

            private string Cap(string word)
            {
                return char.ToUpper(word[0]) + word.Substring(1).ToLower();
            }
            private int GenerateLikes(double avg)
            {
                int floor = (int)Math.Floor(avg);
                double probability = avg - floor;

                return new Randomizer().Double() < probability ? floor + 1 : floor;
            }
        }
    }
