using System;
using System.Collections;
using System.Collections.Generic;
using ThatDeveloperDad.iFX.ServiceModel;

namespace ThatDeveloperDad.iFX.ObjectUtilities;

public interface IValidator<T>
{
        IEnumerable<ServiceError> Validate(T instance);

        /// <summary>
        /// Allows for workload or step-specific validation.
        /// By default, this simply calls the "global" validation.
        /// 
        /// Override this to handle different usage scenarios.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="workloadName"></param>
        /// <returns></returns>
        IEnumerable<ServiceError> Validate(T instance, string workloadName)
                => Validate(instance);
}
