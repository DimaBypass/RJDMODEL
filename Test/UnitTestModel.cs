using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Model;

[TestClass]
public class UnitTestModel
{
    private readonly Nod a;
    private readonly Nod b;
    private readonly Nod c;
    private readonly Nod d;
    private readonly Nod e;
    private readonly Nod f;

    private readonly Section ab;
    private readonly Section ac;
    private readonly Section cd;
    private readonly Section ef;

    public UnitTestModel(){
        a = new(0, 0);
        b = new(3, 0);
        c = new(2, 2);
        d = new(5, 2);
        e = new(7, 0);
        f = new(7, 2);

        ab = new(a, b, 1);
        ac = new(a, c, 2);
        cd = new(c, d, 3);
        ef = new(e, f, 4);
    }

    [TestMethod]
    public void SharpCornerConnectionTest() {

        // под острым углом
        Section.Connect(ab, ac); 
        // не должны присоединиться
        Assert.IsFalse(ab.Connects.Contains(ac));
        Assert.IsFalse(ac.Connects.Contains(ab));
    }

    [TestMethod]
    public void ConnectionWithoutTouchTest() {

        // не имеют общих точек
        Section.Connect(ab, cd); 
        // не должны присоединиться
        Assert.IsFalse(ab.Connects.Contains(cd));
        Assert.IsFalse(cd.Connects.Contains(ab));
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Заданные участки не могут входить в один путь!")]
    public void TrackWithoutConnectTest() {
        // проложить путь между несвязными частями графа
        Track t = new ("test", ab, ef);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Заданные участки не могут входить в один путь!")]
    public void TrackWithoutPathTest() {
        // проложить путь  под острым углом
        Track t = new("test", ab, cd);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Путь пересекается с существующим!")]
    public void CrossTrackTest() {
        // проложить два пути на одном отрезке
        Track t1 = new("test", ac, ac);
        Track t2 = new("test", ac, cd);
    }

    [TestMethod]
    public void EmptyParkTest() {
        // парк путей
        Park p = new( new(), 1);
        // в оболочке не должно быть точек
        Assert.AreEqual(p.Fill().Count, 0);
    }

    [TestMethod]
    public void LineParkTest() {
        // парк из одного прямого пути
        Track t1 = new("test", ab, ab);
        Park p = new(new() { t1 }, 1);
        // в оболочке должно быть 2 точки
        Assert.AreEqual(p.Fill().Count, 2);
    }

    [TestMethod]
    public void TriangleParkTest() {
        // парк из двух прямых путей, в котором 3 точки из 4 лежат на одной линии
        Track t1 = new("test", ab, ab);
        Track t2 = new("test", ef, ef);
        Park p = new(new() { t1, t2 }, 1);
        // в оболочке должно остаться 3 точки
        Assert.AreEqual(p.Fill().Count, 3);
    }
}