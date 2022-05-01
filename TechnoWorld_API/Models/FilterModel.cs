using System;
using System.Linq.Expressions;
using TechoWorld_DataModels;

namespace TechnoWorld_API.Models
{
    public abstract class FilterModel<BaseEntity>
    {
        public abstract Func<BaseEntity, bool> FilterExpression { get; }
    }
}
