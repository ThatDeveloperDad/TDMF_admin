using System;
using System.Collections.Generic;
using DevDad.SaaSAdmin.StoreAccess.Abstractions;

namespace DevDad.SaaSAdmin.StoreAccess.LsApi.LemonSqueezyModels;

internal class LsRequestBuilder
{
    private string _requestType = string.Empty;
    private Type? _requestDataType;
    private object? _requestData;

    private LsRelationships _relationships;

    public LsRequestBuilder()
    {
        _relationships = new();
    }

    /// <summary>
    /// Adds the data required to identify this checkout as Test or Live.
    /// Adds the supplied customerId to include in all future event data for this
    /// customer.
    /// Sets up the "variant" of the product that the customer is purchasing.
    /// </summary>
    /// <param name="customerEntraId"></param>
    /// <param name="productVariantId"></param>
    /// <param name="isTest"></param>
    /// <returns></returns>
    public LsRequestBuilder AsNewCheckout(
            string customerEntraId, 
            int productVariantId,
            string localSystemProductSku,
            bool isTest)
    {
        _requestType = "checkouts";
        _requestDataType = typeof(LsCheckoutReq);
        _requestData = new LsCheckoutReq(customerEntraId, localSystemProductSku, isTest);

        LsNodeData variantNode = new()
        {
            type = "variants",
            id = productVariantId.ToString()
        };
        _relationships.variant = new LsRelationNode(variantNode);

        return this;
    }

    public LsRequestBuilder ForStore(int storeId)
    {
        LsNodeData storeNode = new()
        {
            type = "stores",
            id = storeId.ToString()
        };
        _relationships.store = new LsRelationNode(storeNode);

        return this;
    }

    public LsReqBody<T>? Build<T>() where T : ILsReqData
    {
        if(_requestDataType == null)
        {
            return null;
        }

        return new LsReqBody<T>(
            _requestType, 
            (T)_requestData!, 
            _relationships);
    }

}
