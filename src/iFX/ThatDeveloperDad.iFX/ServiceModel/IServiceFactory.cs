using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace ThatDeveloperDad.iFX.ServiceModel
{
	public interface IServiceFactory<T> where T : ISystemComponent
	{
		T CreateService(IConfiguration config, IServiceProvider? standardDependencies = null);
	}
}
