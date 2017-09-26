using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace Datafixtures
{


    public class FixtureLoader
    {

        private readonly Dictionary<Type, IFixture> fixtures = new Dictionary<Type, IFixture>();
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();


        public void Load()
        {
            foreach (var i in fixtures.Values)
            {
                i.Load();
            }
        }


        public object Add(Type fixture)
        {
            return (object)Add(fixture, new Stack<Type>());
        }


        public F Add<F>()
            where F : IFixture
        {
            return (F)Add(typeof(F), new Stack<Type>());
        }


        protected IFixture Add(Type fixture, Stack<Type> stack)
        {
            if (false == typeof(IFixture).IsAssignableFrom(fixture))
            {
                throw new Exception();
            }

            TryPushStack(fixture, stack);

            if (fixtures.ContainsKey(fixture))
            {
                return fixtures[fixture];
            }

            var ctor = ValidateConstructor(fixture);
            var args = CollectConstructorArgs(ctor, stack);
            var inst = Instantiate(ctor, args.ToArray());
            fixtures.Add(fixture, inst);

            stack.Pop();

            return inst;
        }


        private static void TryPushStack(Type fixture, Stack<Type> stack)
        {
            if (stack.Contains(fixture))
            {
                stack.Push(fixture);
                var msg = String.Format("Detected cyclic dependency: {0}", String.Join(" -> ", stack.Select(t => t.Name)));
                throw new Exception(msg);
            }
            stack.Push(fixture);
        }


        public void RegisterService<T>(T t)
        {
            if (typeof(IFixture).IsAssignableFrom(typeof(T)))
            {
                throw new Exception("Fixtures cannot be registered as a service.");
            }
            services.Add(typeof(T), t);
        }


        private static ConstructorInfo ValidateConstructor(Type fixture)
        {
            var ctors = fixture.GetConstructors(BindingFlags.Instance | BindingFlags.Public).ToArray();
            if (ctors.Length > 1)
            {
                throw new Exception();
            }
            else if (ctors.Length == 0)
            {
                throw new Exception();
            }
            return ctors[0];
        }


        private object[] CollectConstructorArgs(ConstructorInfo ctor, Stack<Type> stack)
        {
            var args = new List<object>();
            foreach (var par in ctor.GetParameters())
            {
                var isFixture = typeof(IFixture).IsAssignableFrom(par.ParameterType);
                var serviceKnown = services.ContainsKey(par.ParameterType);
                if (isFixture)
                {
                    args.Add(Add(par.ParameterType, stack));
                }
                else if (serviceKnown)
                {
                    args.Add(services[par.ParameterType]);
                }
                else
                {
                    throw new Exception(String.Format("Fixture {0} depends on the service {1} in its constructor, but the service is not registered.", ctor.DeclaringType.FullName, par.ParameterType.FullName));
                }
            }
            return args.ToArray();
        }


        private static IFixture Instantiate(ConstructorInfo ctor, object[] args)
        {
            var fix = (IFixture)ctor.Invoke(args);
            return fix;
        }


    }


}
