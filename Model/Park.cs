namespace Model;

// парк, с которым связаны пути
public class Park
{
    internal int ID { get; }
    internal IReadOnlyList<Track> Tracks { get; private set; }
    public Park(List<Track> tracks, int id) {
        ID = id;
        Tracks = tracks
            .Where(t => t.Owner is null)
            .ToList();

        foreach(var track in Tracks) {
            track.Owner = this;
        }
    }

    public List<Nod> Fill() => PolygonFiller.Fill(this);
}