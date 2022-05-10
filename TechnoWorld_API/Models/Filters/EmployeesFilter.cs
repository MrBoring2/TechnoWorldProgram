using System;
using System.Linq.Expressions;
using TechnoWorld_API.Models.Abstractions;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_API.Models.Filters
{
    public class EmployeesFilter : FilterModel<Employee>
    {
        public override Func<Employee, bool> FilterExpression => throw new NotImplementedException();
    }
}
