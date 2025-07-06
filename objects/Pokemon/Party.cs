using System.Text;
using PKLib;

namespace PKLib
{
    public class Party
    {
    private List<Pokemon> _pokemonList;

    // Constructor overload for creating Party with a specific Pokemon list
    public Party(List<Pokemon> pokemonList)
    {
        _pokemonList = new List<Pokemon>(pokemonList); // Create a defensive copy
    }

    public string GetInfo()
    {
        StringBuilder sb = new StringBuilder();
        string notFancyLine = "----------------------------------------";

        sb.AppendLine("Party Information:");
        sb.AppendLine();
        sb.AppendLine(notFancyLine);


        ushort count = 0;

        foreach (Pokemon current in _pokemonList)
        {
            sb.AppendLine($"Party Pokemon #{++count}");
            sb.AppendLine();
            sb.AppendLine(current.GetInfo());
            sb.AppendLine(notFancyLine);
        }

        return sb.ToString();

    }

    public IReadOnlyList<Pokemon> GetParty()
    {
        return _pokemonList.AsReadOnly();
    }
}
}