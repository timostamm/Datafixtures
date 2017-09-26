using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Utilities;


namespace Datafixtures
{

    public class FixtureInitializer<TContext> : IDatabaseInitializer<TContext>
        where TContext : DbContext
    {


        private readonly List<Type> queuedFixtures = new List<Type>();
        private readonly Strategy strat;
        private readonly FixtureLoader loader;


        public FixtureInitializer(Strategy strategy = Strategy.DropCreateDatabaseAlways)
        {
            strat = strategy;
            loader = new FixtureLoader();
        }


        public void Add<F>()
            where F : IFixture
        {
            var fixture = typeof(F);
            if (false == typeof(IFixture).IsAssignableFrom(fixture))
            {
                throw new Exception();
            }
            queuedFixtures.Add(fixture);
        }


        public void InitializeDatabase(TContext context)
        {
            if (null == context)
            {
                throw new ArgumentNullException("context");
            }

            // make sure we run internal code EnsureLoadedForContext
            new CreateDatabaseIfNotExists<TContext>(); 
            
            // apply strategy
            switch (strat)
            { 
                case Strategy.DropCreateDatabaseAlways:
                    context.Database.Delete();
                    context.Database.Create();
                    break;

                case Strategy.CreateDatabaseIfNotExists:
                    context.Database.CreateIfNotExists();
                    break;

                case Strategy.AppendFixtures:
                    break;

                default:
                    throw new NotImplementedException(strat.ToString());
            }

            // register context as DbContext as well as as specific implementation
            loader.RegisterService<TContext>(context);
            loader.RegisterService<DbContext>(context);


            // load fixtures
            foreach (var f in queuedFixtures)
            {
                loader.Add(f);
            }
            loader.Load();


            context.SaveChanges();
        }


    }


    public enum Strategy
    {
        DropCreateDatabaseAlways,
        CreateDatabaseIfNotExists,
        AppendFixtures
    }
    

}
