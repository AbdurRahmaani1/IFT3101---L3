using System.Collections.Generic;

class ProductionCodeMachine
{
    public class Adresse
    {
        public string name;
        public Adresse(string Name) => name = Name;

        public int val = 0;
        public bool enMemoire;
        public List<Registre> descr = [];
        public bool active(List<Adresse> AdressesAct)
        {
            for (int i = 0; i < AdressesAct.Count; i++)
            {
                if (name == AdressesAct[i].name)
                {
                    return true;
                }

            }
            return false;
        }
    }

    //---------------------------------------------------------------------------

    public class Registre
    {
        public string name;
        public Registre(string Name) => name = Name;
        public List<Adresse> descr = [];

        public int val()
        {
            if (!(descr.Count == 0))
            {
                return descr[0].val;
            }
            return 0;
        }
        public int score = 0;

        public int Score(List<Adresse> AdressesAct)
        {
            for (int i = 0; i < descr.Count; i++)
            {
                if (!(descr[i].enMemoire) && (descr[i].descr.Count < 2) && (descr[i].active(AdressesAct))) {
                    score = score + 1;
                }
            }
            return score;
        }
    }
    //-------------------------------------------------------------------

    public class LigneInstruction
    {
        public Op op;
        public List<Adresse> ListA1 = [];
        public string ligne;
        public LigneInstruction(string ligne, ref List<Adresse> ListAd, ref Queue<Adresse> queueAd)
        {
            this.ligne = ligne;
            for (int i = 0; i < ligne.Length; i++)
            {
                if (i != 1 && i != 3)
                {
                    Adresse nouvA = new Adresse(ligne[i].ToString());
                    if (!(ListAd.Exists(a => a.name == ligne[i].ToString())))
                    {
                        ListAd.Add(nouvA);
                    }

                    ListA1.Add(nouvA);
                    if (i > 0)
                    {
                        queueAd.Enqueue(nouvA);
                    }
                    
                }
                if (i == 3)
                {
                    switch (ligne[i])
                    {
                        case '+':
                            op = new Op("ADD");
                            break;
                        case '-':
                            op = new Op("SUB");
                            break;
                        case '*':
                            op = new Op("MUL");
                            break;
                        case '/':
                            op = new Op("DIV");
                            break;
                    }
                }
            }
        }
        public bool copy()
        {
            return (ligne.Length < 5);
        }
    }
    //---------------------------------------------------------------------------


    public class Op {
        public string name;
        public Op(string Name) => name = Name;
    }

    //---------------------------------------------------------------------------
    void load(ref List<string> instructions, ref Registre r, ref Adresse a, ref Queue<Adresse> queueAd)
    {
        string name = a.name;
        if (!(r.descr.Exists(c => c.name == name))) 
        {
            string instr = "LD " + r.name + ", " + a.name;
            if (r.descr.Count > 0)
            {
                System.Console.WriteLine("AAAAAAAAA");
            }
            /*
            else
            {
                System.Console.WriteLine(r.name + " + " + a.name + "ici");
            }
            */
                instructions.Add(instr);

            r.descr.Clear();
            r.descr.Add(a);

            a.descr.Add(r);
        }
        if (queueAd.Count > 0)
        {
            queueAd.Dequeue();
        }
    }

    //---------------------------------------------------------------------------

    void opp(ref List<string> instructions, Op op, ref Adresse a, ref Registre r1, ref Registre r2, ref Registre r3, ref Queue<Adresse> queueAd)
    {
        string instr = op.name + " " + r1.name + ", " + r2.name + ", " + r3.name;
        instructions.Add(instr);

        if (op.name == "MUL")
        {
            a.val = r2.val() * r3.val();
        }

        if (op.name == "ADD")
        {
            a.val = r2.val() + r3.val();
        }

        if (op.name == "SUB")
        {
            a.val = r3.val() - r2.val();
        }

        if (op.name == "DIV")
        {
            a.val = r3.val() / r2.val();
        }

        for (int i = 0; i < r1.descr.Count; i++)
        {
            r1.descr[i].descr.Remove(r1);
        }
        r1.descr.Clear();
        r1.descr.Add(a);

        for (int i = 0; i < a.descr.Count; i++)
        {
            a.descr[i].descr.Remove(a);
        }
        a.descr.Clear();
        a.descr.Add(r1);

        if (queueAd.Count > 0)
        {
            queueAd.Dequeue();
        }
    }

    //-------------------------------------------------------------------

    void store(Adresse a, ref List<string> instructions, List<Adresse> AdressesAct)
    {
        if (!(a.descr.Count == 0) && !(a.enMemoire) && AdressesAct.Exists(ad => ad.name == a.name))
        {
            a.enMemoire = true;
            instructions.Add("ST " + a.name + ", " + a.descr[0].name);
        }
    }
    //-------------------------------------------------------------------

