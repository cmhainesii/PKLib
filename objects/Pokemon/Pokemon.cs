using System.Text;

public struct IV
{
    public ushort Attack;
    public ushort Defense;
    public ushort Speed;
    public ushort Special;
    public ushort HP;
}

public struct Stats
{
    public ushort Attack;
    public ushort Defense;
    public ushort SpecialAttack;
    public ushort SpecialDefense;
    public ushort Speed;
    public ushort HP;
}

public struct EVs
{
    public ushort HP;
    public ushort Attack;
    public ushort Defense;
    public ushort Speed;
    public ushort Special;
}

public class Pokemon
{
    public readonly string speciesName;
    public readonly ushort level;

    public readonly IV ivs;
    public readonly Stats stats;
    public readonly EVs evs;

    public readonly string otName;
    public readonly string nickname;
    public string[] types;

    public readonly byte heldItem;
    public readonly ushort generation;
    public readonly int otId;

    public Pokemon(string name, ushort level, IV ivs, Stats stats, EVs evs, string otName, string nickname, string[] types, int otId, ushort generation, byte heldItem = 0)
    {
        this.speciesName = name;
        this.level = level;
        this.ivs = ivs;
        this.stats = stats;
        this.evs = evs;
        this.otName = otName;
        this.nickname = nickname;
        this.types = (string[])types.Clone();
        this.generation = generation;
        this.otId = otId;
        if (generation != 1)
        {
            this.heldItem = heldItem;
        }
    }

    public string GetInfo()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"{"Name:",16}{speciesName,14}");
        sb.AppendLine($"{"Level:",16}{level,14}");
        
        // Print held item info if generation 2
        if (generation != 1)
        {
            ItemData itemData = new ItemData(2);
            if (heldItem != 0)
            {
                sb.AppendLine($"{"Held Item:",16}{itemData.GetName(heldItem),14}");
            }
            else
            {
                sb.AppendLine($"{"Held Item:",16}{"None",14}");
            }
        }

        sb.AppendLine($"{"Type 1:",16}{types[0],14}");
        sb.AppendLine($"{"Type 2:",16}{types[1],14}");
        sb.AppendLine();
        sb.AppendLine($"{"",16}{"IV Data",7}{"",13}");
        sb.AppendLine($"{"HP:",16}{ivs.HP,13}{(ivs.HP == 15 ? "*" : "")}");
        sb.AppendLine($"{"Attack:",16}{ivs.Attack,13}{(ivs.Attack == 15 ? "*" : "")}");
        sb.AppendLine($"{"Defense:",16}{ivs.Defense,13}{(ivs.Defense == 15 ? "*" : "")}");
        sb.AppendLine($"{"Special:",16}{ivs.Special,13}{(ivs.Special == 15 ? "*" : "")}");
        sb.AppendLine($"{"Speed:",16}{ivs.Speed,13}{(ivs.Speed == 15 ? "*" : "")}");
        sb.AppendLine($"{"Score:",16}{GetIvScore(),14}");
        sb.AppendLine($"{"Percentile:",16}{getIvPercentile(),13:F2}{"%",1}");
        
        if(GetStatScore() > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"{"",16}{"Stats",5}{"",12}");
            sb.AppendLine($"{"HP:",16}{stats.HP,14}");
            sb.AppendLine($"{"Attack:",16}{stats.Attack,14}");
            sb.AppendLine($"{"Defense:",16}{stats.Defense,14}");
            sb.AppendLine($"{"Special Attack:",16}{stats.SpecialAttack,14}");
            sb.AppendLine($"{"Special Defense:",16}{stats.SpecialDefense,14}");
            sb.AppendLine($"{"Speed:",16}{stats.Speed,14}");
            sb.AppendLine($"{"Score:",16}{GetStatScore(),14}");
        }

        if(GetEvScore() > 0) 
        {
            sb.AppendLine();
            sb.AppendLine($"{"",16}{"EV Data",7}{"",15}");
            sb.AppendLine($"{"HP:",16}{evs.HP,14:N0}");
            sb.AppendLine($"{"Attack:",16}{evs.Attack,14:N0}");
            sb.AppendLine($"{"Defense:",16}{evs.Defense,14:N0}");
            sb.AppendLine($"{"Special:",16}{evs.Special,14:N0}");
            sb.AppendLine($"{"Speed:",16}{evs.Speed,14:N0}");
            sb.AppendLine($"{"Score:",16}{GetEvScore(),14:N0}");
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine();
            sb.AppendLine($"{"All Evs:",16}{"0",14}");
            sb.AppendLine();
        }

        sb.AppendLine($"{"OT Name:",16}{otName,14}");
        sb.AppendLine($"{"OT ID:",16}{otId,14:D5}");
        sb.AppendLine($"{"Nickname:",16}{nickname,14}");


        return sb.ToString();
    }

    public override string ToString()
    {
        return speciesName;
    }

    public int GetEvScore()
    {
        return evs.HP + evs.Attack + evs.Defense + evs.Speed + (evs.Special * 2);
    }

    public int GetStatScore()
    {
        return stats.HP + stats.Attack + stats.Defense + stats.Speed + stats.SpecialAttack + stats.SpecialDefense;
    }

    public int GetIvScore()
    {
        return this.ivs.Attack + this.ivs.Defense + this.ivs.Speed + ivs.Special;
    }

    public double getIvPercentile()
    {
        return Math.Round(GetIvScore() / 60.0 * 100.0, 2);
    }





}