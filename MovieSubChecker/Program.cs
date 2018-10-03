using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Threading;


namespace MovieDiskControl
{
	class Program
	{
		[STAThread]
		static void Main()
		{
            Console.BufferHeight = Int16.MaxValue - 1;

            Console.WriteLine("Selecteer de map die de films bevat:");
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			if (fbd.ShowDialog() != DialogResult.OK)
			{
				Console.WriteLine("Geen map gekozen, programma wordt afgesloten.");
				Console.ReadKey();
				return;
			}

			Console.Clear();

			Stick stick = new Stick(fbd.SelectedPath);
			Console.WriteLine("In welke mappen zitten geen films?");
			Toolbox.PrintList(stick.GetAllFolders(), '\t');
			Console.Write("Mappen die beginnen met: ");
			stick.SetUitzondering(Console.ReadLine());

            int code = -1;

            while (code != 0)
            {
                do
                {
                    Console.Clear();

                    Console.WriteLine("Kies wat je wil doen:");
                    Console.WriteLine("1) Toon alle films");
                    Console.WriteLine("2) Toon alle film mappen");
                    Console.WriteLine("3) Toon films die ... in naam hebben");
                    Console.WriteLine("4) Toon een random film om te kijken");
                    Console.WriteLine("5) Toon resultaten");
                    Console.WriteLine("6) Schrijf resultaten naar bestand");
                    Console.WriteLine("7) Verander mappen die films bevatten");
                    Console.WriteLine("8) Export alle films naar XML");
                    Console.WriteLine("0) Afsluiten");

                    try
                    {
                        code = int.Parse(Console.ReadLine().ToString());
                    }
                    catch
                    {
                        code = -1;
                    }

                } while (code < 0 || code > 8);

                switch (code)
                {
                    case 0:
                        break;
                    case 1:
                        Toolbox.PrintList(stick.Films, charVoorString: '\t');
                        break;
                    case 2:
                        Toolbox.PrintList(stick.GetMovieFolders());
                        break;
                    case 3:
                        Console.WriteLine("Film moet ... bevatten:");
                        string par = Console.ReadLine();
                        Toolbox.PrintList(stick.Films, contains: par);
                        break;
                    case 4:
                        Random rand = new Random();
                        int film = rand.Next(0, stick.Films.Count);
                        Console.WriteLine("Gekozen film:");
                        Console.WriteLine("\t" + stick.Films[film].Titel + " - " + stick.Films[film].Jaar);
                        break;
                    case 5:
                        Toolbox.PrintList(stick.Errors);
                        break;
                    case 6:
                        stick.PrintFilmsToFile();
                        stick.PrintErrorsToFile();
                        break;
                    case 7:
                        stick = new Stick(fbd.SelectedPath);
                        Console.WriteLine("In welke mappen zitten geen films?");
                        Toolbox.PrintList(stick.GetAllFolders(), '\t');
                        Console.Write("Mappen die beginnen met: ");
                        stick.SetUitzondering(Console.ReadLine());

                        Console.Clear();
                        break;
                    case 8:
                        Toolbox.ExportMoviesToXML("films.xml", stick);
                        break;
                }

               
                if (code != 0)
                {
                    Console.WriteLine("Enter om door te gaan");
                    Console.Read();
                }
            }
		}
    }
}
