namespace Model;

// узел, один из концов участка
public class Nod
{
    public Nod(double x, double y) {
        X = x;
        Y = y;
    }
    internal double X { get; }
    internal double Y { get; }
    internal static double Distance(Nod a, Nod b) => Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

    // все участки входящие в узел
    private List<Section> Sections { get; set; } = new();

    // прикрепить к участок к точке
    internal void Mount(Section s) {
        if (Sections.Contains(s)) {
            return;
        }
        if (s.A != this && s.B != this) { 
            return; 
        }
        var other = s.A == this ? s.B : s.A;
        var dir = (other.Y == Y)
            ? (other.X < X ? Dir.Left : Dir.Right)
            : (other.Y < Y ? Dir.Up : Dir.Down);
        
        DirSections.Add(dir, s);
        Sections.Add(s);
    }

    // для рисования
    private Dictionary<Dir, Section> DirSections { get; set; } = new();
    internal int DirNumber => DirSections.Count;
    internal bool HasDir(Dir dir) => DirSections.ContainsKey(dir);
    internal int Xi => (int) X;
    internal int Yi => (int) Y;
}   


