using System;

namespace Skeleton.Entities.Interfaces
{
    public interface IHasDate
    {
        DateTime Created { get; set; }
        DateTime LastModified { get; set; }
    }
}