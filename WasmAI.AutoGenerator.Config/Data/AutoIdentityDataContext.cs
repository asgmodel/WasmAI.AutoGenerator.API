using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AutoGenerator.Data;



public class AutoIdentityDataContext<TUser, TRole, TValue> : IdentityDbContext<TUser, TRole, TValue>
    where TUser : IdentityUser<TValue>
    where TRole : IdentityRole<TValue>
    where TValue : IEquatable<TValue>
{

    protected virtual Dictionary<Type, object>? DbSets { get; private set; }


    public AutoIdentityDataContext(DbContextOptions options) : base(options)
    {
        DbSets = new Dictionary<Type, object>();
        DiscoverDbSets();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);



    }
    public virtual DbSet<TEntity>? TSet<TEntity>() where TEntity : class
    {
        if (DbSets != null && DbSets.ContainsKey(typeof(TEntity)))
            return DbSets[typeof(TEntity)] as DbSet<TEntity>;
        return null;
    }


    private void DiscoverDbSets()
    {
        var dbSetProps = this.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .Where(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .ToList();

        foreach (var prop in dbSetProps)
        {
            var entityType = prop.PropertyType.GetGenericArguments()[0];
            var dbSetInstance = prop.GetValue(this); // 

            if (dbSetInstance != null)
                DbSets[entityType] = dbSetInstance;
        }

        Console.WriteLine("Stored DbSet handles:");
        foreach (var kv in DbSets)
        {
            Console.WriteLine($"- Entity: {kv.Key.Name}, Handle Type: {kv.Value.GetType().Name}");
        }
    }


}
