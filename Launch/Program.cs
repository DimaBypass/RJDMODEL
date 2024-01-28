using Model;
namespace Launch;

public class Program
{
    public static void Main()
    {
        Station model = new();
        model.Load();

        Dialog dialog = new(model);

        do {
            string? command = Console.ReadLine();
            dialog.Iterate(command);
        } while (!dialog.Over);
    }
}