    Registre getReg(Adresse a, ref List<Adresse> ListAd, List<Adresse> AdressesAct, ref Queue<Adresse> queueAd, ref List<string> instructions, ref List<Registre> Registres)
    {
        if (ListAd.Exists(d => d.name == a.name && d.descr.Count > 0))
        {
            return ListAd.Find(d => d.name == a.name).descr[0];
        }

        else
        {
            List<int> Scores = [0, 0, 0];
            for (int i = 0; i < Registres.Count; i++)
            {
                Scores[i] = getScore(Registres[i], AdressesAct, ref queueAd);
            }

            for (int i = 0; i < Scores.Count; i++)
            {
                if (Registres[i].score == Scores.Min()) {
                    for (i = 0; i < Registres[i].descr.Count; i++)
                    {
                        store(Registres[i].descr[i], ref instructions, AdressesAct);
                    }
                    return Registres[i];
                }
            }
            return Registres[0];
        }

    }
    //-------------------------------------------------------------------

    int getScore(Registre r, List<Adresse> AdressesAct, ref Queue<Adresse> queueAd)
    {
        int score = r.Score(AdressesAct);
        for (int i = 0; i < r.descr.Count; i++)
        {
            if (queueAd.Contains(r.descr[i]))
            {
                score = score + 1;
            }

        }
        return score;
    }
    //-------------------------------------------------------------------

    void genOp(ref List<Adresse> ListAd, List<Adresse> AdressesAct, ref Queue<Adresse> queueAd, ref List<string> instructions, ref List<Registre> Registres, ref Adresse x, ref Adresse y, Op op, ref Adresse z)
    {
        Registre ry = getReg(y, ref ListAd, AdressesAct, ref queueAd, ref instructions, ref Registres);
        load(ref instructions, ref ry, ref y, ref queueAd);

        Registre rz = getReg(z, ref ListAd, AdressesAct, ref queueAd, ref instructions, ref Registres);
        load(ref instructions, ref rz, ref z, ref queueAd);

        Registre rx = getReg(x, ref ListAd, AdressesAct, ref queueAd, ref instructions, ref Registres);
        opp(ref instructions, op, ref x, ref rx, ref ry, ref rz, ref queueAd);
    }
    //-------------------------------------------------------------------

    void GenCopy(ref List<Adresse> ListAd, List<Adresse> AdressesAct, ref List<string> instructions, ref List<Registre> Registres, ref Queue<Adresse> queueAd, ref Adresse x, ref Adresse y)
    {
        Registre ry = getReg(y, ref ListAd, AdressesAct, ref queueAd, ref instructions, ref Registres);
        load(ref instructions, ref ry, ref y, ref queueAd);

        ry.descr.Add(x);
        x.descr.Clear();
        x.descr.Add(ry);
    }

    //-------------------------------------------------------------------

    static void Main()
    {
        var p = new Program();

        List<Adresse> ListAd = [];
        List<Registre> Registres = [];
        List<Adresse> AdresseAct = [];
        List<string> instructions = [];
        List<LigneInstruction> LigneEntrees = [];
        Queue<Adresse> queueAd = [];
        List<string> reponse = [];

        Registre R1 = new Registre("R1");
        Registre R2 = new Registre("R2");
        Registre R3 = new Registre("R3");

        Adresse x;
        Adresse y;
        Adresse z;

        Registres.Add(R1);
        Registres.Add(R2);
        Registres.Add(R3);

        string input;

        do
        {
            System.Console.WriteLine("entrez une ligne. entrez 0 pour terminer");
            input = System.Console.ReadLine();
            reponse.Add(input);
        }
        while (input != "0");

        reponse.Remove(reponse[(reponse.Count - 1)]);
        for (int i = 0; i < reponse.Count; i++)
        {
            LigneInstruction nouvLigne = new LigneInstruction(reponse[i], ref ListAd, ref queueAd);
            LigneEntrees.Add(nouvLigne);
        }

            for (int i = 0; i < LigneEntrees.Count; i++)
            {
            x = LigneEntrees[i].ListA1[0];
            y = LigneEntrees[i].ListA1[1];
                if (LigneEntrees[i].copy())
                {
                    p.GenCopy(ref ListAd, AdresseAct, ref instructions, ref Registres, ref queueAd, ref x, ref y);
                }
                else
                {
                z = LigneEntrees[i].ListA1[2];
                    p.genOp(ref ListAd, AdresseAct, ref queueAd, ref instructions, ref Registres, ref x, ref y, LigneEntrees[i].op, ref z);
                }
            }
        System.Console.WriteLine("Code machine:" + Environment.NewLine);

            for (int i = 0; i < instructions.Count; i++)
            {
            System.Console.WriteLine(instructions[i] + Environment.NewLine);
            }

        /*for (int i = 0; i < reponse.Count; i++)
        {
            for (int k = 0; k < ListAd.Count; k++)
            {
                System.Console.WriteLine(Environment.NewLine);
                System.Console.WriteLine(i.ToString() + " --- " + ListAd[k].name + " > " + ListAd[k].descr.Count);
                for (int l = 0; l < ListAd[k].descr.Count; l++)
                {
                    System.Console.WriteLine(i.ToString() + " --- " + ListAd[k].name + " > " + ListAd[k].descr[l].name);
                }
                System.Console.WriteLine(Environment.NewLine);
            }
        }*/
        /*for (int i = 0; i < ListAd.Count; i++)
        {
            for (int j = 0; j < ListAd[i].descr.Count; j++)
            {
                //System.Console.WriteLine(ListAd[i].name + "----" + ListAd[i].descr[j].name);
            }
        }*/
    }
                    }
