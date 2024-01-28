using System.Diagnostics;

namespace Model;

// путь
public class Track
{
    internal string Name { get; }
    internal IReadOnlyList<Section> Sections { get; }
    public Track(string name, Section a, Section b) {
        if (!PathFinder.Arrive(a, b, out var path)) {
            throw new Exception("Заданные участки не могут входить в один путь!");
        }
        if (path.Any(s => s.Owner is not null)) {
            throw new Exception("Путь пересекается с существующим!");
        }
        Name = name;
        Sections = path;
        foreach (var sec in Sections) {
            sec.Owner = this;
        }
    }
    internal Park? Owner { get; set; }
}
