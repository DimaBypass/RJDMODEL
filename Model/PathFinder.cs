namespace Model;

// класс для поиска пути
internal class PathFinder
{
    // метка участка для алгоритма Дейкстры с запоминанием пути
    private class DijkstraRecord
    {
        internal DijkstraRecord Move(Section s) => new (path.Append(s).ToList(), weight + s.Length);
        internal List<Section> path;
        internal double weight;
        internal bool marked = false;
        internal DijkstraRecord(List<Section> sec, double w) { 
            path = sec;
            weight = w;
        }
        internal Section Current => path.Last();
    }

    // добраться от одного участка до другого
    public static bool Arrive(Section start, Section finish, out List<Section> path) 
    {
        path = new();

        if (start == finish) {
            path.Add(start);
            return true;
        }

        DijkstraRecord djstart = new (new() { start }, start.Length);
        Dictionary<Section, DijkstraRecord> best = new() {{ start, djstart }};
        var current = start;
        do 
        {
            // выделение участка
            best[current].marked = true;
            // обход смежных участков
            foreach (var s in current.Connects) 
            {
                var dj = best[current].Move(s);
                if (best.ContainsKey(s)) {
                    if (best[s].marked) {
                        continue;
                    }
                    if (dj.weight < best[s].weight) {
                        best[s] = dj;
                    }
                }
                else {
                    best.Add(s, dj);
                }
            }
            // обход невыделенных участков
            Section? new_current = null;
            foreach (var s in best.Keys) {
                if (best[s].marked) { 
                    continue; 
                }
                if (new_current is null || best[s].weight < best[new_current].weight) {
                    new_current = s;
                }
            }

            // проверка новой вершины
            if (new_current is null) {
                return false;
            }
            current = new_current;
        } while (current != finish);

        path = best[current].path;
        return true;
    }
}
