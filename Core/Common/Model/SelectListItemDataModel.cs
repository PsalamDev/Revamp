using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class SelectListItemDataModel
    {
        public SelectListItemDataModel(Guid id,
                                 string code,
                                 string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }



        public Guid Id { get; }
        public string Code { get; }
        public string Name { get; }
    }
}
