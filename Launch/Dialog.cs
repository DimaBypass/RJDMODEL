using Model;

namespace Launch;
internal class Dialog
{
    internal bool  Over { get; set; } = false;
    internal Station Model { get; }

    private enum DialogPage { Main, Fill, Sections, Path }
    private DialogPage CurrentPage { get; set; } = DialogPage.Main;
    private int? StartSection { get; set; } = null;
    private string? path_list;
    internal Dialog(Station model) {
        Model = model;
        Model.Draw(false);
        Iterate(null);
    }

    internal void Iterate (string? command) 
    {
        switch (CurrentPage) {
            case DialogPage.Main:
                switch (command) {
                    case ("0"):
                        Over = true;
                        return;
                    case ("1"):
                        CurrentPage = DialogPage.Fill;
                        break;
                    case ("2"):
                        CurrentPage = DialogPage.Sections;
                        Model.Draw(true);
                        break;
                }
                Rerender();
                return;

            case DialogPage.Fill:
                Model.Draw(false);
                if (command == "0") {
                    CurrentPage = DialogPage.Main;
                    Rerender();
                    return;
                }
                if (!int.TryParse(command, out int park_id)) {
                    Console.WriteLine("Не является номером");
                    return;
                }
                if (!Model.TryDrawPark(park_id)) {
                    Console.WriteLine("Парк с указанным номером не найден");
                    return;
                }
                Rerender();
                return;

            case DialogPage.Sections:
                if (command == "0") {
                    Model.Draw(false);
                    CurrentPage = DialogPage.Main;
                    Rerender();
                    return;
                }
                if (!int.TryParse(command, out int sec_id)) {
                    Console.WriteLine("Не является номером");
                    return;
                }
                if (!Model.SectionExist(sec_id)) {
                    Console.WriteLine("Участок с указанным номером не найден");
                    return;
                }
                if (StartSection is not null) {     
                    if (!Model.TryDrawPath(StartSection.Value, sec_id, out path_list)) {
                        path_list = "Путь между участками не найден!";
                    }
                    CurrentPage = DialogPage.Path;
                    StartSection = null;
                }
                else {
                    StartSection = sec_id;
                }
                Rerender();
                return;

            case DialogPage.Path:
                CurrentPage = DialogPage.Sections;
                path_list = null;
                Model.Draw(true);
                Rerender();
                return;
        }
    }

    private void Rerender() {
        Console.Clear();
        Model.Print();
        Console.WriteLine("================================================================");
        switch (CurrentPage) {
            case DialogPage.Main:
                Console.WriteLine("0 - Выход");
                Console.WriteLine("1 - Заливка парков");
                Console.WriteLine("2 - Поиск пути");
                return;

            case DialogPage.Fill:
                Console.WriteLine(Model.GetParkList());
                Console.WriteLine("================================================================");
                Console.WriteLine("0 - Назад");
                Console.WriteLine("Введите номер парка для вывода на схему");
                return;

            case DialogPage.Sections:
                Console.WriteLine(Model.GetSectionList());
                Console.WriteLine("================================================================");
                Console.WriteLine("0 - Назад");
                if (StartSection is null) {
                    Console.WriteLine("Введите номер начального участка пути");
                }
                else {
                    Console.WriteLine($"Номер начального участка: {StartSection}");
                    Console.WriteLine("Введите номер конечного участка пути");
                }
                return;

            case DialogPage.Path:
                Console.WriteLine(path_list);
                return;
        }
    }





}
