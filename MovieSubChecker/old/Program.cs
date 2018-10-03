using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace old.MovieDiskControl
{
	class Program
	{
		static int totaalFilms = 0;
		static string[] error = new string[250];
		static int errors = 0;

		[STAThread]
		static void Main(string[] args)
		{
			string nog;

			string fileName;
			FolderBrowserDialog fd = new FolderBrowserDialog();
			if (fd.ShowDialog() != DialogResult.OK)
			{
				Console.WriteLine("Geen path opgegevens, programma sluit af");
				Console.ReadKey();
				return;
			}


			fileName = fd.SelectedPath;
			Console.WriteLine("Path: " + fileName);
			Console.WriteLine();

			var dir = Directory.GetDirectories(fileName);

			Console.WriteLine("Welke mappen moeten worden vermeden?");

			foreach (string inf in dir)
			{
				DirectoryInfo dirInf = new DirectoryInfo(inf);
				Console.WriteLine("\t" + dirInf.Name);
			}
			Console.WriteLine();

			Console.WriteLine("Mappen die beginnen met: ");
			string uitzondering = Console.ReadLine();


			do
			{
				totaalFilms = 0;
				error = new string[250];
				errors = 0;

				foreach (var map in dir)
				{
					DirectoryInfo mapInfo = new DirectoryInfo(map);
					if (mapInfo.Name.ToString().Substring(0, uitzondering.Length).Equals(uitzondering) == false)
					{
						Console.WriteLine(map.ToString() + ":");
						foreach (var film in Directory.GetDirectories(map))
						{
							if (Directory.GetDirectories(film).Length == 0) //Dit is geen film met sequels
							{
								Console.WriteLine("\t" + film.ToString());
								printFilm(film);
							}
							else
							{
								foreach (var seq in Directory.GetDirectories(film))
								{
									Console.WriteLine("\t" + seq.ToString() + ":");
									printFilm(seq);
								}

								int aantalSeq = Directory.GetDirectories(film).Length;
								if (film.Contains("(" + aantalSeq + ")") == false)
								{
									string newNaam = "";
									string teken = "";
									DirectoryInfo dirInfo = new DirectoryInfo(film);
									for (int i = 0; i < dirInfo.Name.Length; i++)
									{
										teken = dirInfo.Name.Substring(i, 1);
										if (teken == "(")
										{
											i = dirInfo.Name.Length;
										}else
										{
											newNaam += teken;
										}
									}

									newNaam += "(" + aantalSeq + ")";
									string path = dirInfo.FullName.Replace(dirInfo.Name, "") + "\\" + newNaam;
									Directory.Move(film, path);

									newError(path, "Opgelost - Er was iets mis met het aantal sequels.");
								}
							}
						}
					}
				}
				Console.WriteLine("Totaal aantal: " + totaalFilms + " films");
				Console.WriteLine();

				for (int i = 0; i < errors; i++)
				{
					Console.WriteLine(error[i]);
				}
				Console.WriteLine("Aantal errors: " + errors);

				Directory.CreateDirectory(fileName + "/" + uitzondering + "results/");

				File.WriteAllLines(fileName + "/" + uitzondering + "results/errors.txt", error);

				Console.WriteLine("Typ 1 als het programma opnieuw moet runnen in zelfde dir");
				nog = Console.ReadLine().ToString();
			} while (nog.Contains("1") == true);
		}

		private static void printFilm(string path)
		{
			totaalFilms++;

			DirectoryInfo dirInfo = new DirectoryInfo(path);
			string dirNaam = dirInfo.Name;

			int size = 0;
			var tussen = Directory.GetFiles(path);
			FileInfo[] files = new FileInfo[size];

			foreach (var file in tussen)
			{
				FileInfo info = new FileInfo(file);
				if (!(info.Attributes.HasFlag(FileAttributes.Hidden)))
				{
					Array.Resize(ref files, ++size);
					files[size - 1] = info;
				}
			}

			if (files.Length != 2)
			{
				newError(path.ToString(), "Meer of minder dan 2 bestanden in de map");
			}

			if (!(System.Text.RegularExpressions.Regex.IsMatch(dirNaam, @"\([0-9]{4}\)$")))
			{
				newError(path, "Er klopt iets niet met de benaming van de map");
			}

			foreach (var file in files)
			{
				try
				{
					//Veel te lang om elke film te kopiëren...
					string newNaam = path + "\\" + dirNaam + file.Extension;

					if (file.Name != dirNaam + file.Extension)
					{
						File.Move(file.ToString(), newNaam);
					}
					Console.WriteLine("\t\t" + newNaam.ToString());
				}
				catch
				{
					newError(file.ToString(), "Er ging iets mis tijdens hernoemen of verwijderen van bestand");
				}
			}
		}

		private static void newError(string path, string description)
		{
			error[errors] = String.Format("{0}: {1}", path, description);
			errors++;
		}
	}
}
