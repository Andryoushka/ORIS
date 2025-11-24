using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MigrationFramework;

abstract public class DBSet : Attribute
{

}

public class PrimaryKey : DBSet
{
}

public class Table : DBSet
{
    public string TableName { get; }

    public Table(string tableName)
    {
        TableName = tableName;
    }
}

public class Column : DBSet
{

}