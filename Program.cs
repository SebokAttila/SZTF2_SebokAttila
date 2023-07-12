using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZTF2_Féléves_NGSKJ6
{
    class Program
    {
        static void Main(string[] args)
        {
            //Alap konfiguráció, értékek betöltése
            KonfiguracioOsszeallito config = new KonfiguracioOsszeallito();

            var win7 = new Windows();
            var ubuntu = new Linux();
            var gparted = new GParted();
            var excel = new Excel();
            var word = new Word();
            var notepadPlusPlus = new NotepadPlusPLus();
            var eset = new Eset();

            config.SzoftverHozzaadas(win7, new List<ISzoftver> { excel, word, notepadPlusPlus, eset });
            config.SzoftverHozzaadas(ubuntu, new List<ISzoftver> { gparted, notepadPlusPlus });
            config.SzoftverHozzaadas(gparted, new List<ISzoftver> { ubuntu, notepadPlusPlus });
            config.SzoftverHozzaadas(excel, new List<ISzoftver> { win7, word, notepadPlusPlus, eset });
            config.SzoftverHozzaadas(word, new List<ISzoftver> { win7, excel, notepadPlusPlus, eset });
            config.SzoftverHozzaadas(notepadPlusPlus, new List<ISzoftver> { win7, ubuntu, excel, word, gparted, eset });
            config.SzoftverHozzaadas(eset, new List<ISzoftver> { win7, excel, word, notepadPlusPlus });

            //Menü loop
            while (true)
            {

                Console.WriteLine("Válassz a parancsok közül");
                Console.WriteLine("1. Szoftverek kiválasztása");
                Console.WriteLine("2. Auto konfig");
                Console.WriteLine("0. Kilépés");
                Console.Write("Válasszon egy lehetőséget: ");
                int opcio = int.Parse(Console.ReadLine());
                try
                {
                    switch (opcio)
                    {
                        case 1:
                            KonfiguracioOsszeallitasa(config);
                            break;
                        case 2:
                            AutoConfig(config);
                            break;
                        case 0:
                            Environment.Exit(0);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            ;
        }

        private static ISzoftver SelectAvailableSzoftver(Szamitogep sz, List<TYPE> tipusok, KonfiguracioOsszeallito config)
        {
            //Összes szoftver lekérése, ami kompatibilis a számítógéppel
            List<ISzoftver> elerheto = config.szoftverGraph.GetAllNodes()
                .Where(x => tipusok.Contains(x.Tipus) && config.KombatibilisE(sz, x))
                .ToList();
            //Menü elkészítése az elemekből
            Console.WriteLine("Válasszon az alábbi programok közül:");
            for (int i = 0; i < elerheto.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {elerheto[i].Nev}");
            }
            Console.WriteLine("0. Befejezés");
            //ha nem szám, akkor errort dobunk
            if (int.TryParse(Console.ReadLine(), out int opcio) && opcio >= 0 && opcio <= elerheto.Count)
            {
                if (opcio == 0)
                    return null;
                return elerheto[opcio - 1];
            }
            throw new Exception("Nem jó számot adtál meg");
        }
        private static void KonfiguracioOsszeallitasa(KonfiguracioOsszeallito config)
        {
            //Típusok bekérése
            List<TYPE> tipusok = GetSelectedTypes();
            Szamitogep sz = new Szamitogep();

            ISzoftver szoft;
            do
            {
                szoft = SelectAvailableSzoftver(sz, tipusok, config);
                if (szoft != null)
                {
                    //Feltelepítjük a szoftvert
                    sz += szoft;
                    //Nem kell többet keresni ebből a típusból
                    tipusok.Remove(szoft.Tipus);
                }
                //ha nullt kapunk vissza, a felhasználó tovább akar menni
            } while (szoft != null);
            //Kérdezzük meg, hogy milyen szempont szerint akarja a gépet
            AskForOptimal(config, tipusok, sz);

        }
        private static void AutoConfig(KonfiguracioOsszeallito config)
        {
            //Típusok bekérése
            List<TYPE> tipusok = GetSelectedTypes();
            //Kérdezzük meg, hogy milyen szempont szerint akarja a gépet
            AskForOptimal(config, tipusok, new Szamitogep());
        }

        private static void AskForOptimal(KonfiguracioOsszeallito config, List<TYPE> tipusok, Szamitogep indulo)
        {
            Console.WriteLine("Mi legyen a prioritás?");
            Console.WriteLine("1. LegkevesebbMemoriaigeny");
            Console.WriteLine("2. Legolcsobb");
            if (int.TryParse(Console.ReadLine(), out int opcio))
            {
                if (opcio == 1)
                    Console.WriteLine("Az Ön gépe: " + config.OptimalisKonfiguracioKereses(tipusok, OptimalizalasiSzempont.LegkevesebbMemoriaigeny, indulo).ToString());
                else if (opcio == 2)
                    Console.WriteLine("Az Ön gépe: " + config.OptimalisKonfiguracioKereses(tipusok, OptimalizalasiSzempont.Legolcsobb, indulo).ToString());
                else
                    Console.WriteLine("Nem létező opció");
            }
            else
                Console.WriteLine("Nem létező opció");
        }

        public static List<TYPE> GetSelectedTypes()
        {
            List<TYPE> selectedTypes = new List<TYPE>();
            int input;
            TYPE selectedType;

            Console.WriteLine("Kérlek, válassz a következő típusokból.");
            do
            {
                //Menü megjelenítése
                Console.WriteLine("\nVálassz egy típust:");
                Console.WriteLine("1. Operációs rendszer");
                Console.WriteLine("2. Táblázatkezelő");
                Console.WriteLine("3. Lemezkezelő");
                Console.WriteLine("4. Szövegszerkesztő");
                Console.WriteLine("5. Antivírus");
                Console.WriteLine("0. Befejezés");
                input = int.Parse(Console.ReadLine());
                switch (input)
                {
                    case 1:
                        selectedType = TYPE.OPERACIOS_RENDSZER;
                        break;
                    case 2:
                        selectedType = TYPE.TABLAZATKEZELO;
                        break;
                    case 3:
                        selectedType = TYPE.LEMEZKEZELO;
                        break;
                    case 4:
                        selectedType = TYPE.SZOVEGSZERKESZTO;
                        break;
                    case 5:
                        selectedType = TYPE.ANTIVIRUS;
                        break;
                    default:
                        selectedType = TYPE.EGYEB;
                        break;
                }
                // Ha valami jót választott, akkor hozzáadjuk a listához
                if (selectedType != TYPE.EGYEB)
                {
                    selectedTypes.Add(selectedType);
                }
                    

            } while (input != 0);

            return selectedTypes;
        }
    }
}

