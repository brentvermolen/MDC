using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MovieDiskControl
{
	class Toolbox
	{
		public static string GetTitel(string mapNaam)
		{
			try
			{
				string teken = "";
				int lengte = mapNaam.Length;
				string naam = "";

				for (int i = 0; i < lengte - 1; i++)
				{
					teken = mapNaam.Substring(i, 1);
					if (teken == "(")
					{
						return naam.Substring(0, naam.Length - 1); //Laatste spatie weghalen
					}
					else
					{
						naam += teken;
					}
				}

				return naam;
			}
			catch
			{
				Console.WriteLine("Error: " + mapNaam + " - Er ging iets mis tijdens zoeken van naam van de film");
				return mapNaam;
			}
		}

		public static int GetJaartal(string mapNaam)
		{
			try
			{
				string jaartal = "";
				string naam = mapNaam;
				while (Regex.IsMatch(naam, @"^\d{4}\)$") == false)
				{
					naam = naam.Substring(1, naam.Length - 1);
				}

				int jaar = Int32.Parse(naam.Substring(0, 4));

				return jaar;
			}
			catch
			{
				Console.WriteLine("Error: " + mapNaam + " - Er ging iets mis tijdens zoeken van jaartal van de film");
				return 0;
			}
		}

		public static void PrintList(IList<DirectoryInfo> list, char charVoorString = '\0')
		{
			Console.WriteLine("Mappen:");
			foreach (var loop in list)
			{
				Console.WriteLine(charVoorString + loop.Name);
			}
		}

		public static void PrintList(IList<Film> list, string titel = "Films", char charVoorString = '\0', string contains = "")
		{
			Console.WriteLine(titel + ":");
			foreach (var loop in list)
			{
				if (loop.ToString().ToLower().Contains(contains.ToLower()))
                {
                    Console.WriteLine(charVoorString + loop.ToString());
                }
			}
        }

        public static void PrintList(IList<string> list, string titel = "Errors", char charVoorString = '\0', string contains = "")
        {
            Console.WriteLine(titel + ":");
            foreach (var loop in list)
            {
                if (loop.ToString().ToLower().Contains(contains.ToLower()))
                {
                    Console.WriteLine(charVoorString + loop.ToString());
                }
            }
        }

        public static void PrintList(IList<FileInfo> list, string titel = "Bestanden", char charVoorString = '\0')
		{
			Console.WriteLine(titel + ":");
			foreach (var loop in list)
			{
				Console.WriteLine(charVoorString + loop.Name);
			}
		}

		public static void PrintList(string[] list, char charVoorString = '\0')
		{
			foreach (var loop in list)
			{
				Console.WriteLine(charVoorString + loop.ToString());
			}
		}

        internal static void ExportMoviesToXML(string filename, Stick stick)
        {
            using (XmlWriter writer = XmlWriter.Create(filename))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Movies");

                var movies = stick.Films;

                List<Film> allInXml = new List<Film>();

                foreach (var movie in movies)
                {
                    if (!allInXml.Contains(movie))
                    {
                        allInXml.Add(movie);
                        writer.WriteStartElement("Movie");

                        if (movie.HeeftSequel)
                        {
                            allInXml.Add(movie.Sequels.First());

                            writer.WriteElementString("Titel", movie.Sequels.First().Titel);
                            writer.WriteElementString("ReleaseYear", movie.Sequels.First().Jaar.ToString());
                            writer.WriteElementString("Size", movie.Sequels.First().size.ToString());
                            writer.WriteElementString("HasSequel", movie.HeeftSequel.ToString());

                            writer.WriteStartElement("Sequels");
                            foreach (var m in movie.Sequels)
                            {
                                if (!allInXml.Contains(m))
                                {
                                    allInXml.Add(m);

                                    writer.WriteStartElement("Sequel");

                                    writer.WriteElementString("Titel", m.Titel);
                                    writer.WriteElementString("ReleaseYear", m.Jaar.ToString());
                                    writer.WriteElementString("Size", m.size.ToString());

                                    writer.WriteEndElement();
                                }
                            }
                            writer.WriteEndElement();
                        }
                        else
                        {
                            writer.WriteElementString("Titel", movie.Titel);
                            writer.WriteElementString("ReleaseYear", movie.Jaar.ToString());
                            writer.WriteElementString("Size", movie.size.ToString());
                            writer.WriteElementString("HasSequel", movie.HeeftSequel.ToString());
                        }
                        //writer.WriteElementString("Salary", employee.Salary.ToString());
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();               
            }
        }
    }
}
