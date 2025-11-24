using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigrationFramework;

namespace ConsoleApp;

[Table("TestModel")]
public class TestModel
{
    [PrimaryKey]
    public int Id { get; set; }
    [Column]
    public int Number { get; set; }
    [Column]
    public string Text { get; set; }
}
