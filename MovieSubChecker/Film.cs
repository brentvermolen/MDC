using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDiskControl
{
	class Film
	{
		public static int Aantal;

		public string Titel { get; private set; }
		public int Jaar { get; set; }
		public string Locatie { get; private set; }
        public long size { get; set; }

		public bool HeeftSequel { get; set; }
		public IList<Film> Sequels { get; private set; }

		public Film(string titel, int jaar, string locatie, long size = 0)
		{
			this.Titel = titel;
			this.Jaar = jaar;
			this.Locatie = locatie;
            this.size = size;

			Sequels = new List<Film>();

			Aantal++;
		}

		public void VoegSequelToe(Film sequel)
		{
			Sequels.Add(sequel);
		}

		public void VoegSequelToe(string titel, int jaar, string locatie, long size)
		{
			Sequels.Add(new Film(titel, jaar, locatie, size));
		}

		public override string ToString()
		{
			if (HeeftSequel)
			{
				return Titel + " (" + Sequels.Count + ")";
			}
			else
			{
				return Titel + " (" + Jaar + ")"; 
			}
		}
	}
}
