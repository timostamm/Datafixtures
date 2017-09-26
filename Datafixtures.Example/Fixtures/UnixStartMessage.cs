using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datafixtures.Example.Fixtures
{
    class UnixStartMessage : Fixture<string>
    {

        private readonly StartOfUnixTime start;

        public UnixStartMessage(StartOfUnixTime start)
        {
            this.start = start;
        }

        protected override string DoLoad()
        {
            return "hello world, it is " + start.Result.Year;
        }
    }

}
