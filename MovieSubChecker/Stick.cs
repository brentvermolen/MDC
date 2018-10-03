using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MovieDiskControl
{
	class Stick
	{
		public string Path { get; private set; }
		public string Uitzondering { get; private set; }

		public IList<Film> Films { get; private set; }
		public IList<Film> Sequels { get; private set; }

        public IList<string> Errors { get; private set; }

		public Stick(string path, string uitzondering = "")
		{
			this.Path = path;
			this.Uitzondering = uitzondering;

			Films = new List<Film>();
			Sequels = new List<Film>();
            Errors = new List<string>();

			if (uitzondering != "")
			{
				LookThroughStick();
			}
		}

		private void LookThroughStick()
		{
			foreach (DirectoryInfo dir in GetMovieFolders())
			{
				foreach (DirectoryInfo film in dir.GetDirectories())
				{
					if (film.GetDirectories().Length == 0) //Heeft geen sequels --> is gewoon de film
					{
						string naam = Toolbox.GetTitel(film.Name);
						int jaar = Toolbox.GetJaartal(film.Name);
                        FileInfo[] info = film.GetFiles();

                        if (info[0].Length > 2000000000)
                        {
                            decimal lengte = Math.Round(Decimal.Parse(info[0].Length.ToString()) / 1000000000, 2);
                            Errors.Add(film.ToString() + ": Bestand is groter dan 2GB\t(" + lengte + " Gb)");
                        }

                        if (info.Length != 2)
                        {
                            if (info.Length > 2)
                            {
                                Errors.Add(film.ToString() + ": Meer dan 2 bestanden in map.");
                            }
                            else
                            {
                                if (info.Length == 0)
                                {
                                    Errors.Add(film.ToString() + ": Geen bestanden in map.");
                                }
                                else
                                {
                                    if (info[0].Extension.ToLower().ToLower() == ".srt")
                                    {
                                        Errors.Add(film.ToString() + ": Geen video bestand in map.");
                                    }
                                    else
                                    {
                                        Errors.Add(film.ToString() + ": Geen ondertiteling in map.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            var movieFile = info.FirstOrDefault(f => f.Extension.ToLower() != ".srt");

                            if (movieFile != null)
                            {
                                if (movieFile.Extension.ToLower() != ".mkv")
                                {
                                    Errors.Add(film.ToString() + ": Videobestand is niet van formaat .mkv");
                                }
                            }
                        }

                        Films.Add(new Film(naam, jaar, film.FullName, info[0].Length));
					}
					else
					{
                        Film mov = new Film(Toolbox.GetTitel(film.Name), 0, film.FullName);
						mov.HeeftSequel = true;

						foreach (DirectoryInfo sequel in film.GetDirectories())
                        {
                            FileInfo[] info = sequel.GetFiles();

                            if (info[0].Length > 2000000000)
                            {
                                decimal lengte = Math.Round(Decimal.Parse(info[0].Length.ToString()) / 1000000000, 2);
                                Errors.Add(sequel.ToString() + ": Bestand is groter dan 2GB\t(" + lengte + " Gb)");
                            }

                            if (info.Length != 2)
                            {
                                if (info.Length > 2)
                                {
                                    Errors.Add(mov.Titel + " - " + sequel.ToString() + ": Meer dan 2 bestanden in map.");
                                }
                                else
                                {
                                    if (info.Length == 0)
                                    {
                                        Errors.Add(mov.Titel + " - " + sequel.ToString() + ": Geen bestanden in map.");
                                    }
                                    else
                                    {
                                        if (info[0].Extension.ToLower() == ".srt")
                                        {
                                            Errors.Add(mov.Titel + " - " + sequel.ToString() + ": Geen video bestand in map.");
                                        }
                                        else
                                        {
                                            Errors.Add(mov.Titel + " - " + sequel.ToString() + ": Geen ondertiteling in map.");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var movieFile = info.FirstOrDefault(f => f.Extension.ToLower() != ".srt");

                                if (movieFile != null)
                                {
                                    if (movieFile.Extension.ToLower() != ".mkv")
                                    {
                                        Errors.Add(mov.Titel + " - " + sequel.ToString() + ": Videobestand is niet van formaat .mkv");
                                    }
                                }
                            }

                            Film seq = new Film(Toolbox.GetTitel(film.Name) + " - " + Toolbox.GetTitel(sequel.Name), Toolbox.GetJaartal(sequel.Name), sequel.FullName, info[0].Length);
							mov.VoegSequelToe(seq);
						}

                        mov.Jaar = mov.Sequels.ElementAt(0).Jaar;
                        mov.size = mov.Sequels.Sum(m => m.size);

						Sequels.Add(mov);
                        Films.Add(mov);
					}
				}
			}
		}

		public DirectoryInfo GetStick()
		{
			return new DirectoryInfo(Path);
		}

		public void SetUitzondering(string uitzondering)
		{
			Uitzondering = uitzondering;
			LookThroughStick();
		}

		public IList<DirectoryInfo> GetMovieFolders()
		{
			IList<DirectoryInfo> lijst = new List<DirectoryInfo>();
			foreach (DirectoryInfo dir in GetStick().GetDirectories())
			{
				if (dir.Name.Substring(0, Uitzondering.Length) != Uitzondering)
				{
					lijst.Add(dir);
				}
			}

			return lijst;
		}

		public IList<DirectoryInfo> GetAllFolders()
		{
			return GetStick().GetDirectories();
		}

		/*public IList<Film> GetAllFilms()
		{
			return Films;
		}

		public IList<Film> GetAllSequels()
		{
			return Sequels;
		}*/

		public void PrintFilmsToFile(string filename = "films")
		{
			string[] strings = new string[Films.Count];

			for (int i = 0; i < Films.Count; i++)
			{
				strings[i] = Films.ElementAt(i).ToString();
			}

			Directory.CreateDirectory(Path + "\\" + Uitzondering + "files");
			File.WriteAllLines(Path + "\\" + Uitzondering + "files\\" + filename + ".txt", strings);
        }

        public void PrintErrorsToFile(string filename = "errors")
        {
            Directory.CreateDirectory(Path + "\\" + Uitzondering + "files");
            File.WriteAllLines(Path + "\\" + Uitzondering + "files\\" + filename + ".txt", Errors);
        }

        public override string ToString()
		{
			return Path;
		}
	}
}
