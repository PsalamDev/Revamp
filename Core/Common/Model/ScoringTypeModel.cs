using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class ScoringTypeModel : BaseResponseModel
    {
        public decimal PercentageScale { get; set; }
        public string RatingScale { get; set; }
        public long NumericScale { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class CreateScoringTypeModelRequest
    {
        public decimal PercentageScale { get; set; }
        public string RatingScale { get; set; }
        public long NumericScale { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }

    public class UpdateScoringTypeModelRequest
    {
        public Guid Id { get; set; }
        public decimal PercentageScale { get; set; }
        public string RatingScale { get; set; }
        public long NumericScale { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}
