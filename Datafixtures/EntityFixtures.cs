using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Utilities;


namespace Datafixtures
{

    public static class EntityFixtures
    {


        public static FixtureLoader DropCreateDatabase<TContext>(TContext context)
            where TContext : DbContext
        {
            var connectionString = context.Database.Connection.ConnectionString;
            if (connectionString.ToLowerInvariant().IndexOf("data source=(localdb)") == -1)
            {
                throw new InvalidOperationException("Cannot drop database because data source is not (localdb).");
            }
            return DropCreateDatabaseDoNotEnsureLocalDB<TContext>(context);
        }



        public static FixtureLoader DropCreateDatabaseDoNotEnsureLocalDB<TContext>(TContext context)
            where TContext : DbContext
        {
            context.Database.Delete();
            context.Database.Create();
            var loader = new FixtureLoader();
            loader.RegisterService<DbContext>(context);
            loader.RegisterService<TContext>(context);
            return loader;
        }



    }


}
