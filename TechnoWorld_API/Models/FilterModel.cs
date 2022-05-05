using System;
using System.Linq.Expressions;
using TechoWorld_DataModels_v2;

namespace TechnoWorld_API.Models
{
    public abstract class FilterModel<BaseEntity>
    {
        public abstract Func<BaseEntity, bool> FilterExpression { get; }
    }
}
