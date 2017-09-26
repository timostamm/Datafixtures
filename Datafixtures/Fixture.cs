using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datafixtures
{

    public abstract class Fixture<T> : IFixture
    {


        private T _result;
        private bool _loaded;


        public void Load()
        {
            _result = DoLoad();
            _loaded = true;
        }


        abstract protected T DoLoad();


        public T Result
        {
            get
            {
                if (!_loaded)
                {
                    throw new InvalidOperationException("Fixture was not loaded.");
                }
                return _result;
            }
        }


        object IFixture.Result
        {
            get
            {
                return (object)Result;
            }
        }
    }
    

}
