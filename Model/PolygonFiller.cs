using System.Linq;

namespace Model;

// класс для построения минимальной выпуклой оболочки (алгоритм Грэхэма)
internal class  PolygonFiller
{
    // залить парк (вернуть вершины многоугольника)
    internal static List<Nod> Fill(Park park) 
    {     
        List<Nod> set = new();
        foreach (var track in park.Tracks) {
            foreach (var sec in track.Sections) {
                foreach (var nod in sec.Nods) {
                    if (!set.Contains(nod)) {
                        set.Add(nod);
                    }
                }                
            }
        }
        if (set.Count < 3) { 
            return set; }

        // стартовая точка - самая левая (из нескольких - самая верхняя)
        var zero = set.OrderBy(n => n.X).ThenBy(n => n.Y).First();
        var array = set.ToArray();

        // сортировка по углу относительно zero
        Array.Sort(array, Compare);
        int Compare(Nod a, Nod b) => Math.Sign(Сlockwise(zero, a , b));
        
        // обход с проверкой угла
        List<Nod> hull = new() { array[0], array[1] };
        for (int i = 2; i < array.Length; i++) 
        {
            while (Сlockwise(hull[^2], hull[^1], array[i]) >= 0) {              
                hull.Remove(hull[^1]);
                if (hull.Count < 2) {
                    break;
                }
            }
            hull.Add(array[i]);
        }
        return hull;
    }

    // если < 0, то при переходе из A в B совершен поворот по часовой стрелке относительно O
    // направление поворота проверяю векторным произведением
    private static double Сlockwise(Nod o, Nod a, Nod b) {
        if (a == o) {
            return -1;
        }
        if (b == o) {
            return 1;
        }
        return (a.Y - o.Y) * (b.X - o.X) - (a.X - o.X) * (b.Y - o.Y);
    }
}
