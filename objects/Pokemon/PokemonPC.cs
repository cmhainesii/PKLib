using System.Text;
using PKLib;
public class PokemonPC
{
    private Box[] boxes { get; set; }
    public int count { get; }

    public PokemonPC(GameData gameData)
    {
        boxes = new Box[12];




        for (ushort i = 0; i < 12; ++i)
        {
            boxes[i] = new Box(gameData, (ushort)(i + 1));
        }

        int sum = 0;
        foreach (Box current in boxes)
        {
            sum += current.boxCount;
        }
        count = sum;

    }

    public string GetPcPokemonInfo(int box = 0)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("-----------------------");
        sb.AppendLine("PC Pokemon Information:");
        sb.AppendLine("-----------------------");
        sb.AppendLine();

        for (ushort i = 0; i < 12; ++i)
        {
            sb.AppendLine($"Pokemon Box #{i + 1:D2}");
            sb.AppendLine();
            if (boxes[i].boxCount == 0)
            {
                sb.AppendLine("Box is Empty");
                sb.AppendLine();
                continue;
            }

            sb.AppendLine(boxes[i].GetInfo());
        }


        return sb.ToString();
    }

    public string? GetBoxInfo(int boxNumber)
    {
        string? result = null;

        if ((boxNumber >= 1 && boxNumber <= 12) && boxes.ElementAt(boxNumber - 1).boxCount > 0)
        {
            result = boxes.ElementAt(boxNumber - 1).GetInfo();
        }

        return result;

    }

    public Box GetBox(int boxNumber)
    {
        if (boxNumber >= 1 && boxNumber <= 12)
        {
            return boxes.ElementAt(boxNumber - 1);
        }
        else
        {
            return boxes.ElementAt(0);
        }

    }
}