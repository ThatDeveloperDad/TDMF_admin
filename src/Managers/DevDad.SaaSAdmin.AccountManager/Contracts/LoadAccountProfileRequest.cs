using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Contracts
{
	public class LoadAccountProfileRequest:OperationRequest<string>
	{
        public LoadAccountProfileRequest(string workloadName, string? payload = null) : base(workloadName, payload)
        {
        }

		/// <summary>
		/// This is a pass-through property that exposes the Payload in a friendlier manner.
		/// </summary>
		public string? UserId
		{
			get => Payload;
			set => Payload = value;
		}

        public override string OperationName => "LoadCustomerProfile";
    }
}
