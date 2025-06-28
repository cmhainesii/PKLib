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

        // Header with Pokemon name
        sb.AppendLine("════════════════════════════════════════");
        sb.AppendLine($"║ {_speciesName.ToUpper(),-36} ║");
        sb.AppendLine("════════════════════════════════════════");
        sb.AppendLine();

        // Basic Information Section
        sb.AppendLine("┌─ BASIC INFORMATION ─────────────────┐");
        sb.AppendLine($"│ {"Species:",14}{_speciesName,22} │");
        sb.AppendLine($"│ {"Nickname:",14}{_nickname,22} │");
        sb.AppendLine($"│ {"Level:",14}{_level,22} │");
        sb.AppendLine($"│ {"Type:",14}{(_types[1] != "None" ? $"{_types[0]}/{_types[1]}" : _types[0]),22} │");
        
        // Print held item info if generation 2
        if (_generation != 1)
        {
            ItemData itemData = new ItemData(2);
            string heldItemName = _heldItem != 0 ? itemData.GetName(_heldItem) : "None";
            sb.AppendLine($"│ {"Held Item:",14}{heldItemName,22} │");
        }
        
        sb.AppendLine("└─────────────────────────────────────┘");
        sb.AppendLine();

        // Individual Values (IVs) Section
        sb.AppendLine("┌─ INDIVIDUAL VALUES (IVs) ───────────┐");
        sb.AppendLine($"│ {"HP:",14}{_ivs.HP,3} {(_ivs.HP == 15 ? "★" : " "),1}  {GetIvBar(_ivs.HP),10} │");
        sb.AppendLine($"│ {"Attack:",14}{_ivs.Attack,3} {(_ivs.Attack == 15 ? "★" : " "),1}  {GetIvBar(_ivs.Attack),10} │");
        sb.AppendLine($"│ {"Defense:",14}{_ivs.Defense,3} {(_ivs.Defense == 15 ? "★" : " "),1}  {GetIvBar(_ivs.Defense),10} │");
        sb.AppendLine($"│ {"Special:",14}{_ivs.Special,3} {(_ivs.Special == 15 ? "★" : " "),1}  {GetIvBar(_ivs.Special),10} │");
        sb.AppendLine($"│ {"Speed:",14}{_ivs.Speed,3} {(_ivs.Speed == 15 ? "★" : " "),1}  {GetIvBar(_ivs.Speed),10} │");
        sb.AppendLine($"│{"",-37}│");
        sb.AppendLine($"│ {"Total Score:",14}{GetIvScore(),3} / 60{"",-15}│");
        sb.AppendLine($"│ {"Percentile:",14}{getIvPercentile():F1}%{"",-17}│");
        sb.AppendLine("└─────────────────────────────────────┘");
        sb.AppendLine();

        // Current Stats Section (if available)
        if(GetStatScore() > 0)
        {
            sb.AppendLine("┌─ CURRENT STATS ─────────────────────┐");
            sb.AppendLine($"│ {"HP:",14}{_stats.HP,22} │");
            sb.AppendLine($"│ {"Attack:",14}{_stats.Attack,22} │");
            sb.AppendLine($"│ {"Defense:",14}{_stats.Defense,22} │");
            sb.AppendLine($"│ {"Sp. Attack:",14}{_stats.SpecialAttack,22} │");
            sb.AppendLine($"│ {"Sp. Defense:",14}{_stats.SpecialDefense,22} │");
            sb.AppendLine($"│ {"Speed:",14}{_stats.Speed,22} │");
            sb.AppendLine($"│ {"",-14}{"",-22} │");
            sb.AppendLine($"│ {"Total:",14}{GetStatScore(),22} │");
            sb.AppendLine("└─────────────────────────────────────┘");
            sb.AppendLine();
        }

        // Effort Values Section
        if(GetEvScore() > 0) 
        {
            sb.AppendLine("┌─ EFFORT VALUES (EVs) ───────────────┐");
            sb.AppendLine($"│ {"HP:",14}{_evs.HP,22:N0} │");
            sb.AppendLine($"│ {"Attack:",14}{_evs.Attack,22:N0} │");
            sb.AppendLine($"│ {"Defense:",14}{_evs.Defense,22:N0} │");
            sb.AppendLine($"│ {"Special:",14}{_evs.Special,22:N0} │");
            sb.AppendLine($"│ {"Speed:",14}{_evs.Speed,22:N0} │");
            sb.AppendLine($"│ {"",-14}{"",-22} │");
            sb.AppendLine($"│ {"Total:",14}{GetEvScore(),22:N0} │");
            sb.AppendLine("└─────────────────────────────────────┘");
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine("┌─ EFFORT VALUES (EVs) ───────────────┐");
            sb.AppendLine($"│ {"All EVs:",14}{"0",22} │");
            sb.AppendLine("└─────────────────────────────────────┘");
            sb.AppendLine();
        }

        // Trainer Information Section
        sb.AppendLine("┌─ TRAINER INFORMATION ───────────────┐");
        sb.AppendLine($"│ {"OT Name:",14}{_otName,22} │");
        sb.AppendLine($"│ {"OT ID:",14}{_otId:D5}{"",-17} │");
        
        if (_friendship != 0)
        {
            sb.AppendLine($"│ {"Friendship:",14}{_friendship,22} │");
        }
        
        sb.AppendLine("└─────────────────────────────────────┘");

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

    private string GetIvBar(ushort ivValue)
    {
        // Create a visual bar representation of IV value (0-15)
        int barLength = (int)Math.Round((double)ivValue / 15 * 10);
        string bar = new string('█', barLength) + new string('░', 10 - barLength);
        return bar;
    }





}