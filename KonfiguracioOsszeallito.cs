using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZTF2_Féléves_NGSKJ6
{
    public class KonfiguracioOsszeallito
    {
        //Gráf a kompatibilitásról
        public Graph<ISzoftver> szoftverGraph = new Graph<ISzoftver>();

        public bool KombatibilisE(Szamitogep sz, ISzoftver szoft)
        {
            foreach (ISzoftver szoftver in sz.Szoftverek)
            {
                //Ha akár egy szoftverrel sem kompatibilis(), akkor a géppel sem kompatibilis
                if (!szoftverGraph.IsConnected(szoftver, szoft))
                {
                    return false;
                }
            }
            return true;
        }

        public void SzoftverHozzaadas(ISzoftver szoftver, List<ISzoftver> kompatibilisSzoftverek)
        {
            //Hozzáadjuk a szoftvert a gráfhoz, az AddNode lekezeli, ha már benne van
            szoftverGraph.AddNode(szoftver);
            //Az összes kompatibilis szoftvert hozzáadjuk a gráfhoz, és összekötjük a szoftverrel
            foreach (ISzoftver kompatibilisSzoftver in kompatibilisSzoftverek)
            {
                szoftverGraph.AddNode(kompatibilisSzoftver);
                szoftverGraph.AddEdge(szoftver, kompatibilisSzoftver);
            }
        }
        public void AddSzoftverKompatibilitas(ISzoftver szoftver, ISzoftver other)
        {
            szoftverGraph.AddNode(szoftver);
            szoftverGraph.AddNode(other);
            szoftverGraph.AddEdge(szoftver, other);
        }
        public Szamitogep OptimalisKonfiguracioKereses(List<TYPE> igenyek, OptimalizalasiSzempont szempont)
        {
            return OptimalisKonfiguracioKereses(igenyek, szempont, new Szamitogep());
        }
        public Szamitogep OptimalisKonfiguracioKereses(List<TYPE> igenyek, OptimalizalasiSzempont szempont, Szamitogep indulo)
        {

            if (szempont == OptimalizalasiSzempont.Legolcsobb)
            {
                return LegolcsobbKonfiguracioKereses(igenyek, indulo);
            }
            else if (szempont == OptimalizalasiSzempont.LegkevesebbMemoriaigeny)
            {
                return LegkevesebbMemoriaKonfiguracioKereses(igenyek, indulo);
            }
            else
            {
                throw new ArgumentException("Nem támogatott optimalizálási szempont.");
            }
        }


        public Szamitogep LegolcsobbKonfiguracioKereses(List<TYPE> igenyek, Szamitogep indulo)
        {
            Szamitogep optimalisSzamitogep = null;
            int optimalisAr = int.MaxValue;
            int induloSzoftver = indulo == null ? 0 : indulo.Szoftverek.Count;

            //Optimális konfiguráció keresése backtrackinggel
            LegolcsobbBacktrackingSearch(indulo, igenyek, ref optimalisSzamitogep, ref optimalisAr);

            //Ha nem találtunk megfelelő konfigurációt, akkor kivételt dobunk. Akkor is kivételt dobunk, ha nem telepítettünk annyi alkalmazást, mint amennyi típusunk van.
            if (optimalisSzamitogep == null || optimalisSzamitogep.Szoftverek.Count - induloSzoftver != igenyek.Count)
            {
                throw new ArgumentException("Nem lehet ilyen konfigurációt létrehozni");
            }

            return optimalisSzamitogep;
        }

        private void LegolcsobbBacktrackingSearch(Szamitogep jelenlegiKonfig, List<TYPE> igenyek, ref Szamitogep optimalisSzamitogep, ref int optimalisAr)
        {
            //Ha az összes igényt kielégítettük, akkor megnézzük, hogy ez az eddigi legolcsóbb konfiguráció-e
            if (igenyek.Count == 0)
            {
                int jelenlegiAr = jelenlegiKonfig.Ar;

                if (jelenlegiAr < optimalisAr)
                {
                    optimalisSzamitogep = jelenlegiKonfig;
                    optimalisAr = jelenlegiAr;
                }

                return;
            }

            TYPE tipus = igenyek.First();
            igenyek.RemoveAt(0);

            //Kikeressük az összes olyan szoftvert, amelyik a kívánt típusú
            List<ISzoftver> possibleSoftwares = szoftverGraph.GetAllNodes()
                .Where(s => s.Tipus == tipus)
                .ToList();


            foreach (ISzoftver software in possibleSoftwares)
            {
                //Ha a szoftver kompatibilis a jelenlegi konfigurációval, akkor hozzáadjuk a konfigurációhoz
                if (jelenlegiKonfig.Szoftverek.All(s => szoftverGraph.GetNeighbors(s).Contains(software)))
                {
                    Szamitogep ujKonfig = new Szamitogep(jelenlegiKonfig);
                    ujKonfig += software;
                    //és rekurzívan meghívjuk a függvényt a maradék típussal
                    LegolcsobbBacktrackingSearch(ujKonfig, new List<TYPE>(igenyek), ref optimalisSzamitogep, ref optimalisAr);
                }
            }

            igenyek.Insert(0, tipus);
        }
        //Hasonlóan a legolcsóbb kereséshez, csak itt a memóriaigényt hasonlítjuk össze
        public Szamitogep LegkevesebbMemoriaKonfiguracioKereses(List<TYPE> igenyek, Szamitogep? indulo)
        {
            Szamitogep optimalisSzamitogep = null;
            int optimalisMemoria = int.MaxValue;
            int induloSzoftver = indulo == null ? 0 : indulo.Szoftverek.Count;

            BacktrackingSearchMemoria(indulo, igenyek, ref optimalisSzamitogep, ref optimalisMemoria);

            if (optimalisSzamitogep == null || optimalisSzamitogep.Szoftverek.Count - induloSzoftver != igenyek.Count)
            {
                throw new ArgumentException("Nem lehet ilyen konfigurációt létrehozni");
            }

            return optimalisSzamitogep;
        }

        //Hasonlóan a legolcsóbb kereséshez, csak itt a memóriaigényt hasonlítjuk össze
        private void BacktrackingSearchMemoria(Szamitogep jelenlegiKonfig, List<TYPE> igenyek, ref Szamitogep optimalisSzamitogep, ref int optimalisMemoria)
        {
            if (igenyek.Count == 0)
            {
                int jelenlegiMemoria = jelenlegiKonfig.Memoriaigeny;

                if (jelenlegiMemoria < optimalisMemoria)
                {
                    optimalisSzamitogep = jelenlegiKonfig;
                    optimalisMemoria = jelenlegiMemoria;
                }

                return;
            }

            TYPE tipus = igenyek.First();
            igenyek.RemoveAt(0);

            List<ISzoftver> possibleSoftwares = szoftverGraph.GetAllNodes()
                .Where(s => s.Tipus == tipus)
                .ToList();

            foreach (ISzoftver software in possibleSoftwares)
            {
                if (jelenlegiKonfig.Szoftverek.All(s => szoftverGraph.GetNeighbors(s).Contains(software)))
                {
                    Szamitogep ujKonfig = new Szamitogep(jelenlegiKonfig);
                    ujKonfig += software;

                    BacktrackingSearchMemoria(ujKonfig, new List<TYPE>(igenyek), ref optimalisSzamitogep, ref optimalisMemoria);
                }
            }

            igenyek.Insert(0, tipus);
        }

    }

    public enum OptimalizalasiSzempont
    {
        LegkevesebbMemoriaigeny,
        Legolcsobb
    }



    public class Graph<T>
    {
        private Dictionary<T, HashSet<T>> adjacencyList = new Dictionary<T, HashSet<T>>();

        public void AddNode(T node)
        {
            //Csak akkor adjuk hozzá a csúcsot, ha még nem szerepel a gráfban
            if (!adjacencyList.ContainsKey(node))
            {
                adjacencyList[node] = new HashSet<T>();
            }
        }

        public void AddEdge(T fromNode, T toNode)
        {
            if (adjacencyList.ContainsKey(fromNode) && adjacencyList.ContainsKey(toNode))
            {
                //HashSet miatt nem kell ellenőrizni, hogy már szerepel-e a gráfban
                adjacencyList[fromNode].Add(toNode);
                adjacencyList[toNode].Add(fromNode);
            }
        }

        public List<T> GetNeighbors(T node)
        {
            if (adjacencyList.ContainsKey(node))
            {
                return adjacencyList[node].ToList();
            }

            return new List<T>();
        }

        public List<T> GetAllNodes()
        {
            return adjacencyList.Keys.ToList();
        }

        public bool ContainsNode(T node)
        {
            return adjacencyList.ContainsKey(node);
        }

        internal bool IsConnected(T elso, T masodik)
        {
            //Össze van e kötve a két csúcs
            if (adjacencyList.ContainsKey(elso) && adjacencyList.ContainsKey(masodik))
            {
                return adjacencyList[elso].Contains(masodik);
            }
            else
            {
                return false;
            }

        }
    }
}
