using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datafixtures.Example.Fixtures
{
    class StartOfUnixTime : Fixture<DateTime>
    {
       protected override DateTime DoLoad()
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
