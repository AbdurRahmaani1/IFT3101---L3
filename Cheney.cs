using System.Collections;
using System.Linq;

class Program
{
    public class Racine
    {
        public string name;
        public Racine(string name) => this.name = name;

        public List<Objet> Ref = [];

        public void add(Objet o)
        {
            Ref.Add(o);
            o.reference = true;
        }

    }

    public class Objet
    {
        public string name;
        public int taille;
        public int adresse;
        public bool marque;
        public Objet(string name, int taille, int adresse)
        {
            this.name = name;
            this.taille = taille;
            this.adresse = adresse;
        }


        public List<Objet> Ref = [];

        public bool reference = false;
        public void add(Objet o)
        {
            Ref.Add(o);
            o.reference = true;
        }

    }

    static void Main()
    {
        List<Racine> Racines = [];
        List<Objet> Objets = [];
        List<Objet> ObjetsNonVisites = [];
        string input;
        int intInput;
        List<string> reponse = [];
        int free = 0;
        List<int> value = [];

        int nbrRacines;
        int nbrObjets;

        System.Console.WriteLine("Entrez, en ordre, tous les objets présents dans l'état de la mémoire actuel (et leur taille). Entrez '0' pour terminer");
        while (true)
        {
            input = System.Console.ReadLine();
            if (input == "0")
            {
                break;
            }
            intInput = int.Parse(System.Console.ReadLine());

            Objets.Add(new Objet(input, intInput, free));
            free += intInput;
        }

        System.Console.WriteLine("combien de racines?");
        nbrRacines = int.Parse(System.Console.ReadLine());

        for (int i = 0; i < nbrRacines; i++)
        {
            System.Console.WriteLine("entrez le nom de la " + (i + 1).ToString() + "e racine");
            input = System.Console.ReadLine();
            Racines.Add(new Racine(input));
            System.Console.WriteLine("entrez les objets vers lesquels cette racine (" + Racines[i].name + ") pointe. Entrez '0' pour terminer");
            while (true)
            {
                input = System.Console.ReadLine();
                if (input == "0")
                {
                    break;
                }
                if (Objets.Exists(o => o.name == input))
                {
                    Racines[i].add(Objets.Find(o => o.name == input));
                }

            }
        }
        for (int i = 0; i < Objets.Count; i++)
        {
            System.Console.WriteLine("entrez les objets vers lesquels l'objet (" + Objets[i].name + ") pointe. entrez 0 pour terminer");
            while (true)
            {
                input = System.Console.ReadLine();
                if (input == "0")
                {
                    break;
                }

                if (Objets.Exists(o => o.name == input))
                {
                    Objets[i].add(Objets.Find(o => o.name == input));
                }

            }
        }

        //AFFICHAGE - Debut..
        for (int i = 0; i < Objets.Count; i++)
        {
            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine(Objets[i].name + " (Adresse avant GC: " + Objets[i].adresse.ToString() + ")");
            if (Objets[i].Ref.Count > 0)
            {
                System.Console.WriteLine("  References: ");
                for (int j = 0; j < Objets[i].Ref.Count; j++)
                {
                    System.Console.WriteLine("  " + Objets[i].Ref[j].name + " (" + Objets[i].Ref[j].adresse.ToString() + ")");
                }
            }
            else
            {
                System.Console.WriteLine(Environment.NewLine);
            }
        }

        System.Console.WriteLine("--------------------------------------------------------");
        //AFFICHAGE - Fin..

        free = 500;
        for (int i = 0; i < Racines.Count; i++)
        {
            for (int j = 0; j < Racines[i].Ref.Count; j++)
            {

                if (!Racines[i].Ref[j].marque)
                {
                    Racines[i].Ref[j].marque = true;
                    ObjetsNonVisites.Add(Racines[i].Ref[j]);
                    Racines[i].Ref[j].adresse = free;
                    free += Racines[i].Ref[j].taille;
                }
            }
        }

        for (int i = 0; i < ObjetsNonVisites.Count; i++)
        {
            for (int j = 0; j < ObjetsNonVisites[i].Ref.Count; j++)
            {

                if (!ObjetsNonVisites[i].Ref[j].marque)
                {
                    ObjetsNonVisites[i].Ref[j].marque = true;
                    ObjetsNonVisites.Add(ObjetsNonVisites[i].Ref[j]);
                    ObjetsNonVisites[i].Ref[j].adresse = free;
                    free += ObjetsNonVisites[i].Ref[j].taille;
                }
            }
        }


        //AFFICHAGE - Debut..
        for (int i = 0; i < ObjetsNonVisites.Count; i++)
        {
            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine(ObjetsNonVisites[i].name + " (Adresse apres GC: " + ObjetsNonVisites[i].adresse.ToString() + ")");
            if (ObjetsNonVisites[i].Ref.Count > 0)
            {
                System.Console.WriteLine("  References: ");
                for (int j = 0; j < ObjetsNonVisites[i].Ref.Count; j++)
                {
                    System.Console.WriteLine("  "+ObjetsNonVisites[i].Ref[j].name + " (" + ObjetsNonVisites[i].Ref[j].adresse.ToString() + ")");
                }
            }
            else
            {
                System.Console.WriteLine(Environment.NewLine);
            }
        }
        //AFFICHAGE - Fin..
    }
}
