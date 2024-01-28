namespace Model;

// участок
public class Section {
    public Section(Nod a, Nod b, int id) {
        Nods = new[] { a, b };
        Length = Nod.Distance(a, b);
        ID = id;

        if (a.Y == b.Y) {
            Type = SectionType.Horizontal;
        }
        if (Math.Abs(a.X - b.X) == Math.Abs(a.Y - b.Y)) {
            Type = SectionType.Diagonal;
        }
    }

    internal int ID { get; }
    internal Nod[] Nods { get; }
    internal double Length { get; }
    internal SectionType Type { get; }
    internal Track? Owner { get; set; }
    internal Nod A => Nods[0];
    internal Nod B => Nods[1];  
    internal string Name => $"Участок {ID} (Путь {Owner?.Name ?? "-"})";

    // соединенные участки (под тупым углом, можно проехать) 
    public List<Section> Connects { get; private set; } = new();

    // соединить два участка
    public static void Connect(Section s1, Section s2) {
        // ищу общую точку
        for (int i = 0; i < 2; i ++) {
            for (int j = 0; j < 2; j++) {
                if (s1.Nods[i] == s2.Nods[j]) {

                    // проверяю, что угол между участками тупой, тогда по нему сможет ездить поезд
                    // тупизна угла определяется отрицательным скалярным произведением
                    double dotprod 
                        = (s1.Nods[1 - i].X - s1.Nods[i].X) * (s2.Nods[1 - j].X - s2.Nods[j].X) 
                        + (s1.Nods[1 - i].Y - s1.Nods[i].Y) * (s2.Nods[1 - j].Y - s2.Nods[j].Y);

                    if (dotprod >= 0) {
                        // у накладывающихся (0°) скалярное произведение равно произведению длин
                        if (dotprod == s1.Length * s2.Length) {
                            throw new Exception("Наложение участков!");
                        }
                        return;
                    }
                    s1.Nods[i].Mount(s1);
                    s2.Nods[j].Mount(s2);
                    s1.Connects.Add(s2);
                    s2.Connects.Add(s1);
                }
            }    
        }
    }
}
