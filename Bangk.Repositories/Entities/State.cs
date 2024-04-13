using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Entities
{
    public class State: EntityBase
    {
        public string Name { get; set; }

        [ForeignKey("Countries")]
        public int CountryId { get; set; }
        public Country Countries { get; set; }


    }
}
