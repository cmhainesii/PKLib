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
    public readonly string _speciesName;
    public readonly ushort _level;

    public readonly IV _ivs;
    public readonly Stats _stats;
    public readonly EVs _evs;

    public readonly string _otName;
    public readonly string _nickname;
    public string[] _types;

    public readonly byte _heldItem;
    public readonly ushort _generation;
    public readonly int _otId;

    public readonly int _friendship;

    public Pokemon(string name, ushort level, IV ivs, Stats stats, EVs evs, int friendship, string otName, string nickname, string[] types, int otId, ushort generation, byte heldItem = 0)
    {
        _speciesName = name;
        _level = level;
        _ivs = ivs;
        _stats = stats;
        _evs = evs;
        _otName = otName;
        _nickname = nickname;
        _types = (string[])types.Clone();
        _generation = generation;
        _otId = otId;
        if (generation != 1)
        {
            _heldItem = heldItem;
        }

        _friendship = friendship;
    }

    public string GetInfo()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"{"Name:",16}{_speciesName,14}");
        sb.AppendLine($"{"Level:",16}{_level,14}");
        
        // Print held item info if generation 2
        if (_generation != 1)
        {
            ItemData itemData = new ItemData(2);
            if (_heldItem != 0)
            {
                sb.AppendLine($"{"Held Item:",16}{itemData.GetName(_heldItem),14}");
            }
            else
            {
                sb.AppendLine($"{"Held Item:",16}{"None",14}");
            }
        }

        sb.AppendLine($"{"Type 1:",16}{_types[0],14}");
        sb.AppendLine($"{"Type 2:",16}{_types[1],14}");
        sb.AppendLine();
        sb.AppendLine($"{"",16}{"IV Data",7}{"",13}");
        sb.AppendLine($"{"HP:",16}{_ivs.HP,13}{(_ivs.HP == 15 ? "*" : "")}");
        sb.AppendLine($"{"Attack:",16}{_ivs.Attack,13}{(_ivs.Attack == 15 ? "*" : "")}");
        sb.AppendLine($"{"Defense:",16}{_ivs.Defense,13}{(_ivs.Defense == 15 ? "*" : "")}");
        sb.AppendLine($"{"Special:",16}{_ivs.Special,13}{(_ivs.Special == 15 ? "*" : "")}");
        sb.AppendLine($"{"Speed:",16}{_ivs.Speed,13}{(_ivs.Speed == 15 ? "*" : "")}");
        sb.AppendLine($"{"Score:",16}{GetIvScore(),14}");
        sb.AppendLine($"{"Percentile:",16}{getIvPercentile(),13:F2}{"%",1}");
        
        if(GetStatScore() > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"{"",16}{"Stats",5}{"",12}");
            sb.AppendLine($"{"HP:",16}{_stats.HP,14}");
            sb.AppendLine($"{"Attack:",16}{_stats.Attack,14}");
            sb.AppendLine($"{"Defense:",16}{_stats.Defense,14}");
            sb.AppendLine($"{"Special Attack:",16}{_stats.SpecialAttack,14}");
            sb.AppendLine($"{"Special Defense:",16}{_stats.SpecialDefense,14}");
            sb.AppendLine($"{"Speed:",16}{_stats.Speed,14}");
            sb.AppendLine($"{"Score:",16}{GetStatScore(),14}");
        }

        if(GetEvScore() > 0) 
        {
            sb.AppendLine();
            sb.AppendLine($"{"",16}{"EV Data",7}{"",15}");
            sb.AppendLine($"{"HP:",16}{_evs.HP,14:N0}");
            sb.AppendLine($"{"Attack:",16}{_evs.Attack,14:N0}");
            sb.AppendLine($"{"Defense:",16}{_evs.Defense,14:N0}");
            sb.AppendLine($"{"Special:",16}{_evs.Special,14:N0}");
            sb.AppendLine($"{"Speed:",16}{_evs.Speed,14:N0}");
            sb.AppendLine($"{"Score:",16}{GetEvScore(),14:N0}");
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine();
            sb.AppendLine($"{"All Evs:",16}{"0",14}");
            sb.AppendLine();
        }

        sb.AppendLine($"{"OT Name:",16}{_otName,14}");
        sb.AppendLine($"{"OT ID:",16}{_otId,14:D5}");
        sb.AppendLine($"{"Nickname:",16}{_nickname,14}");

        if (_friendship != 0)
        {
            sb.AppendLine($"{"Friendship:",16}{_friendship,14}");
        }


        return sb.ToString();
    }

    public override string ToString()
    {
        return _speciesName;
    }

    public int GetEvScore()
    {
        return _evs.HP + _evs.Attack + _evs.Defense + _evs.Speed + (_evs.Special * 2);
    }

    public int GetStatScore()
    {
        return _stats.HP + _stats.Attack + _stats.Defense + _stats.Speed + _stats.SpecialAttack + _stats.SpecialDefense;
    }

    public int GetIvScore()
    {
        return this._ivs.Attack + this._ivs.Defense + this._ivs.Speed + _ivs.Special;
    }

    public double getIvPercentile()
    {
        return Math.Round(GetIvScore() / 60.0 * 100.0, 2);
    }





}