using MigrationFramework;
using System.Net;

namespace ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        var model = new TestModel()
        {
            Number = 1,
            Text = "sample text"
        };

        var orm = new ORMContext();
        orm.UpdateTable(model.GetType());

        //var httpListner = new HttpListener();
        //httpListner.Start();
    }
}
