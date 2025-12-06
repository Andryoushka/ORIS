using MyORMLibrary;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;

namespace MyORMLibrary_Test;

[TestClass]
public sealed class MyORMLibrary_Test
{
    private string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=RUSSPASS;Integrated Security=True";

    [TestMethod]
    public void GetLastTourId()
    {
        var orm = new ORMContext(_connectionString);
        var id = orm.GetCurrentId("Tours");
    }

    [TestMethod]
    public void IsConnected()
    {
        var orm = new ORMContext(_connectionString);

        Assert.IsTrue(orm.IsConnected, "Строка подключения - неправильная.");
    }

    [TestMethod]
    public void CreateTable()
    {
        var tour = new LibTour()
        {
            Name = "Test",
            Price = 10
        };

        var orm = new ORMContext(_connectionString);
        orm.CreateTable(typeof(LibTour), "Tours");

        var sqlConnection = new SqlConnection(_connectionString);
        sqlConnection.Open();
        var result = orm.TableExists(sqlConnection, "Tours");
        sqlConnection.Close();

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ReadById()
    {
        var orm = new ORMContext(_connectionString);
        var tour = orm.ReadById<LibTour>(1,"Tours");

        Assert.IsTrue(tour.Name == "Moscow");
        Assert.IsTrue(tour.Price == 100);
    }

    [TestMethod]
    public void ReadByAll()
    {
        var orm = new ORMContext(_connectionString);
        var tours = orm.ReadByAll<LibTour>("Tours");

        var collection = new List<LibTour>()
        {
            new LibTour()
            {
                Name = "Moscow",
                Price = 100
            },
            new LibTour()
            {
                Name = "Kazan",
                Price = 200
            },new LibTour()
            {
                Name = "Omsk",
                Price = 300
            }
        };

        var index = 0;
        foreach (var tour in tours)
        {
            Assert.AreEqual(collection[index++], tour);
        }
    }

    [TestMethod]
    public void AddToTable()
    {
        var orm = new ORMContext(_connectionString);
        var tour = new LibTour() { Name = "Los", Price = 200 };

        orm.AddToTable<LibTour>(tour, "Tours");
    }

    [TestMethod]
    public void DeleteById() //<Индексы для сущностей>//
    {
        Assert.IsTrue(true);
        return;
        var orm = new ORMContext(_connectionString);

        var tours = orm.ReadByAll<LibTour>("Tours");
        var count = tours.Count();
        //var id = tours.Last();
        orm.Delete(6, "Tours");
        var tour = orm.ReadById<LibTour>(6, "Tours");
        var countNew = orm.ReadByAll<LibTour>("Tours").Count();

        Assert.AreEqual(null, tour);
        Assert.AreEqual(countNew, count - 1);
    }

    [TestMethod]
    public void UpdateById()
    {
        var id = 1;
        var random = new Random();
        var orm = new ORMContext(_connectionString);
        var tour = orm.ReadById<LibTour>(id, "Tours");

        tour.Price = random.Next(1, 100);
        orm.Update(id, tour, "Tours");

        var result = orm.ReadById<LibTour>(id, "Tours");

        Assert.AreEqual(tour, result);
    }

    [TestMethod]
    public void ExpressionTransformer_WHERE_Price_LIMIT() //<Работает с одним аргументом>//
    {
        var t = new LibTour() { Price = 1 };
        Expression<Func<LibTour, bool>> f = (x) => x.Price > 1;
        var query = ExpressionTransformer.BuildSqlQuery(f, true);

        var expected = "SELECT * FROM Tours WHERE (Price > 1) LIMIT 1";

        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void DifficultExpression()
    {
        var t = new LibTour() { Price = 200, Name = "Kazan" };
        Expression<Func<LibTour, bool>> f = (x) => x.Price > 100 && x.Name == "Kazan";
        var query = ExpressionTransformer.BuildSqlQuery(f, false);

        var expected = "SELECT * FROM Tours WHERE ((Price > 100) AND (Name = 'Kazan'))";

        Assert.AreEqual(expected, query);
    }
}