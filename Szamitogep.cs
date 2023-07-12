using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZTF2_Féléves_NGSKJ6
{
    public class Szamitogep
    {
        public List<ISzoftver> Szoftverek { get; private set; }
        public int Ar => Szoftverek.Sum(s => s.Ar);
        public int Memoriaigeny => Szoftverek.Sum(s => s.Memoriaigeny);

        public Szamitogep()
        {
            Szoftverek = new List<ISzoftver>();
        }
        public Szamitogep(Szamitogep masikSzamitogep)
        {
            //Másoló ctor
            this.Szoftverek = new List<ISzoftver>(masikSzamitogep.Szoftverek);
        }

        public void SzoftverLefagyott(ISzoftver szoftver)
        {
            Console.WriteLine($"A {szoftver.Nev} lefagyott.");
        }

        public static Szamitogep operator +(Szamitogep sz, ISzoftver szoftver)
        {
            szoftver.Telepit(sz);
            return sz;
        }
        public override string ToString()
        {
            return string.Join(", ", Szoftverek.Select(s => s.Nev));
        }
    }
}
