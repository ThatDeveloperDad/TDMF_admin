using System;
using System.Collections.Generic;
using DevDad.SaaSAdmin.StoreAccess.Abstractions;

namespace DevDad.SaaSAdmin.StoreAccess.LsApi.LemonSqueezyModels;

/// <summary>
/// This class represents the properties we have to include in a CreateChecokout
/// request to lemonsqueezy.
/// 
/// Since that API expects specific spelling and lettercasing, we'll deviate from
/// out ususl C# Naming conventions here.  Don't panic.
/// </summary>
internal class LsCheckoutReq:ILsReqData
{
    public LsCheckoutReq(string customerEntraId, bool isTestMode) 
    {
        test_mode = isTestMode;
        var customData = new LsCheckoutCustomData
        {
            user_identity_id = customerEntraId
        };

        checkout_data = new Dictionary<string, object>();
        checkout_data.Add("custom", customData);
        
    }

    public Dictionary<string, object> checkout_data { get; set; }
    public bool test_mode { get; set; }

    public string expires_at => DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ");
}

internal class LsCheckoutData
{
    public LsCheckoutData()
    {
      custom = new LsCheckoutCustomData();
    }
    public LsCheckoutCustomData custom { get; set; }

    public object[] AsPropertyArray()
    {
        List<object> props = new();

      props.Add(custom);

        return props.ToArray();
    }
}

internal class LsCheckoutCustomData
{
    public string user_identity_id { get; set; } = string.Empty;
}

// template from API Docs:
/* curl -X "POST" "https://api.lemonsqueezy.com/v1/checkouts" \
  -H 'Accept: application/vnd.api+json' \
  -H 'Content-Type: application/vnd.api+json' \
  -H 'Authorization: Bearer {api_key}' \
  -d $'{
  "data": {
    "type": "checkouts",
    "attributes": {
      "custom_price": 50000,
      "product_options": {
        "enabled_variants": [1]
      },
      "checkout_options": {
        "button_color": "#7047EB"
      },
      "checkout_data": {
        "discount_code": "10PERCENTOFF",
        "custom": {
          "user_id": 123
        }
      },
      "expires_at": "2022-10-30T15:20:06Z",
      "preview": true
    },
    "relationships": {
      "store": {
        "data": {
          "type": "stores",
          "id": "1"
        }
      },
      "variant": {
        "data": {
          "type": "variants",
          "id": "1"
        }
      }
    }
  }
}'
 */