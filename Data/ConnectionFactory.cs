using System.Text;
using MongoDB.Driver;
using Pluralize.NET;
using Settings;

namespace Data;

public class ConnectionFactory
{
    public IMongoClient Client() => new MongoClient(ConnectionSettings.Data.ConnectionString);
    public IMongoDatabase Database() => Client().GetDatabase(ConnectionSettings.Data.DatabaseName);
    public IMongoCollection<T> Collection<T>() where T : class => Database().GetCollection<T>(Conventions.UnderscoreSeparatingConvention<T>(10));
}

public static class Conventions
{
    private static IPluralize pluralizer = new Pluralizer();
    public static string UnderscoreSeparatingConvention<T>(int length) where T : class
    {
        var name = typeof(T).Name.Substring(0, typeof(T).Name.Length-length);
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;
        StringBuilder newName = new StringBuilder(name.Length * 2);
        newName.Append(name[0]);
        for (int i = 1; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]))
                if ((name[i - 1] != '_' && !char.IsUpper(name[i - 1])) && 
                    i < name.Length - 1 && !char.IsUpper(name[i + 1]))
                    newName.Append('_');
            newName.Append(name[i]);
        }
        return pluralizer.Pluralize(newName.ToString().ToLower());
    }
}