using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZTF2_Féléves_NGSKJ6
{
    
        public enum TYPE
        {
            OPERACIOS_RENDSZER,
            TABLAZATKEZELO,
            LEMEZKEZELO,
            SZOVEGSZERKESZTO,
            ANTIVIRUS,
            EGYEB
        }

        public delegate void LefagyottEsemenyHandler(ISzoftver szoftver);
        public interface ISzoftver
        {
            event LefagyottEsemenyHandler LefagyottEsemeny;
            string Nev { get; }
            int Ar { get; }
            int Memoriaigeny { get; }
            TYPE Tipus { get; }
            void Telepit(Szamitogep szamitogep);
            void Lefagy();
        }
        public abstract class Szoftver : ISzoftver
        {
            public event LefagyottEsemenyHandler LefagyottEsemeny;
            public abstract string Nev { get; }
            public abstract int Ar { get; }
            public abstract int Memoriaigeny { get; }
            public abstract TYPE Tipus { get; }

            public virtual void Telepit(Szamitogep szamitogep)
            {
                if (szamitogep == null)
                {
                    throw new ArgumentNullException(nameof(szamitogep));
                }

                szamitogep.Szoftverek.Add(this);
                LefagyottEsemeny += szamitogep.SzoftverLefagyott;
            }


            public virtual void Lefagy()
            {
                Console.WriteLine($"{Nev} lefagyott.");
                OnLefagyottEsemeny();
            }
            protected void OnLefagyottEsemeny()
            {
                LefagyottEsemeny?.Invoke(this);
            }
        }

        public class Windows : Szoftver
        {
            public override string Nev => "Win7";
            public override int Ar => 45000;
            public override int Memoriaigeny => 1024;
            public override TYPE Tipus => TYPE.OPERACIOS_RENDSZER;

            public override void Lefagy()
            {
                Console.WriteLine("Kékhalál");
                OnLefagyottEsemeny();
            }
        }

        public abstract class Office : Szoftver
        {
            protected Szamitogep? host = null;

            public override void Lefagy()
            {
                base.Lefagy();

                if (host != null)
                {
                    //Megkeressük a host számítógépen az operációs rendszer típusúszoftvert, és tovább küldjük a lefagyást
                    ISzoftver opRendszer = host.Szoftverek.Find(s => s.Tipus == TYPE.OPERACIOS_RENDSZER);
                    if (opRendszer != null)
                    {
                        opRendszer.Lefagy();
                    }
                }

            }
            public override void Telepit(Szamitogep szamitogep)
            {
                base.Telepit(szamitogep);
                host = szamitogep;
            }

        }

        public class Excel : Office
        {
            public override string Nev => "Excel";
            public override int Ar => 20000;
            public override int Memoriaigeny => 512;
            public override TYPE Tipus => TYPE.TABLAZATKEZELO;

        }

        public class Linux : Szoftver
        {
            public override string Nev => "Ubuntu";
            public override int Ar => 0;
            public override int Memoriaigeny => 2048;
            public override TYPE Tipus => TYPE.OPERACIOS_RENDSZER;
        }

        public class GParted : Szoftver
        {
            public override string Nev => "GParted";
            public override int Ar => 0;
            public override int Memoriaigeny => 1024;
            public override TYPE Tipus => TYPE.LEMEZKEZELO;

        }

        public class Word : Office
        {
            public override string Nev => "Word";
            public override int Ar => 15000;
            public override int Memoriaigeny => 512;
            public override TYPE Tipus => TYPE.SZOVEGSZERKESZTO;

        }

        public class NotepadPlusPLus : Szoftver
        {
            public override string Nev => "Notepad++";
            public override int Ar => 0;
            public override int Memoriaigeny => 512;
            public override TYPE Tipus => TYPE.SZOVEGSZERKESZTO;

        }

        public class Eset : Szoftver
        {
            public override string Nev => "Eset";
            public override int Ar => 10000;
            public override int Memoriaigeny => 256;
            public override TYPE Tipus => TYPE.ANTIVIRUS;

        }
    
}
