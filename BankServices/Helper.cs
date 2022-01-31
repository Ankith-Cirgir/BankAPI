using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServices
{
    public enum GetCharge
    {
        SRTGS = 1,
        SIMPS,
        ORTGS,
        OIMPS,
        PROFITS
    }

    public enum UpdateCharges
    {
        SRTGS = 1,
        SIMPS,
        ORTGS,
        OIMPS,
        NAME,
        PASSWORD,
        REVERT_TRANSACTION
    }
}
