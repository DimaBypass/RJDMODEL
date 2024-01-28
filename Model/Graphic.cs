namespace Model;
public partial class Station {
    internal const int Width = 6;
    internal const int Length = 16;

    private char[,] Map { get; set; } = new char[Length * 3, Width * 2];
    private int MapY(int y) => y * 2;
    private int MapX(int x) => x * 3 + 1;

    // нарисовать граф
    public void Print() {
        for (int y = 0; y < Width * 2; y++) {
            for (int x = 0; x < Length * 3; x++) {
                Console.Write(Map[x, y]);
            }
            Console.WriteLine();
        }
    }

    public void Draw(bool num) {
        for (int y = 0; y < Width * 2; y++) {
            for (int x = 0; x < Length * 3; x++) {
                Map[x, y] = ' ';
            }
        }
        DrawSections(Sections, false, num);
    }

    internal void DrawSections(IEnumerable<Section> sections, bool bold, bool num = false) {
        foreach (var s in sections) {
            DrawTrackSection(s, bold, num);
        }
        foreach (var s in sections) {
            DrawShiftSection(s, bold);
        }
        foreach (var s in sections) {
            for (int i = 0; i < 2; i++) {
                DrawNod(s.Nods[i], false);      
            }
        }
    }
    internal void DrawPark(Park park) {
        foreach(var track in park.Tracks) {
            DrawSections(track.Sections, true);
        }
        var nods = park.Fill();
        foreach (var n in nods) {
            DrawNod(n, true);    
        }
    }

    private void DrawTrackSection(Section s, bool bold, bool num) {     
        if (s.Type != SectionType.Horizontal) {
            return;
        }
        int y = s.A.Yi;
        int x1 = Math.Min(s.A.Xi, s.B.Xi);
        int x2 = Math.Max(s.A.Xi, s.B.Xi);
        char c = bold ? '═' : '─';

        for (int x = x1; x <= x2; x++) {
            int mx = MapX(x);
            int my = MapY(y);
            if (x > x1) { 
                Map[mx - 1, my] = c; 
            }
            if (x > x1 && x < x2) {
                Map[mx, my] = c;
            }
            if (x < x2) {
                Map[mx + 1, my] = c ;
            }
        }
   
        if (num) {
            string id = s.ID.ToString();
            int skip = ((x2 - x1) * 3 - id.Length) / 2 + 1;
            if (skip > 0) {
                for (int i = 0; i < id.Length; i++) {
                    int mx = MapX(x1) + skip + i;
                    int my = MapY(y);
                    Map[mx, my] = id[i];
                }
            }
        }
    }
    private void DrawShiftSection(Section s, bool bold) {
        if (s.Type != SectionType.Diagonal) {
            return;
        }
        var up = s.A.Yi < s.B.Yi ? s.A : s.B; // верхняя
        var dw = s.A.Yi < s.B.Yi ? s.B : s.A; // нижняя     
        bool right = (up.Xi > dw.Xi);         // наклон \left или /right

        char c1 = right ? (bold ? '╔' : '┌') : (bold ? '╗' : '┐');
        char c2 = bold ? '═' : '─';
        char c3 = right ? (bold ? '╝' : '┘') : (bold ? '╚' : '└');
        char c4 = bold ? '║' : '│';

        for (int y = up.Yi; y <= dw.Yi; y++) {
            int x = up.Xi + (y - up.Yi) * (right ? -1 : 1);
            int mx = MapX(x);
            int my = MapY(y);

            if (y > up.Yi && y < dw.Yi) { 
                Map[mx, my] = c4; 
            }
            if (y > up.Yi && my > 0) {
                Map[mx - 1, my - 1] = right ? ' ' : (y == up.Yi ? ' ' : c2);
                Map[mx, my - 1] = y == up.Yi ? ' ' : c1;
                Map[mx + 1, my - 1] = right ? (y == up.Yi ? ' ' : c2) : ' ';
            }
            if (y < dw.Yi && my < (Width * 2 - 1)) {
                Map[mx - 1, my + 1] = right ? (y == dw.Yi ? ' ' : c2) : ' ';
                Map[mx, my + 1] = y == dw.Yi ? ' ' : c3;
                Map[mx + 1, my + 1] = right ? ' ' : (y == dw.Yi ? ' ' : c2);
            }
        }
    }
    private void DrawNod(Nod n, bool bold) {
        if (bold) {
            Map[MapX(n.Xi), MapY(n.Yi)] = '@';
            return;
        }
        var c = ' ';
        switch (n.DirNumber) {
            case 0: 
                return;
            case 1:
                return;
            case 2:
                if (n.HasDir(Dir.Left) && n.HasDir(Dir.Right))  c = '═';
                if (n.HasDir(Dir.Left) && n.HasDir(Dir.Up))     c = '╝';
                if (n.HasDir(Dir.Left) && n.HasDir(Dir.Down))   c = '╗';
                if (n.HasDir(Dir.Right) && n.HasDir(Dir.Up))    c = '╚';
                if (n.HasDir(Dir.Right) && n.HasDir(Dir.Down))  c = '╔';
                if (n.HasDir(Dir.Down) && n.HasDir(Dir.Up))     c = '║';
                break;
            case 3:
                if (!n.HasDir(Dir.Up))      c = '╦';
                if (!n.HasDir(Dir.Down))    c = '╩';
                if (!n.HasDir(Dir.Left))    c = '╠';
                if (!n.HasDir(Dir.Right))   c = '╣';
                break;
            case 4:
                c = '╬';
                break;
        }
        Map[MapX(n.Xi), MapY(n.Yi)] = c;
    }
}

// направление
internal enum Dir { 
    Up, 
    Down, 
    Left, 
    Right 
}

internal enum SectionType
{
    Unknown,
    Horizontal,
    Diagonal
}
