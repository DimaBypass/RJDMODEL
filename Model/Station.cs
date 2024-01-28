using System.Diagnostics;

namespace Model;

// станция
public partial class Station {
    
    // участки
    internal List<Section> Sections { get; set; } = new();
    // пути
    internal List<Track> Tracks { get; set; } = new();
    // парки
    internal List<Park> Parks { get; set; } = new();

    // создание участков
    public void Load(List<Section>? sections = null, List<Track> ? tracks = null, List<Park>? parks = null) {                
        Sections = sections ?? SectionSet();
        for (int i = 0; i < Sections.Count; i++) {
            for (int j = i + 1; j < Sections.Count; j++) {
                Section.Connect(Sections[i], Sections[j]);
            }
        }

        bool custom_set = sections is not null;
        Tracks = tracks ?? (custom_set ? new() : TrackSet());
        Parks = parks ?? (custom_set ? new() : ParkSet());
    }

    private List<Section> SectionSet() {
        var grid = new Nod[Length, Width];
        for (int y = 0; y < Width; y++) {
            for (int x = 0; x < Length; x++) {
                grid[x, y] = new Nod(x, y);
            }
        }

        int id = 1;        
        return new List<Section>() 
        {
            new(grid[3, 1], grid[6, 1], id++),
            new(grid[6, 1], grid[9, 1], id++),

            new(grid[0, 2], grid[2, 2], id++),
            new(grid[2, 2], grid[7, 2], id++),
            new(grid[7, 2], grid[13, 2], id++),
            new(grid[13, 2], grid[15, 2], id++),

            new(grid[2, 3], grid[5, 3], id++),
            new(grid[5, 3], grid[8, 3], id++),
            new(grid[8, 3], grid[11, 3], id++),
            new(grid[11, 3], grid[12, 3], id++),

            new(grid[0, 4], grid[1, 4], id++),
            new(grid[1, 4], grid[3, 4], id++),
            new(grid[3, 4], grid[7, 4], id++),
            new(grid[7, 4], grid[12, 4], id++),
            new(grid[12, 4], grid[13, 4], id++),
            new(grid[13, 4], grid[15, 4], id++),

            new(grid[4, 5], grid[8, 5], id++),
            new(grid[8, 5], grid[12, 5], id++),

            new(grid[2, 2], grid[3, 1], id++),
            new(grid[9, 1], grid[11, 3], id++),
            new(grid[12, 3], grid[13, 2], id++),
            new(grid[1, 4], grid[2, 3], id++),
            new(grid[11, 3], grid[12, 4], id++),
            new(grid[3, 4], grid[4, 5], id++),   
            new(grid[12, 5], grid[13, 4], id++)
        };
    }
    private List<Track> TrackSet() {

        if (Sections.Count < 18) {
            return new();
        }

        return new() {
            new("A", Sections[0], Sections[1]),
            new("B", Sections[3], Sections[4]),
            new("C", Sections[6], Sections[9]),
            new("D", Sections[11], Sections[14]),
            new("E", Sections[16], Sections[17])
        };
    }

    private List<Park> ParkSet() {
        if (Tracks.Count < 5) {
            return new();
        }

        return new() {
            new(new() {Tracks[0], Tracks[4] }, 1),
            new(new() {Tracks[1], Tracks[2], Tracks[3] }, 2)
        };
    }

    public string GetParkList() 
    {
        string result = "";
        foreach (var park in Parks) {
            result += $"Парк {park.ID}\n";
            result += $"\tПути:\n";
            foreach (var track in park.Tracks) {
                result += $"\t{track.Name}\n";
            }
            List<Nod> fill = park.Fill();
            result += $"\tТочки заливки:\n";
            foreach (var n in fill) {
                result += $"\t({n.X}, {n.Y})\n";
            }
        }
        return result; 
    }

    public bool TryDrawPark(int id) {
        var park = Parks.FirstOrDefault(p => p.ID == id);
        if (park is null) { 
            return false; }
        Draw(false);
        DrawPark(park);
        return true;
    }

    public string GetSectionList() => SectionText(Sections);
    
    private string SectionText(List<Section> sections) {
        string result = "";
        foreach (var sec in sections) {
            result += $"{sec.ID}.\t{sec.Name}\n";
        }
        return result;
    }

    public bool SectionExist(int sec_id) {
        var sec = Sections.FirstOrDefault(s => s.ID == sec_id);
        if (sec is null) {
            return false;
        }
        Draw(true);
        DrawSections( new[]{sec}, true, true);
        return true;
    }

    public bool TryDrawPath(int start_id, int finish_id, out string path_text) {
        path_text = "";
        var s1 = Sections.FirstOrDefault(s => s.ID == start_id);
        var s2 = Sections.FirstOrDefault(s => s.ID == finish_id);
        if (s1 is null || s2 is null) {
            throw new Exception("Участки не найдены!");
        }
        bool path_exist = PathFinder.Arrive(s1, s2, out var path);
        if (path_exist) {
            Draw(true);
            DrawSections(path, true);
            path_text = SectionText(path);
        }
        return path_exist;
    }
}
