using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datafixtures
{

    public interface IFixture
    {
        object Result
        {
            get;
        }
        void Load();
    }

}
